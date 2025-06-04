using Avalonia;
using Avalonia.Markup.Xaml.Styling;

namespace AvaloniaThemeManager.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Avalonia.Application"/> class to integrate AvaloniaThemeManager functionality.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Include AvaloniaThemeManager themes in your application
        /// </summary>
        /// <param name="app">The Application instance</param>
        /// <returns>The Application instance for method chaining</returns>
        public static Application IncludeThemeManagerStyles(this Application app)
        {
            // Include the theme manager's default styles
            app.Styles.Add(new StyleInclude(new System.Uri("avares://AvaloniaThemeManager/"))
            {
                Source = new System.Uri("avares://AvaloniaThemeManager/Themes/CustomThemes.axaml")
            });

            return app;
        }
    }
}