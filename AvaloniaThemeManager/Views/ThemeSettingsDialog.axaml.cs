using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaThemeManager.ViewModels;

namespace AvaloniaThemeManager.Views;

/// <summary>
/// Represents a dialog window for managing theme settings in the application.
/// </summary>
/// <remarks>
/// This class provides a user interface for selecting, applying, and resetting themes.
/// It is backed by the <see cref="AvaloniaThemeManager.ViewModels.ThemeSettingsViewModel"/> 
/// to handle the logic and data binding for theme management.
/// </remarks>
public partial class ThemeSettingsDialog : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSettingsDialog"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor sets up the dialog by initializing its components and 
    /// assigning a new instance of <see cref="ThemeSettingsViewModel"/> as its data context.
    /// </remarks>
    public ThemeSettingsDialog()
    {
        InitializeComponent();
        DataContext = new ThemeSettingsViewModel();
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ThemeSettingsViewModel viewModel)
        {
            viewModel.ResetToDefault();
        }
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();

    }
}