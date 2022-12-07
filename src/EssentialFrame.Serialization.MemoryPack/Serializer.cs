using MemoryPack;
using MessagePack;

namespace EssentialFrame.Serialization.MemoryPack;

public class Serializer : ISerializer
{
    public T Deserialize<T>(string value, Type? type)
    {
        return MessagePackSerializer.Deserialize(type, value);
    }

    public string Serialize<T>(T value)
    {
        return MessagePackSerializer.SerializeToJson(value);
    }

    public string Serialize(object command, string[] exclusions)
    {
        throw new NotImplementedException();
    }
}