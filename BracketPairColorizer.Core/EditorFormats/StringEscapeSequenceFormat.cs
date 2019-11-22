using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.STRING_ESCAPE_CLASSIFICATION_NAME)]
    [Name(Constants.STRING_ESCAPE_CLASSIFICATION_NAME)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class StringEscapeSequenceFormat : ClassificationFormatDefinition
    {
        public StringEscapeSequenceFormat()
        {
            this.DisplayName = "BracketPairColorizer String Escape Sequence";
            this.ForegroundColor = Colors.Magenta;
        }
    }
}
