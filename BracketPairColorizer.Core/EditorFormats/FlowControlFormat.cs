using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.FLOW_CONTROL_CLASSIFICATION_NAME)]
    [Name(Constants.FLOW_CONTROL_CLASSIFICATION_NAME)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class FlowControlFormat : ClassificationFormatDefinition
    {
        public FlowControlFormat()
        {
            this.DisplayName = "BracketPairColorizer Flow Control Keyword";
            this.ForegroundColor = Colors.OrangeRed;
        }
    }
}
