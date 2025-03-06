using API.DTOs.Reservation;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ReservationRepository(DataContext context, IMapper mapper) : IReservationRepository
{
    public void AddRoomReservation(RoomReservation roomReservation)
    {
        context.RoomReservations.Add(roomReservation);
    }

    public void DeleteRoomReservation(RoomReservation roomReservation)
    {
        context.RoomReservations.Remove(roomReservation);
    }

    public async Task<IEnumerable<RoomReservationDto>> GetRoomReservationsAsync()
    {
        return await context.RoomReservations
            .ProjectTo<RoomReservationDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<RoomReservation?> GetRoomReservationByIdAsync(int roomReservationId)
    {
        return await context.RoomReservations
            .Include(x => x.ReservedBy)
            .FirstOrDefaultAsync(x => x.Id == roomReservationId);
    }

    public async Task<bool> IsRoomReserved(int roomId, DateTime start, DateTime end)
    {
        return await context.RoomReservations
            .AnyAsync(x => x.RoomId == roomId && start < x.ReservedTo && end > x.ReservedFrom);
    }

    public async Task<IEnumerable<RoomReservation>> GetRoomReservationsInPeriodAsync(int roomId, DateTime start, DateTime end)
    {
        return await context.RoomReservations
            .Where(x => x.RoomId == roomId && start < x.ReservedTo && end > x.ReservedFrom)
            .ToListAsync();
    }
}
