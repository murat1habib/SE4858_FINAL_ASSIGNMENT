using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Contracts.Messages;

public sealed record NewReservationMessage(
    string ReservationId,
    string HotelId,
    DateOnly StartDate,
    DateOnly EndDate,
    string UserId,
    decimal TotalPrice,
    DateTimeOffset CreatedAt
);

