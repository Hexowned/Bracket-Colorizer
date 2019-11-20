using BracketPairColorizer.Languages.BraceScanners;
using BracketPairColorizer.Languages.Sequences;
using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages
{
    public abstract class CBasedLanguage : LanguageInfo
    {
        protected override IBraceScanner NewBraceScanner()
            => new CBraceScanner();

        public override IStringScanner NewStringScanner(string classificationName, string text)
            => new BasicCStringScanner(text);
    }
}
