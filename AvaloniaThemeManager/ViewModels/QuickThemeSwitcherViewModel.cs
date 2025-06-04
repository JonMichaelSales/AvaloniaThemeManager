﻿using System.Collections.ObjectModel;
using Avalonia.Media;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AvaloniaThemeManager.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class QuickThemeSwitcherViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private ThemeInfo? _selectedTheme;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickThemeSwitcherViewModel"/> class
        /// with a default logger instance.
        /// </summary>
        public QuickThemeSwitcherViewModel() : this(
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickThemeSwitcherViewModel"/> class
        /// with the specified logger instance.
        /// </summary>
        /// <param name="logger">
        /// An instance of <see cref="ILogger"/> used for logging operations within the view model.
        /// </param>
        public QuickThemeSwitcherViewModel(ILogger logger)
        {
            _logger = logger;
            AvailableThemes = new ObservableCollection<ThemeInfo>();

            LoadAvailableThemes();
            LoadCurrentTheme();

            // Subscribe to skin manager changes to keep in sync
            SkinManager.Instance.SkinChanged += OnSkinChanged;
        }

        /// <summary>
        /// Gets the collection of available themes that can be selected and applied
        /// within the application.
        /// </summary>
        /// <remarks>
        /// This property is populated by the <see cref="LoadAvailableThemes"/> method,
        /// which retrieves the themes from the <see cref="SkinManager"/>. The collection
        /// is updated dynamically to reflect the available themes.
        /// </remarks>
        public ObservableCollection<ThemeInfo> AvailableThemes { get; }

        /// <summary>
        /// Gets or sets the currently selected theme.
        /// </summary>
        /// <remarks>
        /// When a new theme is selected, the corresponding theme is applied automatically.
        /// The selected theme is synchronized with the <see cref="AvailableThemes"/> collection.
        /// </remarks>
        public ThemeInfo? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _selectedTheme, value) != null)
                {
                    ApplyTheme(value);
                }
            }
        }

        private void LoadAvailableThemes()
        {
            try
            {
                var skinManager = SkinManager.Instance;
                var themeNames = skinManager.GetAvailableSkinNames();

                AvailableThemes.Clear();

                foreach (var themeName in themeNames)
                {
                    var skin = skinManager.GetSkin(themeName);
                    if (skin != null)
                    {
                        var themeInfo = new ThemeInfo
                        {
                            Name = themeName,
                            Description = GetThemeDescription(themeName),
                            PreviewColor = new SolidColorBrush(skin.AccentColor)
                        };
                        AvailableThemes.Add(themeInfo);
                    }
                }

                _logger.LogDebug("Loaded {ThemeCount} themes for quick switcher", AvailableThemes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load themes for quick switcher");
            }
        }

        private void LoadCurrentTheme()
        {
            try
            {
                var currentSkin = SkinManager.Instance.CurrentSkin;
                if (currentSkin?.Name != null)
                {
                    var currentTheme = AvailableThemes.FirstOrDefault(t => t.Name == currentSkin.Name);
                    if (currentTheme != null)
                    {
                        // Set without triggering the setter to avoid recursive application
                        _selectedTheme = currentTheme;
                        this.RaisePropertyChanged(nameof(SelectedTheme));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load current theme for quick switcher");
            }
        }

        private void ApplyTheme(ThemeInfo? themeInfo)
        {
            try
            {
                if (themeInfo != null)
                {
                    SkinManager.Instance.ApplySkin(themeInfo.Name);
                    _logger.LogInformation("Quick theme switch to: {ThemeName}", themeInfo.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply theme via quick switcher: {ThemeName}", themeInfo?.Name);
            }
        }

        private void OnSkinChanged(object? sender, EventArgs e)
        {
            // Update selected theme when skin changes externally
            try
            {
                LoadCurrentTheme();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update quick switcher after skin change");
            }
        }

        private static string GetThemeDescription(string themeName)
        {
            return themeName switch
            {
                "Dark" => "Professional dark theme",
                "Light" => "Clean light theme",
                "Ocean Blue" => "Deep blue ocean theme",
                "Forest Green" => "Nature-inspired green",
                "Purple Haze" => "Rich purple theme",
                "High Contrast" => "Maximum contrast",
                "Cyberpunk" => "Futuristic neon theme",
                _ => "Custom theme"
            };
        }

        /// <summary>
        /// Releases the resources used by the <see cref="QuickThemeSwitcherViewModel"/> class.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the method is being called explicitly to release managed resources.
        /// If <c>true</c>, managed resources are released; otherwise, only unmanaged resources are released.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SkinManager.Instance.SkinChanged -= OnSkinChanged;
            }
            base.Dispose(disposing);
        }
    }
}