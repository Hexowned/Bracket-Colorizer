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
    public class SelectionOutliningCommand : ITextViewCommandHandler
    {
        public Guid CommandGroup => new Guid(Guids.guidBpcTextEditorCmdSet);
        public int CommandId => PkgCmdIdList.cmdidSelectionOutlining;

        [Import]
        public IBpcTelemetry Telemetry { get; set; }

        public bool IsEnabled(ITextView view, ref string commandText)
        {
            if (TextEditor.SupportsOutlines(view))
            {
                if (HasFeatureOutlines(view))
                {
                    commandText = "Remove selection outline";
                    return true;
                } else if (!view.Selection.IsEmpty)
                {
                    commandText = "Outline selection";
                    return true;
                }
            }

            return false;
        }

        public bool Handle(ITextView view)
        {
            if (HasFeatureOutlines(view))
            {
                ClearOutlines(view);
                return true;
            }

            var selection = view.Selection;
            SnapshotSpan? Span = selection.StreamSelectionSpan.SnapshotSpan;
            if (Span.HasValue)
            {
                AddOutlining(Span.Value.Snapshot.TextBuffer, Span.Value);
                CollapseOutlines(selection.TextView);
                Telemetry.WriteEvent("Outline selection");
            }

            return true;
        }

        private void AddOutlining(ITextBuffer buffer, SnapshotSpan span)
        {
            var outlines = SelectionOutliningManager.Get(buffer);
            if (outlines != null)
            {
                outlines.CreateRegionsAround(span);
            }
        }

        private void CollapseOutlines(ITextView textView)
        {
            var controller = OutliningController.Get(textView);
            if (controller != null)
            {
                controller.CollapseSelectionRegions();
            }
        }

        private void ClearOutlines(ITextView view)
        {
            var controller = OutliningController.Get(view);
            if (controller != null)
            {
                controller.RemoveSelectionRegions();
            }
        }

        private bool HasFeatureOutlines(ITextView view)
        {
            var outlines = SelectionOutliningManager.Get(view.TextBuffer);

            return outlines != null && outlines.HasUserOutlines();
        }
    }
}
