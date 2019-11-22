using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Text;
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
            telemetry.FeatureStatus("CurrentLineHighlight", CurrentLineHighlightEnabled);
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
            get { return this.Store.GetBoolean(nameof(FlowControlKeywordsEnabled), true); }
            set { this.Store.SetValue(nameof(FlowControlKeywordsEnabled), value); }
        }

        public bool FlowControlUseItalics
        {
            get { return this.Store.GetBoolean(nameof(FlowControlUseItalics), false); }
            set { this.Store.SetValue(nameof(FlowControlUseItalics), value); }
        }

        public bool VisibilityKeywordsEnabled
        {
            get { return this.Store.GetBoolean(nameof(VisibilityKeywordsEnabled), true); }
            set { this.Store.SetValue(nameof(VisibilityKeywordsEnabled), value); }
        }

        public bool QueryKeywordsEnabled
        {
            get { return this.Store.GetBoolean(nameof(QueryKeywordsEnabled), true); }
            set { this.Store.SetValue(nameof(QueryKeywordsEnabled), value); }
        }

        public bool EscapeSequencesEnabled
        {
            get { return this.Store.GetBoolean(nameof(EscapeSequencesEnabled), true); }
            set { this.Store.SetValue(nameof(EscapeSequencesEnabled), value); }
        }

        public bool CurrentLineHighlightEnabled
        {
            get { return this.Store.GetBoolean(nameof(CurrentLineHighlightEnabled), false); }
            set { this.Store.SetValue(nameof(CurrentLineHighlightEnabled), value); }
        }

        public bool CurrentColumnHighlightEnabled
        {
            get { return this.Store.GetBoolean(nameof(CurrentColumnHighlightEnabled), false); }
            set { this.Store.SetValue(nameof(CurrentColumnHighlightEnabled), value); }
        }

        public ColumnStyle CurrentColumnHighlightStyle
        {
            get { return this.Store.GetEnum<ColumnStyle>(nameof(CurrentColumnHighlightStyle), ColumnStyle.FullBorder); }
            set { this.Store.SetValue(nameof(CurrentColumnHighlightStyle), value); }
        }

        public double HighlightLineWidth
        {
            get { return this.Store.GetDouble(nameof(HighlightLineWidth), 1.4); }
            set { this.Store.SetValue(nameof(HighlightLineWidth), value); }
        }

        public bool PresentationModeEnabled
        {
            get { return this.Store.GetBoolean(nameof(PresentationModeEnabled), true); }
            set { this.Store.SetValue(nameof(PresentationModeEnabled), value); }
        }

        public int PresentationModeDefaultZoom
        {
            get { return this.Store.GetInt32(nameof(PresentationModeDefaultZoom), 100); }
            set { this.Store.SetValue(nameof(PresentationModeDefaultZoom), value); }
        }

        public int PresentationModeEnabledZoom
        {
            get { return this.Store.GetInt32(nameof(PresentationModeEnabled), 150); }
            set { this.Store.SetValue(nameof(PresentationModeEnabledZoom), value); }
        }

        public bool PresentationModeIncludeEnvironmentFonts
        {
            get { return this.Store.GetBoolean(nameof(PresentationModeIncludeEnvironmentFonts), false); }
            set { this.Store.SetValue(nameof(PresentationModeIncludeEnvironmentFonts), value); }
        }

        public bool ModelinesEnabled
        {
            get { return this.Store.GetBoolean(nameof(ModelinesEnabled), true); }
            set { this.Store.SetValue(nameof(ModelinesEnabled), value); }
        }

        public int ModelinesNumberLines
        {
            get { return this.Store.GetInt32(nameof(ModelinesNumberLines), 5); }
            set { this.Store.SetValue(nameof(ModelinesNumberLines), value); }
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
