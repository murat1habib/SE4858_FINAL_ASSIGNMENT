using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Persistence.Entities;

public sealed class RoomAvailability
{
    public long Id { get; set; }

    public string HotelId { get; set; } = default!;
    public Hotel? Hotel { get; set; }

    public DateOnly Date { get; set; }
    public string RoomType { get; set; } = default!;
    public int AvailableCount { get; set; }
    public decimal BasePrice { get; set; }
}

