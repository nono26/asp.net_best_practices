using System.Diagnostics;
using BackEnd.Domain;
using BackEnd.Refactoring.Application.Abstraction.Srlztn;
using BackEnd.Refactoring.Infra.Serialization.Legacy;
using BackEnd.Refactoring.Infra.Serialization.Moderm;
using Xunit;

namespace BackEnd.Tests.RollbackTests;

public class RollbackCriteriaTests
{
    protected ISerializer Baseline { get; } = new LegacyNewtonSoftSerializer();
    protected ISerializer Candidate { get; } = new SystemTextJsonSerializer();

    protected Order Order = new Order
    {
        Id = 42,
        Amount = 9999.99m,
        CustomerName = "Large Customer Ltd",
        Note = "Priority account — handle with care",
        Status = OrderStatus.Shipped,
        CreatedAt = DateTimeOffset.UtcNow
    };

    [Fact]
    public void Candidate_Serialization_IsNotMoreThan10PercentSlower()
    {
        var baselineMs = MeasureMs(() => Baseline.Serialize(Order));
        var candidateMs = MeasureMs(() => Candidate.Serialize(Order));
        var ratio = candidateMs / baselineMs;
        Assert.True(ratio <= 1.10, $"ROLLBACK TRIGGER — ratio was {ratio:F3}x");
    }

    private static double MeasureMs(Action action, int warmup = 100, int measured = 2000)
    {
        for (int i = 0; i < warmup; i++) action();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < measured; i++) action();
        return sw.Elapsed.TotalMilliseconds;
    }
}
