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
    /// Computes Latent Dirichlet Allocation using package `topicmodels`
    /// </summary>
    public class GibbsLDAScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "GibbsLDA.R";
        private readonly string[] _requiredPackages = new string[] { "slam", "tm", "topicmodels" };

        private TLArtifactsCollection _source;
        private TLArtifactsCollection _target;
        private GibbsLDAConfig _config;
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
        public GibbsLDAScript(TLArtifactsCollection source, TLArtifactsCollection target, GibbsLDAConfig config) : base()
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
            DirectoryInfo sourceInfo = SaveArtifactsToCache(_source, "GibbsLDA.source");
            DirectoryInfo targetInfo = SaveArtifactsToCache(_target, "GibbsLDA.target");
            _outputFile = RUtil.ReserveCacheFile("GibbsLDA.out");
            _arguments = new List<object>();
            _arguments.Add(sourceInfo.FullName);
            _arguments.Add(targetInfo.FullName);
            _arguments.Add(_outputFile);
            _arguments.Add(_config.NumTopics);
            _arguments.Add(_config.GibbsIterations);
            _arguments.Add(_config.Alpha);
            _arguments.Add(_config.Beta);
            _arguments.Add(_config.Seed);
        }

        /// <summary>
        /// Import script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            return Similarities.Import(_outputFile);
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
