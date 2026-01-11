using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.NotificationService.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        service = "NotificationService",
        status = "ok",
        utc = DateTimeOffset.UtcNow
    });
}

