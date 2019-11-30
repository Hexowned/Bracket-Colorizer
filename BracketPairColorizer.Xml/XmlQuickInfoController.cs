using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;

namespace BracketPairColorizer.Xml
{
    internal class XmlQuickInfoController : IIntellisenseController
    {
        private ITextView textView;
        private IQuickInfoSession session;
        private readonly IList<ITextBuffer> textBuffers;
        private readonly XmlQuickInfoControllerProvider provider;

        internal XmlQuickInfoController(ITextView textView, IList<ITextBuffer> textBuffers, XmlQuickInfoControllerProvider provider)
        {
            this.textView = textView;
            this.textBuffers = textBuffers;
            this.provider = provider;

            textView.MouseHover += OnTextViewMouseHover;
        }

        public void Detach(ITextView view)
        {
            if (this.textView == textView)
            {
                textView.MouseHover -= this.OnTextViewMouseHover;
                textView = null;
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            this.textBuffers.Add(subjectBuffer);
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
            this.textBuffers.Remove(subjectBuffer);
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            SnapshotPoint? point = this.textView.BufferGraph.MapDownToFirstMatch(
                new SnapshotPoint(this.textView.TextSnapshot, e.Position),
                PointTrackingMode.Positive,
                snapshot => this.textBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor
            );

            if (point != null)
            {
                var triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);

                if (this.provider.QuickInfoBroker.IsQuickInfoActive(this.textView))
                {
                    this.session = this.provider.QuickInfoBroker.TriggerQuickInfo(this.textView, triggerPoint, true);
                }
            }
        }
    }
}
