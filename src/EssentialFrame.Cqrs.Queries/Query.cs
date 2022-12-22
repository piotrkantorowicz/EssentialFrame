using EssentialFrame.Core.Identity;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Queries;

public abstract class Query<T> : IQuery<T>
{
    protected Query(IIdentity identity)
    {
        TenantIdentity = identity.Tenant.Identifier;
        UserIdentity = identity.User.Identifier;
        ServiceIdentity = identity.Service.GetFullIdentifier();
    }

    protected Query(Guid queryIdentifier,
                    IIdentity identity)
        : this(identity) =>
        QueryIdentifier = queryIdentifier;

    public Guid QueryIdentifier { get; } = Guid.NewGuid();

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public string ServiceIdentity { get; }
}



