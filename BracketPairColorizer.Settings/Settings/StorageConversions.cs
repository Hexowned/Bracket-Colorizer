using System;
using System.ComponentModel.Composition;
using System.Globalization;

namespace BracketPairColorizer.Settings.Settings
{
    [Export(typeof(IStorageConversions))]
    internal class StorageConversions : IStorageConversions
    {
        public bool ToBoolean(string value)
        {
            return Convert.ToBoolean(value);
        }

        public int ToInt32(string value)
        {
            int result;
            var styles = NumberStyles.Integer;
            if (!int.TryParse(value, styles, CultureInfo.InvariantCulture, out result))
            {
                return Convert.ToInt32(value, CultureInfo.CurrentCulture);
            }

            return result;
        }

        public long ToInt64(string value)
        {
            long result;
            var styles = NumberStyles.Integer;
            if (!long.TryParse(value, styles, CultureInfo.InvariantCulture, out result))
            {
                return Convert.ToInt64(value, CultureInfo.CurrentCulture);
            }

            return result;
        }

        public double ToDouble(string value)
        {
            double result;
            var styles = NumberStyles.AllowLeadingWhite
                        | NumberStyles.AllowTrailingWhite
                        | NumberStyles.AllowLeadingSign
                        | NumberStyles.AllowDecimalPoint
                        | NumberStyles.AllowThousands
                        | NumberStyles.AllowExponent;
            if (!double.TryParse(value, styles, CultureInfo.InvariantCulture, out result))
            {
                return Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }

            return result;
        }

        public bool ToEnum<T>(string value, out T result) where T : struct
        {
            return Enum.TryParse<T>(value, out result);
        }

        public string[] ToList(string value)
        {
            return string.IsNullOrEmpty(value) ? new string[0] : value.AsList();
        }

        public string ToString(object value)
        {
            string[] list = value as string[];
            if (list != null)
            {
                return list.Fromlist();
            }

            return value != null ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;
        }
    }
}
