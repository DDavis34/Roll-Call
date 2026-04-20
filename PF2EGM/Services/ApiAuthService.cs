using System.Net.Http.Json;

namespace PF2EGM.Services;

public interface IApiAuthService
{
    Task<AuthResult> SignInAsync(string email, string password);
    Task<AuthResult> SignUpAsync(string email, string password);
    Task SignOutAsync();
    Task<string> GetOAuthUrlAsync(string provider);
    Task<bool> IsAuthenticatedAsync();
    Task<AuthResult> SetSessionAsync(string accessToken, string refreshToken);
}

public record AuthResult(bool IsSuccess, string? ErrorMessage = null);

public class ApiAuthService : IApiAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiAuthService> _logger;

    public ApiAuthService(HttpClient http, ILogger<ApiAuthService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/signin", new { email, password });
            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            return result is not null
                ? new AuthResult(result.IsSuccess, result.ErrorMessage)
                : new AuthResult(false, "Unexpected response from server.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign-in request failed");
            return new AuthResult(false, "Could not reach the authentication service.");
        }
    }

    public async Task<AuthResult> SignUpAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/signup", new { email, password });
            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            return result is not null
                ? new AuthResult(result.IsSuccess, result.ErrorMessage)
                : new AuthResult(false, "Unexpected response from server.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign-up request failed");
            return new AuthResult(false, "Could not reach the authentication service.");
        }
    }

    public async Task SignOutAsync()
    {
        try
        {
            await _http.PostAsync("api/auth/signout", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign-out request failed");
        }
    }

    public async Task<string> GetOAuthUrlAsync(string provider)
    {
        try
        {
            var redirectTo = $"{_http.BaseAddress}auth/callback";
            var response = await _http.GetFromJsonAsync<OAuthUrlResponse>(
                $"api/auth/oauth-url?provider={Uri.EscapeDataString(provider)}&redirectTo={Uri.EscapeDataString(redirectTo)}");
            return response?.Url ?? throw new InvalidOperationException("No URL returned.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OAuth URL request failed");
            throw;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<AuthStatusResponse>("api/auth/status");
            return response?.IsAuthenticated ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auth status request failed");
            return false;
        }
    }

    public async Task<AuthResult> SetSessionAsync(string accessToken, string refreshToken)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/session",
                new { accessToken, refreshToken });
            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            return result is not null
                ? new AuthResult(result.IsSuccess, result.ErrorMessage)
                : new AuthResult(false, "Unexpected response from server.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Set session request failed");
            return new AuthResult(false, "Could not establish session.");
        }
    }

    private record ApiAuthResponse(bool IsSuccess, string? ErrorMessage);
    private record OAuthUrlResponse(string Url);
    private record AuthStatusResponse(bool IsAuthenticated);
}
