using Microsoft.AspNetCore.Diagnostics;
using SalesDistrict.Application;

namespace SalesDistrict.Api.ErrorHandling;

public sealed class SalespersonsUpdateExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public SalespersonsUpdateExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not SalespersonsUpdateException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Title = "Invalid salesperson assignment", Detail = exception.Message },
        });
    }
}