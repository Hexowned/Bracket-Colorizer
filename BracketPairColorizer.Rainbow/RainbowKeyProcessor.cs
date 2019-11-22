using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("BracketPairColorizer.rainbow.key.provider")]
    [Order(After = "Default")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(ContentTypes.Text)]
    public class RainbowKeyProcessorProvider : IKeyProcessorProvider
    {
        [Import]
        public IRainbowSettings Settings { get; set; }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new RainbowKeyProcessor(wpfTextView, Settings);
        }
    }

    public class RainbowKeyProcessor : KeyProcessor
    {
        private readonly ITextView theView;
        private readonly IRainbowSettings settings;
        private Stopwatch timer = new Stopwatch();
        private bool startedEffect = false;
        private TimeSpan pressTime;

        public RainbowKeyProcessor(ITextView textView, IRainbowSettings settings)
        {
            this.theView = textView;
            this.theView.LostAggregateFocus += OnLostFocus;
            this.theView.Closed += OnViewClosed;
            this.settings = settings;
            this.pressTime = TimeSpan.FromMilliseconds(settings.RainbowCtrlTimer);
        }

        public override void PreviewKeyDown(KeyEventArgs args)
        {
            var actualView = GetViewFromEvent(args);
            if (args.Key == (Key)(this.settings.RainbowHighlightKey))
            {
                if (this.timer.IsRunning)
                {
                    if (this.timer.Elapsed >= this.pressTime)
                    {
                        this.timer.Stop();
                        var mode = this.settings.RainbowHighlightMode;
                        StartRainbowHighlight(actualView, mode);
                    }
                } else
                {
                    this.timer.Start();
                }
            } else
            {
                this.timer.Stop();
            }
        }

        private ITextView GetViewFromEvent(KeyEventArgs args)
        {
            var view = args.OriginalSource as ITextView;

            return view ?? this.theView;
        }

        public override void PreviewKeyUp(KeyEventArgs args)
        {
            var actualView = GetViewFromEvent(args);
            this.timer.Stop();
            StopRainbowHighlight(actualView);
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            this.theView.LostAggregateFocus -= OnLostFocus;
            this.theView.Closed -= OnViewClosed;
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            this.timer.Stop();
            StopRainbowHighlight(this.theView);
        }

        private void StartRainbowHighlight(ITextView view, RainbowHighlightMode mode)
        {
            if (this.startedEffect) return;
            this.startedEffect = true;

            if (!RainbowProvider.TryMapCaretToBuffer(view, out SnapshotPoint bufferPos)) { return; }

            var buffer = bufferPos.Snapshot.TextBuffer;
            var provider = buffer.Get<RainbowProvider>();
            if (provider == null) { return; }

            var braces = provider.BufferBraces.GetBracePairFromPosition(bufferPos, mode);
            if (braces == null) return;
            SnapshotPoint opening = braces.Item1.ToPoint(bufferPos.Snapshot);
            SnapshotPoint closing = braces.Item2.ToPoint(bufferPos.Snapshot);

            if (RainbowProvider.TryMapToView(view, opening, out opening) && RainbowHighlightProvider.TryMapToView(view, closing, out closing))
            {
                var highlight = RainbowHighlight.Get(view);
                if (highlight != null)
                {
                    highlight.Start(opening, closing, braces.Item1.Depth);
                }
            }
        }

        private void StopRainbowHighlight(ITextView view)
        {
            var highlight = RainbowHighlight.Get(view);
            if (highlight != null) { highlight.Stop(); }

            this.startedEffect = false;
        }
    }
}
