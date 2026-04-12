using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using Moq;

namespace AvaloniaThemeManager.Tests.Theme;

public class SkinResourceApplierTests
{
    private readonly Mock<IApplication> _applicationMock = new();
    private readonly Mock<IResourceDictionary> _resourcesMock = new();
    private readonly List<IResourceProvider> _mergedDictionaries = new();
    private readonly Dictionary<object, object?> _resourceDictionary = new();

    public SkinResourceApplierTests()
    {
        _applicationMock.Setup(application => application.Resources).Returns(_resourcesMock.Object);
        _resourcesMock.Setup(resource => resource[It.IsAny<object>()])
            .Returns<object>(key => _resourceDictionary.TryGetValue(key, out var value) ? value : null);
        _resourcesMock.SetupSet(resource => resource[It.IsAny<object>()] = It.IsAny<object>())
            .Callback<object, object?>((key, value) => _resourceDictionary[key] = value);
        _resourcesMock.Setup(resource => resource.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns<object, object?>((key, value) => _resourceDictionary.TryGetValue(key, out value));
        _resourcesMock.SetupGet(resource => resource.MergedDictionaries).Returns(_mergedDictionaries);
    }

    [Fact]
    public void ApplySkinResources_UpdatesBrushAndTypographyResources()
    {
        var applier = new SkinResourceApplier(_applicationMock.Object);
        var skin = new Skin
        {
            PrimaryColor = Colors.Red,
            AccentColor = Colors.Blue,
            PrimaryBackground = Colors.Black,
            PrimaryTextColor = Colors.White,
            Typography = new TypographyScale
            {
                DisplayLarge = 48,
                BodyMedium = 16
            }
        };

        applier.ApplySkinResources(skin);

        _resourcesMock.VerifySet(resource => resource["PrimaryColorBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
        _resourcesMock.VerifySet(resource => resource["AccentBlueBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
        _resourcesMock.VerifySet(resource => resource["DisplayLargeFontSize"] = 48.0, Times.Once);
        _resourcesMock.VerifySet(resource => resource["BodyMediumFontSize"] = 16.0, Times.Once);
    }

    [Fact]
    public void ApplySkinResources_ReplacesPreviouslyAppliedMergedDictionaries()
    {
        var applier = new SkinResourceApplier(_applicationMock.Object);
        var firstSkin = new Skin
        {
            ControlThemeUris = new Dictionary<string, string>
            {
                ["Button"] = "avares://AvaloniaThemeManager/Themes/First/Button.axaml"
            }
        };
        var secondSkin = new Skin
        {
            StyleUris = new Dictionary<string, string>
            {
                ["Card"] = "avares://AvaloniaThemeManager/Themes/Second/Card.axaml"
            }
        };

        applier.ApplySkinResources(firstSkin);
        applier.ApplySkinResources(secondSkin);

        Assert.Single(_mergedDictionaries);
    }
}
