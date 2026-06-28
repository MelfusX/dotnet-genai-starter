using System.Text.Json;

namespace GenAIPlatform.Infrastructure.Mcp;

internal static class ExternalMcpJsonRoundTrip
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static IReadOnlyDictionary<string, object?>? ToSdkArguments(JsonElement arguments)
    {
        if (arguments.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Dictionary<string, object?>>(
            arguments.GetRawText(),
            SerializerOptions) ?? new Dictionary<string, object?>();
    }

    public static JsonElement CloneObjectSchema(JsonElement schema)
    {
        if (schema.ValueKind == JsonValueKind.Object)
        {
            return JsonSerializer.Deserialize<JsonElement>(schema.GetRawText(), SerializerOptions).Clone();
        }

        return JsonSerializer.SerializeToElement(new { type = "object" }, SerializerOptions);
    }

    public static JsonElement EmptyObject()
    {
        return JsonSerializer.SerializeToElement(new { }, SerializerOptions);
    }
}