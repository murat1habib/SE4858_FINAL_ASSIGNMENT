using System.Collections.Concurrent;
using HotelBooking.Contracts.Dtos;

namespace HotelBooking.HotelService.Storage;

// Key: (HotelId, Date, RoomType)
public sealed class InMemoryAvailabilityStore
{
    private readonly ConcurrentDictionary<string, RoomAvailabilityDto> _items = new();

    private static string Key(string hotelId, DateOnly date, string roomType)
        => $"{hotelId}::{date:yyyy-MM-dd}::{roomType}".ToLowerInvariant();

    // Admin: date range boyunca availability ekle / güncelle
    public void UpsertRange(
        string hotelId,
        DateOnly start,
        DateOnly end,
        string roomType,
        int count,
        decimal basePrice)
    {
        if (end < start)
            throw new ArgumentException("EndDate cannot be earlier than StartDate.");

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var dto = new RoomAvailabilityDto(
                hotelId,
                d,
                roomType,
                count,
                basePrice
            );

            _items[Key(hotelId, d, roomType)] = dto;
        }
    }

    // Admin: hotel bazlı listeleme
    public IReadOnlyList<RoomAvailabilityDto> GetHotel(
        string hotelId,
        DateOnly? start = null,
        DateOnly? end = null)
    {
        var list = _items.Values.Where(x => x.HotelId == hotelId);

        if (start is not null)
            list = list.Where(x => x.Date >= start.Value);

        if (end is not null)
            list = list.Where(x => x.Date <= end.Value);

        return list
            .OrderBy(x => x.Date)
            .ThenBy(x => x.RoomType)
            .ToList();
    }

    // Search & Booking: yeterli kapasite var mı?
    public bool HasAvailability(
        string hotelId,
        DateOnly start,
        DateOnly end,
        string roomType,
        int requiredCount)
    {
        if (end < start)
            return false;

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var key = Key(hotelId, d, roomType);

            if (!_items.TryGetValue(key, out var dto))
                return false;

            if (dto.AvailableCount < requiredCount)
                return false;
        }

        return true;
    }

    // Search & Booking: ortalama gecelik fiyat
    public decimal AverageBasePrice(
        string hotelId,
        DateOnly start,
        DateOnly end,
        string roomType)
    {
        var prices = new List<decimal>();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var key = Key(hotelId, d, roomType);

            if (_items.TryGetValue(key, out var dto))
                prices.Add(dto.BasePrice);
        }

        return prices.Count == 0 ? 0 : prices.Average();
    }

    // Booking: kapasite düşür
    public void DecreaseAvailability(
        string hotelId,
        DateOnly start,
        DateOnly end,
        string roomType,
        int roomCount)
    {
        if (!HasAvailability(hotelId, start, end, roomType, roomCount))
            throw new InvalidOperationException("Not enough availability.");

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var key = Key(hotelId, d, roomType);
            var current = _items[key];

            _items[key] = current with
            {
                AvailableCount = current.AvailableCount - roomCount
            };
        }
    }
}
