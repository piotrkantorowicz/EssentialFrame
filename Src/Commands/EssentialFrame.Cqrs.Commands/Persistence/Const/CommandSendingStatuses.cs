namespace EssentialFrame.Cqrs.Commands.Persistence.Const;

internal static class CommandSendingStatuses
{
    public const string Started = "Started";
    public const string Scheduled = "Scheduled";
    public const string Cancelled = "Cancelled";
    public const string Completed = "Completed";
}