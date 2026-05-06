using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PF2EGM.Services;

public interface IApiCharacterService
{
    Task<IReadOnlyList<SavedCharacterSummary>> GetSavedCharactersAsync();
    Task<SavedCharacterSummary?> GetSavedCharacterAsync(Guid id);
    Task<CharacterSaveResult> SaveCharacterAsync(SaveCharacterRequest request, Guid? id = null);
    Task<bool> DeleteSavedCharacterAsync(Guid id);
}

public record SaveCharacterRequest(
    string Name,
    string? Ancestry,
    string? CharacterClass,
    int Level,
    string Data);

public record SavedCharacterSummary(
    Guid Id,
    string Name,
    string? Ancestry,
    string? CharacterClass,
    int Level,
    string Data,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);

public record CharacterSaveResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    SavedCharacterSummary? Character = null);

public class ApiCharacterService : IApiCharacterService
{
    private readonly HttpClient _http;
    private readonly SupabaseSessionStore _sessionStore;
    private readonly ILogger<ApiCharacterService> _logger;

    public ApiCharacterService(
        HttpClient http,
        SupabaseSessionStore sessionStore,
        ILogger<ApiCharacterService> logger)
    {
        _http = http;
        _sessionStore = sessionStore;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SavedCharacterSummary>> GetSavedCharactersAsync()
    {
        if (!_sessionStore.IsAuthenticated || string.IsNullOrWhiteSpace(_sessionStore.Session?.AccessToken))
        {
            return [];
        }

        try
        {
            using var response = await SendWithFallbackAsync(HttpMethod.Get, "api/characters", _sessionStore.Session.AccessToken);
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<SavedCharacterSummary>>() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load saved characters.");
            return [];
        }
    }

    public async Task<SavedCharacterSummary?> GetSavedCharacterAsync(Guid id)
    {
        if (!_sessionStore.IsAuthenticated || string.IsNullOrWhiteSpace(_sessionStore.Session?.AccessToken))
        {
            return null;
        }

        try
        {
            using var response = await SendWithFallbackAsync(
                HttpMethod.Get,
                $"api/characters/{id}",
                _sessionStore.Session.AccessToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SavedCharacterSummary>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load saved character {CharacterId}.", id);
            return null;
        }
    }

    public async Task<CharacterSaveResult> SaveCharacterAsync(SaveCharacterRequest request, Guid? id = null)
    {
        if (!_sessionStore.IsAuthenticated || string.IsNullOrWhiteSpace(_sessionStore.Session?.AccessToken))
        {
            return new CharacterSaveResult(false, "Log in to save this character to your account.");
        }

        try
        {
            var relativePath = id.HasValue ? $"api/characters/{id.Value}" : "api/characters";
            var method = id.HasValue ? HttpMethod.Put : HttpMethod.Post;

            using var response = await SendWithFallbackAsync(
                method,
                relativePath,
                _sessionStore.Session.AccessToken,
                request);

            if (response.IsSuccessStatusCode)
            {
                var character = await response.Content.ReadFromJsonAsync<SavedCharacterSummary>();
                return character is null
                    ? new CharacterSaveResult(false, "The character was saved, but the response was empty.")
                    : new CharacterSaveResult(true, Character: character);
            }

            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => new CharacterSaveResult(false, "Log in to save this character to your account."),
                System.Net.HttpStatusCode.NotFound => new CharacterSaveResult(false, "That saved character no longer exists."),
                _ => new CharacterSaveResult(false, "Could not save the character right now.")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save character.");
            return new CharacterSaveResult(false, "Could not save the character right now.");
        }
    }

    public async Task<bool> DeleteSavedCharacterAsync(Guid id)
    {
        if (!_sessionStore.IsAuthenticated || string.IsNullOrWhiteSpace(_sessionStore.Session?.AccessToken))
        {
            return false;
        }

        try
        {
            using var response = await SendWithFallbackAsync(
                HttpMethod.Delete,
                $"api/characters/{id}",
                _sessionStore.Session.AccessToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete saved character {CharacterId}.", id);
            return false;
        }
    }

    private async Task<HttpResponseMessage> SendWithFallbackAsync(
        HttpMethod method,
        string relativePath,
        string accessToken,
        object? body = null)
    {
        HttpRequestException? lastException = null;

        foreach (var baseUri in GetCandidateBaseUris())
        {
            try
            {
                var request = new HttpRequestMessage(method, new Uri(baseUri, relativePath));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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
}
