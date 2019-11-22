using BracketPairColorizer.Settings.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;

namespace BracketPairColorizer.Core.Settings
{
    public static class VsSolution
    {
        public static string GetSolutionPath()
        {
            IVsSolution solution = (IVsSolution)
                ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
            if (solution == null) { return null; }

            string solutionDirectory, solutionFile, userOptionsFile;
            int hr = solution.GetSolutionInfo(out solutionDirectory, out solutionFile, out userOptionsFile);
            CheckError(hr, "GetSolutionInfo");

            return string.IsNullOrEmpty(solutionDirectory) ? null : Path.GetFullPath(solutionDirectory);
        }

        public static string MakeRelativePath(string toPath)
        {
            string solutionFile = GetSolutionPath();

            return MakeRelativePath(solutionFile, toPath);
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) { return toPath; }
            // http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
            Uri fromUri, toUri;
            if (Uri.TryCreate(fromPath, UriKind.Absolute, out fromUri) && Uri.TryCreate(toPath, UriKind.Absolute, out toUri))
            {
                if (fromUri.Scheme != toUri.Scheme) { return toPath; }

                Uri relativeUri = fromUri.MakeRelativeUri(toUri);
                string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

                if (toUri.Scheme.ToUpperInvariant() == "FILE")
                {
                    relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                }

                return relativePath;
            }

            return toPath;
        }

        public static ISolutionUserSettings GetUserSettings()
        {
            // TODO: Find a way to enable loading/storing settings in a .SUO file
            IPersistSettings persist = new SuoPersistUserSettings(null);

            return new SolutionUserSettings(persist);
        }

        private static void CheckError(int hr, string operation)
        {
            if (hr != Constants.S_OK)
            {
                var ex = new InvalidOperationException(string.Format("{0} returned 0x{1:x8}", operation, hr));
                PkgSource.LogError(operation, ex);
                throw ex;
            }
        }
    }
}
