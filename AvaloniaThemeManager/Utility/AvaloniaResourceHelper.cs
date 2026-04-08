using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace AvaloniaThemeManager.Utility
{
    /// <summary>
    /// Helper class to refresh all windows in an Avalonia application.
    /// </summary>
    public static class AvaloniaResourceHelper
    {
        /// <summary>
        /// Refreshes all windows in the specified Avalonia application by invalidating their visuals.
        /// </summary>
        /// <param name="app">
        /// The <see cref="Application"/> instance whose windows should be refreshed. 
        /// This is typically the main application instance of an Avalonia application.
        /// </param>
        /// <remarks>
        /// on each of them to trigger a visual refresh. It is particularly useful when applying theme or resource changes.
        /// </remarks>
        public static void RefreshAllWindows(Application app)
        {
            if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                foreach (var window in desktop.Windows)
                {
                    window.InvalidateVisual();
                }
            }
        }
    }
}
