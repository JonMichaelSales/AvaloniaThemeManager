// Extensions/AppBuilderExtensions.cs
using Avalonia;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring and integrating the AvaloniaThemeManager 
    /// into an Avalonia application using the <see cref="AppBuilder"/>.
    /// </summary>
    /// <remarks>
    /// This class contains methods to enable the AvaloniaThemeManager with default or custom configurations.
    /// It simplifies the setup process by allowing developers to chain theme manager configuration
    /// into the application initialization pipeline.
    /// </remarks>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Adds AvaloniaThemeManager to the application
        /// </summary>
        /// <param name="builder">The AppBuilder instance</param>
        /// <returns>The AppBuilder instance for method chaining</returns>
        public static AppBuilder UseThemeManager(this AppBuilder builder)
        {
            return builder.AfterSetup(app =>
            {
                // Initialize the theme manager and load default theme
                var themeManager = SkinManager.Instance;
                themeManager.LoadSavedTheme();
            });
        }

        /// <summary>
        /// Adds AvaloniaThemeManager with custom configuration
        /// </summary>
        /// <param name="builder">The AppBuilder instance</param>
        /// <param name="configure">Configuration action</param>
        /// <returns>The AppBuilder instance for method chaining</returns>
        public static AppBuilder UseThemeManager(this AppBuilder builder, System.Action<SkinManager> configure)
        {
            return builder.AfterSetup(app =>
            {
                var themeManager = SkinManager.Instance;
                configure(themeManager);
                themeManager.LoadSavedTheme();
            });
        }
    }
}

// Extensions/ApplicationExtensions.cs
