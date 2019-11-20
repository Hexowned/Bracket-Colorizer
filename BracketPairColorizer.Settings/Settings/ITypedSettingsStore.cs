namespace BracketPairColorizer.Settings.Settings
{
    public interface ITypedSettingsStore : ISettingsStore, IUpdatableSettings
    {
        string GetString(string name, string defaultValue);

        bool GetBoolean(string name, bool defaultValue);

        int GetInt32(string name, int defaultValue);

        long GetInt64(string name, long defaultValue);

        double GetDouble(string name, long defaultValue);

        T GetEnum<T>(string name, T defaultValue) where T : struct;

        string[] GetList(string name, string[] defaultValue);

        void SetValue(string name, object value);
    }
}
