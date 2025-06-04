using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Utility;
using AvaloniaThemeManager.ViewModels;
using Microsoft.Extensions.Logging;

namespace AvaloniaThemeManager.Views
{
    /// <summary>
    /// Comprehensive demo view showcasing all AvaloniaThemeManager functionality.
    /// Demonstrates theme switching, control themes, icons, typography, and interactive features.
    /// </summary>
    /// <remarks>
    /// This view serves as both a demonstration and testing interface for all theme manager capabilities.
    /// It follows MVVM patterns while providing direct interaction with theme management services.
    /// </remarks>
    public partial class ThemeManagerDemoView : UserControl
    {
        private readonly ILogger _logger;
        private readonly IDialogService? _errorDialogService;

        /// <summary>
        /// Initializes a new instance of the ThemeManagerDemoView with default services.
        /// </summary>
        public ThemeManagerDemoView() : this(
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance,
            null)
        {

            
        }

        /// <summary>
        /// Initializes a new instance of the ThemeManagerDemoView with dependency injection support.
        /// </summary>
        /// <param name="logger">Logger instance for tracking demo interactions and errors</param>
        /// <param name="errorDialogService">Service for displaying error dialogs (optional)</param>
        public ThemeManagerDemoView(ILogger logger, IDialogService? errorDialogService = null)
        {
            _logger = logger;
            _errorDialogService = errorDialogService;

            InitializeComponent();
            InitializeDemo();
        }

        /// <summary>
        /// Initializes the demo view with proper data context and event handlers.
        /// </summary>
        private void InitializeDemo()
        {
            try
            {
                // Set up the data context with the QuickThemeSwitcher ViewModel
                DataContext = new QuickThemeSwitcherViewModel(_logger);

                // Subscribe to theme changes for logging
                SkinManager.Instance.SkinChanged += OnThemeChanged;

                _logger.LogInformation("ThemeManagerDemoView initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize ThemeManagerDemoView");
                ShowErrorMessage("Initialization Error", "Failed to initialize the demo view", ex);
            }
        }

        /// <summary>
        /// Handles theme change events for logging and demo purposes.
        /// </summary>
        private void OnThemeChanged(object? sender, EventArgs e)
        {
            try
            {
                var currentTheme = SkinManager.Instance.CurrentSkin?.Name ?? "Unknown";
                _logger.LogInformation("Theme changed to: {ThemeName}", currentTheme);
                                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling theme change event");
            }
        }

        #region Event Handlers

        /// <summary>
        /// Opens the theme settings dialog for comprehensive theme management.
        /// </summary>
        private void OpenThemeSettings_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Opening theme settings dialog");

                var mainWindow = WindowTools.GetMainWindow();
                if (mainWindow != null) new ThemeSettingsDialog().Show(mainWindow);

                _logger.LogInformation("Theme settings dialog closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open theme settings dialog");
                ShowErrorMessage("Dialog Error", "Failed to open theme settings dialog", ex);
            }
        }

        /// <summary>
        /// Demonstrates theme export functionality.
        /// </summary>
        /// <summary>
        /// Demonstrates theme export functionality using modern StorageProvider API.
        /// </summary>
        private async void ExportTheme_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Starting theme export demo");

                var currentSkin = SkinManager.Instance.CurrentSkin;
                if (currentSkin == null)
                {
                    ShowWarningMessage("Export Warning", "No theme is currently active to export.");
                    return;
                }

                var mainWindow = WindowTools.GetMainWindow();
                if (mainWindow?.StorageProvider == null)
                {
                    ShowErrorMessage("Storage Error", "Storage provider is not available.");
                    return;
                }

                // Define file type options
                var fileTypeChoices = new FilePickerFileType[]
                {
            new("Theme Files")
            {
                Patterns = new[] { "*.json" },
                MimeTypes = new[] { "application/json" }
            },
            FilePickerFileTypes.All
                };

                // Show save file picker
                var saveOptions = new FilePickerSaveOptions
                {
                    Title = "Export Current Theme",
                    FileTypeChoices = fileTypeChoices,
                    SuggestedFileName = $"{currentSkin.Name}_Theme.json",
                    DefaultExtension = "json"
                };

                var result = await mainWindow.StorageProvider.SaveFilePickerAsync(saveOptions);

                if (result != null)
                {
                    var filePath = result.Path.LocalPath;
                    var success = await ThemeImportExport.ExportThemeAsync(
                        currentSkin,
                        filePath,
                        $"Exported from Theme Manager Demo on {DateTime.Now:yyyy-MM-dd}",
                        "Theme Manager Demo User"
                    );

                    if (success)
                    {
                        _logger.LogInformation("Theme exported successfully to: {FilePath}", filePath);
                        ShowSuccessMessage("Export Successful", $"Theme '{currentSkin.Name}' has been exported successfully.");
                    }
                    else
                    {
                        _logger.LogError("Theme export failed to: {FilePath}", filePath);
                        ShowErrorMessage("Export Failed", "Failed to export the current theme. Please check the file path and permissions.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during theme export");
                ShowErrorMessage("Export Error", "An error occurred while exporting the theme", ex);
            }
        }

        /// <summary>
        /// Demonstrates theme import functionality using modern StorageProvider API.
        /// </summary>
        private async void ImportTheme_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Starting theme import demo");

                var mainWindow = WindowTools.GetMainWindow();
                if (mainWindow?.StorageProvider == null)
                {
                    ShowErrorMessage("Storage Error", "Storage provider is not available.");
                    return;
                }

                // Define file type options
                var fileTypeChoices = new FilePickerFileType[]
                {
            new("Theme Files")
            {
                Patterns = new[] { "*.json" },
                MimeTypes = new[] { "application/json" }
            },
            FilePickerFileTypes.All
                };

                // Show open file picker
                var openOptions = new FilePickerOpenOptions
                {
                    Title = "Import Theme",
                    FileTypeFilter = fileTypeChoices,
                    AllowMultiple = false
                };

                var result = await mainWindow.StorageProvider.OpenFilePickerAsync(openOptions);

                if (result.Count > 0)
                {
                    var filePath = result[0].Path.LocalPath;
                    var importResult = await ThemeImportExport.ImportThemeAsync(filePath);

                    if (importResult.Success && importResult.Theme != null)
                    {
                        // Register the imported theme
                        SkinManager.Instance.RegisterSkin(importResult.Theme.Name, importResult.Theme);

                        _logger.LogInformation("Theme imported successfully from: {FilePath}", filePath);
                        ShowSuccessMessage("Import Successful", $"Theme '{importResult.Theme.Name}' has been imported successfully.");

                        // Optionally apply the imported theme
                        SkinManager.Instance.ApplySkin(importResult.Theme.Name);
                    }
                    else
                    {
                        _logger.LogError("Theme import failed from: {FilePath}. Error: {Error}", filePath, importResult.ErrorMessage);

                        if (importResult.Warnings.Any())
                        {
                            await ShowValidationResults("Import Issues", new List<string> { importResult.ErrorMessage ?? "Unknown error" }, importResult.Warnings);
                        }
                        else
                        {
                            ShowErrorMessage("Import Failed", importResult.ErrorMessage ?? "Failed to import the theme file.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during theme import");
                ShowErrorMessage("Import Error", "An error occurred while importing the theme", ex);
            }
        }

        /// <summary>
        /// Shows validation results using the validation error dialog.
        /// </summary>
        private async Task ShowValidationResults(string title, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            try
            {
                var validationDialog = new ValidationErrorDialog
                {
                    Title = title
                };
                validationDialog.SetValidationResults(errors, warnings);
                await validationDialog.ShowDialog(WindowTools.GetMainWindow()!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show validation results dialog");
            }
        }



        /// <summary>
        /// Demonstrates the validation error dialog with sample validation results.
        /// </summary>
        private async void ShowValidationDemo_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Showing validation demo dialog");

                var validationDialog = new ValidationErrorDialog();

                // Create sample validation errors and warnings
                var sampleErrors = new List<string>
                {
                    "Primary text contrast ratio (3.2:1) is below WCAG AA standard (4.5:1)",
                    "Border color has insufficient contrast against secondary background (ratio: 1.4)",
                    "Theme name contains invalid characters"
                };

                var sampleWarnings = new List<string>
                {
                    "Primary text contrast ratio (5.1:1) is below WCAG AAA standard (7.0:1)",
                    "Medium and large font sizes are too similar for optimal visual hierarchy",
                    "Theme name is quite long (35 characters). Consider a shorter name for better UI display",
                    "Accent color is too similar to primary color (1.8:1). May not provide sufficient emphasis"
                };

                validationDialog.SetValidationResults(sampleErrors, sampleWarnings);
                await validationDialog.ShowDialog(WindowTools.GetMainWindow()!);

                _logger.LogInformation("Validation demo dialog closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show validation demo dialog");
                ShowErrorMessage("Demo Error", "Failed to show validation demo dialog", ex);
            }
        }

        /// <summary>
        /// Demonstrates the error dialog with a sample error.
        /// </summary>
        private async void ShowErrorDemo_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Showing error demo dialog");

                var errorDialog = new ErrorDialog
                {
                    Title = "Demo Error Dialog",
                    Message = "This is a demonstration of the error dialog functionality. In a real scenario, this would show actual error information to help users understand and resolve issues.",
                    Exception = new InvalidOperationException("Sample exception for demonstration purposes. This shows how technical details are presented to users.")
                };

                await errorDialog.ShowDialog(WindowTools.GetMainWindow()!);

                _logger.LogInformation("Error demo dialog closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show error demo dialog");
                ShowErrorMessage("Demo Error", "Failed to show error demo dialog", ex);
            }
        }

        /// <summary>
        /// Demonstrates the confirmation dialog functionality.
        /// </summary>
        private async void ShowConfirmationDemo_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Showing confirmation demo dialog");

                var confirmationDialog = new ConfirmationDialog
                {
                    Title = "Demo Confirmation",
                    Message = "This is a demonstration of the confirmation dialog. Would you like to proceed with this demo action?",
                    ConfirmText = "Yes, Proceed",
                    CancelText = "Cancel"
                };

                var result = await confirmationDialog.ShowDialog(WindowTools.GetMainWindow());

                var resultText = result == true ? "confirmed" : "cancelled";
                _logger.LogInformation("Confirmation demo result: {Result}", resultText);

                // Show the result to the user
                if (result == true)
                {
                    ShowSuccessMessage("Demo Result", "You confirmed the demo action!");
                }
                else
                {
                    ShowInfoMessage("Demo Result", "You cancelled the demo action.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show confirmation demo dialog");
                ShowErrorMessage("Demo Error", "Failed to show confirmation demo dialog", ex);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Shows an error message using the available error dialog service or a fallback dialog.
        /// </summary>
        private async void ShowErrorMessage(string title, string message, Exception? exception = null)
        {
            try
            {
                if (_errorDialogService != null)
                {
                    await _errorDialogService.ShowErrorAsync(title, message, exception);
                }
                else
                {
                    var errorDialog = new ErrorDialog
                    {
                        Title = title,
                        Message = message,
                        Exception = exception
                    };
                    await errorDialog.ShowDialog(WindowTools.GetMainWindow()!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show error dialog");
                // Last resort: try a simple message box if available
            }
        }

        /// <summary>
        /// Shows a warning message using the available dialog service or a fallback dialog.
        /// </summary>
        private async void ShowWarningMessage(string title, string message)
        {
            try
            {
                if (_errorDialogService != null)
                {
                    await _errorDialogService.ShowWarningAsync(title, message);
                }
                else
                {
                    var warningDialog = new NotificationDialog
                    {
                        Title = title,
                        Message = message,
                        DialogType = NotificationDialogType.Warning
                    };
                    await warningDialog.ShowDialog(WindowTools.GetMainWindow()!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show warning dialog");
            }
        }

        /// <summary>
        /// Shows an information message using the available dialog service or a fallback dialog.
        /// </summary>
        private async void ShowInfoMessage(string title, string message)
        {
            try
            {
                if (_errorDialogService != null)
                {
                    await _errorDialogService.ShowInfoAsync(title, message);
                }
                else
                {
                    var infoDialog = new NotificationDialog
                    {
                        Title = title,
                        Message = message,
                        DialogType = NotificationDialogType.Information
                    };
                    await infoDialog.ShowDialog(WindowTools.GetMainWindow()!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show info dialog");
            }
        }

        /// <summary>
        /// Shows a success message using a notification dialog.
        /// </summary>
        private async void ShowSuccessMessage(string title, string message)
        {
            try
            {
                var successDialog = new NotificationDialog
                {
                    Title = title,
                    Message = message,
                    DialogType = NotificationDialogType.Information // Using Information type styled as success
                };
                await successDialog.ShowDialog(WindowTools.GetMainWindow()!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to show success dialog");
            }
        }

        #endregion

    }
}