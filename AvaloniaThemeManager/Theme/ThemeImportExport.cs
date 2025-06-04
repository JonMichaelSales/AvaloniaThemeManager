// Theme/ThemeImportExport.cs
using Avalonia;
using Avalonia.Media;
using AvaloniaThemeManager.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Handles theme import and export operations.
    /// </summary>
    public static class ThemeImportExport
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Exports a theme to a JSON file.
        /// </summary>
        public static async Task<bool> ExportThemeAsync(Skin theme, string filePath, string? description = null, string? author = null)
        {
            try
            {
                var serializableTheme = ConvertToSerializable(theme, description, author);
                var json = JsonSerializer.Serialize(serializableTheme, _jsonOptions);

                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting theme: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports an advanced theme with typography to a JSON file.
        /// </summary>
        public static async Task<bool> ExportAdvancedThemeAsync(Skin theme, string filePath, string? description = null, string? author = null)
        {
            try
            {
                var serializableTheme = ConvertToSerializable(theme, description, author);

                // Add advanced typography
                serializableTheme.AdvancedTypography = new SerializableTypography
                {
                    DisplayLarge = theme.Typography.DisplayLarge,
                    DisplayMedium = theme.Typography.DisplayMedium,
                    DisplaySmall = theme.Typography.DisplaySmall,
                    HeadlineLarge = theme.Typography.HeadlineLarge,
                    HeadlineMedium = theme.Typography.HeadlineMedium,
                    HeadlineSmall = theme.Typography.HeadlineSmall,
                    TitleLarge = theme.Typography.TitleLarge,
                    TitleMedium = theme.Typography.TitleMedium,
                    TitleSmall = theme.Typography.TitleSmall,
                    LabelLarge = theme.Typography.LabelLarge,
                    LabelMedium = theme.Typography.LabelMedium,
                    LabelSmall = theme.Typography.LabelSmall,
                    BodyLarge = theme.Typography.BodyLarge,
                    BodyMedium = theme.Typography.BodyMedium,
                    BodySmall = theme.Typography.BodySmall,
                    HeaderFontFamily = theme.HeaderFontFamily.ToString(),
                    BodyFontFamily = theme.BodyFontFamily.ToString(),
                    MonospaceFontFamily = theme.MonospaceFontFamily.ToString(),
                    LineHeight = theme.LineHeight,
                    LetterSpacing = theme.LetterSpacing,
                    EnableLigatures = theme.EnableLigatures
                };

                var json = JsonSerializer.Serialize(serializableTheme, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting advanced theme: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports an inheritable theme to a JSON file.
        /// </summary>
        public static async Task<bool> ExportInheritableThemeAsync(InheritableSkin theme, string filePath, string? description = null, string? author = null)
        {
            try
            {
                var serializableTheme = ConvertToSerializable(theme, description, author);

                // Add inheritance information
                serializableTheme.BaseTheme = theme.BaseThemeName;
                serializableTheme.PropertyOverrides = theme.PropertyOverrides;

                var json = JsonSerializer.Serialize(serializableTheme, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting inheritable theme: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Imports a theme from a JSON file.
        /// </summary>
        public static async Task<ThemeImportResult> ImportThemeAsync(string filePath)
        {
            var result = new ThemeImportResult();

            try
            {
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = $"Theme file does not exist: {filePath}";
                    return result;
                }

                var json = await File.ReadAllTextAsync(filePath);
                var serializableTheme = JsonSerializer.Deserialize<SerializableTheme>(json, _jsonOptions);

                if (serializableTheme == null)
                {
                    result.ErrorMessage = "Invalid theme file format";
                    return result;
                }

                // Validate before converting
                var validation = await ValidateThemeFileAsync(filePath);
                if (!validation.IsValid)
                {
                    result.ErrorMessage = $"Theme validation failed: {string.Join(", ", validation.Errors)}";
                    result.Warnings.AddRange(validation.Warnings);
                    return result;
                }

                result.Theme = ConvertFromSerializable(serializableTheme);
                result.Success = true;
            }
            catch (JsonException ex)
            {
                result.ErrorMessage = $"JSON parsing error: {ex.Message}";
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Unexpected error importing theme: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Imports an advanced theme from a JSON file.
        /// </summary>
        public static async Task<Skin?> ImportAdvancedThemeAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var serializableTheme = JsonSerializer.Deserialize<SerializableTheme>(json, _jsonOptions);

                if (serializableTheme == null) return null;

                var baseSkin = ConvertFromSerializable(serializableTheme);
                

                // Apply advanced typography if present
                if (serializableTheme.AdvancedTypography != null)
                {
                    var typography = serializableTheme.AdvancedTypography;

                    baseSkin.Typography = new TypographyScale
                    {
                        DisplayLarge = typography.DisplayLarge,
                        DisplayMedium = typography.DisplayMedium,
                        DisplaySmall = typography.DisplaySmall,
                        HeadlineLarge = typography.HeadlineLarge,
                        HeadlineMedium = typography.HeadlineMedium,
                        HeadlineSmall = typography.HeadlineSmall,
                        TitleLarge = typography.TitleLarge,
                        TitleMedium = typography.TitleMedium,
                        TitleSmall = typography.TitleSmall,
                        LabelLarge = typography.LabelLarge,
                        LabelMedium = typography.LabelMedium,
                        LabelSmall = typography.LabelSmall,
                        BodyLarge = typography.BodyLarge,
                        BodyMedium = typography.BodyMedium,
                        BodySmall = typography.BodySmall
                    };

                    baseSkin.HeaderFontFamily = new FontFamily(typography.HeaderFontFamily);
                    baseSkin.BodyFontFamily = new FontFamily(typography.BodyFontFamily);
                    baseSkin.MonospaceFontFamily = new FontFamily(typography.MonospaceFontFamily);
                    baseSkin.LineHeight = typography.LineHeight;
                    baseSkin.LetterSpacing = typography.LetterSpacing;
                    baseSkin.EnableLigatures = typography.EnableLigatures;
                }

                return baseSkin;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing advanced theme: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Imports an inheritable theme from a JSON file.
        /// </summary>
        public static async Task<InheritableSkin?> ImportInheritableThemeAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var serializableTheme = JsonSerializer.Deserialize<SerializableTheme>(json, _jsonOptions);

                if (serializableTheme == null) return null;

                var baseSkin = ConvertFromSerializable(serializableTheme);
                var inheritableSkin = new InheritableSkin();

                // Copy all properties from base skin
                CopyPropertiesToInheritable(inheritableSkin, baseSkin);

                // Set inheritance properties
                inheritableSkin.BaseThemeName = serializableTheme.BaseTheme;
                inheritableSkin.PropertyOverrides = serializableTheme.PropertyOverrides ?? new Dictionary<string, object>();

                return inheritableSkin;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing inheritable theme: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates a theme file before importing.
        /// </summary>
        public static async Task<ThemeValidationResult> ValidateThemeFileAsync(string filePath)
        {
            var result = new ThemeValidationResult();

            try
            {
                if (!File.Exists(filePath))
                {
                    result.AddError("Theme file does not exist");
                    return result;
                }

                var json = await File.ReadAllTextAsync(filePath);
                var serializableTheme = JsonSerializer.Deserialize<SerializableTheme>(json, _jsonOptions);

                if (serializableTheme == null)
                {
                    result.AddError("Invalid JSON format");
                    return result;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(serializableTheme.Name))
                {
                    result.AddError("Theme name is required");
                }

                // Try to convert to validate color formats
                try
                {
                    var skin = ConvertFromSerializable(serializableTheme);
                    var validator = new ThemeValidator();
                    var validationResult = validator.ValidateTheme(skin);

                    result.Errors.AddRange(validationResult.Errors);
                    result.Warnings.AddRange(validationResult.Warnings);
                    result.IsValid = validationResult.IsValid && result.Errors.Count == 0;
                }
                catch (Exception ex)
                {
                    result.AddError($"Invalid theme data: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Error reading theme file: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Exports multiple themes to a theme pack file.
        /// </summary>
        public static async Task<bool> ExportThemePackAsync(Dictionary<string, Skin> themes, string filePath, string packName, string? description = null)
        {
            try
            {
                var themePack = new
                {
                    Name = packName,
                    Description = description,
                    Version = "1.0",
                    CreatedDate = DateTime.Now,
                    Themes = themes.Select(kvp => ConvertToSerializable(kvp.Value, null, null)).ToArray()
                };

                var json = JsonSerializer.Serialize(themePack, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting theme pack: {ex.Message}");
                return false;
            }
        }

        private static SerializableTheme ConvertToSerializable(Skin theme, string? description, string? author)
        {
            return new SerializableTheme
            {
                Name = theme.Name ?? "Unnamed Theme",
                Description = description ?? "",
                Author = author ?? "",
                PrimaryColor = theme.PrimaryColor.ToString(),
                SecondaryColor = theme.SecondaryColor.ToString(),
                AccentColor = theme.AccentColor.ToString(),
                PrimaryBackground = theme.PrimaryBackground.ToString(),
                SecondaryBackground = theme.SecondaryBackground.ToString(),
                PrimaryTextColor = theme.PrimaryTextColor.ToString(),
                SecondaryTextColor = theme.SecondaryTextColor.ToString(),
                BorderColor = theme.BorderColor.ToString(),
                ErrorColor = theme.ErrorColor.ToString(),
                WarningColor = theme.WarningColor.ToString(),
                SuccessColor = theme.SuccessColor.ToString(),
                FontFamily = theme.FontFamily.ToString(),
                FontSizeSmall = theme.FontSizeSmall,
                FontSizeMedium = theme.FontSizeMedium,
                FontSizeLarge = theme.FontSizeLarge,
                FontWeight = theme.FontWeight.ToString(),
                BorderRadius = theme.BorderRadius,
                BorderThickness = new SerializableThickness
                {
                    Left = theme.BorderThickness.Left,
                    Top = theme.BorderThickness.Top,
                    Right = theme.BorderThickness.Right,
                    Bottom = theme.BorderThickness.Bottom
                }
            };
        }

        private static Skin ConvertFromSerializable(SerializableTheme serializableTheme)
        {
            var fontWeight = Enum.TryParse<FontWeight>(serializableTheme.FontWeight, out var weight)
                ? weight
                : FontWeight.Normal;

            return new Skin
            {
                Name = serializableTheme.Name,
                PrimaryColor = Color.Parse(serializableTheme.PrimaryColor),
                SecondaryColor = Color.Parse(serializableTheme.SecondaryColor),
                AccentColor = Color.Parse(serializableTheme.AccentColor),
                PrimaryBackground = Color.Parse(serializableTheme.PrimaryBackground),
                SecondaryBackground = Color.Parse(serializableTheme.SecondaryBackground),
                PrimaryTextColor = Color.Parse(serializableTheme.PrimaryTextColor),
                SecondaryTextColor = Color.Parse(serializableTheme.SecondaryTextColor),
                BorderColor = Color.Parse(serializableTheme.BorderColor),
                ErrorColor = Color.Parse(serializableTheme.ErrorColor),
                WarningColor = Color.Parse(serializableTheme.WarningColor),
                SuccessColor = Color.Parse(serializableTheme.SuccessColor),
                FontFamily = new FontFamily(serializableTheme.FontFamily),
                FontSizeSmall = serializableTheme.FontSizeSmall,
                FontSizeMedium = serializableTheme.FontSizeMedium,
                FontSizeLarge = serializableTheme.FontSizeLarge,
                FontWeight = fontWeight,
                BorderRadius = serializableTheme.BorderRadius,
                BorderThickness = new Thickness(
                    serializableTheme.BorderThickness.Left,
                    serializableTheme.BorderThickness.Top,
                    serializableTheme.BorderThickness.Right,
                    serializableTheme.BorderThickness.Bottom
                )
            };
        }

        private static void CopyPropertiesToInheritable(InheritableSkin target, Skin source)
        {
            target.PrimaryColor = source.PrimaryColor;
            target.SecondaryColor = source.SecondaryColor;
            target.AccentColor = source.AccentColor;
            target.PrimaryBackground = source.PrimaryBackground;
            target.SecondaryBackground = source.SecondaryBackground;
            target.PrimaryTextColor = source.PrimaryTextColor;
            target.SecondaryTextColor = source.SecondaryTextColor;
            target.FontFamily = source.FontFamily;
            target.FontSizeSmall = source.FontSizeSmall;
            target.FontSizeMedium = source.FontSizeMedium;
            target.FontSizeLarge = source.FontSizeLarge;
            target.FontWeight = source.FontWeight;
            target.BorderColor = source.BorderColor;
            target.BorderThickness = source.BorderThickness;
            target.BorderRadius = source.BorderRadius;
            target.ErrorColor = source.ErrorColor;
            target.WarningColor = source.WarningColor;
            target.SuccessColor = source.SuccessColor;
            target.Name = source.Name;
        }
    }
}