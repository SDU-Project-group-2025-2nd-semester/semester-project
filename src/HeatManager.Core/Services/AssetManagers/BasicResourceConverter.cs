using HeatManager.Core.Models.Resources;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Services.AssetManagers;

internal class BasicResourceConverter : JsonConverter<Resource>
{
    private static readonly HashSet<string> ValidResources = ["Gas", "Oil", "Electricity"];

    public override Resource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Invalid format for Resource.");
        }

        var name = reader.GetString();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new JsonException("Resource name cannot be null or empty.");
        }

        if (!ValidResources.Contains(name))
        {
            throw new JsonException($"Invalid resource type: {name}");
        }

        return new Resource { Name = name };

    }

    public override void Write(Utf8JsonWriter writer, Resource value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}