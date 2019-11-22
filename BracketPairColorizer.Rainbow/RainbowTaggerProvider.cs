using BracketPairColorizer.Core.Tags;
using BracketPairColorizer.Languages;
using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(ContentTypes.Text)]
    [TagType(typeof(RainbowTag))]
    public class RainbowTaggerProvider : ITaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import]
        public ILanguageFactory LanguageFactory { get; set; }

        [Import]
        public IRainbowSettings Settings { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            RainbowProvider provider = buffer.Properties.GetOrCreateSingletonProperty(()
                => new RainbowProvider(buffer, this));

            return provider.ColorTagger as ITagger<T>;
        }
    }
}
