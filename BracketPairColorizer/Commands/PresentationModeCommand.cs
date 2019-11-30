using BracketPairColorizer.Core.Compatibility;
using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using Microsoft.VisualStudio.Shell;
using System;

namespace BracketPairColorizer.Commands
{
    public class PresentationModeCommand : VisualStudioCommand
    {
        private IBpcSettings settings = SettingsContext.GetSettings();
        private IPresentationModeState state;

        public PresentationModeCommand(BpcPackage package, OleMenuCommandService omcs)
            : base(package, omcs)
        {
            var model = new SComponentModel();
            this.state = model.GetService<IPresentationModeState>();
            Initialize(new Guid(Guids.guidBpcViewCmdSet), PkgCmdIdList.cmdidPresentationMode);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            base.OnBeforeQueryStatus(sender, e);
            Command.Checked = this.state.PresentationModeTurnedOn;
            Command.Enabled = this.settings.PresentationModeEnabled;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            base.OnInvoke(sender, e);
            this.state.TogglePresentationMode();
        }
    }
}
