using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.LINQ_CLASSIFICATION_NAME)]
    [Name(Constants.LINQ_CLASSIFICATION_NAME)]
    [Order(After = Priority.High)]
    public sealed class LinqKeywordFormat : ClassificationFormatDefinition
    {
        public LinqKeywordFormat()
        {
            this.DisplayName = "BracketPairColorizer Query Operator";
            this.ForegroundColor = Colors.MediumSeaGreen;
        }
    }
}
