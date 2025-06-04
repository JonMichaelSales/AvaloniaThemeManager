using Avalonia.Controls;
using AvaloniaThemeManager.ViewModels;

namespace AvaloniaThemeManager.Controls;

/// <summary>
/// Represents a user control that provides a quick theme switching functionality
/// for Avalonia applications. This control is designed to integrate seamlessly
/// with the Avalonia UI framework and is backed by the <see cref="QuickThemeSwitcherViewModel"/>.
/// </summary>
public partial class QuickThemeSwitcher : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuickThemeSwitcher"/> class.
    /// This constructor sets up the control by initializing its components and
    /// assigning a new instance of <see cref="QuickThemeSwitcherViewModel"/> as its data context.
    /// </summary>
    public QuickThemeSwitcher()
    {
        InitializeComponent();
        DataContext = new QuickThemeSwitcherViewModel();
    }
}