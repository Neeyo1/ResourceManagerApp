using API.DTOs.Reservation;
using API.Entities;

namespace API.Interfaces;

public interface IReservationRepository
{
    void AddRoomReservation(RoomReservation roomReservation);
    void DeleteRoomReservation(RoomReservation roomReservation);
    Task<IEnumerable<RoomReservationDto>> GetRoomReservationsAsync();
    Task<RoomReservation?> GetRoomReservationByIdAsync(int roomReservationId);
    Task<bool> IsRoomReserved(int roomId, DateTime start, DateTime end);

    //Temp solution
    Task<IEnumerable<RoomReservation>> GetRoomReservationsInPeriodAsync(int roomId, DateTime start, DateTime end);
}
