using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using AvaloniaThemeManager.Services.Interfaces;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Applies skin resources, typography, and merged dictionaries to the application.
    /// </summary>
    public class SkinResourceApplier : ISkinResourceApplier
    {
        private readonly IApplication _application;
        private readonly List<IResourceProvider> _appliedThemeResources = new();

        /// <summary>
        /// Initializes a new resource applier.
        /// </summary>
        public SkinResourceApplier(IApplication application)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));
        }

        /// <inheritdoc />
        public void ApplySkinResources(Skin skin)
        {
            ArgumentNullException.ThrowIfNull(skin);

            var resources = _application.Resources;
            if (resources == null)
            {
                return;
            }

            UpdateBrush(resources, "PrimaryColorBrush", skin.PrimaryColor);
            UpdateBrush(resources, "SecondaryColorBrush", skin.SecondaryColor);
            UpdateBrush(resources, "AccentBlueBrush", skin.AccentColor);
            UpdateBrush(resources, "GunMetalDarkBrush", skin.PrimaryColor);
            UpdateBrush(resources, "GunMetalMediumBrush", skin.SecondaryColor);
            UpdateBrush(resources, "GunMetalLightBrush", skin.SecondaryBackground);
            UpdateBrush(resources, "BackgroundBrush", skin.PrimaryBackground);
            UpdateBrush(resources, "BackgroundLightBrush", skin.SecondaryBackground);
            var dark = new Color(skin.PrimaryBackground.A, (byte)(skin.PrimaryBackground.R * 0.8), (byte)(skin.PrimaryBackground.G * 0.8), (byte)(skin.PrimaryBackground.B * 0.8));
            UpdateBrush(resources, "BackgroundDarkBrush", dark);
            UpdateBrush(resources, "TextPrimaryBrush", skin.PrimaryTextColor);
            UpdateBrush(resources, "TextSecondaryBrush", skin.SecondaryTextColor);
            UpdateBrush(resources, "BorderBrush", skin.BorderColor);
            UpdateBrush(resources, "ErrorBrush", skin.ErrorColor);
            UpdateBrush(resources, "WarningBrush", skin.WarningColor);
            UpdateBrush(resources, "SuccessBrush", skin.SuccessColor);

            resources["DefaultFontFamily"] = skin.FontFamily;
            resources["FontSizeSmall"] = skin.FontSizeSmall;
            resources["FontSizeMedium"] = skin.FontSizeMedium;
            resources["FontSizeLarge"] = skin.FontSizeLarge;
            resources["DefaultFontWeight"] = skin.FontWeight;
            resources["BorderThickness"] = skin.BorderThickness;
            resources["CornerRadius"] = new CornerRadius(skin.BorderRadius);

            UpdateTypographyResources(resources, skin);
            ApplyControlThemes(resources, skin);
        }

        private void UpdateTypographyResources(IResourceDictionary resources, Skin skin)
        {
            resources["DisplayLargeFontSize"] = skin.Typography.DisplayLarge;
            resources["DisplayMediumFontSize"] = skin.Typography.DisplayMedium;
            resources["DisplaySmallFontSize"] = skin.Typography.DisplaySmall;
            resources["HeadlineLargeFontSize"] = skin.Typography.HeadlineLarge;
            resources["HeadlineMediumFontSize"] = skin.Typography.HeadlineMedium;
            resources["HeadlineSmallFontSize"] = skin.Typography.HeadlineSmall;
            resources["TitleLargeFontSize"] = skin.Typography.TitleLarge;
            resources["TitleMediumFontSize"] = skin.Typography.TitleMedium;
            resources["TitleSmallFontSize"] = skin.Typography.TitleSmall;
            resources["LabelLargeFontSize"] = skin.Typography.LabelLarge;
            resources["LabelMediumFontSize"] = skin.Typography.LabelMedium;
            resources["LabelSmallFontSize"] = skin.Typography.LabelSmall;
            resources["BodyLargeFontSize"] = skin.Typography.BodyLarge;
            resources["BodyMediumFontSize"] = skin.Typography.BodyMedium;
            resources["BodySmallFontSize"] = skin.Typography.BodySmall;
            resources["HeaderFontFamily"] = skin.HeaderFontFamily;
            resources["BodyFontFamily"] = skin.BodyFontFamily;
            resources["MonospaceFontFamily"] = skin.MonospaceFontFamily;
            resources["DefaultLineHeight"] = skin.LineHeight;
            resources["DefaultLetterSpacing"] = skin.LetterSpacing;
        }

        private void ApplyControlThemes(IResourceDictionary resources, Skin skin)
        {
            foreach (var resource in _appliedThemeResources)
            {
                resources.MergedDictionaries.Remove(resource);
            }
            _appliedThemeResources.Clear();

            foreach (var kvp in skin.ControlThemeUris)
            {
                var resource = new ResourceInclude(new Uri("avares://AvaloniaThemeManager/"))
                {
                    Source = new Uri(kvp.Value)
                };
                resources.MergedDictionaries.Add(resource);
                _appliedThemeResources.Add(resource);
            }

            foreach (var kvp in skin.StyleUris)
            {
                var resource = new ResourceInclude(new Uri("avares://AvaloniaThemeManager/"))
                {
                    Source = new Uri(kvp.Value)
                };
                resources.MergedDictionaries.Add(resource);
                _appliedThemeResources.Add(resource);
            }
        }

        private static void UpdateBrush(IResourceDictionary dict, string key, Color color)
        {
            if (dict.TryGetValue(key, out var existingBrush) && existingBrush is SolidColorBrush brush)
            {
                brush.Color = color;
            }
            else
            {
                dict[key] = new SolidColorBrush(color);
            }
        }
    }
}
