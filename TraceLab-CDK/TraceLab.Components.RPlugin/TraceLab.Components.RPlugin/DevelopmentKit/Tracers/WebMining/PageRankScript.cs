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
using System.IO;
using System.Linq;
using System.Reflection;
using RPlugin.Core;
using RPlugin.Exceptions;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.RPlugin.Properties;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using TraceLab.Components.Types.Tracers.WebMining;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Tracers.WebMining
{
    /// <summary>
    /// Computes PageRank for execution traces
    /// </summary>
    public class PageRankScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "PageRank.R";
        private readonly string[] _requiredPackages = new string[] { "base" };

        private PDG _pdg;
        private string _mappingFile;
        private string _outputFile;
        private PageRankConfig _config;
        private string _traceID;

        /// <summary>
        /// Script name
        /// </summary>
        public override string BaseScript
        {
            get
            {
                return _baseScript;
            }
        }

        /// <summary>
        /// Required Packages
        /// </summary>
        public override string[] RequiredPackages
        {
            get
            {
                return _requiredPackages;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="traceID">Trace id</param>
        /// <param name="pdg">Program dependency graph</param>
        /// <param name="config">Configuration object</param>
        public PageRankScript(string traceID, PDG pdg, PageRankConfig config) : base()
        {
            _pdg = pdg;
            _config = config;
            _traceID = traceID;
        }

        /// <summary>
        /// Precompute method
        /// </summary>
        public override void PreCompute()
        {
            RUtil.RegisterScript(Assembly.GetExecutingAssembly(), _baseScript);

            switch (_config.Weight)
            {
                case WebMiningWeightEnum.Binary:
                    NormalizeEdgeBinary();
                    break;
                case WebMiningWeightEnum.Frequency:
                    NormalizeEdgeFrequencies();
                    break;
                default:
                    throw new RExecutionException("Unknown weighting scheme: " + _config.Weight);
            }

            string matrixFile = GenerateTransitionProbabilityMatrix();
            _outputFile = RUtil.ReserveCacheFile("PageRank" + _traceID + ".out");
            _arguments = new List<object>();
            _arguments.Add(matrixFile);
            _arguments.Add(_outputFile);
            _arguments.Add(_config.Beta);
            _arguments.Add(_config.Epsilon);
        }

        /// <summary>
        /// Import script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            IEnumerable<double> ranks = Generics.ImportDoubles(_outputFile, false);
            IEnumerable<string> map = Generics.ImportStrings(_mappingFile);
            if (ranks.Count() != map.Count())
            {
                throw new RDataException("Results file in incorrect format: incorrect number of entries");
            }
            TLSimilarityMatrix rankList = new TLSimilarityMatrix();
            for (int i = 0; i < map.Count(); i++)
            {
                rankList.AddLink(_traceID, map.ElementAt(i), ranks.ElementAt(i));
            }
            return rankList;
        }

        #region Private methods

        private void NormalizeEdgeFrequencies()
        {
            // nodes
            foreach (PDGNode node in _pdg.Nodes)
            {
                double sumNodeFrequencies = 0.0;
                // node outgoing edges
                foreach (PDGEdge edge in node.OutgoingEdges)
                {
                    sumNodeFrequencies += edge.Weight;
                }
                foreach (PDGEdge edge in node.OutgoingEdges)
                {
                    edge.Weight = edge.Weight / sumNodeFrequencies;
                }
            }
        }

        private void NormalizeEdgeBinary()
        {
            // nodes
            foreach (PDGNode node in _pdg.Nodes)
            {
                double binaryWeight = node.OutgoingEdges.Count();
                if (!binaryWeight.Equals(0.0))
                {
                    binaryWeight = 1.0 / binaryWeight;
                }
                foreach (PDGEdge edge in _pdg.GetNode(node.MethodName).OutgoingEdges)
                {
                    edge.Weight = binaryWeight;
                }
            }
        }

        private string GenerateTransitionProbabilityMatrix()
        {
            int n = _pdg.Nodes.Count();
		    double defaultValue = 1.0 / n;
		    double[] rowValues = new double[n];

            FileStream matrixFS = RUtil.CreateCacheFile("PageRank." + _traceID + ".TPM.matrix");
            TextWriter matrixWriter = new StreamWriter(matrixFS);

            FileStream edgeFS = RUtil.CreateCacheFile("PageRank." + _traceID + ".TPM.edges");
            TextWriter edgeWriter = new StreamWriter(edgeFS);

            FileStream mapFS = RUtil.CreateCacheFile("PageRank." + _traceID + ".TPM.map");
            _mappingFile = mapFS.Name;
            TextWriter mapWriter = new StreamWriter(mapFS);

		    for (int nodeIndex = 0; nodeIndex < _pdg.Nodes.Count(); nodeIndex++)
		    {
                PDGNode pdgNode = _pdg.GetNode(nodeIndex);

			    if (pdgNode.OutgoingEdges.Count() == 0)
			    {
				    for (int i = 0; i < n; i++)
				    {
					    rowValues[i] = defaultValue;
				    }
			    }
			    else
			    {
				    for (int i = 0; i < n; i++)
				    {
					    rowValues[i] = 0.0;
				    }
			    }
			
			    edgeWriter.WriteLine(pdgNode.OutgoingEdges.Count());	// write number of outgoing edges for most of the advanced PageRank algorithms

			    for (int indexOutgoingEdge = 0; indexOutgoingEdge < pdgNode.OutgoingEdges.Count(); indexOutgoingEdge++)
			    {
				    PDGEdge pdgOutgoingEdge = pdgNode.OutgoingEdges.ElementAt(indexOutgoingEdge);
				    int columnFrequencies =  _pdg.IndexOf(pdgOutgoingEdge.OutgoingNodeID);
				    // for positive values only
				    if (columnFrequencies == -1)
				    {
					    throw new RDataException("Invalid column index.");
					    // continue;
				    }
				    rowValues[columnFrequencies] = pdgOutgoingEdge.Weight;
			    }

                //for (int i=1;i<=n;i++)
                //{
                //    matrixWriter.Write(rowValues[i]+" ");
                //}
			    matrixWriter.WriteLine(String.Join(" ", rowValues));
                mapWriter.WriteLine(pdgNode.MethodName);
		    }

            matrixWriter.Flush();
            matrixWriter.Close();
            edgeWriter.Flush();
            edgeWriter.Close();
            mapWriter.Flush();
            mapWriter.Close();
            return matrixFS.Name;
        }

        #endregion
    }
}
