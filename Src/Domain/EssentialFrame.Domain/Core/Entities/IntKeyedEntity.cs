using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Entities;

public abstract class IntKeyedEntity<TEntityIdentifier> : Entity<TEntityIdentifier, int>
    where TEntityIdentifier : TypedIdentifierBase<int>
{
    protected IntKeyedEntity(TEntityIdentifier entityIdentifier) : base(entityIdentifier)
    {
    }
}