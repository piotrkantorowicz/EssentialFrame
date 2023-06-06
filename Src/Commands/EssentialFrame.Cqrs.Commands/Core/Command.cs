using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands.Core;

public abstract class Command : ICommand
{
    protected Command()
    {
    }

    protected Command(IIdentityContext identityContext) : this()
    {
        TenantIdentity = identityContext.Tenant.Identifier;
        UserIdentity = identityContext.User.Identifier;
        CorrelationIdentity = identityContext.Correlation.Identifier;
        ServiceIdentity = identityContext.Service.GetFullIdentifier();
    }

    protected Command(Guid aggregateIdentifier, IIdentityContext identityContext) : this(identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext) : this(
        identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        int expectedVersion) : this(aggregateIdentifier, commandIdentifier, identityContext)
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