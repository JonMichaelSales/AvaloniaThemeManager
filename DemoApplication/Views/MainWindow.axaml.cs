using System;
using Avalonia.Controls;
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging.Abstractions;

namespace DemoApplication.Views;

/// <summary>
/// Thin host window for the library demo view.
/// </summary>
public partial class MainWindow : Window
{
    [Obsolete("Use the dependency-injected MainWindow(ThemeManagerDemoView) constructor instead.")]
    public MainWindow()
        : this(new ThemeManagerDemoView(
            AppBuilderExtensions.GetRequiredService<AvaloniaThemeManager.Theme.ISkinManager>(),
            NullLogger.Instance,
            AppBuilderExtensions.GetService<AvaloniaThemeManager.Services.Interfaces.IDialogService>()))
    {
    }

    public MainWindow(ThemeManagerDemoView demoView)
    {
        InitializeComponent();
        DemoViewHost.Content = demoView ?? throw new ArgumentNullException(nameof(demoView));
    }
}
