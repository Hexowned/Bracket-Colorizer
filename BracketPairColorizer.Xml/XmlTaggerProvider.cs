using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Xml
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(XmlConstants.CT_XML)]
    [ContentType(XmlConstants.CT_XAML)]
    [ContentType(XmlConstants.CT_HTML)]
    [ContentType(XmlConstants.CT_HTMLX)]
    [TagType(typeof(ClassificationTag))]
    public class XmlTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import]
        internal IBufferTagAggregatorFactoryService Aggregator { get; set; }

        [Import]
        internal IXmlSettings Settings { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new XmlTagger(buffer, ClassificationRegistry, Aggregator.CreateTagAggregator<IClassificationTag>(buffer), Settings) as ITagger<T>;
        }
    }
}
