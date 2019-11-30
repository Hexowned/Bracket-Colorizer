using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace BracketPairColorizer.Commands
{
    public abstract class VisualStudioCommand
    {
        public BpcPackage Package { get; private set; }
        public OleMenuCommandService CommandService { get; private set; }
        public OleMenuCommand Command { get; private set; }

        public VisualStudioCommand(BpcPackage package, OleMenuCommandService omcs)
        {
            this.CommandService = omcs;
            this.Package = package;
        }

        protected void Initialize(Guid menuGroup, int commandId)
        {
            var cmdId = new CommandID(menuGroup, commandId);

            Command = new OleMenuCommand(this.OnInvoke, cmdId);
            Command.BeforeQueryStatus += OnBeforeQueryStatus;
            CommandService.AddCommand(Command);
        }

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
        }

        protected virtual void OnInvoke(object sender, EventArgs e)
        {
        }
    }
}
