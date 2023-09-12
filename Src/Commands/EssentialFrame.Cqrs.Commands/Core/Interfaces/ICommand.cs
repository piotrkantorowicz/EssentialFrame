using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface ICommand
{
    Guid CommandIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int? ExpectedVersion { get; }

    public IIdentityContext IdentityContext { get; }
}
    