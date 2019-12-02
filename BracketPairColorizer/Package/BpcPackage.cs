using BracketPairColorizer.Commands;
using BracketPairColorizer.Core.Compatibility;
using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Core.Text;
using BracketPairColorizer.Settings.Contracts;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace BracketPairColorizer.Package
{
    [PackageRegistration(UseManagedResourcesOnly = = true)]
    [Guid(Guids.VSPackage)]
    [InstalledProductRegistration("", "", productId: BpcVersion.Version, IconResourceID = 400)]
    // TODO: use [ProvideOptionPage] of the type of options to have to create the options board
    [ProvideMenuResource(1000, 1)]
    public sealed class BpcPackage : Package, IPackageUserOptions
    {
        public const string USER_OPTIONS_KEY = "BpcUserOptions";

        public PresentationModeFontChanger FontChanger { get; private set; }
        private List<VsCommand> commands = new List<VsCommands>();
        private byte[] userOptions;

        protected override void Initialize()
        {
            base.Initialize();

            if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                InitializeViewMenuCommands(mcs);
            }

            this.AddOptionKey(USER_OPTIONS_KEY);
        }

        protected override void Dispose(bool disposing)
        {
            var model = new SComponentModel();
            var ps = model.GetService<IPresentationModeState>();
            if (disposing && ps.PresentationModeTurnedOn)
            {
                ps.TurnOff(notifyChanges: false);
            }

            base.Dispose(disposing);
        }

        protected override void OnLoadOptions(string key, Stream stream)
        {
            base.OnLoadOptions(key, stream);
            if (key == USER_OPTIONS_KEY)
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                this.userOptions = data;
            }
        }

        protected override void OnSaveOptions(string key, Stream stream)
        {
            base.OnSaveOptions(key, stream);
            if (key == USER_OPTIONS_KEY && this.userOptions != null)
            {
                stream.Write(this.userOptions, 0, this.userOptions.Length);
            }
        }

        private void InitializeViewMenuCommands(OleMenuCommandService mcs)
        {
            this.commands.Add(new PresentationModeCommand(this, mcs));
            this.commands.Add(new ObfuscateTextCommand(this, mcs));
        }

        void IPackageUserOptions.Write(byte[] options)
        {
            this.userOptions = options;
        }

        byte[] IPackageUserOptions.Read()
        {
            return this.userOptions;
        }
    }
}
