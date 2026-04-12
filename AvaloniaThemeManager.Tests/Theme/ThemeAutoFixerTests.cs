using Avalonia;
using Avalonia.Media;
using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeAutoFixerTests
{
    private static Skin CreateRuntimeSkin() => new Skin
    {
        Name = "Original Theme",
        PrimaryColor = Color.Parse("#112233"),
        SecondaryColor = Color.Parse("#223344"),
        AccentColor = Color.Parse("#334455"),
        PrimaryBackground = Colors.White,
        SecondaryBackground = Color.Parse("#F2F4F7"),
        PrimaryTextColor = Color.Parse("#F5F5F5"),
        SecondaryTextColor = Color.Parse("#DADADA"),
        FontFamily = new FontFamily("Arial"),
        FontSizeSmall = 6,
        FontSizeMedium = 50,
        FontSizeLarge = 4,
        FontWeight = FontWeight.Bold,
        BorderColor = Color.Parse("#556677"),
        BorderThickness = new Thickness(1, 2, 3, 4),
        BorderRadius = -5,
        ErrorColor = Color.Parse("#880000"),
        WarningColor = Color.Parse("#AA6600"),
        SuccessColor = Color.Parse("#007744"),
        ControlThemeUris = new Dictionary<string, string>
        {
            ["Button"] = "avares://AvaloniaThemeManager/Themes/Test/Button.axaml"
        },
        StyleUris = new Dictionary<string, string>
        {
            ["Card"] = "avares://AvaloniaThemeManager/Themes/Test/Card.axaml"
        },
        Typography = new TypographyScale
        {
            DisplayLarge = 62,
            DisplayMedium = 48,
            DisplaySmall = 36,
            HeadlineLarge = 32,
            HeadlineMedium = 28,
            HeadlineSmall = 24,
            TitleLarge = 20,
            TitleMedium = 18,
            TitleSmall = 16,
            LabelLarge = 14,
            LabelMedium = 12,
            LabelSmall = 10,
            BodyLarge = 18,
            BodyMedium = 16,
            BodySmall = 14
        },
        HeaderFontFamily = new FontFamily("Inter"),
        BodyFontFamily = new FontFamily("Inter"),
        MonospaceFontFamily = new FontFamily("Consolas"),
        LineHeight = 1.8,
        LetterSpacing = 0.25,
        EnableLigatures = false
    };

    [Fact]
    public void AutoFixTheme_DefaultsBlankNameAndNormalizesBasicFields()
    {
        var fixer = new ThemeAutoFixer();
        var skin = CreateRuntimeSkin();
        skin.Name = " ";

        var fixedSkin = fixer.AutoFixTheme(skin);

        Assert.Equal("Custom Theme", fixedSkin.Name);
        Assert.InRange(fixedSkin.FontSizeSmall, 8, 20);
        Assert.InRange(fixedSkin.FontSizeMedium, 10, 24);
        Assert.InRange(fixedSkin.FontSizeLarge, 12, 32);
        Assert.True(fixedSkin.BorderRadius >= 0);
    }

    [Fact]
    public void AutoFixTheme_AdjustsLowContrastTextColors()
    {
        var fixer = new ThemeAutoFixer();
        var skin = CreateRuntimeSkin();

        var fixedSkin = fixer.AutoFixTheme(skin);
        var helper = new ThemeValidationHelper();

        Assert.True(helper.CalculateContrastRatio(fixedSkin.PrimaryTextColor, fixedSkin.PrimaryBackground) >= 4.5);
        Assert.True(helper.CalculateContrastRatio(fixedSkin.SecondaryTextColor, fixedSkin.SecondaryBackground) >= 3.0);
    }

    [Fact]
    public void AutoFixTheme_DoesNotMutateOriginalTheme()
    {
        var fixer = new ThemeAutoFixer();
        var skin = CreateRuntimeSkin();

        var fixedSkin = fixer.AutoFixTheme(skin);

        Assert.NotSame(skin, fixedSkin);
        Assert.Equal("Original Theme", skin.Name);
        Assert.Equal(6, skin.FontSizeSmall);
        Assert.Equal(50, skin.FontSizeMedium);
        Assert.Equal(4, skin.FontSizeLarge);
        Assert.Equal(-5, skin.BorderRadius);
        Assert.Equal(Color.Parse("#F5F5F5"), skin.PrimaryTextColor);
        Assert.Equal(Color.Parse("#DADADA"), skin.SecondaryTextColor);
    }

    [Fact]
    public void AutoFixTheme_PreservesAdvancedRuntimePropertiesWhenCloning()
    {
        var fixer = new ThemeAutoFixer();
        var skin = CreateRuntimeSkin();

        var fixedSkin = fixer.AutoFixTheme(skin);

        Assert.Equal(skin.ControlThemeUris, fixedSkin.ControlThemeUris);
        Assert.Equal(skin.StyleUris, fixedSkin.StyleUris);
        Assert.Equal(skin.Typography.DisplayLarge, fixedSkin.Typography.DisplayLarge);
        Assert.Equal(skin.Typography.TitleMedium, fixedSkin.Typography.TitleMedium);
        Assert.Equal(skin.Typography.BodySmall, fixedSkin.Typography.BodySmall);
        Assert.Equal(skin.HeaderFontFamily.ToString(), fixedSkin.HeaderFontFamily.ToString());
        Assert.Equal(skin.BodyFontFamily.ToString(), fixedSkin.BodyFontFamily.ToString());
        Assert.Equal(skin.MonospaceFontFamily.ToString(), fixedSkin.MonospaceFontFamily.ToString());
        Assert.Equal(skin.LineHeight, fixedSkin.LineHeight);
        Assert.Equal(skin.LetterSpacing, fixedSkin.LetterSpacing);
        Assert.Equal(skin.EnableLigatures, fixedSkin.EnableLigatures);
    }
}
