using Avalonia.Controls;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using Microsoft.Extensions.Logging;

namespace AvaloniaThemeManager.Views;

/// <summary>
/// Comprehensive demo view showcasing all AvaloniaThemeManager functionality.
/// </summary>
public partial class ThemeManagerDemoView : UserControl
{
    private readonly bool _ownsViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeManagerDemoView"/> class using the configured service provider.
    /// </summary>
    [Obsolete("Use the dependency-injected ThemeManagerDemoView(ISkinManager, ILogger, IDialogService?) constructor instead.")]
    public ThemeManagerDemoView()
        : this(
            AvaloniaThemeManager.Extensions.AppBuilderExtensions.GetRequiredService<ISkinManager>(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance,
            AvaloniaThemeManager.Extensions.AppBuilderExtensions.GetService<IDialogService>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeManagerDemoView"/> class with explicit dependencies.
    /// </summary>
    /// <param name="skinManager">The theme manager used by the demo workflows.</param>
    /// <param name="logger">The logger used by the demo view model and service.</param>
    /// <param name="dialogService">The optional dialog service used for demo interactions.</param>
    public ThemeManagerDemoView(ISkinManager skinManager, ILogger logger, IDialogService? dialogService = null)
        : this(CreateViewModel(skinManager, logger, dialogService), ownsViewModel: true)
    {
    }

    internal ThemeManagerDemoView(ThemeManagerDemoViewModel viewModel, bool ownsViewModel = false)
    {
        InitializeComponent();

        _ownsViewModel = ownsViewModel;
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        QuickThemeSwitcherHost.Content = new Controls.QuickThemeSwitcher(viewModel.QuickThemeSwitcherViewModel);
        DetachedFromVisualTree += (_, _) =>
        {
            if (_ownsViewModel && DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        };
    }

    private static ThemeManagerDemoViewModel CreateViewModel(ISkinManager skinManager, ILogger logger, IDialogService? dialogService)
    {
        var quickThemeSwitcherViewModel = new QuickThemeSwitcherViewModel(skinManager, logger);
        var demoService = new ThemeManagerDemoService(skinManager, logger, dialogService);
        return new ThemeManagerDemoViewModel(skinManager, quickThemeSwitcherViewModel, demoService, logger);
    }
}
