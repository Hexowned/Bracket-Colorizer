using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.STRING_ESCAPE_ERROR_NAME)]
    [Name(Constants.STRING_ESCAPE_ERROR_NAME)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class StringEscapeSequenceErrorFormat : ClassificationFormatDefinition
    {
        public StringEscapeSequenceErrorFormat()
        {
            this.DisplayName = "BracketPairColorizer String Escape Sequence";
            this.ForegroundColor = Color.FromRgb(255, 160, 0);
        }
    }
}
