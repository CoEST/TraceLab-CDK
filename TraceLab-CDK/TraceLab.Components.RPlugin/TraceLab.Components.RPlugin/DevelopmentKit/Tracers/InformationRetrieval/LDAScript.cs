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
using TraceLab.Components.RPlugin.Properties;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Computes Latent Dirichlet Allocation using package `lda`
    /// </summary>
    public class LDAScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "LDA.R";
        private readonly string[] _requiredPackages = new string[] { "lda" };

        private TermDocumentMatrix _source;
        private TermDocumentMatrix _target;
        private LDAConfig _config;
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
        public LDAScript(TLArtifactsCollection source, TLArtifactsCollection target, LDAConfig config) : base()
        {
            _source = new TermDocumentMatrix(source);
            _target = new TermDocumentMatrix(target);
            _config = config;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Source artifacts</param>
        /// <param name="target">Target artifacts</param>
        /// <param name="config">Configuration object</param>
        public LDAScript(TermDocumentMatrix source, TermDocumentMatrix target, LDAConfig config) : base()
        {
            _source = source;
            _target = target;
            _config = config;
        }

        /// <summary>
        /// Precompute method
        /// </summary>
        public override void PreCompute()
        {
            RUtil.RegisterScript(Assembly.GetExecutingAssembly(), _baseScript);
            LDACorpus corpus = new LDACorpus("LDA", _source, _target);
            LDACorpusInfo info = corpus.Save();
            _outputFile = RUtil.ReserveCacheFile("LDA.out");
            _arguments = new List<object>();
            _arguments.Add(info.Corpus);
            _arguments.Add(info.Vocab);
            _arguments.Add(info.Edges);
            _arguments.Add(_outputFile);
            _arguments.Add(_config.NumTopics);
            _arguments.Add(_config.NumIterations);
            _arguments.Add(_config.Alpha);
            _arguments.Add(_config.Eta);
            _arguments.Add(_config.PredictionBeta);
            _arguments.Add(_config.Seed);
        }

        /// <summary>
        /// Imports script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            TextReader rfile = new StreamReader(_outputFile);
            string rawdata = rfile.ReadToEnd();
            rfile.Close();
            TLSimilarityMatrix matrix = new TLSimilarityMatrix();
            string[] sims = rawdata.Remove(0,2).Replace(")", String.Empty).Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            int src = 0;
            int tgt = _source.DocMap.Count;
            if (sims.Length != _source.DocMap.Count * _target.DocMap.Count)
            {
                throw new RDataException("Results are incorrect size: " + sims.Length + " vs " + (_source.DocMap.Count * _target.DocMap.Count));
            }
            foreach (string sim in sims)
            {
                matrix.AddLink(_source.DocMap[src], _target.DocMap[tgt - _source.DocMap.Count], Convert.ToDouble(sim.Trim()));
                tgt++;
                if (tgt == _source.DocMap.Count + _target.DocMap.Count)
                {
                    tgt = _source.DocMap.Count;
                    src++;
                }
            }
            return matrix;
        }
    }
}
