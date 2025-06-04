using Avalonia.Controls;
using AvaloniaThemeManager.Services.Interfaces;
using Microsoft.Extensions.Logging;
using AvaloniaThemeManager.Views;
using AvaloniaThemeManager.Utility;

namespace AvaloniaThemeManager.Services
{
    /// <summary>
    /// Service for displaying error dialogs and managing user notifications.
    /// </summary>
    

    /// <summary>
    /// Implementation of error dialog service using Avalonia dialogs.
    /// </summary>
    public class ErrorDialogService : IErrorDialogService
    {
        private readonly ILogger<ErrorDialogService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ErrorDialogService(ILogger<ErrorDialogService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public async Task ShowErrorAsync(string title, string message, Exception? exception = null)
        {
            _logger.LogError(exception, "Error dialog shown: {Title} - {Message}", title, message);

            var dialog = new ErrorDialog
            {
                Title = title,
                Message = message,
                Exception = exception
            };

            if (WindowTools.GetMainWindow() is Window mainWindow)
            {
                await dialog.ShowDialog(mainWindow);
            }
            else
            {
                await dialog.ShowDialog<object?>(null!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public async Task ShowWarningAsync(string title, string message)
        {
            _logger.LogWarning("Warning dialog shown: {Title} - {Message}", title, message);

            var dialog = new Views.NotificationDialog
            {
                Title = title,
                Message = message,
                DialogType = NotificationDialogType.Warning
            };

            if (WindowTools.GetMainWindow() is Window mainWindow)
            {
                await dialog.ShowDialog(mainWindow);
            }
            else
            {
                await dialog.ShowDialog<object?>(null!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public async Task ShowInfoAsync(string title, string message)
        {
            _logger.LogInformation("Info dialog shown: {Title} - {Message}", title, message);

            var dialog = new Views.NotificationDialog
            {
                Title = title,
                Message = message,
                DialogType = NotificationDialogType.Information
            };

            if (WindowTools.GetMainWindow() is { } mainWindow)
            {
                await dialog.ShowDialog(mainWindow);
            }
            else
            {
                await dialog.ShowDialog<object?>(null!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="errors"></param>
        /// <param name="warnings"></param>
        public async Task ShowValidationErrorsAsync(string title, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            var errorList = errors.ToList();
            var warningList = warnings.ToList();

            _logger.LogWarning("Validation dialog shown: {Title} - {ErrorCount} errors, {WarningCount} warnings",
                title, errorList.Count, warningList.Count);

            var dialog = new ValidationErrorDialog
            {
                Title = title,
                Errors = errorList,
                Warnings = warningList
            };

            if (WindowTools.GetMainWindow() is { } mainWindow)
            {
                await dialog.ShowDialog(mainWindow);
            }
            else
            {
                await dialog.ShowDialog<object?>(null!);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="confirmText"></param>
        /// <param name="cancelText"></param>
        /// <returns></returns>
        public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Yes", string cancelText = "No")
        {
            _logger.LogDebug("Confirmation dialog shown: {Title} - {Message}", title, message);

            var dialog = new ConfirmationDialog
            {
                Title = title,
                Message = message,
                ConfirmText = confirmText,
                CancelText = cancelText
            };

            bool? result;
            if (WindowTools.TryGetMainWindow() is { } mainWindow)
            {
                result = await dialog.ShowDialog<bool?>(mainWindow);
            }
            else
            {
                dialog.Show();
                return true;
            }

            return result == true;
        }


    }

    /// <summary>
    /// Enumeration for different types of notification dialogs.
    /// </summary>
    public enum NotificationDialogType
    {
        /// <summary>
        /// 
        /// </summary>
        Information,
        /// <summary>
        /// 
        /// </summary>
        Warning,
        /// <summary>
        /// 
        /// </summary>
        Error
    }
}