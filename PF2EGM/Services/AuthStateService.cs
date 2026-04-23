namespace PF2EGM.Services;

public class AuthStateService
{
    public event Action? OnAuthStateChanged;

    public void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
}