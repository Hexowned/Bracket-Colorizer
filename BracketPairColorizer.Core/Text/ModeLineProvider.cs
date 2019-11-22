using BracketPairColorizer.Core.Utilities;
using BracketPairColorizer.Languages;
using BracketPairColorizer.Languages.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;

namespace BracketPairColorizer.Core.Text
{
    public class ModeLineProvider
    {
        private IWpfTextView theView;
        private static Dictionary<string, Action<IWpfTextView, string>> optionMap;
        private ILanguageFactory languageFactory;

        static ModeLineProvider()
        {
            optionMap = new Dictionary<string, Action<IWpfTextView, string>>();
            InitializeOptionMap();
        }

        public ModeLineProvider(IWpfTextView view, ModeLineFactory factory)
        {
            this.theView = view;
            this.languageFactory = factory.LanguageFactory;
        }

        public void ParseModeline(int numberLine)
        {
            var buffer = this.theView.TextBuffer;
            var snapshot = buffer.CurrentSnapshot;
            if (snapshot.LineCount <= numberLine) { return; }

            var language = this.languageFactory.TryCreateLanguage(snapshot);
            if (language == null) return;

            var firstLine = snapshot.GetLineFromLineNumber(numberLine);

            ITextChars tc = new LineCharacters(firstLine);
            var svc = language.GetService<IFirstLineCommentParser>();
            string commentText = svc.Parse(tc);
            if (string.IsNullOrEmpty(commentText)) { return; }

            var modelineParser = new ModeLineParser();
            var options = modelineParser.Parse(commentText);
            ApplyModeLines(options);
        }

        private void ApplyModeLines(IDictionary<string, string> options)
        {
            foreach (string key in options.Keys)
            {
                ApplyModeLine(key, options[key]);
            }
        }

        private void ApplyModeLine(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = key.StartsWith("no") ? "false" : "true";
            }

            Action<IWpfTextView, string> option;
            if (optionMap.TryGetValue(key, out option))
            {
                option(this.theView, value);
            }
        }

        private static void SetShiftWidth(IWpfTextView view, string value)
        {
            int intValue;
            if (Int32.TryParse(value, out intValue))
            {
                view.Options.SetOptionValue(DefaultOptions.IndentSizeOptionId, intValue);
            }
        }

        private static void SetTabStop(IWpfTextView view, string value)
        {
            int intValue;
            if (Int32.TryParse(value, out intValue))
            {
                view.Options.SetOptionValue(DefaultOptions.TabSizeOptionId, intValue);
            }
        }

        private static void SetFileFormat(IWpfTextView view, string value)
        {
            string eol = null;
            switch (value)
            {
                case "dos":
                    eol = "r\n";
                    break;

                case "unix":
                    eol = "\n";
                    break;

                case "mac":
                    eol = "\r";
                    break;
            }

            if (!string.IsNullOrEmpty(eol))
            {
                view.Options.SetOptionValue(DefaultOptions.NewLineCharacterOptionId, eol);
            }
        }

        private static void InitializeOptionMap()
        {
            optionMap["et"] = SetExpandTab;
            optionMap["expandtab"] = SetExpandTab;
            optionMap["noet"] = SetExpandTab;
            optionMap["noexpandtab"] = SetExpandTab;
            optionMap["ts"] = SetTabStop;
            optionMap["tabstop"] = SetTabStop;
            optionMap["sw"] = SetShiftWidth;
            optionMap["shiftwidth"] = SetShiftWidth;
            optionMap["ff"] = SetFileFormat;
            optionMap["fileformat"] = SetFileFormat;
        }
    }
}
