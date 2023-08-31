namespace EssentialFrame.Domain.Shared;

public abstract class DeletebleObject : IDeletableObject
{
    public DateTimeOffset? DeletedDate { get; private set; }

    public bool IsDeleted { get; private set; }

    public void SafeDelete()
    {
        DeletedDate = DateTimeOffset.UtcNow;
        IsDeleted = true;
    }

    public void UnDelete()
    {
        DeletedDate = null;
        IsDeleted = false;
    }
}