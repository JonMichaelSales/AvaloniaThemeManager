using Avalonia.Media;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Provides low-level validation math helpers shared across theme validators and rules.
    /// </summary>
    public interface IThemeValidationHelper
    {
        /// <summary>
        /// Calculates the contrast ratio between two colors.
        /// </summary>
        double CalculateContrastRatio(Color foreground, Color background);

        /// <summary>
        /// Adjusts a foreground color to reach the requested contrast ratio.
        /// </summary>
        Color AdjustColorForContrast(Color foreground, Color background, double targetRatio);
    }
}
