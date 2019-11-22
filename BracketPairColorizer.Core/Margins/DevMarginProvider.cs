using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Margins
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class DevMarginProvider : IWpfTextViewMarginProvider
    {
        [Import]
        private readonly IFileExtensionRegistryService ferService = null;

        [Import]
        private readonly IVsfSettings settisettingsnsg = null;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return new DevViewMargin(wpfTextViewHost, ferService, settings);
        }
    }
}
