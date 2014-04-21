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
    /// Computes precision and recall for each link in ranklist.
    /// Results can be graphed as a curve.
    /// </summary>
    public class PrecisionRecallCurveComputation : MetricComputation
    {
        #region Members

        private const string _name = "Precision-Recall Curve";
        private const string _description = "Computes the precision and recall at every cutpoint in the ranklist. This is an inherent ranklist granularity (even when used in the query component).";

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
        private string _precisionFormat;
        private string _recallFormat;

        #endregion

        /// <summary>
        /// Constructor for the precision-recall curve computation
        /// </summary>
        /// <param name="candidateMatrix">Candidate links</param>
        /// <param name="answerMatrix">Answer matrix</param>
        public PrecisionRecallCurveComputation(TLSimilarityMatrix candidateMatrix, TLSimilarityMatrix answerMatrix)
            : base()
        {
            _matrix = candidateMatrix;
            _oracle = answerMatrix;
            _precisionFormat = String.Format("{{0:D{0}}}_Precision", Math.Floor(Math.Log10(candidateMatrix.Count)));
            _recallFormat = String.Format("{{0:D{0}}}_Recall", Math.Floor(Math.Log10(candidateMatrix.Count)));
        }

        /// <summary>
        /// Computes the precision-recall curve of the given similarity matrix using the answer matrix provided.
        /// </summary>
        protected override void ComputeImplementation()
        {
            _oracle.Threshold = 0;
            int correct = 0;
            TLLinksList links = _matrix.AllLinks;
            links.Sort();
            Results = new SerializableDictionary<string, double>();
            for (int linkNumber = 1; linkNumber <= links.Count; linkNumber++)
            {
                if (_oracle.IsLinkAboveThreshold(links[linkNumber - 1].SourceArtifactId, links[linkNumber - 1].TargetArtifactId))
                {
                    correct++;
                }
                Results.Add(String.Format(_precisionFormat, linkNumber), correct / (double)linkNumber);
                Results.Add(String.Format(_recallFormat, linkNumber), correct / (double)_oracle.Count);
            }
        }

        /// <summary>
        /// Generates a summarizing Metric for results analysis
        /// </summary>
        /// <returns>Metric object</returns>
        protected override Metric GenerateSummaryImplementation()
        {
            LineSeries data = new LineSeries(_name, _description);
            for (int i = 1; i <= Results.Count / 2; i++)
            {
                double x, y;
                try
                {
                    x = Results[String.Format(_recallFormat, i)];
                    y = Results[String.Format(_precisionFormat, i)];
                }
                catch (KeyNotFoundException e)
                {
                    throw new DevelopmentKitException("Results in incorrect format.", e);
                }
                data.AddPoint(new Point(x, y));
            }
            return data;
        }
    }
}
