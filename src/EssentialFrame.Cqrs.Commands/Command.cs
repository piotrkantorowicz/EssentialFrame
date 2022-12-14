using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Cqrs.Commands;

public record Command() : ICommand
{
    public Command(IIdentity identity)
        : this()
    {
        var tenantIdentity = identity.Tenant.Identifier;
        var userIdentity = identity.User.Identifier;
        var serviceIdentity = identity.Service.GetFullIdentifier();

        TenantIdentity = Guid.Empty == TenantIdentity ? tenantIdentity : TenantIdentity;
        UserIdentity = Guid.Empty == UserIdentity ? userIdentity : UserIdentity;

        ServiceIdentity = string.IsNullOrEmpty(ServiceIdentity)
            ? serviceIdentity
            : ServiceIdentity;
    }

    public Command(Guid aggregateIdentifier,
                   Guid commandIdentifier,
                   IIdentity identity)
        : this(identity)
    {
        AggregateIdentifier = aggregateIdentifier;
        CommandIdentifier = commandIdentifier;
    }

    public Command(Guid aggregateIdentifier,
                   Guid commandIdentifier,
                   IIdentity identity,
                   int? expectedVersion = null)
        : this(aggregateIdentifier,
               commandIdentifier,
               identity)
    {
        if (expectedVersion is not null)
        {
            ExpectedVersion = expectedVersion;
        }
    }

    public Guid AggregateIdentifier { get; }

    public int? ExpectedVersion { get; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public Guid CommandIdentifier { get; } = Guid.NewGuid();
}
