using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record PricingPredictRequest(
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    string RoomType,
    int RoomCount,
    int People
);

