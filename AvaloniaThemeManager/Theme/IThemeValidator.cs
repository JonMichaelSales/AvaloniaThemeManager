using Avalonia.Media;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Validates and repairs theme configurations.
    /// </summary>
    public interface IThemeValidator
    {
        /// <summary>
        /// Validates a theme and returns the aggregated results.
        /// </summary>
        ThemeValidationResult ValidateTheme(Skin theme);

        /// <summary>
        /// Attempts to automatically repair common theme issues.
        /// </summary>
        Skin AutoFixTheme(Skin theme);

        /// <summary>
        /// Calculates the contrast ratio between two colors.
        /// </summary>
        double CalculateContrastRatio(Color foreground, Color background);
    }
}
