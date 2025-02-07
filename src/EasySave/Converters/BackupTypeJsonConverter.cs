using System.Text.Json;
using System.Text.Json.Serialization;
using EasySave.Models;

namespace EasySave.Converters;

public class BackupTypeJsonConverter : JsonConverter<BackupType>
{
    public override BackupType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader);
        doc.RootElement.TryGetProperty("Type", out var typeElement);

        var typeName = typeElement.GetString() ?? string.Empty;

        return typeName switch
        {
            "FullBackup" => JsonSerializer.Deserialize<FullBackup>(doc.RootElement.GetRawText(), options),
            "DifferentialBackup" => JsonSerializer.Deserialize<DifferentialBackup>(doc.RootElement.GetRawText(), options),
            _ => throw new JsonException($"Unknown BackupType: {typeName}")
        };
    }

    public override void Write(Utf8JsonWriter writer, BackupType value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().Name);
        
        foreach (var property in value.GetType().GetProperties())
        {
            var propValue = property.GetValue(value);
            if (propValue == null) continue;
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propValue, options);
        }

        writer.WriteEndObject();
    }
}

