using BracketPairColorizer.Core.Compatibility;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.IO;
using VsOle = Microsoft.VisualStudio.OLE.Interop;

namespace BracketPairColorizer.Core.Settings
{
    public static class TextEditor
    {
        public static ITextCaret GetCurrentCaret()
        {
            var view = GetCurrentView();

            return view?.Caret;
        }

        public static ITextSelection GetCurrentSelection()
        {
            var view = GetCurrentView();

            if (view == null) { return null; }
            if (view.Selection.IsEmpty)
                return null;

            return view.Selection;
        }

        public static ITextView GetCurrentView()
        {
            var textManager = (IVsTextManager)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));
            int hr = textManager.GetActiveView(1, null, out var textView);

            if (hr != Constants.S_OK || textView == null)
                return null;

            var componentModel = new SComponentModel();
            var factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            return factory.GetWpfTextView(textView);
        }

        public static bool SupportsOutlines(ITextView view)
        {
            var componentModel = new SComponentModel();
            var outliningService = componentModel.GetService<IOutliningManagerService>();

            if (outliningService == null) { return false; }

            var outliningManager = outliningService.GetOutliningManager(view);

            return outliningService != null && outliningManager.Enabled;
        }

        public static string GetFileName(ITextBuffer buffer)
        {
            if (buffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer adapter))
            {
                if (adapter is IPersistFileFormat pff)
                {
                    string fileName;
                    try
                    {
                        int hr = pff.GetCurFile(out fileName, out uint formatIndex);
                        if (hr == Constants.E_NOTIMPL)
                            return null;
                        CheckError(hr, "GetCurFile");
                    } catch (NotImplementedException)
                    {
                        fileName = null;
                    }

                    return fileName;
                }
            }

            return null;
        }

        public static SnapshotSpan? MapSelectionToPrimaryBuffer(ITextSelection selection)
        {
            var span = selection.StreamSelectionSpan.SnapshotSpan;
            var view = selection.TextView;
            var buffer = GetPrimaryBuffer(view);
            var locations = view.BufferGraph.MapDownToBuffer(span, SpanTrackingMode.EdgeInclusive, buffer);

            if (locations.Count > 0)
            {
                span = new SnapshotSpan(locations[0].Start, locations[locations.Count - 1].End);
            }

            return span;
        }

        public static SnapshotPoint? MapCaretToPrimaryBuffer(ITextView view)
        {
            var buffer = GetPrimaryBuffer(view);
            var caret = view.Caret;
            var point = view.BufferGraph.MapDownToBuffer(caret.Position.BufferPosition, PointTrackingMode.Negative, buffer, PositionAffinity.Predecessor);

            return point;
        }

        public static ITextBuffer GetPrimaryBuffer(ITextView view)
        {
            var buffers = view.BufferGraph.GetTextBuffers((x) => !x.ContentType.IsOfType("projection"));
            if (buffers.Count <= 0)
            {
                return view.BufferGraph.TopBuffer;
            }

            return buffers[0];
        }

        public static bool IsNonProjectOrElisionBufferType(Type type)
        {
            if (typeof(IProjectionBuffer).IsAssignableFrom(type))
                return false;
            if (typeof(IElisionBuffer).IsAssignableFrom(type))
                return false;

            return true;
        }

        public static bool IsNonProjectionOrElisionBuffer(ITextBuffer buffer)
        {
            if ((buffer as IProjectionBuffer) != null)
                return false;
            if ((buffer as IElisionBuffer) != null)
                return false;

            return true;
        }

        public static void DisplayMessageInStatusBar(string message)
        {
            var bar = (IVsStatusbar)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsStatusbar));
            if (bar != null)
            {
                bar.SetText(message);
            }
        }

        public static void OpenBufferInPlainTextEditorAsReadOnly(ITextBuffer buffer)
        {
            OpenBufferInEditorAsReadOnly(buffer, "txt");
        }

        public static void OpenBufferInEditorAsReadOnly(ITextBuffer buffer, string extension)
        {
            string filePath = SaveBufferToTempPath(buffer, extension);

            var uiShell = (IVsUIShellOpenDocument)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShellOpenDocument));
            var oleSvcProvider = (VsOle.IServiceProvider)
                ServiceProvider.GlobalProvider.GetService(typeof(VsOle.IServiceProvider));

            var editorType = VSConstants.VsEditorFactoryGuid.TextEditor_guid;
            var logicalView = VSConstants.LOGVIEWID.TextView_guid;
            var frame = VsShellUtilities.OpenDocumentWithSpecificEditor(ServiceProvider.GlobalProvider, filePath, editorType, logicalView);

            if (frame != null)
            {
                MarkDocumentAsTemporary(filePath);
                MarkDocumentInFrameAsReadOnly(frame);
                frame.Show();
            }
        }

        private static void MarkDocumentInFrameAsReadOnly(IVsWindowFrame frame)
        {
            var textView = VsShellUtilities.GetTextView(frame);
            if (textView.GetBuffer(out var textLines) == Constants.S_OK)
            {
                var vsBuffer = textLines as IVsTextBuffer;
                vsBuffer.SetStateFlags((uint)(BUFFERSTATEFLAGS.BSF_USER_READONLY | BUFFERSTATEFLAGS.BSF_FILESYS_READONLY));
            }
        }

        private static void MarkDocumentAsTemporary(string moniker)
        {
            var docTable = (IVsRunningDocumentTable)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsRunningDocumentTable));

            uint lockType = (uint)_VSRDTFLAGS.RDT_DontAddToMRU
                          | (uint)_VSRDTFLAGS.RDT_NonCreatable
                          | (uint)_VSRDTFLAGS.RDT_VirtualDocument
                          | (uint)_VSRDTFLAGS.RDT_PlaceHolderDoc;

            int hr = docTable.FindAndLockDocument(
                dwRDTLockType: lockType,
                pszMkDocument: moniker,
                ppHier: out var hierarchy,
                pitemid: out uint itemid,
                ppunkDocData: out var docData,
                pdwCookie: out uint documentCookie
                );
            CheckError(hr, "FindAndLockDocument");
            docTable.ModifyDocumentFlags(documentCookie, lockType, 1);
        }

        private static string SaveBufferToTempPath(ITextBuffer buffer, string extension)
        {
            string tempDirectory = Path.GetTempPath();
            string file = Path.Combine(tempDirectory, Path.GetRandomFileName());
            file += "." + extension;
            File.WriteAllText(file, buffer.CurrentSnapshot.GetText());

            return file;
        }

        private static void CheckError(int hr, string operation)
        {
            if (hr != Constants.S_OK)
            {
                var ex = new InvalidOperationException(string.Format("{0} returned 0x{1:x8}", operation, hr));
                PkgSource.LogError(operation, ex);
                throw ex;
            }
        }
    }
}
