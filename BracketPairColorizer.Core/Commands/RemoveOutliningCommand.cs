using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Outlining;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Commands
{
    [Export(typeof(ITextViewCommandHandler))]
    public class RemoveOutliningCommand : ITextViewCommandHandler
    {
        public Guid CommandGroup => new Guid(Guids.guidBpcTextEditorCmdSet);
        public int CommandId => PkgCmdIdList.cmdidRemoveOutlining;

        [Import]
        public IBpcTelemetry Telemetry { get; set; }

        public bool IsEnable(ITextView view, ref string commandText)
        {
            var caret = view.Caret;
            if (caret == null)
                return false;

            var point = TextEditor.MapCaretToPrimaryBuffer(view);
            if (point != null)
            {
                var outlining = UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);

                return outlining.IsInOutliningRegion(point.Value);
            }

            return false;
        }

        public bool Handle(ITextView view)
        {
            var caret = view.Caret;
            if (caret == null)
                return false;

            var point = TextEditor.MapCaretToPrimaryBuffer(view);
            if (point != null)
            {
                var outlining = UserOutliningManager.Get(point.Value.Snapshot.TextBuffer);
                outlining.RemoveAt(point.Value);
                Telemetry.WriteEvent("Remove Outlining");
            }

            return true;
        }
    }
}
