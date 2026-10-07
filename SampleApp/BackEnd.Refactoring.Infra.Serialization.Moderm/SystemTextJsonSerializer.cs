namespace BackEnd.Refactoring.Infra.Serialization.Moderm;

public class SystemTextJsonSerializer : ISerializer
{
    private readonly System.Text.Json.JsonSerializerOptions _options = new ()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropeprtyNamingPolicy = null,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }   
    };

    public string Serialize<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj, _options);
    }

    public T Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options);
    } 
}
