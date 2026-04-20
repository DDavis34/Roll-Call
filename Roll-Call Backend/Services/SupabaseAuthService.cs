using Supabase;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;

namespace RollCallBackend.Services;

public interface ISupabaseAuthService
{
    Task<AuthResult> SignInAsync(string email, string password);
    Task<AuthResult> SignUpAsync(string email, string password);
    Task SignOutAsync();
    Task<string> GetOAuthUrlAsync(string provider, string redirectTo);
    Task<bool> IsAuthenticatedAsync();
}

public record AuthResult(bool IsSuccess, string? ErrorMessage = null);

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly Supabase.Client _client;
    private readonly ILogger<SupabaseAuthService> _logger;

    public SupabaseAuthService(Supabase.Client client, ILogger<SupabaseAuthService> logger)
    {
        _client = client;
        _logger  = logger;
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);
            return session?.User is not null
                ? new AuthResult(true)
                : new AuthResult(false, "Sign-in failed. Please check your credentials.");
        }
        catch (GotrueException ex)
        {
            _logger.LogWarning("Supabase sign-in error: {Message}", ex.Message);
            return new AuthResult(false, FriendlyError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected sign-in error");
            return new AuthResult(false, "An unexpected error occurred.");
        }
    }

    public async Task<AuthResult> SignUpAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignUp(email, password);
            return session?.User is not null
                ? new AuthResult(true)
                : new AuthResult(false, "Sign-up failed.");
        }
        catch (GotrueException ex)
        {
            return new AuthResult(false, FriendlyError(ex.Message));
        }
    }

    public async Task SignOutAsync() => await _client.Auth.SignOut();

    public async Task<string> GetOAuthUrlAsync(string provider, string redirectTo)
    {
        var p = Enum.Parse<Supabase.Gotrue.Constants.Provider>(provider, ignoreCase: true);
        var state = await _client.Auth.SignIn(p, new SignInOptions { RedirectTo = redirectTo });
        return state!.Uri!.ToString();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var session = _client.Auth.CurrentSession;
        return session?.User is not null && session.ExpiresAt() > DateTime.UtcNow;
    }

    private static string FriendlyError(string raw) => raw.ToLower() switch
    {
        var s when s.Contains("invalid login") => "Invalid email or password.",
        var s when s.Contains("email not confirmed") => "Please confirm your email before signing in.",
        var s when s.Contains("user already registered") => "An account with this email already exists.",
        _ => "Something went wrong. Please try again."
    };
}
