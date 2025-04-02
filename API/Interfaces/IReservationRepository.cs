using shared.DTOs.Reservation;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IReservationRepository
{
    void AddRoomReservation(RoomReservation roomReservation);
    void DeleteRoomReservation(RoomReservation roomReservation);
    Task<PagedList<RoomReservationDto>> GetRoomReservationsAsync(RoomReservationParams roomReservationParams);
    Task<RoomReservation?> GetRoomReservationByIdAsync(int roomReservationId);
    Task<bool> IsRoomReserved(int roomId, DateTime start, DateTime end);
    Task<IEnumerable<RoomReservation>> GetRoomReservationsInPeriodAsync(int roomId, DateTime start, DateTime end);
}
