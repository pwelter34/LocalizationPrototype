

using FluentCommand;

using Localization.Web.Components;
using Localization.Web.Models;
using Localization.Web.Services;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Localization.Web;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents();

        // Add HybridCache and HttpContextAccessor
        builder.Services.AddHybridCache();
        builder.Services.AddHttpContextAccessor();

        // Configure request localization with database-loaded cultures
        builder.Services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationSetup>();
        builder.Services.AddLocalization();

        // Configure FluentCommand with SQL Server and "Localization" connection string
        builder.Services.AddFluentCommand(builder => builder
            .UseConnectionName("Localization")
            .UseSqlServer()
        );

        // Register localization services
        builder.Services.TryAddSingleton<CultureService>();
        builder.Services.TryAddTransient<ILocalizationProvider, DatabaseLocalization>();

        // Configure theme state provider
        builder.Services.TryAddScoped<ThemeStateProvider<Theme>, DatabaseThemeProvider>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();

        // Apply request localization middleware
        app.UseRequestLocalization();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>();

        app.Run();
    }
}
