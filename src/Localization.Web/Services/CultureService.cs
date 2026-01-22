using Microsoft.AspNetCore.Localization;

namespace Localization.Web.Services;

public class CultureService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CultureService> _logger;

    public CultureService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CultureService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public event EventHandler<string>? CultureChanged;

    public void Change(string cultureName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cultureName);

        SaveCultureCookie(cultureName);

        CultureChanged?.Invoke(this, cultureName);
    }

    private void SaveCultureCookie(string cultureName)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext not available, culture cookie not set");
                return;
            }

            var options = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Secure = httpContext.Request.IsHttps
            };

            var requestCulture = new RequestCulture(cultureName);
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                options);

            _logger.LogInformation("Culture changed to {Culture} and cookie set", cultureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting culture cookie for culture {Culture}", cultureName);
        }
    }

}
