namespace Localization.Web.Services;

public record ThemeState<TState>(TState? Current) where TState : class;
