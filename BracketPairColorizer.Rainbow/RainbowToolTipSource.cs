using BracketPairColorizer.Rainbow.Settings;
using BracketPairColorizer.Rainbow.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace BracketPairColorizer.Rainbow
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("BracketPairColorizer.rainbow.tooltip.source")]
    [ContentType(ContentTypes.Text)]
    public class RainbowToolTipSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        public IToolTipWindowProvider ToolTipProvider { get; set; }

        [Import]
        public IRainbowSettings Settings { get; set; }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new RainbowToolTipSource(textBuffer, this);
        }
    }

    public sealed class RainbowToolTipSource : IQuickInfoSource
    {
        private ITextBuffer textBuffer;
        private RainbowToolTipSourceProvider provider;
        private IToolTipWindow toolTipWindow;

        public RainbowToolTipSource(ITextBuffer textBuffer, RainbowToolTipSourceProvider provider)
        {
            this.textBuffer = textBuffer;
            this.provider = provider;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            if (!this.provider.Settings.RainbowToolTipsEnabled) { return; }

            SnapshotPoint? triggerPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) { return; }

            SnapshotPoint? otherBrace;
            if (!FindOtherBrace(triggerPoint.Value, out otherBrace)) { return; }

            if (!otherBrace.HasValue)
            {
                TextEditor.DisplayMessageInStatusBar("No matching brace found.");
                return;
            }

            if (this.toolTipWindow != null)
            {
                this.toolTipWindow.Dispose();
                this.toolTipWindow = null;
            }

            session.Dismiss += OnSessionDismissed;

            if (this.toolTipWindow == null)
            {
                this.toolTipWindow = this.provider.ToolTipProvider.CreateToolTip(session.TextView);
                this.toolTipWindow.SetSize(60, 5);
            }

            var span = new SnapshotSpan(triggerPoint.Value, 1);
            applicableToSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgePositive);

            var element = this.toolTipWindow.GetWindow(otherBrace.Value);
            if (element != null)
            {
                quickInfoContent.Add(element);
                session.Set(new RainbowToolTipContext());
            }
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            var session = (IQuickInfoSession)sender;
            session.Dismissed -= OnSessionDismissed;
            if (this.toolTipWindow != null)
            {
                this.toolTipWindow.Dispose();
                this.toolTipWindow = null;
            }
        }

        private bool IsTooClose(SnapshotPoint point1, SnapshotPoint point2)
        {
            int distance = Math.Abs(point1 - point2);

            return distance < 100;
        }

        private bool FindOtherBrace(SnapshotPoint possibleBrace, out SnapshotPoint? otherBrace)
        {
            otherBrace = null;
            var rainbow = this.textBuffer.Get<RainbowProvider>();
            if (rainbow == null) { return false; }
            if (!possibleBrace.IsValid()) { return false; }
            if (!rainbow.BufferBraces.BraceChars.Contains(possibleBrace.GetChar())) { return false; }

            var bracePair = rainbow.BufferBraces.GetBracePair(possibleBrace);
            if (bracePair == null) { return true; }
            if (possibleBrace.Position == bracePair.Item1.Position)
            {
                otherBrace = bracePair.Item2.ToPoint(possibleBrace.Snapshot);
            } else
            {
                otherBrace = bracePair.Item1.ToPoint(possibleBrace.Snapshot);
            }

            return true;
        }

        public void Dispose()
        {
        }
    }
}
