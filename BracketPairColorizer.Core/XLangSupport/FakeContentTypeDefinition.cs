using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.XLangSupport
{
    internal static class FakeContentTypeDefinition
    {
        [Export]
        [Name(ContentTypes.XLang)]
        [BaseDefinition(ContentTypes.Code)]
        internal static ContentTypeDefinition FakeXLang { get; set; }
    }
}
