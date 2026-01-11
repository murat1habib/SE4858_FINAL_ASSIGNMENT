using HotelBooking.Contracts.Dtos;
using HotelBooking.Persistence.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/hotels")]
public sealed class HotelController : ControllerBase
{
    private readonly HotelBookingDbContext _db;

    public HotelController(HotelBookingDbContext db)
    {
        _db = db;
    }

    [HttpGet("{hotelId}")]
    public async Task<ActionResult<HotelDto>> GetById([FromRoute] string hotelId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(hotelId)) return BadRequest("hotelId is required.");

        var h = await _db.Hotels.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == hotelId, ct);

        if (h is null) return NotFound();

        return Ok(new HotelDto(
            Id: h.Id,
            Name: h.Name,
            Destination: h.Destination,
            Lat: h.Lat,
            Lng: h.Lng,
            Description: h.Description
        ));
    }
}
