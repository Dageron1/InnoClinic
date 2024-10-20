//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Http;
//using System.Text.Json;

//namespace InnoClinic.Shared.Middlewares.ExceptionHandlers;

//public class ExceptionHandler : IExceptionHandler
//{
//    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
//    {
//        Console.WriteLine($"Exception type: {exception.GetType().Name}");

//        httpContext.Response.ContentType = "application/problem+json";

//        //var statusCode = exception switch
//        //{
//        //    AuthException authEx => MapErrorCodeToStatusCode(authEx.ErrorCode),
//        //    _ => StatusCodes.Status500InternalServerError
//        //};

//        int statusCode;
//        string detail;

//        if (exception is AuthException authEx)
//        {
//            statusCode = MapErrorCodeToStatusCode(authEx.ErrorCode);
//            detail = $"Error Code: {authEx.ErrorCode}";
//        }
//        else
//        {
//            statusCode = StatusCodes.Status500InternalServerError;
//            detail = "An unexpected error occurred.";
//        }

//        var problemDetails = new CustomProblemDetails
//        {
//            Status = statusCode,
//            Title = exception.Message,
//            Detail = detail,
//            RequestId = httpContext.TraceIdentifier
//        };

//        httpContext.Response.ContentType = "application/json";
//        httpContext.Response.StatusCode = statusCode;

//        var responseJson = JsonSerializer.Serialize(problemDetails);
//        await httpContext.Response.WriteAsync(responseJson, cancellationToken);

//        return true;
//    }

//    private static int MapErrorCodeToStatusCode(ErrorCode errorCode) => errorCode switch
//    {
//        ErrorCode.InvalidCredentials => StatusCodes.Status401Unauthorized,
//        ErrorCode.InvalidToken => StatusCodes.Status401Unauthorized,
//        ErrorCode.EmailNotConfirmed => StatusCodes.Status400BadRequest,
//        ErrorCode.EmailAlreadyConfirmed => StatusCodes.Status400BadRequest,
//        ErrorCode.Forbid => StatusCodes.Status403Forbidden,
//        ErrorCode.UserAlreadyExists => StatusCodes.Status409Conflict,
//        ErrorCode.InvalidUser => StatusCodes.Status400BadRequest,
//        ErrorCode.InvalidEmailOrPassword => StatusCodes.Status400BadRequest,
//        ErrorCode.InvalidData => StatusCodes.Status422UnprocessableEntity,
//        ErrorCode.NoUsersFound => StatusCodes.Status404NotFound,
//        ErrorCode.SavingError => StatusCodes.Status500InternalServerError,
//        ErrorCode.DeletionFailed => StatusCodes.Status500InternalServerError,
//        ErrorCode.Conflict => StatusCodes.Status409Conflict,
//        ErrorCode.InternalServerError => StatusCodes.Status500InternalServerError,
//        _ => StatusCodes.Status500InternalServerError
//    };
//}
