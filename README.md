# Localization Prototype

A Blazor application demonstrating database-driven localization and domain-based theming.

## Table of Contents

- [Localization System](#localization-system)
  - [Services](#localization-services)
  - [LocalizedText Component](#localizedtext-component)
  - [Usage Examples](#localization-usage-examples)
- [Theme System](#theme-system)
  - [Services](#theme-services)
  - [ThemeStateView Component](#themestateview-component)
  - [Usage Examples](#theme-usage-examples)

---

## Localization System

The localization system provides database-driven, multi-culture support with caching for optimal performance.

### Localization Services

#### ILocalizationProvider

The core interface for localization services, providing methods to retrieve localized strings and available cultures.

```csharp
public interface ILocalizationProvider
{
    // Get a localized string by key, with optional formatting arguments
    ValueTask<string?> GetStringAsync(
        string name,
        object[]? args = null,
        CultureInfo? culture = null,
        CancellationToken cancellationToken = default);

    // Get all available cultures
    ValueTask<IReadOnlyList<Culture>> GetCulturesAsync(
        CancellationToken cancellationToken = default);

    IReadOnlyList<Culture> GetCultures();
}
```

**Key Features:**

- Async string retrieval with format argument support
- Culture auto-detection from `CultureInfo.CurrentUICulture`
- Returns `null` if translation not found

#### DatabaseLocalization

The concrete implementation of `ILocalizationProvider` that loads translations from a SQL Server database.

**Features:**

- **Two-tier caching** using `HybridCache` for performance
- **Culture-specific cache tags** for selective invalidation
- **Automatic culture detection** from request context
- **Format string support** using `string.Format`

**Cache Strategy:**

- Culture list cached indefinitely
- Individual strings cached by key + culture
- Tagged caching enables bulk invalidation by culture

**Example Query:**

```csharp
// Retrieves localized text from Content table
var value = await _dataSession
    .Sql(builder => builder
        .Select<Content>()
        .Column(p => p.LocalizeText)
        .Where(p => p.CultureCode, culture.Name)
        .Where(p => p.LocalizeKey, name)
    )
    .QueryValueAsync<string?>(cancellationToken);
```

#### CultureService

Service for managing culture changes at runtime.

**Features:**

- Change user culture by setting a persistent cookie
- Cookie expires after 1 year
- Raises `CultureChanged` event for reactive updates
- Secure cookie configuration (HTTPS, SameSite)

**Usage:**

```csharp
public class MyCultureSelector
{
    [Inject]
    public required CultureService CultureService { get; set; }

    private void ChangeCulture(string cultureCode)
    {
        CultureService.Change(cultureCode); // Sets cookie and raises event
        // Page refresh required to apply new culture
    }
}
```

#### RequestLocalizationSetup

Configures ASP.NET Core's request localization middleware at application startup.

**Process:**

1. Loads all active cultures from database synchronously
2. Identifies default culture (first culture marked `IsDefault` or first in list)
3. Configures supported cultures for the application
4. Enables culture headers in responses

**Registration:**

```csharp
builder.Services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationSetup>();
builder.Services.AddLocalization();
```

### LocalizedText Component

A Blazor component that displays localized text from the database.

**Component Location:** `Components/Common/LocalizedText.razor`

**Features:**

- Async loading with optional loading template
- Format argument support (similar to `string.Format`)
- Change detection for key and arguments
- Fallback content when translation is missing
- Renders as `MarkupString` (supports HTML)

**Parameters:**

- `Key` (required) - The localization key to look up
- `Arguments` - Format arguments for string interpolation
- `ChildContent` - Fallback content when translation not found
- `LoadingTemplate` - Custom loading indicator (default: "Loading...")

### Localization Usage Examples

#### Basic Usage

```razor
<!-- Simple localized text -->
<LocalizedText Key="WelcomeMessage" />

<!-- With fallback content -->
<LocalizedText Key="Greeting">
    <span>Hello, World!</span>
</LocalizedText>

<!-- With custom loading template -->
<LocalizedText Key="Loading">
    <LoadingTemplate>
        <div class="spinner-border"></div>
    </LoadingTemplate>
</LocalizedText>
```

#### With Format Arguments

```razor
<!-- Database: "Hello, {0}! You have {1} messages." -->
<LocalizedText Key="WelcomeWithName" Arguments="@(new object[] { userName, messageCount })" />

<!-- Renders as: "Hello, John! You have 5 messages." -->
```

#### Culture Selector Example

```razor
@inject ILocalizationProvider Localizer
@inject CultureService CultureService

<div class="culture-selector">
    @foreach (var culture in _cultures)
    {
        <button class="btn btn-link" @onclick="@(() => ChangeCulture(culture.CultureCode))">
            <span class="fi fi-@culture.CountryCode.ToLower()"></span>
            @culture.DisplayName
        </button>
    }
</div>

@code {
    private IReadOnlyList<Culture> _cultures = Array.Empty<Culture>();

    protected override async Task OnInitializedAsync()
    {
        _cultures = await Localizer.GetCulturesAsync();
    }

    private void ChangeCulture(string cultureCode)
    {
        CultureService.Change(cultureCode);
        // Reload the page to apply new culture
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}
```

---

## Theme System

The theme system provides domain-based theming with automatic loading and caching, structured like Blazor's authentication state.

### Theme Services

#### ThemeState&lt;TState&gt;

A generic record that represents the current theme state.

```csharp
public record ThemeState<TState>(TState? Current) where TState : class;
```

**Properties:**

- `Current` - The theme model instance, or `null` if no theme found

**Usage:**

```csharp
if (themeState.Current is not null)
{
    var siteName = themeState.Current.SiteName;
    var logo = themeState.Current.SiteLogo;
}
```

#### ThemeStateProvider&lt;TState&gt;

Abstract base class for loading theme state based on URL domain.

```csharp
public abstract class ThemeStateProvider<TState> where TState : class
{
    // Gets the current theme state (cached after first load)
    public async ValueTask<ThemeState<TState>> GetThemeAsync();

    // Must be implemented by derived classes
    protected abstract ValueTask<ThemeState<TState>> LoadThemeAsync();

    // Forces reload on next call
    public void InvalidateCache();
}
```

**Caching Behavior:**

- First call loads theme from data source
- Subsequent calls return cached instance
- Cache persists for lifetime of scoped service (per HTTP request)
- Call `InvalidateCache()` to force reload

#### DatabaseThemeProvider

Concrete implementation that loads themes from database based on the request's domain name.

**Features:**

- **Domain-based lookup** using `HttpContext.Request.Host`
- **Two-tier caching** (in-memory + HybridCache)
- **Case-insensitive domain matching**
- **Graceful fallback** when no theme found
- **Comprehensive logging**

**Cache Strategy:**

```csharp
// Cache key format
$"Theme:Domain:{domain}"

// Cached indefinitely via HybridCache
// Per-request cache via scoped service lifetime
```

**Domain Resolution:**

```csharp
// Example: https://contoso.com:8080/path
// Extracts: "contoso.com" (lowercased)
var domain = httpContext.Request.Host.Host.ToLowerInvariant();
```

**Service Registration:**

```csharp
// Registered as scoped - new instance per request
builder.Services.TryAddScoped<ThemeStateProvider<Theme>, DatabaseThemeProvider>();
```

### ThemeStateView Component

A generic component that loads the theme and provides it to child components via cascading parameter.

**Component Location:** `Components/Common/ThemeStateView.razor`

**Type Parameter:**

- `TState` - The theme model type (must be a class)

**Parameters:**

- `ChildContent` (required) - The content to render with theme context
- `LoadingTemplate` - Custom loading indicator (default: "Loading...")

**Features:**

- Loads theme asynchronously on initialization
- Cascades `ThemeState<TState>` to all child components
- Shows loading template until theme is loaded
- Fixed cascading value (won't trigger re-renders on change)

**Usage in App.razor:**

```razor
<ThemeStateView TState="Theme">
    <Routes />
</ThemeStateView>
```

### Theme Usage Examples

#### Basic Theme Access

```razor
@code {
    [CascadingParameter]
    public required ThemeState<Theme> ThemeState { get; set; }

    private string GetSiteName()
    {
        return ThemeState.Current?.SiteName ?? "Default Site Name";
    }
}
```

#### Layout with Theme

```razor
@inherits LayoutComponentBase
@layout MainLayout

<div class="page-container">
    @if (ThemeState.Current is not null)
    {
        <header>
            <img src="@ThemeState.Current.SiteLogo" alt="@ThemeState.Current.SiteName" />
            <h1>@ThemeState.Current.SiteName</h1>
        </header>
    }

    <main>
        @Body
    </main>

    @if (ThemeState.Current is not null)
    {
        <footer>
            <p>&copy; @DateTime.Now.Year @ThemeState.Current.LegalName</p>
            <p>Support: @ThemeState.Current.SupportEmail</p>
        </footer>
    }
</div>

@code {
    [CascadingParameter]
    public required ThemeState<Theme> ThemeState { get; set; }
}
```

#### Dynamic Styling

```razor
@if (ThemeState.Current?.SiteStyle is not null)
{
    <style>
        @((MarkupString)ThemeState.Current.SiteStyle)
    </style>
}

<div class="themed-content">
    @* Your content here *@
</div>

@code {
    [CascadingParameter]
    public required ThemeState<Theme> ThemeState { get; set; }
}
```

#### Handling No Theme Found

```razor
@if (ThemeState.Current is null)
{
    <div class="alert alert-warning">
        <strong>Notice:</strong> No theme configured for this domain.
        Using default styling.
    </div>
}
else
{
    <div class="branded-header" style="background: @ThemeState.Current.PrimaryColor">
        <h1>@ThemeState.Current.SiteName</h1>
    </div>
}

@code {
    [CascadingParameter]
    public required ThemeState<Theme> ThemeState { get; set; }
}
```

#### Custom Theme Provider Example

If you need a different theme model or loading strategy:

```csharp
// Custom theme model
public class CustomTheme
{
    public string Name { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
}

// Custom provider
public class CustomThemeProvider : ThemeStateProvider<CustomTheme>
{
    protected override async ValueTask<ThemeState<CustomTheme>> LoadThemeAsync()
    {
        // Your custom loading logic
        var theme = await LoadFromCustomSource();
        return new ThemeState<CustomTheme>(theme);
    }
}

// Register in Program.cs
builder.Services.TryAddScoped<ThemeStateProvider<CustomTheme>, CustomThemeProvider>();

// Use in App.razor
<ThemeStateView TState="CustomTheme">
    <Routes />
</ThemeStateView>
```

---

## Database Schema

### Culture Table

Stores available cultures for the application.

```sql
CREATE TABLE [dbo].[Culture]
(
    [CultureCode] NVARCHAR(10) PRIMARY KEY,
    [DisplayName] NVARCHAR(100) NOT NULL,
    [CountryCode] NVARCHAR(2),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [DisplayOrder] INT NOT NULL DEFAULT 0
)
```

### Content Table

Stores localized text content.

```sql
CREATE TABLE [dbo].[Content]
(
    [Id] INT PRIMARY KEY IDENTITY,
    [LocalizeKey] NVARCHAR(200) NOT NULL,
    [CultureCode] NVARCHAR(10) NOT NULL,
    [LocalizeText] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_Content_Culture FOREIGN KEY ([CultureCode])
        REFERENCES [dbo].[Culture]([CultureCode])
)
```

### Theme Table

Stores theme configurations by domain.

```sql
CREATE TABLE [dbo].[Theme]
(
    [Id] INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500),
    [DomainName] NVARCHAR(255) NOT NULL UNIQUE,
    [SiteName] NVARCHAR(100),
    [SiteLogo] NVARCHAR(500),
    [SiteStyle] NVARCHAR(MAX),
    [LegalName] NVARCHAR(200),
    [SupportPhoneNumber] NVARCHAR(20),
    [SupportEmail] NVARCHAR(100)
)
```

---

## Configuration

### Connection String

Add to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Localization": "Server=.;Database=LocalizationDb;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Required services
builder.Services.AddRazorComponents();
builder.Services.AddHybridCache();
builder.Services.AddHttpContextAccessor();

// Localization
builder.Services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationSetup>();
builder.Services.AddLocalization();

// Database access
builder.Services.AddFluentCommand(builder => builder
    .UseConnectionName("Localization")
    .UseSqlServer()
);

// Register services
builder.Services.TryAddSingleton<CultureService>();
builder.Services.TryAddTransient<ILocalizationProvider, DatabaseLocalization>();
builder.Services.TryAddScoped<ThemeStateProvider<Theme>, DatabaseThemeProvider>();

var app = builder.Build();

// Middleware
app.UseRequestLocalization(); // Must be before UseAntiforgery
app.UseAntiforgery();
app.MapRazorComponents<App>();

app.Run();
```

---

## Performance Characteristics

### Localization

- **Culture List**: Loaded once at startup, cached in memory
- **Translations**: Cached per culture + key using HybridCache
- **Cache Duration**: Indefinite (manual invalidation only)
- **Database Queries**: Only on cache misses

### Theme System

- **Per Request**: One provider instance per HTTP request (scoped)
- **Per Request Cache**: Theme loaded once, reused within request
- **Cross-Request Cache**: HybridCache stores theme by domain
- **Cache Duration**: Indefinite (manual invalidation only)
- **Database Queries**: Only on cache misses

---

## License

This project is a prototype for demonstration purposes.
