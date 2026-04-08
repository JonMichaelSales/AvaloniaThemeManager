using System.Collections;
using Avalonia.Styling;
using AvaloniaThemeManager.Services.Interfaces;

namespace AvaloniaThemeManager.Services
{
    /// <summary>
    /// Adapts an Avalonia <see cref="Styles"/> collection to <see cref="IStylesCollection"/>.
    /// </summary>
    public class AvaloniaStylesWrapper : IStylesCollection
    {
        private readonly Styles _styles;

        /// <summary>
        /// Initializes a new wrapper for the provided styles collection.
        /// </summary>
        /// <param name="styles">The underlying Avalonia styles collection.</param>
        public AvaloniaStylesWrapper(Styles styles)
        {
            _styles = styles ?? throw new ArgumentNullException(nameof(styles));
        }

        /// <inheritdoc />
        public void Add(IStyle style)
        {
            _styles.Add(style);
        }

        /// <inheritdoc />
        public bool Remove(IStyle style)
        {
            return _styles.Remove(style);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _styles.Clear();
        }

        /// <inheritdoc />
        public int Count => _styles.Count;

        /// <inheritdoc />
        public IEnumerator<IStyle> GetEnumerator()
        {
            return _styles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
