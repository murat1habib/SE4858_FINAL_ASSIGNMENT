using System.Collections.Concurrent;
using HotelBooking.Contracts.Dtos;

namespace HotelBooking.NotificationService.Storage;

public sealed class InMemoryNotificationStore
{
    private readonly ConcurrentDictionary<string, NotificationDto> _items = new();

    public NotificationDto Add(NotificationDto dto)
    {
        _items[dto.Id] = dto;
        return dto;
    }

    public IReadOnlyList<NotificationDto> List(string? hotelId = null, int take = 50)
    {
        var q = _items.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(hotelId))
            q = q.Where(x => x.HotelId == hotelId);

        return q.OrderByDescending(x => x.CreatedAt).Take(take).ToList();
    }
}
