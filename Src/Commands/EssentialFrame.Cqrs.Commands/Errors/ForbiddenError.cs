using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class ForbiddenError : ICommandError
{
    public string Message => "Access to requested resource is forbidden";
}