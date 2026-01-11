using HotelBooking.Contracts.Dtos;
using HotelBooking.HotelService.ML;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.HotelService.Controllers;

[ApiController]
[Route("api/v1/pricing")]
public sealed class PricingController : ControllerBase
{
    private readonly PricingModelService _model;

    public PricingController(PricingModelService model)
    {
        _model = model;
    }

    [HttpPost("predict")]
    public ActionResult<PricingPredictResponse> Predict([FromBody] PricingPredictRequest req)
    {
        if (req.EndDate < req.StartDate) return BadRequest("EndDate cannot be earlier than StartDate.");
        if (string.IsNullOrWhiteSpace(req.Destination)) return BadRequest("Destination is required.");
        if (string.IsNullOrWhiteSpace(req.RoomType)) return BadRequest("RoomType is required.");

        var month = req.StartDate.Month; // basit feature
        var predicted = _model.Predict(req.Destination, month, req.RoomType, stars: 0);

        // model float döner, response decimal
        var price = Math.Round((decimal)predicted, 2);
        return Ok(new PricingPredictResponse(price));
    }
}
