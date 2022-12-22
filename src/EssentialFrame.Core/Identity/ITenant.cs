namespace EssentialFrame.Core.Identity;

public interface ITenant
{
    Guid Identifier { get; }

    string Code { get; }

    string Name { get; }

    int Key { get; }
}




