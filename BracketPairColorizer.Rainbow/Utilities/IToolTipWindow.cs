using Microsoft.VisualStudio.Text;
using System;

namespace BracketPairColorizer.Rainbow.Utilities
{
    public interface IToolTipWindow : IDisposable
    {
        void SetSize(int widthCharacters, int heightCharacters);

        object GetWindow(SnapshotPoint bufferPosition);
    }
}
