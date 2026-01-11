using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record ReservationDto(
    string Id,
    string HotelId,
    string UserId,
    DateOnly StartDate,
    DateOnly EndDate,
    string RoomType,
    int RoomCount,
    decimal TotalPrice,
    DateTimeOffset CreatedAt
);

