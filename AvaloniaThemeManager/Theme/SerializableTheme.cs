using Avalonia.Media;
using Avalonia;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// 
    /// </summary>
    public class SerializableTheme
    {
        /// <summary>
        /// Gets or sets the name of the theme.
        /// </summary>
        /// <remarks>
        /// This property represents the unique identifier or display name of the theme.
        /// It is a required field and must not be null, empty, or whitespace.
        /// </remarks>
        public string Name { get; set; } = "";
        /// <summary>
        /// Gets or sets a description of the theme, providing additional context or details about its purpose or design.
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Gets or sets the version of the theme.
        /// </summary>
        /// <remarks>
        /// This property indicates the version of the theme, which can be useful for compatibility checks
        /// or identifying updates to the theme.
        /// </remarks>
        public string Version { get; set; } = "1.0";
        /// <summary>
        /// Gets or sets the author of the theme.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the theme's author.
        /// </value>
        public string Author { get; set; } = "";
        /// <summary>
        /// Gets or sets the date and time when the theme was created.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> representing the creation date and time of the theme.
        /// </value>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Colors as hex strings for JSON serialization
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryColor { get; set; } = "#343B48";
        /// <summary>
        /// 
        /// </summary>
        public string SecondaryColor { get; set; } = "#3D4654";
        /// <summary>
        /// 
        /// </summary>
        public string AccentColor { get; set; } = "#3498DB";
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryBackground { get; set; } = "#2C313D";
        /// <summary>
        /// 
        /// </summary>
        public string SecondaryBackground { get; set; } = "#464F62";
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryTextColor { get; set; } = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public string SecondaryTextColor { get; set; } = "#CCCCCC";
        /// <summary>
        /// 
        /// </summary>
        public string BorderColor { get; set; } = "#5D6778";
        /// <summary>
        /// 
        /// </summary>
        public string ErrorColor { get; set; } = "#E74C3C";
        /// <summary>
        /// 
        /// </summary>
        public string WarningColor { get; set; } = "#F39C12";
        /// <summary>
        /// 
        /// </summary>
        public string SuccessColor { get; set; } = "#2ECC71";


        // Typography
        /// <summary>
        /// 
        /// </summary>
        public string FontFamily { get; set; } = "Segoe UI, San Francisco, Helvetica, Arial, sans-serif";
        /// <summary>
        /// 
        /// </summary>
        public double FontSizeSmall { get; set; } = 10;
        /// <summary>
        /// 
        /// </summary>
        public double FontSizeMedium { get; set; } = 12;
        /// <summary>
        /// 
        /// </summary>
        public double FontSizeLarge { get; set; } = 16;
        /// <summary>
        /// 
        /// </summary>
        public string FontWeight { get; set; } = "Normal";

        // Layout
        /// <summary>
        /// 
        /// </summary>
        public double BorderRadius { get; set; } = 4;
        /// <summary>
        /// 
        /// </summary>
        public SerializableThickness BorderThickness { get; set; } = new() { Left = 1, Top = 1, Right = 1, Bottom = 1 };

        // Advanced typography (optional)
        /// <summary>
        /// 
        /// </summary>
        public SerializableTypography? AdvancedTypography { get; set; }

        // Inheritance (optional)
        /// <summary>
        /// 
        /// </summary>
        public string? BaseTheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object>? PropertyOverrides { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ThemeConverterExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static Skin ToSkin(this SerializableTheme theme)
        {
            return new Skin
            {
                Name = theme.Name,
                PrimaryColor = Color.Parse(theme.PrimaryColor),
                SecondaryColor = Color.Parse(theme.SecondaryColor),
                AccentColor = Color.Parse(theme.AccentColor),
                PrimaryBackground = Color.Parse(theme.PrimaryBackground),
                SecondaryBackground = Color.Parse(theme.SecondaryBackground),
                PrimaryTextColor = Color.Parse(theme.PrimaryTextColor),
                SecondaryTextColor = Color.Parse(theme.SecondaryTextColor),
                BorderColor = Color.Parse(theme.BorderColor),
                ErrorColor = Color.Parse(theme.ErrorColor),
                WarningColor = Color.Parse(theme.WarningColor),
                SuccessColor = Color.Parse(theme.SuccessColor),
                FontFamily = new FontFamily(theme.FontFamily),
                FontSizeSmall = theme.FontSizeSmall,
                FontSizeMedium = theme.FontSizeMedium,
                FontSizeLarge = theme.FontSizeLarge,
                FontWeight = Enum.TryParse<FontWeight>(theme.FontWeight, true, out var fw) ? fw : FontWeight.Normal,
                BorderRadius = theme.BorderRadius,
                BorderThickness = new Thickness(
                    theme.BorderThickness.Left,
                    theme.BorderThickness.Top,
                    theme.BorderThickness.Right,
                    theme.BorderThickness.Bottom
                ),
                // AdvancedTypography and PropertyOverrides could be used here too if needed
            };
        }
    }

}
