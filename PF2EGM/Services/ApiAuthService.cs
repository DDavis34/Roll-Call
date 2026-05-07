using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
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
    private readonly SupabaseSessionStore _sessionStore;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<ApiAuthService> _logger;

    public ApiAuthService(
        HttpClient http,
        SupabaseSessionStore sessionStore,
        NavigationManager navigationManager,
        ILogger<ApiAuthService> logger)
    {
        _http = http;
        _sessionStore = sessionStore;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        try
        {
            using var response = await SendWithFallbackAsync(
                HttpMethod.Post,
                "api/auth/signin",
                new { email, password });

            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            if (result is null)
            {
                return new AuthResult(false, "Unexpected response from server.");
            }

            if (result.IsSuccess && result.Session is not null)
            {
                _sessionStore.SetSession(result.Session);
            }

            return new AuthResult(result.IsSuccess, result.ErrorMessage);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Sign-in request failed");
            return new AuthResult(false, BuildConnectivityMessage());
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
            using var response = await SendWithFallbackAsync(
                HttpMethod.Post,
                "api/auth/signup",
                new { email, password });

            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            if (result is null)
            {
                return new AuthResult(false, "Unexpected response from server.");
            }

            if (result.IsSuccess && result.Session is not null)
            {
                _sessionStore.SetSession(result.Session);
            }

            return new AuthResult(result.IsSuccess, result.ErrorMessage);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Sign-up request failed");
            return new AuthResult(false, BuildConnectivityMessage());
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
            using var response = await SendWithFallbackAsync(HttpMethod.Post, "api/auth/signout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign-out request failed");
        }
        finally
        {
            _sessionStore.Clear();
        }
    }

    public async Task<string> GetOAuthUrlAsync(string provider)
    {
        try
        {
            var redirectTo = new Uri(new Uri(_navigationManager.BaseUri), "auth/callback").ToString();

            using var responseMessage = await SendWithFallbackAsync(
                HttpMethod.Get,
                $"api/auth/oauth-url?provider={Uri.EscapeDataString(provider)}&redirectTo={Uri.EscapeDataString(redirectTo)}");

            var response = await responseMessage.Content.ReadFromJsonAsync<OAuthUrlResponse>();
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
        if (!_sessionStore.IsAuthenticated || string.IsNullOrWhiteSpace(_sessionStore.Session?.AccessToken))
        {
            return false;
        }

        try
        {
            using var responseMessage = await SendWithFallbackAsync(
                HttpMethod.Get,
                "api/auth/status",
                accessToken: _sessionStore.Session.AccessToken);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            var response = await responseMessage.Content.ReadFromJsonAsync<AuthStatusResponse>();
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
            using var response = await SendWithFallbackAsync(
                HttpMethod.Post,
                "api/auth/session",
                new { accessToken, refreshToken });

            var result = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            if (result is null)
            {
                return new AuthResult(false, "Unexpected response from server.");
            }

            if (result.IsSuccess && result.Session is not null)
            {
                _sessionStore.SetSession(result.Session);
            }

            return new AuthResult(result.IsSuccess, result.ErrorMessage);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Set session request failed");
            return new AuthResult(false, BuildConnectivityMessage());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Set session request failed");
            return new AuthResult(false, "Could not establish session.");
        }
    }

    private async Task<HttpResponseMessage> SendWithFallbackAsync(
        HttpMethod method,
        string relativePath,
        object? body = null,
        string? accessToken = null)
    {
        HttpRequestException? lastException = null;

        foreach (var baseUri in GetCandidateBaseUris())
        {
            try
            {
                var requestUri = new Uri(baseUri, relativePath);
                var request = new HttpRequestMessage(method, requestUri);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                if (body is not null)
                {
                    request.Content = JsonContent.Create(body);
                }

                var response = await _http.SendAsync(request);
                return response;
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                _logger.LogWarning(ex, "Request to {BaseUri} failed, trying next candidate if available.", baseUri);
            }
        }

        throw lastException ?? new HttpRequestException("No backend URL candidates were available.");
    }

    private IEnumerable<Uri> GetCandidateBaseUris()
    {
        var baseUri = _http.BaseAddress ?? new Uri("https://localhost:7001/");
        yield return EnsureTrailingSlash(baseUri);

        var isLocalhost = baseUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                          baseUri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);

        if (!isLocalhost)
        {
            yield break;
        }

        var fallbackUris = new[]
        {
            new Uri("https://localhost:7001/"),
            new Uri("http://localhost:5001/")
        };

        foreach (var uri in fallbackUris.Where(uri => uri != EnsureTrailingSlash(baseUri)))
        {
            yield return uri;
        }
    }

    private static Uri EnsureTrailingSlash(Uri uri) =>
        uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? uri
            : new Uri($"{uri.AbsoluteUri}/");

    private string BuildConnectivityMessage() =>
        "Could not reach the authentication service. Make sure the backend is running on https://localhost:7001 or http://localhost:5001.";

    private record ApiAuthResponse(bool IsSuccess, string? ErrorMessage, SupabaseSessionInfo? Session);
    private record OAuthUrlResponse(string Url);
    private record AuthStatusResponse(bool IsAuthenticated, string? UserId, string? Email);
}
