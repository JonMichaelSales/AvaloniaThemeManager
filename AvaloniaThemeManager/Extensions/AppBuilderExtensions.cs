using Avalonia;
using AvaloniaThemeManager.Theme;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaThemeManager.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring and integrating the AvaloniaThemeManager 
    /// into an Avalonia application using the <see cref="AppBuilder"/>.
    /// </summary>
    /// <remarks>
    /// This class contains methods to enable the AvaloniaThemeManager with default or custom configurations.
    /// It simplifies the setup process by allowing developers to chain theme manager configuration
    /// into the application initialization pipeline.
    /// </remarks>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Static service provider holder for accessing services throughout the application
        /// </summary>
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// Action to be executed when application is ready
        /// </summary>
        private static Action? _initializationAction;

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get => _serviceProvider ?? throw new InvalidOperationException("Service provider not initialized. Ensure UseThemeManager() was called during application setup.");
            private set => _serviceProvider = value;
        }

        /// <summary>
        /// Adds AvaloniaThemeManager to the application with dependency injection support
        /// </summary>
        /// <param name="builder">The AppBuilder instance</param>
        /// <param name="configureServices">Optional service configuration action</param>
        /// <returns>The AppBuilder instance for method chaining</returns>
        public static AppBuilder UseThemeManager(this AppBuilder builder, Action<IServiceCollection>? configureServices = null)
        {
            return builder.AfterSetup(appBuilder =>
            {
                // Set up dependency injection
                var services = new ServiceCollection();

                // Add theme manager services
                services.AddThemeManagerServices();

                // Allow additional service configuration
                configureServices?.Invoke(services);

                // Build and store the service provider
                ServiceProvider = services.BuildServiceProvider();

                // Store initialization action to be called when application is ready
                _initializationAction = () =>
                {
                    try
                    {
                        var skinManager = ServiceProvider.GetRequiredService<ISkinManager>();
                        skinManager.LoadSavedTheme();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error initializing theme manager: {ex.Message}");
                    }
                };

                InitializeThemeManager();
            });
        }

        /// <summary>
        /// Adds AvaloniaThemeManager with an initialization callback using the service abstraction.
        /// </summary>
        /// <param name="builder">The AppBuilder instance</param>
        /// <param name="configure">Configuration action</param>
        /// <returns>The AppBuilder instance for method chaining</returns>
        public static AppBuilder UseThemeManager(this AppBuilder builder, Action<ISkinManager> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            return builder.AfterSetup(appBuilder =>
            {
                var services = new ServiceCollection();
                services.AddThemeManagerServices();
                ServiceProvider = services.BuildServiceProvider();

                _initializationAction = () =>
                {
                    try
                    {
                        var skinManager = ServiceProvider.GetRequiredService<ISkinManager>();
                        configure(skinManager);
                        skinManager.LoadSavedTheme();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring theme manager: {ex.Message}");
                    }
                };

                InitializeThemeManager();
            });
        }

        /// <summary>
        /// Internal method to be called by the Application when it's ready to initialize the theme manager
        /// This should be called from Application.OnFrameworkInitializationCompleted()
        /// </summary>
        internal static void InitializeThemeManager()
        {
            _initializationAction?.Invoke();
            _initializationAction = null; // Clear after execution
        }

        /// <summary>
        /// Gets a service from the application's service provider
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service provider is not available</exception>
        public static T GetRequiredService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Gets a service from the application's service provider
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The service instance, or null if not found</returns>
        public static T? GetService<T>() where T : class
        {
            try
            {
                return ServiceProvider.GetService<T>();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        internal static void ConfigureServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        internal static void ResetForTests()
        {
            _serviceProvider = null;
            _initializationAction = null;
        }

        internal static void SetInitializationActionForTests(Action initializationAction)
        {
            _initializationAction = initializationAction ?? throw new ArgumentNullException(nameof(initializationAction));
        }

        /// <summary>
        /// Extension method for Application to get services (for compatibility)
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <param name="app">The application instance</param>
        /// <returns>The service instance</returns>
        public static T GetRequiredService<T>(this Application app) where T : notnull
        {
            return GetRequiredService<T>();
        }

        /// <summary>
        /// Extension method for Application to get services (for compatibility)
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <param name="app">The application instance</param>
        /// <returns>The service instance, or null if not found</returns>
        public static T? GetService<T>(this Application app) where T : class
        {
            return GetService<T>();
        }
    }
}
