using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using AvaloniaThemeManager.Views;

namespace DemoApplication.Views
{
    /// <summary>
    /// Comprehensive demo window showcasing all AvaloniaThemeManager functionality.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ThemeSettingsViewModel _viewModel;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Initializes a new instance of the ThemeManagerDemoView class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize ViewModel and services
            _viewModel = new ThemeSettingsViewModel();
            _dialogService = new DialogService(
                Microsoft.Extensions.Logging.Abstractions.NullLogger<DialogService>.Instance);

            DataContext = _viewModel;

            // Wire up event handlers
            WireUpEventHandlers();

            // Subscribe to theme changes to update status bar
            SkinManager.Instance.SkinChanged += OnThemeChanged;

            // Initialize status bar
            UpdateCurrentThemeStatus();
        }

        private void WireUpEventHandlers()
        {
            // Dialog demo buttons
            var showInfoButton = this.FindControl<Button>("ShowInfoButton");
            var showSuccessButton = this.FindControl<Button>("ShowSuccessButton");
            var showThemeSettingsButton = this.FindControl<Button>("ShowThemeSettingsButton");
            var showWarningButton = this.FindControl<Button>("ShowWarningButton");
            var showConfirmationButton = this.FindControl<Button>("ShowConfirmationButton");
            var showValidationButton = this.FindControl<Button>("ShowValidationButton");
            var showErrorButton = this.FindControl<Button>("ShowErrorButton");
            var showExceptionButton = this.FindControl<Button>("ShowExceptionButton");
            var showCriticalErrorButton = this.FindControl<Button>("ShowCriticalErrorButton");

            // Theme management buttons
            var resetThemeButton = this.FindControl<Button>("ResetThemeButton");
            var openThemeSettingsButton = this.FindControl<Button>("OpenThemeSettingsButton");
            var exportThemeButton = this.FindControl<Button>("ExportThemeButton");
            var importThemeButton = this.FindControl<Button>("ImportThemeButton");
            var exportPackButton = this.FindControl<Button>("ExportPackButton");
            var validateThemeButton = this.FindControl<Button>("ValidateThemeButton");
            var checkAccessibilityButton = this.FindControl<Button>("CheckAccessibilityButton");
            var contrastTestButton = this.FindControl<Button>("ContrastTestButton");
            var createThemeButton = this.FindControl<Button>("CreateThemeButton");
            var inheritanceButton = this.FindControl<Button>("InheritanceButton");
            var typographyButton = this.FindControl<Button>("TypographyButton");

            // Wire up dialog demo events
            if (showInfoButton != null)
                showInfoButton.Click += ShowInfoButton_Click;
            if (showSuccessButton != null)
                showSuccessButton.Click += ShowSuccessButton_Click;
            if (showThemeSettingsButton != null)
                showThemeSettingsButton.Click += ShowThemeSettingsButton_Click;
            if (showWarningButton != null)
                showWarningButton.Click += ShowWarningButton_Click;
            if (showConfirmationButton != null)
                showConfirmationButton.Click += ShowConfirmationButton_Click;
            if (showValidationButton != null)
                showValidationButton.Click += ShowValidationButton_Click;
            if (showErrorButton != null)
                showErrorButton.Click += ShowErrorButton_Click;
            if (showExceptionButton != null)
                showExceptionButton.Click += ShowExceptionButton_Click;
            if (showCriticalErrorButton != null)
                showCriticalErrorButton.Click += ShowCriticalErrorButton_Click;

            // Wire up theme management events
            if (resetThemeButton != null)
                resetThemeButton.Click += ResetThemeButton_Click;
            if (openThemeSettingsButton != null)
                openThemeSettingsButton.Click += OpenThemeSettingsButton_Click;
            if (exportThemeButton != null)
                exportThemeButton.Click += ExportThemeButton_Click;
            if (importThemeButton != null)
                importThemeButton.Click += ImportThemeButton_Click;
            if (exportPackButton != null)
                exportPackButton.Click += ExportPackButton_Click;
            if (validateThemeButton != null)
                validateThemeButton.Click += ValidateThemeButton_Click;
            if (checkAccessibilityButton != null)
                checkAccessibilityButton.Click += CheckAccessibilityButton_Click;
            if (contrastTestButton != null)
                contrastTestButton.Click += ContrastTestButton_Click;
            if (createThemeButton != null)
                createThemeButton.Click += CreateThemeButton_Click;
            if (inheritanceButton != null)
                inheritanceButton.Click += InheritanceButton_Click;
            if (typographyButton != null)
                typographyButton.Click += TypographyButton_Click;
        }

        #region Dialog Demo Event Handlers

        private async void ShowInfoButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Information Demo",
                "This is an example of an information dialog. It's used to display helpful information to the user without requiring any specific action.");
        }

        private async void ShowSuccessButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Operation Successful",
                "Your theme has been applied successfully! All changes take effect immediately across the application.");
        }

        private async void ShowThemeSettingsButton_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new ThemeSettingsDialog();
            await dialog.ShowDialog(this);
        }

        private async void ShowWarningButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowWarningAsync(
                "Warning Demo",
                "This is a warning dialog. It alerts users to potential issues that don't prevent operation but should be noted.");
        }

        private async void ShowConfirmationButton_Click(object? sender, RoutedEventArgs e)
        {
            var result = await _dialogService.ShowConfirmationAsync(
                "Confirmation Demo",
                "Are you sure you want to reset all theme settings to their default values? This action cannot be undone.",
                "Reset Settings",
                "Keep Current");

            await _dialogService.ShowInfoAsync(
                "Confirmation Result",
                $"You selected: {(result ? "Reset Settings" : "Keep Current")}");
        }

        private async void ShowValidationButton_Click(object? sender, RoutedEventArgs e)
        {
            var errors = new List<string>
            {
                "Primary text contrast ratio (2.1:1) fails WCAG AA minimum (4.5:1)",
                "Theme name contains invalid characters"
            };

            var warnings = new List<string>
            {
                "Border color has low contrast against primary background",
                "Font size progression could be improved for better visual hierarchy",
                "Theme name is quite long and may affect UI display"
            };

            await _dialogService.ShowValidationErrorsAsync(
                "Theme Validation Results",
                errors,
                warnings);
        }

        private async void ShowErrorButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowErrorAsync(
                "Error Demo",
                "This demonstrates how errors are displayed. This would typically be shown when an operation fails.");
        }

        private async void ShowExceptionButton_Click(object? sender, RoutedEventArgs e)
        {
            var exception = new InvalidOperationException("Sample exception for demonstration purposes");

            await _dialogService.ShowErrorAsync(
                "Exception Demo",
                "An unexpected error occurred while processing your request. Technical details are available in the expandable section below.",
                exception);
        }

        private async void ShowCriticalErrorButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowErrorAsync(
                "Critical System Error",
                "A critical error has occurred that prevents the application from continuing. Please save your work and restart the application.");
        }

        #endregion

        #region Theme Management Event Handlers

        private void ResetThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel.ResetToDefault();
        }

        private async void OpenThemeSettingsButton_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new ThemeSettingsDialog();
            await dialog.ShowDialog(this);
        }
        
        

        private async void ExportThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var path = string.Empty;
                var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Export Theme",
                    SuggestedFileName = "theme.json",
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("Theme Files") { Patterns = new[] { "*.json" } },
                        new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                    }
                });

                if (file is not null)
                {
                    path = file.Path.LocalPath;
                    // use `path` or open stream via `file.OpenWriteAsync()`
                }

                if (!string.IsNullOrEmpty(path))
                {
                    var currentSkin = SkinManager.Instance.CurrentSkin;
                    if (currentSkin != null)
                    {
                        var success = await ThemeImportExport.ExportThemeAsync(
                            currentSkin,
                            path,
                            "Exported from AvaloniaThemeManager Demo",
                            Environment.UserName);

                        if (success)
                        {
                            await _dialogService.ShowInfoAsync(
                                "Export Successful",
                                $"Theme '{currentSkin.Name}' has been exported successfully to:\n{path}");
                        }
                        else
                        {
                            await _dialogService.ShowErrorAsync(
                                "Export Failed",
                                "Failed to export the theme. Please check file permissions and try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Export Error", "An error occurred during theme export.", ex);
            }
        }

        private async void ImportThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Import Theme File",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                new FilePickerFileType("Theme Files") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
            }
                });

                if (files?.Count > 0)
                {
                    var file = files[0];

                    // Use file.Path.LocalPath or stream
                    var path = file.TryGetLocalPath();

                    if (!string.IsNullOrEmpty(path))
                    {
                        var importResult = await ThemeImportExport.ImportThemeAsync(path);

                        if (importResult.Success && importResult.Theme != null)
                        {
                            SkinManager.Instance.RegisterSkin(importResult.Theme.Name, importResult.Theme);
                            SkinManager.Instance.ApplySkin(importResult.Theme.Name);

                            await _dialogService.ShowInfoAsync(
                                "Import Successful",
                                $"Theme '{importResult.Theme.Name}' has been imported and applied successfully!");
                        }
                        else
                        {
                            await _dialogService.ShowErrorAsync(
                                "Import Failed",
                                importResult.ErrorMessage ?? "Unknown error occurred during import.");
                        }
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(
                            "Import Error",
                            "Unable to access the selected file's path.");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Import Error", "An error occurred during theme import.", ex);
            }
        }


        private async void ExportPackButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var path = string.Empty;
                var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Export Theme",
                    SuggestedFileName = "theme.json",
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("Theme Files") { Patterns = new[] { "*.json" } },
                        new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                    }
                });

                if (file is not null)
                {
                    path = file.Path.LocalPath;
                    // use `path` or open stream via `file.OpenWriteAsync()`
                }

                if (!string.IsNullOrEmpty(path))
                {
                    var allThemes = new Dictionary<string, Skin>();
                    var themeNames = SkinManager.Instance.GetAvailableSkinNames();

                    foreach (var themeName in themeNames)
                    {
                        var skin = SkinManager.Instance.GetSkin(themeName);
                        if (skin != null)
                        {
                            allThemes[themeName] = skin;
                        }
                    }

                    var success = await ThemeImportExport.ExportThemePackAsync(
                        allThemes,
                        path,
                        "AvaloniaThemeManager Complete Pack",
                        "Complete collection of all available themes");

                    if (success)
                    {
                        await _dialogService.ShowInfoAsync(
                            "Export Successful",
                            $"Theme pack with {allThemes.Count} themes has been exported successfully!");
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(
                            "Export Failed",
                            "Failed to export the theme pack. Please check file permissions and try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Export Error", "An error occurred during theme pack export.", ex);
            }
        }

        private async void ValidateThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var currentSkin = SkinManager.Instance.CurrentSkin;
                if (currentSkin != null)
                {
                    var validator = new ThemeValidator();
                    var validationResult = validator.ValidateTheme(currentSkin);

                    if (validationResult.IsValid)
                    {
                        await _dialogService.ShowInfoAsync(
                            "Validation Successful",
                            $"Theme '{currentSkin.Name}' passed all validation checks!");
                    }
                    else
                    {
                        await _dialogService.ShowValidationErrorsAsync(
                            $"Validation Results for '{currentSkin.Name}'",
                            validationResult.Errors,
                            validationResult.Warnings);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Validation Error", "An error occurred during theme validation.", ex);
            }
        }

        private async void CheckAccessibilityButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var currentSkin = SkinManager.Instance.CurrentSkin;
                if (currentSkin != null)
                {
                    var validator = new ThemeValidator();
                    var primaryContrast = validator.CalculateContrastRatio(currentSkin.PrimaryTextColor, currentSkin.PrimaryBackground);
                    var secondaryContrast = validator.CalculateContrastRatio(currentSkin.SecondaryTextColor, currentSkin.SecondaryBackground);

                    var message = $"Accessibility Analysis for '{currentSkin.Name}':\n\n" +
                                  $"Primary Text Contrast: {primaryContrast:F2}:1 " +
                                  $"({(primaryContrast >= 4.5 ? "✓ WCAG AA" : "✗ Below WCAG AA")})\n" +
                                  $"Secondary Text Contrast: {secondaryContrast:F2}:1 " +
                                  $"({(secondaryContrast >= 3.0 ? "✓ Acceptable" : "✗ Too Low")})\n\n" +
                                  $"Font Sizes:\n" +
                                  $"Small: {currentSkin.FontSizeSmall}px " +
                                  $"({(currentSkin.FontSizeSmall >= 12 ? "✓" : "✗ Too Small")})\n" +
                                  $"Medium: {currentSkin.FontSizeMedium}px\n" +
                                  $"Large: {currentSkin.FontSizeLarge}px";

                    await _dialogService.ShowInfoAsync("Accessibility Check", message);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Accessibility Check Error", "An error occurred during accessibility analysis.", ex);
            }
        }

        private async void ContrastTestButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Color Contrast Test",
                "Use this demo interface to test color contrast by switching between different themes and observing text readability. " +
                "Pay attention to:\n\n" +
                "• Primary text on backgrounds\n" +
                "• Secondary text visibility\n" +
                "• Button text contrast\n" +
                "• Icon visibility\n" +
                "• Status color differentiation");
        }

        private async void CreateThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Custom Theme Creation",
                "To create a custom theme:\n\n" +
                "1. Create a new Skin instance\n" +
                "2. Set colors and properties\n" +
                "3. Register with SkinManager\n" +
                "4. Apply using ApplySkin()\n\n" +
                "Example:\n" +
                "var customSkin = new Skin { Name = \"MyTheme\" };\n" +
                "SkinManager.Instance.RegisterSkin(\"MyTheme\", customSkin);");
        }

        private async void InheritanceButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Theme Inheritance",
                "Theme inheritance allows you to:\n\n" +
                "• Create variants of existing themes\n" +
                "• Override specific properties\n" +
                "• Maintain consistency across related themes\n\n" +
                "Use InheritableSkin class and ThemeInheritanceManager for advanced inheritance scenarios.");
        }

        private async void TypographyButton_Click(object? sender, RoutedEventArgs e)
        {
            await _dialogService.ShowInfoAsync(
                "Typography System",
                "Advanced typography features include:\n\n" +
                "• Font family management\n" +
                "• Typography scale system\n" +
                "• Line height and letter spacing\n" +
                "• Accessibility font sizing\n\n" +
                "Use AdvancedSkin and AdvancedSkinManager for enhanced typography control.");
        }

        #endregion

        #region Theme Change Handling

        private void OnThemeChanged(object? sender, EventArgs e)
        {
            UpdateCurrentThemeStatus();
        }

        private void UpdateCurrentThemeStatus()
        {
            var statusText = this.FindControl<TextBlock>("CurrentThemeStatus");
            if (statusText != null)
            {
                var currentSkin = SkinManager.Instance.CurrentSkin;
                statusText.Text = $"Current Theme: {currentSkin?.Name ?? "Unknown"}";
            }
        }

        #endregion

        #region Cleanup

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            SkinManager.Instance.SkinChanged -= OnThemeChanged;

            // Dispose of ViewModel if it implements IDisposable
            _viewModel?.Dispose();

            base.OnClosed(e);
        }

        #endregion
    }
}