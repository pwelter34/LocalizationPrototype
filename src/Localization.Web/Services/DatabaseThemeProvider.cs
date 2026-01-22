using FluentCommand;

using Localization.Web.Models;

using Microsoft.Extensions.Caching.Hybrid;

namespace Localization.Web.Services;

public class DatabaseThemeProvider : ThemeStateProvider<Theme>
{
    private readonly HybridCache _hybridCache;
    private readonly IDataSession _dataSession;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DatabaseThemeProvider> _logger;

    public DatabaseThemeProvider(
        HybridCache hybridCache,
        IDataSession dataSession,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DatabaseThemeProvider> logger)
    {
        _hybridCache = hybridCache;
        _dataSession = dataSession;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async ValueTask<ThemeState<Theme>> LoadThemeAsync()
    {
        var domain = GetCurrentDomain();

        if (string.IsNullOrEmpty(domain))
        {
            _logger.LogWarning("Unable to determine current domain for theme loading.");
            return new ThemeState<Theme>(null);
        }

        var cacheKey = $"Theme:Domain:{domain}";
        var cacheOptions = new HybridCacheEntryOptions();

        var theme = await _hybridCache.GetOrCreateAsync(
            key: cacheKey,
            factory: async (token) => await GetThemeFromDatabaseAsync(domain, token),
            options: cacheOptions,
            cancellationToken: default
        );

        if (theme is null)
            _logger.LogInformation("No theme found for domain: {Domain}", domain);
        else
            _logger.LogDebug("Theme '{ThemeName}' loaded for domain: {Domain}", theme.Name, domain);

        return new ThemeState<Theme>(theme);
    }

    private string? GetCurrentDomain()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        var host = httpContext.Request.Host;
        return host.Host.ToLowerInvariant();
    }

    private async Task<Theme?> GetThemeFromDatabaseAsync(
        string domain,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Querying database for theme with domain: {Domain}", domain);

            return await _dataSession
                .Sql(builder => builder
                    .Select<Theme>()
                    .Where(p => p.DomainName, domain)
                    .Tag()
                )
                .QuerySingleAsync<Theme>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving theme from database for domain: {Domain}; {ErrorMessage}", domain, ex.Message);
            return null;
        }
    }
}
