namespace AvaloniaThemeManager.Theme.ValidationRules
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorContrastValidationRule : IThemeValidationRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public ThemeValidationResult Validate(Skin theme)
        {
            var result = new ThemeValidationResult();
            var validator = new ThemeValidator();

            // Check primary text contrast
            var primaryContrast = validator.CalculateContrastRatio(theme.PrimaryTextColor, theme.PrimaryBackground);
            if (primaryContrast < 4.5) // WCAG AA standard
            {
                result.AddError($"Primary text contrast ratio ({primaryContrast:F2}) is below WCAG AA standard (4.5:1)");
            }
            else if (primaryContrast < 7.0) // WCAG AAA standard
            {
                result.AddWarning($"Primary text contrast ratio ({primaryContrast:F2}) is below WCAG AAA standard (7.0:1)");
            }

            // Check secondary text contrast
            var secondaryContrast = validator.CalculateContrastRatio(theme.SecondaryTextColor, theme.SecondaryBackground);
            if (secondaryContrast < 3.0) // More lenient for secondary text
            {
                result.AddError($"Secondary text contrast ratio ({secondaryContrast:F2}) is below minimum standard (3.0:1)");
            }

            // Check accent color readability
            var accentContrast = validator.CalculateContrastRatio(theme.PrimaryTextColor, theme.AccentColor);
            if (accentContrast < 3.0)
            {
                result.AddWarning($"Accent color contrast with primary text ({accentContrast:F2}) may be difficult to read");
            }

            return result;
        }
    }
}
