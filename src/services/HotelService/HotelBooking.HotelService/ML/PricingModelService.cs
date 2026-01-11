using Microsoft.ML;

namespace HotelBooking.HotelService.ML;

public sealed class PricingModelService
{
    private readonly PredictionEngine<PricingModelInput, PricingModelOutput> _engine;

    public PricingModelService(IWebHostEnvironment env)
    {
        var ml = new MLContext(seed: 42);

        var modelPath = Path.Combine(env.ContentRootPath, "ML", "model.zip");
        if (!File.Exists(modelPath))
            throw new FileNotFoundException($"ML model not found at: {modelPath}");

        using var fs = File.OpenRead(modelPath);
        var model = ml.Model.Load(fs, out _);

        _engine = ml.Model.CreatePredictionEngine<PricingModelInput, PricingModelOutput>(model);
    }

    public float Predict(string destination, int month, string roomType, float stars = 0)
    {
        var input = new PricingModelInput
        {
            Destination = destination,
            Month = month,
            RoomType = roomType,
            Stars = stars
        };

        var output = _engine.Predict(input);
        return output.PredictedPricePerNight;
    }
}

