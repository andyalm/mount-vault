using System.Text.Json.Serialization;

namespace MountVault.Secrets;

public record VaultApiResponse<TData>
{
    [JsonPropertyName("data")]
    public TData Data { get; set; } = default!;
};