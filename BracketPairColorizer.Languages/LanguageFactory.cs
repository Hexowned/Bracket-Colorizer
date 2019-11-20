using BracketPairColorizer.Settings.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Languages
{
    [Export(typeof(ILanguageFactory))]
    public class LanguageFactory : ILanguageFactory
    {
        [ImportMany]
        public List<ILanguage> Languages { get; set; }

        private ILanguage defaultLanguage;

        [ImportingConstructor]
        public LanguageFactory(ITypedSettingsStore store)
        {
            this.defaultLanguage = new DefaultLanguage(store);
        }

        public IEnumerable<ILanguage> GetAllLanguages()
        {
            return this.Languages;
        }

        public ILanguage TryCreateLanguage(Func<string, bool> contentTypeMatcher)
        {
            foreach (ILanguage lang in Languages)
            {
                if (lang.MatchesContentType(contentTypeMatcher))
                {
                    return lang;
                }
            }

            return defaultLanguage;
        }

        public ILanguage TryCreateLanguage(string key)
        {
            foreach (ILanguage lang in Languages)
            {
                if (lang.Settings.KeyName == key)
                {
                    return lang;
                }
            }

            return defaultLanguage;
        }
    }
}
