using EssentialFrame.Domain.Shared;

namespace EssentialFrame.Domain.Entities;

public interface IEntity : IDeletableDomainObject
{
    public Guid EntityIdentifier { get; }
}