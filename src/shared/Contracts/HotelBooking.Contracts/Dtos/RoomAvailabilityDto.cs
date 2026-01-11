using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record RoomAvailabilityDto(
    string HotelId,
    DateOnly Date,
    string RoomType,
    int AvailableCount,
    decimal BasePrice
);

