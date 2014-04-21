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
    /// Program Dependency Graph
    /// </summary>
    [Serializable]
    [WorkspaceType]
    public class PDG
    {
        #region Static utilities

        /// <summary>
        /// Converts a BiGramCollection to a PDG
        /// </summary>
        /// <param name="bigrams"></param>
        /// <returns>PDG</returns>
        public static PDG Convert(BiGramCollection bigrams)
        {
            PDG pdg = new PDG();
            
            foreach (BiGram bigram in bigrams)
            {
                if (!pdg._nodes.ContainsKey(bigram.Caller))
                {
                    pdg.Add(new PDGNode(bigram.Caller));
                }
                pdg._nodes[bigram.Caller].AddChild(bigram.Callee);
                if (!pdg._nodes.ContainsKey(bigram.Callee))
                    pdg.Add(new PDGNode(bigram.Callee));
            }

            return pdg;
        }

        /// <summary>
        /// Deep copy constructor for PDG
        /// </summary>
        /// <param name="pdg">Original PDG</param>
        /// <returns>New PDG</returns>
        public static PDG DeepCopy(PDG pdg)
        {
            PDG pdgCopy = new PDG();
            pdgCopy._nodes = new SerializableDictionary<string,PDGNode>();
            foreach (KeyValuePair<string, PDGNode> kvpNode in pdg._nodes)
            {
                PDGNode nodeCopy = new PDGNode(kvpNode.Value.MethodName);
                foreach (PDGEdge edge in kvpNode.Value.OutgoingEdges)
                {
                    nodeCopy.SetEdgeWeight(edge.OutgoingNodeID, edge.Weight);
                }
                pdgCopy.Add(nodeCopy);
            }
            return pdgCopy;
        }

        #endregion

        private SerializableDictionary<string, PDGNode> _nodes;
        private List<string> _mapping;
        private SerializableDictionary<string, int> _indexes;

        /// <summary>
        /// Enumeration of nodes in the PDG
        /// </summary>
        public IEnumerable<PDGNode> Nodes
        {
            get
            {
                return _nodes.Values;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PDG()
        {
            _nodes = new SerializableDictionary<string, PDGNode>();
            _mapping = new List<string>();
            _indexes = new SerializableDictionary<string, int>();
        }

        /// <summary>
        /// Adds a node to the PDG
        /// </summary>
        /// <param name="pdgNode">Node to add</param>
        public void Add(PDGNode pdgNode)
        {
            _nodes.Add(pdgNode.MethodName, pdgNode);
            _mapping.Add(pdgNode.MethodName);
            _indexes.Add(pdgNode.MethodName, _mapping.Count - 1);
        }

        /// <summary>
        /// Gets a node based on its id
        /// </summary>
        /// <param name="node">Node id</param>
        /// <returns>Node object</returns>
        public PDGNode GetNode(string node)
	    {
            PDGNode pdgNode;
            _nodes.TryGetValue(node, out pdgNode);
		    return pdgNode;
	    }

        /// <summary>
        /// Gets a node based on its index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Node object</returns>
        public PDGNode GetNode(int index)
        {
            return _nodes[_mapping[index]];
        }

        /// <summary>
        /// Gets index of node based on node id
        /// </summary>
        /// <param name="nodeID">Node id</param>
        /// <returns>Index of node</returns>
        public int IndexOf(string nodeID)
        {
            if (!_indexes.ContainsKey(nodeID))
            {
                return -2;
            }
            return _indexes[nodeID];
        }

        /// <summary>
        /// Prints a human-readable format PDG representation
        /// </summary>
        /// <returns>PDG string</returns>
        public string Print()
	    {
		    StringBuilder sb = new StringBuilder("===PDG BEGIN===");
            sb.AppendLine();
		    foreach (PDGNode node in Nodes)
		    {
                sb.AppendLine(node.ToString());
		    }
		    sb.AppendLine("===PDG END===");
            return sb.ToString();
	    }
    }
}
