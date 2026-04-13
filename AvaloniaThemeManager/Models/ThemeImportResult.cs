using AvaloniaThemeManager.Theme;

namespace AvaloniaThemeManager.Models
{
    /// <summary>
    /// Represents the result of importing a theme file.
    /// </summary>
    public class ThemeImportResult
    {
        /// <summary>
        /// Gets or sets the imported theme when the import succeeds.
        /// </summary>
        public Skin? Theme { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the import completed successfully.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets the error message when the import fails.
        /// </summary>
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// Gets or sets any non-fatal warnings produced during import.
        /// </summary>
        public List<string> Warnings { get; set; } = new();
    }
}
