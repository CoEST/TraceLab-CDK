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
using System.Text;
using TraceLab.Components.RPlugin.Properties;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Metrics.Traceability
{
    /// <summary>
    /// Computes Principal Component Analysis for a collection of similarity matrices
    /// </summary>
    public class PCAScript : RScript
    {
        private readonly string _baseScript = Settings.Default.Resources + "PCA.R";
        private readonly string[] _requiredPackages = new string[] { "psych", "GPArotation" };

        private TLSimilarityMatrix[] _matrices;
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
        /// <param name="matrices">Input matrices</param>
        public PCAScript(params TLSimilarityMatrix[] matrices) : base()
        {
            _matrices = matrices;
        }

        /// <summary>
        /// Precompute method
        /// </summary>
        public override void PreCompute()
        {
            RUtil.RegisterScript(Assembly.GetExecutingAssembly(), _baseScript);
            string tableFile = CreateTable(_matrices);
            _outputFile = RUtil.ReserveCacheFile("PCA.out");
            _arguments = new List<object>();
            _arguments.Add(tableFile);
            _arguments.Add(_outputFile);
        }

        /// <summary>
        /// Returns an array containing the contributions of each matrix in the order they were added
        /// </summary>
        /// <param name="result">REngine result object</param>
        /// <returns>double[]</returns>
        public override object ImportResults(RScriptResult result)
        {
            List<double> pca = new List<double>();
            TextReader file = new StreamReader(_outputFile);
            file.ReadLine();
            string line = String.Empty;
            while ((line = file.ReadLine()) != null)
            {
                string[] entries = line.Split(',');
                if (entries.Length != 3)
                {
                    throw new RDataException("Error in PCA output.");
                }
                pca.Add(Convert.ToDouble(entries[2]));
            }
            if (pca.Count != _matrices.Length)
            {
                throw new RDataException("PCA returned the incorrect number of entries.");
            }
            return pca.ToArray();
        }

        /// <summary>
        /// Creates a table of results for input into PCA
        /// </summary>
        /// <param name="matrices">Array of matrices</param>
        /// <returns>Table in R format</returns>
        private string CreateTable(params TLSimilarityMatrix[] matrices)
        {
            if (matrices.Length < 2)
            {
                throw new RDataException("Must have at least 2 matrices.");
            }
            FileStream tableFile = RUtil.CreateCacheFile("PCA.table");
            TextWriter tableWriter = new StreamWriter(tableFile);
            tableWriter.Write("M1");
            for (int i = 1; i < matrices.Length; i++)
            {
                if (matrices[i].Count != matrices[0].Count)
                {
                    throw new RDataException("Matrices have different count of links.");
                }
                tableWriter.Write(String.Format("\tM{0}", i + 1));
            }
            tableWriter.WriteLine();
            foreach (TLSingleLink link in matrices[0].AllLinks)
            {
                tableWriter.Write(String.Format("{0}_{1}\t{2}",
                    link.SourceArtifactId,
                    link.TargetArtifactId,
                    link.Score
                ));
                for (int i = 1; i < matrices.Length; i++)
                {
                    tableWriter.Write(String.Format("\t{0}", matrices[i].GetScoreForLink(link.SourceArtifactId, link.TargetArtifactId)));
                }
                tableWriter.WriteLine();
            }
            tableWriter.Flush();
            tableWriter.Close();
            return tableFile.Name;
        }
    }
}
