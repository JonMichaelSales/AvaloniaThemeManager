namespace AvaloniaThemeManager.Theme;

/// <summary>
/// Provides the main API for registering, applying, and restoring skins.
/// </summary>
public interface ISkinManager
{
    /// <summary>
    /// Gets the currently applied skin, if any.
    /// </summary>
    Skin? CurrentSkin { get; }
    /// <summary>
    /// Occurs after the current skin changes.
    /// </summary>
    event EventHandler? SkinChanged;
    /// <summary>
    /// Registers a skin under the provided name.
    /// </summary>
    /// <param name="name">The name used to register the skin.</param>
    /// <param name="skin">The skin instance to register.</param>
    void RegisterSkin(string? name, Skin? skin);
    /// <summary>
    /// Gets a skin by name.
    /// </summary>
    /// <param name="name">The registered skin name.</param>
    /// <returns>The matching skin, or the current skin when the registry falls back to it.</returns>
    Skin? GetSkin(string? name);
    /// <summary>
    /// Gets the names of all registered skins.
    /// </summary>
    /// <returns>A list of registered skin names.</returns>
    List<string> GetAvailableSkinNames();
    /// <summary>
    /// Applies a registered skin by name.
    /// </summary>
    /// <param name="skinName">The name of the skin to apply.</param>
    void ApplySkin(string? skinName);
    /// <summary>
    /// Applies the provided skin instance directly.
    /// </summary>
    /// <param name="skin">The skin to apply.</param>
    void ApplySkin(Skin? skin);
    /// <summary>
    /// Persists the selected theme name for future startup restoration.
    /// </summary>
    /// <param name="themeName">The theme name to persist.</param>
    void SaveSelectedTheme(string? themeName);
    /// <summary>
    /// Loads and applies the previously saved theme, if one is available and registered.
    /// </summary>
    void LoadSavedTheme();
}
