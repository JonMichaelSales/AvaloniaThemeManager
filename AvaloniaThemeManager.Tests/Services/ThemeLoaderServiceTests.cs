using System.Text;
using AvaloniaThemeManager.Services;

namespace AvaloniaThemeManager.Tests.Services;

public sealed class ThemeLoaderServiceTests
{
    [Fact]
    public void LoadSkins_LoadsAllConfiguredThemes_WhenThemeJsonIsAvailable()
    {
        var themeJson = CreateThemeJson("Loaded Theme");
        var service = new ThemeLoaderService(
            ["Dark", "Light"],
            _ => CreateStream(themeJson),
            _ => false);

        var skins = service.LoadSkins();

        Assert.Equal(["Loaded Theme", "Loaded Theme"], skins.Select(skin => skin.Name ?? string.Empty).ToArray());
    }

    [Fact]
    public void LoadSkins_OnlyIncludesExistingControlThemeUris()
    {
        var service = new ThemeLoaderService(
            ["Sparse", "Rich"],
            uri => CreateStream(CreateThemeJson(uri.ToString().Contains("/Sparse/") ? "Sparse" : "Rich")),
            uri =>
            {
                var path = uri.ToString();
                return path.Contains("/Sparse/") && (path.EndsWith("Button.axaml", StringComparison.Ordinal) || path.EndsWith("TextBlock.axaml", StringComparison.Ordinal))
                    || path.Contains("/Rich/") && (path.EndsWith("Button.axaml", StringComparison.Ordinal) || path.EndsWith("TextBox.axaml", StringComparison.Ordinal) || path.EndsWith("ComboBox.axaml", StringComparison.Ordinal) || path.EndsWith("Slider.axaml", StringComparison.Ordinal));
            });

        var skins = service.LoadSkins();
        var sparse = Assert.Single(skins, skin => skin.Name == "Sparse");
        var rich = Assert.Single(skins, skin => skin.Name == "Rich");

        Assert.Equal(
            ["Button", "TextBlock"],
            sparse.ControlThemeUris.Keys.OrderBy(key => key).ToArray());
        Assert.DoesNotContain("TextBox", sparse.ControlThemeUris.Keys);

        Assert.Contains("TextBox", rich.ControlThemeUris.Keys);
        Assert.Contains("ComboBox", rich.ControlThemeUris.Keys);
        Assert.Contains("Slider", rich.ControlThemeUris.Keys);
    }

    [Fact]
    public void LoadSkins_SkipsTheme_WhenThemeAssetCannotBeOpened()
    {
        var service = new ThemeLoaderService(
            ["Broken"],
            _ => throw new FileNotFoundException("missing"),
            _ => false);

        var skins = service.LoadSkins();

        Assert.Empty(skins);
    }

    [Fact]
    public void LoadSkins_SkipsTheme_WhenThemeJsonIsInvalid()
    {
        var service = new ThemeLoaderService(
            ["Broken"],
            _ => CreateStream("{ not json }"),
            _ => false);

        var skins = service.LoadSkins();

        Assert.Empty(skins);
    }

    [Fact]
    public void LoadSkins_UsesAssetExistsFilter_ForControlThemeUris()
    {
        const string json = """
        {
          "name": "Filtered",
          "primaryColor": "#343B48",
          "secondaryColor": "#3D4654",
          "accentColor": "#3498DB",
          "primaryBackground": "#2C313D",
          "secondaryBackground": "#464F62",
          "primaryTextColor": "#FFFFFF",
          "secondaryTextColor": "#CCCCCC",
          "borderColor": "#5D6778",
          "errorColor": "#E74C3C",
          "warningColor": "#F39C12",
          "successColor": "#2ECC71",
          "fontFamily": "Segoe UI",
          "fontSizeSmall": 10,
          "fontSizeMedium": 12,
          "fontSizeLarge": 16,
          "fontWeight": "Normal",
          "borderRadius": 4,
          "borderThickness": { "left": 1, "top": 1, "right": 1, "bottom": 1 }
        }
        """;

        var service = new ThemeLoaderService(
            ["Filtered"],
            _ => CreateStream(json),
            uri => uri.ToString().EndsWith("Button.axaml", StringComparison.Ordinal));

        var skins = service.LoadSkins();
        var skin = Assert.Single(skins);

        Assert.Equal("Filtered", skin.Name);
        Assert.Equal(["Button"], skin.ControlThemeUris.Keys);
    }

    private static Stream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    private static string CreateThemeJson(string themeName)
    {
        return $$"""
        {
          "name": "{{themeName}}",
          "primaryColor": "#343B48",
          "secondaryColor": "#3D4654",
          "accentColor": "#3498DB",
          "primaryBackground": "#2C313D",
          "secondaryBackground": "#464F62",
          "primaryTextColor": "#FFFFFF",
          "secondaryTextColor": "#CCCCCC",
          "borderColor": "#5D6778",
          "errorColor": "#E74C3C",
          "warningColor": "#F39C12",
          "successColor": "#2ECC71",
          "fontFamily": "Segoe UI",
          "fontSizeSmall": 10,
          "fontSizeMedium": 12,
          "fontSizeLarge": 16,
          "fontWeight": "Normal",
          "borderRadius": 4,
          "borderThickness": { "left": 1, "top": 1, "right": 1, "bottom": 1 }
        }
        """;
    }
}
