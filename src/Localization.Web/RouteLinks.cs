namespace Localization.Web;

public static class RouteLinks
{
    public static string Home() => "/";
    public static string NotFound() => "/not-found";
    public static string Privacy() => "/privacy";
    public static string Faq() => "/faq";
    public static string Culture(string code, string? returnUrl = null) => $"/culture/{code}?returnUrl={returnUrl}";
}
