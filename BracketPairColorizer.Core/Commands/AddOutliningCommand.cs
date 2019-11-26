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
    public class AddOutliningCommand : ITextViewCommandHandler
    {
        public Guid CommandGroup => new Guid(Guids.guidBpcTextEditorCmdSet);
        public int CommandId => PkgCmdIdList.cmdidAddOutlining;

        [Import]
        public IBpcTelemetry Telemetry { get; set; }

        public bool IsEnabled(ITextView view, ref string commandText)
        {
            return TextEditor.SupportsOutlines(view) && !view.Selection.IsEmpty;
        }

        public bool Handle(ITextView view)
        {
            var selection = view.Selection;
            if (selection != null)
            {
                if (selection.Mode == TextSelectionMode.Box) { return false; }

                SnapshotSpan? span = TextEditor.MapSelectionToPrimaryBuffer(selection);
                if (span != null)
                {
                    AddOutlining(span.Value.Snapshot.TextBuffer, span.Value);
                    var oc = OutliningController.Get(view);
                    if (oc != null)
                    {
                        oc.CollapseRegion(span.Value);
                    }

                    return true;
                }
            }

            return false;
        }

        private void AddOutlining(ITextBuffer buffer, SnapshotSpan span)
        {
            var outlines = UserOutliningManager.Get(buffer);
            outlines.Add(span);
            Telemetry.WriteEvent("Add Outlining");
        }
    }
}
