using API.DTOs;
using API.DTOs.Account;
using API.DTOs.Reservation;
using API.DTOs.Room;
using API.Entities;
using AutoMapper;

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
        
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
