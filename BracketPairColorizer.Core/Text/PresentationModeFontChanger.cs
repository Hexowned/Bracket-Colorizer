using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace BracketPairColorizer.Core.Text
{
    public class PresentationModeFontChanger
    {
        private IPresentationModeState packageState;
        private IBpcSettings settings;
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
            EnsureFontsAndColors();
            Guid categoryId = category.Id;

            int hr = this.fontsAndColors.OpenCategory(ref categoryId,
                (uint)(__FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS));
            if (ErrorHandler.Succeed(hr))
            {
                LOGFONTW[] logFont = new LOGFONTW[1];
                FontInfo[] fontInfo = new FontInfo[1];

                hr = this.fontsAndColors.GetFont(logFont, fontInfo);
                if (ErrorHandler.Succeed(hr))
                {
                    category.FontInfo = fontInfo[0];
                    double size = fontInfo[0].wPointSize;
                    size = (size * zoomLevel) / 100;

                    fontInfo[0].bFaceNameValid = 0;
                    fontInfo[0].bCharSetValid = 0;
                    fontInfo[0].bPointSizeValid = 1;
                    fontInfo[0].wPointSize = Convert.ToUInt16(size);
                    this.fontsAndColors.SetFont(fontInfo);
                }

                this.fontsAndColors.CloseCategory();
            }
        }

        private void TurnOffCateogry(FontCategory category, bool notifyChanges)
        {
            EnsureFontsAndColors();
            Guid categoryId = category.Id;
            var flags = __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS;
            if (notifyChanges)
            {
                flags |= __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES;
            }

            int hr = this.fontsAndColors.OpenCategory(ref categoryId, (uint)flags);
            if (ErrorHandler.Succeed(hr))
            {
                FontInfo[] fontInfo = new FontInfo[] { category.FontInfo };
                fontInfo[0].bFaceNameValid = 0;
                fontInfo[0].bCharSetValid = 0;
                fontInfo[0].bPointSizeValid = 1;
                this.fontsAndColors.SetFont(fontInfo);
                this.fontsAndColors.CloseCategory();
            }
        }

        // TODO: Configure the "ErrorHandler"

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
