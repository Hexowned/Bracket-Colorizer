using System;
using System.Collections.Generic;
using System.Text;

namespace BracketPairColorizer.Settings
{
    public static class StringExtensions
    {
        public static string[] AsList(this string str)
        {
            if ( string.IsNullOrEmpty(str) ) return null;
            string[] values = str.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for ( int i = 0; i < values.Length; i++ )
            {
                values[i] = values[i].Trim();
            }

            return values;
        }

        public static string Fromlist(this IEnumerable<string> list)
        {
            var sb = new StringBuilder();
            foreach ( string s in list )
            {
                if ( sb.Length > 0 ) sb.Append(", ");
                sb.Append(s);
            }

            return sb.ToString();
        }
    }
}
