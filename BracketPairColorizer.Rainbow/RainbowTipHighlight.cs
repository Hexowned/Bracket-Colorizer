using BracketPairColorizer.Rainbow.Classifications;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(ViewRoles.ToolTipView)]
    public class RainbowTipHighlightProvider : IWpfTextViewCreationListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(RainbowTipHighlight.LAYER)]
        [Order(Before = PredefinedAdornmentLayers.Selection)]
        public AdornmentLayerDefinition HighlightLayer = null;

        [Import]
        public IEditorFormatMapService FormatMapService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            var formatMap = FormatMapService.GetEditorFormatMap(textView);
            textView.Set(new RainbowTipHighlight(textView, formatMap));
        }
    }

    public class RainbowTipHighlight
    {
        public const string LAYER = "BracketPairColorizer.rainbow.tip.highlight";
        public const string TAG = "BracketPairColorizer.rainbow.tip";
        private IWpfTextView textView;
        private IEditorFormatMap formatMap;
        private IAdornmentLayer layer;

        public RainbowTipHighlight(IWpfTextView textView, IEditorFormatMap formatMap)
        {
            this.textView = textView;
            this.formatMap = formatMap;
            this.layer = textView.GetAdornmentLayer(LAYER);
            AddHighlight();
            this.textView.Closed += OnViewClosed;
            this.textView.ViewportWidthChanged += OnViewportSizeChanged;
            this.textView.ViewportHeightChanged += OnViewportSizeChanged;
            this.textView.LayoutChanged += OnLayoutChanged;
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            this.layer.RemoveAllAdornments();
            AddHighlight();
        }

        private void OnViewportSizeChanged(object sender, EventArgs e)
        {
            this.layer.RemoveAllAdornments();
            AddHighlight();
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.textView != null)
            {
                this.textView.Closed -= OnViewClosed;
                this.textView.ViewportWidthChanged -= OnViewportSizeChanged;
                this.textView.ViewportHeightChanged -= OnViewportSizeChanged;
                this.textView = null;
                this.formatMap = null;
                this.layer = null;
            }
        }

        private void AddHighlight()
        {
            ViewTipProperty viewTip = this.textView.Get<ViewTipProperty>();
            if (viewTip == null) { return; }

            SnapshotPoint viewPos;
            if (!RainbowProvider.TryMapToView(this.textView, viewTip.Position, out viewPos)) { return; }

            var line = this.textView.TextViewLines.GetTextViewLineContainingBufferPosition(viewPos);
            if (line == null) { return; }

            var rc = new Rect(new Point(this.textView.ViewportLeft, line.TextTop),
                new Point(Math.Max(this.textView.ViewportRight, line.TextRight), line.TextBottom));

            var properties = this.formatMap.GetProperties(Rainbows.TipHighlight);
            rc = CreateVisual(line.Extent, rc, properties);
        }

        private Rect CreateVisual(SnapshotSpan span, Rect rc, ResourceDictionary properties)
        {
            Rectangle highlight = new Rectangle()
            {
                UseLayoutRounding = true,
                SnapsToDevicePixels = true,
                Fill = new SolidColorBrush((Color)properties["BackgroundColor"]),
                Opacity = 0.10,
                Width = rc.Width,
                Height = rc.Height,
            };

            Canvas.SetLeft(highlight, rc.Left);
            Canvas.SetTop(highlight, rc.Top);

            this.layer = AddAdornment(AdornmentPositioningBehavior.TextRelative, span, TAG, highlight, null);

            return rc;
        }
    }
}
