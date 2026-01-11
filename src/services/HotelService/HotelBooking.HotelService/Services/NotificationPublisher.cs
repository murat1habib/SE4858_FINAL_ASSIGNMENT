using System.Net.Http.Json;
using HotelBooking.Contracts.Messages;

namespace HotelBooking.HotelService.Services;

public sealed class NotificationPublisher
{
    private readonly HttpClient _http;

    public NotificationPublisher(HttpClient http)
    {
        _http = http;
    }

    public async Task PublishNewReservationAsync(NewReservationMessage msg, CancellationToken ct = default)
    {
        // ✅ NotificationService: POST /api/v1/notifications
        var res = await _http.PostAsJsonAsync("/api/v1/notifications", msg, ct);
        res.EnsureSuccessStatusCode();
    }
}
