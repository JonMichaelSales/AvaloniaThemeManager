using Avalonia.Media;
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Models;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AvaloniaThemeManager.Tests.Extensions;

public sealed class AppBuilderExtensionsTests : IDisposable
{
    public AppBuilderExtensionsTests()
    {
        AppBuilderExtensions.ResetForTests();
    }

    public void Dispose()
    {
        AppBuilderExtensions.ResetForTests();
        AppSettings.Instance.Theme = "Dark";
    }

    [Fact]
    public void GetRequiredService_ReturnsConfiguredSkinManager()
    {
        using var testApp = new TestApplication();
        var skinManager = CreateSkinManager(testApp);
        var provider = new ServiceCollection()
            .AddSingleton(skinManager)
            .AddSingleton<ISkinManager>(serviceProvider => serviceProvider.GetRequiredService<SkinManager>())
            .BuildServiceProvider();

        AppBuilderExtensions.ConfigureServiceProvider(provider);

        var resolved = AppBuilderExtensions.GetRequiredService<ISkinManager>();

        Assert.Same(skinManager, resolved);
    }

    [Fact]
    public void InitializeThemeManager_LoadsSavedThemeIntoConfiguredService()
    {
        using var testApp = new TestApplication();
        var skinManager = CreateSkinManager(testApp);
        skinManager.RegisterSkin("SavedTheme", new Skin { Name = "SavedTheme", AccentColor = Colors.Coral });

        var provider = new ServiceCollection()
            .AddSingleton(skinManager)
            .AddSingleton<ISkinManager>(serviceProvider => serviceProvider.GetRequiredService<SkinManager>())
            .BuildServiceProvider();

        AppBuilderExtensions.ConfigureServiceProvider(provider);
        AppSettings.Instance.Theme = "SavedTheme";
        AppBuilderExtensions.SetInitializationActionForTests(() =>
        {
            var resolvedManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
            resolvedManager.LoadSavedTheme();
        });

        AppBuilderExtensions.InitializeThemeManager();

        Assert.Equal("SavedTheme", skinManager.CurrentSkin?.Name);
    }

    [Fact]
    public void ThemeSettingsViewModel_DefaultConstructor_UsesConfiguredSkinManager()
    {
        var skinManagerMock = CreateSkinManagerMock();
        ConfigureProviderWithMock(skinManagerMock);

        var viewModel = new ThemeSettingsViewModel();

        Assert.Single(viewModel.AvailableThemes);
        viewModel.SelectedTheme = viewModel.AvailableThemes[0];

        skinManagerMock.Verify(manager => manager.ApplySkin("Dark"), Times.AtLeastOnce);
    }

    [Fact]
    public void QuickThemeSwitcherViewModel_DefaultConstructor_UsesConfiguredSkinManager()
    {
        var skinManagerMock = CreateSkinManagerMock();
        ConfigureProviderWithMock(skinManagerMock);

        using var viewModel = new QuickThemeSwitcherViewModel();

        Assert.Single(viewModel.AvailableThemes);
        viewModel.SelectedTheme = viewModel.AvailableThemes[0];

        skinManagerMock.Verify(manager => manager.ApplySkin("Dark"), Times.Once);
    }

    [Fact]
    public void ThemeSettingsViewModel_DefaultConstructor_ThrowsWhenServiceProviderIsNotConfigured()
    {
        AppBuilderExtensions.ResetForTests();

        Assert.Throws<InvalidOperationException>(() => new ThemeSettingsViewModel());
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
            .Returns(new List<string> { "Dark" });
        skinManagerMock.Setup(manager => manager.GetSkin("Dark"))
            .Returns(darkSkin);
        skinManagerMock.Setup(manager => manager.GetSkin(It.Is<string?>(name => name != "Dark")))
            .Returns((Skin?)null);
        skinManagerMock.SetupGet(manager => manager.CurrentSkin)
            .Returns(darkSkin);

        return skinManagerMock;
    }

    private static void ConfigureProviderWithMock(Mock<ISkinManager> skinManagerMock)
    {
        var provider = new ServiceCollection()
            .AddSingleton(skinManagerMock.Object)
            .BuildServiceProvider();

        AppBuilderExtensions.ConfigureServiceProvider(provider);
    }

    private static SkinManager CreateSkinManager(TestApplication testApp)
    {
        var themeLoader = new Mock<IThemeLoaderService>();
        themeLoader.Setup(service => service.LoadSkins(It.IsAny<string>()))
            .Returns(new List<Skin>());

        var applicationMock = new Mock<IApplication>();
        applicationMock.SetupGet(application => application.Resources)
            .Returns(testApp.Resources);
        applicationMock.SetupGet(application => application.ApplicationLifetime)
            .Returns(testApp.ApplicationLifetime);
        applicationMock.SetupGet(application => application.AppStyles)
            .Returns(new AvaloniaStylesWrapper(testApp.Styles));

        return new SkinManager(themeLoader.Object, applicationMock.Object);
    }
}
