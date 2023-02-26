using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands.Core;

public abstract class Command : ICommand
{
    protected Command()
    {
    }

    protected Command(IIdentity identity) : this()
    {
        TenantIdentity = identity.Tenant.Identifier;
        UserIdentity = identity.User.Identifier;
        CorrelationIdentity = identity.Correlation.Identifier;
        ServiceIdentity = identity.Service.GetFullIdentifier();
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, IIdentity identity) : this(identity)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, IIdentity identity, int expectedVersion) : this(
        aggregateIdentifier, commandIdentifier, identity)
    {
        ExpectedVersion = expectedVersion;
    }

    public Guid AggregateIdentifier { get; }

    public int? ExpectedVersion { get; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public Guid CorrelationIdentity { get; }

    public Guid CommandIdentifier { get; } = Guid.NewGuid();
}