namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Persists and retrieves the selected theme name.
    /// </summary>
    public interface IThemeSelectionStore
    {
        /// <summary>
        /// Saves the selected theme name.
        /// </summary>
        void SaveSelectedTheme(string? themeName);

        /// <summary>
        /// Gets the previously saved theme name, if any.
        /// </summary>
        string? GetSavedThemeName();
    }
}
