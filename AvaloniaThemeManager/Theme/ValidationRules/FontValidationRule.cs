namespace AvaloniaThemeManager.Theme.ValidationRules
{
    /// <summary>
    /// 
    /// </summary>
    public class FontSizeValidationRule : IThemeValidationRule
    {
        /// <summary>
        /// 
        /// </summary>
        public ThemeValidationResult Validate(Skin theme)
        {
            var result = new ThemeValidationResult();

            if (theme.FontSizeSmall < 8 || theme.FontSizeSmall > 20)
            {
                result.AddError($"Small font size ({theme.FontSizeSmall}) should be between 8 and 20");
            }

            if (theme.FontSizeMedium < 10 || theme.FontSizeMedium > 24)
            {
                result.AddError($"Medium font size ({theme.FontSizeMedium}) should be between 10 and 24");
            }

            if (theme.FontSizeLarge < 12 || theme.FontSizeLarge > 32)
            {
                result.AddError($"Large font size ({theme.FontSizeLarge}) should be between 12 and 32");
            }

            if (theme.FontSizeSmall >= theme.FontSizeMedium)
            {
                result.AddError("Small font size should be smaller than medium font size");
            }

            if (theme.FontSizeMedium >= theme.FontSizeLarge)
            {
                result.AddError("Medium font size should be smaller than large font size");
            }

            return result;
        }
    }
}
