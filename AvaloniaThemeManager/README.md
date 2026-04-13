# AvaloniaThemeManager

Theme management library for Avalonia UI applications with built-in themes, runtime theme switching, import/export, validation, and ready-made theme UI components.

[![NuGet Version](https://img.shields.io/nuget/v/AvaloniaSkinManager.svg)](https://www.nuget.org/packages/AvaloniaSkinManager/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AvaloniaSkinManager.svg)](https://www.nuget.org/packages/AvaloniaSkinManager/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Package

Install the published package:

```bash
dotnet add package AvaloniaSkinManager
```

## What it provides

- 12 built-in themes, including `Dark`, `Light`, `Ocean Blue`, `ModernIce`, `RetroTerminal`, and `Material Design 3`
- runtime theme switching through `ISkinManager`
- saved-theme restoration during app startup
- theme import/export and theme-pack export helpers
- validation and auto-fix services for custom themes
- ready-to-use UI components:
  - `QuickThemeSwitcher`
  - `ThemeSettingsDialog`
  - `ThemeManagerDemoView`

## Quick start

### 1. Configure the app

```csharp
using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaThemeManager.Extensions;

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseThemeManager()
        .WithInterFont()
        .UseReactiveUI();
```

### 2. Merge the library theme resources

```xml
<Application
    x:Class="YourApp.App"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceInclude Source="avares://AvaloniaThemeManager/Themes/CustomThemes.axaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>

    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```

### 3. Resolve and use `ISkinManager`

```csharp
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Theme;

var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();

skinManager.ApplySkin("Dark");
var availableThemes = skinManager.GetAvailableSkinNames();
```

## DI-friendly usage

The library now prefers explicit dependency injection. Compatibility constructors still exist on a few UI types, but new code should pass dependencies explicitly.

### Open the theme settings dialog

```csharp
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging.Abstractions;

var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
var dialog = new ThemeSettingsDialog(skinManager, NullLogger.Instance);
await dialog.ShowDialog(this);
```

### Use the quick switcher

```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="clr-namespace:AvaloniaThemeManager.Controls;assembly=AvaloniaThemeManager">
    <controls:QuickThemeSwitcher />
</UserControl>
```

### Use the demo view

```csharp
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging.Abstractions;

var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
var dialogService = AppBuilderExtensions.GetRequiredService<IDialogService>();

var demoView = new ThemeManagerDemoView(
    skinManager,
    NullLogger.Instance,
    dialogService);
```

## Import, export, and validation

```csharp
using AvaloniaThemeManager.Theme;

var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
var validator = AppBuilderExtensions.GetRequiredService<IThemeValidator>();

var currentSkin = skinManager.CurrentSkin;
var validation = validator.ValidateTheme(currentSkin!);

if (validation.IsValid)
{
    await ThemeImportExport.ExportThemeAsync(currentSkin!, "theme.json");
}
```

The default export path now writes the full runtime theme model, including typography and control/style URI metadata.

## Custom themes

```csharp
using Avalonia.Media;
using AvaloniaThemeManager.Theme;

var customTheme = new Skin
{
    Name = "My Theme",
    PrimaryColor = Color.Parse("#343B48"),
    SecondaryColor = Color.Parse("#3D4654"),
    AccentColor = Color.Parse("#3498DB"),
    PrimaryBackground = Color.Parse("#2C313D"),
    SecondaryBackground = Color.Parse("#464F62"),
    PrimaryTextColor = Color.Parse("#FFFFFF"),
    SecondaryTextColor = Color.Parse("#CCCCCC"),
    BorderColor = Color.Parse("#5D6778"),
    ErrorColor = Color.Parse("#E74C3C"),
    WarningColor = Color.Parse("#F39C12"),
    SuccessColor = Color.Parse("#2ECC71")
};

var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
skinManager.RegisterSkin(customTheme.Name, customTheme);
skinManager.ApplySkin(customTheme.Name);
```

## Public services

- `ISkinManager`
- `IThemeValidator`
- `IThemeAutoFixer`
- `IThemeLoaderService`
- `IDialogService`
- `ThemeInheritanceManager`

## Notes

- The package ID is `AvaloniaSkinManager`.
- The library namespace remains `AvaloniaThemeManager`.
- Startup initialization restores the last saved theme when `UseThemeManager()` is used.
- Demo and compatibility constructors are still present, but new code should prefer explicit constructors and resolved services.

## Changelog

### 2.0.0

- moved the library to a DI-first model
- removed the old `SkinManager.Instance` singleton path
- fixed full-fidelity theme import/export
- fixed inheritable theme resolution for typography and URI overrides
- unified validation policy and split validation from autofix
- reduced demo app and demo view orchestration code-behind

## Support

- [Repository](https://github.com/JonMichaelSales/AvaloniaThemeManager)
- [Issues](https://github.com/JonMichaelSales/AvaloniaThemeManager/issues)
