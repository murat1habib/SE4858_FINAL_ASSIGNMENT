using System.Text.Json;
using Azure.Messaging.ServiceBus;
using HotelBooking.Contracts.Messages;

namespace HotelBooking.HotelService.Services;

public sealed class NotificationPublisher
{
    private readonly ServiceBusSender _sender;

    public NotificationPublisher(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public async Task PublishNewReservationAsync(NewReservationMessage msg, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(msg);
        var sbMsg = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            Subject = "reservation.created"
        };

        await _sender.SendMessageAsync(sbMsg, ct);
    }
}
