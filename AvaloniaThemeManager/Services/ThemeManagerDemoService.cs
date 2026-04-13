using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Utility;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging;

namespace AvaloniaThemeManager.Services;

internal sealed class ThemeManagerDemoService : IThemeManagerDemoService
{
    private readonly ISkinManager _skinManager;
    private readonly ILogger _logger;
    private readonly IDialogService? _dialogService;

    public ThemeManagerDemoService(ISkinManager skinManager, ILogger logger, IDialogService? dialogService)
    {
        _skinManager = skinManager ?? throw new ArgumentNullException(nameof(skinManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogService = dialogService;
    }

    public async Task<string> OpenThemeSettingsAsync()
    {
        try
        {
            var mainWindow = WindowTools.TryGetMainWindow();
            if (mainWindow == null)
            {
                await ShowWarningAsync("Dialog Warning", "Main window is not available.");
                return "Unable to open theme settings";
            }

            var dialog = new ThemeSettingsDialog(_skinManager, _logger);
            await dialog.ShowDialog(mainWindow);
            return "Theme settings dialog closed";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open theme settings dialog");
            await ShowErrorAsync("Dialog Error", "Failed to open theme settings dialog.", ex);
            return "Failed to open theme settings";
        }
    }

    public async Task<string> ExportThemeAsync()
    {
        try
        {
            var currentSkin = _skinManager.CurrentSkin;
            if (currentSkin == null)
            {
                await ShowWarningAsync("Export Warning", "No theme is currently active to export.");
                return "Theme export skipped";
            }

            var mainWindow = WindowTools.TryGetMainWindow();
            if (mainWindow?.StorageProvider == null)
            {
                await ShowErrorAsync("Storage Error", "Storage provider is not available.");
                return "Theme export unavailable";
            }

            var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export Current Theme",
                FileTypeChoices =
                [
                    new FilePickerFileType("Theme Files")
                    {
                        Patterns = ["*.json"],
                        MimeTypes = ["application/json"]
                    },
                    FilePickerFileTypes.All
                ],
                SuggestedFileName = $"{currentSkin.Name}_Theme.json",
                DefaultExtension = "json"
            });

            if (result == null)
            {
                return "Theme export canceled";
            }

            var success = await ThemeImportExport.ExportThemeAsync(
                currentSkin,
                result.Path.LocalPath,
                $"Exported from Theme Manager Demo on {DateTime.Now:yyyy-MM-dd}",
                "Theme Manager Demo User");

            if (!success)
            {
                await ShowErrorAsync("Export Failed", "Failed to export the current theme. Please check the file path and permissions.");
                return "Theme export failed";
            }

            await ShowInfoAsync("Export Successful", $"Theme '{currentSkin.Name}' has been exported successfully.");
            return $"Theme exported: {currentSkin.Name}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during theme export");
            await ShowErrorAsync("Export Error", "An error occurred while exporting the theme.", ex);
            return "Theme export failed";
        }
    }

    public async Task<string> ImportThemeAsync()
    {
        try
        {
            var mainWindow = WindowTools.TryGetMainWindow();
            if (mainWindow?.StorageProvider == null)
            {
                await ShowErrorAsync("Storage Error", "Storage provider is not available.");
                return "Theme import unavailable";
            }

            var result = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import Theme",
                FileTypeFilter =
                [
                    new FilePickerFileType("Theme Files")
                    {
                        Patterns = ["*.json"],
                        MimeTypes = ["application/json"]
                    },
                    FilePickerFileTypes.All
                ],
                AllowMultiple = false
            });

            if (result.Count == 0)
            {
                return "Theme import canceled";
            }

            var importResult = await ThemeImportExport.ImportThemeAsync(result[0].Path.LocalPath);
            if (!importResult.Success || importResult.Theme == null)
            {
                if (importResult.Warnings.Any())
                {
                    await ShowValidationErrorsAsync(
                        "Import Issues",
                        [importResult.ErrorMessage ?? "Unknown error"],
                        importResult.Warnings);
                }
                else
                {
                    await ShowErrorAsync("Import Failed", importResult.ErrorMessage ?? "Failed to import the theme file.");
                }

                return "Theme import failed";
            }

            _skinManager.RegisterSkin(importResult.Theme.Name, importResult.Theme);
            _skinManager.ApplySkin(importResult.Theme.Name);
            await ShowInfoAsync("Import Successful", $"Theme '{importResult.Theme.Name}' has been imported successfully.");
            return $"Theme imported: {importResult.Theme.Name}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during theme import");
            await ShowErrorAsync("Import Error", "An error occurred while importing the theme.", ex);
            return "Theme import failed";
        }
    }

    public async Task<string> ShowValidationDemoAsync()
    {
        try
        {
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

            await ShowValidationErrorsAsync("Theme Validation Demo", sampleErrors, sampleWarnings);
            return "Validation demo shown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show validation demo dialog");
            await ShowErrorAsync("Demo Error", "Failed to show validation demo dialog.", ex);
            return "Validation demo failed";
        }
    }

    public async Task<string> ShowErrorDemoAsync()
    {
        try
        {
            var exception = new InvalidOperationException(
                "Sample exception for demonstration purposes. This shows how technical details are presented to users.");

            await ShowErrorAsync(
                "Demo Error Dialog",
                "This is a demonstration of the error dialog functionality. In a real scenario, this would show actual error information to help users understand and resolve issues.",
                exception);

            return "Error demo shown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show error demo dialog");
            await ShowErrorAsync("Demo Error", "Failed to show error demo dialog.", ex);
            return "Error demo failed";
        }
    }

    public async Task<string> ShowConfirmationDemoAsync()
    {
        try
        {
            var result = await ShowConfirmationAsync(
                "Demo Confirmation",
                "This is a demonstration of the confirmation dialog. Would you like to proceed with this demo action?",
                "Yes, Proceed",
                "Cancel");

            if (result)
            {
                await ShowInfoAsync("Demo Result", "You confirmed the demo action!");
                return "Confirmation demo confirmed";
            }

            await ShowInfoAsync("Demo Result", "You cancelled the demo action.");
            return "Confirmation demo cancelled";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show confirmation demo dialog");
            await ShowErrorAsync("Demo Error", "Failed to show confirmation demo dialog.", ex);
            return "Confirmation demo failed";
        }
    }

    private async Task ShowErrorAsync(string title, string message, Exception? exception = null)
    {
        if (_dialogService != null)
        {
            await _dialogService.ShowErrorAsync(title, message, exception);
            return;
        }

        var mainWindow = WindowTools.TryGetMainWindow();
        if (mainWindow == null)
        {
            _logger.LogWarning("Unable to show error dialog because no main window is available");
            return;
        }

        var dialog = new ErrorDialog
        {
            Title = title,
            Message = message,
            Exception = exception
        };

        await dialog.ShowDialog(mainWindow);
    }

    private async Task ShowWarningAsync(string title, string message)
    {
        if (_dialogService != null)
        {
            await _dialogService.ShowWarningAsync(title, message);
            return;
        }

        await ShowNotificationAsync(title, message, NotificationDialogType.Warning);
    }

    private async Task ShowInfoAsync(string title, string message)
    {
        if (_dialogService != null)
        {
            await _dialogService.ShowInfoAsync(title, message);
            return;
        }

        await ShowNotificationAsync(title, message, NotificationDialogType.Information);
    }

    private async Task ShowValidationErrorsAsync(string title, IEnumerable<string> errors, IEnumerable<string> warnings)
    {
        if (_dialogService != null)
        {
            await _dialogService.ShowValidationErrorsAsync(title, errors, warnings);
            return;
        }

        var mainWindow = WindowTools.TryGetMainWindow();
        if (mainWindow == null)
        {
            _logger.LogWarning("Unable to show validation dialog because no main window is available");
            return;
        }

        var dialog = new ValidationErrorDialog
        {
            Title = title
        };
        dialog.SetValidationResults(errors, warnings);
        await dialog.ShowDialog(mainWindow);
    }

    private async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText)
    {
        if (_dialogService != null)
        {
            return await _dialogService.ShowConfirmationAsync(title, message, confirmText, cancelText);
        }

        var mainWindow = WindowTools.TryGetMainWindow();
        if (mainWindow == null)
        {
            _logger.LogWarning("Unable to show confirmation dialog because no main window is available");
            return false;
        }

        var dialog = new ConfirmationDialog
        {
            Title = title,
            Message = message,
            ConfirmText = confirmText,
            CancelText = cancelText
        };

        return await dialog.ShowDialog(mainWindow) == true;
    }

    private async Task ShowNotificationAsync(string title, string message, NotificationDialogType dialogType)
    {
        var mainWindow = WindowTools.TryGetMainWindow();
        if (mainWindow == null)
        {
            _logger.LogWarning("Unable to show notification dialog because no main window is available");
            return;
        }

        var dialog = new NotificationDialog
        {
            Title = title,
            Message = message,
            DialogType = dialogType
        };

        await dialog.ShowDialog(mainWindow);
    }
}
