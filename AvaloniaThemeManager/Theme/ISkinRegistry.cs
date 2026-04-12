namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Stores registered skins and the currently applied skin.
    /// </summary>
    public interface ISkinRegistry
    {
        /// <summary>
        /// Gets or sets the currently applied skin.
        /// </summary>
        Skin? CurrentSkin { get; set; }

        /// <summary>
        /// Registers a skin with the specified name.
        /// </summary>
        void RegisterSkin(string? name, Skin? skin);

        /// <summary>
        /// Gets a registered skin by name, or the current skin when the name is null or unknown.
        /// </summary>
        Skin? GetSkin(string? name);

        /// <summary>
        /// Gets all registered skin names.
        /// </summary>
        List<string> GetAvailableSkinNames();

        /// <summary>
        /// Attempts to get a registered skin by name without falling back to the current skin.
        /// </summary>
        bool TryGetRegisteredSkin(string name, out Skin? skin);
    }
}
