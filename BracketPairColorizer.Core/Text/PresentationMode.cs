using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Text.Editor;
using System;

namespace BracketPairColorizer.Core.Text
{
    public class PresentationMode
    {
        private IWpfTextView theView;
        private IVsfSettings settings;
        private IPresentationModeState state;

        public PresentationMode(IWpfTextView textView, IPresentationModeState state, IVsfSettings settings)
        {
            this.theView = textView;
            this.settings = settings;
            this.state = state;

            state.PresentationModeChanged += OnPresentationModeChanged;

            settings.SettingsChanged += OnSettingsChanged;
            textView.Closed += OnTextViewClosed;
            textView.ViewportWidthChanged += OnViewportWidthChanged;
        }

        private void OnPresentationModeChanged(object sender, EventArgs e)
        {
            if (this.theView != null)
            {
                SetZoomLevel(this.theView);
            }
        }

        private void OnViewportWidthChanged(object sender, EventArgs e)
        {
            this.theView.ViewportWidthChanged -= OnViewportWidthChanged;
            SetZoomLevel(this.theView);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            SetZoomLevel(this.theView);
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            if (this.settings != null)
            {
                this.settings.SettingsChanged -= OnSettingsChanged;
                this.settings = null;
            }

            if (this.theView != null)
            {
                this.state.PresentationModeChanged -= OnPresentationModeChanged;
                this.theView.Closed -= OnTextViewClosed;
                this.theView.ViewportWidthChanged -= OnViewportWidthChanged;
                this.theView = null;
            }
        }

        private void SetZoomLevel(IWpfTextView textView)
        {
            if (textView.IsPeekTextWindow())
                return;
            if (this.settings.PresentationModeEnabled)
            {
                int zoomLevel = this.state.GetPresentationModeZoomLevel();
                if (textView.ZoomLevel != 100 && this.state.PresentationModeTurnedOn)
                    return;
                textView.ZoomLevel = zoomLevel;
            }
        }
    }
}
