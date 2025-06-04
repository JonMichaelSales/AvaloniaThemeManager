// Services/ServiceCollectionExtensions.cs

using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.ValidationRules;
using AvaloniaThemeManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaloniaThemeManager.Extensions
{
    /// <summary>
    /// Provides extension methods for registering theme management services in an Avalonia application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services required for theme management in an Avalonia application.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the theme management services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the theme management services registered.</returns>
        /// <remarks>
        /// This method registers the following services:
        /// <list type="bullet">
        /// <item>Logging services with console output and a minimum log level of Debug.</item>
        /// <item>Singleton instances of <see cref="SkinManager"/></item>
        /// <item>Transient instances of <see cref="ThemeSettingsViewModel"/> and <see cref="QuickThemeSwitcherViewModel"/>.</item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddThemeManagerServices(this IServiceCollection services)
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Singletons
            services.AddSingleton<ISkinManager,SkinManager>();
            services.AddSingleton<IThemeLoaderService, ThemeLoaderService>();
            services.AddSingleton<IErrorDialogService, ErrorDialogService>();
            services.AddSingleton<IThemeValidationRule, BorderValidationRule>();
            services.AddSingleton<IThemeValidationRule, ColorContrastValidationRule>();
            services.AddSingleton<IThemeValidationRule, NameValidationRule>();
            services.AddSingleton<IThemeValidationRule, AccessibilityValidationRule>();
            services.AddSingleton<ThemeInheritanceManager>();

            // ViewModels
            services.AddTransient<ThemeSettingsViewModel>();
            services.AddTransient<QuickThemeSwitcherViewModel>();

            return services;
        }
    }
}