using Supabase;

namespace RollCallBackend.Services;

public interface ISupabaseUserContextService
{
    Task<SupabaseUserContext?> GetCurrentUserAsync(CancellationToken ct = default);
}

public record SupabaseUserContext(string UserId, string? Email);

public class SupabaseUserContextService : ISupabaseUserContextService
{
    private readonly Supabase.Client _client;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SupabaseUserContextService> _logger;

    public SupabaseUserContextService(
        Supabase.Client client,
        IHttpContextAccessor httpContextAccessor,
        ILogger<SupabaseUserContextService> logger)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<SupabaseUserContext?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var authorization = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization) ||
            !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var accessToken = authorization["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        try
        {
            var user = await _client.Auth.GetUser(accessToken);
            return user is null || string.IsNullOrWhiteSpace(user.Id)
                ? null
                : new SupabaseUserContext(user.Id, user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve Supabase user from bearer token.");
            return null;
        }
    }
}
