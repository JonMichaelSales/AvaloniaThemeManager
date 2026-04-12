using Avalonia.Media;
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Theme.ValidationRules;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeValidatorTests
{
    [Fact]
    public void ValidateTheme_ReturnsAggregatedErrorsAndWarnings()
    {
        var mockRule1 = new Mock<IThemeValidationRule>();
        var mockRule2 = new Mock<IThemeValidationRule>();
        var skin = new Skin();

        mockRule1.Setup(r => r.Validate(skin)).Returns(new ThemeValidationResult
        {
            Errors = new List<string> { "Error1" },
            Warnings = new List<string> { "Warning1" }
        });
        mockRule2.Setup(r => r.Validate(skin)).Returns(new ThemeValidationResult
        {
            Errors = new List<string> { "Error2" },
            Warnings = new List<string> { "Warning2" }
        });

        var validator = new ThemeValidator(
            new[] { mockRule1.Object, mockRule2.Object },
            new ThemeValidationHelper(),
            new ThemeAutoFixer());

        var result = validator.ValidateTheme(skin);

        Assert.False(result.IsValid);
        Assert.Contains("Error1", result.Errors);
        Assert.Contains("Error2", result.Errors);
        Assert.Contains("Warning1", result.Warnings);
        Assert.Contains("Warning2", result.Warnings);
    }

    [Fact]
    public void ValidateTheme_IsValidTrueWhenNoErrors()
    {
        var mockRule = new Mock<IThemeValidationRule>();
        var skin = new Skin();
        mockRule.Setup(r => r.Validate(skin)).Returns(new ThemeValidationResult
        {
            Errors = new List<string>(),
            Warnings = new List<string>()
        });

        var validator = new ThemeValidator(new[] { mockRule.Object }, new ThemeValidationHelper(), new ThemeAutoFixer());

        var result = validator.ValidateTheme(skin);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void AutoFixTheme_DelegatesToThemeAutoFixerBehavior()
    {
        var skin = new Skin
        {
            Name = null,
            FontSizeSmall = 2,
            FontSizeMedium = 100,
            FontSizeLarge = -5,
            BorderRadius = -10,
            PrimaryTextColor = Colors.Black,
            PrimaryBackground = Colors.White,
            SecondaryTextColor = Colors.Black,
            SecondaryBackground = Colors.White
        };

        var validator = new ThemeValidator();
        var fixer = new ThemeAutoFixer();

        var fixedSkin = validator.AutoFixTheme(skin);
        var directFixedSkin = fixer.AutoFixTheme(skin);

        Assert.Equal("Custom Theme", fixedSkin.Name);
        Assert.InRange(fixedSkin.FontSizeSmall, 8, 20);
        Assert.InRange(fixedSkin.FontSizeMedium, 10, 24);
        Assert.InRange(fixedSkin.FontSizeLarge, 12, 32);
        Assert.True(fixedSkin.BorderRadius >= 0);
        Assert.Equal(directFixedSkin.Name, fixedSkin.Name);
        Assert.Equal(directFixedSkin.FontSizeSmall, fixedSkin.FontSizeSmall);
        Assert.Equal(directFixedSkin.FontSizeMedium, fixedSkin.FontSizeMedium);
        Assert.Equal(directFixedSkin.FontSizeLarge, fixedSkin.FontSizeLarge);
        Assert.Equal(directFixedSkin.BorderRadius, fixedSkin.BorderRadius);
        Assert.Equal(directFixedSkin.PrimaryTextColor, fixedSkin.PrimaryTextColor);
        Assert.Equal(directFixedSkin.SecondaryTextColor, fixedSkin.SecondaryTextColor);
    }

    [Fact]
    public void AutoFixTheme_ClonesSkinAndDoesNotMutateOriginal()
    {
        var skin = new Skin
        {
            Name = "Original",
            FontSizeSmall = 10,
            BorderRadius = 5
        };

        var validator = new ThemeValidator();

        var fixedSkin = validator.AutoFixTheme(skin);

        Assert.NotSame(skin, fixedSkin);
        Assert.Equal("Original", skin.Name);
        Assert.Equal(10, skin.FontSizeSmall);
        Assert.Equal(5, skin.BorderRadius);
    }

    [Theory]
    [InlineData(255, 255, 255, 0, 0, 0, 21.0)]
    [InlineData(0, 0, 0, 255, 255, 255, 21.0)]
    [InlineData(128, 128, 128, 128, 128, 128, 1.0)]
    public void CalculateContrastRatio_ReturnsExpectedRatio(
        byte fr, byte fg, byte fb, byte br, byte bg, byte bb, double expected)
    {
        var validator = new ThemeValidator();
        var foreground = Color.FromRgb(fr, fg, fb);
        var background = Color.FromRgb(br, bg, bb);

        var ratio = validator.CalculateContrastRatio(foreground, background);

        Assert.Equal(expected, ratio, 1);
    }

    [Fact]
    public void ParameterlessAndDiValidators_UseSameDefaultPolicy()
    {
        var services = new ServiceCollection();
        services.AddThemeManagerServices();
        using var provider = services.BuildServiceProvider();

        var directValidator = new ThemeValidator();
        var diValidator = provider.GetRequiredService<IThemeValidator>();
        var skin = new Skin
        {
            Name = "Test",
            FontSizeSmall = 6,
            FontSizeMedium = 6,
            FontSizeLarge = 6,
            BorderThickness = new Avalonia.Thickness(0),
            BorderColor = Colors.White,
            PrimaryBackground = Colors.White,
            SecondaryBackground = Colors.White,
            PrimaryTextColor = Colors.White,
            SecondaryTextColor = Colors.White
        };

        var directResult = directValidator.ValidateTheme(skin);
        var diResult = diValidator.ValidateTheme(skin);

        Assert.Equal(directResult.IsValid, diResult.IsValid);
        Assert.Equal(directResult.Errors.OrderBy(error => error), diResult.Errors.OrderBy(error => error));
        Assert.Equal(directResult.Warnings.OrderBy(warning => warning), diResult.Warnings.OrderBy(warning => warning));
    }

    [Fact]
    public void AddThemeManagerServices_RegistersUnifiedValidationRuleSet()
    {
        var services = new ServiceCollection();
        services.AddThemeManagerServices();
        using var provider = services.BuildServiceProvider();

        var ruleTypes = provider.GetServices<IThemeValidationRule>()
            .Select(rule => rule.GetType())
            .ToList();

        Assert.Contains(typeof(ColorContrastValidationRule), ruleTypes);
        Assert.Contains(typeof(FontSizeValidationRule), ruleTypes);
        Assert.Contains(typeof(BorderValidationRule), ruleTypes);
        Assert.Contains(typeof(NameValidationRule), ruleTypes);
        Assert.Contains(typeof(AccessibilityValidationRule), ruleTypes);
        Assert.IsType<ThemeValidator>(provider.GetRequiredService<IThemeValidator>());
        Assert.IsType<ThemeAutoFixer>(provider.GetRequiredService<IThemeAutoFixer>());
    }

    [Fact]
    public void AddThemeManagerServices_ValidatorAndAutoFixerProduceEquivalentAutoFixResults()
    {
        var services = new ServiceCollection();
        services.AddThemeManagerServices();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IThemeValidator>();
        var fixer = provider.GetRequiredService<IThemeAutoFixer>();
        var skin = new Skin
        {
            Name = null,
            FontSizeSmall = 2,
            FontSizeMedium = 100,
            FontSizeLarge = -5,
            BorderRadius = -10,
            PrimaryTextColor = Colors.Black,
            PrimaryBackground = Colors.White,
            SecondaryTextColor = Colors.Black,
            SecondaryBackground = Colors.White
        };

        var validatorResult = validator.AutoFixTheme(skin);
        var fixerResult = fixer.AutoFixTheme(skin);

        Assert.Equal(fixerResult.Name, validatorResult.Name);
        Assert.Equal(fixerResult.FontSizeSmall, validatorResult.FontSizeSmall);
        Assert.Equal(fixerResult.FontSizeMedium, validatorResult.FontSizeMedium);
        Assert.Equal(fixerResult.FontSizeLarge, validatorResult.FontSizeLarge);
        Assert.Equal(fixerResult.BorderRadius, validatorResult.BorderRadius);
        Assert.Equal(fixerResult.PrimaryTextColor, validatorResult.PrimaryTextColor);
        Assert.Equal(fixerResult.SecondaryTextColor, validatorResult.SecondaryTextColor);
    }

    [Fact]
    public void ColorContrastValidationRule_UsesInjectedHelper()
    {
        var helperMock = new Mock<IThemeValidationHelper>();
        helperMock.SetupSequence(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()))
            .Returns(4.0)
            .Returns(3.5)
            .Returns(2.5);

        var rule = new ColorContrastValidationRule(helperMock.Object);
        var result = rule.Validate(new Skin());

        helperMock.Verify(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()), Times.Exactly(3));
        Assert.NotEmpty(result.Errors);
        Assert.NotEmpty(result.Warnings);
    }

    [Fact]
    public void BorderValidationRule_UsesInjectedHelper()
    {
        var helperMock = new Mock<IThemeValidationHelper>();
        helperMock.SetupSequence(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()))
            .Returns(1.0)
            .Returns(1.0)
            .Returns(1.0);

        var rule = new BorderValidationRule(helperMock.Object);
        var result = rule.Validate(new Skin());

        helperMock.Verify(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()), Times.Exactly(3));
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void AccessibilityValidationRule_UsesInjectedHelper()
    {
        var helperMock = new Mock<IThemeValidationHelper>();
        helperMock.Setup(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()))
            .Returns(2.0);

        var rule = new AccessibilityValidationRule(helperMock.Object);
        var result = rule.Validate(new Skin());

        helperMock.Verify(h => h.CalculateContrastRatio(It.IsAny<Color>(), It.IsAny<Color>()), Times.AtLeastOnce);
        Assert.NotEmpty(result.Errors);
    }
}
