namespace BracketPairColorizer.Settings.Settings
{
    public interface ISettingsExport
    {
        void Export<T>(T settingsObject) where T : class;

        void Import(ISettingsStore store);

        void Load(string sourcePath);

        void Save(string targetPath);
    }
}
