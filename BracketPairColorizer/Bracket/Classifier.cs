#region USING_DIRECTIVES

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion USING_DIRECTIVES

namespace BracketPairColorizer.Bracket
{
    public class Classifier : IClassifier
    {
        private IClassificationTypeRegistryService _registry;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        private static Random random = new Random();

        internal Classifier(IClassificationTypeRegistryService registry)
        {
            _registry = registry;
        }

        #region CLASSIFICATION_SPANS

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;
            List<ClassificationSpan> spans = new List<ClassificationSpan>();

            if (snapshot.Length == 0)
                return spans;

            int _start = 0;
            int _end = snapshot.LineCount - 1;

            List<Tuple<int, int, char>> bracket = new List<Tuple<int, int, char>>();

            for (int i = _start; i < _end; i++)
            {
                ITextSnapshotLine line = snapshot.GetLineFromLineNumber(i);

                string text = line.Snapshot.GetText(new SnapshotSpan(line.Start, line.Length));
                int _open = text.IndexOf("(");
                int _close = text.IndexOf(")");

                while (_open > -1 || _close > -1)
                {
                    if (_open > _close && _close != -1)
                    {
                        bracket.Add(new Tuple<int, int, char>(i, _close, ')'));
                        _open = text.IndexOf("(", _close + 1);
                        _close = text.IndexOf(")", _close + 1);
                    }
                    else if (_open < _close && _open != -1)
                    {
                        bracket.Add(new Tuple<int, int, char>(i, _open, '('));
                        _open = text.IndexOf(")", _open + 1);
                        _close = text.IndexOf("(", _open + 1);
                    }
                    else if (_open == -1 && _close != -1)
                    {
                        bracket.Add(new Tuple<int, int, char>(i, _close, ')'));
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (bracket.Where(s => s.Item3 == '(').ToList().Count != bracket
                       .Where(s => s.Item3 == ')').ToList().Count)
            {
                return spans;
            }

            while (bracket.Count > 0)
            {
                Tuple<int, int, char> line1 = bracket.First();
                Tuple<int, int, char> line2 = null;

                if (line1.Item3 != '(')
                {
                    return spans;
                }

                bracket.Remove(line1);

                int par = 1;
                for (int i = 0; i < bracket.Count; ++i)
                {
                    Tuple<int, int, char> checker = bracket[i];

                    if (checker.Item3 == ')')
                        par--;
                    else if (checker.Item3 == '(')
                        par++;

                    if (par == 0)
                    {
                        line2 = checker;
                        bracket.Remove(line2);
                        break;
                    }
                }

                string _tp = random.Next(1, 9).ToString();

                IClassificationType classificationType = _registry.GetClassificationType(_tp);
                ITextSnapshotLine lineStart = snapshot.GetLineFromLineNumber(line1.Item1);
                ITextSnapshotLine lineEnd = snapshot.GetLineFromLineNumber(line2.Item1);

                SnapshotSpan bracketStart = new SnapshotSpan(span.Snapshot, new Span(lineStart.Start + line1.Item2, 1));
                SnapshotSpan bracketEnd = new SnapshotSpan(span.Snapshot, new Span(lineEnd.Start + line2.Item2, 1));

                if (classificationType != null)
                {
                    spans.Add(new ClassificationSpan(bracketStart, classificationType));
                    spans.Add(new ClassificationSpan(bracketEnd, classificationType));
                }
            }

            return spans;
        }

        #endregion CLASSIFICATION_SPANS
    }
}