using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.Sequences
{
    public class FSharpStringScanner : IStringScanner
    {
        private ITextChars text;
        private const string escapeCharacter = "\"\\'ntbrafv";

        public FSharpStringScanner(string theText)
        {
            this.text = new StringChars(theText);

            if (theText.StartsWith("\"\"\"") || theText.StartsWith("@"))
            {
                this.text.SkipRemainder();
            }
            // always skip the first char
            // (since quotes are included in the string)
            this.text.Next();
        }

        public StringPart? Next()
        {
            while (!this.text.AtEnd)
            {
                if (this.text.Char() == '\\')
                {
                    StringPart part = new StringPart();
                    if (TryParseEscapeSequence(ref part))
                        return part;
                } else if (this.text.Char() == '%')
                {
                    StringPart part = new StringPart();
                    if (TryParseFormatSpecifier(ref part))
                        return part;
                } else
                {
                    this.text.Next();
                }
            }

            return null;
        }

        private bool TryParseEscapeSequence(ref StringPart part)
        {
            // TODO:
        }

        private bool TryParseFormatSpecifier(ref StringPart part)
        {
            // TODO:
        }

        private TextSpan? TryParseShortUnicode()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!this.text.Char().IsHexDigit()) { return null; }
                this.text.Next();
            }

            return new TextSpan(this.text.Position - 6, 6);
        }

        private TextSpan? TryParseLongUnicode()
        {
            for (int i = 0; i < 8; i++)
            {
                if (!this.text.Char().IsHexDigit()) { return null; }
                this.text.Next();
            }

            return new TextSpan(this.text.Position - 10, 10);
        }
    }
}
