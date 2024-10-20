using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.Shared;

public class CustomProblemDetails : ProblemDetails
{
    public string RequestId { get; set; }
}
