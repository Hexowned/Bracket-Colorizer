using BracketPairColorizer.Core.Utilities;
using System.Collections.Generic;

namespace BracketPairColorizer.Core.Text
{
    public class ModeLineParser : IModeLineParser
    {
        public IDictionary<string, string> Parse(string text)
        {
            var result = new Dictionary<string, string>();
            ITokenizer tokenizer = new SimpleTokenizer(text);
            ParseModeLine(tokenizer, result);

            return result;
        }

        private void ParseModeLine(ITokenizer tokenizer, Dictionary<string, string> result)
        {
            if (!tokenizer.Next())
                return;

            string tool = tokenizer.Token;
            if (!tokenizer.Next())
                return;
            if (tokenizer.Token != ":")
                return;
            if (!tokenizer.Next())
                return;

            if (tokenizer.Token == "set")
            {
                tokenizer.Next();
                ParseListNoDelimiter(tokenizer, result);
            } else
            {
                ParseList(tokenizer, result, ":");
            }
        }

        private void ParseList(ITokenizer tokenizer, Dictionary<string, string> result, string delimiter)
        {
            while (!tokenizer.AtEnd)
            {
                string p1 = tokenizer.Token;
                tokenizer.Next();
                string p2 = tokenizer.Token;
                if (p2 == "=")
                {
                    tokenizer.Next();
                    string p3 = tokenizer.Token;
                    result[p1] = p3;
                    tokenizer.Next();
                } else
                {
                    result[p1] = "";
                }

                if (tokenizer.Token != delimiter)
                    break;
                tokenizer.Next();
            }
        }

        private void ParseListNoDelimiter(ITokenizer tokenizer, Dictionary<string, string> result)
        {
            while (!tokenizer.AtEnd && tokenizer.Token != ":")
            {
                string p1 = tokenizer.Token;
                tokenizer.Next();
                string p2 = tokenizer.Token;
                if (p2 == "=")
                {
                    tokenizer.Next();
                    string p3 = tokenizer.Token;
                    result[p1] = p3;
                    tokenizer.Next();
                } else
                {
                    result[p1] = "";
                }
            }
        }
    }
}
