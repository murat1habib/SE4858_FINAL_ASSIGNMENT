using System.Collections.Concurrent;
using HotelBooking.Contracts.Dtos;

namespace HotelBooking.HotelService.Storage;

public sealed class InMemoryReservationStore
{
    private readonly ConcurrentDictionary<string, ReservationDto> _items = new();

    public ReservationDto Add(ReservationDto reservation)
    {
        _items[reservation.Id] = reservation;
        return reservation;
    }

    public ReservationDto? Get(string id)
        => _items.TryGetValue(id, out var r) ? r : null;
}

