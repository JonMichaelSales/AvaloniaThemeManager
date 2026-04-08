using Avalonia.Media;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Theme;
using global::AvaloniaThemeManager.Services.Interfaces;
using Moq;

namespace AvaloniaThemeManager.Tests.Integration
{
    public class SkinManagerIntegrationTests : IDisposable
    {
        private readonly TestApplication _testApp;
        private readonly SkinManager _skinManager;

        public SkinManagerIntegrationTests()
        {
            _testApp = new TestApplication();
            var themeLoader = new Mock<IThemeLoaderService>();
            themeLoader.Setup(s => s.LoadSkins(It.IsAny<string>())).Returns(new List<Skin>());

            // Create a mock or wrapper for IApplication
            var applicationMock = new Mock<IApplication>();
            applicationMock.Setup(a => a.Resources).Returns(_testApp.Resources);
            applicationMock.Setup(a => a.ApplicationLifetime).Returns(_testApp.ApplicationLifetime);

            // Wrap the Avalonia Styles in your IStylesCollection wrapper
            applicationMock.Setup(a => a.AppStyles).Returns(new AvaloniaStylesWrapper(_testApp.Styles));

            // Use the public constructor - no reflection needed!
            _skinManager = new SkinManager(themeLoader.Object, applicationMock.Object);
        }

        [Fact]
        public void ApplySkin_WithRealApplication_UpdatesResources()
        {
            // Arrange
            var skin = new Skin
            {
                Name = "IntegrationTestSkin",
                PrimaryColor = Colors.Red,
                AccentColor = Colors.Blue
            };

            // Act
            _skinManager.ApplySkin(skin);

            // Assert
            Assert.True(_testApp.Resources.ContainsKey("PrimaryColorBrush"));
            Assert.True(_testApp.Resources.ContainsKey("AccentBlueBrush"));

            var primaryBrush = _testApp.Resources["PrimaryColorBrush"] as SolidColorBrush;
            var accentBrush = _testApp.Resources["AccentBlueBrush"] as SolidColorBrush;

            Assert.NotNull(primaryBrush);
            Assert.NotNull(accentBrush);
            Assert.Equal(Colors.Red, primaryBrush.Color);
            Assert.Equal(Colors.Blue, accentBrush.Color);
        }

        public void Dispose()
        {
            _testApp?.Dispose();
        }
    }
}