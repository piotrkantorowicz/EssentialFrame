namespace EssentialFrame.Cqrs.CommandStore.Const;

internal static class CommandExecutionStatuses
{
    public const string Started = "Started";
    public const string Scheduled = "Scheduled";
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";
}
