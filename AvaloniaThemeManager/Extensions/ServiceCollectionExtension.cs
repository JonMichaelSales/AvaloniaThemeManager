using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.ValidationRules;
using AvaloniaThemeManager.Theme;
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
        public static IServiceCollection AddThemeManagerServices(this IServiceCollection services)
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Core services
            services.AddSingleton<ISkinManager, SkinManager>();
            services.AddSingleton<IThemeLoaderService, ThemeLoaderService>();
            services.AddSingleton<IDialogService, DialogService>(); // Updated service name

            // Validation rules
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

        /// <summary>
        /// Registers theme management services and initializes the static MessageBox.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddThemeManagerWithMessageBox(this IServiceCollection services)
        {
            services.AddThemeManagerServices();

            // Build a temporary provider to initialize MessageBox
            var provider = services.BuildServiceProvider();
            MessageBox.Initialize(provider);

            return services;
        }
    }
}