using System;

namespace EssentialFrame.Serialization.Interfaces;

public interface ISerializer
{
    T Deserialize<T>(string value);

    T Deserialize<T>(string value, Type type) where T : class;

    object Deserialize(string value, Type type);

    string Serialize<T>(T value);
}