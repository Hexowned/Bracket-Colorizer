namespace BracketPairColorizer.Settings.Settings
{
    public interface ISolutionUserSettings
    {
        void Store<T>(string filePath, T settingsObject) where T : ISettingsObject;

        T Load<T>(string filePath) where T : ISettingsObject, new();
    }
}
