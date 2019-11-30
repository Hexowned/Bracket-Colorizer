using BracketPairColorizer.Settings.Settings;

namespace BracketPairColorizer.Xml
{
    public interface IXmlSettings : IUpdatableSettings
    {
        bool XmlnsPrefixEnabled { get; set; }
        bool XmlCloseTagEnabled { get; set; }
        bool XmlMatchTagsEnabled { get; set; }

        void Save();
    }
}
