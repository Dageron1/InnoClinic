using InnoClinic.AuthService.Services.Error;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace InnoClinic.AuthService.Middlewares;

public class ExceptionHandler : IExceptionHandler
{
    private readonly IStringLocalizer _localizer;

    public ExceptionHandler(IStringLocalizerFactory localizerFactory)
    {
        var assemblyName = typeof(Program).Assembly.GetName().Name;
        _localizer = localizerFactory.Create("ErrorMessages", assemblyName!);
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/problem+json";

        AuthException? authEx = exception as AuthException;
        var statusCode = authEx != null ? MapErrorCodeToStatusCode(authEx.ErrorCode) : StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = authEx is not null ? authEx.ErrorCode.ToString() : "Internal Server Error",
            Detail = authEx is not null ? GetDetail(authEx.ErrorCode) : "An internal server error occurred.",
            Extensions = { ["RequestId"] = httpContext.TraceIdentifier }
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        var responseJson = JsonSerializer.Serialize(problemDetails);
        await httpContext.Response.WriteAsync(responseJson, cancellationToken);

        return true;
    }

    private static int MapErrorCodeToStatusCode(ErrorCode errorCode) => errorCode switch
    {
        ErrorCode.InvalidCredentials => StatusCodes.Status401Unauthorized,
        ErrorCode.InvalidToken => StatusCodes.Status401Unauthorized,
        ErrorCode.EmailNotConfirmed => StatusCodes.Status400BadRequest,
        ErrorCode.EmailAlreadyConfirmed => StatusCodes.Status400BadRequest,
        ErrorCode.Forbid => StatusCodes.Status403Forbidden,
        ErrorCode.UserAlreadyExists => StatusCodes.Status409Conflict,
        ErrorCode.InvalidUser => StatusCodes.Status400BadRequest,
        ErrorCode.InvalidEmailOrPassword => StatusCodes.Status400BadRequest,
        ErrorCode.InvalidData => StatusCodes.Status422UnprocessableEntity,
        ErrorCode.NoUsersFound => StatusCodes.Status404NotFound,
        ErrorCode.SavingError => StatusCodes.Status500InternalServerError,
        ErrorCode.DeletionFailed => StatusCodes.Status500InternalServerError,
        ErrorCode.Conflict => StatusCodes.Status409Conflict,
        ErrorCode.InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private string GetDetail(ErrorCode errorCode) => errorCode switch
    {
        ErrorCode.InvalidCredentials => _localizer["InvalidCredentials"],
        ErrorCode.InvalidToken => _localizer["InvalidToken"],
        ErrorCode.EmailNotConfirmed => _localizer["EmailNotConfirmed"],
        ErrorCode.EmailAlreadyConfirmed => _localizer["EmailAlreadyConfirmed"],
        ErrorCode.Forbid => _localizer["Forbid"],
        ErrorCode.UserAlreadyExists => _localizer["UserAlreadyExists"],
        ErrorCode.InvalidUser => _localizer["InvalidUser"],
        ErrorCode.InvalidEmailOrPassword => _localizer["InvalidEmailOrPassword"],
        ErrorCode.InvalidData => _localizer["InvalidData"],
        ErrorCode.NoUsersFound => _localizer["NoUsersFound"],
        ErrorCode.SavingError => _localizer["SavingError"],
        ErrorCode.DeletionFailed => _localizer["DeletionFailed"],
        ErrorCode.Conflict => _localizer["Conflict"],
        ErrorCode.InternalServerError => _localizer["InternalServerError"],
        _ => _localizer["UnknownError"]
    };
}
