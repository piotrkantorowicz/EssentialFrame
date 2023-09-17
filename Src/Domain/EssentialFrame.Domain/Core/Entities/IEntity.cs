using EssentialFrame.Domain.Core.Shared;

namespace EssentialFrame.Domain.Core.Entities;

public interface IEntity : IDeletableDomainObject
{
    public Guid EntityIdentifier { get; }
}