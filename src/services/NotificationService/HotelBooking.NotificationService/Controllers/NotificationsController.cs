using HotelBooking.Contracts.Dtos;
using HotelBooking.Contracts.Messages;
using HotelBooking.Persistence.Db;
using HotelBooking.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.NotificationService.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly HotelBookingDbContext _db;

    public NotificationsController(HotelBookingDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List(
        [FromQuery] string? hotelId,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        var q = _db.Notifications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(hotelId))
            q = q.Where(x => x.HotelId == hotelId);

        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new NotificationDto(
                x.Id,
                x.HotelId,
                x.Type,
                x.Message,
                x.CreatedAt,
                x.IsRead
            ))
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] NewReservationMessage msg, CancellationToken ct)
    {
        var entity = new Notification
        {
            Id = Guid.NewGuid().ToString("N"),
            HotelId = msg.HotelId,
            Type = "NewReservation",
            Message = $"New reservation {msg.ReservationId} ({msg.StartDate:yyyy-MM-dd} to {msg.EndDate:yyyy-MM-dd})",
            CreatedAt = msg.CreatedAt,
            IsRead = false
        };

        _db.Notifications.Add(entity);
        await _db.SaveChangesAsync(ct);

        var dto = new NotificationDto(
            entity.Id, entity.HotelId, entity.Type, entity.Message, entity.CreatedAt, entity.IsRead
        );

        return CreatedAtAction(nameof(List), new { hotelId = dto.HotelId }, dto);
    }
}
