using HotelBooking.NotificationService.Workers;
using HotelBooking.NotificationService.Messaging;
using HotelBooking.NotificationService.Storage;
using HotelBooking.Persistence.Db;
using Microsoft.EntityFrameworkCore;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

var sbConn = builder.Configuration["SERVICEBUS_CONNECTION"];
if (string.IsNullOrWhiteSpace(sbConn))
    throw new InvalidOperationException("SERVICEBUS_CONNECTION is not set");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<QueueConsumerWorker>();
builder.Services.AddSingleton<InMemoryReservationQueue>();
builder.Services.AddSingleton<InMemoryNotificationStore>();

builder.Services.AddDbContext<HotelBookingDbContext>(opt =>
{
    var cs =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

    opt.UseSqlServer(cs);
});

builder.Services.AddSingleton(_ => new ServiceBusClient(sbConn));
builder.Services.AddHostedService<QueueConsumerWorker>();

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

app.Run();

