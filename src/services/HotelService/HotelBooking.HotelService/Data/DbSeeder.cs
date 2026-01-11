using HotelBooking.Persistence.Db;
using HotelBooking.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.HotelService.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();

        await db.Database.MigrateAsync(ct);

        if (await db.Hotels.AnyAsync(ct)) return;

        db.Hotels.AddRange(
            new Hotel { Id = "hotel-1", Name = "Hotel Swiss", Destination = "Bodrum", Lat = 37.0340, Lng = 27.4305, Description = "Central Bodrum hotel" },
            new Hotel { Id = "hotel-2", Name = "Hyde Bodrum", Destination = "Bodrum", Lat = 37.0930, Lng = 27.3790, Description = "Beachfront resort" },
            new Hotel { Id = "hotel-3", Name = "City Inn", Destination = "Izmir", Lat = 38.4237, Lng = 27.1428, Description = "Downtown Izmir" }
        );

        await db.SaveChangesAsync(ct);
    }
}

