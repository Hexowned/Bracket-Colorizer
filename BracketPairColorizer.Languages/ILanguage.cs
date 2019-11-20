using BracketPairColorizer.Languages.Utilities;
using System;

namespace BracketPairColorizer.Languages
{
    public interface ILanguage
    {
        ILanguageSettings Settings { get; }
        Func<string, string> NormalizationFunction { get; }

        T GetService<T>();

        IStringScanner NewStringScanner(string classificationName, string text);

        bool MatchesContentType(Func<string, bool> contentTypeMatches);

        bool IsKeywordClassification(string classificationType);
    }
}
