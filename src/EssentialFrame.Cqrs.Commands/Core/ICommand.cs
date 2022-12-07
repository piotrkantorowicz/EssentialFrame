namespace EssentialFrame.Cqrs.Commands.Core;

public interface ICommand
{
    Guid AggregateIdentifier { get; }

    int? ExpectedVersion { get; }

    Guid IdentityTenant { get; }

    Guid IdentityUser { get; }

    Guid CommandIdentifier { get; }
}
