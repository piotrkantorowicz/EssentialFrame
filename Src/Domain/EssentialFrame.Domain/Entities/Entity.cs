namespace EssentialFrame.Domain.Entities;

public abstract class Entity
{
    protected Entity(Guid entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public Guid EntityIdentifier { get; }
}