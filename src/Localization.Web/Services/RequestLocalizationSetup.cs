using Microsoft.Extensions.Options;

namespace Localization.Web.Services;

public class RequestLocalizationSetup : IConfigureOptions<RequestLocalizationOptions>
{
    private readonly ILocalizationProvider _localizationProvider;
    private readonly ILogger<RequestLocalizationSetup> _logger;

    public RequestLocalizationSetup(
        ILocalizationProvider localizationProvider,
        ILogger<RequestLocalizationSetup> logger)
    {
        _localizationProvider = localizationProvider;
        _logger = logger;
    }

    public void Configure(RequestLocalizationOptions options)
    {
        // Load cultures from database synchronously at startup
        var cultures = _localizationProvider.GetCultures();

        var defaultCulture = cultures.FirstOrDefault(c => c.IsDefault)?.CultureCode
            ?? cultures.FirstOrDefault()?.CultureCode
            ?? "en-US";

        var supportedCultures = cultures
            .Select(c => c.CultureCode)
            .ToArray();

        options.ApplyCurrentCultureToResponseHeaders = true;

        options
            .SetDefaultCulture(defaultCulture)
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    }

}
