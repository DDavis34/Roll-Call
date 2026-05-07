namespace PF2EGM.Services;

public record SupabaseSessionInfo(string UserId, string? Email, string AccessToken, string RefreshToken);

public class SupabaseSessionStore
{
    public SupabaseSessionInfo? Session { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Session?.AccessToken);

    public void SetSession(SupabaseSessionInfo session)
    {
        Session = session;
    }

    public void Clear()
    {
        Session = null;
    }
}
