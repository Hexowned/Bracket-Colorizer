namespace BracketPairColorizer.Xml
{
    public class HtmlMarkup : IMarkupLanguage
    {
        public bool IsDelimiter(string name)
        {
            return name == "HTML Tag Delimiter" || name == "HTML Operator";
        }

        public bool IsName(string name)
        {
            return name == "HTML Element Name";
        }

        public bool IsAttribute(string name)
        {
            return name == "HTML Attribute Name";
        }

        public bool IsRazorTag(string name)
        {
            return name == "RazorTagHelperElement";
        }
    }
}