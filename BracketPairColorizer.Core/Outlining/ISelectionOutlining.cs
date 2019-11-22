using Microsoft.VisualStudio.Text;

namespace BracketPairColorizer.Core.Outlining
{
    public interface ISelectionOutlining : IUserOutlining
    {
        void CreateRegionsAround(SnapshotSpan selectionSpan);
    }
}
