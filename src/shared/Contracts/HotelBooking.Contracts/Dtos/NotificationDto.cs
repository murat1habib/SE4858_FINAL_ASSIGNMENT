using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Dtos;

public sealed record NotificationDto(
    string Id,
    string HotelId,
    string Type,
    string Message,
    DateTimeOffset CreatedAt,
    bool IsRead
);

