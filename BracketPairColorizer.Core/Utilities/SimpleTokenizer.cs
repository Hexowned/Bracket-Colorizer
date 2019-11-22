using BracketPairColorizer.Languages.Utilities;
using System.Text;

namespace BracketPairColorizer.Core.Utilities
{
    public class SimpleTokenizer : ITokenizer
    {
        private ITextChars tc;
        private bool reachedEnd;
        private string currentToken;

        public bool AtEnd => this.reachedEnd;
        public string Token => this.currentToken;

        public SimpleTokenizer(string text)
        {
            this.tc = new StringCharacters(text);
        }

        public bool Next()
        {
            if (this.tc.AtEnd)
            {
                this.currentToken = "";
                this.reachedEnd = true;

                return false;
            }

            while (!this.tc.AtEnd && char.IsWhiteSpace(this.tc.Char()))
            {
                this.tc.Next();
            }

            if (this.tc.AtEnd)
            {
                this.currentToken = "";
                this.reachedEnd = true;

                return false;
            }

            if (!char.IsLetterOrDigit(this.tc.Char()))
            {
                char ch = this.tc.Char();
                this.tc.Next();
                this.currentToken = "" + ch;

                return true;
            }

            var sb = new StringBuilder();
            while (!this.tc.AtEnd)
            {
                char ch = this.tc.Char();
                if (!char.IsLetterOrDigit(ch))
                    break;
                this.tc.Next();
                sb.Append(ch);
            }

            this.currentToken = sb.ToString();

            return true;
        }
    }
}
