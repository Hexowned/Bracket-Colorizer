using System.Collections.Generic;

namespace BracketPairColorizer.Settings.Settings
{
    public interface ICustomExport
    {
        IDictionary<string, object> Export();
    }
}
