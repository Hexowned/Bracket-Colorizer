using Microsoft.VisualStudio.Text;

namespace BracketPairColorizer.Core.Outlining
{
    public interface IOutliningController
    {
        void CollapseSelectionRegions();

        void RemoveSelectionRegions();

        void CollapseRegion(SnapshotSpan span);
    }
}
