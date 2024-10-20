using InnoClinic.AuthService.Constants;

namespace InnoClinic.AuthService.Utilities;

//public static class ResponseMapper
//{
//    public static int GetStatusCode(AuthOperationResult errorCode)
//    {
//        return errorCode switch
//        {
//            AuthOperationResult.Success => StatusCodes.Status200OK,
//            AuthOperationResult.Created => StatusCodes.Status201Created,
//            AuthOperationResult.UserAlreadyExists => StatusCodes.Status409Conflict,
//            AuthOperationResult.InvalidEmailOrPassword => StatusCodes.Status401Unauthorized,
//            _ => StatusCodes.Status400BadRequest
//        };
//    }

//    public static string GetMessage(AuthOperationResult errorCode)
//    {
//        return errorCode switch
//        {
//            AuthOperationResult.Success => "Operation completed successfully.",
//            AuthOperationResult.Created => "User successfully registered.",
//            AuthOperationResult.UserAlreadyExists => "User already exists.",
//            AuthOperationResult.InvalidEmailOrPassword => "Incorrect login or password.",
//            _ => "Bad request"
//        };
//    }

//    public static string? GetDescription(AuthOperationResult errorCode)
//    {
//        return errorCode switch
//        {
//            AuthOperationResult.Success => null,
//            AuthOperationResult.Created => "Registration is completed. A link to confirm your email has been sent.",
//            AuthOperationResult.UserAlreadyExists => "The user with this email is already registered. If this is you, click the reset password button.",
//            AuthOperationResult.InvalidEmailOrPassword => "Invalid user credentials. Check your username or password.",
//            _ => "Bad request"
//        };
//    }
//}
