namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// 
    /// </summary>
    public interface IThemeValidationRule
    {
        /// <summary>
        /// 
        /// </summary>
        ThemeValidationResult Validate(Skin theme);
    }
}
