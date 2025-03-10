using System.Security.Claims;
using API.DTOs.Account;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AuthController(UserManager<AppUser> userManager, ITokenService tokenService,
    IMapper mapper, IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet("google-login")]
    public ActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleResponse))
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<ActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return BadRequest("Google authentication failed.");
        }

        var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return BadRequest("Email not found");
        }

        var user = await userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == email.ToUpper());
        if (user == null || user.Email == null) // register
        {
            var firstName = claims?.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            if (firstName == null || lastName == null)
            {
                return BadRequest("Failed to register");
            }

            user = new AppUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                EmailConfirmed = true,
                UserName = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user);
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
        else // login
        {
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
    }
}
