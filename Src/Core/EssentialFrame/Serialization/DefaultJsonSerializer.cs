using System;
using System.Text.Json;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Serialization;

public sealed class DefaultJsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public DefaultJsonSerializer(JsonSerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions();
    }

    public T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, _serializerOptions);
    }

    public T Deserialize<T>(string value, Type type) where T : class
    {
        object deserialized = JsonSerializer.Deserialize(value, type, _serializerOptions);

        return deserialized as T;
    }

    public object Deserialize(string value, Type type)
    {
        return JsonSerializer.Deserialize(value, type, _serializerOptions);
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _serializerOptions);
    }
}