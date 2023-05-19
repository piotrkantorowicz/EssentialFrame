using System;
using System.Text.Json;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Serialization;

public class DefaultJsonSerializer : ISerializer
{
    public T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value);
    }

    public T Deserialize<T>(string value, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(value, options);
    }

    public T Deserialize<T>(string value, Type type) where T : class
    {
        object deserialized = JsonSerializer.Deserialize(value, type);

        return deserialized as T;
    }

    public T Deserialize<T>(string value, Type type, JsonSerializerOptions options) where T : class
    {
        object deserialized = JsonSerializer.Deserialize(value, type, options);

        return deserialized as T;
    }

    public object Deserialize(string value, Type type)
    {
        return JsonSerializer.Deserialize(value, type);
    }

    public object Deserialize(string value, Type type, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize(value, type, options);
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value);
    }

    public string Serialize<T>(T value, JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(value, options);
    }
}