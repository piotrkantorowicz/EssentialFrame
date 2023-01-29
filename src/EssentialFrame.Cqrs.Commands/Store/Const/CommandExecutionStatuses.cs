namespace EssentialFrame.Cqrs.Commands.Store.Const;

internal static class CommandExecutionStatuses
{
    public const string WaitingForExecution = "WaitingForExecution";
    public const string ExecutionCancelled = "ExecutionCancelled";
    public const string ExecutedSuccessfully = "ExecutedSuccessfully";
    public const string ExecutedWithErrors = "ExecutedWithErrors";
}