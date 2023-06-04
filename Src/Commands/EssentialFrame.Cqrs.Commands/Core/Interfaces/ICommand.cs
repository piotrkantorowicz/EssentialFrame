namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface ICommand
{
    Guid CommandIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int? ExpectedVersion { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    Guid CorrelationIdentity { get; }

    string ServiceIdentity { get; }
}