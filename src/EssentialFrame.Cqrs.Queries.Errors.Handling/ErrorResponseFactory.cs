using System.Net;
using EssentialFrame.Cqrs.Errors.Core;
using EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace EssentialFrame.Cqrs.Queries.Errors.Handling;

public static class ErrorResponseFactory
{
    public static IActionResult GetResponse(IError error)
    {
        switch (error)
        {
            case NotFoundError notFound: return new NotFoundObjectResult(new NotFoundProblemDetails(notFound.Message));
            case ForbiddenError forbidden:
                return new ObjectResult(new ForbiddenProblemDetails(forbidden.Message))
                       {
                           StatusCode = (int)HttpStatusCode.Forbidden
                       };
            case UnexpectedError unexpectedExceptionError:
                return new ObjectResult(new InternalServerErrorProblemDetails(unexpectedExceptionError.Message))
                       {
                           StatusCode = (int)HttpStatusCode.InternalServerError
                       };
            default:
                return new ObjectResult(new InternalServerErrorProblemDetails("An unexpected error occurred"))
                       {
                           StatusCode = (int)HttpStatusCode.InternalServerError
                       };
        }
    }
}
