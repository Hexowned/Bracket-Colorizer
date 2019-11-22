using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(ViewRoles.EmbeddedPeekTextView)]
    public class RainbowLinesProvider : IWpfTextViewConnectionListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(RainbowLines.LAYER)]
        [Order(After = PredefinedAdornmentLayers.Text)]
        public AdornmentLayerDefinition LinesLayer = null;

        [Import]
        public IClassificationFormatMapService FormatMapService { get; set; }

        [Import]
        public IClassificationTypeRegistryService RegistryService { get; set; }

        [Import]
        public IRainbowSettings Settings { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            textView.Set(new RainbowLines(textView, this));
        }

        public IClassificationType[] GetRainbowTags()
        {
            return RainbowColorTagger.GetRainbows(RegistryService, Rainbows.MaxDepth);
        }

        public IClassificationFormatMap GetFormatMap(ITextView textView)
        {
            return FormatMapService.GetClassificationFormatMap(textView);
        }

        public int GetRainbowDepth()
        {
            return this.Settings.RainbowDepth;
        }
    }

    public class RainbowLines
    {
        public const String LAYER = "BracketPairColorizer.rainbow.lines";
        public const String TAG = "BracketPairColorizer.rainbow";
        private const double LINE_WIDTH = 1.5;
        private IWpfTextView view;
        private Brush[] rainbowColors;
        private IClassificationFormatMap formatMap;
        private SnapshotSpan currentSpan;
        private readonly IAdornmentLayer layer;
        private readonly RainbowLinesProvider provider;
        private readonly LinePoint[] slbuffer;
        private readonly Dispatcher dispatcher;

        public RainbowLines(IWpfTextView textView, RainbowLinesProvider provider)
        {
            this.view = textView;
            this.provider = provider;
            this.formatMap = provider.GetFormatMap(textView);
            this.rainbowColors = GetRainbowColors(provider.GetRainbowTags());
            this.layer = textView.GetAdornmentLayer(LAYER);
            this.dispatcher = Dispatcher.CurrentDispatcher;

            this.provider.Settings.SettingsChanged += OnSettingsChanged;
            this.view.Caret.PositionChanged += OnCaretPositionChanged;
            this.view.Options.OptionChanged += OnOptionsChanged;
            this.view.LayoutChanged += OnLayoutChanged;
            this.view.ViewportLeftChanged += OnViewportLeftChanged;
            this.view.ViewportWidthChanged += OnViewportWidthChanged;
            this.view.ViewportHeightChanged += OnViewportHeightChanged;
            this.view.Closed += OnViewClosed;

            this.formatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
            this.slbuffer = new LinePoint[2];
        }

        private Brush[] GetRainbowColors(IClassificationType[] rainbows)
        {
            Brush[] brushes = new Brush[rainbows.Length];
            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i] = this.formatMap.GetTextProperties(rainbows[i]).ForegroundBrush;
            }

            return brushes;
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            this.provider.Settings.SettingsChanged -= OnSettingsChanged;
            if (this.view != null)
            {
            }

            if (this.formatMap != null)
            {
            }
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
        }

        private void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
        }

        private void OnViewportHeightChanged(object sender, EventArgs e)
        {
        }

        private void OnViewportWidthChanged(object sender, EventArgs e)
        {
        }

        private void OnViewportLeftChanged(object sender, EventArgs e)
        {
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
        }

        private SnapshotPoint GetPosition(SnapshotPoint position)
        {
            return position.Snapshot == this.view.TextSnapshot
                ? position
                : position.TranslateTo(this.view.TextSnapshot, PointTrackingMode.Positive);
        }

        private void RedrawVisuals(SnapshotPoint caret, bool forceRedraw)
        {
            if (!this.provider.Settings.RainbowLinesEnabled)
            {
                this.layer.RemoveAllAdornments();
                return;
            }

            if (!RainbowProvider.TryMapPosToBuffer(this.view, caret, out caret))
            {
                this.layer.RemoveAllAdornments();
                return;
            }

            var provider = caret.Snapshot.TextBuffer.Get<RainbowProvider>();
            var braces = provider?.BufferBraces.GetBracePairFromPosition(caret, RainbowHighlightMode.TrackInsertionPoint);
            if (braces == null)
            {
                this.currentSpan = default(SnapshotSpan);
                this.layer.RemoveAllAdornments();
                return;
            }

            SnapshotPoint opening = braces.Item1.ToPoint(caret.Snapshot);
            SnapshotPoint closing = braces.Item2.ToPoint(caret.Snapshot);

            if (RainbowProvider.TryMapToView(this.view, opening, out opening) & RainbowProvider.TryMapToView(this.view, closing, out closing))
            {
                var newSpan = new SnapshotSpan(opening, closing);

                if (this.currentSpan == newSpan && !forceRedraw) { return; } // don't redraw

                this.currentSpan = default(SnapshotSpan);
                var path = CreateVisuals(opening, closing, caret);
                this.layer.RemoveAllAdornments();
                if (path != null)
                {
                    var adornment = MakeAdornment(path, braces.Item1.Depth);
                    this.layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, newSpan, TAG, adornment, null);
                    this.currentSpan = newSpan;
                }
            }
        }

        private UIElement MakeAdornment(Geometry spanGeometry, int depth)
        {
            if (spanGeometry.CanFreeze)
            {
                spanGeometry.Freeze();
            }

            return new Path()
            {
                Data = spanGeometry,
                Stroke = GetRainbowBrush(depth),
                StrokeThickness = LINE_WIDTH,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                SnapsToDevicePixels = true
            };
        }

        private Brush GetRainbowBrush(int depth)
        {
            return this.rainbowColors[depth % this.provider.GetRainbowDepth()];
        }

        private Geometry CreateVisuals(SnapshotPoint opening, SnapshotPoint closing, SnapshotPoint caret)
        {
            var openLine = this.view.TextViewLines.GetTextViewLineContainingBufferPosition(opening);
            var closeLine = this.view.TextViewLines.GetTextViewLineContainingBufferPosition(closing);

            IList<LinePoint> points = null;
            if (openLine != null && closeLine != null && openLine.Extent == closeLine.Extent)
            {
                points = SingleLineSpan(opening, closing);
            } else
            {
                points = MultiLineSpan(opening, closing);
            }

            if (points.Count == 0) { return null; }

            var geometry = new PathGeometry();
            var p = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                var p1 = points[i];
                if (!p.SkipNext)
                {
                    geometry.AddGeometry(new LineGeometry(p, p1));
                }

                p = p1;
            }

            return geometry;
        }

        private IList<LinePoint> MultiLineSpan(SnapshotPoint opening, SnapshotPoint closing)
        {
            var indent = CalculateLeftOfFirstChar(opening, this.view.FormattedLineSource);
            var lines = this.view.TextViewLines.GetTextViewLinesIntersectingSpan(new SnapshotSpan(opening, closing));

            bool useViewportRight = false;
            var guidelineX = (indent + ((this.view.FormattedLineSource.ColumnWidth) / 2)) + LINE_WIDTH;
            if (guidelineX < this.view.ViewportLeft)
            {
                guidelineX = this.view.ViewportRight - 2;
                useViewportRight = true;
            }

            var points = new List<LinePoint>();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (i == 0 && line.ContainsBufferPosition(opening))
                {
                    var openBounds = line.GetCharacterBounds(opening);
                    if (openBounds.Left > guidelineX)
                    {
                        points.Add(new LinePoint(openBounds.Right, line.Bottom));
                    } else if (useViewportRight)
                    {
                        points.Add(new LinePoint(openBounds.Left, line.Bottom));
                    }

                    points.Add(new LinePoint(guidelineX, line.Bottom));
                } else
                {
                    var xPos = line.GetBufferPositionFromXCoordinate(guidelineX);
                    if (xPos.HasValue && !char.IsWhiteSpace(xPos.Value.GetChar()))
                    {
                        points.Add(new LinePoint(guidelineX, line.Top, true));
                    }

                    points.Add(new LinePoint(guidelineX, line.Bottom));
                    if (i == lines.Count - 1 && line.ContainsBufferPosition(closing))
                    {
                        var closeBounds = line.GetCharacterBounds(closing);
                        if (closeBounds.Right < guidelineX || closeBounds.Left > guidelineX)
                        {
                            points.Add(new LinePoint(closeBounds.Right, line.Bottom));
                        }
                    }
                }
            }

            return points;
        }

        private double CalculateLeftOfFirstChar(SnapshotPoint open, IFormattedLineSource fls)
        {
            var line = open.GetContainingLine();
            var x = 0d;
            var start = line.Start;
            int spacesSinceLastTab = 0;
            while (char.IsWhiteSpace(start.GetChar()))
            {
                char ch = start.GetChar();
                if (ch == ' ')
                {
                    spacesSinceLastTab = ++spacesSinceLastTab % fls.TabSize;
                    x += fls.ColumnWidth;
                } else if (ch == '\t')
                {
                    x += ((fls.TabSize - spacesSinceLastTab) * fls.ColumnWidth);
                    spacesSinceLastTab = 0;
                }
                start = start + 1;
            }

            return x;
        }

        private IList<LinePoint> SingleLineSpan(SnapshotPoint opening, SnapshotPoint closing)
        {
            var startb = this.view.TextViewLines.GetCharacterBounds(opening);
            var endb = this.view.TextViewLines.GetCharacterBounds(closing);

            this.slbuffer[0] = new LinePoint(startb.Left, startb.Bottom);
            this.slbuffer[1] = new LinePoint(endb.Right, endb.Bottom);

            return this.slbuffer;
        }

        private struct LinePoint
        {
            public readonly double X;
            public readonly double Y;
            public readonly bool SkipNext;

            public LinePoint(double x, double y, bool skipNext = false)
            {
                this.X = x;
                this.Y = y;
                this.SkipNext = skipNext;
            }

            public static implicit operator Point(LinePoint p)
                => new Point(p.X, p.Y);
        }
    }
}
