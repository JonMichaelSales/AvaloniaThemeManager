using Avalonia.Media;
using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Tests.Theme;

public class SkinRegistryTests
{
    [Fact]
    public void RegisterSkin_AddsSkinAndPreservesLookup()
    {
        var registry = new SkinRegistry();
        var skin = new Skin { Name = "Registered", AccentColor = Colors.CadetBlue };

        registry.RegisterSkin("Registered", skin);

        Assert.Same(skin, registry.GetSkin("Registered"));
        Assert.Contains("Registered", registry.GetAvailableSkinNames());
    }

    [Fact]
    public void GetSkin_NullOrUnknownName_FallsBackToCurrentSkin()
    {
        var registry = new SkinRegistry();
        var currentSkin = new Skin { Name = "Current" };
        registry.CurrentSkin = currentSkin;

        Assert.Same(currentSkin, registry.GetSkin(null));
        Assert.Same(currentSkin, registry.GetSkin("Missing"));
    }

    [Fact]
    public void TryGetRegisteredSkin_DoesNotUseCurrentSkinFallback()
    {
        var registry = new SkinRegistry();
        registry.CurrentSkin = new Skin { Name = "Current" };

        var found = registry.TryGetRegisteredSkin("Missing", out var skin);

        Assert.False(found);
        Assert.Null(skin);
    }
}
