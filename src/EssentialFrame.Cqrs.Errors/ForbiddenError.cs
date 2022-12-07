using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Errors;

public class ForbiddenError : IGeneralError
{
    public string Message => "Access to requested resource is forbidden";
}
