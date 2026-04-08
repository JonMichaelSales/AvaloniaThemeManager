// Theme/ThemeInheritance.cs
using Avalonia;
using Avalonia.Media;
using System.Text.Json;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Represents a theme that can inherit from a base theme and override specific properties.
    /// </summary>
    public class InheritableSkin : Skin
    {
        private readonly HashSet<string> _setProperties = new();
        private readonly Dictionary<string, string> _defaultControlThemeUris;
        private readonly Dictionary<string, string> _defaultStyleUris;
        private readonly TypographyScale _defaultTypography;

        /// <summary>
        /// Gets or sets the name of the base theme this theme inherits from.
        /// </summary>
        public string? BaseThemeName { get; set; }

        /// <summary>
        /// Gets or sets the collection of property overrides for this theme.
        /// </summary>
        public Dictionary<string, object> PropertyOverrides { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the InheritableSkin class.
        /// </summary>
        public InheritableSkin()
        {
            PropertyOverrides = new Dictionary<string, object>();
            _defaultControlThemeUris = CloneDictionary(ControlThemeUris);
            _defaultStyleUris = CloneDictionary(StyleUris);
            _defaultTypography = CloneTypography(Typography);
        }

        // Override property setters to track which properties have been explicitly set
        /// <inheritdoc />
        public new Color PrimaryColor
        {
            get => base.PrimaryColor;
            set
            {
                base.PrimaryColor = value;
                _setProperties.Add(nameof(PrimaryColor));
            }
        }

        /// <inheritdoc />
        public new Color SecondaryColor
        {
            get => base.SecondaryColor;
            set
            {
                base.SecondaryColor = value;
                _setProperties.Add(nameof(SecondaryColor));
            }
        }

        /// <inheritdoc />
        public new Color AccentColor
        {
            get => base.AccentColor;
            set
            {
                base.AccentColor = value;
                _setProperties.Add(nameof(AccentColor));
            }
        }

        /// <inheritdoc />
        public new Color PrimaryBackground
        {
            get => base.PrimaryBackground;
            set
            {
                base.PrimaryBackground = value;
                _setProperties.Add(nameof(PrimaryBackground));
            }
        }

        /// <inheritdoc />
        public new Color SecondaryBackground
        {
            get => base.SecondaryBackground;
            set
            {
                base.SecondaryBackground = value;
                _setProperties.Add(nameof(SecondaryBackground));
            }
        }

        /// <inheritdoc />
        public new Color PrimaryTextColor
        {
            get => base.PrimaryTextColor;
            set
            {
                base.PrimaryTextColor = value;
                _setProperties.Add(nameof(PrimaryTextColor));
            }
        }

        /// <inheritdoc />
        public new Color SecondaryTextColor
        {
            get => base.SecondaryTextColor;
            set
            {
                base.SecondaryTextColor = value;
                _setProperties.Add(nameof(SecondaryTextColor));
            }
        }

        /// <inheritdoc />
        public new FontFamily FontFamily
        {
            get => base.FontFamily;
            set
            {
                base.FontFamily = value;
                _setProperties.Add(nameof(FontFamily));
            }
        }

        /// <inheritdoc />
        public new double FontSizeSmall
        {
            get => base.FontSizeSmall;
            set
            {
                base.FontSizeSmall = value;
                _setProperties.Add(nameof(FontSizeSmall));
            }
        }

        /// <inheritdoc />
        public new double FontSizeMedium
        {
            get => base.FontSizeMedium;
            set
            {
                base.FontSizeMedium = value;
                _setProperties.Add(nameof(FontSizeMedium));
            }
        }

        /// <inheritdoc />
        public new double FontSizeLarge
        {
            get => base.FontSizeLarge;
            set
            {
                base.FontSizeLarge = value;
                _setProperties.Add(nameof(FontSizeLarge));
            }
        }

        /// <inheritdoc />
        public new FontWeight FontWeight
        {
            get => base.FontWeight;
            set
            {
                base.FontWeight = value;
                _setProperties.Add(nameof(FontWeight));
            }
        }

        /// <inheritdoc />
        public new Color BorderColor
        {
            get => base.BorderColor;
            set
            {
                base.BorderColor = value;
                _setProperties.Add(nameof(BorderColor));
            }
        }

        /// <inheritdoc />
        public new Thickness BorderThickness
        {
            get => base.BorderThickness;
            set
            {
                base.BorderThickness = value;
                _setProperties.Add(nameof(BorderThickness));
            }
        }

        /// <inheritdoc />
        public new double BorderRadius
        {
            get => base.BorderRadius;
            set
            {
                base.BorderRadius = value;
                _setProperties.Add(nameof(BorderRadius));
            }
        }

        /// <inheritdoc />
        public new Color ErrorColor
        {
            get => base.ErrorColor;
            set
            {
                base.ErrorColor = value;
                _setProperties.Add(nameof(ErrorColor));
            }
        }

        /// <inheritdoc />
        public new Color WarningColor
        {
            get => base.WarningColor;
            set
            {
                base.WarningColor = value;
                _setProperties.Add(nameof(WarningColor));
            }
        }

        /// <inheritdoc />
        public new Color SuccessColor
        {
            get => base.SuccessColor;
            set
            {
                base.SuccessColor = value;
                _setProperties.Add(nameof(SuccessColor));
            }
        }

        /// <inheritdoc />
        public new string? Name
        {
            get => base.Name;
            set
            {
                base.Name = value;
                _setProperties.Add(nameof(Name));
            }
        }

        /// <inheritdoc />
        public new Dictionary<string, string> ControlThemeUris
        {
            get => base.ControlThemeUris;
            set
            {
                base.ControlThemeUris = value ?? new Dictionary<string, string>();
                _setProperties.Add(nameof(ControlThemeUris));
            }
        }

        /// <inheritdoc />
        public new Dictionary<string, string> StyleUris
        {
            get => base.StyleUris;
            set
            {
                base.StyleUris = value ?? new Dictionary<string, string>();
                _setProperties.Add(nameof(StyleUris));
            }
        }

        /// <inheritdoc />
        public new TypographyScale Typography
        {
            get => base.Typography;
            set
            {
                base.Typography = value ?? new TypographyScale();
                _setProperties.Add(nameof(Typography));
            }
        }

        /// <inheritdoc />
        public new FontFamily HeaderFontFamily
        {
            get => base.HeaderFontFamily;
            set
            {
                base.HeaderFontFamily = value;
                _setProperties.Add(nameof(HeaderFontFamily));
            }
        }

        /// <inheritdoc />
        public new FontFamily BodyFontFamily
        {
            get => base.BodyFontFamily;
            set
            {
                base.BodyFontFamily = value;
                _setProperties.Add(nameof(BodyFontFamily));
            }
        }

        /// <inheritdoc />
        public new FontFamily MonospaceFontFamily
        {
            get => base.MonospaceFontFamily;
            set
            {
                base.MonospaceFontFamily = value;
                _setProperties.Add(nameof(MonospaceFontFamily));
            }
        }

        /// <inheritdoc />
        public new double LineHeight
        {
            get => base.LineHeight;
            set
            {
                base.LineHeight = value;
                _setProperties.Add(nameof(LineHeight));
            }
        }

        /// <inheritdoc />
        public new double LetterSpacing
        {
            get => base.LetterSpacing;
            set
            {
                base.LetterSpacing = value;
                _setProperties.Add(nameof(LetterSpacing));
            }
        }

        /// <inheritdoc />
        public new bool EnableLigatures
        {
            get => base.EnableLigatures;
            set
            {
                base.EnableLigatures = value;
                _setProperties.Add(nameof(EnableLigatures));
            }
        }

        /// <summary>
        /// Creates a resolved skin by applying inheritance and overrides.
        /// </summary>
        /// <param name="baseTheme">The base theme to inherit from.</param>
        /// <returns>A fully resolved Skin with all properties applied.</returns>
        public Skin CreateResolvedSkin(Skin? baseTheme = null)
        {
            var resolved = new Skin();

            // Start with base theme if provided
            if (baseTheme != null)
            {
                CopyPropertiesFrom(resolved, baseTheme);
            }

            // Apply current theme's explicitly set properties only
            CopySetPropertiesFrom(resolved, this);

            // Apply property overrides
            ApplyOverrides(resolved);

            return resolved;
        }

        private void CopyPropertiesFrom(Skin target, Skin source)
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
            target.ControlThemeUris = CloneDictionary(source.ControlThemeUris);
            target.StyleUris = CloneDictionary(source.StyleUris);
            target.Typography = CloneTypography(source.Typography);
            target.HeaderFontFamily = source.HeaderFontFamily;
            target.BodyFontFamily = source.BodyFontFamily;
            target.MonospaceFontFamily = source.MonospaceFontFamily;
            target.LineHeight = source.LineHeight;
            target.LetterSpacing = source.LetterSpacing;
            target.EnableLigatures = source.EnableLigatures;
        }

        private void CopySetPropertiesFrom(Skin target, InheritableSkin source)
        {
            // Only copy properties that have been explicitly set
            if (source._setProperties.Contains(nameof(PrimaryColor)))
                target.PrimaryColor = source.PrimaryColor;
            if (source._setProperties.Contains(nameof(SecondaryColor)))
                target.SecondaryColor = source.SecondaryColor;
            if (source._setProperties.Contains(nameof(AccentColor)))
                target.AccentColor = source.AccentColor;
            if (source._setProperties.Contains(nameof(PrimaryBackground)))
                target.PrimaryBackground = source.PrimaryBackground;
            if (source._setProperties.Contains(nameof(SecondaryBackground)))
                target.SecondaryBackground = source.SecondaryBackground;
            if (source._setProperties.Contains(nameof(PrimaryTextColor)))
                target.PrimaryTextColor = source.PrimaryTextColor;
            if (source._setProperties.Contains(nameof(SecondaryTextColor)))
                target.SecondaryTextColor = source.SecondaryTextColor;
            if (source._setProperties.Contains(nameof(FontFamily)))
                target.FontFamily = source.FontFamily;
            if (source._setProperties.Contains(nameof(FontSizeSmall)))
                target.FontSizeSmall = source.FontSizeSmall;
            if (source._setProperties.Contains(nameof(FontSizeMedium)))
                target.FontSizeMedium = source.FontSizeMedium;
            if (source._setProperties.Contains(nameof(FontSizeLarge)))
                target.FontSizeLarge = source.FontSizeLarge;
            if (source._setProperties.Contains(nameof(FontWeight)))
                target.FontWeight = source.FontWeight;
            if (source._setProperties.Contains(nameof(BorderColor)))
                target.BorderColor = source.BorderColor;
            if (source._setProperties.Contains(nameof(BorderThickness)))
                target.BorderThickness = source.BorderThickness;
            if (source._setProperties.Contains(nameof(BorderRadius)))
                target.BorderRadius = source.BorderRadius;
            if (source._setProperties.Contains(nameof(ErrorColor)))
                target.ErrorColor = source.ErrorColor;
            if (source._setProperties.Contains(nameof(WarningColor)))
                target.WarningColor = source.WarningColor;
            if (source._setProperties.Contains(nameof(SuccessColor)))
                target.SuccessColor = source.SuccessColor;
            if (source._setProperties.Contains(nameof(Name)))
                target.Name = source.Name;
            if (source._setProperties.Contains(nameof(ControlThemeUris)) || !DictionariesEqual(source.ControlThemeUris, source._defaultControlThemeUris))
                MergeDictionary(target.ControlThemeUris, source.ControlThemeUris);
            if (source._setProperties.Contains(nameof(StyleUris)) || !DictionariesEqual(source.StyleUris, source._defaultStyleUris))
                MergeDictionary(target.StyleUris, source.StyleUris);
            if (source._setProperties.Contains(nameof(Typography)) || !TypographyEquals(source.Typography, source._defaultTypography))
                MergeTypography(target.Typography, source.Typography, source._defaultTypography);
            if (source._setProperties.Contains(nameof(HeaderFontFamily)))
                target.HeaderFontFamily = source.HeaderFontFamily;
            if (source._setProperties.Contains(nameof(BodyFontFamily)))
                target.BodyFontFamily = source.BodyFontFamily;
            if (source._setProperties.Contains(nameof(MonospaceFontFamily)))
                target.MonospaceFontFamily = source.MonospaceFontFamily;
            if (source._setProperties.Contains(nameof(LineHeight)))
                target.LineHeight = source.LineHeight;
            if (source._setProperties.Contains(nameof(LetterSpacing)))
                target.LetterSpacing = source.LetterSpacing;
            if (source._setProperties.Contains(nameof(EnableLigatures)))
                target.EnableLigatures = source.EnableLigatures;
        }

        private void ApplyOverrides(Skin target)
        {
            foreach (var kvp in PropertyOverrides)
            {
                var property = typeof(Skin).GetProperty(kvp.Key);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        var value = ConvertValue(kvp.Value, property.PropertyType);
                        property.SetValue(target, value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to apply override for {kvp.Key}: {ex.Message}");
                    }
                }
            }
        }

        private static Dictionary<string, string> CloneDictionary(Dictionary<string, string>? source)
        {
            return source == null ? new Dictionary<string, string>() : new Dictionary<string, string>(source);
        }

        private static void MergeDictionary(Dictionary<string, string> target, Dictionary<string, string>? source)
        {
            if (source == null)
                return;

            foreach (var kvp in source)
            {
                target[kvp.Key] = kvp.Value;
            }
        }

        private static bool DictionariesEqual(Dictionary<string, string>? left, Dictionary<string, string>? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null || left.Count != right.Count)
                return false;

            foreach (var kvp in left)
            {
                if (!right.TryGetValue(kvp.Key, out var value) || value != kvp.Value)
                    return false;
            }

            return true;
        }

        private static TypographyScale CloneTypography(TypographyScale? source)
        {
            source ??= new TypographyScale();

            return new TypographyScale
            {
                DisplayLarge = source.DisplayLarge,
                DisplayMedium = source.DisplayMedium,
                DisplaySmall = source.DisplaySmall,
                HeadlineLarge = source.HeadlineLarge,
                HeadlineMedium = source.HeadlineMedium,
                HeadlineSmall = source.HeadlineSmall,
                TitleLarge = source.TitleLarge,
                TitleMedium = source.TitleMedium,
                TitleSmall = source.TitleSmall,
                LabelLarge = source.LabelLarge,
                LabelMedium = source.LabelMedium,
                LabelSmall = source.LabelSmall,
                BodyLarge = source.BodyLarge,
                BodyMedium = source.BodyMedium,
                BodySmall = source.BodySmall
            };
        }

        private static bool TypographyEquals(TypographyScale? left, TypographyScale? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null)
                return false;

            return left.DisplayLarge == right.DisplayLarge &&
                   left.DisplayMedium == right.DisplayMedium &&
                   left.DisplaySmall == right.DisplaySmall &&
                   left.HeadlineLarge == right.HeadlineLarge &&
                   left.HeadlineMedium == right.HeadlineMedium &&
                   left.HeadlineSmall == right.HeadlineSmall &&
                   left.TitleLarge == right.TitleLarge &&
                   left.TitleMedium == right.TitleMedium &&
                   left.TitleSmall == right.TitleSmall &&
                   left.LabelLarge == right.LabelLarge &&
                   left.LabelMedium == right.LabelMedium &&
                   left.LabelSmall == right.LabelSmall &&
                   left.BodyLarge == right.BodyLarge &&
                   left.BodyMedium == right.BodyMedium &&
                   left.BodySmall == right.BodySmall;
        }

        private static void MergeTypography(TypographyScale target, TypographyScale source, TypographyScale defaults)
        {
            if (source.DisplayLarge != defaults.DisplayLarge) target.DisplayLarge = source.DisplayLarge;
            if (source.DisplayMedium != defaults.DisplayMedium) target.DisplayMedium = source.DisplayMedium;
            if (source.DisplaySmall != defaults.DisplaySmall) target.DisplaySmall = source.DisplaySmall;
            if (source.HeadlineLarge != defaults.HeadlineLarge) target.HeadlineLarge = source.HeadlineLarge;
            if (source.HeadlineMedium != defaults.HeadlineMedium) target.HeadlineMedium = source.HeadlineMedium;
            if (source.HeadlineSmall != defaults.HeadlineSmall) target.HeadlineSmall = source.HeadlineSmall;
            if (source.TitleLarge != defaults.TitleLarge) target.TitleLarge = source.TitleLarge;
            if (source.TitleMedium != defaults.TitleMedium) target.TitleMedium = source.TitleMedium;
            if (source.TitleSmall != defaults.TitleSmall) target.TitleSmall = source.TitleSmall;
            if (source.LabelLarge != defaults.LabelLarge) target.LabelLarge = source.LabelLarge;
            if (source.LabelMedium != defaults.LabelMedium) target.LabelMedium = source.LabelMedium;
            if (source.LabelSmall != defaults.LabelSmall) target.LabelSmall = source.LabelSmall;
            if (source.BodyLarge != defaults.BodyLarge) target.BodyLarge = source.BodyLarge;
            if (source.BodyMedium != defaults.BodyMedium) target.BodyMedium = source.BodyMedium;
            if (source.BodySmall != defaults.BodySmall) target.BodySmall = source.BodySmall;
        }

        // Update the ConvertValue method in InheritableSkin.cs
        private object? ConvertValue(object? value, Type targetType)
        {
            if (value == null) return null;

            if (targetType == typeof(Color) && value is string colorString)
            {
                return Color.Parse(colorString);
            }

            if (targetType == typeof(FontFamily) && value is string fontString)
            {
                return new FontFamily(fontString);
            }

            if (targetType == typeof(FontWeight) && value is string fontWeightString)
            {
                return Enum.TryParse<FontWeight>(fontWeightString, out var weight)
                    ? weight
                    : FontWeight.Normal;
            }

            if (targetType == typeof(Thickness))
            {
                if (value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        return new Thickness(element.GetDouble());
                    }
                    else if (element.ValueKind == JsonValueKind.String)
                    {
                        return Thickness.Parse(element.GetString() ?? "0");
                    }
                    else if (element.ValueKind == JsonValueKind.Object)
                    {
                        var left = element.TryGetProperty("left", out var leftProp) ? leftProp.GetDouble() : 0;
                        var top = element.TryGetProperty("top", out var topProp) ? topProp.GetDouble() : 0;
                        var right = element.TryGetProperty("right", out var rightProp) ? rightProp.GetDouble() : 0;
                        var bottom = element.TryGetProperty("bottom", out var bottomProp) ? bottomProp.GetDouble() : 0;
                        return new Thickness(left, top, right, bottom);
                    }
                }
                else if (value is string thicknessString)
                {
                    return Thickness.Parse(thicknessString);
                }
            }

            if (targetType == typeof(CornerRadius))
            {
                if (value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        return new CornerRadius(element.GetDouble());
                    }
                    else if (element.ValueKind == JsonValueKind.String)
                    {
                        var radiusString = element.GetString() ?? "0";
                        return double.TryParse(radiusString, out var radius)
                            ? new CornerRadius(radius)
                            : new CornerRadius(0);
                    }
                }
                else if (value is string radiusString)
                {
                    return double.TryParse(radiusString, out var radius)
                        ? new CornerRadius(radius)
                        : new CornerRadius(0);
                }
            }

            // Try standard type conversion as fallback
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }
    }
}
