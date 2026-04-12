using AvaloniaThemeManager.Models;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Theme selection persistence backed by <see cref="AppSettings"/>.
    /// </summary>
    public class AppSettingsThemeSelectionStore : IThemeSelectionStore
    {
        /// <inheritdoc />
        public void SaveSelectedTheme(string? themeName)
        {
            if (themeName != null)
            {
                AppSettings.Instance.Theme = themeName;
                AppSettings.Instance.Save();
            }
        }

        /// <inheritdoc />
        public string? GetSavedThemeName() => AppSettings.Instance.Theme;
    }
}
