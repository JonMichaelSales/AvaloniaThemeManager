namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Repairs common theme issues while preserving the original theme state.
    /// </summary>
    public class ThemeAutoFixer : IThemeAutoFixer
    {
        private readonly IThemeValidationHelper _validationHelper;

        /// <summary>
        /// Initializes a new fixer with the default helper implementation.
        /// </summary>
        public ThemeAutoFixer()
            : this(new ThemeValidationHelper())
        {
        }

        /// <summary>
        /// Initializes a new fixer with the provided validation helper.
        /// </summary>
        public ThemeAutoFixer(IThemeValidationHelper validationHelper)
        {
            _validationHelper = validationHelper ?? throw new ArgumentNullException(nameof(validationHelper));
        }

        /// <summary>
        /// Attempts to automatically repair common theme issues.
        /// </summary>
        public Skin AutoFixTheme(Skin theme)
        {
            ArgumentNullException.ThrowIfNull(theme);

            var fixedTheme = CloneSkin(theme);

            if (string.IsNullOrWhiteSpace(fixedTheme.Name))
            {
                fixedTheme.Name = "Custom Theme";
            }

            fixedTheme.FontSizeSmall = Math.Max(8, Math.Min(20, fixedTheme.FontSizeSmall));
            fixedTheme.FontSizeMedium = Math.Max(10, Math.Min(24, fixedTheme.FontSizeMedium));
            fixedTheme.FontSizeLarge = Math.Max(12, Math.Min(32, fixedTheme.FontSizeLarge));
            fixedTheme.BorderRadius = Math.Max(0, fixedTheme.BorderRadius);

            FixColorContrast(fixedTheme);

            return fixedTheme;
        }

        private Skin CloneSkin(Skin original)
        {
            return new Skin
            {
                Name = original.Name,
                PrimaryColor = original.PrimaryColor,
                SecondaryColor = original.SecondaryColor,
                AccentColor = original.AccentColor,
                PrimaryBackground = original.PrimaryBackground,
                SecondaryBackground = original.SecondaryBackground,
                PrimaryTextColor = original.PrimaryTextColor,
                SecondaryTextColor = original.SecondaryTextColor,
                FontFamily = original.FontFamily,
                FontSizeSmall = original.FontSizeSmall,
                FontSizeMedium = original.FontSizeMedium,
                FontSizeLarge = original.FontSizeLarge,
                FontWeight = original.FontWeight,
                BorderColor = original.BorderColor,
                BorderThickness = original.BorderThickness,
                BorderRadius = original.BorderRadius,
                ErrorColor = original.ErrorColor,
                WarningColor = original.WarningColor,
                SuccessColor = original.SuccessColor,
                ControlThemeUris = new Dictionary<string, string>(original.ControlThemeUris),
                StyleUris = new Dictionary<string, string>(original.StyleUris),
                Typography = new TypographyScale
                {
                    DisplayLarge = original.Typography.DisplayLarge,
                    DisplayMedium = original.Typography.DisplayMedium,
                    DisplaySmall = original.Typography.DisplaySmall,
                    HeadlineLarge = original.Typography.HeadlineLarge,
                    HeadlineMedium = original.Typography.HeadlineMedium,
                    HeadlineSmall = original.Typography.HeadlineSmall,
                    TitleLarge = original.Typography.TitleLarge,
                    TitleMedium = original.Typography.TitleMedium,
                    TitleSmall = original.Typography.TitleSmall,
                    LabelLarge = original.Typography.LabelLarge,
                    LabelMedium = original.Typography.LabelMedium,
                    LabelSmall = original.Typography.LabelSmall,
                    BodyLarge = original.Typography.BodyLarge,
                    BodyMedium = original.Typography.BodyMedium,
                    BodySmall = original.Typography.BodySmall
                },
                HeaderFontFamily = original.HeaderFontFamily,
                BodyFontFamily = original.BodyFontFamily,
                MonospaceFontFamily = original.MonospaceFontFamily,
                LineHeight = original.LineHeight,
                LetterSpacing = original.LetterSpacing,
                EnableLigatures = original.EnableLigatures
            };
        }

        private void FixColorContrast(Skin theme)
        {
            var primaryContrastRatio = _validationHelper.CalculateContrastRatio(theme.PrimaryTextColor, theme.PrimaryBackground);
            if (primaryContrastRatio < 4.5)
            {
                theme.PrimaryTextColor = _validationHelper.AdjustColorForContrast(theme.PrimaryTextColor, theme.PrimaryBackground, 4.5);
            }

            var secondaryContrastRatio = _validationHelper.CalculateContrastRatio(theme.SecondaryTextColor, theme.SecondaryBackground);
            if (secondaryContrastRatio < 3.0)
            {
                theme.SecondaryTextColor = _validationHelper.AdjustColorForContrast(theme.SecondaryTextColor, theme.SecondaryBackground, 3.0);
            }
        }
    }
}
