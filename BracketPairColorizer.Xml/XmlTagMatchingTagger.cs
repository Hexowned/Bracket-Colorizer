using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Sgml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace BracketPairColorizer.Xml
{
    public sealed class XmlTagMatchingTagger : ITagger<TextMarkerTag>, IDisposable
    {
        private ITextView theView;
        private ITextBuffer theBuffer;
        private IXmlSettings settings;
        private SnapshotSpan? currentSpan;
        private ITagAggregator<IClassificationTag> aggregator;
        private IMarkupLanguage language;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public XmlTagMatchingTagger(ITextView textView, ITextBuffer buffer, ITagAggregator<IClassificationTag> aggregator, IXmlSettings settings)
        {
            this.theView = textView;
            this.theBuffer = buffer;
            this.aggregator = aggregator;
            this.settings = settings;
            this.currentSpan = null;
            this.language = new XmlMarkup();

            this.theView.Closed += OnViewClosed;
            this.theView.Caret.PositionChanged += CaretPositionChanged;
            this.theView.LayoutChanged += ViewLayoutChanged;
            this.settings.SettingsChanged += OnSettingsChanged;
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (!this.settings.XmlMatchTagsEnabled)
                yield break;
            if (spans.Count == 0)
                yield break;
            if (!this.currentSpan.HasValue)
                yield break;

            var current = this.currentSpan.Value;
            if (current.Snapshot != spans[0].Snapshot)
            {
                current = current.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgePositive);
            }

            var currentTag = CompleteTag(current);
            string text = currentTag.GetText();
            if (text.Contains('?'))
                yield break;

            SnapshotSpan? complementTag = null;
            if (text.StartsWith("</"))
            {
                complementTag = FindOpeningTag(current.Snapshot, currentTag.End, current.GetText());
                if (complementTag != null)
                {
                    complementTag = ExtendOpeningTag(complementTag.Value);
                }
            } else
            {
                string searchFor = "</" + current.GetText() + ">";
                currentTag = ExtendOpeningTag(currentTag);
                complementTag = FindClosingTag(current.Snapshot, currentTag.Start, searchFor);
            }

            var defaultTag = new TextMarkerTag("bracehighlight");
            var alternateTag = new TextMarkerTag("other error");
            if (complementTag.HasValue)
            {
                yield return new TagSpan<TextMarkerTag>(currentTag, defaultTag);
                yield return new TagSpan<TextMarkerTag>(complementTag.Value, defaultTag);
            } else
            {
                yield return new TagSpan<TextMarkerTag>(currentTag, currentTag.GetText().EndsWith("/>") ? defaultTag : alternateTag);
            }
        }

        private SnapshotSpan ExtendOpeningTag(SnapshotSpan currentTag)
        {
            var snapshot = currentTag.Snapshot;
            int end = -1;
            string currentQuote = null;

            for (int i = currentTag.Start; i < snapshot.Length; i++)
            {
                string ch = snapshot.GetText(i, 1);
                if (currentQuote == null)
                {
                    if (ch == "\"" || ch == "'")
                    {
                        currentQuote = ch;
                    } else if (ch == ">")
                    {
                        end = i;
                        break;
                    }
                } else if (ch == currentQuote)
                {
                    currentQuote = null;
                }
            }

            if (end > currentTag.Start)
            {
                return new SnapshotSpan(snapshot, currentTag.Start, end - currentTag.Start + 1);
            }

            return currentTag;
        }

        private SnapshotSpan? FindClosingTag(ITextSnapshot snapshot, int searchStart, string searchFor)
        {
            string textToSearch = snapshot.GetText(searchStart, snapshot.Length - searchStart);

            using (var reader = new SgmlReader())
            {
                reader.InputStream = new StringReader(textToSearch);
                reader.WhitespaceHandling = WhitespaceHandling.All;
                try
                {
                    reader.Read();
                    if (!reader.IsEmptyElement)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == 1)
                                break;
                        }

                        var originalLine = snapshot.GetLineFromPosition(searchStart);
                        int startOffset = searchStart - originalLine.Start.Position;
                        int newStart = 0;

                        if (reader.LineNumber == 1)
                        {
                            var line = snapshot.GetLineFromPosition(searchStart);
                            newStart = line.Start.Position + startOffset + reader.LinePosition - 2;
                        } else
                        {
                            int newLineNumber = originalLine.LineNumber + reader.LineNumber - 1;
                            var newLine = snapshot.GetLineFromLineNumber(newLineNumber);
                            newStart = newLine.Start.Position + reader.LinePosition - 1;
                        }

                        newStart -= reader.Name.Length + 3;
                        SnapshotSpan? newSpan = new SnapshotSpan(snapshot, newStart, searchFor.Length);
                        if (newSpan.Value.GetText() != searchFor)
                        {
                            Trace.WriteLine(string.Format("Searching for '{0}', but found '{1}'.", searchFor, newSpan.Value.GetText()));
                            newSpan = null;
                        }

                        return newSpan;
                    }
                } catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Exception while parsing document: {0}.", ex.ToString()));
                }
            }

            return null;
        }

        private SnapshotSpan? FindOpeningTag(ITextSnapshot snapshot, int searchEnd, string searchFor)
        {
            string textToSearch = snapshot.GetText(0, searchEnd);
            int originalLineNumber = snapshot.GetLineNumberFromPosition(searchEnd);
            using (var reader = new SgmlReader())
            {
                reader.InputStream = new StringReader(textToSearch);
                reader.WhitespaceHandling = WhitespaceHandling.All;
                try
                {
                    var openingPositions = new Stack<int>();
                    while (reader.Read())
                    {
                        if (reader.LocalName != searchFor)
                        {
                            continue;
                        }

                        if (reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement)
                        {
                            int lineNumber = reader.LineNumber - 1;
                            var line = snapshot.GetLineFromLineNumber(lineNumber);
                            int position = line.Start.Position + reader.LinePosition - searchFor.Length;

                            position = BacktrackToLessThan(snapshot, position);
                            string textFound = snapshot.GetText(position, 10);
                            openingPositions.Push(position);
                        } else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (openingPositions.Count <= 0)
                            {
                                return null;
                            }

                            var line = snapshot.GetLineFromLineNumber(reader.LineNumber - 1);
                            int position = line.Start.Position + reader.LinePosition;
                            if (position >= searchEnd)
                                break;

                            openingPositions.Pop();
                        }
                    }

                    if (openingPositions.Count > 0)
                    {
                        int position = openingPositions.Pop();

                        return new SnapshotSpan(snapshot, position, searchFor.Length + 2);
                    }
                } catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Exception while parsing document: {0}.", ex.ToString()));
                }
            }

            return null;
        }

        private int BacktrackToLessThan(ITextSnapshot snapshot, int start)
        {
            int rs = start - 1;

            while (snapshot.GetText(rs, 1) != "</")
            {
                rs--;
            }

            return rs;
        }

        private SnapshotSpan CompleteTag(SnapshotSpan current)
        {
            var snapshot = current.Snapshot;
            int end = current.End < snapshot.Length ? current.End + 1 : current.End;
            int start = BacktrackToLessThan(snapshot, current.Start);

            return new SnapshotSpan(snapshot, start, end - start);
        }

        public void Dispose()
        {
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.theView != null)
            {
                this.theView.Closed -= OnViewClosed;
                this.theView.Caret.PositionChanged -= CaretPositionChanged;
                this.theView.LayoutChanged -= ViewLayoutChanged;
                this.theView = null;
            }

            if (this.settings != null)
            {
                this.settings.SettingsChanged -= OnSettingsChanged;
                this.settings = null;
            }

            if (this.aggregator != null)
            {
                this.aggregator.Dispose();
                this.aggregator = null;
            }

            this.theBuffer = null;
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (this.theView != null)
            {
                UpdateAtCaretPosition(this.theView.Caret.Position);
            }
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot)
            {
                UpdateAtCaretPosition(this.theView.Caret.Position);
            }
        }

        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        private void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            var point = caretPosition.Point.GetPoint(this.theBuffer, caretPosition.Affinity);
            if (!point.HasValue)
                return;

            this.currentSpan = GetTagAtPoint(point.Value);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(this.theBuffer.CurrentSnapshot.GetSpan()));
        }

        private SnapshotSpan? GetTagAtPoint(SnapshotPoint point)
        {
            int pos = point.Position >= 1 ? point.Position - 1 : 0;
            var testSpan = new SnapshotSpan(point.Snapshot, new Span(pos, 0));

            foreach (var tagSpan in this.aggregator.GetTags(testSpan))
            {
                string tagName = tagSpan.Tag.ClassificationType.Classification;
                if (!this.language.IsName(tagName))
                    continue;

                foreach (var span in tagSpan.Span.GetSpans(point.Snapshot.TextBuffer))
                {
                    if (span.Contains(point.Position))
                    {
                        return span;
                    }
                }
            }

            return null;
        }
    }
}
