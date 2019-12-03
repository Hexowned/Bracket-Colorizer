#region USING_DIRECTIVES

using BracketPairColorizer.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion USING_DIRECTIVES

namespace BracketPairColorizer.Bracket
{
    public class Tagger : ITagger<IClassificationTag>
    {
        private readonly ITextBuffer _buffer;
        private RoslynDocument cache;
        private readonly IClassificationTypeRegistryService _registry;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private static readonly Random Random = new Random();

        internal Tagger(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;
            _registry = registry;
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                return Enumerable.Empty<ITagSpan<IClassificationTag>>();
            }

            if (cache == null || cache.Snapshot != spans[0].Snapshot)
            {
                var task = RoslynDocument.Resolve(_buffer, spans[0].Snapshot);
                task.Wait();

                if (task.IsFaulted)
                {
                    return Enumerable.Empty<ITagSpan<IClassificationTag>>();
                }

                cache = task.Result;
            }

            return FindBracketSpans(cache, spans);
        }

        public IEnumerable<ITagSpan<IClassificationTag>> FindBracketSpans(RoslynDocument doc, NormalizedSnapshotSpanCollection spans)
        {
            var snapshot = spans[0].Snapshot;
            var _nodeOrToken = (SyntaxNodeOrToken)doc.SyntaxRoot;
            int _dept = 0;

            List<ITagSpan<IClassificationTag>> _tagSpans = new List<ITagSpan<IClassificationTag>>();

            void nextChildNode(List<ITagSpan<IClassificationTag>> tagSpans, SyntaxNodeOrToken nodeOrToken, int dept)
            {
                foreach (var child in nodeOrToken.ChildNodesAndTokens())
                {
                    if (child.ChildNodesAndTokens().Count > 0)
                        nextChildNode(tagSpans, child, ++dept);
                }

                List<SyntaxNodeOrToken> brace = nodeOrToken.ChildNodesAndTokens().Where(child =>
                       child.IsKind(SyntaxKind.OpenBraceToken)
                    || child.IsKind(SyntaxKind.CloseBraceToken)
                    || child.IsKind(SyntaxKind.OpenParenToken)
                    || child.IsKind(SyntaxKind.CloseParenToken)
                    || child.IsKind(SyntaxKind.OpenBracketToken)
                    || child.IsKind(SyntaxKind.CloseBracketToken))
                    .ToList();

                if (brace.Count == 0)
                    return;
                if (brace.Where(s => s.ToString() == "(").ToList().Count != brace.Where(s => s.ToString() == ")").ToList().Count)
                {
                    return;
                }
                if (brace.Where(s => s.ToString() == "{").ToList().Count != brace.Where(s => s.ToString() == "}").ToList().Count)
                {
                    return;
                }
                if (brace.Where(s => s.ToString() == "[").ToList().Count != brace.Where(s => s.ToString() == "]").ToList().Count)
                {
                    return;
                }

                while (brace.Count > 0)
                {
                    SyntaxNodeOrToken line1 = brace.First();
                    SyntaxNodeOrToken line2 = null;

                    string start = line1.ToString();
                    string end = ")";

                    if (start == "{")
                    {
                        end = "}";
                    }
                    if (start == "[")
                    {
                        end = "]";
                    }
                    if (start != "(" && start != "{" && start != "[")
                    {
                        break;
                    }

                    brace.Remove(line1);

                    int par = 1;
                    for (int i = 0; i < brace.Count; ++i)
                    {
                        SyntaxNodeOrToken check = brace[i];

                        if (check.ToString() == end)
                            par--;
                        else if (check.ToString() == start)
                            par++;

                        if (par == 0)
                        {
                            line2 = check;
                            brace.Remove(line2);
                            break;
                        }
                    }

                    string _tp = ((dept % 8) + 1).ToString();
                    IClassificationType classificationType = _registry.GetClassificationType(_tp);

                    tagSpans.Add(line1.Span.ToTagSpan(snapshot, classificationType));

                    if (line2 != null)
                        tagSpans.Add(line2.Span.ToTagSpan(snapshot, classificationType));
                    else
                        return;
                }
            }

            nextChildNode(_tagSpans, _nodeOrToken, _dept);

            foreach (ITagSpan<IClassificationTag> tag in _tagSpans)
            {
                yield return tag;
            }
        }
    }
}