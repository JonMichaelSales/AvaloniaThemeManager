using Avalonia.Media;
using AvaloniaThemeManager.Theme;
using Moq;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeValidatorTests
{
    [Fact]
    public void ValidateTheme_ReturnsAggregatedErrorsAndWarnings()
    {
        // Arrange
        var mockRule1 = new Mock<IThemeValidationRule>();
        var mockRule2 = new Mock<IThemeValidationRule>();
        var skin = new Skin();

        var result1 = new ThemeValidationResult
        {
            Errors = new List<string> { "Error1" },
            Warnings = new List<string> { "Warning1" }
        };
        var result2 = new ThemeValidationResult
        {
            Errors = new List<string> { "Error2" },
            Warnings = new List<string> { "Warning2" }
        };

        mockRule1.Setup(r => r.Validate(skin)).Returns(result1);
        mockRule2.Setup(r => r.Validate(skin)).Returns(result2);

        var validator = new ThemeValidatorTestable(mockRule1.Object, mockRule2.Object);

        // Act
        var result = validator.ValidateTheme(skin);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Error1", result.Errors);
        Assert.Contains("Error2", result.Errors);
        Assert.Contains("Warning1", result.Warnings);
        Assert.Contains("Warning2", result.Warnings);
    }

    [Fact]
    public void ValidateTheme_IsValidTrueWhenNoErrors()
    {
        // Arrange
        var mockRule = new Mock<IThemeValidationRule>();
        var skin = new Skin();
        var resultNoErrors = new ThemeValidationResult
        {
            Errors = new List<string>(),
            Warnings = new List<string>()
        };
        mockRule.Setup(r => r.Validate(skin)).Returns(resultNoErrors);

        var validator = new ThemeValidatorTestable(mockRule.Object);

        // Act
        var result = validator.ValidateTheme(skin);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void AutoFixTheme_FixesInvalidNameAndFontSizesAndBorderRadius()
    {
        // Arrange
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

        // Act
        var fixedSkin = validator.AutoFixTheme(skin);

        // Assert
        Assert.Equal("Custom Theme", fixedSkin.Name);
        Assert.InRange(fixedSkin.FontSizeSmall, 8, 20);
        Assert.InRange(fixedSkin.FontSizeMedium, 10, 24);
        Assert.InRange(fixedSkin.FontSizeLarge, 12, 32);
        Assert.True(fixedSkin.BorderRadius >= 0);
    }

    [Fact]
    public void AutoFixTheme_ClonesSkinAndDoesNotMutateOriginal()
    {
        // Arrange
        var skin = new Skin
        {
            Name = "Original",
            FontSizeSmall = 10,
            BorderRadius = 5
        };

        var validator = new ThemeValidator();

        // Act
        var fixedSkin = validator.AutoFixTheme(skin);

        // Assert
        Assert.NotSame(skin, fixedSkin);
        Assert.Equal("Original", skin.Name);
        Assert.Equal(10, skin.FontSizeSmall);
        Assert.Equal(5, skin.BorderRadius);
    }

    [Theory]
    [InlineData(255, 255, 255, 0, 0, 0, 21.0)] // White on Black
    [InlineData(0, 0, 0, 255, 255, 255, 21.0)] // Black on White
    [InlineData(128, 128, 128, 128, 128, 128, 1.0)] // Same color
    public void CalculateContrastRatio_ReturnsExpectedRatio(
        byte fr, byte fg, byte fb, byte br, byte bg, byte bb, double expected)
    {
        // Arrange
        var validator = new ThemeValidator();
        var foreground = Color.FromRgb(fr, fg, fb);
        var background = Color.FromRgb(br, bg, bb);

        // Act
        var ratio = validator.CalculateContrastRatio(foreground, background);

        // Assert
        Assert.Equal(expected, ratio, 1); // Allow small floating point error
    }

    // Helper to inject custom rules for testing
    private class ThemeValidatorTestable : ThemeValidator
    {
        public ThemeValidatorTestable(params IThemeValidationRule[] rules)
        {
            var field = typeof(ThemeValidator)
                .GetField("_validationRules", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(field);
            field.SetValue(this, new List<IThemeValidationRule>(rules));
        }
    }
}
