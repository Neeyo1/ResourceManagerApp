using API.DTOs;
using API.DTOs.Reservation;
using API.DTOs.Room;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class RoomRepository(DataContext context, IMapper mapper) : IRoomRepository
{
    public void AddRoom(Room room)
    {
        context.Rooms.Add(room);
    }

    public void DeleteRoom(Room room)
    {
        context.Rooms.Remove(room);
    }

    public async Task<PagedList<RoomDto>> GetRoomsAsync(RoomParams roomParams)
    {
        var query = context.Rooms.AsQueryable();

        if (roomParams.Name != null)
        {
            query = query.Where(x => x.Name.Contains(roomParams.Name));
        }

        if (roomParams.MinCapacity != null)
        {
            query = query.Where(x => x.Capacity >= roomParams.MinCapacity);
        }

        if (roomParams.MaxCapacity != null)
        {
            query = query.Where(x => x.Capacity <= roomParams.MaxCapacity);
        }

        query = roomParams.Type switch
        {
            "conference-room" => query.Where(x => x.RoomType == RoomType.ConferenceRoom),
            "interview-room" => query.Where(x => x.RoomType == RoomType.InterviewRoom),
            "meeting-room" => query.Where(x => x.RoomType == RoomType.MeetingRoom),
            "training-room" => query.Where(x => x.RoomType == RoomType.TrainingRoom),
            _ => query //"all"
        };

        query = roomParams.OrderBy switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name-desc" => query.OrderByDescending(x => x.Name),
            "capacity" => query.OrderBy(x => x.Capacity),
            "capacity-desc" => query.OrderByDescending(x => x.Capacity),
            _ => query.OrderBy(x => x.Name) //"name"
        };

        return await PagedList<RoomDto>.CreateAsync(
            query.ProjectTo<RoomDto>(mapper.ConfigurationProvider), 
            roomParams.PageNumber, roomParams.PageSize);
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await context.Rooms
            .FindAsync(roomId);
    }

    public async Task<Room?> GetRoomWithDetailsByIdAsync(int roomId)
    {
        return await context.Rooms
            .Include(x => x.RoomReservations)
            .FirstOrDefaultAsync(x => x.Id == roomId);
    }

    public async Task<PagedList<RoomWithReservationsDto>> GetRoomsStatusAsync(RoomStatusParams roomStatusParams)
    {
        var query = context.Rooms
            .Include(x => x.RoomReservations)
            .ThenInclude(x => x.User)
            .Select(room => new RoomWithReservations
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                RoomType = room.RoomType,
                RoomStatus = room.RoomReservations
                    .Any(x => roomStatusParams.Start < x.ReservedTo && roomStatusParams.End > x.ReservedFrom)
                    ? RoomStatus.Unavaiable
                    : RoomStatus.Avaiable,
                RoomReservations = room.RoomReservations
                    .Where(x => roomStatusParams.Start < x.ReservedTo && roomStatusParams.End > x.ReservedFrom)
                    .Select(x => new RoomReservationDto
                    {
                        Id = x.Id,
                        ReservedFrom = x.ReservedFrom,
                        ReservedTo = x.ReservedTo,
                        ReservedBy = mapper.Map<MemberDto>(x.User)
                    }).ToList()
            });

        if (roomStatusParams.Name != null)
        {
            query = query.Where(x => x.Name.Contains(roomStatusParams.Name));
        }

        if (roomStatusParams.MinCapacity != null)
        {
            query = query.Where(x => x.Capacity >= roomStatusParams.MinCapacity);
        }

        if (roomStatusParams.MaxCapacity != null)
        {
            query = query.Where(x => x.Capacity <= roomStatusParams.MaxCapacity);
        }

        query = roomStatusParams.Type switch
        {
            "conference-room" => query.Where(x => x.RoomType == RoomType.ConferenceRoom),
            "interview-room" => query.Where(x => x.RoomType == RoomType.InterviewRoom),
            "meeting-room" => query.Where(x => x.RoomType == RoomType.MeetingRoom),
            "training-room" => query.Where(x => x.RoomType == RoomType.TrainingRoom),
            _ => query //"all"
        };

        query = roomStatusParams.OrderBy switch
        {
            "name" => query.OrderBy(x => x.Name),
            "name-desc" => query.OrderByDescending(x => x.Name),
            "capacity" => query.OrderBy(x => x.Capacity),
            "capacity-desc" => query.OrderByDescending(x => x.Capacity),
            _ => query.OrderBy(x => x.Name) //"name"
        };

        query = roomStatusParams.Status switch
        {
            "avaiable" => query.Where(x => x.RoomStatus == RoomStatus.Avaiable),
            "unavaiable" => query.Where(x => x.RoomStatus == RoomStatus.Unavaiable),
            _ => query //"all"
        };

        return await PagedList<RoomWithReservationsDto>.CreateAsync(
            query.ProjectTo<RoomWithReservationsDto>(mapper.ConfigurationProvider), 
            roomStatusParams.PageNumber, roomStatusParams.PageSize);
    }
}
