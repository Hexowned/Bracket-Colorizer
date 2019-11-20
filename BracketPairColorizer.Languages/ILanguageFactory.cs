using System;
using System.Collections.Generic;

namespace BracketPairColorizer.Languages
{
    public interface ILanguageFactory
    {
        IEnumerable<ILanguage> GetAllLanguages();

        ILanguage TryCreateLanguage(Func<string, bool> contentTypeMatcher);

        ILanguage TryCreateLanguage(string key);
    }
}
