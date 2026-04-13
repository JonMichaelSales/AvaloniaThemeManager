using Avalonia.Media;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reactive.Linq;

namespace AvaloniaThemeManager.Tests.ViewModels;

public sealed class ThemeManagerDemoViewModelTests
{
    [Fact]
    public void Constructor_InitializesVersionAndStatusProperties()
    {
        var skinManager = CreateSkinManagerMock().Object;
        var demoService = new FakeThemeManagerDemoService();
        using var quickThemeSwitcherViewModel = new QuickThemeSwitcherViewModel(skinManager, NullLogger.Instance);
        using var viewModel = new ThemeManagerDemoViewModel(skinManager, quickThemeSwitcherViewModel, demoService, NullLogger.Instance);

        Assert.StartsWith("Version ", viewModel.LibraryVersionText);
        Assert.StartsWith("Avalonia ", viewModel.AvaloniaVersionText);
        Assert.Equal("Ready - All theme components loaded successfully", viewModel.StatusText);
        Assert.Equal("Theme validation passed", viewModel.ValidationStatusText);
    }

    [Fact]
    public void Commands_UpdateStatusFromService()
    {
        var skinManager = CreateSkinManagerMock().Object;
        var demoService = new FakeThemeManagerDemoService
        {
            OpenThemeSettingsStatus = "Theme settings dialog closed"
        };

        using var quickThemeSwitcherViewModel = new QuickThemeSwitcherViewModel(skinManager, NullLogger.Instance);
        using var viewModel = new ThemeManagerDemoViewModel(skinManager, quickThemeSwitcherViewModel, demoService, NullLogger.Instance);

        viewModel.OpenThemeSettingsCommand.Execute().Wait();

        Assert.Equal("Theme settings dialog closed", viewModel.StatusText);
    }

    [Fact]
    public void SkinChanged_UpdatesStatusText()
    {
        var skinManagerMock = CreateSkinManagerMock();
        var demoService = new FakeThemeManagerDemoService();
        using var quickThemeSwitcherViewModel = new QuickThemeSwitcherViewModel(skinManagerMock.Object, NullLogger.Instance);
        using var viewModel = new ThemeManagerDemoViewModel(skinManagerMock.Object, quickThemeSwitcherViewModel, demoService, NullLogger.Instance);

        skinManagerMock.SetupGet(manager => manager.CurrentSkin).Returns(new Skin { Name = "Ocean Blue", AccentColor = Colors.CornflowerBlue });
        skinManagerMock.Raise(manager => manager.SkinChanged += null, EventArgs.Empty);

        Assert.Equal("Ready - Current theme: Ocean Blue", viewModel.StatusText);
    }

    [Fact]
    public void Dispose_UnsubscribesFromSkinChanged()
    {
        var skinManagerMock = CreateSkinManagerMock();
        var demoService = new FakeThemeManagerDemoService();
        var removeCount = 0;
        skinManagerMock.SetupRemove(manager => manager.SkinChanged -= It.IsAny<EventHandler>())
            .Callback(() => removeCount++);

        using var quickThemeSwitcherViewModel = new QuickThemeSwitcherViewModel(skinManagerMock.Object, NullLogger.Instance);
        var viewModel = new ThemeManagerDemoViewModel(skinManagerMock.Object, quickThemeSwitcherViewModel, demoService, NullLogger.Instance);

        viewModel.Dispose();

        Assert.True(removeCount >= 1);
    }

    private static Mock<ISkinManager> CreateSkinManagerMock()
    {
        var skinManagerMock = new Mock<ISkinManager>();
        var darkSkin = new Skin
        {
            Name = "Dark",
            AccentColor = Colors.DodgerBlue
        };

        skinManagerMock.Setup(manager => manager.GetAvailableSkinNames())
            .Returns(["Dark"]);
        skinManagerMock.Setup(manager => manager.GetSkin("Dark"))
            .Returns(darkSkin);
        skinManagerMock.SetupGet(manager => manager.CurrentSkin)
            .Returns(darkSkin);

        return skinManagerMock;
    }

    private sealed class FakeThemeManagerDemoService : IThemeManagerDemoService
    {
        public string OpenThemeSettingsStatus { get; set; } = "Theme settings dialog opened";
        public string ExportThemeStatus { get; set; } = "Theme exported";
        public string ImportThemeStatus { get; set; } = "Theme imported";
        public string ValidationDemoStatus { get; set; } = "Validation demo shown";
        public string ErrorDemoStatus { get; set; } = "Error demo shown";
        public string ConfirmationDemoStatus { get; set; } = "Confirmation demo confirmed";

        public Task<string> OpenThemeSettingsAsync() => Task.FromResult(OpenThemeSettingsStatus);

        public Task<string> ExportThemeAsync() => Task.FromResult(ExportThemeStatus);

        public Task<string> ImportThemeAsync() => Task.FromResult(ImportThemeStatus);

        public Task<string> ShowValidationDemoAsync() => Task.FromResult(ValidationDemoStatus);

        public Task<string> ShowErrorDemoAsync() => Task.FromResult(ErrorDemoStatus);

        public Task<string> ShowConfirmationDemoAsync() => Task.FromResult(ConfirmationDemoStatus);
    }
}
