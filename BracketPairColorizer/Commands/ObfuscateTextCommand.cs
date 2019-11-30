using BracketPairColorizer.Core.Compatibility;
using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Core.Text;
using Microsoft.VisualStudio.Shell;
using System;

namespace BracketPairColorizer.Commands
{
    public class ObfuscateTextCommand : VisualStudioCommand
    {
        private IBpcTelemetry telemetry;

        public ObfuscateTextCommand(BpcPackage package, OleMenuCommandService omcs)
            : base(package, omcs)
        {
            Initialize(new Guid(Guids.guidBpcViewCmdSet), PkgCmdIdList.cmdidObfuscateText);
            var model = new SComponentModel();
            this.telemetry = model.GetService<IBpcTelemetry>();
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            base.OnBeforeQueryStatus(sender, e);
            Command.Checked = TextObfuscationState.Enabled;
            Command.Enabled = true;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            base.OnInvoke(sender, e);
            TextObfuscationState.Invert();
            this.telemetry?.WriteEvent("Obfuscate Text");
        }
    }
}
