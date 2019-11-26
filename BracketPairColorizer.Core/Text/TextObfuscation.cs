using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Core.Tags;
using BracketPairColorizer.Core.Utilities;
using BracketPairColorizer.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace BracketPairColorizer.Core.Text
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(ObfuscatedTextTag))]
    public class TextObfuscationProvider : IViewTaggerProvider
    {
        [Import]
        private IClassificationTypeRegistryService registryService = null;

        [Import]
        private IBpcSettings settings = null;

        public ITagger<T> CreateTagger<T>(ITextView view, ITextBuffer buffer) where T : ITag
        {
            var obfuscationType = this.registryService.GetClassificationType(Constants.OBFUSCATED_TEXT);

            return new TextObfuscationTagger(view, buffer, obfuscationType, this.settings) as ITagger<T>;
        }
    }

    public class TextObfuscationTagger : ITagger<ObfuscatedTextTag>
    {
        private ITextView theView;
        private ITextBuffer theBuffer;
        private IClassificationType obfuscationType;
        private IBpcSettings settings;
        private bool enabled;
        private List<RegexEntry> expressionsToSearch;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public TextObfuscationTagger(ITextView view, ITextBuffer buffer, IClassificationType obfuscationType, IBpcSettings settings)
        {
            this.theView = view;
            this.theBuffer = buffer;
            this.obfuscationType = obfuscationType;
            this.enabled = TextObfuscationState.Enabled;
            this.settings = settings;
            this.expressionsToSearch = settings.TextObfuscationRegexes.ListFromJson<RegexEntry>();

            this.settings.SettingsChanged += OnSettingsChanged;
            TextObfuscationState.EnabledChanged += OnEnabledChanged;
            this.theView.Closed += OnViewClosed;
        }

        public IEnumerable<ITagSpan<ObfuscatedTextTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            bool scan = this.enabled && this.expressionsToSearch.Count > 0 && spans.Count > 0;
            if (!scan) { yield break; }

            var tag = new ObfuscatedTextTag(this.obfuscationType);
            foreach (var span in spans)
            {
                var snapshot = span.Snapshot;
                var line = span.Start.GetContainingLine();
                do
                {
                    var tags = this.expressionsToSearch.SelectMany(entry => entry.Match(line)).Select(match => new TagSpan<ObfuscatedTextTag>(match, tag));

                    foreach (var tagSpan in tags) { yield return tagSpan; }

                    if (line.LineNumber + 1 >= snapshot.LineCount)
                        break;
                    line = snapshot.GetLineFromLineNumber(line.LineNumber + 1);
                } while (line.End < span.End);
            }
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.settings != null)
            {
                this.settings.SettingsChanged -= OnSettingsChanged;
                this.settings = null;
            }

            if (this.theView != null)
            {
                this.theView.Closed -= OnViewClosed;
                TextObfuscationState.EnabledChanged -= OnEnabledChanged;
                this.theView = null;
                this.theBuffer = null;
                this.obfuscationType = null;
                this.expressionsToSearch = null;
            }
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            this.enabled = TextObfuscationState.Enabled;
            var snapshot = this.theBuffer.CurrentSnapshot;
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            ReportTagsChanged(span);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            this.expressionsToSearch = this.settings.TextObfuscationRegexes.ListFromJson<RegexEntry>();
            var snapshot = this.theBuffer.CurrentSnapshot;
            var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
            ReportTagsChanged(span);
        }

        private void ReportTagsChanged(SnapshotSpan span)
        {
            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
        }
    }

    public static class TextObfuscationState
    {
        public static bool Enabled { get; private set; }

        public static event EventHandler EnabledChanged;

        public static void Invert()
        {
            Change(!Enabled);
        }

        public static void Change(bool enabled)
        {
            Enabled = enabled;
            EnabledChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
