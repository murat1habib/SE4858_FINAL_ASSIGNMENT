using HotelBooking.Contracts.Dtos;
using HotelBooking.NotificationService.Messaging;
using HotelBooking.NotificationService.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotelBooking.NotificationService.Workers;

public sealed class QueueConsumerWorker : BackgroundService
{
    private readonly ILogger<QueueConsumerWorker> _logger;
    private readonly InMemoryReservationQueue _queue;
    private readonly InMemoryNotificationStore _store;

    public QueueConsumerWorker(
        ILogger<QueueConsumerWorker> logger,
        InMemoryReservationQueue queue,
        InMemoryNotificationStore store)
    {
        _logger = logger;
        _queue = queue;
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueueConsumerWorker started (in-memory).");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var msg) && msg is not null)
            {
                var notif = new NotificationDto(
                    Id: Guid.NewGuid().ToString("N"),
                    HotelId: msg.HotelId,
                    Type: "NewReservation",
                    Message: $"New reservation {msg.ReservationId} ({msg.StartDate:yyyy-MM-dd} to {msg.EndDate:yyyy-MM-dd})",
                    CreatedAt: DateTimeOffset.UtcNow,
                    IsRead: false
                );

                _store.Add(notif);
                _logger.LogInformation("Consumed reservation message -> notification created. ReservationId={ReservationId}", msg.ReservationId);
            }
            else
            {
                await Task.Delay(300, stoppingToken);
            }
        }
    }
}
