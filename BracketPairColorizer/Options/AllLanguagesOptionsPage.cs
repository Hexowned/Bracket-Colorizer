using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BracketPairColorizer.Options
{
    [Guid(Guids.AllLanguagesOptions)]
    public class AllLanguagesOptionsPage : DialogPage
    {
        private readonly UserControl dialog = new UserControl();
        protected override IWin32Window Window => this.dialog;
    }
}
