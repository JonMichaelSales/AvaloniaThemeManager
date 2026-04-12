using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace AvaloniaThemeManager.Tests.Services;

public class DialogServiceTests
{
    [Fact]
    public async Task ShowConfirmationAsync_ReturnsFalse_WhenNoMainWindowIsAvailable()
    {
        var loggerMock = new Mock<ILogger<DialogService>>();
        var service = new DialogService(loggerMock.Object);

        var result = await service.ShowConfirmationAsync("Confirm", "Proceed?");

        Assert.False(result);
    }

    [Fact]
    public async Task ShowConfirmationAsync_LogsWarning_WhenNoMainWindowIsAvailable()
    {
        var loggerMock = new Mock<ILogger<DialogService>>();
        var service = new DialogService(loggerMock.Object);

        _ = await service.ShowConfirmationAsync("Confirm", "Proceed?");

        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("no main window is available", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, false)]
    public void MapConfirmationResult_UsesExplicitConfirmationOnly(bool? dialogResult, bool expected)
    {
        var actual = DialogService.MapConfirmationResult(dialogResult);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task MessageBox_ShowConfirmationAsync_ForwardsFailClosedBehavior()
    {
        var dialogServiceMock = new Mock<IDialogService>();
        dialogServiceMock
            .Setup(service => service.ShowConfirmationAsync("Forwarded Title", "Forwarded message", "Yes", "No"))
            .ReturnsAsync(false);

        var services = new ServiceCollection();
        services.AddSingleton(dialogServiceMock.Object);
        MessageBox.Initialize(services.BuildServiceProvider());

        var result = await MessageBox.ShowConfirmationAsync("Forwarded message", "Forwarded Title");

        Assert.False(result);
        dialogServiceMock.Verify(
            service => service.ShowConfirmationAsync("Forwarded Title", "Forwarded message", "Yes", "No"),
            Times.Once);
    }
}
