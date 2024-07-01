namespace CryptoWorker.Domain.Entities;

/// <summary>List all coins with market data.</summary>
public class Markets
{
    [JsonPropertyName("name")]
    public required string DisplayName { get; set; }

    [JsonPropertyName("market_cap_rank")]
    public required int Rank { get; set; }

    [JsonPropertyName("symbol")]
    public required string SymbolName { get; set; }
}
