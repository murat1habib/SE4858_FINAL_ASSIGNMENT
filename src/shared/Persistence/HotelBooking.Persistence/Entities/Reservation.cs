using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Persistence.Entities;

public sealed class Reservation
{
    public string Id { get; set; } = default!;
    public string HotelId { get; set; } = default!;
    public string UserId { get; set; } = default!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public string RoomType { get; set; } = default!;
    public int RoomCount { get; set; }

    public decimal TotalPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

