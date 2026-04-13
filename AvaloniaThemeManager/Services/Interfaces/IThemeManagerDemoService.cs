using System.Threading.Tasks;

namespace AvaloniaThemeManager.Services.Interfaces;

internal interface IThemeManagerDemoService
{
    Task<string> OpenThemeSettingsAsync();
    Task<string> ExportThemeAsync();
    Task<string> ImportThemeAsync();
    Task<string> ShowValidationDemoAsync();
    Task<string> ShowErrorDemoAsync();
    Task<string> ShowConfirmationDemoAsync();
}
