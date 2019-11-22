using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Rainbow
{
    public interface ITextLinesSource
    {
        int Length { get; }

        int GetLineNumberFromPosition(int position);

        ITextChars GetLineFromLineNumber(int lineNumber);

        bool IsSame(ITextLinesSource source);
    }
}
