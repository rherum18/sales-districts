using Microsoft.AspNetCore.Diagnostics;
using SalesDistrict.Application;

namespace SalesDistrict.Api.ErrorHandling;

public sealed class SalespersonsUpdateExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not SalespersonsUpdateException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Title = "Invalid salesperson assignment", Detail = exception.Message },
        });
    }
}