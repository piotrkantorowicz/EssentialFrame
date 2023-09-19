using EssentialFrame.Domain.Core.ValueObjects;

namespace EssentialFrame.Domain.Core.Events;

public class DomainEventIdentity
{
    public DomainEventIdentity(Guid tenantIdentity, Guid userIdentity, Guid correlationIdentity, string serviceIdentity)
    {
        TenantIdentifier = TenantIdentifier.New(tenantIdentity);
        UserIdentifier = UserIdentifier.New(userIdentity);
        CorrelationIdentifier = CorrelationIdentifier.New(correlationIdentity);
        ServiceIdentifier = ServiceIdentifier.New(serviceIdentity);
    }

    public DomainEventIdentity(TenantIdentifier tenantIdentifier, UserIdentifier userIdentifier,
        CorrelationIdentifier correlationIdentifier, ServiceIdentifier serviceIdentifier)
    {
        TenantIdentifier = tenantIdentifier;
        UserIdentifier = userIdentifier;
        CorrelationIdentifier = correlationIdentifier;
        ServiceIdentifier = serviceIdentifier;
    }

    public TenantIdentifier TenantIdentifier { get; }

    public UserIdentifier UserIdentifier { get; }

    public CorrelationIdentifier CorrelationIdentifier { get; }

    public ServiceIdentifier ServiceIdentifier { get; }
}