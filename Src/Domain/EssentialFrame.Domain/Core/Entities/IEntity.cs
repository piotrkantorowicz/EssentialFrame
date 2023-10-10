using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Entities;

public interface IEntity<out TEntityIdentifier, TType> where TEntityIdentifier : TypedIdentifierBase<TType>
{
    public TEntityIdentifier EntityIdentifier { get; }
}