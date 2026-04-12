namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Default in-memory storage for registered skins and current skin state.
    /// </summary>
    public class SkinRegistry : ISkinRegistry
    {
        private readonly Dictionary<string, Skin?> _availableSkins = new();

        /// <inheritdoc />
        public Skin? CurrentSkin { get; set; }

        /// <inheritdoc />
        public void RegisterSkin(string? name, Skin? skin)
        {
            if (skin != null && name != null)
            {
                skin.Name = name;
                _availableSkins[name] = skin;
            }
        }

        /// <inheritdoc />
        public Skin? GetSkin(string? name)
        {
            if (name != null && _availableSkins.TryGetValue(name, out var skin))
            {
                return skin;
            }

            return CurrentSkin;
        }

        /// <inheritdoc />
        public List<string> GetAvailableSkinNames() => _availableSkins.Keys.ToList();

        /// <inheritdoc />
        public bool TryGetRegisteredSkin(string name, out Skin? skin)
        {
            if (name == null)
            {
                skin = null;
                return false;
            }

            return _availableSkins.TryGetValue(name, out skin);
        }
    }
}
