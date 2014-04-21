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
    /// RScript interface
    /// </summary>
    public interface IRScript
    {
        // properties

        /// <summary>
        /// Script name
        /// </summary>
        string BaseScript { get; }

        /// <summary>
        /// Required packages
        /// </summary>
        string[] RequiredPackages { get; }

        /// <summary>
        /// Script arguments
        /// </summary>
        object[] Arguments { get; }
        
        // methods
        
        /// <summary>
        /// Precompute method
        /// </summary>
        void PreCompute();

        /// <summary>
        /// Import results
        /// </summary>
        /// <param name="result">RScriptResult object</param>
        /// <returns>Script results</returns>
        object ImportResults(RScriptResult result);
    }
}
