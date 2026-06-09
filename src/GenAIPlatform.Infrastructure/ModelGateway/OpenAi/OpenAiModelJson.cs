using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi;

internal static class OpenAiModelJson
{
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
