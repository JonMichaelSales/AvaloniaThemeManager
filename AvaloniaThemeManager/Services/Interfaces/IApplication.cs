using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;

namespace AvaloniaThemeManager.Services.Interfaces
{
    /// <summary>
    /// Provides access to the Avalonia application resources, lifetime, and styles needed by the theme manager.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Gets the application resource dictionary.
        /// </summary>
        IResourceDictionary Resources { get; }

        /// <summary>
        /// Gets the current application lifetime, if one is available.
        /// </summary>
        IApplicationLifetime? ApplicationLifetime { get; }

        /// <summary>
        /// Gets the application style collection.
        /// </summary>
        IStylesCollection AppStyles { get; }
    }
}
