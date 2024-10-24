namespace InnoClinic.OfficesService.Services.Error
{
    public class OfficeException : Exception
    {
        public OfficeException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }
    }
}
