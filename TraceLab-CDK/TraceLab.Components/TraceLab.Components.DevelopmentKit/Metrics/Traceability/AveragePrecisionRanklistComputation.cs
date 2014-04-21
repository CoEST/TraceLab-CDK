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

namespace TraceLab.Components.DevelopmentKit.Metrics.Traceability
{
    /// <summary>
    /// Computes average precision at ranklist granularity
    /// </summary>
    public class AveragePrecisionRanklistComputation : MetricComputation
    {
        #region Members

        private const string _name = "Average Precision (ranklist granularity)";
        private const string _description = "Average precision measures average of the precision values for each correct link. This metric is calculated on the total ranklist.";

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
        /// Constructor
        /// </summary>
        /// <param name="candidateMatrix">Similarities</param>
        /// <param name="answerMatrix">Oracle</param>
        public AveragePrecisionRanklistComputation(TLSimilarityMatrix candidateMatrix, TLSimilarityMatrix answerMatrix)
            : base()
        {
            _matrix = candidateMatrix;
            _oracle = answerMatrix;
        }

        /// <summary>
        /// Called from MetricComputation
        /// </summary>
        protected override void ComputeImplementation()
        {
            Results = new SerializableDictionary<string, double>();
            double sumOfPrecisions = 0.0;
            int currentLink = 0;
            int correctSoFar = 0;
            TLLinksList links = _matrix.AllLinks;
            links.Sort();
            foreach (TLSingleLink link in links)
            {
                currentLink++;
                if (_oracle.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId))
                {
                    correctSoFar++;
                    sumOfPrecisions += correctSoFar / (double)currentLink;
                }
            }
            Results.Add("AveragePrecision", sumOfPrecisions / _oracle.AllLinks.Count);
        }

        /// <summary>
        /// Called from MetricComputation
        /// </summary>
        protected override Metric GenerateSummaryImplementation()
        {
            BoxSummaryData data = new BoxSummaryData(Name, Description);
            data.AddPoint(new BoxPlotPoint(Results.Values.ToArray()));
            return data;
        }
    }
}
