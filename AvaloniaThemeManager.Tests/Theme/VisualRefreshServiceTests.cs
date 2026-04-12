using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless.XUnit;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using Moq;

namespace AvaloniaThemeManager.Tests.Theme;

public class VisualRefreshServiceTests
{
    [AvaloniaFact]
    public void Refresh_WithDesktopWindows_DoesNotThrow()
    {
        var applicationMock = new Mock<IApplication>();
        var lifetimeMock = new Mock<IClassicDesktopStyleApplicationLifetime>();
        lifetimeMock.Setup(lifetime => lifetime.Windows).Returns(new List<Window>
        {
            new Window
            {
                Content = new StackPanel
                {
                    Children =
                    {
                        new Border(),
                        new ContentControl { Content = new Button() }
                    }
                }
            }
        });
        applicationMock.SetupGet(application => application.ApplicationLifetime).Returns(lifetimeMock.Object);

        var service = new VisualRefreshService(applicationMock.Object);

        var exception = Record.Exception(service.Refresh);

        Assert.Null(exception);
    }
}
