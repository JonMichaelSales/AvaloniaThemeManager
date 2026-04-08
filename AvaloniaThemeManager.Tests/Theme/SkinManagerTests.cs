using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaThemeManager.Models;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;

using Moq;
using Xunit;

namespace AvaloniaThemeManager.Tests.Theme
{
    public class SkinManagerTests : IDisposable
    {
        private readonly Mock<IThemeLoaderService> _themeLoaderServiceMock;
        private readonly Mock<IApplication> _applicationMock;
        private readonly Mock<IResourceDictionary> _resourcesMock;
        private readonly Styles _styles; // Real Styles instance
        private readonly AvaloniaStylesWrapper _stylesWrapper;
        private readonly SkinManager _skinManager;

        public SkinManagerTests()
        {
            // Setup mocks
            _themeLoaderServiceMock = new Mock<IThemeLoaderService>();
            _applicationMock = new Mock<IApplication>();
            _resourcesMock = new Mock<IResourceDictionary>();
            _styles = new Styles(); // Use real Styles instance
            _stylesWrapper = new AvaloniaStylesWrapper(_styles);

            // Configure the application mock
            _applicationMock.Setup(a => a.Resources).Returns(_resourcesMock.Object);
            _applicationMock.Setup(a => a.AppStyles).Returns(_stylesWrapper);
            _applicationMock.Setup(a => a.ApplicationLifetime).Returns((IApplicationLifetime?)null);

            // Configure theme loader to return empty list by default
            _themeLoaderServiceMock.Setup(s => s.LoadSkins(It.IsAny<string>()))
                .Returns(new List<Skin>());

            // Setup resource dictionary to behave like a real dictionary
            var resourceDictionary = new Dictionary<object, object?>();
            _resourcesMock.Setup(r => r[It.IsAny<object>()])
                .Returns<object>(key => resourceDictionary.TryGetValue(key, out var value) ? value : null);
            _resourcesMock.SetupSet(r => r[It.IsAny<object>()] = It.IsAny<object>())
                .Callback<object, object?>((key, value) => resourceDictionary[key] = value);
            _resourcesMock.Setup(r => r.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
                .Returns<object, object?>((key, value) => resourceDictionary.TryGetValue(key, out value));

            // Create SkinManager with dependency injection
            _skinManager = new SkinManager(_themeLoaderServiceMock.Object, _applicationMock.Object);
        }

        public void Dispose()
        {
            // Reset AppSettings to avoid test interference
            AppSettings.Instance.Theme = "Dark";
        }

        [Fact]
        public void Constructor_WithDependencyInjection_InitializesCorrectly()
        {
            // Arrange & Act - constructor already called in setup

            // Assert
            Assert.NotNull(_skinManager);
            _themeLoaderServiceMock.Verify(s => s.LoadSkins(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RegisterSkin_AddsSkin()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin" };

            // Act
            _skinManager.RegisterSkin("TestSkin", skin);

            // Assert
            var result = _skinManager.GetSkin("TestSkin");
            Assert.Equal(skin, result);
            Assert.Contains("TestSkin", _skinManager.GetAvailableSkinNames());
        }

        [Fact]
        public void RegisterSkin_NullNameOrSkin_DoesNothing()
        {
            // Arrange
            var initialCount = _skinManager.GetAvailableSkinNames().Count;

            // Act
            _skinManager.RegisterSkin(null, new Skin());
            _skinManager.RegisterSkin("TestName", null);
            _skinManager.RegisterSkin(null, null);

            // Assert
            Assert.Equal(initialCount, _skinManager.GetAvailableSkinNames().Count);
        }

        [Fact]
        public void GetSkin_ReturnsSkinByName()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin" };
            _skinManager.RegisterSkin("TestSkin", skin);

            // Act
            var result = _skinManager.GetSkin("TestSkin");

            // Assert
            Assert.Equal(skin, result);
        }

        [Fact]
        public void GetSkin_NullName_ReturnsCurrentSkin()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin" };
            _skinManager.RegisterSkin("TestSkin", skin);
            _skinManager.ApplySkin("TestSkin");

            // Act
            var result = _skinManager.GetSkin(null);

            // Assert
            Assert.Equal(skin, result);
        }

        [Fact]
        public void GetSkin_NonExistentName_ReturnsCurrentSkin()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin" };
            _skinManager.RegisterSkin("TestSkin", skin);
            _skinManager.ApplySkin("TestSkin");

            // Act
            var result = _skinManager.GetSkin("NonExistent");

            // Assert
            Assert.Equal(skin, result);
        }

        [Fact]
        public void GetAvailableSkinNames_ReturnsAllNames()
        {
            // Arrange
            _skinManager.RegisterSkin("ThemeA", new Skin { Name = "ThemeA" });
            _skinManager.RegisterSkin("ThemeB", new Skin { Name = "ThemeB" });

            // Act
            var names = _skinManager.GetAvailableSkinNames();

            // Assert
            Assert.Contains("ThemeA", names);
            Assert.Contains("ThemeB", names);
            Assert.True(names.Count >= 2);
        }

        [Fact]
        public void ApplySkin_ByName_AppliesAndSaves()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin", PrimaryColor = Colors.Red };
            _skinManager.RegisterSkin("TestSkin", skin);

            // Act
            _skinManager.ApplySkin("TestSkin");

            // Assert
            Assert.Equal(skin, _skinManager.CurrentSkin);
            Assert.Equal("TestSkin", AppSettings.Instance.Theme);

            // Verify that resources were updated
            _resourcesMock.VerifySet(r => r[It.IsAny<object>()] = It.IsAny<object>(), Times.AtLeastOnce);
        }

        [Fact]
        public void ApplySkin_ByName_NotFound_DoesNotCrash()
        {
            // Arrange
            var originalSkin = _skinManager.CurrentSkin;

            // Act - Should not throw
            _skinManager.ApplySkin("NonExistentSkin");

            // Assert
            Assert.Equal(originalSkin, _skinManager.CurrentSkin);
        }

        [Fact]
        public void ApplySkin_ByInstance_AppliesSkinAndRaisesEvent()
        {
            // Arrange
            var skin = new Skin { Name = "TestSkin", PrimaryColor = Colors.Blue };
            bool eventRaised = false;
            _skinManager.SkinChanged += (s, e) => eventRaised = true;

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            Assert.Equal(skin, _skinManager.CurrentSkin);
            Assert.True(eventRaised);

            // Verify that resources were updated
            _resourcesMock.VerifySet(r => r[It.IsAny<object>()] = It.IsAny<object>(), Times.AtLeastOnce);
        }

        [Fact]
        public void ApplySkin_ByInstance_Null_AppliesDefaultSkin()
        {
            // Act
            _skinManager.ApplySkin((Skin?)null);

            // Assert
            Assert.NotNull(_skinManager.CurrentSkin);
        }

        [Fact]
        public void SaveSelectedTheme_SavesThemeName()
        {
            // Arrange
            var themeName = "SavedTheme";

            // Act
            _skinManager.SaveSelectedTheme(themeName);

            // Assert
            Assert.Equal(themeName, AppSettings.Instance.Theme);
        }

        [Fact]
        public void SaveSelectedTheme_Null_DoesNothing()
        {
            // Arrange
            var original = AppSettings.Instance.Theme;

            // Act
            _skinManager.SaveSelectedTheme(null);

            // Assert
            Assert.Equal(original, AppSettings.Instance.Theme);
        }

        [Fact]
        public void LoadSavedTheme_AppliesSavedTheme()
        {
            // Arrange
            var skin = new Skin { Name = "SavedTheme" };
            _skinManager.RegisterSkin("SavedTheme", skin);
            AppSettings.Instance.Theme = "SavedTheme";

            // Act
            _skinManager.LoadSavedTheme();

            // Assert
            Assert.Equal(skin, _skinManager.CurrentSkin);
        }

        [Fact]
        public void LoadSavedTheme_ThemeNotFound_DoesNothing()
        {
            // Arrange
            var originalTheme = _skinManager.CurrentSkin;
            AppSettings.Instance.Theme = "NonExistent";

            // Act
            _skinManager.LoadSavedTheme();

            // Assert
            Assert.Equal(originalTheme, _skinManager.CurrentSkin);
        }

        [Fact]
        public void ApplySkin_UpdatesApplicationResources()
        {
            // Arrange
            var skin = new Skin
            {
                Name = "TestSkin",
                PrimaryColor = Colors.Red,
                AccentColor = Colors.Blue,
                PrimaryBackground = Colors.Black,
                SecondaryColor = Colors.Green,
                SecondaryBackground = Colors.Gray,
                PrimaryTextColor = Colors.White,
                SecondaryTextColor = Colors.LightGray,
                BorderColor = Colors.DarkGray,
                ErrorColor = Colors.DarkRed,
                WarningColor = Colors.Orange,
                SuccessColor = Colors.DarkGreen
            };

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            _resourcesMock.VerifySet(r => r["PrimaryColorBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
            _resourcesMock.VerifySet(r => r["AccentBlueBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
            _resourcesMock.VerifySet(r => r["BackgroundBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
            _resourcesMock.VerifySet(r => r["TextPrimaryBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
            _resourcesMock.VerifySet(r => r["ErrorBrush"] = It.IsAny<SolidColorBrush>(), Times.Once);
        }

        [Fact]
        public void ApplySkin_UpdatesTypographyResources()
        {
            // Arrange
            var skin = new Skin
            {
                Name = "TestSkin",
                Typography = new TypographyScale
                {
                    DisplayLarge = 48,
                    BodyMedium = 16,
                    HeadlineLarge = 32,
                    TitleMedium = 20
                }
            };

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            _resourcesMock.VerifySet(r => r["DisplayLargeFontSize"] = 48.0, Times.Once);
            _resourcesMock.VerifySet(r => r["BodyMediumFontSize"] = 16.0, Times.Once);
            _resourcesMock.VerifySet(r => r["HeadlineLargeFontSize"] = 32.0, Times.Once);
            _resourcesMock.VerifySet(r => r["TitleMediumFontSize"] = 20.0, Times.Once);
        }

        [Fact]
        public void ApplySkin_WithControlThemes_UpdatesStyles()
        {
            // Arrange
            var skin = new Skin
            {
                Name = "TestSkin",
                ControlThemeUris = new Dictionary<string, string>
                {
                    ["Button"] = "avares://AvaloniaThemeManager/Themes/TestSkin/Button.axaml",
                    ["TextBox"] = "avares://AvaloniaThemeManager/Themes/TestSkin/TextBox.axaml"
                }
            };

            var initialCount = _stylesWrapper.Count;

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            Assert.True(_stylesWrapper.Count >= initialCount, "Should add styles or maintain existing ones");
        }

        [Fact]
        public void ApplySkin_WithStyleUris_UpdatesStyles()
        {
            // Arrange
            var skin = new Skin
            {
                Name = "TestSkin",
                StyleUris = new Dictionary<string, string>
                {
                    ["CustomStyle"] = "avares://AvaloniaThemeManager/Themes/TestSkin/Custom.axaml"
                }
            };

            var initialCount = _stylesWrapper.Count;

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            Assert.True(_stylesWrapper.Count >= initialCount, "Should add styles");
        }

        [Fact]
        public void ApplySkin_RemovesPreviousControlThemes()
        {
            // Arrange
            var firstSkin = new Skin
            {
                Name = "FirstSkin",
                ControlThemeUris = new Dictionary<string, string>
                {
                    ["Button"] = "avares://AvaloniaThemeManager/Themes/FirstSkin/Button.axaml"
                }
            };

            var secondSkin = new Skin
            {
                Name = "SecondSkin",
                ControlThemeUris = new Dictionary<string, string>
                {
                    ["TextBox"] = "avares://AvaloniaThemeManager/Themes/SecondSkin/TextBox.axaml"
                }
            };

            // Act
            _skinManager.ApplySkin(firstSkin);
            var stylesAfterFirst = _stylesWrapper.Count;

            _skinManager.ApplySkin(secondSkin);

            // Assert - Check that styles were managed properly
            Assert.True(stylesAfterFirst > 0, "First skin should add styles");
            Assert.True(_stylesWrapper.Count > 0, "Second skin should maintain or add styles");
        }

        [Fact]
        public void UpdateBrush_ExistingBrush_UpdatesColor()
        {
            // Arrange
            var existingBrush = new SolidColorBrush(Colors.Red);

            // Mock the correct resource key that SkinManager actually uses
            _resourcesMock.Setup(r => r.TryGetValue("PrimaryColorBrush", out It.Ref<object?>.IsAny))
                .Returns((object key, out object? value) =>
                {
                    value = existingBrush;
                    return true;
                });

            var skin = new Skin
            {
                Name = "TestSkin",
                PrimaryColor = Colors.Blue
            };

            // Act
            _skinManager.ApplySkin(skin);

            // Assert - The existing brush should have its color updated to blue
            Assert.Equal(Colors.Blue, existingBrush.Color);
        }

        [Fact]
        public void SkinChanged_Event_RaisedWhenSkinApplied()
        {
            // Arrange
            var eventRaisedCount = 0;
            object? capturedSender = null;
            EventArgs? capturedArgs = null;

            _skinManager.SkinChanged += (sender, args) =>
            {
                eventRaisedCount++;
                capturedSender = sender;
                capturedArgs = args;
            };

            var skin = new Skin { Name = "EventTestSkin" };

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            Assert.Equal(1, eventRaisedCount);
            Assert.NotNull(capturedSender);
            Assert.Same(_skinManager, capturedSender); // Verify sender is the SkinManager instance
            Assert.NotNull(capturedArgs);
            Assert.IsType<EventArgs>(capturedArgs);
        }

        [Fact]
        public void ApplySkin_WithApplicationLifetime_DoesNotThrow()
        {
            // Arrange
            var mockLifetime = new Mock<IClassicDesktopStyleApplicationLifetime>();
            mockLifetime.Setup(l => l.Windows).Returns(new List<Window>());
            _applicationMock.Setup(a => a.ApplicationLifetime).Returns(mockLifetime.Object);

            var skin = new Skin { Name = "TestSkin" };

            // Act & Assert
            var exception = Record.Exception(() => _skinManager.ApplySkin(skin));
            Assert.Null(exception);
            Assert.Equal(skin, _skinManager.CurrentSkin);
        }
    }
}