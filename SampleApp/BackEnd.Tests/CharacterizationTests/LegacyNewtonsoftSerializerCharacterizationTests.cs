using BackEnd.Domain;
using BackEnd.Refactoring.Infra.Serialization.Legacy;
using Xunit;

namespace BackEnd.Tests.Characterization;

public class LegacyNewtonsoftSerializerCharacterizationTests
{
    private static readonly LegacyNewtonSoftSerializer Serializer = new();

    [Fact]
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

        Assert.Equal(expected, json);
    }

    [Fact]
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

        Assert.NotNull(order);
        Assert.Equal(42, order.Id);
        Assert.Equal(99.99m, order.Amount);
        Assert.Equal("Ada", order.CustomerName);
        Assert.Null(order.Note);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero), order.CreatedAt);
    }

    [Fact]
    public void Deserialize_WhenJsonIsNull_ThrowsInvalidOperationException()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => Serializer.Deserialize<Order>("null"));

        Assert.Equal("Deserialization return null", exception.Message);
    }
}
