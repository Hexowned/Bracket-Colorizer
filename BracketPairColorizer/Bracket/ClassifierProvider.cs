﻿#region USING_DIRECTIVES

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

#endregion USING_DIRECTIVES

namespace BracketPairColorizer.Bracket
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")]
    internal class ClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        private static Classifier classifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (classifier == null)
                classifier = new Classifier(ClassificationTypeRegistry);

            return classifier;
        }
    }

    [Export(typeof(ITaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof(IClassificationTag))]
    internal class TaggerProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)new Tagger(buffer, ClassificationTypeRegistry);
        }
    }
}