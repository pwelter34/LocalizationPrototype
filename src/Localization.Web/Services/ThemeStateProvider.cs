namespace Localization.Web.Services;

public abstract class ThemeStateProvider<TState> where TState : class
{
    private ThemeState<TState>? _cached;

    public async ValueTask<ThemeState<TState>> GetThemeAsync()
    {
        if (_cached is not null)
            return _cached;

        _cached = await LoadThemeAsync();
        return _cached;
    }

    protected abstract ValueTask<ThemeState<TState>> LoadThemeAsync();

    public void InvalidateCache() => _cached = null;
}
