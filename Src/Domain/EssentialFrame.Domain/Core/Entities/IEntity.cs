using EssentialFrame.Domain.Core.Shared;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Entities;

public interface IEntity<out TEntityIdentifier, TType> : IDeletableDomainObject
    where TEntityIdentifier : TypedIdentifierBase<TType>
{
    public TEntityIdentifier EntityIdentifier { get; }
}