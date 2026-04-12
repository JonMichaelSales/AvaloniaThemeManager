using AvaloniaThemeManager.Models;
using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Tests.Theme;

public class ThemeSelectionStoreTests : IDisposable
{
    private readonly string? _originalTheme = AppSettings.Instance.Theme;

    public void Dispose()
    {
        AppSettings.Instance.Theme = _originalTheme;
    }

    [Fact]
    public void SaveSelectedTheme_PersistsThemeName()
    {
        var store = new AppSettingsThemeSelectionStore();

        store.SaveSelectedTheme("SavedTheme");

        Assert.Equal("SavedTheme", store.GetSavedThemeName());
    }

    [Fact]
    public void SaveSelectedTheme_Null_DoesNotOverwriteCurrentTheme()
    {
        var store = new AppSettingsThemeSelectionStore();
        AppSettings.Instance.Theme = "Original";

        store.SaveSelectedTheme(null);

        Assert.Equal("Original", store.GetSavedThemeName());
    }
}
