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
        /// <summary>
        /// Gets or sets the name of the base theme this theme inherits from.
        /// </summary>
        public string? BaseThemeName { get; set; }

        /// <summary>
        /// Gets or sets the collection of property overrides for this theme.
        /// </summary>
        public Dictionary<string, object> PropertyOverrides { get; set; } = new();

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

            // Apply current theme's base properties
            CopyPropertiesFrom(resolved, this);

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