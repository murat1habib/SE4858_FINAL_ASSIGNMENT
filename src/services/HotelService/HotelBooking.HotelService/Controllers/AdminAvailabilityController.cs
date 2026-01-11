using HotelBooking.Contracts.Dtos;
using HotelBooking.Persistence.Db;
using HotelBooking.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/admin/availability")]
public sealed class AdminAvailabilityController : ControllerBase
{
    private readonly HotelBookingDbContext _db;

    public AdminAvailabilityController(HotelBookingDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] AvailabilityUpsertRequest req, CancellationToken ct)
    {
        if (req.EndDate < req.StartDate) return BadRequest("EndDate cannot be earlier than StartDate.");

        for (var d = req.StartDate; d <= req.EndDate; d = d.AddDays(1))
        {
            var existing = await _db.Availability
                .FirstOrDefaultAsync(x => x.HotelId == req.HotelId && x.Date == d && x.RoomType == req.RoomType, ct);

            if (existing is null)
            {
                _db.Availability.Add(new RoomAvailability
                {
                    HotelId = req.HotelId,
                    Date = d,
                    RoomType = req.RoomType,
                    AvailableCount = req.AvailableCount,
                    BasePrice = req.BasePrice
                });
            }
            else
            {
                existing.AvailableCount = req.AvailableCount;
                existing.BasePrice = req.BasePrice;
            }
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { status = "ok" });
    }

    [HttpGet("{hotelId}")]
    public async Task<ActionResult<IReadOnlyList<RoomAvailabilityDto>>> GetHotel(
        string hotelId,
        [FromQuery] DateOnly? start,
        [FromQuery] DateOnly? end,
        CancellationToken ct)
    {
        var q = _db.Availability.AsNoTracking().Where(x => x.HotelId == hotelId);

        if (start is not null) q = q.Where(x => x.Date >= start.Value);
        if (end is not null) q = q.Where(x => x.Date <= end.Value);

        var items = await q
            .OrderBy(x => x.Date)
            .ThenBy(x => x.RoomType)
            .Select(x => new RoomAvailabilityDto(x.HotelId, x.Date, x.RoomType, x.AvailableCount, x.BasePrice))
            .ToListAsync(ct);

        return Ok(items);
    }
}
