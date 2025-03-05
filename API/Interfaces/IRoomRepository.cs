using API.DTOs.Room;
using API.Entities;

namespace API.Interfaces;

public interface IRoomRepository
{
    void AddRoom(Room room);
    void DeleteRoom(Room room);
    Task<IEnumerable<RoomDto>> GetRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task<Room?> GetRoomWithDetailsByIdAsync(int roomId);
}
