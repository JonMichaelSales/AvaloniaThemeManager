using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Theme;
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
        public static IServiceCollection AddThemeManagerServices(this IServiceCollection services)
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Application abstraction
            services.AddSingleton<IApplication, ApplicationWrapper>();
                
            services.AddSingleton<IThemeLoaderService, ThemeLoaderService>();
            services.AddSingleton<ISkinRegistry, SkinRegistry>();
            services.AddSingleton<IThemeSelectionStore, AppSettingsThemeSelectionStore>();
            services.AddSingleton<ISkinResourceApplier>(serviceProvider =>
                new SkinResourceApplier(serviceProvider.GetRequiredService<IApplication>()));
            services.AddSingleton<IVisualRefreshService>(serviceProvider =>
                new VisualRefreshService(serviceProvider.GetRequiredService<IApplication>()));
            services.AddSingleton<SkinManager>(serviceProvider => new SkinManager(
                serviceProvider.GetRequiredService<IThemeLoaderService>(),
                serviceProvider.GetRequiredService<ISkinRegistry>(),
                serviceProvider.GetRequiredService<IThemeSelectionStore>(),
                serviceProvider.GetRequiredService<ISkinResourceApplier>(),
                serviceProvider.GetRequiredService<IVisualRefreshService>()));
            services.AddSingleton<ISkinManager>(serviceProvider => serviceProvider.GetRequiredService<SkinManager>());
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IThemeValidationHelper, ThemeValidationHelper>();
            services.AddSingleton<IThemeAutoFixer>(serviceProvider =>
                new ThemeAutoFixer(serviceProvider.GetRequiredService<IThemeValidationHelper>()));

            // Theme inheritance manager - will automatically inject ISkinManager
            services.AddSingleton<ThemeInheritanceManager>();

            // Validation rules (existing)
            services.AddSingleton<IThemeValidationRule>(serviceProvider =>
                new BorderValidationRule(serviceProvider.GetRequiredService<IThemeValidationHelper>()));
            services.AddSingleton<IThemeValidationRule>(serviceProvider =>
                new ColorContrastValidationRule(serviceProvider.GetRequiredService<IThemeValidationHelper>()));
            services.AddSingleton<IThemeValidationRule, FontSizeValidationRule>();
            services.AddSingleton<IThemeValidationRule, NameValidationRule>();
            services.AddSingleton<IThemeValidationRule>(serviceProvider =>
                new AccessibilityValidationRule(serviceProvider.GetRequiredService<IThemeValidationHelper>()));
            services.AddSingleton<ThemeValidator>(serviceProvider => new ThemeValidator(
                serviceProvider.GetServices<IThemeValidationRule>(),
                serviceProvider.GetRequiredService<IThemeValidationHelper>(),
                serviceProvider.GetRequiredService<IThemeAutoFixer>()));
            services.AddSingleton<IThemeValidator>(serviceProvider => serviceProvider.GetRequiredService<ThemeValidator>());

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
