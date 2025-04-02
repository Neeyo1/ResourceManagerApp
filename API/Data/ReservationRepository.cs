using shared.DTOs.Reservation;
using API.Entities;
using API.Helpers;
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

    public async Task<PagedList<RoomReservationDto>> GetRoomReservationsAsync(RoomReservationParams roomReservationParams)
    {
        var query = context.RoomReservations.AsQueryable();

        if (roomReservationParams.ReservedFrom != null)
        {
            query = query.Where(x => x.ReservedFrom >= roomReservationParams.ReservedFrom);
        }

        if (roomReservationParams.ReservedTo != null)
        {
            query = query.Where(x => x.ReservedTo >= roomReservationParams.ReservedTo);
        }

        if (roomReservationParams.RoomId != null)
        {
            query = query.Where(x => x.RoomId == roomReservationParams.RoomId);
        }

        if (roomReservationParams.UserId != null)
        {
            query = query.Where(x => x.UserId == roomReservationParams.UserId);
        }

        query = roomReservationParams.Status switch
        {
            "completed" => query.Where(x => x.ReservedTo < DateTime.UtcNow),
            "ongoing" => query.Where(x => x.ReservedFrom < DateTime.UtcNow && x.ReservedTo > DateTime.UtcNow),
            "upcoming" => query.Where(x => x.ReservedFrom > DateTime.UtcNow),
            _ => query //"all"
        };

        query = roomReservationParams.OrderBy switch
        {
            "date" => query.OrderBy(x => x.ReservedFrom),
            "date-desc" => query.OrderByDescending(x => x.ReservedFrom),
            _ => query.OrderBy(x => x.ReservedFrom) //"date"
        };

        return await PagedList<RoomReservationDto>.CreateAsync(
            query.ProjectTo<RoomReservationDto>(mapper.ConfigurationProvider), 
            roomReservationParams.PageNumber, roomReservationParams.PageSize);
    }

    public async Task<RoomReservation?> GetRoomReservationByIdAsync(int roomReservationId)
    {
        return await context.RoomReservations
            .Include(x => x.User)
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
