using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.XLangSupport
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ContentTypes.Code)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class XLangTextViewCreationListener : IWpfTextViewCreationListener
    {
        public static readonly Guid FakeTag = new Guid("");

        [Import]
        public IContentTypeRegistryService Registry { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            var buffer = textView.TextBuffer;
            if (buffer.ContentType.TypeName != ContentTypes.Code)
                return;
            if (buffer.CurrentSnapshot.Length == 0)
            {
                ChangeContentType(buffer);
            }
        }

        private void ChangeContentType(ITextBuffer buffer)
        {
            var fakeXLang = Registry.GetContentType(ContentTypes.XLang);
            buffer.ChangeContentType(fakeXLang, FakeTag);
        }
    }
}
