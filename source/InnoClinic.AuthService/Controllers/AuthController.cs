using InnoClinic.AuthService.DTOs;
using InnoClinic.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.AuthService.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register([FromBody] RegistrationRequestDto requestModel, CancellationToken cancellationToken)
    {
        var authServiceResult = await _authService.RegisterAsync(requestModel, cancellationToken);

        return StatusCode(201, new { Token = authServiceResult });
    }

    [HttpPost("sessions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto model, CancellationToken cancellationToken)
    {
        var authServiceResult = await _authService.LoginAsync(model, cancellationToken);

        return Ok(new { Token = authServiceResult });
    }
}
