using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record HotelDto(
    string Id,
    string Name,
    string Destination,
    double Lat,
    double Lng,
    string? Description
);

