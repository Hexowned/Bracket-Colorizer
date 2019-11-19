﻿#region USING_DIRECTIVES

using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

#endregion USING_DIRECTIVES

namespace BracketColorizer
{
    public static class RoslynExtensions
    {
        public static ITagSpan<IClassificationTag> ToTagSpan(this TextSpan span, ITextSnapshot snapshot, IClassificationType classificationType)
        {
            return new TagSpan<IClassificationTag>(
                new SnapshotSpan(snapshot, span.Start, span.Length),
                new ClassificationTag(classificationType));
        }

        public static string GetText(this ITextSnapshot snapshot, TextSpan span)
        {
            return snapshot.GetText(new Span(span.Start, span.Length));
        }
    }
}