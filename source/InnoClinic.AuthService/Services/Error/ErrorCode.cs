namespace InnoClinic.AuthService.Services.Error;

public enum ErrorCode
{
    Success,
    Created,

    InvalidCredentials,
    InvalidToken,
    EmailNotConfirmed,
    EmailAlreadyConfirmed,
    Forbid,

    UserAlreadyExists,
    InvalidUser,

    InvalidEmailOrPassword,
    InvalidData,
    Conflict,
    NoUsersFound,

    SavingError,
    DeletionFailed,
    InternalServerError
}
