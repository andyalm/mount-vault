using System.Text.Json.Serialization;

namespace MountVault.Secrets;

public record ListResponse
{
    [JsonPropertyName("keys")]
    public string[] Keys { get; set; } = Array.Empty<string>();
}