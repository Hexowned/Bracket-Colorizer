using Microsoft.VisualStudio.Text.Editor;
using System;

namespace BracketPairColorizer.Core.Commands
{
    public interface ITextViewCommandHandler
    {
        Guid CommandGroup { get; }
        int CommandId { get; }

        bool IsEnabled(ITextView view, ref string commandText);

        bool Handle(ITextView view);
    }
}
