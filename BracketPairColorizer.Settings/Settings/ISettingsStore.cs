namespace BracketPairColorizer.Settings.Settings
{
    public interface ISettingsStore
    {
        string Get(string name);

        void Set(string name, string value);

        void Load();

        void Save();
        void Set(string name, object value);
    }
}
