using System.Collections.ObjectModel;
using System.Text.Json;
using HeatManager.Core.Models.Producers;
using System.Text.Json.Serialization;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Services;

public class AssetManager : IAssetManager
{
    private readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json");
    public ObservableCollection<IHeatProductionUnit> ProductionUnits { get; private set; } = new ObservableCollection<IHeatProductionUnit>();

    public AssetManager()
    {
        LoadUnits(DataFilePath);
    }

    public void LoadUnits(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException($"Data file not found at {filepath}");
        }

        var json = File.ReadAllText(filepath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new BasicResourceConverter() }
        };

        var jsonData = JsonSerializer.Deserialize<JsonDataStructure>(json, options) ?? new JsonDataStructure();

        var allUnits = (jsonData.HeatProductionUnits ?? Enumerable.Empty<IHeatProductionUnit>())
            .Concat(jsonData.ElectricityProductionUnits ?? Enumerable.Empty<IHeatProductionUnit>())
            .ToList();

        ProductionUnits = new ObservableCollection<IHeatProductionUnit>(allUnits);

    }
}

internal class BasicResourceConverter : JsonConverter<IBasicResource>
{
    private static readonly HashSet<string> ValidResources = new() { "Gas", "Oil", "Electricity" };

    public override BasicResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var name = reader.GetString();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new JsonException("BasicResource name cannot be null or empty.");
            }

            if (!ValidResources.Contains(name))
            {
                throw new JsonException($"Invalid resource type: {name}");
            }

            return new BasicResource { Name = name };
        }

        throw new JsonException("Invalid format for BasicResource.");
    }

    public override void Write(Utf8JsonWriter writer, IBasicResource value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}

public class JsonDataStructure
{
    public List<HeatProductionUnit>? HeatProductionUnits { get; set; }
    public List<ElectricityProductionUnit>? ElectricityProductionUnits { get; set; }
}
