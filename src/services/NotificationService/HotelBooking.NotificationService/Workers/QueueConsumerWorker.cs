using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HotelBooking.Contracts.Messages;
using HotelBooking.Contracts.Dtos;
using HotelBooking.NotificationService.Storage;

namespace HotelBooking.NotificationService.Workers;

public sealed class QueueConsumerWorker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly InMemoryNotificationStore _store;
    private readonly ILogger<QueueConsumerWorker> _logger;

    public QueueConsumerWorker(
        ServiceBusClient client,
        IConfiguration cfg,
        InMemoryNotificationStore store,
        ILogger<QueueConsumerWorker> logger)
    {
        _store = store;
        _logger = logger;

        var queueName = cfg["SERVICEBUS_QUEUE_NAME"] ?? "reservation-created";
        _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        _processor.ProcessMessageAsync += OnMessage;
        _processor.ProcessErrorAsync += OnError;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }

    private async Task OnMessage(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var msg = JsonSerializer.Deserialize<NewReservationMessage>(body);

            if (msg != null)
            {
                _store.Add(new NotificationDto(
                    Guid.NewGuid().ToString(),
                    msg.HotelId,
                    "reservation.created",
                    $"New reservation: {msg.HotelId} {msg.StartDate}-{msg.EndDate}",
                    DateTimeOffset.UtcNow,
                    false
                ));
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process SB message");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task OnError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "ServiceBus error: {EntityPath}", args.EntityPath);
        return Task.CompletedTask;
    }
}
