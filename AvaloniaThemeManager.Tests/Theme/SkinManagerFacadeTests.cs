using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Services;
using Moq;

namespace AvaloniaThemeManager.Tests.Theme;

public class SkinManagerFacadeTests
{
    [Fact]
    public void ApplySkin_ByName_DelegatesLookupApplyAndPersistence()
    {
        var themeLoader = new Mock<IThemeLoaderService>();
        themeLoader.Setup(service => service.LoadSkins(It.IsAny<string>())).Returns(new List<Skin>());
        var registry = new Mock<ISkinRegistry>();
        var selectionStore = new Mock<IThemeSelectionStore>();
        var resourceApplier = new Mock<ISkinResourceApplier>();
        var visualRefresh = new Mock<IVisualRefreshService>();
        var skin = new Skin { Name = "Delegated" };
        registry.Setup(service => service.TryGetRegisteredSkin("Delegated", out skin)).Returns(true);

        var manager = new SkinManager(themeLoader.Object, registry.Object, selectionStore.Object, resourceApplier.Object, visualRefresh.Object);

        manager.ApplySkin("Delegated");

        registry.Verify(service => service.TryGetRegisteredSkin("Delegated", out skin), Times.Once);
        resourceApplier.Verify(service => service.ApplySkinResources(skin), Times.Once);
        visualRefresh.Verify(service => service.Refresh(), Times.Once);
        selectionStore.Verify(service => service.SaveSelectedTheme("Delegated"), Times.Once);
    }

    [Fact]
    public void LoadSavedTheme_AppliesOnlyRegisteredSavedThemes()
    {
        var themeLoader = new Mock<IThemeLoaderService>();
        themeLoader.Setup(service => service.LoadSkins(It.IsAny<string>())).Returns(new List<Skin>());
        var registry = new Mock<ISkinRegistry>();
        var selectionStore = new Mock<IThemeSelectionStore>();
        var resourceApplier = new Mock<ISkinResourceApplier>();
        var visualRefresh = new Mock<IVisualRefreshService>();

        selectionStore.Setup(service => service.GetSavedThemeName()).Returns("SavedTheme");
        Skin? ignoredSkin;
        registry.Setup(service => service.TryGetRegisteredSkin("SavedTheme", out ignoredSkin)).Returns(false);

        var manager = new SkinManager(themeLoader.Object, registry.Object, selectionStore.Object, resourceApplier.Object, visualRefresh.Object);

        manager.LoadSavedTheme();

        resourceApplier.Verify(service => service.ApplySkinResources(It.IsAny<Skin>()), Times.Never);
        visualRefresh.Verify(service => service.Refresh(), Times.Never);
    }
}
