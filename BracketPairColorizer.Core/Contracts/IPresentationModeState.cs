using System;

namespace BracketPairColorizer.Core.Contracts
{
    public interface IPresentationModeState
    {
        event EventHandler PresentationModeChanged;

        bool PresentationModeTurnedOn { get; }

        void TogglePresentationMode();

        void TurnOff(bool notifyChanges);

        int GetPresentationModeZoomLevel();

        T GetService<T>();
    }
}
