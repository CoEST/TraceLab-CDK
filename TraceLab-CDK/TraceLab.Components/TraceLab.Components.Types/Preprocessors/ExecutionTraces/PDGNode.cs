// TraceLab Component Library
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
    /// PDG node representation
    /// </summary>
    [Serializable]
    [WorkspaceType]
    public class PDGNode
    {
        /// <summary>
        /// Method name
        /// </summary>
        public string MethodName { get; private set; }
	    
        //public string DocumentID { get; private set; }

        /// <summary>
        /// Collection of outgoing edges
        /// </summary>
        public IEnumerable<PDGEdge> OutgoingEdges
        {
            get
            {
                return Children.Values;
            }
        }

        private SerializableDictionary<string, PDGEdge> Children;

        private PDGNode() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">Method name</param>
	    public PDGNode(string methodName)//, string documentID)
	    {
		    MethodName = methodName;
		    //DocumentID = documentID;
		    Children = new SerializableDictionary<string, PDGEdge>();
	    }

        /// <summary>
        /// Adds an outgoing node
        /// </summary>
        /// <param name="childID">Outgoing node id</param>
	    public void AddChild(string childID)
	    {
            if (Children.ContainsKey(childID))
            {
                Children[childID].Weight += 1;
            }
            else
            {
                PDGEdge edge = new PDGEdge(childID);
                edge.Weight = 1;
			    Children.Add(childID, edge);
            }
	    }

        /// <summary>
        /// Sets edge weight
        /// </summary>
        /// <param name="childID">Outgoing node id</param>
        /// <param name="weight">Edge weight</param>
        public void SetEdgeWeight(string childID, double weight)
        {
            if (Children.ContainsKey(childID))
            {
                Children[childID].Weight = weight;
            }
            else
            {
                PDGEdge edge = new PDGEdge(childID);
                edge.Weight = weight;
                Children.Add(childID, edge);
            }
        }

        /// <summary>
        /// Gets edge based on outgoing id
        /// </summary>
        /// <param name="childID">outgoing id</param>
        /// <returns>Edge object</returns>
        public PDGEdge GetEdge(string childID)
        {
            if (Children.ContainsKey(childID))
            {
                return Children[childID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// String representation of node
        /// </summary>
        /// <returns>Node string</returns>
	    public override string ToString()
	    {
            return "PDGNode [MethodName=" + MethodName + ", outgoingEdges=(" + String.Join(",", OutgoingEdges) + ")]";
	    }
    }
}
