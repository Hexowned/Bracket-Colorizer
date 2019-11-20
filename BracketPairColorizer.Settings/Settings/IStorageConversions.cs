namespace BracketPairColorizer.Settings.Settings
{
    public interface IStorageConversions
    {
        bool ToBoolean(string value);

        int ToInt32(string value);

        long ToInt64(string value);

        double ToDouble(string value);

        bool ToEnum<T>(string value, out T result) where T : struct;

        string[] ToList(string value);

        string ToString(object value);
    }
}
