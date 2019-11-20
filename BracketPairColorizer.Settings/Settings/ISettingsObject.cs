using Newtonsoft.Json;

namespace BracketPairColorizer.Settings.Settings
{
    public interface ISettingsObject
    {
        string Name { get; }

        void Read(JsonTextReader reader);

        void Save(JsonTextWriter writer);
    }
}
