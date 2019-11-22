using BracketPairColorizer.Core.Utilities;
using BracketPairColorizer.Languages;
using BracketPairColorizer.Languages.BraceScanners;
using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BracketPairColorizer.Rainbow
{
    public class TextBufferBraces : ITextBufferBraces
    {
        private List<BracePosition> braces;
        private List<CharPosition> braceErrors;
        private SortedList<char, char> braceList;
        private IBraceScanner braceScanner;
        private ILanguage language;
        private RainbowColoringMode coloringMode;
        public ITextSnapshot Snapshot { get; private set; }
        public string BraceChars { get; private set; }
        public int LastParsedPosition { get; private set; }
        public bool Enabled => this.language?.Settings.Enabled ?? false;

        public TextBufferBraces(ITextSnapshot snapshot, ILanguage language, RainbowColoringMode coloringMode)
        {
            this.Snapshop = snapshot;
            this.LastParsedPosition = -1;
            this.language = language;
            this.coloringMode = new SortedList<char, char>();
            this.braces = new List<BracePosition>();
            this.braceErrors = new List<CharPosition>();

            if (this.language != null)
            {
                this.braceScanner = this.language.GetService<IBraceScanner>();
                this.braceScanner.Clear();
                this.BraceChars = this.braceScanner.BraceList;
                for (int i = 0; i < BraceChars.Length; i += 2)
                {
                    this.braceList.Add(BraceChars[i], BraceChars[i + 1]);
                }
            }
        }

        public void Invalidate(SnapshotPoint startPoint)
        {
            if (ScanIsUnnecessary()) return;

            var newSnapshot = startPoint.Snapshot;
            this.Snapshot = newSnapshot;

            int index = FindIndexOfBraceBefore(startPoint.Position);
            index = AdjustForInvalidation(index);
            if (index >= 0)
            {
                InvalidateFromBraceAtIndex(newSnapshot, index + 1);
            } else
            {
                InvalidateFromBraceAtIndex(newSnapshot, 0);
            }

            this.InvalidateBraceErrorsFromPos(startPoint.Position);
        }

        private int AdjustForInvalidation(int index)
        {
            int newIndex = index;
            IResumeControl control = this.braceScanner as IResumeControl;
            if (control != null)
            {
                for (; newIndex > 0; newIndex--)
                {
                    if (control.CanResume(this.braces[newIndex].ToCharPos()))
                        break;
                }
            }

            if (newIndex > 0 && newIndex != index)
                newIndex--;

            return newIndex;
        }

        public void UpdateSnapshot(ITextSnapshot snapshot)
        {
            this.Snapshot = snapshot;
        }

        public IEnumerable<BracePosition> BracesInSpans(NormalizedSnapshotSpanCollection spans)
        {
            if (ScanIsUnnecessary()) yield break;

            for (int i = 0; i < spans.Count; i++)
            {
                var wantedSpan = new TextSpan(spans[i].Start, spans[i].Length);
                EnsureLinesInPreferredSpan(wantedSpan);
                int startIndex = FindIndexOfBraceAtOrAfter(wantedSpan.Start);
                if (startIndex < 0) { continue; }

                for (int j = startIndex; j < this.braces.Count; j++)
                {
                    BracePosition bp = this.braces[j];
                    if (bp.Position > wantedSpan.End) break;
                    yield return bp;
                }
            }
        }

        public IEnumerable<CharPosition> ErrorBracesInSpans(NormalizedSnapshotSpanCollection spans)
        {
            if (ScanIsUnnecessary())
                return Enumerable.Empty<CharPosition>();

            var fullSpan = spans.Complete();
            var wantedSpan = new TextSpan(fullSpan.Start, fullSpan.Length);
            EnsureLinesInPreferredSpan(wantedSpan);
            if (this.braceErrors.Count == 0)
                return Enumerable.Empty<CharPosition>();

            return from e in this.braceErrors
                   from span in spans
                   where e.Position >= span.Start
                   && e.Position <= span.End
                   select e;
        }

        public IEnumerable<BracePosition> BracesFromPosition(int position)
        {
            if (ScanIsUnnecessary()) return new BracePosition[0];
            SnapshotSpan snan = new SnapshotSpan(Snapshot, position, Snapshot.Length - position);

            return BracesInSpans(new NormalizedSnapshotSpanCollection(span));
        }

        public Tuple<BracePosition, BracePosition> GetBracePair(SnapshotPoint point)
        {
            if (point.Snapshot != this.Snapshot || point.Position >= Snapshot.Length) { return null; }

            var span = point.SpanUntil();
            this.EnsureLinesInPreferredSpan(new TextSpan(span.Start, span.Length));

            int index = FindIndexOfBraceAtOrAfter(point.Position);
            if (index < 0) return null;
            BracePosition one = this.braces[index];
            if (one.Position != point.Position) { return null; }

            if (IsOpeningBrace(one.Brace))
            {
                return GetBracePairFromPosition(point, RainbowHighlightMode.TrackNextScope);
            } else
            {
                return GetBracePairFromPosition(point, RainbowHighlightMode.TrackInsertionPoint);
            }
        }

        public Tuple<BracePosition, BracePosition> GetBracePairFromPosition(SnapshotPoint point, RainbowHighlightMode mode)
        {
            if (point.Snapshot != this.Snapshot || point.Position >= Snapshot.Length) { return null; }

            var span = point.SpanUntil();
            this.EnsureLinesInPreferredSpan(new TextSpan(span.Start, span.Length));

            int openIndex = -1;
            BracePosition? opening = null;

            if (mode == RainbowHighlightMode.TrackInsertionPoint)
            {
                opening = FindClosestOpeningBrace(point.Position, out openIndex);
            } else
            {
                opening = CheckForBraceAtPositionOrClosestOpeningBrace(point.Position, out openIndex);
            }

            if (opening == null) { return null; }

            for (int i = openIndex + 1; i < this.braces.Count; i++)
            {
                if (i == this.braces.Count - 1)
                {
                    this.ContinueParsing(this.LastParsedPosition, this.Snapshot.Length);
                }

                var closing = this.braces[i];
                if (this.IsOpeningBrace(closing.Brace))
                    continue;
                if (this.braceList[opening.Value.Brace] == closing.Brace && closing.Depth == opening.Value.Depth)
                {
                    return new Tuple<BracePosition, BracePosition>(opening.Value, closing);
                }
            }

            return null;
        }

        private BracePosition? CheckForBraceAtPositionOrClosestOpeningBrace(int position, out int openIndex)
        {
            openIndex = FindIndexOfBraceAtOrAfter(position);
            if (openIndex >= 0)
            {
                BracePosition pos = this.braces[openIndex];
                if (IsOpeningBrace(pos.Brace) && pos.Position == position) { return pos; }
            }

            return FindClosestOpeningBrace(position, out openIndex);
        }

        private BracePosition? FindClosestOpeningBrace(int position, out int openIndex)
        {
            openIndex = FindIndexOfBraceBefore(position);
            if (openIndex < 0) { return null; }

            int pairs = 0;
            while (openIndex >= 0)
            {
                BracePosition current = this.braces[openIndex];
                if (!IsOpeningBrace(current.Brace))
                {
                    pairs++;
                } else if (pairs == 0)
                {
                    return current;
                } else { pairs--; }

                openIndex--;
            }

            return null;
        }

        private void EnsureLinesInPreferredSpan(TextSpan span)
        {
            var snapshot = this.Snapshot;
            int minSpanLen = Math.Max(100, (int)(snapshot.Length * 0.10));
            var realSpan = span;
            int lastPosition = this.LastParsedPosition;

            if (lastPosition > 0 && lastPosition >= span.End) { return; }

            int parseFrom = lastPosition + 1;
            int parseUntil = Math.Min(snapshot.Length, Math.Max(span.End, parseFrom + minSpanLen));

            ContinueParsing(parseFrom, parseUntil);
        }

        private void ContinueParsing(int parseFrom, int parseUntil)
        {
            int startPosition = 0;
            int lastGoodBrace = 0;
            int lastState = 0;

            var pairs = GetStacker(this.coloringMode);
            for (int i = 0; i < this.braces.Count; i++)
            {
                BracePosition r = this.braces[i];
                if (r.Position > parseFrom) break;
                if (IsOpeningBrace(r.Brace))
                {
                    pairs.Push(r.ToCharPos());
                } else if (pairs.Count(r.Brace) > 0)
                {
                    pairs.Pop(r.Brace);
                }

                startPosition = r.Position + 1;
                lastGoodBrace = i;
                lastState = r.State;
            }

            if (lastGoodBrace < this.braces.Count - 1)
            {
                this.braces.RemoveRange(lastGoodBrace + 1, this.braces.Count - lastGoodBrace - 1);
            }

            ExtractBraces(pairs, startPosition, parseUntil, lastState);
        }

        private void ExtractBraces(IBraceScanner pairs, int startOffset, int endOffset, int state)
        {
            this.braceScanner.Reset(state);
            int lineNumber = Snapshot.GetLineNumberFromPosition(startOffset);
            while (lineNumber < Snapshot.LineCount)
            {
                var line = Snapshot.GetLineFromLineNumber(lineNumber++);
                var lineOffset = startOffset > 0 ? startOffset - line.Start : 0;
                if (line.Length != 0)
                {
                    ExtractFromLine(pairs, line, lineOffset);
                }

                startOffset = 0;
                this.LastParsedPosition = line.End;
                if (line.End >= endOffset)
                    break;
            }
        }

        private void ExtractFromLine(IBraceStacker pairs, ITextSnapshotLine line, int lineOffset)
        {
            var lc = new LineCharacters(line, lineOffset);
            var pos = CharPosition.Empty;
            while (!lc.AtEnd)
            {
                if (!this.braceScanner.Extract(lc, ref pos))
                    continue;
                MatchBrace(pairs, pos);
            }

            this.LastParsedPosition = line.End;
        }

        private void MatchBrace(IBraceStacker pairs, CharPosition pos)
        {
            if (IsOpeningBrace(pos))
            {
                Add(pairs.Push(pos));
            } else if (pairs.Count(pos.Char) > 0)
            {
                var p = pairs.Peek(pos.Char);
                if (this.braceList[p.Brace] == pos.Char)
                {
                    pairs.Pop(pos.Char);
                    Add(new BracePosition(pos, p.Depth));
                } else
                {
                    this.braceErrors.Add(pos);
                }
            } else
            {
                this.braceErrors.Add(pos);
            }
        }

        private void Add(BracePosition brace)
        {
            this.braces.Add(brace);
            LastParsedPosition = brace.Position;
        }

        private IBraceStacker GetStacker(RainbowColoringMode mode)
        {
            switch (mode)
            {
                case RainbowColoringMode.Unified:
                    return new UnifiedBraceStacker();

                case RainbowColoringMode.PerBrace:
                    return new PerBraceStacker(this.BraceChars);

                default:
                    throw new InvalidOperationException("Invalid rainbow coloring mode");
            }
        }

        private int FindIndexOfBraceAtOrAfter(int position)
        {
            int first = 0;
            int last = this.braces.Count - 1;
            int candidate = -1;
            while (first <= last)
            {
                int mid = (first + last) / 2;
                BracePosition midPos = this.braces[mid];
                if (midPos.Position < position)
                {
                    first = mid + 1;
                } else if (midPos.Position > position)
                {
                    candidate = mid;
                    last = mid - 1;
                } else
                {
                    candidate = mid;
                    break;
                }
            }

            return candidate;
        }

        private int FindIndexOfBraceBefore(int position)
        {
            int first = 0;
            int last = this.braces.Count - 1;
            int candidate = -1;
            while (first <= last)
            {
                int mid = (first + last) / 2;
                BracePosition midPos = this.braces[mid];
                if (midPos.Position < position)
                {
                    candidate = mid;
                    first = mid + 1;
                } else if (midPos.Position > position)
                {
                    last = mid - 1;
                } else
                {
                    candidate = mid - 1;
                    break;
                }
            }

            return candidate;
        }

        private void InvalidateFromBraceAtIndex(ITextSnapshot snapshot, int index)
        {
            if (index < this.braces.Count)
            {
                this.braces.RemoveRange(index, this.braces.Count - index);
            }

            if (this.braces.Count > 0)
            {
                this.LastParsedPosition = this.braces[this.braces.Count - 1].Position;
            } else
            {
                this.LastParsedPosition = -1;
            }
        }

        private void InvalidateFromErrorsFromPos(int position)
        {
            int lastPos = -1;
            for (int i = 0; i < this.braceErrors.Count; i++)
            {
                lastPos = i;
                CharPosition ch = this.braceErrors[i];
                if (ch.Position >= position) break;
            }

            if (lastPos >= 0 && lastPos < this.braceErrors.Count)
            {
                this.braceErrors.RemoveRange(lastPos, this.braceErrors.Count - lastPos);
            }
        }

        private bool IsOpeningBrace(char ch)
        {
            var keys = this.braceList.Keys;
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] == ch) return true;
            }

            return false;
        }

        private bool ScanIsUnnecessary()
        {
            return this.language == null
                || string.IsNullOrEmpty(this.BraceChars);
        }
    }
}
