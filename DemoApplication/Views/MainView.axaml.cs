using System;
using Avalonia.Controls;
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging.Abstractions;

namespace DemoApplication.Views;

public partial class MainView : Window
{
    [Obsolete("Use the dependency-injected MainView(ThemeManagerDemoView) constructor instead.")]
    public MainView()
        : this(new ThemeManagerDemoView(
            AppBuilderExtensions.GetRequiredService<AvaloniaThemeManager.Theme.ISkinManager>(),
            NullLogger.Instance,
            AppBuilderExtensions.GetService<AvaloniaThemeManager.Services.Interfaces.IDialogService>()))
    {
    }

    public MainView(ThemeManagerDemoView demoView)
    {
        InitializeComponent();
        DemoViewHost.Content = demoView ?? throw new ArgumentNullException(nameof(demoView));
    }
}
