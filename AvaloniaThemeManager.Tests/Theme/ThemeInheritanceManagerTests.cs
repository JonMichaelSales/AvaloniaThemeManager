using System.Reflection;
using AvaloniaThemeManager.Theme;
using Moq;
using Avalonia.Media;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeInheritanceManagerTests
{
    private readonly Mock<ISkinManager> _skinManagerMock;
    private readonly ThemeInheritanceManager _manager;

    public ThemeInheritanceManagerTests()
    {
        _skinManagerMock = new Mock<ISkinManager>();
        _manager = new ThemeInheritanceManager(_skinManagerMock.Object);
    }

    [Fact]
    public void RegisterInheritableTheme_StoresThemeAndClearsCache()
    {
        var theme = new InheritableSkin();
        _manager.RegisterInheritableTheme("Test", theme);

        // Use reflection to access private fields for verification
        var themesField = typeof(ThemeInheritanceManager)
            .GetField("_inheritableThemes", BindingFlags.NonPublic | BindingFlags.Instance);
        var cacheField = typeof(ThemeInheritanceManager)
            .GetField("_resolvedCache", BindingFlags.NonPublic | BindingFlags.Instance);

        var themes = GetPrivateField<Dictionary<string, InheritableSkin>>("_inheritableThemes");
        var cache = GetPrivateField<Dictionary<string, Skin>>("_resolvedCache");

        Assert.True(themes.ContainsKey("Test"));
        Assert.Equal("Test", theme.Name);
        Assert.False(cache.ContainsKey("Test"));
    }

    [Fact]
    public void GetResolvedTheme_ReturnsNullIfThemeNotRegistered()
    {
        var result = _manager.GetResolvedTheme("NotRegistered");
        Assert.Null(result);
    }

    [Fact]
    public void GetResolvedTheme_ReturnsCachedThemeIfPresent()
    {
        var skin = new Skin();
        // Set up cache via reflection
        var cache = GetPrivateField<Dictionary<string, Skin>>("_resolvedCache");
        cache["Cached"] = skin;

        var result = _manager.GetResolvedTheme("Cached");
        Assert.Same(skin, result);
    }

    [Fact]
    public void GetResolvedTheme_ResolvesThemeWithNoBase()
    {
        var theme = new InheritableSkin
        {
            Name = "Base",
            BaseThemeName = null,
            PrimaryColor = Color.Parse("#FF0000") // Red for testing
        };

        _manager.RegisterInheritableTheme("Base", theme);

        var result = _manager.GetResolvedTheme("Base");

        Assert.NotNull(result);
        Assert.Equal("Base", result.Name);
        Assert.Equal(Color.Parse("#FF0000"), result.PrimaryColor);
    }

    [Fact]
    public void GetResolvedTheme_ResolvesThemeWithBaseThemeInManager()
    {
        var baseTheme = new InheritableSkin
        {
            Name = "Base",
            BaseThemeName = null,
            PrimaryColor = Color.Parse("#0000FF") // Blue
        };

        var derivedTheme = new InheritableSkin
        {
            Name = "Derived",
            BaseThemeName = "Base",
            SecondaryColor = Color.Parse("#00FF00") // Green
        };

        _manager.RegisterInheritableTheme("Base", baseTheme);
        _manager.RegisterInheritableTheme("Derived", derivedTheme);

        var result = _manager.GetResolvedTheme("Derived");

        Assert.NotNull(result);
        Assert.Equal("Derived", result.Name);
        Assert.Equal(Color.Parse("#0000FF"), result.PrimaryColor); // Inherited from base
        Assert.Equal(Color.Parse("#00FF00"), result.SecondaryColor); // From derived
    }

    [Fact]
    public void GetResolvedTheme_ResolvesThemeWithBaseThemeFromSkinManager()
    {
        // Arrange
        var externalBase = new Skin
        {
            Name = "ExternalBase",
            PrimaryColor = Color.Parse("#FF00FF"), // Magenta
            SecondaryColor = Color.Parse("#FFFF00") // Yellow
        };
        _skinManagerMock.Setup(x => x.GetSkin("ExternalBase")).Returns(externalBase);

        var derivedTheme = new InheritableSkin
        {
            Name = "Derived",
            BaseThemeName = "ExternalBase",
            AccentColor = Color.Parse("#00FFFF") // Cyan - this should be added to the base
        };

        _manager.RegisterInheritableTheme("Derived", derivedTheme);

        // Act
        var result = _manager.GetResolvedTheme("Derived");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Derived", result.Name);
        Assert.Equal(Color.Parse("#FF00FF"), result.PrimaryColor); // Inherited from external base
        Assert.Equal(Color.Parse("#FFFF00"), result.SecondaryColor); // Inherited from external base
        Assert.Equal(Color.Parse("#00FFFF"), result.AccentColor); // From derived theme
        _skinManagerMock.Verify(x => x.GetSkin("ExternalBase"), Times.Once);
    }

    [Fact]
    public void CreateVariant_CreatesAndRegistersVariant()
    {
        var overrides = new Dictionary<string, object> { { "PrimaryColor", "Red" } };

        var variant = _manager.CreateVariant("Base", "Variant", overrides);

        Assert.Equal("Variant", variant.Name);
        Assert.Equal("Base", variant.BaseThemeName);
        Assert.Equal(overrides, variant.PropertyOverrides);

        // Check registration
        var themes = GetPrivateField<Dictionary<string, InheritableSkin>>("_inheritableThemes");
        Assert.True(themes.ContainsKey("Variant"));
    }

    [Fact]
    public void ClearCache_RemovesAllCachedThemes()
    {
        var cache = GetPrivateField<Dictionary<string, Skin>>("_resolvedCache");
        cache["Test"] = new Skin();

        _manager.ClearCache();

        Assert.Empty(cache);
    }

    [Fact]
    public void Constructor_WithNullSkinManager_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ThemeInheritanceManager(null!));
    }

    [Fact]
    public void Constructor_WithSkinManager_StoresSkinManager()
    {
        var skinManagerMock = new Mock<ISkinManager>();
        var manager = new ThemeInheritanceManager(skinManagerMock.Object);

        // Verify the manager was created successfully
        Assert.NotNull(manager);
    }

    [Fact]
    public void GetResolvedTheme_CachesResolvedTheme()
    {
        var theme = new InheritableSkin
        {
            Name = "Cacheable",
            BaseThemeName = null,
            PrimaryColor = Color.Parse("#123456")
        };

        _manager.RegisterInheritableTheme("Cacheable", theme);

        // First call should resolve and cache
        var result1 = _manager.GetResolvedTheme("Cacheable");

        // Second call should return cached version
        var result2 = _manager.GetResolvedTheme("Cacheable");

        Assert.Same(result1, result2); // Should be same instance from cache
    }

    [Fact]
    public void GetResolvedTheme_WithPropertyOverrides_AppliesOverrides()
    {
        var baseTheme = new Skin
        {
            Name = "BaseForOverrides",
            PrimaryColor = Color.Parse("#000000"), // Black
            SecondaryColor = Color.Parse("#FFFFFF") // White
        };
        _skinManagerMock.Setup(x => x.GetSkin("BaseForOverrides")).Returns(baseTheme);

        var derivedTheme = new InheritableSkin
        {
            Name = "WithOverrides",
            BaseThemeName = "BaseForOverrides"
        };

        // Add property override for PrimaryColor
        derivedTheme.PropertyOverrides["PrimaryColor"] = "#FF0000"; // Red

        _manager.RegisterInheritableTheme("WithOverrides", derivedTheme);

        var result = _manager.GetResolvedTheme("WithOverrides");

        Assert.NotNull(result);
        Assert.Equal(Color.Parse("#FF0000"), result.PrimaryColor); // Should be overridden to red
        Assert.Equal(Color.Parse("#FFFFFF"), result.SecondaryColor); // Should remain white from base
    }

    [Fact]
    public void GetResolvedTheme_PreservesTypographyOverridesFromDerivedTheme()
    {
        var baseTheme = new Skin
        {
            Name = "BaseTypography"
        };
        baseTheme.Typography.BodyLarge = 18;
        baseTheme.Typography.TitleMedium = 24;
        _skinManagerMock.Setup(x => x.GetSkin("BaseTypography")).Returns(baseTheme);

        var derivedTheme = new InheritableSkin
        {
            Name = "DerivedTypography",
            BaseThemeName = "BaseTypography"
        };
        derivedTheme.Typography.BodyLarge = 30;

        _manager.RegisterInheritableTheme("DerivedTypography", derivedTheme);

        var result = _manager.GetResolvedTheme("DerivedTypography");

        Assert.NotNull(result);
        Assert.Equal(30, result.Typography.BodyLarge);
        Assert.Equal(24, result.Typography.TitleMedium);
    }

    [Fact]
    public void GetResolvedTheme_PreservesStyleAndControlThemeUriOverrides()
    {
        var baseTheme = new Skin
        {
            Name = "BaseUris",
            ControlThemeUris = new Dictionary<string, string>
            {
                ["Button"] = "avares://Base/Button.axaml"
            },
            StyleUris = new Dictionary<string, string>
            {
                ["BaseStyle"] = "avares://Base/Style.axaml"
            }
        };
        _skinManagerMock.Setup(x => x.GetSkin("BaseUris")).Returns(baseTheme);

        var derivedTheme = new InheritableSkin
        {
            Name = "DerivedUris",
            BaseThemeName = "BaseUris"
        };
        derivedTheme.ControlThemeUris["TextBox"] = "avares://Derived/TextBox.axaml";
        derivedTheme.StyleUris["DerivedStyle"] = "avares://Derived/Style.axaml";

        _manager.RegisterInheritableTheme("DerivedUris", derivedTheme);

        var result = _manager.GetResolvedTheme("DerivedUris");

        Assert.NotNull(result);
        Assert.Equal("avares://Base/Button.axaml", result.ControlThemeUris["Button"]);
        Assert.Equal("avares://Derived/TextBox.axaml", result.ControlThemeUris["TextBox"]);
        Assert.Equal("avares://Base/Style.axaml", result.StyleUris["BaseStyle"]);
        Assert.Equal("avares://Derived/Style.axaml", result.StyleUris["DerivedStyle"]);
    }

    [Fact]
    public void GetResolvedTheme_PreservesAdvancedTypographyPropertyOverrides()
    {
        var baseTheme = new Skin
        {
            Name = "BaseAdvancedTypography",
            HeaderFontFamily = new FontFamily("Inter"),
            BodyFontFamily = new FontFamily("Inter"),
            MonospaceFontFamily = new FontFamily("Cascadia Code"),
            LineHeight = 1.2,
            LetterSpacing = 0.1,
            EnableLigatures = false
        };
        _skinManagerMock.Setup(x => x.GetSkin("BaseAdvancedTypography")).Returns(baseTheme);

        var derivedTheme = new InheritableSkin
        {
            Name = "DerivedAdvancedTypography",
            BaseThemeName = "BaseAdvancedTypography",
            HeaderFontFamily = new FontFamily("Aptos"),
            BodyFontFamily = new FontFamily("Calibri"),
            MonospaceFontFamily = new FontFamily("Fira Code"),
            LineHeight = 1.8,
            LetterSpacing = 0.5,
            EnableLigatures = true
        };

        _manager.RegisterInheritableTheme("DerivedAdvancedTypography", derivedTheme);

        var result = _manager.GetResolvedTheme("DerivedAdvancedTypography");

        Assert.NotNull(result);
        Assert.Equal("Aptos", result.HeaderFontFamily.ToString());
        Assert.Equal("Calibri", result.BodyFontFamily.ToString());
        Assert.Equal("Fira Code", result.MonospaceFontFamily.ToString());
        Assert.Equal(1.8, result.LineHeight);
        Assert.Equal(0.5, result.LetterSpacing);
        Assert.True(result.EnableLigatures);
    }

    private T GetPrivateField<T>(string fieldName) where T : class
    {
        var field = typeof(ThemeInheritanceManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);

        var value = field.GetValue(_manager);
        Assert.IsType<T>(value);
        return (T)value;
    }
}
