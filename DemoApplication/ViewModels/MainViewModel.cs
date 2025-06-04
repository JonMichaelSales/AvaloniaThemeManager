using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using AvaloniaThemeManager.Utility;
using ReactiveUI;

namespace DemoApplication.ViewModels
{
    public class MainWindowViewModel : AvaloniaThemeManager.ViewModels.ViewModelBase
    {
        private readonly Timer _progressTimer;
        private string _currentThemeName = "Dark";
        private string _sampleText = "This is sample text for demonstration";
        private bool _notificationsEnabled = true;
        private double _sliderValue = 65;
        private double _progressValue = 45;
        private string _statusText = "Ready";

        public MainWindowViewModel()
        {
            // Initialize sample data
            SampleData = new ObservableCollection<SampleDataItem>
            {
                new() { Name = "Revenue", Value = "$124,500", Status = "Up" },
                new() { Name = "Users", Value = "8,432", Status = "Stable" },
                new() { Name = "Conversion", Value = "3.2%", Status = "Down" },
                new() { Name = "Performance", Value = "98.5%", Status = "Up" },
                new() { Name = "Satisfaction", Value = "4.8/5", Status = "Up" }
            };

            // Initialize available themes
            LoadAvailableThemes();

            // Commands
            OpenThemeSettingsCommand = ReactiveCommand.CreateFromTask(OpenThemeSettingsAsync);
            ExportThemeCommand = ReactiveCommand.CreateFromTask(ExportThemeAsync);
            OpenDocumentationCommand = ReactiveCommand.Create(OpenDocumentation);
            OpenGitHubCommand = ReactiveCommand.Create(OpenGitHub);

            // Subscribe to theme changes
            SkinManager.Instance.SkinChanged += OnThemeChanged;
            UpdateCurrentThemeName();

            // Setup progress animation
            _progressTimer = new Timer(100);
            _progressTimer.Elapsed += OnProgressTimerElapsed;
            _progressTimer.Start();

            StatusText = "Demo application loaded successfully";
        }

        #region Properties

        public string CurrentThemeName
        {
            get => _currentThemeName;
            set => this.RaiseAndSetIfChanged(ref _currentThemeName, value);
        }

        public string SampleText
        {
            get => _sampleText;
            set => this.RaiseAndSetIfChanged(ref _sampleText, value);
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => this.RaiseAndSetIfChanged(ref _notificationsEnabled, value);
        }

        public double SliderValue
        {
            get => _sliderValue;
            set => this.RaiseAndSetIfChanged(ref _sliderValue, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set => this.RaiseAndSetIfChanged(ref _progressValue, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        public ObservableCollection<SampleDataItem> SampleData { get; }
        public ObservableCollection<ThemeInfo> AvailableThemes { get; } = new();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> OpenThemeSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportThemeCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenDocumentationCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenGitHubCommand { get; }

        #endregion

        #region Private Methods

        private void LoadAvailableThemes()
        {
            var themeManager = SkinManager.Instance;
            var themeNames = themeManager.GetAvailableSkinNames();

            AvailableThemes.Clear();
            foreach (var themeName in themeNames)
            {
                var skin = themeManager.GetSkin(themeName);
                if (skin != null)
                {
                    AvailableThemes.Add(new ThemeInfo
                    {
                        Name = themeName,
                        Description = GetThemeDescription(themeName),
                        PreviewColor = new Avalonia.Media.SolidColorBrush(skin.AccentColor)
                    });
                }
            }
        }

        private string GetThemeDescription(string themeName)
        {
            return themeName switch
            {
                "Dark" => "Professional dark theme with blue accents. Easy on the eyes for extended use.",
                "Light" => "Clean light theme with dark text. Perfect for bright environments.",
                "Ocean Blue" => "Deep blue theme inspired by ocean depths. Calming and focused.",
                "Forest Green" => "Nature-inspired green theme. Relaxing and earthy.",
                "Purple Haze" => "Rich purple theme with mystical vibes. Creative and bold.",
                "High Contrast" => "Maximum contrast for accessibility. Clear and distinct colors.",
                "Cyberpunk" => "Futuristic neon theme with hot pink accents. Edgy and modern.",
                _ => "Custom theme with unique styling and colors."
            };
        }

        private void OnThemeChanged(object? sender, EventArgs e)
        {
            UpdateCurrentThemeName();
            StatusText = $"Theme changed to {CurrentThemeName}";
        }

        private void UpdateCurrentThemeName()
        {
            var currentSkin = SkinManager.Instance.CurrentSkin;
            CurrentThemeName = currentSkin?.Name ?? "Unknown";
        }

        private void OnProgressTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Animate progress bar
            ProgressValue = (ProgressValue + 1) % 101;
        }

        private async Task OpenThemeSettingsAsync()
        {
            try
            {
                StatusText = "Opening theme settings...";

                // Create and show theme settings dialog
                var themeSettingsDialog = new AvaloniaThemeManager.Views.ThemeSettingsDialog();

                // Get the main window
                var mainWindow = WindowTools.GetMainWindow();
                if (mainWindow != null)
                {
                    await themeSettingsDialog.ShowDialog(mainWindow);
                    StatusText = "Theme settings closed";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error opening theme settings: {ex.Message}";
            }
        }

        private async Task ExportThemeAsync()
        {
            try
            {
                StatusText = "Exporting current theme...";

                // Simulate export process
                await Task.Delay(1000);

                StatusText = $"Successfully exported {CurrentThemeName} theme";
            }
            catch (Exception ex)
            {
                StatusText = $"Error exporting theme: {ex.Message}";
            }
        }

        private void OpenDocumentation()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://github.com/yourusername/AvaloniaThemeManager#readme",
                    UseShellExecute = true
                };
                Process.Start(psi);
                StatusText = "Opened documentation in browser";
            }
            catch (Exception ex)
            {
                StatusText = $"Error opening documentation: {ex.Message}";
            }
        }

        private void OpenGitHub()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://github.com/yourusername/AvaloniaThemeManager",
                    UseShellExecute = true
                };
                Process.Start(psi);
                StatusText = "Opened GitHub repository in browser";
            }
            catch (Exception ex)
            {
                StatusText = $"Error opening GitHub: {ex.Message}";
            }
        }

#endregion
    }

    public class SampleDataItem
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Status { get; set; } = "";
    }
}