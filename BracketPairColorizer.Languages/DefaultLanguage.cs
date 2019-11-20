using BracketPairColorizer.Languages.BraceScanners;
using BracketPairColorizer.Settings.Settings;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Languages
{
    public class DefaultLanguage : LanguageInfo, ILanguage
    {
        protected override string[] SupportedContentTypes => new string[0];
        public ILanguageSettings Settings { get; private set; }

        [ImportingConstructor]
        public DefaultLanguage(ITypedSettingsStore store)
        {
            this.Settings = new DefaultSettings(store);
        }

        public override bool MatchesContentType(Func<string, bool> _)
        {
            return true;
        }

        protected override IBraceScanner NewBraceScanner()
            => new DefaultBraceScanner();
    }

    public class DefaultSettings : LanguageSettings
    {
        protected override string[] ControlFlowDefaults => EMPTY;
        protected override string[] LinqDefaults => EMPTY;
        protected override string[] VisibilityDefaults => EMPTY;

        public DefaultSettings(ITypedSettingsStore store)
            : base("Text", store)
        {
        }
    }
}
