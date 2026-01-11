using Microsoft.ML.Data;

namespace HotelBooking.HotelService.ML;

public sealed class PricingModelInput
{
    public string Destination { get; set; } = "";
    public float Month { get; set; }
    public string RoomType { get; set; } = "";
    public float Stars { get; set; }
    public float PricePerNight { get; set; } // label (predict’te kullanılmıyor ama class’ta durabilir)
}

public sealed class PricingModelOutput
{
    [ColumnName("Score")]
    public float PredictedPricePerNight { get; set; }
}

