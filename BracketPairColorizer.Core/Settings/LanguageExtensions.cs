using BracketPairColorizer.Languages;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Linq;

namespace BracketPairColorizer.Core.Settings
{
    public static class LanguageExtensions
    {
        private static readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;

        public static bool IsControlFlowKeyword(this ILanguage language, string text)
        {
            return language.Settings.ControlFlow.Contains(language.NormalizationFunction(text), comparer);
        }

        public static bool IsVisibilityKeyword(this ILanguage language, string text)
        {
            return language.Settings.Visibility.Contains(language.NormalizationFunction(text), comparer);
        }

        public static bool IsLinqKeyword(this ILanguage language, string text)
        {
            return language.Settings.Linq.Contains(language.NormalizationFunction(text), comparer);
        }

        public static ILanguage TryCreateLanguage(this ILanguageFactory factory, ITextBuffer buffer)
        {
            return factory.TryCreateLanguage(buffer.ContentType);
        }

        public static ILanguage TryCreateLanguage(this ILanguageFactory factory, ITextSnapshot snapshot)
        {
            return factory.TryCreateLanguage(snapshot.ContentType);
        }

        public static ILanguage TryCreateLanguage(this ILanguageFactory factory, IContentType contentType)
        {
            Func<string, bool> matcher = (string language) => contentType.IsOfType(language);

            return factory.TryCreateLanguage(matcher);
        }
    }
}
