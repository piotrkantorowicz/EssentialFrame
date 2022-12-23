namespace EssentialFrame.Core.Identity;

public interface IService
{
    public Guid Identifier { get; }

    public string Name { get; }

    public string GetFullIdentifier();
}
