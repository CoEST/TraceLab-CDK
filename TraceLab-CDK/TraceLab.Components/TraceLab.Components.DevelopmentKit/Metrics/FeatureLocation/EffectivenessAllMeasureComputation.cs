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
using TraceLabSDK.Types;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.DevelopmentKit.Metrics.FeatureLocation
{
    /// <summary>
    /// Computes effectiveness measures for gold set
    /// </summary>
    public class EffectivenessAllMeasureComputation : MetricComputation
    {
        #region Members

        private const string _name = "Effectiveness All Measure";
        private const string _description = "Computes the index position of correct links. This metric is calculated per query.";

        /// <summary>
        /// Metric name
        /// </summary>
        public override string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Metric description
        /// </summary>
        public override string Description
        {
            get { return _description; }
        }

        private TLSimilarityMatrix _matrix;
        private TLSimilarityMatrix _oracle;

        #endregion

        /// <summary>
        /// Constructor for the effectiveness all measure computation
        /// </summary>
        /// <param name="candidateMatrix">Candidate links</param>
        /// <param name="answerMatrix">Answer matrix</param>
        public EffectivenessAllMeasureComputation(TLSimilarityMatrix candidateMatrix, TLSimilarityMatrix answerMatrix)
            : base()
        {
            _matrix = candidateMatrix;
            _oracle = answerMatrix;
        }

        /// <summary>
        /// Computes the effectiveness all measure of the given similarity matrix using the answer matrix provided.
        /// </summary>
        protected override void ComputeImplementation()
        {
            _oracle.Threshold = 0;
            Results = new SerializableDictionary<string, double>();
            foreach (string query in _oracle.SourceArtifactsIds)
            {
                TLLinksList links = _matrix.GetLinksAboveThresholdForSourceArtifact(query);
                links.Sort();
                for (int i = 0; i < links.Count; i++)
                {
                    if (_oracle.IsLinkAboveThreshold(query, links[i].TargetArtifactId))
                    {
                        Results.Add(String.Format("{0}_{1}", query, links[i].TargetArtifactId), i);
                    }
                }
            }
        }

        /// <summary>
        /// Generates a summarizing Metric for results analysis
        /// </summary>
        /// <returns>Metric object</returns>
        protected override Metric GenerateSummaryImplementation()
        {
            BoxSummaryData data = new BoxSummaryData(Name, Description);
            data.AddPoint(new BoxPlotPoint(Results.Values.ToArray()));
            return data;
        }
    }
}
