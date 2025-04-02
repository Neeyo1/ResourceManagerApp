using shared.DTOs.Reservation;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ReservationsController(IUnitOfWork unitOfWork, IMapper mapper,
    UserManager<AppUser> userManager) : BaseApiController
{
    [HttpGet("rooms")]
    public async Task<ActionResult<PagedList<RoomReservationDto>>> GetRoomReservations(
        [FromQuery] RoomReservationParams roomReservationParams)
    {
        var roomReservations = await unitOfWork.ReservationRepository.GetRoomReservationsAsync(roomReservationParams);
        Response.AddPaginationHeader(roomReservations);

        return Ok(roomReservations);
    }

    [HttpGet("rooms/{roomReservationId}")]
    public async Task<ActionResult<RoomReservationDto>> GetRoomReservation(int roomReservationId)
    {
        var roomReservation = await unitOfWork.ReservationRepository.GetRoomReservationByIdAsync(roomReservationId);
        if (roomReservation == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<RoomReservationDto>(roomReservation));
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<RoomReservationDto>> CreateRoomReservation(RoomReservationCreateDto roomReservationCreateDto)
    {
        var user = await userManager.FindByEmailAsync(User.GetEmail());
        if (user == null)
        {
            return BadRequest("Failed to find user");
        }

        var room = await unitOfWork.RoomRepository.GetRoomByIdAsync(roomReservationCreateDto.RoomId);
        if (room == null)
        {
            return BadRequest("Failed to find room of given id");
        }

        var isRoomReserved = await unitOfWork.ReservationRepository.IsRoomReserved(room.Id,
            roomReservationCreateDto.ReservedFrom, roomReservationCreateDto.ReservedTo);
        if (isRoomReserved)
        {
            return BadRequest("This reservation collides with another one");
        }

        var roomReservation = mapper.Map<RoomReservation>(roomReservationCreateDto);
        roomReservation.User = user;

        unitOfWork.ReservationRepository.AddRoomReservation(roomReservation);

        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetRoomReservation), new {roomReservationId = roomReservation.Id}, mapper.Map<RoomReservationDto>(roomReservation));
        }
        return BadRequest("Failed to create roomReservation");
    }

    [HttpPut("rooms/{roomReservationId}")]
    public async Task<ActionResult<RoomReservationDto>> UpdateRoomReservation(RoomReservationUpdateDto roomReservationUpdateDto, int roomReservationId)
    {
        var user = await userManager.FindByEmailAsync(User.GetEmail());
        if (user == null)
        {
            return BadRequest("Failed to find user");
        }

        var roomReservation = await unitOfWork.ReservationRepository.GetRoomReservationByIdAsync(roomReservationId);
        if (roomReservation == null)
        {
            return BadRequest("Failed to find roomReservation");
        }
        if (roomReservation.User != user)
        {
            return Unauthorized();
        }

        var reservedFrom = roomReservationUpdateDto.ReservedFrom ?? roomReservation.ReservedFrom;
        var reservedTo = roomReservationUpdateDto.ReservedTo ?? roomReservation.ReservedTo;

        var roomReservations = await unitOfWork.ReservationRepository.GetRoomReservationsInPeriodAsync(
            roomReservation.RoomId, reservedFrom, reservedTo);
        if (roomReservations.Count() > 1)
        {
            return BadRequest("This reservation collides with another one");
        }
        else if(roomReservations.Count() == 1 && roomReservations.First().User != user)
        {
            return BadRequest("This reservation collides with another one");
        }

        roomReservation.ReservedFrom = reservedFrom;
        roomReservation.ReservedTo = reservedTo;

        if (await unitOfWork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to update roomReservation");
    }

    [HttpDelete("rooms/{roomReservationId}")]
    public async Task<ActionResult> DeleteRoomReservation(int roomReservationId)
    {
        var user = await userManager.FindByEmailAsync(User.GetEmail());
        if (user == null)
        {
            return BadRequest("Failed to find user");
        }

        var roomReservation = await unitOfWork.ReservationRepository.GetRoomReservationByIdAsync(roomReservationId);
        if (roomReservation == null)
        {
            return BadRequest("Failed to find roomReservation");
        }
        if (roomReservation.User != user)
        {
            return Unauthorized();
        }
        
        unitOfWork.ReservationRepository.DeleteRoomReservation(roomReservation);

        if (await unitOfWork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to delete roomReservation");
    }
}
