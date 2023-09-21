using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Core;

public abstract class Query<T> : IQuery<T>
{
    protected Query(IIdentityContext identityContext)
    {
        TenantIdentity = identityContext.Tenant.Identifier;
        UserIdentity = identityContext.User.Identifier;
        ServiceIdentity = identityContext.Service.GetFullIdentifier();
    }

    protected Query(Guid queryIdentifier, IIdentityContext identityContext) : this(identityContext)
    {
        QueryIdentifier = queryIdentifier;
    }

    public Guid QueryIdentifier { get; } = Guid.NewGuid();

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public string ServiceIdentity { get; }
}