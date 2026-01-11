using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record SearchRequest(
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    int People,
    int RoomCount,
    int Page = 1,
    int PageSize = 10
);

