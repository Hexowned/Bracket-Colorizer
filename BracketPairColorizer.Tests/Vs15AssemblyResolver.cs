using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BracketPairColorizer.Tests
{
    public class Vs15AssemblyResolver
    {
        private const int REDFB_E_CLASSNOTREG = unchecked((int)0x80040154);
        private static readonly string vsInstallDirectory;
        private static readonly string[] assemblyLocations;

        static Vs15AssemblyResolverFixture()
        {
            vsInstallDirectory = TryFindVs15InstallDirectory();
            if (!string.IsNullOrEmpty(vsInstallDirectory))
            {
                assemblyLocations = new string[]
                {
                    Path.Combine(vsInstallDirectory, ""),
                    Path.Combine(vsInstallDirectory, ""),
                    Path.Combine(vsInstallDirectory, ""),
                    Path.Combine(vsInstallDirectory, ""),
                };
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            }
        }

        private static string TryFindVs15InstallDirectory()
        {
            var setupConfiguration = GetVsConfiguration();
            if (setupConfiguration != null)
            {
                var instances = setupConfiguration.EnumInstances();
                ISetupInstance[] idata = new ISetupInstance[1];
                int numberFetch = 0;
                instances.Next(1, idata, out numberFetch);
                if (numberFetch > 0)
                {
                    return idata[0].GetInstallationPath();
                }
            }

            return null;
        }

        private static ISetupConfiguration GetVsConfiguration()
        {
            try
            {
                return new SetupConfiguration();
            } catch (COMException ex) when (ex.HResult == REDFB_E_CLASSNOTREG)
            {
                try
                {
                    ISetupConfiguration setupConfiguration;
                    var result = GetSetupConfiguration(out setupConfiguration, IntPtr.Zero);

                    return result < 0 ? null : setupConfiguration;
                } catch (DllNotFoundException)
                {
                    return null;
                }
            }
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            foreach (string directory in assemblyLocations)
            {
                string path = Path.Combine(directory, name.Name + ".dll");
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
            }

            return null;
        }

        [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
        private static extern int GetSetupConfiguration(
        [MarshalAs(UnmanagedType.Interface), Out] out ISetupConfiguration configuration, IntPtr reserved);
    }

    [CollectionDefinition("DependsOnVS")]
    public class DependsOnVSCollection : ICollectionFixture<Vs15AssemblyResolverFixture>
}
