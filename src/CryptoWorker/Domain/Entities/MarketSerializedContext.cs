namespace CryptoWorker.Domain.Entities;

[JsonSerializable(typeof(List<Markets>))]
public sealed partial class MarketSerializedContext : JsonSerializerContext;