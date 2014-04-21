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
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RPlugin.Core
{
    /// <summary>
    /// Responsible for returning the output of calling a script.
    /// It is up to the user to check the results of ExitCode.
    /// </summary>
    public class RScriptResult
    {
        /// <summary>
        /// Standard output of the script
        /// </summary>
        public string Output
        {
            get
            {
                return ScriptOutput.ToString();
            }
            internal set
            {
                ScriptOutput = new StringBuilder(value);
            }
        }

        /// <summary>
        /// Standard error of the script
        /// </summary>
        public string Error
        {
            get
            {
                return ScriptError.ToString();
            }
            internal set
            {
                ScriptError = new StringBuilder(value);
            }
        }

        /// <summary>
        /// Script exit code
        /// </summary>
        public int ExitCode { get; internal set; }

        private StringBuilder ScriptOutput;
        private StringBuilder ScriptError;

        internal RScriptResult()
        {
            ScriptOutput = new StringBuilder();
            ScriptError = new StringBuilder();
            ExitCode = 0;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="output">Standard output</param>
        /// <param name="error">Standard error</param>
        /// <param name="exit">Exit code</param>
        internal RScriptResult(string output, string error, int exit)
        {
            Output = output;
            Error = error;
            ExitCode = exit;
        }


        internal void ScriptOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                ScriptOutput.Append(outLine.Data);
            }
        }

        internal void ScriptErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                ScriptError.Append(outLine.Data);
            }
        }
    }
}
