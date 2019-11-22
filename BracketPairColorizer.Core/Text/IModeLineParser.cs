using System.Collections.Generic;

namespace BracketPairColorizer.Core.Text
{
    public interface IModeLineParser
    {
        IDictionary<string, string> Parse(string text);
    }
}
