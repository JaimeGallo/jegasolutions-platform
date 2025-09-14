using System.Text.Json;
using System.Text.Json.Serialization;

namespace JEGASolutions.Landing.Infrastructure.Utils;

public static class JsonUtils
{
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}