using Microsoft.VisualStudio.Text.Editor;

namespace BracketPairColorizer.Rainbow.Utilities
{
    public interface IToolTipWindowProvider
    {
        IToolTipWindow CreateToolTip(ITextView textView);
    }
}
