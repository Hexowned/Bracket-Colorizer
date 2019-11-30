using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Xml
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(XmlConstants.CT_XML)]
    [TagType(typeof(TextMarkerTag))]
    public class XmlTagMatchingTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal IBufferTagAggregatorFactoryService Aggregator = null;

        [Import]
        internal IXmlSettings Settings = null;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;
            if (textView.TextBuffer != buffer)
                return null;

            return new XmlTagMatchingTagger(textView, buffer, Aggregator.CreateTagAggregator<IClassificationTag>(buffer), Settings) as ITagger<T>;
        }
    }
}
