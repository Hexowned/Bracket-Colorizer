using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace BracketPairColorizer.Core.Settings
{
    public static class ReSharper
    {
        public const string PackageId = ""; // TODO:
        private static Lazy<bool> isInstalled = new Lazy<bool>(GetInstalled);

        public static bool Installed => isInstalled.Value;

        private static bool GetInstalled()
        {
            var vsShell = (IVsShell)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsShell));
            if (vsShell != null)
            {
                var pkgId = new Guid(PackageId);
                int installed = 0;
                int hr = vsShell.IsPackageInstalled(ref pkgId, out installed);

                if (ErrorHandler.Succeeded(hr))
                {
                    return installed != 0;
                }
            }

            return false;
        }
    }
}
