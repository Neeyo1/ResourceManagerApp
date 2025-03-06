using System.Text.Json;
using System.Text.Json.Serialization;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DbInitializer
{
    public static async Task InitDb(DataContext context, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager, ILogger<DbInitializer> logger)
    {
        await SeedData(context, userManager, roleManager, logger);
    }
    private static async Task SeedData(DataContext context, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager, ILogger<DbInitializer> logger)
    {
        logger.LogInformation("------ Seeding process started ------");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        logger.LogInformation("------ Seeding roles started ------");
        if (await context.Roles.AnyAsync())
        {
            logger.LogInformation("------ Seeding roles skipped ------");
        }
        else
        {
            await roleManager.CreateAsync(new AppRole { Name = "User" });
            await roleManager.CreateAsync(new AppRole { Name = "Manager" });
            await roleManager.CreateAsync(new AppRole { Name = "Admin" });
            logger.LogInformation("------ Seeding roles completed ------");
        }

        logger.LogInformation("------ Seeding users started ------");
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("------ Seeding users skipped ------");
        }
        else
        {
            var adminUser = new AppUser
            {
                UserName = "admin",
                FirstName = "Admin",
                LastName = "Admin",
                Email = "TestAdminEmail"
            };
            await userManager.CreateAsync(adminUser, "zaq1@WSX");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("------ Seeding users completed ------");
        }
        
        logger.LogInformation("------ Seeding rooms started ------");
        if (await context.Rooms.AnyAsync())
        {
            logger.LogInformation("------ Seeding rooms skipped ------");
        }
        else
        {
            var roomsData = await File.ReadAllTextAsync("Data/rooms.json");
            var rooms = JsonSerializer.Deserialize<IEnumerable<Room>>(roomsData, options);
            if (rooms == null)
            {
                logger.LogInformation("------ Seeding rooms failed ------");
            }
            else
            {
                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
                logger.LogInformation("------ Seeding rooms completed ------");
            }
            
        }

        logger.LogInformation("------ Seeding room reservations started ------");
        if (await context.RoomReservations.AnyAsync())
        {
            logger.LogInformation("------ Seeding room reservations skipped ------");
        }
        else
        {
            var roomReservationsData = await File.ReadAllTextAsync("Data/roomReservations.json");
            var roomReservations = JsonSerializer.Deserialize<IEnumerable<RoomReservation>>(roomReservationsData, options);
            if (roomReservations == null)
            {
                logger.LogInformation("------ Seeding room reservations failed ------");
            }
            else
            {
                await context.RoomReservations.AddRangeAsync(roomReservations);
                await context.SaveChangesAsync();
                logger.LogInformation("------ Seeding room reservations completed ------");
            }
        }

        logger.LogInformation("------ Seeding process completed ------");
    }
}

