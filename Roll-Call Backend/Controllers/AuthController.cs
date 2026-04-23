using Microsoft.AspNetCore.Mvc;
using RollCallBackend.Services;

namespace RollCallBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISupabaseAuthService _authService;

    public AuthController(ISupabaseAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] AuthRequest request)
    {
        var result = await _authService.SignInAsync(request.Email, request.Password);
        return result.IsSuccess
            ? Ok(new AuthResponse(true, null))
            : BadRequest(new AuthResponse(false, result.ErrorMessage));
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] AuthRequest request)
    {
        var result = await _authService.SignUpAsync(request.Email, request.Password);
        return result.IsSuccess
            ? Ok(new AuthResponse(true, null))
            : BadRequest(new AuthResponse(false, result.ErrorMessage));
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOutUser()
    {
        await _authService.SignOutAsync();
        return Ok();
    }

    [HttpGet("oauth-url")]
    public async Task<IActionResult> GetOAuthUrl([FromQuery] string provider, [FromQuery] string redirectTo)
    {
        var url = await _authService.GetOAuthUrlAsync(provider, redirectTo);
        return Ok(new { url });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var isAuthenticated = await _authService.IsAuthenticatedAsync();
        return Ok(new { isAuthenticated });
    }

    [HttpPost("session")]
    public async Task<IActionResult> SetSession([FromBody] SessionRequest request)
    {
        var result = await _authService.SetSessionAsync(request.AccessToken, request.RefreshToken);
        return result.IsSuccess
            ? Ok(new AuthResponse(true, null))
            : BadRequest(new AuthResponse(false, result.ErrorMessage));
    }
}

public record SessionRequest(string AccessToken, string RefreshToken);

public record AuthRequest(string Email, string Password);
public record AuthResponse(bool IsSuccess, string? ErrorMessage);
