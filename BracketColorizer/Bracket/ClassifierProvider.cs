#region USING_DIRECTIVES

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

#endregion USING_DIRECTIVES

namespace BracketColorizer.Bracket
{
    //TODO: figure out this casting issue

    //[Export(typeof(IClassifierProvider))]
    //[ContentType("CSharp")]
    //internal class ClassifierProvider : IClassifierProvider
    //{
    //    [Import]
    //    internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;
    //
    //   static Classifier classifier;
    //
    //    public IClassifier GetClassifier(ITextBuffer buffer)
    //    {
    //        if (classifier == null)
    //            classifier = new Classifier(ClassificationTypeRegistry);
    //
    //        return classifier;
    //    }
    //}

    [Export(typeof(ITaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof(IClassificationTag))]
    internal class ClassifierDefinitions : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)new Tagger(buffer, ClassificationTypeRegistry);
        }
    }
}