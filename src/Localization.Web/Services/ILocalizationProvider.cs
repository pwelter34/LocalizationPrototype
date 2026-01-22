// Ignore Spelling: Localizer

using System.Globalization;

using Localization.Web.Models;

namespace Localization.Web.Services;

public interface ILocalizationProvider
{
    ValueTask<string?> GetStringAsync(
        string name,
        object[]? args = null,
        CultureInfo? culture = null,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<Culture>> GetCulturesAsync(
        CancellationToken cancellationToken = default);

    IReadOnlyList<Culture> GetCultures();
}
