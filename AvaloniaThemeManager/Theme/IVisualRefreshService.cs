namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Refreshes application visuals after a theme change.
    /// </summary>
    public interface IVisualRefreshService
    {
        /// <summary>
        /// Refreshes the application visuals.
        /// </summary>
        void Refresh();
    }
}
