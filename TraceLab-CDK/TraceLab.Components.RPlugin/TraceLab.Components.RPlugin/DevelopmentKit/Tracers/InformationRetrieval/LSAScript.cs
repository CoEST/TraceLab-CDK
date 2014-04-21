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
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.RPlugin.Properties;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Computes Latent Semantic Analysis (also known as Latent Semantic Indexing or LSI)
    /// </summary>
    public class LSAScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "LSA.R";
        private readonly string[] _requiredPackages = new string[] { "lsa" };

        private TLArtifactsCollection _source;
        private TLArtifactsCollection _target;
        private LSAConfig _config;
        private string _outputFile;
        private string _SourceFile;
        private string _TargetFile;
        private string _mapFile;

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
        public LSAScript(TLArtifactsCollection source, TLArtifactsCollection target, LSAConfig config) : base()
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
            CheckParameters();
            _outputFile = RUtil.ReserveCacheFile("LSA.out");
            DirectoryInfo corpusDir = SaveArtifactsToCache(_source, _target, "LSA.corpus");
            _arguments = new List<object>();
            _arguments.Add(corpusDir.FullName);
            _arguments.Add(_SourceFile);
            _arguments.Add(_TargetFile);
            _arguments.Add(_outputFile);
            _arguments.Add(_config.Dimensions);
        }

        /// <summary>
        /// Import script results
        /// </summary>
        /// <param name="result">RScriptResults object</param>
        /// <returns>Script results</returns>
        public override object ImportResults(RScriptResult result)
        {
            // index = id - 1
            string[] ids = Generics.ImportStrings(_mapFile);
            TextReader resultsMatrix = new StreamReader(_outputFile);
            TLSimilarityMatrix matrix = new TLSimilarityMatrix();
            string[] sources = resultsMatrix.ReadLine().Split();
            string line;
            while ((line = resultsMatrix.ReadLine()) != null)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;
                // [0] target id, [x+] source sims index = x - 1
                string[] entries = line.Split();
                string entry = ids[Convert.ToInt32(entries[0]) - 1];
                for (int i = 0; i < sources.Length; i++)
                {
                    matrix.AddLink(ids[Convert.ToInt32(sources[i]) - 1], entry, Convert.ToDouble(entries[i + 1]));
                }
            }
            resultsMatrix.Close();
            return matrix;
        }

        private void CheckParameters()
        {
            if (_config.Dimensions < 1)
            {
                throw new RDataException("Dimensions (" + _config.Dimensions + ") cannot be less than 1.");
            }
            if (_config.Dimensions > _source.Count + _target.Count)
            {
                throw new RDataException("Dimensions (" + _config.Dimensions + ") cannot be greater than the number of documents (" + (_source.Count + _target.Count) + ").");
            }
        }

        private DirectoryInfo SaveArtifactsToCache(TLArtifactsCollection source, TLArtifactsCollection target, string name)
        {
            DirectoryInfo infoDir = RUtil.CreateCacheDirectory(name);
            FileStream sFile = RUtil.CreateCacheFile("LSA.corpus.source");
            TextWriter sourceWriter = new StreamWriter(sFile);
            _SourceFile = sFile.Name;
            FileStream tFile = RUtil.CreateCacheFile("LSA.corpus.target");
            TextWriter targetWriter = new StreamWriter(tFile);
            _TargetFile = tFile.Name;
            FileStream mFile = RUtil.CreateCacheFile("LSA.corpus.map");
            TextWriter mapWriter = new StreamWriter(mFile);
            _mapFile = mFile.Name;

            int fileIndex = 1;

            foreach (TLArtifact artifact in source.Values)
            {
                TextWriter tw = new StreamWriter(Path.Combine(infoDir.FullName, fileIndex.ToString()));
                tw.Write(artifact.Text);
                tw.Flush();
                tw.Close();
                sourceWriter.WriteLine(fileIndex);
                mapWriter.WriteLine(artifact.Id);
                fileIndex++;
            }
            sourceWriter.Flush();
            sourceWriter.Close();

            foreach (TLArtifact artifact in target.Values)
            {
                TextWriter tw = new StreamWriter(Path.Combine(infoDir.FullName, fileIndex.ToString()));
                tw.Write(artifact.Text);
                tw.Flush();
                tw.Close();
                targetWriter.WriteLine(fileIndex);
                mapWriter.WriteLine(artifact.Id);
                fileIndex++;
            }
            targetWriter.Flush();
            targetWriter.Close();

            mapWriter.Flush();
            mapWriter.Close();

            return infoDir;
        }
    }
}
