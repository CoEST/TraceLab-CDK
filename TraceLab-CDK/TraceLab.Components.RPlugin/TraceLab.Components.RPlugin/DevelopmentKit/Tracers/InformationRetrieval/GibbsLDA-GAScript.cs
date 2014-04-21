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
using System.Linq;
using System.Reflection;
using System.Text;
using TraceLab.Components.RPlugin.Properties;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Computes ideal parameters for LDA using a genetic algorithm
    /// </summary>
    public class GibbsLDA_GAScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "GibbsLDA-GA.R";
        private readonly string[] _requiredPackages = new string[] { "GA", "slam", "tm", "topicmodels" };

        private TLArtifactsCollection _source;
        private TLArtifactsCollection _target;
        private GibbsLDA_GAConfig _config;
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
        public GibbsLDA_GAScript(TLArtifactsCollection source, TLArtifactsCollection target, GibbsLDA_GAConfig config)
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
            DirectoryInfo sourceInfo = SaveArtifactsToCache(_source, "GibbsLDA-GA.source");
            DirectoryInfo targetInfo = SaveArtifactsToCache(_target, "GibbsLDA-GA.target");
            _outputFile = RUtil.ReserveCacheFile("GibbsLDA-GA.out");
            _arguments = new List<object>();
            _arguments.Add(sourceInfo.FullName);
            _arguments.Add(targetInfo.FullName);
            _arguments.Add(_outputFile);
            _arguments.Add(100);
            _arguments.Add(_config.GibbsIterations);
            _arguments.Add(_config.Alpha);
            _arguments.Add(_config.Beta);
            _arguments.Add(_config.GA_Iterations);
            _arguments.Add(_config.Run);
            _arguments.Add(_config.PermutationRate);
            _arguments.Add(_config.Population);
            _arguments.Add(_config.Elitism);
            _arguments.Add(_config.Seed);
        }

        /// <summary>
        /// Returns an ideal GibbsLDA configuration object
        /// </summary>
        /// <param name="result">REngine results object</param>
        /// <returns>GibbsLDAConfig object</returns>
        public override object ImportResults(RScriptResult result)
        {
            TextReader resultFile = new StreamReader(_outputFile);
            string[] results = resultFile.ReadToEnd().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (results.Length != 4)
            {
                throw new RDataException("Results file in incorrect format (" + _outputFile + ")");
            }
            return new GibbsLDAConfig
            {
                NumTopics = Convert.ToInt32(Math.Round(Convert.ToDouble(results[0]))),
                GibbsIterations = Convert.ToInt32(Math.Round(Convert.ToDouble(results[1]))),
                Alpha = Convert.ToDouble(results[2]),
                Beta = Convert.ToDouble(results[3]),
                Seed = _config.Seed,
            };
        }

        /// <summary>
        /// Saves a TLArtifactsCollection to the cache
        /// </summary>
        /// <param name="artifacts"></param>
        /// <param name="name"></param>
        private DirectoryInfo SaveArtifactsToCache(TLArtifactsCollection artifacts, string name)
        {
            DirectoryInfo info = RUtil.CreateCacheDirectory(name);
            foreach (TLArtifact artifact in artifacts.Values)
            {
                TextWriter tw = new StreamWriter(Path.Combine(info.FullName, artifact.Id));
                tw.Write(artifact.Text);
                tw.Flush();
                tw.Close();
            }
            return info;
        }
    }
}
