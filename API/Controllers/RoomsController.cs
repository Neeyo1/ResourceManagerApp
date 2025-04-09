using shared.DTOs.Room;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using shared.Contracts.Room;

namespace API.Controllers;

[Authorize]
public class RoomsController(IUnitOfWork unitOfWork, IMapper mapper,
    IPublishEndpoint publishEndpoint) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<RoomDto>>> GetRooms([FromQuery] RoomParams roomParams)
    {
        var rooms = await unitOfWork.RoomRepository.GetRoomsAsync(roomParams);
        Response.AddPaginationHeader(rooms);
        
        return Ok(rooms);
    }

    [HttpGet("{roomId}")]
    public async Task<ActionResult<RoomDto>> GetRoom(int roomId)
    {
        var room = await unitOfWork.RoomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<RoomDto>(room));
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom(RoomCreateDto roomCreateDto)
    {
        var room = mapper.Map<Room>(roomCreateDto);

        unitOfWork.RoomRepository.AddRoom(room);
        
        var roomDto = mapper.Map<RoomDto>(room);
        await publishEndpoint.Publish(mapper.Map<RoomCreated>(roomDto));

        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetRoom), new {roomId = room.Id}, roomDto);
        }
        return BadRequest("Failed to create room");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("{roomId}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(RoomUpdateDto roomUpdateDto, int roomId)
    {
        var room = await unitOfWork.RoomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            return BadRequest("Failed to find room");
        }

        room.Name = roomUpdateDto.Name ?? room.Name;
        room.Capacity = roomUpdateDto.Capacity ?? room.Capacity;
        room.RoomType = Enum.TryParse(roomUpdateDto.RoomType, out RoomType result) ? result : room.RoomType;

        if (await unitOfWork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to update room");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("{roomId}")]
    public async Task<ActionResult> DeleteRoom(int roomId)
    {
        var room = await unitOfWork.RoomRepository.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            return BadRequest("Failed to find room");
        }
        
        unitOfWork.RoomRepository.DeleteRoom(room);

        if (await unitOfWork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to delete room");
    }

    [HttpGet("status")]
    public async Task<ActionResult<PagedList<RoomWithReservationsDto>>> GetRoomsStatus(
        [FromQuery] RoomStatusParams roomStatusParams)
    {
        var roomsStatus = await unitOfWork.RoomRepository.GetRoomsStatusAsync(roomStatusParams);
        Response.AddPaginationHeader(roomsStatus);
        
        return Ok(roomsStatus);
    }
}
