using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.Sequences
{
    public class PowerShellStringScanner : IStringScanner
    {
        private string text;
        private int start;

        private PowerShellStringScanner(string text)
        {
            this.text = text;
            // quotes are included, so start at 1
            this.start = 1;
            // single-quoted string in powersell don't support escape sequences
            if (text.StartsWith("'") || text.StartsWith("@"))
                this.start = text.Length;
        }

        public StringPart? Next()
        {
            while (this.start < this.text.Length - 2)
            {
                if (this.text[this.start] == '`')
                {
                    var span = new TextSpan(this.start, 2);
                    this.start += 2;

                    return new StringPart(span);
                }

                this.start++;
            }

            return null;
        }
    }
}
