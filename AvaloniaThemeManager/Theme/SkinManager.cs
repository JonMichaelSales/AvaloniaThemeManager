using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Manages available skins and orchestrates applying them to the application.
    /// </summary>
    public class SkinManager : ISkinManager
    {
        private readonly IThemeLoaderService _themeLoaderService;
        private readonly ISkinRegistry _skinRegistry;
        private readonly IThemeSelectionStore _themeSelectionStore;
        private readonly ISkinResourceApplier _skinResourceApplier;
        private readonly IVisualRefreshService _visualRefreshService;

        /// <summary>
        /// Gets the currently applied skin.
        /// </summary>
        public Skin? CurrentSkin
        {
            get => _skinRegistry.CurrentSkin;
            private set => _skinRegistry.CurrentSkin = value;
        }

        /// <summary>
        /// Event that is raised when the skin is changed.
        /// </summary>
        public event EventHandler? SkinChanged;

        /// <summary>
        /// Initializes a new instance using default collaborators derived from the application abstraction.
        /// </summary>
        public SkinManager(IThemeLoaderService themeLoaderService, IApplication application)
            : this(themeLoaderService, application, application.AppStyles)
        {
        }

        /// <summary>
        /// Initializes a new instance using default collaborators derived from the application abstraction.
        /// </summary>
        public SkinManager(IThemeLoaderService themeLoaderService, IApplication application, IStylesCollection styles)
            : this(
                themeLoaderService,
                new SkinRegistry(),
                new AppSettingsThemeSelectionStore(),
                new SkinResourceApplier(application),
                new VisualRefreshService(application))
        {
            ArgumentNullException.ThrowIfNull(application);
            ArgumentNullException.ThrowIfNull(styles);
        }

        /// <summary>
        /// Initializes a new instance with focused collaborators.
        /// </summary>
        public SkinManager(
            IThemeLoaderService themeLoaderService,
            ISkinRegistry skinRegistry,
            IThemeSelectionStore themeSelectionStore,
            ISkinResourceApplier skinResourceApplier,
            IVisualRefreshService visualRefreshService)
        {
            _themeLoaderService = themeLoaderService ?? throw new ArgumentNullException(nameof(themeLoaderService));
            _skinRegistry = skinRegistry ?? throw new ArgumentNullException(nameof(skinRegistry));
            _themeSelectionStore = themeSelectionStore ?? throw new ArgumentNullException(nameof(themeSelectionStore));
            _skinResourceApplier = skinResourceApplier ?? throw new ArgumentNullException(nameof(skinResourceApplier));
            _visualRefreshService = visualRefreshService ?? throw new ArgumentNullException(nameof(visualRefreshService));

            RegisterDefaultSkins();
        }

        private void RegisterDefaultSkins()
        {
            var themePath = Path.Combine(AppContext.BaseDirectory, "Themes");
            var skins = _themeLoaderService.LoadSkins(themePath);
            foreach (var skin in skins)
            {
                RegisterSkin(skin.Name, skin);
            }
        }

        /// <inheritdoc />
        public void RegisterSkin(string? name, Skin? skin)
        {
            _skinRegistry.RegisterSkin(name, skin);
        }

        /// <inheritdoc />
        public Skin? GetSkin(string? name)
        {
            return _skinRegistry.GetSkin(name);
        }

        /// <inheritdoc />
        public List<string> GetAvailableSkinNames()
        {
            return _skinRegistry.GetAvailableSkinNames();
        }

        /// <inheritdoc />
        public void ApplySkin(string? skinName)
        {
            try
            {
                if (skinName != null && _skinRegistry.TryGetRegisteredSkin(skinName, out var skin))
                {
                    ApplySkin(skin);
                    SaveSelectedTheme(skinName);
                }
                else
                {
                    Console.WriteLine($"Skin not found: {skinName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying skin {skinName}: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void ApplySkin(Skin? skin)
        {
            skin ??= new Skin();
            CurrentSkin = skin;

            try
            {
                _skinResourceApplier.ApplySkinResources(skin);
                _visualRefreshService.Refresh();
                SkinChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying custom skin: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public void SaveSelectedTheme(string? themeName)
        {
            _themeSelectionStore.SaveSelectedTheme(themeName);
        }

        /// <inheritdoc />
        public void LoadSavedTheme()
        {
            var themeName = _themeSelectionStore.GetSavedThemeName();
            if (!string.IsNullOrEmpty(themeName) && _skinRegistry.TryGetRegisteredSkin(themeName, out _))
            {
                ApplySkin(themeName);
            }
        }
    }
}
