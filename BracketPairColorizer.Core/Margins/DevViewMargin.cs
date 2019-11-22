using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Linq;
using System.Windows;

namespace BracketPairColorizer.Core.Margins
{
    public sealed class DevViewMargin : IWpfTextViewMargin
    {
        private IFileExtensionRegistryService extensionRegistry;
        private IWpfTextViewHost wpfTextViewHost;
        private IWpfTextView textView;
        private IVsfSettings settings;
        private DevMarginVisual visual;
        private DevMarginViewModel model;

        public DevViewMargin(IWpfTextViewHost wpfTextViewHost, IFileExtensionRegistryService fers, IVsfSettings settings)
        {
            this.model = new DevMarginViewModel();
            this.wpfTextViewHost = wpfTextViewHost;
            this.extensionRegistry = fers;
            this.settings = settings;
            this.settings.SettingsChanged += OnSettingsChanged;
            this.wpfTextViewHost.Closed += OnTextViewHostClosed();

            this.visual = new DevMarginVisual(this.model, settings);
            this.visual.ViewBuffer += OnViewBuffer;

            UpdateVisibility();
            InitializeTextView();
            RefreshBufferGraphList();
            this.model.RefreshView(wpfTextViewHost.TextView);
        }

        public FrameworkElement VisualElement => this.visual;

        public bool Enabled => this.settings?.DeveloperMarginEnabled ?? false;

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (marginName == Constants.DEV_MARGIN)
                return this;
            return null;
        }

        public double MarginSize => this.visual.ActualHeight + 2;

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (this.wpfTextViewHost != null)
            {
                this.wpfTextViewHost.Closed -= OnTextViewHostClosed;
                this.wpfTextViewHost = null;
            }

            if (this.textView != null)
            {
                this.textView.BufferGraph.GraphBuffersChanged -= OnGraphBuffersChanged;
                this.textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphBufferContentTypeChanged;
                this.textView.Caret.PositionChanged -= OnCaretPositionChanged;
                this.textView.TextViewModel.EditBuffer.PostChanged -= OnBufferPostChanged;
                this.textView = null;
            }

            if (this.visual != null)
            {
                this.visual.ViewBuffer -= OnViewBuffer;
                this.visual = null;
            }

            this.extensionRegistry = null;
        }

        private void InitializeTextView()
        {
            this.textView = this.wpfTextViewHost.TextView;
            this.textView.BufferGraph.GraphBuffersChanged += OnGraphBuffersChanged;
            this.textView.BufferGraph.GraphBufferContentTypeChanged += OnGraphBufferContentTypeChanged;
            this.textView.Caret.PositionChanged += OnCaretPositionChanged;
            this.textView.TextViewModel.EditBuffer.PostChanged += OnBufferPostChanged;
        }

        private void OnTextViewHostClosed(object sender, EventArgs e)
        {
            Cleanup();
        }

        private void OnBufferPostChanged(object sender, EventArgs e)
        {
            UpdateCaretPosition(this.textView.Caret.Position);
        }

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateCaretPosition(e.NewPosition);
        }

        private void OnGraphBufferContentTypeChanged(object sender, GraphBufferContentTypeChangedEventArgs e)
        {
            RefreshBufferGraphList();
        }

        private void OnGraphBuffersChanged(object sender, GraphBuffersChangedEventArgs e)
        {
            RefreshBufferGraphList();
        }

        private void OnViewBuffer(object sender, EventArgs e)
        {
            var buffer = GetSelectedBuffer();
            if (buffer != null)
            {
                OpenBufferInEditor(buffer);
            }
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (this.visual != null)
            {
                UpdateVisibility();
            }
        }

        private void UpdateCaretPosition(CaretPosition caret)
        {
            ITextBuffer currentBuffer = GetSelectedBuffer();
            if (currentBuffer == null) { return; }

            SnapshotPoint? bufferPos = null;

            if (currentBuffer == caret.BufferPosition.Snapshot.TextBuffer)
            {
                bufferPos = caret.BufferPosition;
            } else
            {
                bufferPos = this.textView.BufferGraph.MapDownToBuffer(caret.BufferPosition,
                    PointTrackingMode.Negative, currentBuffer, PositionAffinity.Predecessor);

                if (!bufferPos.HasValue)
                {
                    bufferPos = this.textView.BufferGraph.MapUpToBuffer(caret.BufferPosition,
                        PointTrackingMode.Negative, PositionAffinity.Predecessor, currentBuffer);
                }
            }

            if (bufferPos.HasValue)
            {
                this.model.BufferPosition = bufferPos.Value.Position.ToString();
            } else
            {
                this.model.BufferPosition = "--";
            }
        }

        private void UpdateVisibility()
        {
            this.visual.Visibility = this.Enabled
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private ITextBuffer GetSelectedBuffer()
        {
            if (this.textView == null || this.textView.BufferGraph == null)
                return null;
            if (this.model.SelectedBuffer == null)
                return null;
            var buffers = this.textView.BufferGraph.GetTextBuffers(b => true);
            int selectedIndex = this.model.SelectedBuffer.Index;
            foreach (var b in buffers)
            {
                if (selectedIndex == 0)
                    return b;
                selectedIndex--;
            }

            return null;
        }

        private void OpenBufferInEditor(ITextBuffer buffer)
        {
            try
            {
                string extension = this.extensionRegistry.GetExtensionsForContentType(buffer.ContentType).FirstOrDefault();
                if (string.IsNullOrEmpty(extension))
                {
                    TextEditor.OpenBufferInPlainTextEditorAsReadOnly(buffer);
                } else
                {
                    TextEditor.OpenBufferInEditorAsReadOnly(buffer, extension);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "[BPC] Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshBufferGraphList()
        {
            this.model.RefreshBuffers(this.textView.BufferGraph);
        }
    }
}
