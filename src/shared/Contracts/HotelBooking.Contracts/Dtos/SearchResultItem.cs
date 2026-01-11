using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record SearchResultItem(
    string HotelId,
    string HotelName,
    string Destination,
    double Lat,
    double Lng,
    decimal PricePerNight,
    bool IsDiscounted
);

