namespace EssentialFrame.Cqrs.Events.Core.Interfaces;

public interface IEvent
{
    Guid EventIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int? ExpectedVersion { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    Guid CorrelationIdentity { get; }

    string ServiceIdentity { get; }
}