using System;

namespace LegelisteApp.Services;

public class ChangelogService
{
    public event Action? OnShowRequested;

    public void RequestShow()
    {
        OnShowRequested?.Invoke();
    }
}
