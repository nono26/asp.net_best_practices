using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BackEnd.Domain;
using BackEnd.Refactoring.Infra.Serialization.Legacy;
using BackEnd.Refactoring.Infra.Serialization.Moderm;

namespace BackEnd.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class SerializerBenchmarks
{
    private readonly LegacyNewtonSoftSerializer _legacy = new();
    private readonly SystemTextJsonSerializer _modern = new();

    private Order _order = null!;
    private string _json = null!;

    [GlobalSetup]
    public void Setup()
    {
        _order = new Order
        {
            Id = 42,
            Amount = 9999.99m,
            CustomerName = "Ada Lovelace",
            Note = "Priority customer — handle with care",
            Status = OrderStatus.Shipped,
            CreatedAt = new DateTimeOffset(2024, 5, 17, 8, 9, 10, TimeSpan.Zero)
        };

        _json = _legacy.Serialize(_order);
    }

    [Benchmark(Baseline = true, Description = "Legacy - Serialize")]
    public string Legacy_Serialize() => _legacy.Serialize(_order);

    [Benchmark(Description = "Modern - Serialize")]
    public string Modern_Serialize() => _modern.Serialize(_order);

    [Benchmark(Description = "Legacy - Deserialize")]
    public Order Legacy_Deserialize() => _legacy.Deserialize<Order>(_json);

    [Benchmark(Description = "Modern - Deserialize")]
    public Order Modern_Deserialize() => _modern.Deserialize<Order>(_json);
}
