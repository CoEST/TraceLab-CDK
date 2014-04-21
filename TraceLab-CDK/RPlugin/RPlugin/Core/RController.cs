// RPlugin - A framework for running R scripts in .NET
// Copyright © 2012-2013 SEMERU
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using RPlugin.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace RPlugin.Core
{
    internal static class RController
    {
        internal static bool HasSetupRun;
        internal static string Temp;
        internal static string Cache;
        internal static string Scripts;

        /// <summary>
        /// Static constructor
        /// Everything in here must be run before calling other methods
        /// </summary>
        static RController()
        {
            HasSetupRun = false;
            Temp = Path.Combine(Path.GetTempPath(), Properties.Settings.Default.TempPath);
            Cache = Path.Combine(Temp, Properties.Settings.Default.TempCache);
            Scripts = Path.Combine(Temp, Properties.Settings.Default.TempScripts);
            // create temp paths
            Directory.CreateDirectory(Temp);
            Directory.CreateDirectory(Cache);
            Directory.CreateDirectory(Scripts);
            // extract scripts
            foreach (string resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".R"))
                {
                    ExtractResource(Scripts, resourceName);
                }
            }
            // success
            HasSetupRun = true;
        }

        internal static void RequirePackages(string RScriptPath, params string[] args)
        {
            RequireSetupRun();
            // check packages
            RScriptResult RCheck = RunScript(RScriptPath, Path.Combine(Scripts, Settings.Default.Resources + "PackageInstaller.R"), "0 " + String.Join(" ", args));
            if (RCheck.ExitCode != 0)
            {
                HasSetupRun = false;
                throw new Exception("There was an error in the application:\n" + RCheck.Error);
            }
            // install missing packages
            if (!String.IsNullOrWhiteSpace(RCheck.Output.Trim()))
            {
                RScriptResult RInstall = RunScriptAdmin(RScriptPath, Path.Combine(Scripts, Settings.Default.Resources + "PackageInstaller.R"), "1 " + RCheck.Output.Trim());
                if (RInstall.ExitCode != 0)
                {
                    HasSetupRun = false;
                    throw new Exception("There was an error installing packages.");
                }
            }
        }

        /// <summary>
        /// Calls RScript executable and runs the given script as a hidden process
        /// </summary>
        /// <param name="RScriptPath">Path to RScript executable</param>
        /// <param name="script">Script location</param>
        /// <param name="args">Script arguments</param>
        /// <returns>Script output</returns>
        internal static RScriptResult RunScript(string RScriptPath, string script, params object[] args)
        {
            RequireSetupRun();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = RScriptPath,
                Arguments = "--verbose \"" + Path.Combine(Scripts, script) + "\" " + String.Join(" ", args),
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
            RScriptResult result = new RScriptResult();
            Process R = new Process();
            R.StartInfo = info;
            R.OutputDataReceived += result.ScriptOutputHandler;
            R.ErrorDataReceived += result.ScriptErrorHandler;
            R.Start();
            R.BeginOutputReadLine();
            R.BeginErrorReadLine();
            R.WaitForExit();
            result.ExitCode = R.ExitCode;
            return result;
        }

        /// <summary>
        /// Calls RScript executable and runs the given script with administrative priveleges
        /// Must be run in a visible window
        /// Fails if the user denies priveleges
        /// </summary>
        /// <param name="RScriptPath">Path to RScript executable</param>
        /// <param name="script">Script location</param>
        /// <param name="args">Script arguments</param>
        /// <returns>Script output</returns>
        internal static RScriptResult RunScriptAdmin(string RScriptPath, string script, params object[] args)
        {
            RequireSetupRun();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C \"call \"" + RScriptPath + "\" \"" + Path.Combine(Scripts, script) + "\" " + String.Join(" ", args) + "\"",
                UseShellExecute = true,
                Verb = "runas",
            };
            Process RAdmin = new Process();
            RAdmin.StartInfo = info;
            RAdmin.Start();
            RAdmin.WaitForExit();
            return new RScriptResult(null, null, RAdmin.ExitCode);
        }

        /// <summary>
        /// Removes all files and subdirectories from the cache
        /// </summary>
        internal static void ClearCache()
        {
            ClearCache(Cache);
        }

        /// <summary>
        /// Removes all files and subdirectories from the cache recursively
        /// </summary>
        /// <param name="path">path to clear</param>
        internal static void ClearCache(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                foreach (FileInfo file in dInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception) { }
                }
                foreach (DirectoryInfo dir in dInfo.GetDirectories())
                {
                    try
                    {
                        ClearCache(dir.FullName);
                    }
                    catch (Exception) { }
                }
                if (!path.Equals(Cache))
                {
                    try
                    {
                        Directory.Delete(path);
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Creates a new file in the cache.
        /// </summary>
        /// <param name="name">Name of file</param>
        /// <param name="overwrite">Overwrites an existing file if true.</param>
        /// <returns>FileStream reference</returns>
        internal static FileStream CreateCacheFile(string name, bool overwrite)
        {
            RequireSetupRun();
            string filename = Path.Combine(Cache, name);
            if (File.Exists(filename) && !overwrite)
            {
                throw new Exception("Filename collision in cache: " + filename);
            }
            else
            {
                return File.Create(filename);
            }
        }

        /// <summary>
        /// Creates a new directory in the cache.
        /// </summary>
        /// <returns>Directory info, null if checkExists &amp;&amp; exists</returns>
        internal static DirectoryInfo CreateCacheDirectory(string name, bool checkExists)
        {
            RequireSetupRun();
            string dirname = Path.Combine(Cache, name);
            if (Directory.Exists(dirname) && checkExists)
            {
                return null;
            }
            else
            {
                return Directory.CreateDirectory(dirname);
            }
        }

        /// <summary>
        /// Extracts an embedded resource to disk
        /// </summary>
        /// <param name="dir">Target directory</param>
        /// <param name="ResourceName">Resource short name</param>
        internal static void ExtractResource(string dir, string ResourceName)
        {
            File.WriteAllText(Path.Combine(dir, ResourceName),
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)).ReadToEnd());
        }

        /// <summary>
        /// Returns the (textual) contents of an embedded resource
        /// </summary>
        /// <param name="ResourceName">Resource short name</param>
        /// <returns>Resource contents</returns>
        internal static string GetResourceContents(string ResourceName)
        {
            return (new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))).ReadToEnd();
        }

        /// <summary>
        /// Checks to see if setup has been run.
        /// Throws an error if it has not.
        /// External code can check HasSetupRun themselves for this value.
        /// </summary>
        internal static void RequireSetupRun()
        {
            if (!HasSetupRun)
            {
                throw new Exception("Setup has not been run.");
            }
        }
    }
}
