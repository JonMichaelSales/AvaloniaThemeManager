using System;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaThemeManager.Services.Interfaces;
using AvaloniaThemeManager.Theme;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AvaloniaThemeManager.ViewModels;

internal sealed class ThemeManagerDemoViewModel : ViewModelBase
{
    private readonly ISkinManager _skinManager;
    private readonly IThemeManagerDemoService _demoService;
    private readonly ILogger _logger;
    private string _statusText = "Ready - All theme components loaded successfully";
    private string _validationStatusText = "Theme validation passed";

    public ThemeManagerDemoViewModel(
        ISkinManager skinManager,
        QuickThemeSwitcherViewModel quickThemeSwitcherViewModel,
        IThemeManagerDemoService demoService,
        ILogger logger)
    {
        _skinManager = skinManager ?? throw new ArgumentNullException(nameof(skinManager));
        QuickThemeSwitcherViewModel = quickThemeSwitcherViewModel ?? throw new ArgumentNullException(nameof(quickThemeSwitcherViewModel));
        _demoService = demoService ?? throw new ArgumentNullException(nameof(demoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LibraryVersionText = $"Version {FormatVersion(typeof(ThemeManagerDemoViewModel).Assembly.GetName().Version)}";
        AvaloniaVersionText = $"Avalonia {FormatVersion(typeof(Avalonia.Application).Assembly.GetName().Version)}";

        OpenThemeSettingsCommand = ReactiveCommand.CreateFromTask(OpenThemeSettingsAsync);
        ExportThemeCommand = ReactiveCommand.CreateFromTask(ExportThemeAsync);
        ImportThemeCommand = ReactiveCommand.CreateFromTask(ImportThemeAsync);
        ShowValidationDemoCommand = ReactiveCommand.CreateFromTask(ShowValidationDemoAsync);
        ShowErrorDemoCommand = ReactiveCommand.CreateFromTask(ShowErrorDemoAsync);
        ShowConfirmationDemoCommand = ReactiveCommand.CreateFromTask(ShowConfirmationDemoAsync);

        _skinManager.SkinChanged += OnSkinChanged;
    }

    public QuickThemeSwitcherViewModel QuickThemeSwitcherViewModel { get; }

    public string LibraryVersionText { get; }

    public string AvaloniaVersionText { get; }

    public string StatusText
    {
        get => _statusText;
        private set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    public string ValidationStatusText
    {
        get => _validationStatusText;
        private set => this.RaiseAndSetIfChanged(ref _validationStatusText, value);
    }

    public ReactiveCommand<Unit, Unit> OpenThemeSettingsCommand { get; }

    public ReactiveCommand<Unit, Unit> ExportThemeCommand { get; }

    public ReactiveCommand<Unit, Unit> ImportThemeCommand { get; }

    public ReactiveCommand<Unit, Unit> ShowValidationDemoCommand { get; }

    public ReactiveCommand<Unit, Unit> ShowErrorDemoCommand { get; }

    public ReactiveCommand<Unit, Unit> ShowConfirmationDemoCommand { get; }

    private async Task OpenThemeSettingsAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.OpenThemeSettingsAsync(), "Theme settings failed");
    }

    private async Task ExportThemeAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.ExportThemeAsync(), "Theme export failed");
    }

    private async Task ImportThemeAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.ImportThemeAsync(), "Theme import failed");
    }

    private async Task ShowValidationDemoAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.ShowValidationDemoAsync(), "Validation demo failed");
    }

    private async Task ShowErrorDemoAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.ShowErrorDemoAsync(), "Error demo failed");
    }

    private async Task ShowConfirmationDemoAsync()
    {
        StatusText = await ExecuteAsync(() => _demoService.ShowConfirmationDemoAsync(), "Confirmation demo failed");
    }

    private async Task<string> ExecuteAsync(Func<Task<string>> action, string fallbackStatus)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Demo command execution failed");
            return fallbackStatus;
        }
    }

    private void OnSkinChanged(object? sender, EventArgs e)
    {
        try
        {
            var currentThemeName = _skinManager.CurrentSkin?.Name ?? "Unknown";
            StatusText = $"Ready - Current theme: {currentThemeName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update demo status after theme change");
        }
    }

    private static string FormatVersion(Version? version)
    {
        if (version == null)
        {
            return "Unknown";
        }

        return version.Build >= 0
            ? $"{version.Major}.{version.Minor}.{version.Build}"
            : $"{version.Major}.{version.Minor}";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _skinManager.SkinChanged -= OnSkinChanged;
            QuickThemeSwitcherViewModel.Dispose();
        }

        base.Dispose(disposing);
    }
}
