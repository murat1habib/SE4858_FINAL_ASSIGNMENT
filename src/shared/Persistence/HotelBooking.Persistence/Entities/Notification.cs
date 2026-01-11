using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Persistence.Entities;

public sealed class Notification
{
    public string Id { get; set; } = default!;
    public string HotelId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

