﻿// TraceLab Component Library
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
using TraceLabSDK;

namespace TraceLab.Components.Types.Preprocessors.ExecutionTraces
{
    /// <summary>
    /// PDG edge representation
    /// </summary>
    [Serializable]
    [WorkspaceType]
    public class PDGEdge
    {
        /// <summary>
        /// Outgoing node id
        /// </summary>
        public string OutgoingNodeID;

        /// <summary>
        /// Edge weight
        /// </summary>
        public double Weight;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outgoingNodeID">Outgoing node id</param>
        public PDGEdge(string outgoingNodeID)
        {
            OutgoingNodeID = outgoingNodeID;
            Weight = -1;
        }

        /// <summary>
        /// String representation of edge
        /// </summary>
        /// <returns>Edge string</returns>
        public override string ToString()
        {
            return "PDGEdge [OutgoingNodeID=" + OutgoingNodeID + ", Weight=" + Weight + "]";
        }
    }
}
