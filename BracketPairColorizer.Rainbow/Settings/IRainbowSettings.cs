using BracketPairColorizer.Settings.Settings;

namespace BracketPairColorizer.Rainbow.Settings
{
    public interface IRainbowSettings : IUpdatableSettings
    {
        int RanbowDepth { get; set; }
        bool RainbowTagsEnabled { get; set; }
        bool RainbowColorize { get; set; }
        bool RainbowCtrlTimer { get; set; }
        bool RainbowToolTipsEnabled { get; set; }
        bool RainbowLinesEnabled { get; set; }
        RainbowHighlightMode RainbowHighlightMode { get; set; }
        RainbowColoringMode RainbowColoringMode { get; set; }

        void Save();
    }
}
