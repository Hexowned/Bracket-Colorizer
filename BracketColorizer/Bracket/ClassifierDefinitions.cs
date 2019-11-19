#region USING_DIRECTIVES

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

#endregion USING_DIRECTIVES

namespace BracketColorizer.Bracket
{
    internal static class ClassificationDefinitions
    {
        #region TYPE_DEFINITIONS

        [Export]
        [Name("0")]
        internal static ClassificationTypeDefinition bracketClassificationDefinitions = null;

        [Export]
        [Name("1")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition1 = null;

        [Export]
        [Name("2")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition2 = null;

        [Export]
        [Name("3")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition3 = null;

        [Export]
        [Name("4")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition4 = null;

        [Export]
        [Name("5")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition5 = null;

        [Export]
        [Name("6")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition6 = null;

        [Export]
        [Name("7")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition7 = null;

        [Export]
        [Name("8")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition8 = null;

        [Export]
        [Name("9")]
        [BaseDefinition("0")]
        internal static ClassificationTypeDefinition Definition9 = null;

        #endregion TYPE_DEFINITIONS

        #region FORMAT_PRODUCTIONS

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "1")]
        [Name("1")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format1 : ClassificationFormatDefinition
        {
            public Format1()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "2")]
        [Name("2")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format2 : ClassificationFormatDefinition
        {
            public Format2()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "3")]
        [Name("3")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format3 : ClassificationFormatDefinition
        {
            public Format3()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "4")]
        [Name("4")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format4 : ClassificationFormatDefinition
        {
            public Format4()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "5")]
        [Name("5")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format5 : ClassificationFormatDefinition
        {
            public Format5()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "6")]
        [Name("6")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format6 : ClassificationFormatDefinition
        {
            public Format6()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "7")]
        [Name("7")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format7 : ClassificationFormatDefinition
        {
            public Format7()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "8")]
        [Name("8")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format8 : ClassificationFormatDefinition
        {
            public Format8()
            {
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "9")]
        [Name("9")]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        internal sealed class Format9 : ClassificationFormatDefinition
        {
            public Format9()
            {
            }
        }

        #endregion FORMAT_PRODUCTIONS
    }
}