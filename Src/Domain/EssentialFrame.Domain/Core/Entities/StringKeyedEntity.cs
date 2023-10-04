using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Entities;

public abstract class StringKeyedEntity<TEntityIdentifier> : Entity<TEntityIdentifier, string>
    where TEntityIdentifier : TypedIdentifierBase<string>
{
    protected StringKeyedEntity(TEntityIdentifier entityIdentifier) : base(entityIdentifier)
    {
    }
}