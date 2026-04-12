using Avalonia.Media;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Default implementation of shared theme validation math helpers.
    /// </summary>
    public class ThemeValidationHelper : IThemeValidationHelper
    {
        /// <inheritdoc />
        public double CalculateContrastRatio(Color foreground, Color background)
        {
            try
            {
                var fgLuminance = GetRelativeLuminance(foreground);
                var bgLuminance = GetRelativeLuminance(background);

                var lighter = Math.Max(fgLuminance, bgLuminance);
                var darker = Math.Min(fgLuminance, bgLuminance);

                return (lighter + 0.05) / (darker + 0.05);
            }
            catch (Exception)
            {
                return 1.0;
            }
        }

        /// <inheritdoc />
        public Color AdjustColorForContrast(Color foreground, Color background, double targetRatio)
        {
            var bgLuminance = GetRelativeLuminance(background);
            var isDarkBackground = bgLuminance < 0.5;

            var step = isDarkBackground ? 10 : -10;
            var adjustedColor = foreground;

            for (int i = 0; i < 25; i++)
            {
                var ratio = CalculateContrastRatio(adjustedColor, background);
                if (ratio >= targetRatio)
                {
                    break;
                }

                adjustedColor = Color.FromRgb(
                    (byte)Math.Max(0, Math.Min(255, adjustedColor.R + step)),
                    (byte)Math.Max(0, Math.Min(255, adjustedColor.G + step)),
                    (byte)Math.Max(0, Math.Min(255, adjustedColor.B + step))
                );
            }

            return adjustedColor;
        }

        private double GetRelativeLuminance(Color color)
        {
            var r = GetLuminanceComponent(color.R / 255.0);
            var g = GetLuminanceComponent(color.G / 255.0);
            var b = GetLuminanceComponent(color.B / 255.0);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private double GetLuminanceComponent(double component)
        {
            return component <= 0.03928
                ? component / 12.92
                : Math.Pow((component + 0.055) / 1.055, 2.4);
        }
    }
}
