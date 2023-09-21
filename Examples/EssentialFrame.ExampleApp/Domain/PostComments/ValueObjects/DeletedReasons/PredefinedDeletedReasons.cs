using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;

public record PredefinedDeletedReasons : Enumeration<PredefinedDeletedReasons>
{
    public static readonly PredefinedDeletedReasons PostDeleted = new(1, "Post has been deleted");

    private PredefinedDeletedReasons(int value, string displayName) : base(value, displayName)
    {
    }
}