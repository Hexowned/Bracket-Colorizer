using System;

namespace BracketPairColorizer.Xml
{
    class XamlMarkup : IMarkupLanguage
    {
        public bool IsDelimiter(string tagName)
        {
            return tagName == "XAML Delimiter";
        }

        public bool IsName(string tagName)
        {
            return tagName == "XAML Name";
        }

        public bool IsAttribute(string tagName)
        {
            return tagName == "XAML Attribute";
        }

        public bool IsRazorTag(string tagName)
        {
            return false;
        }
    }
}