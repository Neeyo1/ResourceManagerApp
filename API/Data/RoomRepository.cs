using API.DTOs.Room;
using API.Entities;
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

    public async Task<IEnumerable<RoomDto>> GetRoomsAsync()
    {
        return await context.Rooms
            .ProjectTo<RoomDto>(mapper.ConfigurationProvider)
            .ToListAsync();
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
}
