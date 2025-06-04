using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace AvaloniaThemeManager.Utility
{
    /// <summary>
    /// Provides utility methods for working with Avalonia <see cref="Window"/> instances,
    /// including retrieving the main application window.
    /// </summary>
    public static class WindowTools
    {
        /// <summary>
        /// Attempts to retrieve the main application <see cref="Window"/>.
        /// </summary>
        /// <returns>
        /// The main <see cref="Window"/> instance of the application if available; otherwise, <c>null</c>.
        /// </returns>
        public static Window? TryGetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the main application window.
        /// </summary>
        /// <returns>The main <see cref="Window"/> instance of the application.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the main window is not available.
        /// </exception>
        public static Window GetMainWindow()
        {
            return TryGetMainWindow() ?? throw new InvalidOperationException("Main window is not available");
        }
    }

}
