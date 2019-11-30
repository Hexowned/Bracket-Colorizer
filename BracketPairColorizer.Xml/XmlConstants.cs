using Microsoft.VisualStudio.Language.StandardClassification;
using System;

namespace BracketPairColorizer.Xml
{
    public static class XmlConstants
    {
        public const string CT_XML = "XML";
        public const string CT_XAML = "XAML";
        public const string CT_HTML = "HTML";

        public const string CT_HTMLX = "htmlx";
        public const string XML_CLOSING = "BracketPairColorizer.xml.closing";
        public const string XML_PREFIX = "BracketPairColorizser.xml.prefix";
        public const string XML_CLOSING_PREFIX = "BracketPairColorizer.xml.closing.prefix";
        public const string RAZOR_CLOSING = "BracketPairColorizer.razor.closing.element";
        public const string DELIMITER = PredefinedClassificationTypeNames.Operator;
    }
}