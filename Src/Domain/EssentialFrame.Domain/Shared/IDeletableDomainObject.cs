namespace EssentialFrame.Domain.Shared;

public interface IDeletableDomainObject
{
    public DateTimeOffset? DeletedDate { get; }

    public bool IsDeleted { get; }

    public void SafeDelete();

    public void UnDelete();
}