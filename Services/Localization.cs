using System.Globalization;
using BO3SoundSuite.Models;

namespace BO3SoundSuite.Services;

public static class Localization
{
    public static AppLanguage Current { get; private set; } = AppLanguage.Auto;

    public static bool IsFrench => Resolve(Current) == AppLanguage.Français;

    public static void Set(AppLanguage language) => Current = language;

    public static AppLanguage Resolve(AppLanguage language)
    {
        if (language != AppLanguage.Auto) return language;
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase)
            ? AppLanguage.Français
            : AppLanguage.English;
    }

    public static string T(string fr, string en) => IsFrench ? fr : en;
}
