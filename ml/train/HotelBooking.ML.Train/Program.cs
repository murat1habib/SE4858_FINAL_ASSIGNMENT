using Microsoft.ML;
using Microsoft.ML.Data;
using System.Globalization;
using System.Text;

internal static class Program
{
    // Prepared training schema
    public sealed class ModelInput
    {
        [LoadColumn(0)] public string Destination { get; set; } = "";
        [LoadColumn(1)] public float Month { get; set; }
        [LoadColumn(2)] public string RoomType { get; set; } = "";
        [LoadColumn(3)] public float Stars { get; set; }

        [LoadColumn(4), ColumnName("Label")]
        public float PricePerNight { get; set; }
    }

    static int Main()
    {
        // baseDir => ...\ml\train
        var baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var dataDir = Path.Combine(baseDir, "data");
        var outDir = Path.Combine(baseDir, "output");

        Directory.CreateDirectory(outDir);

        var preparedPath = Path.Combine(dataDir, "prepared.csv");
        var modelPath = Path.Combine(outDir, "model.zip");

        var inputs = Directory.GetFiles(dataDir, "booking_*.csv");
        if (inputs.Length == 0)
        {
            Console.WriteLine($"No booking_*.csv found in: {dataDir}");
            return 1;
        }

        Console.WriteLine($"DataDir: {dataDir}");
        Console.WriteLine("Found CSV files:");
        foreach (var f in inputs) Console.WriteLine(" - " + Path.GetFileName(f));

        // 1) Prepare merged dataset
        PrepareMerged(inputs, preparedPath);
        Console.WriteLine($"Prepared CSV created: {preparedPath}");

        // 2) Train
        var ml = new MLContext(seed: 42);

        var data = ml.Data.LoadFromTextFile<ModelInput>(
            path: preparedPath,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true,
            trimWhitespace: true);

        var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline =
            ml.Transforms.Categorical.OneHotEncoding("DestEnc", nameof(ModelInput.Destination))
            .Append(ml.Transforms.Categorical.OneHotEncoding("RoomEnc", nameof(ModelInput.RoomType)))
            .Append(ml.Transforms.Concatenate("Features",
                "DestEnc",
                nameof(ModelInput.Month),
                "RoomEnc",
                nameof(ModelInput.Stars)))
            .Append(ml.Regression.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

        var model = pipeline.Fit(split.TrainSet);

        var preds = model.Transform(split.TestSet);
        var metrics = ml.Regression.Evaluate(preds, labelColumnName: "Label");

        Console.WriteLine($"R^2:  {metrics.RSquared:0.###}");
        Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:0.###}");

        ml.Model.Save(model, split.TrainSet.Schema, modelPath);
        Console.WriteLine($"Saved model: {modelPath}");

        return 0;
    }

    private static void PrepareMerged(string[] inputFiles, string preparedPath)
    {
        // Your CSV header is: Hotels, Prices, Descriptions, Full_Descriptions
        // We'll use:
        // - Destination: from file name (bcn/mar)
        // - Month: default (or guessed) because CSV has no date column
        // - RoomType: default "Standard"
        // - Stars: default 0
        // - PricePerNight: from Prices column

        using var sw = new StreamWriter(preparedPath, false, new UTF8Encoding(false));
        sw.WriteLine("Destination,Month,RoomType,Stars,PricePerNight");

        foreach (var file in inputFiles)
        {
            var destination = GuessDestinationFromFileName(file);
            var month = 6;              // dataset doesn't have date; default month
            var roomType = "Standard";  // not provided
            var stars = 0f;             // not provided

            var lines = File.ReadLines(file).ToList();
            if (lines.Count < 2) continue;

            // Map columns by header (safe)
            var header = SplitCsvLine(lines[0])
                .Select(x => x.Trim().Trim('"'))
                .ToList();

            int hotelsIdx = header.FindIndex(h => h.Equals("Hotels", StringComparison.OrdinalIgnoreCase));
            int pricesIdx = header.FindIndex(h => h.Equals("Prices", StringComparison.OrdinalIgnoreCase));

            if (pricesIdx < 0)
                throw new InvalidOperationException($"Prices column not found in {Path.GetFileName(file)}");

            for (int i = 1; i < lines.Count; i++)
            {
                var row = SplitCsvLine(lines[i]);
                if (row.Count <= pricesIdx) continue;

                var priceRaw = row[pricesIdx];
                var price = ParseFloat(priceRaw);
                if (price <= 0) continue;

                // hotel name not used in training, but we keep data generic
                sw.WriteLine($"{Q(destination)},{month},{Q(roomType)},{stars.ToString(CultureInfo.InvariantCulture)},{price.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }

    private static string GuessDestinationFromFileName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
        if (name.Contains("bcn")) return "Barcelona";
        if (name.Contains("mar")) return "Marseille"; // if it's Madrid for you, change to "Madrid"
        return "Unknown";
    }

    private static float ParseFloat(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return 0;

        s = s.Replace("€", "")
             .Replace("EUR", "", StringComparison.OrdinalIgnoreCase)
             .Trim();

        if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
        if (float.TryParse(s, NumberStyles.Any, new CultureInfo("fr-FR"), out v)) return v;
        if (float.TryParse(s, NumberStyles.Any, new CultureInfo("tr-TR"), out v)) return v;

        var cleaned = new string(s.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
        if (float.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out v)) return v;
        if (float.TryParse(cleaned, NumberStyles.Any, new CultureInfo("fr-FR"), out v)) return v;

        return 0;
    }

    private static string Q(string s)
    {
        s = s.Replace("\"", "\"\"");
        return $"\"{s}\"";
    }

    // Minimal CSV splitter (supports quotes)
    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(ch);
            }
        }

        result.Add(sb.ToString());
        return result;
    }
}
