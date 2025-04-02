using shared.DTOs.Account;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
    IMapper mapper, IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == loginDto.Email.ToUpper());
        if (user == null || user.Email == null)
        {
            return BadRequest("Invalid email or password");
        }
        if (!await userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return BadRequest("Invalid email or password");
        }

        var userToReturn = mapper.Map<UserDto>(user);
        userToReturn.Token = await tokenService.CreateToken(user);

        var refreshToken = tokenService.CreateRefreshToken(user.Email);
        HttpContext.SetRefreshToken(refreshToken.Token);

        if (await unitOfWork.Complete())
        {
            return Ok(userToReturn);
        }

        return BadRequest("Failed to login");
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var existingUser = await userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == registerDto.Email.ToUpper());
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists, please login instead");
        }

        var user = mapper.Map<AppUser>(registerDto);
        user.EmailConfirmed = true;
        user.UserName = Guid.NewGuid().ToString();

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var userToReturn = mapper.Map<UserDto>(user);
        userToReturn.Token = await tokenService.CreateToken(user);

        var refreshToken = tokenService.CreateRefreshToken(user.Email!);
        HttpContext.SetRefreshToken(refreshToken.Token);

        if (await unitOfWork.Complete())
        {
            return Ok(userToReturn);
        }

        return BadRequest("Failed to register");
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<UserDto>> ChangePassword(UserEditPasswordDto userEditPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(User.GetEmail());
        if (user == null || user.Email == null)
        {
            return BadRequest("Could not find user");
        }

        var result = await userManager.ChangePasswordAsync(user, userEditPasswordDto.CurrentPassword, 
            userEditPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        var userToReturn = mapper.Map<UserDto>(user);
        userToReturn.Token = await tokenService.CreateToken(user);

        var refreshToken = tokenService.CreateRefreshToken(user.Email);
        HttpContext.SetRefreshToken(refreshToken.Token);

        if (await unitOfWork.Complete())
        {
            return Ok(userToReturn);
        }

        return BadRequest("Failed to login");
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserDto>> Refresh(TokenDto tokenDto)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(tokenDto.Token);
        if (principal == null) 
        {
            return BadRequest("Invalid token");
        }

        var email = principal.GetEmail();

        var refreshToken = HttpContext.GetRefreshToken();
        if (refreshToken == null)
        {
            return BadRequest("No refresh token was provided");
        }

        var storedRefreshToken = await unitOfWork.TokenRepository.GetRefreshToken(email, refreshToken);
        if (storedRefreshToken == null)
        {
            return BadRequest("Invalid refresh token");
        }

        if (storedRefreshToken.ExpiryDate < DateTime.UtcNow) 
        {
            return BadRequest("Invalid refresh token");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null || user.Email == null)
        {
            return BadRequest("Could not find user");
        }

        unitOfWork.TokenRepository.RemoveRefreshToken(storedRefreshToken);

        var userToReturn = mapper.Map<UserDto>(user);
        userToReturn.Token = await tokenService.CreateToken(user);
            
        var newRefreshToken = tokenService.CreateRefreshToken(email);
        HttpContext.SetRefreshToken(newRefreshToken.Token);

        if (await unitOfWork.Complete())
        {
            return Ok(userToReturn);
        }

        return BadRequest("Failed to refresh token");
    }
}
