﻿using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace BracketPairColorizer.Core.Commands
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("viasfora.command.handler")]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class TextViewCommandListener : IBpcTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService { get; set; }

        [ImportMany]
        public List<ITextViewCommandHandler> CommandHandlers { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(()
                => new TextViewCommandHandler(this, textViewAdapter, textView));
        }

        public ITextViewCommandHandler FindHandler(Guid cmdGroup, int cmdId)
        {
            foreach (var cmd in this.CommandHandlers)
            {
                if (cmd.CommandGroup == cmdGroup && cmd.CommandId == cmdId)
                {
                    return cmd;
                }
            }

            return null;
        }
    }

    public class TextViewCommandHandler : IOleCommandTarget
    {
        private TextViewCommandListener provider;
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;

        public TextViewCommandHandler(TextViewCommandListener provider, IVsTextView viewAdapter, ITextView textView)
        {
            this.provider = provider;
            this.textView = textView;
            viewAdapter.AddCommandFilter(this, out this.nextCommandHandler);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            var cmdId = (int)prgCmds[0].cmdID;
            var handler = this.provider.FindHandler(pguidCmdGroup, cmdId);

            if (handler != null)
            {
                string commandText = "";
                bool isEnabled = handler.IsEnabled(this.textView, ref commandText);
                if (isEnabled)
                {
                    prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;

                    if (!string.IsNullOrEmpty(commandText))
                    {
                        SetOleCmdText(pCmdText, commandText);
                    }

                    return VSConstants.S_OK;
                }
            }

            return this.nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public void SetOleCmdText(IntPtr pCmdText, string text)
        {
            var CmdText = (OLECMDTEXT)Marshal.PtrToStructure(pCmdText, typeof(OLECMDTEXT));
            char[] buffer = text.ToCharArray();
            var pText = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "rgwz"));
            var pCwActual = (IntPtr)((long)pCmdText + (long)Marshal.OffsetOf(typeof(OLECMDTEXT), "cwActual"));
            // The max chars we copy is our string, or one less than the buffer size, since we need a null at the end.
            int maxChars = (int)Math.Min(CmdText.cwBuf - 1, buffer.Length);
            Marshal.Copy(buffer, 0, pText, maxChars);
            // append a null
            Marshal.WriteInt16((IntPtr)((long)pText + (long)maxChars * 2), (Int16)0);
            // write out the length + null char
            Marshal.WriteInt32(pCwActual, maxChars + 1);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            int hr = VSConstants.S_OK;
            var cmdId = (int)nCmdID;
            var handler = this.provider.FindHandler(pguidCmdGroup, cmdId);
            bool handled = false;

            if (handler != null)
            {
                handled = handler.Handle(this.textView);
            }

            if (!handled)
            {
                // Let other commands handle it
                hr = this.nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }

            return hr;
        }
    }
}
