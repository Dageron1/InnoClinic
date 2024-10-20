namespace InnoClinic.Shared
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string RequestId { get; set; }
    }
}
