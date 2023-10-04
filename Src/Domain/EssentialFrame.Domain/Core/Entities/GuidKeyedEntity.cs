using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Entities;

public abstract class GuidKeyedEntity<TEntityIdentifier> : Entity<TEntityIdentifier, Guid>
    where TEntityIdentifier : TypedIdentifierBase<Guid>
{
    protected GuidKeyedEntity(TEntityIdentifier entityIdentifier) : base(entityIdentifier)
    {
    }
}