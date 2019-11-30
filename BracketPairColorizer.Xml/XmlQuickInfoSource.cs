using BracketPairColorizer.Core.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace BracketPairColorizer.Xml
{
    internal class XmlQuickInfoSource : IQuickInfoSource
    {
        private ITextBuffer textBuffer;
        private XmlQuickInfoSourceProvider provider;

        public XmlQuickInfoSource(ITextBuffer buffer, XmlQuickInfoSourceProvider provider)
        {
            this.textBuffer = textBuffer;
            this.provider = provider;
        }

        public void Dispose()
        {
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);

            if (!subjectTriggerPoint.HasValue)
            {
                return;
            }

            var currentSnapshot = subjectTriggerPoint.Value.Snapshot;
            var querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);
            var tagAggregator = GetAggregator(session);
            var extent = FindExtentAtPoint(subjectTriggerPoint);

            if (CheckForPrefixTag(tagAggregator, extent.Span))
            {
                string prefix = extent.Span.GetText();
                string url = FindNSUri(extent.Span, GetDocText(extent.Span));

                applicableToSpan = currentSnapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
                quickInfoContent.Add(CreateInfoText(prefix, url));
            }
        }

        private UIElement CreateInfoText(string xmlns, string url)
        {
            var textBlock = new TextBlock();
            var hl = new Hyperlink(new Run(url));
            textBlock.Inlines.AddRange(new Inline[]
            {
                new Bold(new Run("Prefix: ")),
                new Run(xmlns),
                new LineBreak(),
                new Bold(new Run("Namespace: ")),
                hl
            });

            textBlock.Background = Brushes.Transparent;
            textBlock.SetResourceReference(TextBlock.ForegroundProperty, VsColors.ToolTipTextBrushKey);
            hl.SetResourceReference(Hyperlink.ForegroundProperty, VsColors.PanelHyperlinkBrushKey);

            return textBlock;
        }

        private TextExtent FindExtentAtPoint(SnapshotSpan? subjectTriggerPoint)
        {
            var navigator = this.provider.NavigatorService.GetTextStructureNavigator(this.textBuffer);
            var extent = navigator.GetExtentOfWord(subjectTriggerPoint.Value);

            return extent;
        }

        private string FindNSUri(SnapshotSpan span, string docText)
        {
            string subtext = FindMinTextToParse(span, docText);
            using (var sr = new StringReader(subtext))
            {
                var settings = new XmlReaderSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                using (var reader = XmlReader.Create(sr, settings))
                {
                    string thisPrefix = span.GetText();
                    string lastUriForPrefix = ReadXmlUntilEnd(reader, thisPrefix);

                    return string.IsNullOrEmpty(lastUriForPrefix) ? "unknown" : lastUriForPrefix;
                }
            }
        }

        private static string ReadXmlUntilEnd(XmlReader reader, string thisPrefix)
        {
            string lastUriForPrefix = null;
            try
            {
                while (reader.Read())
                {
                    if (reader.Prefix == thisPrefix)
                    {
                        lastUriForPrefix = reader.NamespaceURI;
                    } else if (reader.NodeType == XmlNodeType.Element)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Prefix == thisPrefix)
                            {
                                lastUriForPrefix = reader.NamespaceURI;
                            }
                        }
                    }
                }
            } catch
            {
            }

            return lastUriForPrefix;
        }

        private static string FindMinTextToParse(SnapshotSpan span, string docText)
        {
            string subText = docText;
            int endElement = docText.IndexOf('>', span.Span.End);
            if (endElement > 0 && endElement < docText.Length - 1)
            {
                subText = docText.Substring(0, endElement + 1);
            }

            return subText;
        }

        private string GetDocText(SnapshotSpan span)
        {
            return span.Snapshot.GetText();
        }

        private bool CheckForPrefixTag(ITagAggregator<IClassificationTag> tagAggregator, SnapshotSpan span)
        {
            string text = span.GetText();
            if (text.StartsWith("<") || text.Contains(":"))
            {
                return false;
            }

            var firstMatch = from tagSpan in tagAggregator.GetTags(span)
                             let tagName = tagSpan.Tag.ClassificationType.Classification
                             where tagName == XmlConstants.XML_PREFIX || tagName == XmlConstants.XML_CLOSING_PREFIX
                             select tagSpan;

            return firstMatch.FirstOrDefault() != null;
        }

        private ITagAggregator<IClassificationTag> GetAggregator(IQuickInfoSession session)
        {
            return this.provider.AggregatorFactory.CreateTagAggregator<IClassificationTag>(session.TextView);
        }
    }
}
