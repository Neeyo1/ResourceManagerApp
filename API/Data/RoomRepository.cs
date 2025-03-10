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

    public async Task<IEnumerable<RoomWithReservationsDto>> GetRoomsWithReservationsByIdAsync(
        DateTime start, DateTime end)
    {
        var rooms = await context.Rooms
            .Include(x => x.RoomReservations)
            .ThenInclude(x => x.User)
            .ToListAsync();

        return rooms.Select(room =>
        {
            var reservations = room.RoomReservations
                .Where(x => start < x.ReservedTo && end > x.ReservedFrom)
                .ToList();

            var roomStatus = reservations.Count == 0
                ? RoomStatus.Avaiable
                : reservations.Any(x => x.ReservedFrom > start || x.ReservedTo < end)
                    ? RoomStatus.PartiallyAvaiable
                    : RoomStatus.Unavaiable;

            var dto = mapper.Map<RoomWithReservationsDto>(room);
            dto.RoomStatus = roomStatus.ToString();
            dto.RoomReservations = mapper.Map<List<RoomReservationDto>>(reservations);

            return dto;
        }).ToList();
    }
}
