using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace BracketPairColorizer.Core.Text
{
    public class PresentationModeFontChanger
    {
        private IPresentationModeState packageState;
        private IVsfSettings settings;
        private FontCategory[] categories;
        private IVsFontAndColorStorage fontsAndColors;
        private bool enabled;

        public PresentationModeFontChanger(IPresentationModeState state)
        {
            this.packageState = state;
            this.enabled = true;
            this.settings = SettingsContext.GetSettings();
            this.categories = GetCategories();
            this.fontsAndColors = null;
        }

        public void TurnOn()
        {
            if (!this.settings.PresentationModeIncludeEnvironmentFonts)
                return;

            double zoomLevel = this.packageState.GetPresentationModeZoomLevel();
            this.enabled = true;
            foreach (var category in this.categories)
            {
                TurnOnCategory(category, zoomLevel);
            }
        }

        public void TurnOff(bool notifyChanges = true)
        {
            if (this.enabled)
            {
                foreach (var category in this.categories)
                {
                    TurnOffCateogry(category, notifyChanges);
                }
            }
        }

        public void EnsureFontsAndColors()
        {
            if (this.fontsAndColors == null)
            {
                this.fontsAndColors = this.packageState.GetService<IVsFontAndColorStorage>();
            }
        }

        private void TurnOnCategory(FontCategory category, double zoomLevel)
        {
        }

        private void TurnOffCateogry(FontCategory category, bool notifyChanges)
        {
        }

        private FontCategory[] GetCategories()
        {
            return new FontCategory[]
            {
                // TODO:
                // Environment Font
                new FontCategory(""),
                // Statement Completion
                new FontCategory(""),
                // Editor Tooltip
                new FontCategory(""),
            };
        }
    }

    internal class FontCategory
    {
        public Guid Id;
        public FontInfo FontInfo;

        public FontCategory(string categoryId)
        {
            this.Id = Guid.Parse(categoryId);
        }
    }
}
