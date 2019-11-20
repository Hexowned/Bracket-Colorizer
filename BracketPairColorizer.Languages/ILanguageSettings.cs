namespace BracketPairColorizer.Languages
{
    public interface ILanguageSettings
    {
        string KeyName { get; }
        string[] ControlFlow { get; set; }
        string[] Linq { get; set; }
        string[] Visibility { get; set; }
        bool Enabled { get; set; }

        void Load();

        void Save();
    }
}
