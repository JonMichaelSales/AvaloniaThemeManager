# AvaloniaThemeManager

A comprehensive theme management library for Avalonia UI applications with multiple built-in themes, dynamic theme switching, and extensive control styling.

[![NuGet Version](https://img.shields.io/nuget/v/AvaloniaThemeManager.svg)](https://www.nuget.org/packages/AvaloniaThemeManager/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AvaloniaThemeManager.svg)](https://www.nuget.org/packages/AvaloniaThemeManager/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

- **7 Built-in Themes**: Dark, Light, Ocean Blue, Forest Green, Purple Haze, High Contrast, and Cyberpunk
- **Dynamic Theme Switching**: Change themes at runtime with smooth transitions
- **Persistent Settings**: Automatically saves and restores user theme preferences
- **Comprehensive Styling**: Extensive theme support for all major Avalonia controls
- **Quick Switcher Control**: Ready-to-use theme switcher component
- **Theme Settings Dialog**: Complete settings UI for theme management
- **MVVM Architecture**: Reactive and clean separation of concerns
- **Easy Integration**: Simple setup with AppBuilder extensions

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package AvaloniaThemeManager
```

Or via Package Manager Console:

```powershell
Install-Package AvaloniaThemeManager
```

## Quick Start

### 1. Setup in Program.cs

```csharp
using Avalonia;
using AvaloniaThemeManager.Extensions;

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseThemeManager() // Add this line
        .LogToTrace()
        .UseReactiveUI();
```

### 2. Include Styles in App.axaml

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="YourApp.App">
    <Application.Styles>
        <FluentTheme />
        <!-- Include AvaloniaThemeManager styles -->
        <StyleInclude Source="avares://AvaloniaThemeManager/Themes/CustomThemes.axaml" />
    </Application.Styles>
</Application>
```

### 3. Basic Theme Switching

```csharp
using AvaloniaThemeManager.Theme;

// Switch to a different theme
SkinManager.Instance.ApplySkin("Dark");
SkinManager.Instance.ApplySkin("Ocean Blue");
SkinManager.Instance.ApplySkin("Cyberpunk");

// Get available themes
var availableThemes = SkinManager.Instance.GetAvailableSkinNames();

// Listen for theme changes
SkinManager.Instance.SkinChanged += (sender, args) =>
{
    Console.WriteLine("Theme changed!");
};
```

## Built-in Themes

| Theme Name | Description | Preview |
|------------|-------------|---------|
| **Dark** | Professional dark theme with blue accents | ![Dark Theme](https://via.placeholder.com/100x60/2C313D/FFFFFF?text=Dark) |
| **Light** | Clean light theme perfect for bright environments | ![Light Theme](https://via.placeholder.com/100x60/F5F5F5/333333?text=Light) |
| **Ocean Blue** | Deep blue theme inspired by ocean depths | ![Ocean Blue](https://via.placeholder.com/100x60/0F2A4A/FFFFFF?text=Ocean) |
| **Forest Green** | Nature-inspired green theme | ![Forest Green](https://via.placeholder.com/100x60/1B3A2D/FFFFFF?text=Forest) |
| **Purple Haze** | Rich purple theme with mystical vibes | ![Purple Haze](https://via.placeholder.com/100x60/301E4E/FFFFFF?text=Purple) |
| **High Contrast** | Maximum contrast for accessibility | ![High Contrast](https://via.placeholder.com/100x60/000000/FFFFFF?text=Contrast) |
| **Cyberpunk** | Futuristic neon theme with hot pink accents | ![Cyberpunk](https://via.placeholder.com/100x60/0A0321/F0F0FF?text=Cyber) |

## Usage Examples

### Using the Quick Theme Switcher Control

```xml
<UserControl xmlns:controls="clr-namespace:AvaloniaThemeManager.Controls;assembly=AvaloniaThemeManager">
    <StackPanel>
        <!-- Quick theme switcher dropdown -->
        <controls:QuickThemeSwitcher />
        
        <!-- Your other UI elements -->
        <Button Content="Sample Button" />
        <TextBox Watermark="Sample TextBox" />
    </StackPanel>
</UserControl>
```

### Opening the Theme Settings Dialog

```csharp
using AvaloniaThemeManager.Views;

private async void OpenThemeSettings()
{
    var dialog = new ThemeSettingsDialog();
    await dialog.ShowDialog(this); // 'this' is your parent window
}
```

### Creating Custom Themes

```csharp
using AvaloniaThemeManager.Theme;
using Avalonia.Media;

// Create a custom theme
var customTheme = new Skin
{
    Name = "My Custom Theme",
    PrimaryColor = Color.Parse("#FF6B6B"),
    SecondaryColor = Color.Parse("#4ECDC4"),
    AccentColor = Color.Parse("#45B7D1"),
    PrimaryBackground = Color.Parse("#2C3E50"),
    SecondaryBackground = Color.Parse("#34495E"),
    PrimaryTextColor = Color.Parse("#FFFFFF"),
    SecondaryTextColor = Color.Parse("#BDC3C7"),
    BorderColor = Color.Parse("#7F8C8D"),
    ErrorColor = Color.Parse("#E74C3C"),
    WarningColor = Color.Parse("#F39C12"),
    SuccessColor = Color.Parse("#2ECC71")
};

// Register and apply the custom theme
SkinManager.Instance.RegisterSkin("Custom", customTheme);
SkinManager.Instance.ApplySkin("Custom");
```

### Advanced Configuration

```csharp
// Configure theme manager during startup
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseThemeManager(manager =>
        {
            // Register custom themes
            manager.RegisterSkin("Corporate", corporateTheme);
            
            // Set default theme
            manager.ApplySkin("Dark");
        })
        .LogToTrace()
        .UseReactiveUI();
```

## MVVM Integration

### Theme Settings ViewModel

```csharp
using AvaloniaThemeManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ThemeSettingsViewModel ThemeSettings { get; }
    
    public MainWindowViewModel()
    {
        ThemeSettings = new ThemeSettingsViewModel();
    }
}
```

### Data Binding in XAML

```xml
<Window xmlns:vm="clr-namespace:AvaloniaThemeManager.ViewModels;assembly=AvaloniaThemeManager">
    <ComboBox ItemsSource="{Binding ThemeSettings.AvailableThemes}"
              SelectedItem="{Binding ThemeSettings.SelectedTheme}">
        <ComboBox.ItemTemplate>
            <DataTemplate>
                <StackPanel Orientation="Horizontal" Spacing="8">
                    <Ellipse Width="12" Height="12" Fill="{Binding PreviewColor}" />
                    <TextBlock Text="{Binding Name}" />
                </StackPanel>
            </DataTemplate>
        </ComboBox.ItemTemplate>
    </ComboBox>
</Window>
```

## Styled Controls

AvaloniaThemeManager provides comprehensive styling for:

- **Buttons** (Primary, Secondary, Browse, Toolbar variants)
- **TextBoxes** (Standard, Dialog variants)
- **ComboBoxes** and ComboBoxItems
- **CheckBoxes** with custom styling
- **TabControls** and TabItems
- **Borders** (Default, Card, Status Bar, Toolbar variants)
- **TextBlocks** (Various typography styles)
- **Separators** (Horizontal and Vertical)
- **PathIcons** with multiple size variants
- **Expanders** with animated transitions
- **Windows** (Default and Dialog variants)

## Persistence

Theme preferences are automatically saved to:
- **Windows**: `%LocalAppData%/ResumeForge/appsettings.json`
- **macOS**: `~/Library/Application Support/ResumeForge/appsettings.json`
- **Linux**: `~/.local/share/ResumeForge/appsettings.json`

## Requirements

- **.NET 8.0** or higher
- **Avalonia UI 11.3.0** or higher
- **C# 12** language features

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Clone the repository
2. Open in Visual Studio 2022 or JetBrains Rider
3. Restore NuGet packages
4. Build and run the sample application

### Adding New Themes

1. Create a new `Skin` object with your color scheme
2. Register it in `SkinManager.RegisterDefaultSkins()`
3. Add theme description in `GetThemeDescription()` method
4. Test with all styled controls

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Changelog

### v1.0.0 (2025-01-XX)
- Initial release
- 7 built-in themes
- Comprehensive control styling
- Dynamic theme switching
- Settings persistence
- Quick switcher component
- Theme settings dialog
- MVVM architecture

## Support

- **Issues**: [GitHub Issues](https://github.com/jonsmith/AvaloniaThemeManager/issues)
- **Discussions**: [GitHub Discussions](https://github.com/jonsmith/AvaloniaThemeManager/discussions)
- **Documentation**: [Wiki](https://github.com/jonsmith/AvaloniaThemeManager/wiki)

## Acknowledgments

- Built with [Avalonia UI](https://avaloniaui.net/)
- Inspired by Material Design and Fluent Design principles
- Icons from [Material Design Icons](https://materialdesignicons.com/)

---

**Made with ❤️ for the Avalonia UI community**