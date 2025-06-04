using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ThemeImportResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Skin? Theme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Warnings { get; set; } = new();
    }
}
