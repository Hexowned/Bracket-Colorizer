using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace BracketPairColorizer.Core.EditorFormats
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.FORMAT_SPECIFIER_NAME)]
    [Name(Constants.FORMAT_SPECIFIER_NAME)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class FormatSpecifierFormat : ClassificationFormatDefinition
    {
        public FormatSpecifierFormat()
        {
            this.DisplayName = "BracketPairColorizer Format Specifier";
            this.ForegroundColor = Colors.MediumSlateBlue;
        }
    }
}
