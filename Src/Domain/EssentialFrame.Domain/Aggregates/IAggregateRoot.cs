using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot<out T> where T : TypedGuidIdentifier
{
    public T AggregateIdentifier { get; }

    public Guid TenantIdentifier { get; }

    public IIdentityContext IdentityContext { get; }
}