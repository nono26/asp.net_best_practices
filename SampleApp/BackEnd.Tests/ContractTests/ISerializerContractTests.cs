using BackEnd.Domain;
using BackEnd.Refactoring.Application.Abstraction.Srlztn;
using BackEnd.Refactoring.Infra.Serialization.Legacy;
using BackEnd.Refactoring.Infra.Serialization.Moderm;

namespace BackEnd.Tests.Characterization;

[TestClass]
public abstract class ISerializerContractTests
{
    protected abstract ISerializer CreateSut();

    [TestMethod]
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

        Assert.IsNotNull(result);
        Assert.AreEqual(original.Id, result.Id);
        Assert.AreEqual(original.Amount, result.Amount);
        Assert.AreEqual(original.CustomerName, result.CustomerName);
        Assert.AreEqual(original.Note, result.Note);
        Assert.AreEqual(original.Status, result.Status);
        Assert.AreEqual(original.CreatedAt, result.CreatedAt);
    }

    [TestMethod]
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

        Assert.IsNotNull(result);
        Assert.AreEqual(7, result.Id);
        Assert.AreEqual(12.50m, result.Amount);
        Assert.AreEqual("José", result.CustomerName);
        Assert.IsNull(result.Note);
        Assert.AreEqual(OrderStatus.Shipped, result.Status);
        Assert.AreEqual(new DateTimeOffset(2024, 5, 17, 8, 9, 10, TimeSpan.Zero), result.CreatedAt);
    }

    [TestMethod]
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

        Assert.IsNotNull(result);
        Assert.AreEqual(original.Id, result.Id);
        Assert.AreEqual(original.Amount, result.Amount);
        Assert.AreEqual(original.CustomerName, result.CustomerName);
        Assert.AreEqual(original.Note, result.Note);
        Assert.AreEqual(original.Status, result.Status);
        Assert.AreEqual(original.CreatedAt, result.CreatedAt);
    }
}

[TestClass]
public class LegacyNewtonSoftSerializerContractTests : ISerializerContractTests
{
    protected override ISerializer CreateSut() => new LegacyNewtonSoftSerializer();
}

[TestClass]
public class SystemTextJsonSerializerContractTests : ISerializerContractTests
{
    protected override ISerializer CreateSut() => new SystemTextJsonSerializer();
}
