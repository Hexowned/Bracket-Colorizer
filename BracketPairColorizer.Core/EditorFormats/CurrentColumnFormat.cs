using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.COLUMN_HIGHLIGHT)]
    [Name(Constants.COLUMN_HIGHLIGHT)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CurrentColumnFormat : ClassificationFormatDefinition
    {
        public CurrentColumnFormat()
        {
            this.DisplayName = "BracketPairColorizer Current Column";
            this.ForegroundColor = Colors.LightGray;
            this.ForegroundOpacity = 0.3;
            this.BackgroundOpacity = 0.3;
        }
    }
}
