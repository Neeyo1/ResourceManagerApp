using API.DTOs.Room;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class RoomsController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<RoomDto>>> GetRooms([FromQuery] RoomParams roomParams)
    {
        var rooms = await unitOfWork.RoomRepository.GetRoomsAsync(roomParams);
        Response.AddPaginationHeader(rooms);
        
        return Ok(rooms);
    }

    [Authorize]
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom(RoomCreateDto roomCreateDto)
    {
        var room = mapper.Map<Room>(roomCreateDto);

        unitOfWork.RoomRepository.AddRoom(room);

        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetRoom), new {roomId = room.Id}, mapper.Map<RoomDto>(room));
        }
        return BadRequest("Failed to create room");
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<RoomWithReservationsDto>>> GetRoomsStatus(string dateStart, string dateEnd)
    {
        if (!DateTime.TryParse(dateStart, out var start) || !DateTime.TryParse(dateEnd, out var end))
        {
            return BadRequest("Invalid date format");
        }

        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        Console.WriteLine(start);
        Console.WriteLine(end);

        var roomsStatus = await unitOfWork.RoomRepository.GetRoomsWithReservationsByIdAsync(start, end);
        
        return Ok(roomsStatus);
    }
}
