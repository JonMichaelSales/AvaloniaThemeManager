
namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Manages theme inheritance and variant creation with dependency injection support.
    /// </summary>
    public class ThemeInheritanceManager
    {
        private readonly Dictionary<string, InheritableSkin> _inheritableThemes = new();
        private readonly Dictionary<string, Skin> _resolvedCache = new();
        private readonly ISkinManager _skinManager;

        /// <summary>
        /// Initializes a new instance of the ThemeInheritanceManager class.
        /// </summary>
        /// <param name="skinManager">The skin manager to use for resolving base themes.</param>
        /// <exception cref="ArgumentNullException">Thrown when skinManager is null.</exception>
        public ThemeInheritanceManager(ISkinManager skinManager)
        {
            _skinManager = skinManager ?? throw new ArgumentNullException(nameof(skinManager));
        }

        /// <summary>
        /// Registers an inheritable theme.
        /// </summary>
        public void RegisterInheritableTheme(string name, InheritableSkin theme)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Theme name cannot be null or empty", nameof(name));
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            theme.Name = name;
            _inheritableThemes[name] = theme;
            _resolvedCache.Remove(name); // Clear cache
        }

        /// <summary>
        /// Gets a resolved theme with inheritance applied.
        /// </summary>
        public Skin? GetResolvedTheme(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

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

            // Fall back to skin manager (now uses injected dependency)
            return _skinManager.GetSkin(theme.BaseThemeName);
        }

        /// <summary>
        /// Creates a theme variant by overriding specific properties.
        /// </summary>
        public InheritableSkin CreateVariant(string baseName, string variantName, Dictionary<string, object> overrides)
        {
            if (string.IsNullOrEmpty(baseName))
                throw new ArgumentException("Base theme name cannot be null or empty", nameof(baseName));
            if (string.IsNullOrEmpty(variantName))
                throw new ArgumentException("Variant theme name cannot be null or empty", nameof(variantName));
            if (overrides == null)
                throw new ArgumentNullException(nameof(overrides));

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