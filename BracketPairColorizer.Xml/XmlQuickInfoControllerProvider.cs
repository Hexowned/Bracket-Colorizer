using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Xml
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("BracketPairColorizer Xml QuickInfo Controller")]
    [ContentType(XmlConstants.CT_XML)]
    [ContentType(XmlConstants.CT_XAML)]
    internal class XmlQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new XmlQuickInfoController(textView, subjectBuffers, this);
        }
    }
}
