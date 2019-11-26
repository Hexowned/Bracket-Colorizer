using BracketPairColorizer.Core.Text;
using BracketPairColorizer.Settings.Settings;
using System;

namespace BracketPairColorizer.Core.Settings
{
    public interface IBpcSettings : IUpdatableSettings
    {
        bool KeywordClassifierEnabled { get; set; }
        bool FlowControlKeywordsEnabled { get; set; }
        bool VisibilityKeywordsEnabled { get; set; }
        bool QueryKeywordsEnabled { get; set; }
        bool FlowControlUseItalics { get; set; }
        bool EscapeSequencesEnabled { get; set; }

        bool CurrentLineHighlightEnabled { get; set; }
        bool CurrentColumnHighlightEnabled { get; set; }
        ColumnStyle CurrentColumnHighlightStyle { get; set; }
        double HighlightLineWidth { get; set; }

        bool PresentationModeEnabled { get; set; }
        int PresentationModeDefaultZoom { get; set; }
        int PresentationModeEnabledZoom { get; set; }
        bool PresentationModeIncludeEnvironmentFonts { get; set; }

        bool ModelinesEnabled { get; set; }
        int ModelinesNumberLines { get; set; }

        bool DeveloperMarginEnabled { get; set; }
        Outlining.AutoExpandMode AutoExpandRegions { get; set; }
        bool BoldAsItalicsEnabled { get; set; }
        String TextObfuscationRegexes { get; set; }
        bool TelemetryEnabled { get; set; }

        void Load();

        void Save();
    }
}
