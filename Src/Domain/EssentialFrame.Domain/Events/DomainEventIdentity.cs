using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events;

public class DomainEventIdentity
{
    public DomainEventIdentity(Guid tenantIdentity, Guid userIdentity, Guid correlationIdentity, string serviceIdentity)
    {
        TenantIdentity = TenantIdentifier.New(tenantIdentity);
        UserIdentity = UserIdentifier.New(userIdentity);
        CorrelationIdentity = CorrelationIdentifier.New(correlationIdentity);
        ServiceIdentity = ServiceIdentifier.New(serviceIdentity);
    }

    public DomainEventIdentity(TenantIdentifier tenantIdentity, UserIdentifier userIdentity,
        CorrelationIdentifier correlationIdentity, ServiceIdentifier serviceIdentity)
    {
        TenantIdentity = tenantIdentity;
        UserIdentity = userIdentity;
        CorrelationIdentity = correlationIdentity;
        ServiceIdentity = serviceIdentity;
    }

    public TenantIdentifier TenantIdentity { get; }

    public UserIdentifier UserIdentity { get; }

    public CorrelationIdentifier CorrelationIdentity { get; }

    public ServiceIdentifier ServiceIdentity { get; }
}