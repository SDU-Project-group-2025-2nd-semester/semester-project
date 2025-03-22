using System.Collections.ObjectModel;
using System.Text.Json;
using HeatManager.Core.Models.Producers;
using System.Text.Json.Serialization;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.ViewModels;

public class AssetManagerViewModel : IAssetManagerViewModel
{
    private string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json");
    public ObservableCollection<IHeatProductionUnit> ProductionUnits { get; private set; } = new ObservableCollection<IHeatProductionUnit>();

    public AssetManagerViewModel()
    {
        LoadUnits(DataFilePath);
    }

    public void LoadUnits(string filepath)
    {
        try
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine($"Error: Data file not found at {filepath}");
                return;
            }

            var json = File.ReadAllText(filepath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new BasicResourceConverter() } // Add the converter here
            };

            ProductionUnits.Clear();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine("Error: JSON root is not an array.");
                    return;
                }

                foreach (var element in root.EnumerateArray())
                {
                    // Check if the element is a Electricity- or HeatProductionUnit
                    if (element.TryGetProperty("MaxElectricity", out JsonElement typeElement))
                    {
                        try
                        {
                            var unit = JsonSerializer.Deserialize<ElectricityProductionUnit>(element.GetRawText(), options);
                            if (unit != null)
                            {
                                ProductionUnits.Add(unit);
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"Error deserializing element: {ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            var unit = JsonSerializer.Deserialize<HeatProductionUnit>(element.GetRawText(), options);
                            if (unit != null)
                            {
                                ProductionUnits.Add(unit);
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"Error deserializing element: {ex.Message}");
                        }
                    }

                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex}");
        }
    }

}


internal class BasicResourceConverter : JsonConverter<IBasicResource>
{
    public override BasicResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var name = reader.GetString();
            if (name == null)
            {
                throw new JsonException("BasicResource name cannot be null.");
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
