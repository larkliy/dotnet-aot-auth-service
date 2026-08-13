using System.Net;
using AuthService;
using AuthService.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.ExceptionHandlers;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            UserNotFoundException => StatusCodes.Status404NotFound,
            _ => 0
        };

        if (status == 0)
            return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = ((HttpStatusCode)status).ToString() },
            SerializationContext.Default.ProblemDetails,
            cancellationToken: cancellationToken);
        return true;
    }
}