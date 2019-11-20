using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public interface IBraceScanner
    {
        string BraceList { get; }

        void Reset(int state);

        bool Extract(ITextChars text, ref CharPosition pos);
    }
}
