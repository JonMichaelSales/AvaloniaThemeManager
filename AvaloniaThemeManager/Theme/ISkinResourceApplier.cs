namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Applies a skin's resources to the application.
    /// </summary>
    public interface ISkinResourceApplier
    {
        /// <summary>
        /// Applies the provided skin's resources.
        /// </summary>
        void ApplySkinResources(Skin skin);
    }
}
