using BracketPairColorizer.Core.Contracts;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace BracketPairColorizer.Rainbow.Utilities
{
    [Export(typeof(IToolTipWindowProvider))]
    public class ToolTipWindowProvider : IToolTipWindowProvider
    {
        [Import]
        public ITextEditorFactoryService EditorFactory { get; set; }

        [Import]
        public IEditorOptionsFactoryService OptionsFactory { get; set; }

        [Import]
        public IVsFeatures VsFeatures { get; set; }

        public IToolTipWindow CreateToolTip(ITextView textView)
        {
            return new ToolTipWindow(textView, this);
        }
    }

    public sealed class ToolTipWindow : IToolTipWindow
    {
        private ITextView sourceTextView;
        private IToolTipWindowProvider provider;
        private IWpfTextView tipView;
        private int linesDisplayed;
        private SnapshotPoint pointToDisplay;
        private Border wrapper;
        private const double ZoomFactor = 0.80;
        private const double WidthFactor = 0.60;

        public ToolTipWindow(ITextView source, IToolTipWindowProvider provider)
        {
            this.sourceTextView = source;
            this.provider = provider;
        }

        public void SetSize(int widthCharacters, int heightCharacters)
        {
            if (this.tipView == null)
            {
                CreateTipView();
            }

            double zoom = (this.tipView.ZoomLevel / 100.0);
            double sourceZoom = this.GetSourceZoomFactor();
            double width = Math.Max(sourceZoom * WidthFactor * this.sourceTextView.ViewportWidth,
                zoom * widthCharacters * this.tipView.FormattedLineSource.ColumnWidth);
            double height = zoom * heightCharacters * this.tipView.LineHeight;
            this.wrapper.Width = width;
            this.wrapper.Height = height;
            this.wrapper.BorderThickness = new Thickness(0);
            this.linesDisplayed = heightCharacters;
        }

        public object GetWindow(SnapshotPoint bufferPosition)
        {
            if (this.tipView == null)
            {
                CreateTipView();
            }

            this.pointToDisplay = bufferPosition;
            var viewTipProp = this.tipView.Get<ViewTipProperty>();
            viewTipProp.Position = bufferPosition;

            return this.wrapper;
        }

        public void Dispose()
        {
            ReleaseView();
            this.sourceTextView = null;
        }

        private void OnViewportWidthChanged(object sender, EventArgs e)
        {
            this.tipView.ViewportWidthChanged -= this.OnViewportWidthChanged;
            if (this.tipView.ViewportRight > this.tipView.ViewportLeft)
            {
                this.ScrollIntoView(this.pointToDisplay);
            }
        }

        private void ScrollIntoView(SnapshotPoint bufferPosition)
        {
        }

        private void SetViewportLeft()
        {
        }

        private SnapshotPoint? FindFirstNonWhiteSpaceCharacter(ITextViewLine line)
        {
            SnapshotSpan span = line.Extent;
            for (SnapshotPoint i = span.Start; i < span.End; i += 1)
            {
                if (!char.IsWhiteSpace(i.GetChar())) { return i; }
            }

            return null;
        }

        private void CreateTipView()
        {
        }

        private double GetSourceZoomFactor()
        {
            IWpfTextView wpfSource = this.sourceTextView as IWpfTextView;
            if (wpfSource != null)
            {
                return wpfSource.ZoomLevel / 100;
            } else { return 1.0; }
        }

        private void ReleaseView()
        {
            if (this.tipView != null)
            {
                this.tipView.ViewportWidthChanged -= this.OnViewportWidthChanged;
                this.wrapper.Child = null;
                try
                {
                    this.tipView.Close();
                } catch
                {
                    // swallow
                }

                this.tipView = null;
            }
        }

        // TextViewModel for the ToolTip window
        private class TipTextViewModel : ITextViewModel
        {
            public ITextBuffer DataBuffer { get; private set; }
            public ITextBuffer EditBuffer { get; private set; }
            public ITextBuffer VisualBuffer => this.EditBuffer;
            public ITextDataModel DataModel { get; private set; }
            public PropertyCollection Properties { get; private set; }

            public TipTextViewModel(ITextView source)
            {
                this.DataBuffer = source.TextViewModel.DataBuffer;
                this.EditBuffer = source.TextViewModel.EditBuffer;
                this.DataModel = source.TextViewModel.DataModel;
                this.Properties = new PropertyCollection();
            }

            public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
            {
                if (editBufferPoint.Snapshot.TextBuffer != this.EditBuffer)
                    throw new InvalidOperationException("editBufferPoint is not on the edit buffer");

                return editBufferPoint.TranslateTo(this.EditBuffer.CurrentSnapshot, PointTrackingMode.Positive);
            }

            public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
            {
                if (editBufferPoint.Snapshot.TextBuffer != this.EditBuffer)
                    throw new InvalidOperationException("editBufferPoint is not on the edit buffer");

                return editBufferPoint.TranslateTo(targetVisualSnapshot, PointTrackingMode.Positive);
            }

            public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
            {
                return editBufferPoint.Snapshot.TextBuffer == this.EditBuffer;
            }

            public void Dispose()
            {
            }
        }
    }
}
