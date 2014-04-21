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

using RPlugin.Core;
using RPlugin.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.RPlugin.Properties;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Computes Relational Topic Model 
    /// </summary>
    public class RTMScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "RTM.R";
        private readonly string[] _requiredPackages = new string[] { "lda" };

        private LDACorpus _corpus;
        private RTMConfig _config;
        private LDACorpusInfo _info;
        private string _outputFile;

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
        /// <param name="source">Source artifacts</param>
        /// <param name="target">Target artifacts</param>
        /// <param name="config">Configuration object</param>
        public RTMScript(TLArtifactsCollection source, TLArtifactsCollection target, RTMConfig config) : base()
        {
            _corpus = new LDACorpus("RTM", new TermDocumentMatrix(source), new TermDocumentMatrix(target));
            _config = config;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source artifacts</param>
        /// <param name="target">Target artifacts</param>
        /// <param name="config">Configuration object</param>
        public RTMScript(TermDocumentMatrix source, TermDocumentMatrix target, RTMConfig config) : base()
        {
            _corpus = new LDACorpus("RTM", source, target);
            _config = config;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <param name="sourceIDs">Source artifacts ids</param>
        /// <param name="targetIDs">Target artifacts ids</param>
        /// <param name="config">Configuration object</param>
        public RTMScript(TermDocumentMatrix matrix, IEnumerable<string> sourceIDs, IEnumerable<string> targetIDs, RTMConfig config)
        {
            _corpus = new LDACorpus("RTM", matrix, sourceIDs, targetIDs);
            _config = config;
        }

        /// <summary>
        /// Precompute method
        /// </summary>
        public override void PreCompute()
        {
            RUtil.RegisterScript(Assembly.GetExecutingAssembly(), _baseScript);
            _info = _corpus.Save();
            _outputFile = RUtil.ReserveCacheFile("RTM.out");
            _arguments = new List<object>();
            _arguments.Add(_info.Corpus);
            _arguments.Add(_info.Vocab);
            _arguments.Add(_info.Edges);
            _arguments.Add(_info.Links);
            _arguments.Add(_outputFile);
            _arguments.Add(_config.NumTopics);
            _arguments.Add(_config.NumIterations);
            _arguments.Add(_config.Alpha);
            _arguments.Add(_config.Eta);
            _arguments.Add(_config.RTMBeta);
            _arguments.Add(_config.PredictionBeta);
            _arguments.Add(_config.Seed);
        }

        /// <summary>
        /// Import script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            TextReader rfile = new StreamReader(_outputFile);
            string rawdata = rfile.ReadToEnd();
            rfile.Close();
            TLSimilarityMatrix matrix = new TLSimilarityMatrix();
            string[] sims = rawdata.Remove(0, 2).Replace(")", String.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] edges = Generics.ImportStrings(_info.Edges);
            if (sims.Length != edges.Length)
                throw new RDataException("Results are incorrect size: " + sims.Length + " vs " + edges.Length);
            for (int i = 0; i < sims.Length; i++)
            {
                string[] split = edges[i].Split();
                matrix.AddLink(_corpus.Map[Convert.ToInt32(split[0])], _corpus.Map[Convert.ToInt32(split[1])], Convert.ToDouble(sims[i]));
            }
            //int src = 0;
            //int tgt = _source.DocMap.Count;
            //if (sims.Length != _source.DocMap.Count * _target.DocMap.Count)
            //{
            //    throw new RDataException("Results are incorrect size: " + sims.Length + " vs " + (_source.DocMap.Count * _target.DocMap.Count));
            //}
            //foreach (string sim in sims)
            //{
            //    matrix.AddLink(_source.DocMap[src], _target.DocMap[tgt - _source.DocMap.Count], Convert.ToDouble(sim.Trim()));
            //    tgt++;
            //    if (tgt == _source.DocMap.Count + _target.DocMap.Count)
            //    {
            //        tgt = _source.DocMap.Count;
            //        src++;
            //    }
            //}
            return matrix;
        }
    }
}
