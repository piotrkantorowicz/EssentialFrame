namespace EssentialFrame.Serialization;

public interface ISerializer
{
    T Deserialize<T>(string value, Type type);

    string Serialize<T>(T value);

    string Serialize(object command, string[] exclusions);
}
