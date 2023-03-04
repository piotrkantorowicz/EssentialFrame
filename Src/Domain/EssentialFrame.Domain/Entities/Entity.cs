using EssentialFrame.Domain.Base;

namespace EssentialFrame.Domain.Entities;

public abstract class Entity : BusinessRuleDomainObject
{
    protected Entity(Guid entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public Guid EntityIdentifier { get; }
}