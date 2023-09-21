using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Identity;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Domain.Core.Events;

public class DomainIdentity : ValueObject
{
    private DomainIdentity(TenantIdentifier tenantIdentifier, UserIdentifier userIdentifier,
        CorrelationIdentifier correlationIdentifier, ServiceIdentifier serviceIdentifier)
    {
        TenantIdentifier = tenantIdentifier ?? throw new ArgumentNullException(nameof(tenantIdentifier));
        UserIdentifier = userIdentifier ?? throw new ArgumentNullException(nameof(userIdentifier));
        CorrelationIdentifier = correlationIdentifier ?? throw new ArgumentNullException(nameof(correlationIdentifier));
        ServiceIdentifier = serviceIdentifier ?? throw new ArgumentNullException(nameof(serviceIdentifier));
    }

    public static DomainIdentity New(IIdentityContext identityContext)
    {
        TenantIdentifier tenantIdentifier = TenantIdentifier.New(identityContext.Tenant.Identifier);
        UserIdentifier userIdentifier = UserIdentifier.New(identityContext.User.Identifier);
        CorrelationIdentifier correlationIdentifier = CorrelationIdentifier.New(identityContext.Correlation.Identifier);
        ServiceIdentifier serviceIdentifier = ServiceIdentifier.New(identityContext.Service.GetFullIdentifier());

        return new DomainIdentity(tenantIdentifier, userIdentifier, correlationIdentifier, serviceIdentifier);
    }

    public static implicit operator DomainIdentity(IdentityContext identityContext)
    {
        return New(identityContext);
    }

    public TenantIdentifier TenantIdentifier { get; }

    public UserIdentifier UserIdentifier { get; }

    public CorrelationIdentifier CorrelationIdentifier { get; }

    public ServiceIdentifier ServiceIdentifier { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TenantIdentifier;
        yield return UserIdentifier;
        yield return CorrelationIdentifier;
        yield return ServiceIdentifier;
    }
}