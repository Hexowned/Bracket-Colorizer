using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class DefaultBraceScanner : IBraceScanner
    {
        public string BraceList => string.Empty;

        public void Reset(int state)
        {
        }

        public bool Extract(ITextChars text, ref CharPosition pos)
        {
            text.SkipRemainder();

            return false;
        }
    }
}
