namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Repairs common theme issues without mutating the original skin instance.
    /// </summary>
    public interface IThemeAutoFixer
    {
        /// <summary>
        /// Attempts to automatically repair common theme issues.
        /// </summary>
        Skin AutoFixTheme(Skin theme);
    }
}
