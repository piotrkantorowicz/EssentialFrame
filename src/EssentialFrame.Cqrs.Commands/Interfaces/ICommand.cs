namespace EssentialFrame.Cqrs.Commands.Interfaces;

public interface ICommand
{
    Guid CommandIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int? ExpectedVersion { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    string ServiceIdentity { get; }
}
