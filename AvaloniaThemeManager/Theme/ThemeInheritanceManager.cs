using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// 
    /// </summary>
    public class ThemeInheritanceManager
    {
        private readonly Dictionary<string, InheritableSkin> _inheritableThemes = new();
        private readonly Dictionary<string, Skin> _resolvedCache = new();

        /// <summary>
        /// Registers an inheritable theme.
        /// </summary>
        public void RegisterInheritableTheme(string name, InheritableSkin theme)
        {
            theme.Name = name;
            _inheritableThemes[name] = theme;
            _resolvedCache.Remove(name); // Clear cache
        }

        /// <summary>
        /// Gets a resolved theme with inheritance applied.
        /// </summary>
        public Skin? GetResolvedTheme(string name)
        {
            if (_resolvedCache.TryGetValue(name, out var cached))
            {
                return cached;
            }

            if (!_inheritableThemes.TryGetValue(name, out var inheritableTheme))
            {
                return null;
            }

            var baseTheme = GetBaseTheme(inheritableTheme);
            var resolved = inheritableTheme.CreateResolvedSkin(baseTheme);

            _resolvedCache[name] = resolved;
            return resolved;
        }

        private Skin? GetBaseTheme(InheritableSkin theme)
        {
            if (string.IsNullOrEmpty(theme.BaseThemeName))
            {
                return null;
            }

            // Handle recursive inheritance
            if (_inheritableThemes.TryGetValue(theme.BaseThemeName, out var baseInheritable))
            {
                return GetResolvedTheme(theme.BaseThemeName);
            }

            // Fall back to regular skin manager
            return SkinManager.Instance.GetSkin(theme.BaseThemeName);
        }

        /// <summary>
        /// Creates a theme variant by overriding specific properties.
        /// </summary>
        public InheritableSkin CreateVariant(string baseName, string variantName, Dictionary<string, object> overrides)
        {
            var variant = new InheritableSkin
            {
                Name = variantName,
                BaseThemeName = baseName,
                PropertyOverrides = overrides
            };

            RegisterInheritableTheme(variantName, variant);
            return variant;
        }

        /// <summary>
        /// Clears the resolved theme cache.
        /// </summary>
        public void ClearCache()
        {
            _resolvedCache.Clear();
        }
    }
}
