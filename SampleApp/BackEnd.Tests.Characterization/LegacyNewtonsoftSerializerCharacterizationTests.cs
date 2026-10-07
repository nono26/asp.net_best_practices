using BackEnd.Domain;
using BackEnd.Refactoring.Infra.Serialization.Legacy;

namespace BackEnd.Tests.Characterization;

[TestClass]
public class LegacyNewtonsoftSerializerCharacterizationTests
{
    private static readonly LegacyNewtonSoftSerializer Serializer = new();

    [TestMethod]
    public void Serialize_UsesLegacyFormattingAndStringEnumNames()
    {
        var order = new Order
        {
            Id = 42,
            Amount = 99.99m,
            CustomerName = "Ada",
            Note = null,
            Status = OrderStatus.Pending,
            CreatedAt = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero)
        };

        var json = Serializer.Serialize(order);

        var expected = """
        {
          "Id": 42,
          "Amount": 99.99,
          "CustomerName": "Ada",
          "Note": null,
          "Status": "Pending",
          "CreatedAt": "2024-01-02T03:04:05+00:00"
        }
        """;

        Assert.AreEqual(expected, json);
    }

    [TestMethod]
    public void Deserialize_MapsLegacyJsonIntoAnOrder()
    {
        const string json = """
        {
          "Id": 42,
          "Amount": 99.99,
          "CustomerName": "Ada",
          "Note": null,
          "Status": "Pending",
          "CreatedAt": "2024-01-02T03:04:05+00:00"
        }
        """;

        var order = Serializer.Deserialize<Order>(json);

        Assert.IsNotNull(order);
        Assert.AreEqual(42, order.Id);
        Assert.AreEqual(99.99m, order.Amount);
        Assert.AreEqual("Ada", order.CustomerName);
        Assert.IsNull(order.Note);
        Assert.AreEqual(OrderStatus.Pending, order.Status);
        Assert.AreEqual(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero), order.CreatedAt);
    }

    [TestMethod]
    public void Deserialize_WhenJsonIsNull_ThrowsInvalidOperationException()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => Serializer.Deserialize<Order>("null"));

        Assert.AreEqual("Deserialization return null", exception.Message);
    }
}
