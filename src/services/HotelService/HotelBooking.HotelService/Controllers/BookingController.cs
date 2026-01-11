using HotelBooking.Contracts.Dtos;
using HotelBooking.Contracts.Messages;
using HotelBooking.HotelService.Services;
using HotelBooking.Persistence.Db;
using HotelBooking.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/bookings")]
public sealed class BookingController : ControllerBase
{
    private readonly HotelBookingDbContext _db;
    private readonly NotificationPublisher _publisher;

    public BookingController(HotelBookingDbContext db, NotificationPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResult>> Book([FromBody] BookingRequest req, CancellationToken ct)
    {
        // ---- validation
        if (req.EndDate < req.StartDate) return BadRequest("EndDate cannot be earlier than StartDate.");
        if (req.RoomCount <= 0) return BadRequest("RoomCount must be >= 1.");
        if (string.IsNullOrWhiteSpace(req.HotelId)) return BadRequest("HotelId is required.");
        if (string.IsNullOrWhiteSpace(req.RoomType)) return BadRequest("RoomType is required.");

        // Auth henüz yok; şimdilik header’dan user alalım
        var userId = Request.Headers.TryGetValue("X-User-Id", out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : "anonymous";

        // gece sayısı
        var nights = req.EndDate.DayNumber - req.StartDate.DayNumber;
        if (nights <= 0) nights = 1;

        // login discount (Authorization varsa %10)
        var isLoggedIn = Request.Headers.ContainsKey("Authorization");

        // ---- transaction: availability düş + reservation insert atomik olsun
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // 1) date range için availability kayıtlarını çek
        var start = req.StartDate;
        var end = req.EndDate;

        var availRows = await _db.Availability
            .Where(x => x.HotelId == req.HotelId
                        && x.RoomType == req.RoomType
                        && x.Date >= start
                        && x.Date <= end)
            .ToListAsync(ct);

        // 2) her gün için kayıt var mı ve yeterli mi?
        // Eksik gün varsa -> yok say (müsait değil)
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var row = availRows.FirstOrDefault(x => x.Date == d);
            if (row is null) return Conflict("Not enough availability for selected dates.");
            if (row.AvailableCount < req.RoomCount) return Conflict("Not enough availability for selected dates.");
        }

        // 3) fiyat: basePrice ortalaması * nights * roomCount
        var avgBase = availRows.Average(x => x.BasePrice);
        var total = avgBase * nights * req.RoomCount;
        if (isLoggedIn) total = Math.Round(total * 0.90m, 2);

        // 4) kapasite düş
        foreach (var row in availRows)
        {
            row.AvailableCount -= req.RoomCount;
        }

        // 5) reservation ekle
        var reservation = new Reservation
        {
            Id = Guid.NewGuid().ToString("N"),
            HotelId = req.HotelId,
            UserId = userId,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            RoomType = req.RoomType,
            RoomCount = req.RoomCount,
            TotalPrice = total,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Reservations.Add(reservation);

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        // 6) transaction sonrası publish (DB commit oldu)
        // notification down olursa booking’i bozmak istemezsen try/catch ekleyebilirsin.
        await _publisher.PublishNewReservationAsync(new NewReservationMessage(
            ReservationId: reservation.Id,
            HotelId: reservation.HotelId,
            StartDate: reservation.StartDate,
            EndDate: reservation.EndDate,
            UserId: reservation.UserId,
            TotalPrice: reservation.TotalPrice,
            CreatedAt: reservation.CreatedAt
        ), ct);

        return Ok(new BookingResult(reservation.Id, reservation.TotalPrice));
    }
}
