using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Languages;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Text
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType("text")]
    public class ModeLineFactory : IWpfTextViewCreationListener
    {
        [Import]
        public ILanguageFactory LanguageFactory { get; set; }

        [Import]
        public IVsfSettings Settings { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            if (Settings.ModelinesEnabled)
            {
                var provider = new ModeLineProvider(textView, this);
                for (int i = 0; i < Settings.ModelinesNumberLines; i++)
                {
                    provider.ParseModeline(i);
                }
            }
        }
    }
}
