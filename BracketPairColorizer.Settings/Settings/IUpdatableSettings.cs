using System;

namespace BracketPairColorizer.Settings.Settings
{
    public interface IUpdatableSettings
    {
        event EventHandler SettingsChanged;
    }
}
