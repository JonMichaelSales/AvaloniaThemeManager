using System.Text.Json;
using Avalonia;
using Avalonia.Media;
using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeImportExportTests
{
    private Skin CreateTestSkin() => new Skin
    {
        Name = "Valid Theme",
        PrimaryColor = Color.Parse("#1F2937"),
        SecondaryColor = Color.Parse("#475569"),
        AccentColor = Color.Parse("#005A9E"),
        PrimaryBackground = Colors.White,
        SecondaryBackground = Color.Parse("#F2F4F7"),
        PrimaryTextColor = Color.Parse("#111111"),
        SecondaryTextColor = Color.Parse("#4A5568"),
        FontFamily = new FontFamily("Arial"),
        FontSizeSmall = 12,
        FontSizeMedium = 14,
        FontSizeLarge = 18,
        FontWeight = FontWeight.Bold,
        BorderColor = Color.Parse("#6B7280"),
        BorderThickness = new Thickness(1, 2, 3, 4),
        BorderRadius = 5,
        ErrorColor = Color.Parse("#B42318"),
        WarningColor = Color.Parse("#B54708"),
        SuccessColor = Color.Parse("#027A48"),
        ControlThemeUris = new Dictionary<string, string>
        {
            ["Button"] = "avares://AvaloniaThemeManager/Themes/Test/Button.axaml"
        },
        StyleUris = new Dictionary<string, string>
        {
            ["CustomStyle"] = "avares://AvaloniaThemeManager/Themes/Test/Custom.axaml"
        },
        Typography = new TypographyScale
        {
            DisplayLarge = 60,
            DisplayMedium = 48,
            DisplaySmall = 37,
            HeadlineLarge = 33,
            HeadlineMedium = 29,
            HeadlineSmall = 25,
            TitleLarge = 23,
            TitleMedium = 17,
            TitleSmall = 15,
            LabelLarge = 15,
            LabelMedium = 13,
            LabelSmall = 10,
            BodyLarge = 19,
            BodyMedium = 15,
            BodySmall = 13
        },
        HeaderFontFamily = new FontFamily("Arial"),
        BodyFontFamily = new FontFamily("Arial"),
        MonospaceFontFamily = new FontFamily("Consolas"),
        LineHeight = 1.2,
        LetterSpacing = 0.1,
        EnableLigatures = true
    };

    private InheritableSkin CreateTestInheritableSkin()
    {
        var skin = new InheritableSkin
        {
            Name = "Inherit",
            BaseThemeName = "BaseTheme",
            PropertyOverrides = new Dictionary<string, object> { { "PrimaryColor", "#FF0000" } },
            HeaderFontFamily = new FontFamily("Inter"),
            BodyFontFamily = new FontFamily("Inter"),
            MonospaceFontFamily = new FontFamily("Fira Code"),
            LineHeight = 1.4,
            LetterSpacing = 0.2,
            EnableLigatures = false
        };
        skin.ControlThemeUris["Button"] = "avares://AvaloniaThemeManager/Themes/Inherit/Button.axaml";
        skin.StyleUris["CustomStyle"] = "avares://AvaloniaThemeManager/Themes/Inherit/Custom.axaml";
        skin.Typography.TitleLarge = 30;
        return skin;
    }

    private static SerializableTheme DeserializeThemeFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<SerializableTheme>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private static void AssertSkinsEquivalent(Skin expected, Skin actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.PrimaryColor, actual.PrimaryColor);
        Assert.Equal(expected.SecondaryColor, actual.SecondaryColor);
        Assert.Equal(expected.AccentColor, actual.AccentColor);
        Assert.Equal(expected.PrimaryBackground, actual.PrimaryBackground);
        Assert.Equal(expected.SecondaryBackground, actual.SecondaryBackground);
        Assert.Equal(expected.PrimaryTextColor, actual.PrimaryTextColor);
        Assert.Equal(expected.SecondaryTextColor, actual.SecondaryTextColor);
        Assert.Equal(expected.BorderColor, actual.BorderColor);
        Assert.Equal(expected.ErrorColor, actual.ErrorColor);
        Assert.Equal(expected.WarningColor, actual.WarningColor);
        Assert.Equal(expected.SuccessColor, actual.SuccessColor);
        Assert.Equal(expected.FontFamily.ToString(), actual.FontFamily.ToString());
        Assert.Equal(expected.FontSizeSmall, actual.FontSizeSmall);
        Assert.Equal(expected.FontSizeMedium, actual.FontSizeMedium);
        Assert.Equal(expected.FontSizeLarge, actual.FontSizeLarge);
        Assert.Equal(expected.FontWeight, actual.FontWeight);
        Assert.Equal(expected.BorderRadius, actual.BorderRadius);
        Assert.Equal(expected.BorderThickness, actual.BorderThickness);
        Assert.Equal(expected.ControlThemeUris, actual.ControlThemeUris);
        Assert.Equal(expected.StyleUris, actual.StyleUris);
        Assert.Equal(expected.Typography.DisplayLarge, actual.Typography.DisplayLarge);
        Assert.Equal(expected.Typography.DisplayMedium, actual.Typography.DisplayMedium);
        Assert.Equal(expected.Typography.DisplaySmall, actual.Typography.DisplaySmall);
        Assert.Equal(expected.Typography.HeadlineLarge, actual.Typography.HeadlineLarge);
        Assert.Equal(expected.Typography.HeadlineMedium, actual.Typography.HeadlineMedium);
        Assert.Equal(expected.Typography.HeadlineSmall, actual.Typography.HeadlineSmall);
        Assert.Equal(expected.Typography.TitleLarge, actual.Typography.TitleLarge);
        Assert.Equal(expected.Typography.TitleMedium, actual.Typography.TitleMedium);
        Assert.Equal(expected.Typography.TitleSmall, actual.Typography.TitleSmall);
        Assert.Equal(expected.Typography.LabelLarge, actual.Typography.LabelLarge);
        Assert.Equal(expected.Typography.LabelMedium, actual.Typography.LabelMedium);
        Assert.Equal(expected.Typography.LabelSmall, actual.Typography.LabelSmall);
        Assert.Equal(expected.Typography.BodyLarge, actual.Typography.BodyLarge);
        Assert.Equal(expected.Typography.BodyMedium, actual.Typography.BodyMedium);
        Assert.Equal(expected.Typography.BodySmall, actual.Typography.BodySmall);
        Assert.Equal(expected.HeaderFontFamily.ToString(), actual.HeaderFontFamily.ToString());
        Assert.Equal(expected.BodyFontFamily.ToString(), actual.BodyFontFamily.ToString());
        Assert.Equal(expected.MonospaceFontFamily.ToString(), actual.MonospaceFontFamily.ToString());
        Assert.Equal(expected.LineHeight, actual.LineHeight);
        Assert.Equal(expected.LetterSpacing, actual.LetterSpacing);
        Assert.Equal(expected.EnableLigatures, actual.EnableLigatures);
    }

    [Fact]
    public async Task ExportThemeAsync_WritesFullFidelityThemeFile()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var serialized = DeserializeThemeFile(filePath);
        Assert.Equal("Valid Theme", serialized.Name);
        Assert.Equal("2.0", serialized.Version);
        Assert.Equal("avares://AvaloniaThemeManager/Themes/Test/Button.axaml", serialized.ControlThemeUris["Button"]);
        Assert.Equal("avares://AvaloniaThemeManager/Themes/Test/Custom.axaml", serialized.StyleUris["CustomStyle"]);
        Assert.NotNull(serialized.AdvancedTypography);
        Assert.Equal(60, serialized.AdvancedTypography!.DisplayLarge);
        Assert.Equal("Consolas", serialized.AdvancedTypography.MonospaceFontFamily);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ExportAdvancedThemeAsync_WritesSameFullFidelityShape()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportAdvancedThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var serialized = DeserializeThemeFile(filePath);
        Assert.Equal("2.0", serialized.Version);
        Assert.NotNull(serialized.AdvancedTypography);
        Assert.Equal(17, serialized.AdvancedTypography!.TitleMedium);
        Assert.Equal("avares://AvaloniaThemeManager/Themes/Test/Button.axaml", serialized.ControlThemeUris["Button"]);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ExportInheritableThemeAsync_WritesFullFidelityThemeWithInheritanceMetadata()
    {
        var skin = CreateTestInheritableSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportInheritableThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var serialized = DeserializeThemeFile(filePath);
        Assert.Equal("Inherit", serialized.Name);
        Assert.Equal("BaseTheme", serialized.BaseTheme);
        Assert.Equal("avares://AvaloniaThemeManager/Themes/Inherit/Button.axaml", serialized.ControlThemeUris["Button"]);
        Assert.NotNull(serialized.AdvancedTypography);
        Assert.Equal(30, serialized.AdvancedTypography!.TitleLarge);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportThemeAsync_RoundTripsFullRuntimeSkin()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportThemeAsync(skin, filePath);

        var result = await ThemeImportExport.ImportThemeAsync(filePath);

        Assert.True(result.Success);
        Assert.NotNull(result.Theme);
        AssertSkinsEquivalent(skin, result.Theme);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportThemeAsync_ReturnsError_WhenFileMissing()
    {
        var filePath = Path.Combine(Path.GetTempPath(), "nonexistent.json");
        var result = await ThemeImportExport.ImportThemeAsync(filePath);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task ImportThemeAsync_ImportsLegacyBasicThemeFileWithDefaults()
    {
        var filePath = Path.GetTempFileName();
        const string legacyJson = """
        {
          "name": "Legacy",
          "version": "1.0",
          "primaryColor": "#1F2937",
          "secondaryColor": "#475569",
          "accentColor": "#005A9E",
          "primaryBackground": "#FFFFFF",
          "secondaryBackground": "#F2F4F7",
          "primaryTextColor": "#111111",
          "secondaryTextColor": "#4A5568",
          "borderColor": "#6B7280",
          "errorColor": "#B42318",
          "warningColor": "#B54708",
          "successColor": "#027A48",
          "fontFamily": "Segoe UI",
          "fontSizeSmall": 12,
          "fontSizeMedium": 14,
          "fontSizeLarge": 18,
          "fontWeight": "Bold",
          "borderRadius": 5,
          "borderThickness": {
            "left": 1,
            "top": 2,
            "right": 3,
            "bottom": 4
          }
        }
        """;
        await File.WriteAllTextAsync(filePath, legacyJson);

        var result = await ThemeImportExport.ImportThemeAsync(filePath);

        Assert.True(result.Success);
        Assert.NotNull(result.Theme);
        Assert.Equal("Legacy", result.Theme.Name);
        Assert.Empty(result.Theme.ControlThemeUris);
        Assert.Empty(result.Theme.StyleUris);
        Assert.Contains("Segoe UI", result.Theme.HeaderFontFamily.ToString());
        Assert.Contains("Segoe UI", result.Theme.BodyFontFamily.ToString());
        Assert.Contains("Consolas", result.Theme.MonospaceFontFamily.ToString());
        Assert.Equal(1.5, result.Theme.LineHeight);
        Assert.Equal(0, result.Theme.LetterSpacing);
        Assert.True(result.Theme.EnableLigatures);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportAdvancedThemeAsync_ReturnsSkin_WhenValid()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportAdvancedThemeAsync(skin, filePath);

        var imported = await ThemeImportExport.ImportAdvancedThemeAsync(filePath);

        Assert.NotNull(imported);
        AssertSkinsEquivalent(skin, imported);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportAdvancedThemeAsync_ReturnsNull_WhenInvalid()
    {
        var filePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(filePath, "{ invalid json }");

        var imported = await ThemeImportExport.ImportAdvancedThemeAsync(filePath);

        Assert.Null(imported);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportInheritableThemeAsync_ReturnsInheritableSkin_WhenValid()
    {
        var skin = CreateTestInheritableSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportInheritableThemeAsync(skin, filePath);

        var imported = await ThemeImportExport.ImportInheritableThemeAsync(filePath);

        Assert.NotNull(imported);
        Assert.Equal("Inherit", imported.Name);
        Assert.Equal("BaseTheme", imported.BaseThemeName);
        Assert.Equal("avares://AvaloniaThemeManager/Themes/Inherit/Button.axaml", imported.ControlThemeUris["Button"]);
        Assert.Equal("Inter", imported.HeaderFontFamily.ToString());
        Assert.Equal(30, imported.Typography.TitleLarge);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportInheritableThemeAsync_ReturnsNull_WhenInvalid()
    {
        var filePath = Path.GetTempFileName();
        await File.WriteAllTextAsync(filePath, "{ invalid json }");

        var imported = await ThemeImportExport.ImportInheritableThemeAsync(filePath);

        Assert.Null(imported);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ValidateThemeFileAsync_ReturnsValidResult_WhenValid()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportThemeAsync(skin, filePath);

        var result = await ThemeImportExport.ValidateThemeFileAsync(filePath);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ValidateThemeFileAsync_ReturnsError_WhenFileMissing()
    {
        var filePath = Path.Combine(Path.GetTempPath(), "nonexistent.json");
        var result = await ThemeImportExport.ValidateThemeFileAsync(filePath);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("does not exist"));
    }

    [Fact]
    public async Task ValidateThemeFileAsync_UsesUnifiedValidatorPolicy()
    {
        var filePath = Path.GetTempFileName();
        const string invalidThemeJson = """
        {
          "name": "Test",
          "version": "2.0",
          "primaryColor": "#FFFFFF",
          "secondaryColor": "#FFFFFF",
          "accentColor": "#FFFFFF",
          "primaryBackground": "#FFFFFF",
          "secondaryBackground": "#FFFFFF",
          "primaryTextColor": "#FFFFFF",
          "secondaryTextColor": "#FFFFFF",
          "borderColor": "#FFFFFF",
          "errorColor": "#FFFFFF",
          "warningColor": "#FFFFFF",
          "successColor": "#FFFFFF",
          "fontFamily": "Segoe UI",
          "fontSizeSmall": 6,
          "fontSizeMedium": 6,
          "fontSizeLarge": 6,
          "fontWeight": "Normal",
          "borderRadius": 4,
          "borderThickness": {
            "left": 0,
            "top": 0,
            "right": 0,
            "bottom": 0
          }
        }
        """;
        await File.WriteAllTextAsync(filePath, invalidThemeJson);

        var result = await ThemeImportExport.ValidateThemeFileAsync(filePath);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("Small font size"));
        Assert.Contains(result.Errors, error => error.Contains("Primary text contrast ratio"));
        Assert.Contains(result.Warnings, warning => warning.Contains("confusing"));
        File.Delete(filePath);
    }

    [Fact]
    public async Task ExportThemePackAsync_WritesFullFidelityThemes()
    {
        var skin = CreateTestSkin();
        var themes = new Dictionary<string, Skin> { { "Test", skin } };
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportThemePackAsync(themes, filePath, "PackName", "desc");

        Assert.True(result);
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("PackName", json);
        Assert.Contains("Test", json);
        Assert.Contains("\"version\": \"2.0\"", json);
        Assert.Contains("\"controlThemeUris\"", json);
        Assert.Contains("\"advancedTypography\"", json);
        File.Delete(filePath);
    }

    [Fact]
    public async Task SerializableTheme_ToSkin_MatchesImportThemeConversion()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportThemeAsync(skin, filePath);

        var serializableTheme = DeserializeThemeFile(filePath);
        var directConversion = serializableTheme.ToSkin();
        var importResult = await ThemeImportExport.ImportThemeAsync(filePath);

        Assert.True(importResult.Success);
        Assert.NotNull(importResult.Theme);
        AssertSkinsEquivalent(directConversion, importResult.Theme);
        File.Delete(filePath);
    }
}
