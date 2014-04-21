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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RPlugin.Core
{
    /// <summary>
    /// Utilities class
    /// </summary>
    public static class RUtil
    {
        private static object UniqueIDLock = new object();

        /// <summary>
        /// Creates a unique directory in the cache.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Directory information</returns>
        public static DirectoryInfo CreateCacheDirectory(string name)
        {
            DirectoryInfo info;
            lock (UniqueIDLock)
            {
                int index = 0;
                while (Directory.Exists(Path.Combine(RController.Cache, index.ToString() + "." + name)))
                {
                    index++;
                }
                info = RController.CreateCacheDirectory(index.ToString() + "." + name, true);
            }
            return info;
        }

        /// <summary>
        /// Creates and opens a unique file in the cache.
        /// Do this if you plan on writing to the file immediately.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Pointer to file</returns>
        public static FileStream CreateCacheFile(string name)
        {
            FileStream info;
            lock (UniqueIDLock)
            {
                int index = 0;
                while (File.Exists(Path.Combine(RController.Cache, index.ToString() + "." + name)))
                {
                    index++;
                }
                info = RController.CreateCacheFile(index.ToString() + "." + name, false);
            }
            return info;
        }

        /// <summary>
        /// Creates a unique placeholder file in the cache.
        /// Do this if you plan to write to the file in an R script.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Pointer to file</returns>
        public static string ReserveCacheFile(string name)
        {
            FileStream info;
            lock (UniqueIDLock)
            {
                int index = 0;
                while (File.Exists(Path.Combine(RController.Cache, index.ToString() + "." + name)))
                {
                    index++;
                }
                info = RController.CreateCacheFile(index.ToString() + "." + name, false);
                info.Close();
            }
            return info.Name;
        }

        /// <summary>
        /// Registers a script with RPlugin from a resource stored in an assembly
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="resourceName">Resource name</param>
        /// <returns>True if successful, false if script already exists</returns>
        public static bool RegisterScript(Assembly assembly, string resourceName)
        {
            string destination = Path.Combine(RController.Scripts, resourceName);
            if (!File.Exists(destination))
            {
                File.WriteAllText(destination, new StreamReader(assembly.GetManifestResourceStream(resourceName)).ReadToEnd());
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a script with RPlugin from a file on disk
        /// </summary>
        /// <param name="scriptPath">Path to script</param>
        /// <returns>True if successful, false if script already exists</returns>
        public static bool RegisterScript(string scriptPath)
        {
            string destination = Path.Combine(RController.Scripts, Path.GetFileName(scriptPath));
            if (!File.Exists(destination))
            {
                File.Copy(scriptPath, destination);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
