using SpanJson;

namespace EssentialFrame.Serialization.SpanJson;

public class SpanJsonSerializer : ISerializer
{
    public T Deserialize<T>(string value) => JsonSerializer.Generic.Utf16.Deserialize<T>(value);

    public T Deserialize<T>(string value, Type type)
        where T : class
    {
        var serialized = JsonSerializer.NonGeneric.Utf16.Deserialize(value, type);

        return serialized as T;
    }

    public string Serialize<T>(T value) => JsonSerializer.Generic.Utf16.Serialize(value);
}
