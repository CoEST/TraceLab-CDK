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

namespace TraceLab.Components.DevelopmentKit.Tracers.WebMining
{
    /// <summary>
    /// Calculates HITS for an execution trace
    /// </summary>
    public class HITSScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "HITS.R";
        private readonly string[] _requiredPackages = new string[] { "base" };

        private PDG _pdg;
        private string _mappingFile;
        private string _authorityFile;
        private string _hubFile;
        private HITSConfig _config;
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
        public HITSScript(string traceID, PDG pdg, HITSConfig config) : base()
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
            string matrixFile = GenerateAdjacencyMatrix();
            _authorityFile = RUtil.ReserveCacheFile("HITS." + _traceID + ".authorities");
            _hubFile = RUtil.ReserveCacheFile("HITS." + _traceID + ".hubs");
            _arguments = new List<object>();
            _arguments.Add(matrixFile);
            _arguments.Add(_authorityFile);
            _arguments.Add(_hubFile);
            _arguments.Add(_config.Epsilon);
        }

        /// <summary>
        /// Imports script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            IEnumerable<double> authorities = Generics.ImportDoubles(_authorityFile, false);
            IEnumerable<double> hubs = Generics.ImportDoubles(_hubFile, false);
            IEnumerable<string> map = Generics.ImportStrings(_mappingFile);
            HITSResult results = new HITSResult();
            for (int i = 0; i < map.Count(); i++)
            {
                results.Hubs.AddLink(_traceID, map.ElementAt(i), hubs.ElementAt(i));
                results.Authorities.AddLink(_traceID, map.ElementAt(i), authorities.ElementAt(i));
            }
            return results;
        }

        #region Private methods

        private string GenerateAdjacencyMatrix()
        {
            int n = _pdg.Nodes.Count();
		    double defaultValue = 1.0 / n;
		    double[] rowValues = new double[n];

            FileStream matrixFS = RUtil.CreateCacheFile("HITS." + _traceID + ".TPM.matrix");
            TextWriter matrixWriter = new StreamWriter(matrixFS);

            //FileStream edgeFS = RUtil.CreateCacheFile("HITS." + _traceID + ".TPM.edges");
            //TextWriter edgeWriter = new StreamWriter(edgeFS);

            FileStream mapFS = RUtil.CreateCacheFile("HITS." + _traceID + ".TPM.map");
            _mappingFile = mapFS.Name;
            TextWriter mapWriter = new StreamWriter(mapFS);

		    for (int nodeIndex = 0; nodeIndex < _pdg.Nodes.Count(); nodeIndex++)
		    {
                PDGNode pdgNode = _pdg.GetNode(nodeIndex);

			    for (int i = 0; i < n; i++)
				{
					rowValues[i] = 0;
				}
			
			    //edgeWriter.WriteLine(pdgNode.OutgoingEdges.Count());	// write number of outgoing edges for Topical HITS algorithm

			    for (int indexOutgoingEdge = 0; indexOutgoingEdge < pdgNode.OutgoingEdges.Count(); indexOutgoingEdge++)
			    {
				    PDGEdge pdgOutgoingEdge = pdgNode.OutgoingEdges.ElementAt(indexOutgoingEdge);
				    int columnFrequencies = _pdg.IndexOf(pdgOutgoingEdge.OutgoingNodeID);

				    // for positive values only
				    if ((columnFrequencies < 0))
				    {
					    throw new RDataException();
					    // continue;
				    }
                    if (_config.Weight == WebMiningWeightEnum.Binary)
                    {
                        rowValues[columnFrequencies] = 1;
                    }
                    else
                    if (_config.Weight == WebMiningWeightEnum.Frequency)
                    {
                        rowValues[columnFrequencies] = pdgOutgoingEdge.Weight;
                    }
                    else
                    {
                        throw new RDataException("Unknown weighting scheme: " + _config.Weight);
                    }
			    }

                //for (int i=1;i<=n;i++)
                //{
                //    matrixWriter.Write(rowValuesFrequencies[i]+" ");
                //    binaryWriter.Write(rowValuesBinary[i]+" ");
                //}
			    matrixWriter.WriteLine(String.Join(" ", rowValues));
                mapWriter.WriteLine(pdgNode.MethodName);
		    }

            matrixWriter.Flush();
            matrixWriter.Close();
            //edgeWriter.Flush();
            //edgeWriter.Close();
            mapWriter.Flush();
            mapWriter.Close();
            return matrixFS.Name;
        }

        #endregion
    }
}
