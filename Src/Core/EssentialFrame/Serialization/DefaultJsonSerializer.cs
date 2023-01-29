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

    public T Deserialize<T>(string value, Type type) where T : class
    {
        object deserialized = JsonSerializer.Deserialize(value, type);

        return deserialized as T;
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value);
    }
}