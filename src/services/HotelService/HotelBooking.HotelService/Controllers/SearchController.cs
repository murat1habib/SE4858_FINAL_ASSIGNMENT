using HotelBooking.Contracts.Dtos;
using HotelBooking.Persistence.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/hotels")]
public sealed class SearchController : ControllerBase
{
    private const string DefaultRoomType = "Standard";
    private readonly HotelBookingDbContext _db;

    public SearchController(HotelBookingDbContext db)
    {
        _db = db;
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<SearchResultItem>>> Search([FromBody] SearchRequest req, CancellationToken ct)
    {
        if (req.EndDate < req.StartDate) return BadRequest("EndDate cannot be earlier than StartDate.");
        if (req.RoomCount <= 0) return BadRequest("RoomCount must be >= 1.");
        if (string.IsNullOrWhiteSpace(req.Destination)) return BadRequest("Destination is required.");

        var page = req.Page <= 0 ? 1 : req.Page;
        var pageSize = req.PageSize is <= 0 or > 100 ? 10 : req.PageSize;

        // login varsa %10 indirim gösteriyoruz
        var isLoggedIn = Request.Headers.ContainsKey("Authorization");
        var isDiscounted = isLoggedIn;

        // 1) destination’a göre oteller
        var hotelsQ = _db.Hotels.AsNoTracking()
            .Where(h => h.Destination == req.Destination);

        // 2) date aralığı inclusive gün sayısı
        var dayCount = (req.EndDate.DayNumber - req.StartDate.DayNumber) + 1;
        if (dayCount <= 0) return BadRequest("Invalid date range.");

        // 3) Her gün yeterli availability olan hotelId’ler
        var eligibleHotelIdsQ =
            _db.Availability.AsNoTracking()
                .Where(a =>
                    a.Date >= req.StartDate && a.Date <= req.EndDate &&
                    a.RoomType == DefaultRoomType &&
                    a.AvailableCount >= req.RoomCount)
                .GroupBy(a => a.HotelId)
                .Where(g => g.Count() == dayCount)
                .Select(g => g.Key);

        // 4) otelleri eligible ile kes
        var filteredQ = hotelsQ.Where(h => eligibleHotelIdsQ.Contains(h.Id));

        var totalCount = await filteredQ.CountAsync(ct);

        var hotels = await filteredQ
            .OrderBy(h => h.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // 5) pricePerNight hesapla:
        // date range içindeki basePrice ortalaması * roomCount (oda başı nightly yerine, senin modelde rooms üzerinden)
        // DTO: PricePerNight olduğu için nights ile çarpmıyoruz.
        var hotelIds = hotels.Select(h => h.Id).ToList();

        var avgNightPrices = await _db.Availability.AsNoTracking()
            .Where(a => hotelIds.Contains(a.HotelId)
                        && a.RoomType == DefaultRoomType
                        && a.Date >= req.StartDate && a.Date <= req.EndDate)
            .GroupBy(a => a.HotelId)
            .Select(g => new { HotelId = g.Key, Avg = g.Average(x => x.BasePrice) })
            .ToListAsync(ct);

        var priceMap = avgNightPrices.ToDictionary(x => x.HotelId, x => x.Avg);

        var items = hotels.Select(h =>
        {
            var avg = priceMap.TryGetValue(h.Id, out var p) ? p : 0m;

            // roomCount etkisi:
            var pricePerNight = avg * req.RoomCount;

            // discount (login varsa %10)
            if (isLoggedIn) pricePerNight = Math.Round(pricePerNight * 0.90m, 2);

            return new SearchResultItem(
                HotelId: h.Id,
                HotelName: h.Name,
                Destination: h.Destination,
                Lat: h.Lat,
                Lng: h.Lng,
                PricePerNight: pricePerNight,
                IsDiscounted: isDiscounted
            );
        }).ToList();

        return Ok(new PagedResult<SearchResultItem>(
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            Items: items
        ));
    }
}
