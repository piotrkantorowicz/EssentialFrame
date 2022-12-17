using System.Net;
using EssentialFrame.Cqrs.Errors.Core;
using EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EssentialFrame.Cqrs.Commands.Errors.Handling;

public static class ErrorResponseFactory
{
    public static IActionResult GetResponse(ModelStateDictionary modelStateDictionary)
    {
        var validationErrors = modelStateDictionary
                               .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                               .SelectMany(x => x.Value.Errors.Select(y => new KeyValuePair<string, string>(x.Key,
                                                                          y.ErrorMessage)))
                               .ToList();

        return new BadRequestObjectResult(new ValidationErrorProblemDetails(validationErrors));
    }

    public static IActionResult GetResponse(IError error)
    {
        switch (error)
        {
            case NotFoundError notFound: return new NotFoundObjectResult(new NotFoundProblemDetails(notFound.Message));
            case ConflictError conflict:
                return new ConflictObjectResult(new ConflictErrorProblemDetails(conflict.Message));
            case DbError dbError: return new ConflictObjectResult(new ConflictErrorProblemDetails(dbError.Message));
            case UnprocessableError unprocessableEntity:
                return new UnprocessableEntityObjectResult(new UnprocessableEntityProblemDetails(unprocessableEntity
                                                               .Message));
            case ValidationError validationError:
                return new BadRequestObjectResult(new ValidationErrorProblemDetails(validationError.ValidationErrors));
            case DomainRuleError domainRuleValidationError:
                return new ConflictObjectResult(new DomainRuleValidationProblemDetails(domainRuleValidationError
                                                        .BrokenRule,
                                                    domainRuleValidationError.Message));
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

