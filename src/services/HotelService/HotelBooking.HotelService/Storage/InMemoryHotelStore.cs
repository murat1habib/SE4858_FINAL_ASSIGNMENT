using HotelBooking.Contracts.Dtos;

namespace HotelBooking.HotelService.Storage;

public sealed class InMemoryHotelStore
{
    // Demo data (sonra DB’ye taşınacak)
    private static readonly List<HotelDto> Hotels =
    [
        new("hotel-1", "Hotel Swiss", "Bodrum", 37.0340, 27.4305, "Central Bodrum hotel"),
        new("hotel-2", "Hyde Bodrum", "Bodrum", 37.0930, 27.3790, "Beachfront resort"),
        new("hotel-3", "City Inn", "Izmir", 38.4237, 27.1428, "Downtown Izmir")
    ];

    public IReadOnlyList<HotelDto> GetAll() => Hotels;
}

