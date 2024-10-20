using InnoClinic.AuthService.Services.Error;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InnoClinic.AuthService.Middlewares;

public class ExceptionHandler : IExceptionHandler
{
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

    private static string GetDetail(ErrorCode errorCode) => errorCode switch
    {
        ErrorCode.InvalidCredentials => "The email or password you entered is incorrect. Please check your credentials and try again.",
        ErrorCode.InvalidToken => "The provided token is invalid or has expired. Please log in again.",
        ErrorCode.EmailNotConfirmed => "Your email address has not been confirmed. Please check your email for a confirmation link.",
        ErrorCode.EmailAlreadyConfirmed => "This email address has already been confirmed. You can log in with your credentials.",
        ErrorCode.Forbid => "You do not have permission to perform this action.",
        ErrorCode.UserAlreadyExists => "A user with this email address already exists. Try logging in or use a different email.",
        ErrorCode.InvalidUser => "The user could not be found. Please check the provided information.",
        ErrorCode.InvalidEmailOrPassword => "The email or password provided is invalid. Please try again.",
        ErrorCode.InvalidData => "The provided data is invalid or incomplete. Please check the input and try again.",
        ErrorCode.NoUsersFound => "No users were found with the provided information.",
        ErrorCode.SavingError => "An error occurred while saving your data. Please try again later.",
        ErrorCode.DeletionFailed => "Failed to delete the requested resource. Please try again.",
        ErrorCode.Conflict => "There is a conflict with the existing data. Please resolve the conflict and try again.",
        ErrorCode.InternalServerError => "An unexpected error occurred. Please try again later or contact support.",
        _ => "An unknown error occurred."
    };
}
