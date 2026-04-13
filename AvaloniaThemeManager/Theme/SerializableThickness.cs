namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Represents a serializable thickness value for theme JSON payloads.
    /// </summary>
    public class SerializableThickness
    {
        /// <summary>
        /// Gets or sets the left thickness component.
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// Gets or sets the top thickness component.
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// Gets or sets the right thickness component.
        /// </summary>
        public double Right { get; set; }
        /// <summary>
        /// Gets or sets the bottom thickness component.
        /// </summary>
        public double Bottom { get; set; }

    }
}
