using System.Collections.Generic;

namespace BracketPairColorizer.Languages.Utilities
{
    public interface IModeLineParser
    {
        IDictionary<string, string> Parse(string text);
    }
}
