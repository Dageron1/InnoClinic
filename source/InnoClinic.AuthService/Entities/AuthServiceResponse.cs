using InnoClinic.AuthService.Services.Error;
using System.Text.Json.Serialization;

namespace InnoClinic.AuthService.Entities;

public class AuthServiceResponse
{
    public ErrorCode OperationResult { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Result { get; set; }
}
