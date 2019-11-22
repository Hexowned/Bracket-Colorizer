using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Settings.Settings;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Settings
{
    [Export(typeof(IVsfSettings))]
    public class VsfSettings : SettingsBase, IVsfSettings
    {
        [ImportingConstructor]
        public VsfSettings(ITypedSettingsStore store, IVsfTelemetry telemetry)
            : base(store)
        {
            telemetry.FeatureStatus("DeveloperMargin", DeveloperMarginEnabled);
            telemetry.FeatureStatus("Modelines", ModelinesEnabled);
            telemetry.FeatureStatus("BoldAsItalics", BoldAsItalicsEnabled);
            telemetry.FeatureStatus("CurrentColumnHighlight", CurrentColumnHighlightEnabled);
            telemetry.FeatureStatus("CurrentLineHighlight", CurrentLineHighlighEnabled);
            telemetry.FeatureStatus("EscapeSequences", EscapeSequencesEnabled);
            telemetry.FeatureStatus("QueryKeywords", QueryKeywordsEnabled);
            telemetry.FeatureStatus("FlowControlKeywords", FlowControlKeywordsEnabled);
            telemetry.FeatureStatus("VisibilityKeywords", VisibilityKeywordsEnabled);
        }

        public bool KeywordClassifierEnabled
        {
            get { return this.Store.GetBoolean(nameof(KeywordClassifierEnabled), true); }
            set { this.Store.SetValue(nameof(KeywordClassifierEnabled), value); }
        }

        public bool FlowControlKeywordsEnabled
        {
        }

        public bool VisibilityKeywordsEnabled
        {
        }

        public bool QueryKeywordsEnabled
        {
        }

        public bool FlowControlUseItalics
        {
        }

        public bool EscapeSequencesEnabled
        {
        }

        public bool CurrentLineHighlighEnabled
        {
        }

        public bool CurrentColumnHighlightEnabled
        {
        }

        public ColumnStyle CurrentColumnHighlightStyle
        {
        }

        public double HighlightLineWidth
        {
        }

        public bool PresentationmodeEnabled
        {
        }

        public int PresentationModeDefaultZoom
        {
        }

        public int PresentationModeEnabledZoom
        {
        }

        public bool PresentationModeIncludeEnvironmentFonts
        {
        }

        public bool ModelinesEnabled
        {
        }

        public int ModelinesNumberLines
        {
        }

        public bool DeveloperMarginEnabled
        {
            get { return this.Store.GetBoolean(nameof(DeveloperMarginEnabled), true); }
            set { this.Store.SetValue(nameof(AutoExpandRegions), value); }
        }

        public Outlining.AutoExpandMode AutoExpandRegions
        {
            get { return this.Store.GetEnum(nameof(AutoExpandRegions), Outlining.AutoExpandMode.No); }
            set { this.Store.SetValue(nameof(AutoExpandRegions), value); }
        }

        public bool BoldAsItalicsEnabled
        {
            get { return this.Store.GetBoolean(nameof(BoldAsItalicsEnabled), false); }
            set { this.Store.SetValue(nameof(BoldAsItalicsEnabled), value); }
        }

        public string TextObfuscationRegexes
        {
            get { return this.Store.GetString(nameof(TextObfuscationRegexes), ""); }
            set { this.Store.SetValue(nameof(TextObfuscationRegexes), value); }
        }

        public bool TelemetryEnabled
        {
            get { return this.Store.GetBoolean(nameof(TelemetryEnabled), true); }
            set { this.Store.SetValue(nameof(TelemetryEnabled), value); }
        }
    }
}
