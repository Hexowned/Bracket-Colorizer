using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using IVsOutliningManager = Microsoft.VisualStudio.Text.Outlining.IOutliningManager;
using IVsOutliningManagerService = Microsoft.VisualStudio.Text.Outlining.IOutliningManagerService;

namespace BracketPairColorizer.Core.Outlining
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Structured)]
    [Name("BracketPairColorizer.outlining.controller")]
    [ContentType(ContentTypes.Any)]
    [ContentType(ContentTypes.Projection)]
    public class OutliningControllerListener : IWpfTextViewCreationListener
    {
        [Import]
        private IVsOutliningManagerService outlining = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            var manager = this.outlining.GetOutliningManager(textView);
            if (manager != null)
            {
                textView.Properties.GetOrCreateSingletonProperty(()
                    => new OutliningController(textView, manager) as OutliningController);
            }
        }
    }

    public class OutliningController : IOutliningController
    {
        private ITextView theView;
        private IVsOutliningManager outliningManager;
        private Dispatcher currentDispatcher;
        private DispatcherTimer timer;

        public OutliningController(ITextView view, IVsOutliningManager manager)
        {
            this.theView = view;
            this.outliningManager = manager;
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
            this.timer = new DispatcherTimer(DispatcherPriority.Background, this.currentDispatcher);
            this.timer.Tick += OnTimerTick;
            this.timer.Interval = TimeSpan.FromMilliseconds(50);

            this.theView.Closed += OnViewClosed;
        }

        public static IOutliningController Get(ITextView view)
        {
            return view.Get<IOutliningController>();
        }

        public void CollapseSelectionRegions()
        {
            var buffer = this.theView.TextBuffer;
            var outlining = SelectionOutliningManager.Get(this.theView.TextBuffer);
            var allDoc = buffer.CurrentSnapshot.GetSpan();

            var regions = outlining.GetTags(new NormalizedSnapshotSpanCollection(allDoc));
            this.theView.LayoutChanged += OnTextViewLayoutChanged;
            foreach (var regionSpan in regions)
            {
                TryCollapseRegion(regionSpan);
            }
        }

        public void RemoveSelectionRegions()
        {
            var buffer = this.theView.TextBuffer;
            var outlining = SelectionOutliningManager.Get(this.theView.TextBuffer);

            this.theView.LayoutChanged += OnTextViewLayoutChanged;
            outlining.RemoveAll(buffer.CurrentSnapshot);
        }

        public void CollapseRegion(SnapshotSpan span)
        {
            if (this.theView.TextBuffer == span.Snapshot.TextBuffer)
            {
                TryCollapseRegion(span);
                return;
            }
            var mappedSpans = this.theView.BufferGraph.MapUpToBuffer(
              span, SpanTrackingMode.EdgePositive,
              this.theView.TextBuffer
            );
            foreach (var ms in mappedSpans)
            {
                TryCollapseRegion(ms);
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.theView != null)
            {
                this.theView.Closed -= OnViewClosed;
                this.theView.LayoutChanged -= OnTextViewLayoutChanged;
                this.theView = null;
            }
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer = null;
            }
            this.outliningManager = null;
            this.currentDispatcher = null;
        }

        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (this.theView != null)
            {
                this.theView.LayoutChanged -= OnTextViewLayoutChanged;
                this.timer.Start();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var view = this.theView;
            var timer = this.timer;
            if (view == null || view.InLayout)
            {
                return;
            }
            timer.Stop();
            var selection = view.Selection;
            if (selection != null)
            {
                view.ViewScroller.EnsureSpanVisible(selection.StreamSelectionSpan.SnapshotSpan, EnsureSpanVisibleOptions.AlwaysCenter);
            } else
            {
                var caretPos = view.Caret.Position.BufferPosition;
                var caretLine = view.GetTextViewLineContainingBufferPosition(caretPos);
                view.ViewScroller.EnsureSpanVisible(caretLine.Extent, EnsureSpanVisibleOptions.AlwaysCenter);
            }
        }

        private void TryCollapseRegion(SnapshotSpan regionSpan)
        {
            var collapsible = this.outliningManager.GetAllRegions(regionSpan);
            foreach (var c in collapsible)
            {
                if (c.Extent.GetSpan(regionSpan.Snapshot) == regionSpan)
                {
                    var result = this.outliningManager.TryCollapse(c);
                    if (result == null || !result.IsCollapsed)
                    {
                        this.outliningManager.TryCollapse(c);
                    }
                }
            }
        }
    }
}
