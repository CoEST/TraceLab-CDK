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
    /// Computes mean average precision
    /// </summary>
    public class MeanAveragePrecisionComputation : MetricComputation
    {
        #region Members

        private const string _name = "Mean Average Precision";
        private const string _description = "Mean average precision measures average of the average precision values for each correct link per query.";

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
        public MeanAveragePrecisionComputation(TLSimilarityMatrix candidateMatrix, TLSimilarityMatrix answerMatrix)
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
            IMetricComputation avgP = new AveragePrecisionQueryComputation(_matrix, _oracle);
            avgP.Compute();
            Results.Add("MeanAveragePrecision", avgP.Results.Values.Average());
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
