using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Text
{
    internal sealed class CurrentColumnAdornmentFactory : IWpfTextViewMarginProvider
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import]
        public IClassificationFormatMapService FormatMapService { get; set; }

        [Import]
        public IBpcSettings Settigs { get; set; }

        [Export(typeof(AdornmentLayerDefinition))]
        [Name(ZoomConstants.COLUMN_HIGHLIGHT)]
        [Order(Before = AdornmentLayers.Interline)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            var classification = ClassificationRegistry.GetClassificationType(Constants.COLUMN_HIGHLIGHT);
            var map = FormatMapService.GetClassificationFormatMap(textView);

            textView.Properties.GetOrCreateSingletonProperty(() =>
            {
                return new CurrentColumnAdornment(textView, map, classification, Settings);
            });
        }
    }
}
