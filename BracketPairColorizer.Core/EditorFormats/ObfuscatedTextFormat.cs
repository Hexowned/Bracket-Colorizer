using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.OBFUSCATED_TEXT)]
    [Name(Constants.OBFUSCATED_TEXT)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ObfuscatedTextFormatDefinition : ClassificationFormatDefinition
    {
        public ObfuscatedTextFormatDefinition()
        {
            this.DisplayName = "BracketPairColorizer Obfuscated Text";
            this.ForegroundColor = Colors.Transparent;
            this.ForegroundOpacity = 0;
            this.BackgroundOpacity = 0.7;
            this.BackgroundColor = Colors.WhiteSmoke;
            this.ForegroundCustomizable = false;
        }
    }
}
