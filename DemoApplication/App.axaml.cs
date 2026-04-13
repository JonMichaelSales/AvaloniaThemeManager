using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaThemeManager.Extensions;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.Views;
using DemoApplication.Views;
using Microsoft.Extensions.Logging.Abstractions;

namespace DemoApplication;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var skinManager = AppBuilderExtensions.GetRequiredService<ISkinManager>();
        var dialogService = AppBuilderExtensions.GetRequiredService<IDialogService>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var demoView = new ThemeManagerDemoView(
                skinManager,
                NullLogger.Instance,
                dialogService);

            desktop.MainWindow = new MainWindow(demoView);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var demoView = new ThemeManagerDemoView(
                skinManager,
                NullLogger.Instance,
                dialogService);

            singleViewPlatform.MainView = new MainView(demoView);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
