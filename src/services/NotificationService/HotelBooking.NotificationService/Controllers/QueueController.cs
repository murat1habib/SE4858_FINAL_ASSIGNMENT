using HotelBooking.Contracts.Messages;
using HotelBooking.NotificationService.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.NotificationService.Controllers;

[ApiController]
[Route("api/v1/queue")]
public sealed class QueueController : ControllerBase
{
    private readonly InMemoryReservationQueue _queue;

    public QueueController(InMemoryReservationQueue queue)
    {
        _queue = queue;
    }

    [HttpPost("reservations")]
    public IActionResult Enqueue([FromBody] NewReservationMessage msg)
    {
        _queue.Enqueue(msg);
        return Accepted(new { status = "queued" });
    }
}

