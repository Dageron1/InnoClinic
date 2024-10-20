namespace InnoClinic.AuthService.Services.Error;

public class AuthException : Exception
{
    public AuthException(ErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }

    public ErrorCode ErrorCode { get; }
}
