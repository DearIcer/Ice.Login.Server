using System.Text.Json.Serialization;

namespace Ice.Login.Service.Common.JWT;

public class JwtTokenConfig
{
    [JsonPropertyName("secret")] public string Secret { get; set; } = string.Empty;

    [JsonPropertyName("issuer")] public string Issuer { get; set; } = string.Empty;

    [JsonPropertyName("audience")] public string Audience { get; set; } = string.Empty;
}