using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.LINE_HIGHLIGHT)]
    [Name(Constants.LINE_HIGHLIGHT)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class CurrentLineFormat : ClassificationFormatDefinition
    {
        public CurrentLineFormat()
        {
            this.DisplayName = "BracketPairColorizer Current Line";
            this.ForegroundColor = Colors.LightGray;
            this.ForegroundOpacity = 0.3;
            this.BackgroundOpacity = 0.3;
        }
    }
}
