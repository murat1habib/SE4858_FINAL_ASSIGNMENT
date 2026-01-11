using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record UpsertAvailabilityRequest(
    string HotelId,
    DateOnly StartDate,
    DateOnly EndDate,
    string RoomType,
    int AvailableCount,
    decimal BasePrice
);

