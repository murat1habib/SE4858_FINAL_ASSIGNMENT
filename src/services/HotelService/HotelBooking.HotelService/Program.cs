using HotelBooking.HotelService.Storage;
using HotelBooking.HotelService.Services;
using HotelBooking.Persistence.Db;
using Microsoft.EntityFrameworkCore;
using HotelBooking.HotelService.Data;


var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("DefaultConnection is not set. Use env var ConnectionStrings__DefaultConnection.");

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
builder.Services.AddDbContext<HotelBookingDbContext>(opt => opt.UseSqlServer(cs));

builder.Services.AddHttpClient<NotificationPublisher>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002");
});
builder.Services.AddSingleton<HotelBooking.HotelService.ML.PricingModelService>();
builder.Services.AddDbContext<HotelBookingDbContext>(opt =>
    opt.UseSqlServer(cs));



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
