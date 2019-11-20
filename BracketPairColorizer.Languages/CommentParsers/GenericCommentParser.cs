using BracketPairColorizer.Languages.Utilities;
using System.Text;

namespace BracketPairColorizer.Languages.CommentParsers
{
    public class GenericCommentParser : IFirstLineCommentParser
    {
        public string Parse(ITextChars tc)
        {
            SkipWhitespace(tc);
            if (tc.Char() == '/' && tc.NChar() == '/')
            {
                // C single line comment
                tc.Skip(2);
                return TrimmedRemainder(tc);
            } else if (tc.Char() == '/' && tc.NChar() == '*')
            {
                // C multi line comment
                tc.Skip(2);
                return TrimmedMinus(tc, "*/");
            } else if (tc.Char() == '(' && tc.NChar() == '*')
            {
                // F# multi line comment
                tc.Skip(2);
                return TrimmedMinus(tc, "*)");
            } else if (tc.Char() == '-' && tc.NChar() == '-')
            {
                // SQL single line comment
                tc.Skip(2);
                return TrimmedRemainder(tc);
            } else if (tc.Char() == '#')
            {
                // Python single line comment
                tc.Skip(1);
                return TrimmedRemainder(tc);
            } else if (tc.Char() == '\'')
            {
                // Visual Basic single line comment
                tc.Skip(1);
                return TrimmedRemainder(tc);
            } else if (tc.Char() == '<' && tc.NChar() == '!' && tc.NNChar() == '-')
            {
                // XML comment
                tc.Skip(3);
                if (tc.Char() == '-')
                {
                    tc.Next();
                    return TrimmedMinus(tc, "-->");
                }
            }

            return null;
        }

        private void SkipWhitespace(ITextChars tc)
        {
            while (!tc.AtEnd && char.IsWhiteSpace(tc.Char()))
            {
                tc.Next();
            }
        }

        // we assume endChars is 2 or 3, which is the most common case
        private string TrimmedMinus(ITextChars tc, string t)
        {
            StringBuilder buffer = new StringBuilder();
            while (!tc.AtEnd)
            {
                if (tc.Char() == t[0] && tc.NChar() == t[1])
                {
                    if (t.Length <= 2 || tc.NNChar() == t[2])
                    {
                        break;
                    }
                }

                buffer.Append(tc.Char());
                tc.Next();
            }

            return buffer.ToString().Trim();
        }

        private string TrimmedRemainder(ITextChars tc)
        {
            return tc.GetRemainder().Trim();
        }
    }
}
