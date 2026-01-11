using HotelBooking.Persistence.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/destinations")]
public sealed class DestinationsController : ControllerBase
{
    private const string CacheKey = "destinations:v1";
    private readonly IMemoryCache _cache;
    private readonly HotelBookingDbContext _db;
    private readonly ILogger<DestinationsController> _logger;

    public DestinationsController(IMemoryCache cache, HotelBookingDbContext db, ILogger<DestinationsController> logger)
    {
        _cache = cache;
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<string>>> Get(CancellationToken ct)
    {
        if (_cache.TryGetValue(CacheKey, out IReadOnlyList<string>? cached) && cached is not null)
        {
            _logger.LogInformation("Destinations cache HIT");
            return Ok(cached);
        }

        _logger.LogInformation("Destinations cache MISS (rebuilding)");

        var destinations = await _db.Hotels.AsNoTracking()
            .Select(h => h.Destination)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        _cache.Set(CacheKey, destinations, TimeSpan.FromMinutes(5));
        return Ok(destinations);
    }
}
