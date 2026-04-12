using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaThemeManager.Services.Interfaces;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Performs recursive visual invalidation for desktop windows after a theme change.
    /// </summary>
    public class VisualRefreshService : IVisualRefreshService
    {
        private readonly IApplication _application;

        /// <summary>
        /// Initializes a new visual refresh service.
        /// </summary>
        public VisualRefreshService(IApplication application)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <inheritdoc />
        public void Refresh()
        {
            if (_application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                foreach (var window in desktop.Windows)
                {
                    window.InvalidateVisual();
                    InvalidateRecursive(window);
                }
            }
        }

        private void InvalidateRecursive(Control control)
        {
            control.InvalidateVisual();

            if (control is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    InvalidateRecursive(child);
                }
            }
            else if (control is ContentControl contentControl && contentControl.Content is Control nestedControl)
            {
                InvalidateRecursive(nestedControl);
            }
            else if (control is ItemsControl itemsControl && itemsControl.ItemsPanelRoot is Control itemsPanel)
            {
                InvalidateRecursive(itemsPanel);
            }
        }
    }
}
