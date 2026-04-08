using Avalonia.Styling;
using System.Collections.Generic;

namespace AvaloniaThemeManager.Services.Interfaces
{
    /// <summary>
    /// Represents a mutable collection of Avalonia styles.
    /// </summary>
    public interface IStylesCollection : IEnumerable<IStyle>
    {
        /// <summary>
        /// Adds a style to the collection.
        /// </summary>
        /// <param name="style">The style to add.</param>
        void Add(IStyle style);

        /// <summary>
        /// Removes a style from the collection.
        /// </summary>
        /// <param name="style">The style to remove.</param>
        /// <returns><c>true</c> if the style was removed; otherwise, <c>false</c>.</returns>
        bool Remove(IStyle style);

        /// <summary>
        /// Removes all styles from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the number of styles in the collection.
        /// </summary>
        int Count { get; }
    }
}
