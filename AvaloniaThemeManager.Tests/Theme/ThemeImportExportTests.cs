using Avalonia.Media;
using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeImportExportTests
{
    private Skin CreateTestSkin() => new Skin
    {
        Name = "Test",
        PrimaryColor = Colors.Red,
        SecondaryColor = Colors.Green,
        AccentColor = Colors.Blue,
        PrimaryBackground = Colors.White,
        SecondaryBackground = Colors.Black,
        PrimaryTextColor = Colors.Black,        // Changed from Gray to Black for better contrast
        SecondaryTextColor = Colors.White,
        FontFamily = new FontFamily("Arial"),
        FontSizeSmall = 10,
        FontSizeMedium = 12,
        FontSizeLarge = 14,
        FontWeight = FontWeight.Bold,
        BorderColor = Colors.Yellow,
        BorderThickness = new Avalonia.Thickness(1, 2, 3, 4),
        BorderRadius = 5,
        ErrorColor = Colors.Orange,
        WarningColor = Colors.Purple,
        SuccessColor = Colors.Brown,
        Typography = new TypographyScale(),
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
            PropertyOverrides = new Dictionary<string, object> { { "PrimaryColor", "#FF0000" } }
        };
        // Set other properties as needed
        return skin;
    }

    [Fact]
    public async Task ExportThemeAsync_WritesFileAndReturnsTrue()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Test", json);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ExportAdvancedThemeAsync_WritesFileAndReturnsTrue()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportAdvancedThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Test", json);
        Assert.Contains("advancedTypography", json);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ExportInheritableThemeAsync_WritesFileAndReturnsTrue()
    {
        var skin = CreateTestInheritableSkin();
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportInheritableThemeAsync(skin, filePath, "desc", "author");

        Assert.True(result);
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Inherit", json);
        Assert.Contains("BaseTheme", json);
        File.Delete(filePath);
    }

    [Fact]
    public async Task ImportThemeAsync_ReturnsThemeImportResult_Success()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportThemeAsync(skin, filePath);

        var result = await ThemeImportExport.ImportThemeAsync(filePath);

        Assert.True(result.Success);
        Assert.NotNull(result.Theme);
        Assert.Equal("Test", result.Theme.Name);
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
    public async Task ImportAdvancedThemeAsync_ReturnsSkin_WhenValid()
    {
        var skin = CreateTestSkin();
        var filePath = Path.GetTempFileName();
        await ThemeImportExport.ExportAdvancedThemeAsync(skin, filePath);

        var imported = await ThemeImportExport.ImportAdvancedThemeAsync(filePath);

        Assert.NotNull(imported);
        Assert.Equal("Test", imported.Name);
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
    public async Task ExportThemePackAsync_WritesFileAndReturnsTrue()
    {
        var skin = CreateTestSkin();
        var themes = new Dictionary<string, Skin> { { "Test", skin } };
        var filePath = Path.GetTempFileName();

        var result = await ThemeImportExport.ExportThemePackAsync(themes, filePath, "PackName", "desc");

        Assert.True(result);
        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains("PackName", json);
        Assert.Contains("Test", json);
        File.Delete(filePath);
    }
}