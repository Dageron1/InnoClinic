namespace InnoClinic.OfficesService.Services.Error
{
    public enum ErrorCode
    {
        Success,
        Created,

        NoContent,

        Forbid,

        Conflict,

        SavingError,
        DeletionFailed,
        InternalServerError
    }
}
