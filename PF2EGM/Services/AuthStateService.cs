namespace PF2EGM.Services;

public class AuthStateService
{
    public event Action? OnAuthStateChanged;
    public string? UserEmail { get; set; }

    public void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
}