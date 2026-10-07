using BackEnd.Domain;
using BackEnd.Refactoring.Application.Abstraction.Srlztn;
using BackEnd.Refactoring.Infra.Serialization.Legacy;
using BackEnd.Refactoring.Infra.Serialization.Moderm;
using Xunit;

namespace BackEnd.Tests.ContractTests;

public abstract class ISerializerContractTests
{
    protected abstract ISerializer CreateSut();

    [Fact]
    public void Serialize_ThenDeserialize_RoundTripsOrder()
    {
        var sut = CreateSut();
        var original = new Order
        {
            Id = 42,
            Amount = 99.99m,
            CustomerName = "Ada",
            Note = "Priority customer",
            Status = OrderStatus.Confirmed,
            CreatedAt = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero)
        };

        var json = sut.Serialize(original);
        var result = sut.Deserialize<Order>(json);

        Assert.NotNull(result);
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Amount, result.Amount);
        Assert.Equal(original.CustomerName, result.CustomerName);
        Assert.Equal(original.Note, result.Note);
        Assert.Equal(original.Status, result.Status);
        Assert.Equal(original.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public void Deserialize_JsonWithEnumDateAndDecimal_MapsValues()
    {
        var sut = CreateSut();
        const string json = """
        {
          "Id": 7,
          "Amount": 12.50,
          "CustomerName": "José",
          "Note": null,
          "Status": "Shipped",
          "CreatedAt": "2024-05-17T08:09:10+00:00"
        }
        """;

        var result = sut.Deserialize<Order>(json);

        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal(12.50m, result.Amount);
        Assert.Equal("José", result.CustomerName);
        Assert.Null(result.Note);
        Assert.Equal(OrderStatus.Shipped, result.Status);
        Assert.Equal(new DateTimeOffset(2024, 5, 17, 8, 9, 10, TimeSpan.Zero), result.CreatedAt);
    }

    [Fact]
    public void Serialize_AndDeserialize_WithUnicodeAndEmptyString_PreservesData()
    {
        var sut = CreateSut();
        var original = new Order
        {
            Id = 0,
            Amount = 0m,
            CustomerName = "François ☕",
            Note = string.Empty,
            Status = OrderStatus.Cancelled,
            CreatedAt = DateTimeOffset.UnixEpoch
        };

        var json = sut.Serialize(original);
        var result = sut.Deserialize<Order>(json);

        Assert.NotNull(result);
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Amount, result.Amount);
        Assert.Equal(original.CustomerName, result.CustomerName);
        Assert.Equal(original.Note, result.Note);
        Assert.Equal(original.Status, result.Status);
        Assert.Equal(original.CreatedAt, result.CreatedAt);
    }
}

public class LegacyNewtonSoftSerializerContractTests : ISerializerContractTests
{
    protected override ISerializer CreateSut() => new LegacyNewtonSoftSerializer();
}

public class SystemTextJsonSerializerContractTests : ISerializerContractTests
{
    protected override ISerializer CreateSut() => new SystemTextJsonSerializer();
}
