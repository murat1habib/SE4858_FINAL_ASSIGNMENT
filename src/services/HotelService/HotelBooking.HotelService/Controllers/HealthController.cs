using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        service = "HotelService",
        status = "ok",
        utc = DateTimeOffset.UtcNow
    });
}

