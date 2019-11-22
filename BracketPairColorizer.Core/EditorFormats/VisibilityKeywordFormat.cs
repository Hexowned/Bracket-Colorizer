using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.VISIBILITY_CLASSIFICATION_NAME)]
    [Name(Constants.VISIBILITY_CLASSIFICATION_NAME)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class VisibilityKeywordFormat : ClassificationFormatDefinition
    {
        public VisibilityKeywordFormat()
        {
            this.DisplayName = "BracketPairColorizer Visibility Keyword";
            this.ForegroundColor = Colors.DimGray;
        }
    }
}
