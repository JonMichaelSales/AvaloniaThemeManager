using System.Reflection;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Theme;
using AvaloniaThemeManager.ViewModels;
using AvaloniaThemeManager.Views;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AvaloniaThemeManager.Tests.Views;

public sealed class DialogViewTests
{
    [AvaloniaFact]
    public void ConfirmationDialog_OnOpened_PopulatesContent_AndConfirmSetsResult()
    {
        using var app = new TestApplication();
        var dialog = new ConfirmationDialog
        {
            Message = "Proceed?",
            ConfirmText = "Do it",
            CancelText = "Stop"
        };

        InvokeOnOpened(dialog);

        Assert.Equal("Proceed?", dialog.FindControl<TextBlock>("MessageText")!.Text);
        Assert.Equal("Do it", dialog.FindControl<Button>("ConfirmButton")!.Content);
        Assert.Equal("Stop", dialog.FindControl<Button>("CancelButton")!.Content);

        dialog.FindControl<Button>("ConfirmButton")!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.True(dialog.DialogResult);
    }

    [AvaloniaFact]
    public void ConfirmationDialog_CancelSetsResultFalse()
    {
        using var app = new TestApplication();
        var dialog = new ConfirmationDialog();

        dialog.FindControl<Button>("CancelButton")!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        Assert.False(dialog.DialogResult);
    }

    [AvaloniaFact]
    public void NotificationDialog_OnOpened_PopulatesMessageAndAdditionalContent_WithoutMainWindow()
    {
        using var app = new TestApplication();
        var additionalContent = new TextBlock { Text = "More details" };
        var dialog = new NotificationDialog
        {
            Message = "Heads up",
            DialogType = NotificationDialogType.Warning,
            AdditionalContent = additionalContent
        };

        InvokeOnOpened(dialog);

        Assert.Equal("Warning", dialog.Title);
        Assert.Equal("Warning", dialog.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("Heads up", dialog.FindControl<TextBlock>("MessageText")!.Text);
        Assert.Same(additionalContent, dialog.FindControl<ContentPresenter>("AdditionalContentPresenter")!.Content);
    }

    [AvaloniaFact]
    public void ErrorDialog_OnOpened_ShowsExceptionDetails_WhenExceptionExists()
    {
        using var app = new TestApplication();
        var dialog = new ErrorDialog
        {
            Title = "Load Error",
            Message = "Something failed",
            Exception = new InvalidOperationException("Boom", new Exception("Inner"))
        };

        InvokeOnOpened(dialog);

        Assert.Equal("Load Error", dialog.FindControl<TextBlock>("TitleText")!.Text);
        Assert.Equal("Something failed", dialog.FindControl<TextBlock>("MessageText")!.Text);
        Assert.True(dialog.FindControl<Expander>("ExceptionExpander")!.IsVisible);
        Assert.True(dialog.FindControl<Button>("CopyButton")!.IsVisible);
        Assert.Contains("InvalidOperationException", dialog.FindControl<TextBlock>("ExceptionText")!.Text);
        Assert.Contains("Inner Exception 1", dialog.FindControl<TextBlock>("ExceptionText")!.Text);
    }

    [AvaloniaFact]
    public void ValidationErrorDialog_OnOpened_PopulatesErrorsAndWarnings()
    {
        using var app = new TestApplication();
        var dialog = new ValidationErrorDialog();
        dialog.SetValidationResults(["error-1", "error-2"], ["warning-1"]);

        InvokeOnOpened(dialog);

        Assert.Equal("2 Errors", dialog.FindControl<TextBlock>("ErrorCountText")!.Text);
        Assert.Equal("1 Warning", dialog.FindControl<TextBlock>("WarningCountText")!.Text);
        Assert.True(dialog.FindControl<StackPanel>("ErrorsSection")!.IsVisible);
        Assert.True(dialog.FindControl<StackPanel>("WarningsSection")!.IsVisible);
        Assert.Equal("Validation Issues", dialog.FindControl<TextBlock>("TitleText")!.Text);
    }

    [AvaloniaFact]
    public void ValidationErrorDialog_OnOpened_HandlesWarningsOnlyWithoutMainWindow()
    {
        using var app = new TestApplication();
        var dialog = new ValidationErrorDialog();
        dialog.SetValidationResults([], ["warning-1"]);

        InvokeOnOpened(dialog);

        Assert.False(dialog.FindControl<StackPanel>("ErrorsSection")!.IsVisible);
        Assert.True(dialog.FindControl<StackPanel>("WarningsSection")!.IsVisible);
        Assert.Equal("Validation Warnings", dialog.Title);
        Assert.Equal("Validation Warnings", dialog.FindControl<TextBlock>("TitleText")!.Text);
    }

    [AvaloniaFact]
    public void ThemeSettingsDialog_ResetCommand_ResetsViewModelToDarkTheme()
    {
        using var app = new TestApplication();
        var skinManagerMock = new Mock<ISkinManager>();
        var dark = new Skin { Name = "Dark", AccentColor = Avalonia.Media.Colors.Black };
        var light = new Skin { Name = "Light", AccentColor = Avalonia.Media.Colors.White };

        skinManagerMock.Setup(manager => manager.GetAvailableSkinNames()).Returns(["Dark", "Light"]);
        skinManagerMock.Setup(manager => manager.GetSkin("Dark")).Returns(dark);
        skinManagerMock.Setup(manager => manager.GetSkin("Light")).Returns(light);
        skinManagerMock.SetupGet(manager => manager.CurrentSkin).Returns(light);

        var viewModel = new ThemeSettingsViewModel(skinManagerMock.Object, NullLogger.Instance);
        var dialog = new ThemeSettingsDialog(viewModel);

        Assert.Equal("Light", viewModel.SelectedTheme?.Name);

        viewModel.ResetThemeCommand.Execute().FirstAsync().Wait();

        Assert.Equal("Dark", viewModel.SelectedTheme?.Name);
    }

    private static void InvokeOnOpened(Window dialog)
    {
        dialog.GetType()
            .GetMethod("OnOpened", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(dialog, [EventArgs.Empty]);
    }
}
