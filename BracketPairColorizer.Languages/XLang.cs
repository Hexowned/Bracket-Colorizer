using BracketPairColorizer.Languages.BraceScanners;
using BracketPairColorizer.Languages.Lang;
using BracketPairColorizer.Languages.Sequences;
using BracketPairColorizer.Languages.Utilities;
using BracketPairColorizer.Settings.Settings;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Languages
{
    [Export(typeof(ILanguage))]
    public class XLang : CBasedLanguage, ILanguage
    {
        public const string ContentType = "Fake.XLANG/s";

        protected override string[] SupportedContentTypes
            => new string[] { ContentType };

        public ILanguageSettings Settings { get; private set; }

        [ImportingConstructor]
        public XLang(ITypedSettingsStore store)
        {
            this.Settings = new XLangSettings(store);
        }

        protected override IBraceScanner NewBraceScanner()
            => new CSharpBraceScanner();

        public override IStringScanner NewStringScanner(string classificationName, string text)
            => new CSharpsStringScanner(text);
    }

    internal class XLangSettings : LanguageSettings
    {
        protected override string[] ControlFlowDefaults => EMPTY;
        protected override string[] LinqDefaults => EMPTY;
        protected override string[] VisibilityDefaults => EMPTY;

        public XLangSettings(ITypedSettingsStore store)
            : base(Langs.XLang, store)
        {
        }
    }
}
