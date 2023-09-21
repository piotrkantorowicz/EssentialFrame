using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands.Core;

public abstract class Command : ICommand
{
    protected Command(IdentityContext identityContext)
    {
        if (identityContext == null)
        {
            throw new ArgumentNullException(nameof(identityContext));
        }

        if (identityContext.Tenant == null)
        {
            throw new ArgumentException($"{nameof(identityContext.Tenant)} cannot be null.");
        }

        if (identityContext.User == null)
        {
            throw new ArgumentException($"{nameof(identityContext.User)} cannot be null.");
        }

        if (identityContext.Correlation == null)
        {
            throw new ArgumentException($"{nameof(identityContext.Correlation)} cannot be null.");
        }

        if (identityContext.Service == null)
        {
            throw new ArgumentException($"{nameof(identityContext.Service)} cannot be null.");
        }

        IdentityContext = identityContext;
    }

    protected Command(Guid aggregateIdentifier, IdentityContext identityContext) : this(identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, IdentityContext identityContext) : this(
        identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;
    }

    protected Command(Guid aggregateIdentifier, Guid commandIdentifier, int expectedVersion,
        IdentityContext identityContext) : this(aggregateIdentifier, commandIdentifier, identityContext)
    {
        ExpectedVersion = expectedVersion;
    }

    public Guid AggregateIdentifier { get; } = Guid.NewGuid();

    public int? ExpectedVersion { get; }

    public IdentityContext IdentityContext { get; }

    public Guid CommandIdentifier { get; } = Guid.NewGuid();
}