using System.Collections.Concurrent;
using HotelBooking.Contracts.Messages;

namespace HotelBooking.NotificationService.Messaging;

public sealed class InMemoryReservationQueue
{
    private readonly ConcurrentQueue<NewReservationMessage> _queue = new();

    public void Enqueue(NewReservationMessage msg) => _queue.Enqueue(msg);

    public bool TryDequeue(out NewReservationMessage? msg)
        => _queue.TryDequeue(out msg);
}

