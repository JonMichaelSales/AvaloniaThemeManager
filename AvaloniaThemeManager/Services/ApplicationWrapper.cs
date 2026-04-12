using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using AvaloniaThemeManager.Services.Interfaces;
using Avalonia;

namespace AvaloniaThemeManager.Services
{
    /// <summary>
    /// Wraps the current Avalonia <see cref="Application"/> for theme-manager access.
    /// </summary>
    public class ApplicationWrapper : IApplication
    {
        private readonly Application _application;
        private readonly IStylesCollection _stylesWrapper;

        /// <summary>
        /// Initializes a wrapper around <see cref="Application.Current"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Application.Current"/> is <c>null</c>.</exception>
        public ApplicationWrapper()
        {
            _application = Application.Current ?? throw new InvalidOperationException("Application.Current must not be null.");
            _stylesWrapper = new AvaloniaStylesWrapper(_application.Styles);
        }

        /// <summary>
        /// Initializes a wrapper around the provided Avalonia application instance.
        /// </summary>
        /// <param name="application">The application instance to wrap.</param>
        public ApplicationWrapper(Application application)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));
            _stylesWrapper = new AvaloniaStylesWrapper(application.Styles);
        }

        /// <inheritdoc />
        public IResourceDictionary Resources => _application.Resources;

        /// <inheritdoc />
        public IApplicationLifetime? ApplicationLifetime => _application.ApplicationLifetime;

        /// <inheritdoc />
        public IStylesCollection AppStyles => _stylesWrapper;
    }
}
