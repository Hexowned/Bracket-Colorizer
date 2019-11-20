using System.IO;

namespace BracketPairColorizer.Settings.Settings
{
    public class FilePersistUserSettings : IPersistSettings
    {
        public const string FILENAME = "settings.vsfuser";
        private readonly string settingsFile;

        public FilePersistUserSettings(string location)
        {
            string path = Path.GetFullPath(location);
            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = Path.GetDirectoryName(path);
            }

            this.settingsFile = Path.Combine(path, FILENAME);
        }

        public void Write(byte[] data)
        {
            File.WriteAllBytes(this.settingsFile, data);
        }

        public byte[] Read()
        {
            if (!SettingsFileExists())
            {
                return null;
            }

            return File.ReadAllBytes(this.settingsFile);
        }

        private bool SettingsFileExists()
        {
            return File.Exists(this.settingsFile);
        }
    }
}
