using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Persistence.Entities;

public sealed class Hotel
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? Description { get; set; }
}

