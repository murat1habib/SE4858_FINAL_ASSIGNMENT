using Azure.Messaging.ServiceBus;
using HotelBooking.HotelService.Services;
using HotelBooking.HotelService.Storage;
using HotelBooking.Persistence.Db;
using HotelBooking.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Console logs (Azure Log Stream'de görünür)
builder.Logging.AddConsole();

// --- Service Bus settings ---
var sbConn = builder.Configuration["SERVICEBUS_CONNECTION"];
var sbQueue = builder.Configuration["SERVICEBUS_QUEUE_NAME"] ?? "reservation-created";

if (string.IsNullOrWhiteSpace(sbConn))
    throw new InvalidOperationException("SERVICEBUS_CONNECTION is not set");

// --- Connection String ---
var cs =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException(
        "DefaultConnection is not set. Use env var ConnectionStrings__DefaultConnection."
    );

// --- Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<HotelBookingDbContext>(opt => opt.UseSqlServer(cs));

builder.Services.AddSingleton<InMemoryAvailabilityStore>();
builder.Services.AddSingleton<InMemoryHotelStore>();
builder.Services.AddSingleton<InMemoryReservationStore>();

builder.Services.AddSingleton<HotelBooking.HotelService.ML.PricingModelService>();

// ✅ Service Bus DI
builder.Services.AddSingleton(_ => new ServiceBusClient(sbConn));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(sbQueue);
});

// ✅ Publisher DI (HTTP değil, SB kullanacak)
builder.Services.AddScoped<NotificationPublisher>();

var app = builder.Build();

// --- Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ✅ Migrate + Seed (tek yerde kalsın)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
    await db.Database.MigrateAsync();

    // Eğer hiç hotel yoksa seed
    if (!db.Hotels.Any())
    {
        db.Hotels.AddRange(
            new Hotel { Id = "hotel-1", Name = "Hotel Swiss", Destination = "Bodrum", Lat = 37.034, Lng = 27.430, Description = "Central Bodrum hotel" },
            new Hotel { Id = "hotel-2", Name = "Marina Stay", Destination = "Izmir", Lat = 38.423, Lng = 27.142, Description = "Alsancak seaside" }
        );
        await db.SaveChangesAsync();
    }

    // Availability seed (örnek: Standard oda)
    var start = new DateOnly(2026, 3, 10);
    var end = new DateOnly(2026, 3, 15);

    for (var d = start; d <= end; d = d.AddDays(1))
    {
        if (!db.Availability.Any(a => a.HotelId == "hotel-1" && a.Date == d && a.RoomType == "Standard"))
            db.Availability.Add(new RoomAvailability { HotelId = "hotel-1", Date = d, RoomType = "Standard", AvailableCount = 10, BasePrice = 100 });

        if (!db.Availability.Any(a => a.HotelId == "hotel-2" && a.Date == d && a.RoomType == "Standard"))
            db.Availability.Add(new RoomAvailability { HotelId = "hotel-2", Date = d, RoomType = "Standard", AvailableCount = 10, BasePrice = 120 });
    }

    await db.SaveChangesAsync();
}

app.Run();
