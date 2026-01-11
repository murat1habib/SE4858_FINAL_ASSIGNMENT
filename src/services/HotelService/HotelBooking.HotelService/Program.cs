using HotelBooking.HotelService.Storage;
using HotelBooking.HotelService.Services;
using HotelBooking.Persistence.Db;
using Microsoft.EntityFrameworkCore;
using HotelBooking.HotelService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InMemoryAvailabilityStore>();
builder.Services.AddSingleton<InMemoryHotelStore>();
builder.Services.AddSingleton<InMemoryReservationStore>();
builder.Services.AddHttpClient<NotificationPublisher>(client =>
{
    // Localde gateway üzerinden gitsin
    client.BaseAddress = new Uri("http://localhost:5080/notify");
});
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<HotelBookingDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("HotelBookingDb");
    opt.UseSqlServer(cs);
});
builder.Services.AddHttpClient<NotificationPublisher>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002");
});
builder.Services.AddSingleton<HotelBooking.HotelService.ML.PricingModelService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await DbSeeder.SeedAsync(app.Services);

app.Run();
