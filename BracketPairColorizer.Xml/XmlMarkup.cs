using System;

namespace BracketPairColorizer.Xml
{
    public class XmlMarkup : IMarkupLanguage
    {
        public bool IsDelimiter(string name)
        {
            return name == "XML Delimeter";
        }

        public bool IsName(string name)
        {
            return name == "XML Name";
        }

        public bool IsAttribute(string name)
        {
            return name == "XML Attribute";
        }

        public bool IsRazorTag(string name)
        {
            return false;
        }
    }
}