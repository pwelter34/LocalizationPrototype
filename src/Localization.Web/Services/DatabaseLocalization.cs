// Ignore Spelling: Localizer

using System.Globalization;
using System.Threading;

using FluentCommand;

using Localization.Web.Models;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Localization;

namespace Localization.Web.Services;

public class DatabaseLocalization : ILocalizationProvider
{
    private readonly HybridCache _hybridCache;
    private readonly IDataSession _dataSession;
    private readonly ILogger<DatabaseLocalization> _logger;

    public DatabaseLocalization(
        HybridCache hybridCache,
        IDataSession dataSession,
        ILogger<DatabaseLocalization> logger)
    {
        _hybridCache = hybridCache;
        _dataSession = dataSession;
        _logger = logger;
    }


    public IReadOnlyList<Culture> GetCultures()
    {
        try
        {
            _logger.LogDebug("Querying database for cultures.");

            var cultures = _dataSession
                .Sql(builder => builder
                    .Select<Culture>()
                    .Where(p => p.IsActive, true)
                    .OrderBy(p => p.DisplayOrder)
                    .Tag()
                )
                .Query<Culture>();

            return cultures.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cultures from database; {ErrorMessage}", ex.Message);
            return [];
        }
    }

    public async ValueTask<IReadOnlyList<Culture>> GetCulturesAsync(
        CancellationToken cancellationToken = default)
    {
        var cacheOptions = new HybridCacheEntryOptions();
        const string cacheKey = "Localizer:Culture";

        return await _hybridCache.GetOrCreateAsync(
            key: cacheKey,
            factory: (token) => GetCulturesFromDatabaseAsync(token),
            options: cacheOptions,
            cancellationToken: cancellationToken
        );
    }


    public async ValueTask<string?> GetStringAsync(
        string name,
        object[]? args = null,
        CultureInfo? culture = null,
        CancellationToken cancellationToken = default)
    {
        var keyCulture = culture ?? CultureInfo.CurrentUICulture;
        var cacheKey = $"name={name}&culture={keyCulture.Name}";

        var cacheOptions = new HybridCacheEntryOptions();
        var cacheTag = $"Localizer:Culture:{keyCulture.Name}";

        var value = await _hybridCache.GetOrCreateAsync(
            key: cacheKey,
            factory: async (token) => await GetStringFromDatabase(name, keyCulture, token),
            options: cacheOptions,
            tags: [cacheTag],
            cancellationToken: cancellationToken
        );

        // format the string if there are arguments
        if (value != null && args?.Length > 0)
            return string.Format(keyCulture, value, args);

        return value;
    }


    protected async Task<string?> GetStringFromDatabase(
        string name,
        CultureInfo culture,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Querying database for localized string. Name: {Name}, Culture: {Culture}", name, culture.Name);

            return await _dataSession
                .Sql(builder => builder
                    .Select<Content>()
                    .Column(p => p.LocalizeText)
                    .Where(p => p.CultureCode, culture.Name)
                    .Where(p => p.LocalizeKey, name)
                    .Tag()
                )
                .QueryValueAsync<string?>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving localization string from database. Name: {Name}, Culture: {Culture}; {ErrorMessage}", name, culture.Name, ex.Message);
            return null;
        }
    }

    protected async ValueTask<IReadOnlyList<Culture>> GetCulturesFromDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Querying database for cultures.");

            var cultures = await _dataSession
                .Sql(builder => builder
                    .Select<Culture>()
                    .Where(p => p.IsActive, true)
                    .OrderBy(p => p.DisplayOrder)
                    .Tag()
                )
                .QueryAsync<Culture>(cancellationToken);

            return cultures.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cultures from database; {ErrorMessage}", ex.Message);
            return [];
        }
    }
}
