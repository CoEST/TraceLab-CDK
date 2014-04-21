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

using RPlugin.Exceptions;
using System.IO;

namespace RPlugin.Core
{
    /// <summary>
    /// Public facing R execution engine
    /// </summary>
    public class REngine
    {
        private string _path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to RScript executable</param>
        public REngine(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not find part of the path to the RScript executable.", path);
            }
            _path = path;
        }

        /// <summary>
        /// Executes an R script
        /// </summary>
        /// <param name="script">R script</param>
        /// <returns>Script results</returns>
        public object Execute(IRScript script)
        {
            RController.RequirePackages(_path, script.RequiredPackages);
            script.PreCompute();
            RScriptResult result = RController.RunScript(_path, script.BaseScript, script.Arguments);
            if (result.ExitCode != 0)
            {
                throw new RExecutionException(script.BaseScript, script.Arguments, result.Output, result.Error);
            }
            return script.ImportResults(result);
        }
    }
}
