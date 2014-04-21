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

namespace RPlugin.Core
{
    /// <summary>
    /// Base class for IRScript inherited classes
    /// </summary>
    public abstract class RScript : IRScript
    {
        /// <summary>
        /// Script arguments
        /// </summary>
        protected List<object> _arguments;

        /// <summary>
        /// Script name
        /// </summary>
        public abstract string BaseScript
        {
            get;
        }

        /// <summary>
        /// Required packages
        /// </summary>
        public abstract string[] RequiredPackages
        {
            get;
        }

        /// <summary>
        /// Script arguments
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return _arguments.ToArray();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RScript()
        {
            _arguments = new List<object>();
        }

        /// <summary>
        /// Precompute method
        /// </summary>
        public abstract void PreCompute();

        /// <summary>
        /// Imports script results
        /// </summary>
        /// <param name="result">RScriptResult object</param>
        /// <returns>Script results</returns>
        public abstract object ImportResults(RScriptResult result);
    }
}
