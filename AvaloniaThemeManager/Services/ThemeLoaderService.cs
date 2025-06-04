using Avalonia.Platform;
using AvaloniaThemeManager.Theme;
using System.Text.Json;

namespace AvaloniaThemeManager.Services
{
    /// <summary>
    /// Defines a service for loading themes in an Avalonia application.
    /// </summary>
    /// <remarks>
    /// This interface provides methods to load theme skins from a specified directory.
    /// Implementations of this interface are responsible for parsing and managing theme-related resources.
    /// </remarks>
    public interface IThemeLoaderService
    {
        /// <summary>
        /// Loads a collection of theme skins from the specified root directory.
        /// </summary>
        /// <param name="themesRoot">
        /// The root directory containing theme skin definitions.
        /// </param>
        /// <returns>
        /// A list of <see cref="Skin"/> objects representing the loaded theme skins.
        /// </returns>
        List<Skin> LoadSkins(string themesRoot);
    }

    /// <summary>
    /// Provides functionality to load and manage themes for the Avalonia application.
    /// </summary>
    /// <remarks>
    /// This service is responsible for loading theme configurations and resources from a specified directory structure.
    /// It processes theme definitions, control themes, and styles, making them available for use within the application.
    /// </remarks>
    public class ThemeLoaderService : IThemeLoaderService
    {

        // List of known embedded themes (keep in sync with package)
        private readonly string[] _embeddedThemes = new[]
        {
            "Dark", "Light", "Ocean Blue", "Cyberpunk",
            "RetroTerminal", "Purple Haze", "Forest Green", "High Contrast", "ModernIce"
        };

        /// <summary>
        /// Loads a collection of <see cref="Skin"/> objects from the specified root directory.
        /// </summary>
        /// <returns>
        /// A list of <see cref="Skin"/> objects representing the loaded themes.
        /// </returns>
        /// <remarks>
        /// This method scans the specified directory for subdirectories containing theme definitions.
        /// Each theme is expected to have a "theme.json" file and optionally "ControlThemes" and "Styles" directories
        /// containing .axaml files. The method parses these resources and constructs <see cref="Skin"/> objects
        /// with appropriate URIs for control themes and styles.
        /// </remarks>
        public List<Skin> LoadSkins(string _ = "")
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var skins = new List<Skin>();

            foreach (var themeName in _embeddedThemes)
            {
                var basePath = $"avares://AvaloniaThemeManager/Themes/{themeName}";
                var themeJsonPath = $"{basePath}/theme.json";

                try
                {
                    using var stream = AssetLoader.Open(new Uri(themeJsonPath));
                    using var reader = new StreamReader(stream);
                    var json = reader.ReadToEnd();

                    var serializableTheme = JsonSerializer.Deserialize<SerializableTheme>(json, jsonOptions);
                    if (serializableTheme is null) continue;

                    var skin = serializableTheme.ToSkin();

                    // Load ControlTheme resources
                    var controlThemes = new[]
                    {
                        "Button", "TextBlock", "TextBox", "CheckBox",
                        "ComboBox", "Expander", "Slider", "TabControl"
                    };

                    foreach (var key in controlThemes)
                    {
                        var controlPath = $"{basePath}/ControlThemes/{key}.axaml";
                        skin.ControlThemeUris[key] = controlPath;
                    }

                    skins.Add(skin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load theme '{themeName}': {ex.Message}");
                }
            }

            return skins;
        }
    }
}
