﻿using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Languages;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace BracketPairColorizer.Core.Tags
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(ContentTypes.Text)]
    [TagType(typeof(KeywordTag))]
    public class KeywordTaggerProvider : IViewTaggerProvider
    {
        [Import]
        public IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        [Import]
        public IClassificationFormatMapService formatService = null;

        [Import]
        public IBufferTagAggregatorFactoryService Aggregator { get; set; }

        [Import]
        public ILanguageFactory LanguageFactory { get; set; }

        [Import]
        public IVsfSettings Settings { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            var map = this.formatService.GetClassificationFormatMap(textView);
            var italicsFixer = textView.Properties.GetOrCreateSingletonProperty(() =>
            new ItalicsFormatter(textView, map, Settings));

            italicsFixer.AddClassification(Constants.FLOW_CONTROL_CLASSIFICATION_NAME);

            return new KeywordTagger(buffer, this) as ITagger<T>;
        }
    }

    public class ItalicsFormatter
    {
        private ITextView textView;
        private IClassificationFormatMap formatMap;
        private IVsfSettings settings;
        private IList<string> classificationTypes;
        private bool working;

        public ItalicsFormatter(ITextView textView, IClassificationFormatMap map, IVsfSettings settings)
        {
            this.textView = textView;
            this.formatMap = map;
            this.settings = settings;
            this.working = false;
            this.classificationTypes = new List<string>();
            this.settings.SettingsChanged += OnSettingsChanged;
            this.formatMap.ClassificationFormatMappingChanged += OnMappingChanged;
            this.textView.GotAggregateFocus += OnTextViewFocus;
        }

        private void OnTextViewFocus(object sender, EventArgs e)
        {
            this.textView.GotAggregateFocus -= OnTextViewFocus;
            FixIt();
        }

        public void AddClassification(string name)
        {
            this.classificationTypes.Add(name);
        }

        public void FixIt()
        {
            FixItalics();
        }

        private void FixItalics()
        {
            if (this.working || this.formatMap.IsInBatchUpdate) { return; }

            this.working = true;
            this.formatMap = BeginBatchUpdate();
            try
            {
                foreach (var classifierType in this.formatMap.CurrentPriorityOrder)
                {
                    if (classifierType == null) { continue; }

                    if (this.classificationTypes.Contains(classifierType.Classification))
                        SetItalics(classifierType, this.settings.FlowControlUseItalics);
                }
            } finally
            {
                this.formatMap.EndBatchUpdate();

                Task.Delay(500).ContinueWith((parentTask)
                    => this.working = false);
            }
        }

        private void SetItalics(IClassificationType classifierType, bool enable)
        {
            var tp = this.formatMap.GetTextProperties(classifierType);

            if (!tp.Italic)
            {
                tp = tp.SetItalic(enable);
                this.formatMap.SetTextProperties(classifierType, tp);
            }
        }

        public void OnSettingsChanged(object sender, EventArgs e)
        {
            FixItalics();
        }

        private void OnMappingChanged(object sender, EventArgs e)
        {
            FixItalics();
        }
    }
}
