using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaThemeManager.Icons;
using AvaloniaThemeManager.Services;
using AvaloniaThemeManager.Utility;

namespace AvaloniaThemeManager.Views
{
    /// <summary>
    /// A dialog for displaying notification messages with different types (Information, Warning, Error).
    /// </summary>
    public partial class NotificationDialog : Window
    {
        /// <summary>
        /// Gets or sets the message to display in the dialog.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Gets or sets the type of notification dialog.
        /// </summary>
        public NotificationDialogType DialogType { get; set; } = NotificationDialogType.Information;

        /// <summary>
        /// Initializes a new instance of the NotificationDialog class.
        /// </summary>
        public NotificationDialog()
        {
            InitializeComponent();
            OkButton.Click += OkButton_Click;
        }

        /// <summary>
        /// Called when the window is opened to configure the dialog based on its type.
        /// </summary>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            ConfigureDialogType();
            UpdateContent();
        }

        private void ConfigureDialogType()
        {
            switch (DialogType)
            {
                case NotificationDialogType.Information:
                    Title = "Information";
                    TitleText.Text = "Information";
                    HeaderIcon.Data = ApplicationIcons.InformationGeometry;
                    HeaderBorder.Background = WindowTools.GetMainWindow()!.FindResource("AccentBlueBrush") as IBrush;
                    break;

                case NotificationDialogType.Warning:
                    Title = "Warning";
                    TitleText.Text = "Warning";
                    HeaderIcon.Data = ApplicationIcons.WarningGeometry;
                    HeaderBorder.Background = WindowTools.GetMainWindow()!.FindResource("WarningBrush") as IBrush;
                    break;

                case NotificationDialogType.Error:
                    Title = "Error";
                    TitleText.Text = "Error";
                    HeaderIcon.Data = ApplicationIcons.ErrorGeometry;
                    HeaderBorder.Background = WindowTools.GetMainWindow()!.FindResource("ErrorBrush") as IBrush;
                    break;
            }
        }

        private void UpdateContent()
        {
            MessageText.Text = Message;
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}