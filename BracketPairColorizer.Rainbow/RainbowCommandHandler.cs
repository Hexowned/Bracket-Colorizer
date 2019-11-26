using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("BracketPairColorizer.rainbow.handler")]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class RainbowCommandHandler : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        [Import]
        internal IEditorOperationsFactoryService EditorOperationsFactory { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(() => new RainbowCommandHandler(this, textViewAdapter, textView));
        }
    }

    public class RainbowCommandHandler : IOleCommandTarget
    {
        private RainbowCommandHandlerProvider provider;
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;

        public RainbowCommandHandler(RainbowCommandHandlerProvider provider, IVsTextView viewAdapter, ITextView textView)
        {
            this.provider = provider;
            this.textView = textView;
            viewAdapter.AddCommandFilter(this, out this.nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            bool enabled = false;
            if (pguidCmdGroup == Guids.BpcTextEditorCmdSet)
            {
                var cmdId = (int)prgCmds[0].cmdID;
                switch (cmdId)
                {
                    // TODO:
                }
            }

            if (enabled)
            {
                prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;

                return VSConstants.S_OK;
            }

            return this.nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            var mode = RainbowHighlightMode.TrackInsertionPoint;
            int hr = VSConstants.S_OK;
            bool handled = false;
            if (pguidCmdGroup == Guids.BpcTextEditorCmdSet)
            {
                switch ((int)nCmdID)
                {
                    // TODO:
                }
            }

            if (!handled)
            {
                hr = this.nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            return hr;
        }

        private bool MoveRainbow(bool previous, RainbowHighlightMode mode)
        {
            SnapshotPoint bufferPos;
            if (!RainbowProvider.TryMapCaretToBuffer(this.textView, out bufferPos)) { return false; }

            ITextBuffer buffer = bufferPos.Snapshot.TextBuffer;
            RainbowProvider provider = buffer.Get<RainbowProvider>();
            if (provider == null) { return false; }

            var braces = provider.BufferBraces.GetBracePairFromPosition(bufferPos, mode);
            if (braces == null) { return false; }

            if (previous && braces.Item1.Position == bufferPos.Position - 1)
            {
                braces = provider.BufferBraces.GetBracePairFromPosition(bufferPos - 1, mode);
            } else if (!previous && braces.Item2.Position == bufferPos.Position)
            {
                braces = provider.BufferBraces.GetBracePairFromPosition(bufferPos + 1, mode);
            }

            if (braces == null) { return false; }

            if (previous)
            {
                SnapshotPoint opening = braces.Item1.ToPoint(bufferPos.Snapshot);
                MoveCaretTo(opening + 1);
            } else
            {
                SnapshotPoint closing = braces.Item2.ToPoint(bufferPos.Snapshot);
                MoveCaretTo(closing);
            }

            return true;
        }

        private void MoveCaretTo(SnapshotPoint position)
        {
            SnapshotPoint viewPos;
            if (RainbowProvider.TryMapToView(this.textView, position, out viewPos))
            {
                this.textView.Caret.MoveTo(viewPos);
                var span = new SnapshotPoint(viewPos, 0);
                this.textView.ViewScroller.EnsureSpanVisible(span);
            }
        }
    }
}
