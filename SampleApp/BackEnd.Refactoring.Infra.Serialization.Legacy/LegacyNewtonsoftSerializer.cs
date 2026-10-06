using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using BackEnd.Refactoring.Application.Abstration.Srlztn;
using Newtonsoft.Json.Converters;
namespace BackEnd.Refactoring.Infra.Serialization.Legacy;

public class LegacyNewtonSoftSerializer : ISerializer
{
    private readonly JsonSerializerSettings _settings= new()
    {
        NullValueHandling = NullValueHandling.Include,
        Formatting = Formatting.Indented,
        Converters= {new StringEnumConverter()}
    };

    public T Deserialize<T>(string json)
        => JsonConvert.DeserializeObject<T>(json, _settings)
            ?? throw new InvalidOperationException("Deserialization return null");

    public string Serialize<T>(T value)
        => JsonConvert.SerializeObject(value, _settings);
}
