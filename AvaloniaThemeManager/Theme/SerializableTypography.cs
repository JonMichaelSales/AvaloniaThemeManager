namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Represents the advanced typography section of a serialized theme.
    /// </summary>
    public class SerializableTypography
    {
        /// <summary>
        /// Gets or sets the display-large font size.
        /// </summary>
        public double DisplayLarge { get; set; } = 57;
        /// <summary>
        /// Gets or sets the display-medium font size.
        /// </summary>
        public double DisplayMedium { get; set; } = 45;
        /// <summary>
        /// Gets or sets the display-small font size.
        /// </summary>
        public double DisplaySmall { get; set; } = 36;
        /// <summary>
        /// Gets or sets the headline-large font size.
        /// </summary>
        public double HeadlineLarge { get; set; } = 32;
        /// <summary>
        /// Gets or sets the headline-medium font size.
        /// </summary>
        public double HeadlineMedium { get; set; } = 28;
        /// <summary>
        /// Gets or sets the headline-small font size.
        /// </summary>
        public double HeadlineSmall { get; set; } = 24;
        /// <summary>
        /// Gets or sets the title-large font size.
        /// </summary>
        public double TitleLarge { get; set; } = 22;
        /// <summary>
        /// Gets or sets the title-medium font size.
        /// </summary>
        public double TitleMedium { get; set; } = 16;
        /// <summary>
        /// Gets or sets the title-small font size.
        /// </summary>
        public double TitleSmall { get; set; } = 14;
        /// <summary>
        /// Gets or sets the label-large font size.
        /// </summary>
        public double LabelLarge { get; set; } = 14;
        /// <summary>
        /// Gets or sets the label-medium font size.
        /// </summary>
        public double LabelMedium { get; set; } = 12;
        /// <summary>
        /// Gets or sets the label-small font size.
        /// </summary>
        public double LabelSmall { get; set; } = 11;
        /// <summary>
        /// Gets or sets the body-large font size.
        /// </summary>
        public double BodyLarge { get; set; } = 16;
        /// <summary>
        /// Gets or sets the body-medium font size.
        /// </summary>
        public double BodyMedium { get; set; } = 14;
        /// <summary>
        /// Gets or sets the body-small font size.
        /// </summary>
        public double BodySmall { get; set; } = 12;
        /// <summary>
        /// Gets or sets the preferred header font-family stack.
        /// </summary>
        public string HeaderFontFamily { get; set; } = "Segoe UI, San Francisco, Helvetica, Arial, sans-serif";
        /// <summary>
        /// Gets or sets the preferred body font-family stack.
        /// </summary>
        public string BodyFontFamily { get; set; } = "Segoe UI, San Francisco, Helvetica, Arial, sans-serif";
        /// <summary>
        /// Gets or sets the preferred monospace font-family stack.
        /// </summary>
        public string MonospaceFontFamily { get; set; } = "Consolas, Monaco, 'Courier New', monospace";
        /// <summary>
        /// Gets or sets the line-height multiplier.
        /// </summary>
        public double LineHeight { get; set; } = 1.5;
        /// <summary>
        /// Gets or sets the letter-spacing adjustment.
        /// </summary>
        public double LetterSpacing { get; set; } = 0;
        /// <summary>
        /// Gets or sets a value indicating whether ligatures are enabled.
        /// </summary>
        public bool EnableLigatures { get; set; } = true;
    }
}
