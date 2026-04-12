// Theme/ThemeValidation.cs

using Avalonia.Media;
using AvaloniaThemeManager.Theme.ValidationRules;

namespace AvaloniaThemeManager.Theme
{
    /// <summary>
    /// Validates theme configurations and provides error recovery.
    /// </summary>
    public class ThemeValidator : IThemeValidator
    {
        private readonly List<IThemeValidationRule> _validationRules;
        private readonly IThemeValidationHelper _validationHelper;
        private readonly IThemeAutoFixer _themeAutoFixer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeValidator"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the default validation rules for theme validation, 
        /// including checks for color contrast, font size, border consistency, naming conventions, 
        /// and accessibility compliance.
        /// </remarks>
        public ThemeValidator()
            : this(new ThemeValidationHelper(), new ThemeAutoFixer())
        {
        }

        private ThemeValidator(IThemeValidationHelper validationHelper, IThemeAutoFixer themeAutoFixer)
            : this(CreateDefaultRules(validationHelper), validationHelper, themeAutoFixer)
        {
        }

        /// <summary>
        /// Initializes a new validator with the provided rules and helper dependency.
        /// </summary>
        /// <param name="validationRules">The validation rules to apply.</param>
        /// <param name="validationHelper">The shared validation helper.</param>
        /// <param name="themeAutoFixer">The shared theme auto-fixer.</param>
        public ThemeValidator(
            IEnumerable<IThemeValidationRule> validationRules,
            IThemeValidationHelper validationHelper,
            IThemeAutoFixer themeAutoFixer)
        {
            _validationRules = validationRules?.ToList() ?? throw new ArgumentNullException(nameof(validationRules));
            _validationHelper = validationHelper ?? throw new ArgumentNullException(nameof(validationHelper));
            _themeAutoFixer = themeAutoFixer ?? throw new ArgumentNullException(nameof(themeAutoFixer));
        }

        /// <summary>
        /// Validates a theme and returns validation results.
        /// </summary>
        public ThemeValidationResult ValidateTheme(Skin theme)
        {
            var result = new ThemeValidationResult();

            foreach (var rule in _validationRules)
            {
                var ruleResult = rule.Validate(theme);

                result.Errors.AddRange(ruleResult.Errors);
                result.Warnings.AddRange(ruleResult.Warnings);
            }

            // FIX: Properly set IsValid based on errors
            result.IsValid = result.Errors.Count == 0;

            return result;
        }

        /// <summary>
        /// Attempts to fix validation errors automatically.
        /// </summary>
        public Skin AutoFixTheme(Skin theme)
        {
            return _themeAutoFixer.AutoFixTheme(theme);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public double CalculateContrastRatio(Color foreground, Color background)
        {
            return _validationHelper.CalculateContrastRatio(foreground, background);
        }

        private static IEnumerable<IThemeValidationRule> CreateDefaultRules(IThemeValidationHelper validationHelper)
        {
            return new IThemeValidationRule[]
            {
                new ColorContrastValidationRule(validationHelper),
                new FontSizeValidationRule(),
                new BorderValidationRule(validationHelper),
                new NameValidationRule(),
                new AccessibilityValidationRule(validationHelper)
            };
        }
    }
   
}
