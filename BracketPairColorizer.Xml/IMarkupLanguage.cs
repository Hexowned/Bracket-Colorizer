namespace BracketPairColorizer.Xml
{
    interface IMarkupLanguage
    {
        bool IsDelimiter(string name);
        bool IsName(string name);
        bool IsAttribute(string name);
        bool IsRazorTag(string name);
    }
}