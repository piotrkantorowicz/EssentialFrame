using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands;

public class Command : ICommand
{
    public Command() => CommandIdentifier = Guid.NewGuid();

    public Command(IIdentity identity)
        : this()
    {
        var tenant = identity.Tenant.Identifier;
        var user = identity.User.Identifier;

        IdentityTenant = Guid.Empty == IdentityTenant ? tenant : IdentityTenant;
        IdentityUser = Guid.Empty == IdentityUser ? user : IdentityUser;
    }

    public Command(Guid aggregateIdentifier,
                   Guid commandIdentifier,
                   IIdentity identity)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;

        var tenant = identity.Tenant.Identifier;
        var user = identity.User.Identifier;

        IdentityTenant = Guid.Empty == IdentityTenant ? tenant : IdentityTenant;
        IdentityUser = Guid.Empty == IdentityUser ? user : IdentityUser;
    }

    public Command(Guid aggregateIdentifier,
                   Guid commandIdentifier,
                   IIdentity identity,
                   int? expectedVersion = null)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;

        if (expectedVersion is not null)
        {
            ExpectedVersion = expectedVersion;
        }

        var tenant = identity.Tenant.Identifier;
        var user = identity.User.Identifier;

        IdentityTenant = Guid.Empty == IdentityTenant ? tenant : IdentityTenant;
        IdentityUser = Guid.Empty == IdentityUser ? user : IdentityUser;
    }

    public Guid AggregateIdentifier { get; }

    public int? ExpectedVersion { get; }

    public Guid IdentityTenant { get; }

    public Guid IdentityUser { get; }

    public Guid CommandIdentifier { get; }
}
