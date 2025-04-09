using shared.DTOs;
using shared.DTOs.Account;
using shared.DTOs.Reservation;
using shared.DTOs.Room;
using API.Entities;
using AutoMapper;
using shared.Contracts.Room;
using shared.Contracts.Reservation;
using API.Entities.ElasticSearch;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, UserDto>();
        CreateMap<AppUser, MemberDto>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Room, RoomDto>();
        CreateMap<RoomCreateDto, Room>();
        CreateMap<RoomReservation, RoomReservationDto>()
            .ForMember(x => x.ReservedBy, y => y.MapFrom(z => z.User));
        CreateMap<RoomReservationCreateDto, RoomReservation>();
        CreateMap<RoomWithReservations, RoomWithReservationsDto>();
        CreateMap<RoomDto, RoomCreated>();
        CreateMap<RoomCreated, RoomES>();
        CreateMap<RoomReservationDto, ReservationCreated>();
        CreateMap<ReservationCreated, RoomReservationES>();
        
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
