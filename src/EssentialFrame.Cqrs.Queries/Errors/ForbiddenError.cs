using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Errors;

public class ForbiddenError : IQueryError
{
    public string Message => "Access to requested resource is forbidden";
}