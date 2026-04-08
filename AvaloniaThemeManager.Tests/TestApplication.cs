using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia;

namespace AvaloniaThemeManager.Tests
{
    /// <summary>  
    /// Test application for integration tests  
    /// </summary>  
    public class TestApplication : Application, IDisposable
    {
        private bool _disposed = false;

        public TestApplication()
        {
            Resources = new ResourceDictionary();
            // Use collection initialization to simplify adding styles  
            Styles.Clear();
            Styles.Add(new Styles());
            // Set as current for testing  
            SetCurrentApplication(this);
        }

        public override void Initialize()
        {
            // Minimal initialization for testing  
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                SetCurrentApplication(null);
                _disposed = true;
            }
        }

        private static void SetCurrentApplication(Application? application)
        {
            typeof(Application).GetProperty("Current", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.SetValue(null, application);
        }
    }
}