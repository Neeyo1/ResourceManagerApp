using API.DTOs.Room;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IRoomRepository
{
    void AddRoom(Room room);
    void DeleteRoom(Room room);
    Task<PagedList<RoomDto>> GetRoomsAsync(RoomParams roomParams);
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task<Room?> GetRoomWithDetailsByIdAsync(int roomId);
    Task<IEnumerable<RoomWithReservationsDto>> GetRoomsWithReservationsByIdAsync(DateTime start, DateTime end);
}
