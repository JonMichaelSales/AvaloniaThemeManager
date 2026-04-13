using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using AvaloniaThemeManager.Views;
using DemoApplication.Views;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AvaloniaThemeManager.Tests.DemoApplication;

public sealed class MainWindowTests
{
    [AvaloniaFact]
    public void MainWindow_HostsInjectedDemoView_WithoutLookingUpMissingControls()
    {
        var skinManager = new Mock<AvaloniaThemeManager.Theme.ISkinManager>();
        var demoView = new ThemeManagerDemoView(skinManager.Object, NullLogger.Instance);
        var window = new MainWindow(demoView);

        var host = window.FindControl<ContentControl>("DemoViewHost");

        Assert.NotNull(host);
        Assert.Same(demoView, host!.Content);
    }
}
