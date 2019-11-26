using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Outlining;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Commands
{
    [Export(typeof(ITextViewCommandHandler))]
    public class ClearOutliningCommand : ITextViewCommandHandler
    {
        public Guid CommandGroup => new Guid(Guids.guidBpcTextEditorCmdSet);
        public int CommandId => PkgCmdIdList.cmdidClearOutlining;

        [Import]
        public IBpcTelemetry Telemetry { get; set; }

        public bool IsEnabled(ITextView view, ref string commandText)
        {
            var outlining = GetOutlining(view, out var buffer);

            return outlining != null && outlining.HasUserOutlines();
        }

        public bool Handle(ITextView view)
        {
            var outlining = GetOutlining(view, out var buffer);
            outlining?.RemoveAll(buffer.CurrentSnapshot);
            Telemetry.WriteEvent("Clear outlining");

            return true;
        }

        private IUserOutlining GetOutlining(ITextView view, out ITextBuffer buffer)
        {
            buffer = TextEditor.GetPrimaryBuffer(view);

            return UserOutliningManager.Get(buffer);
        }
    }
}
