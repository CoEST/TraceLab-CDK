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

using System.Linq;
using TraceLabSDK;
using TraceLabSDK.Types;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.DevelopmentKit.Metrics.Traceability
{
    /// <summary>
    /// Computes precision of returned links for each source artifact
    /// </summary>
    public class PrecisionQueryComputation : MetricComputation
    {
        #region Members

        private const string _name = "Precision (query granularity)";
        private const string _description = "Precision measures the fraction of correctly retrieved documents among the retrieved documents. This metric is calculated per source artifact.";

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
        /// Constructor for the precision computation (query granularity)
        /// </summary>
        /// <param name="candidateMatrix">Candidate links</param>
        /// <param name="answerMatrix">Answer matrix</param>
        public PrecisionQueryComputation(TLSimilarityMatrix candidateMatrix, TLSimilarityMatrix answerMatrix)
            : base()
        {
            _matrix = candidateMatrix;
            _oracle = answerMatrix;
        }

        /// <summary>
        /// Computes the precision of each source artifact in the similarity matrix using the answer matrix provided.
        /// </summary>
        protected override void ComputeImplementation()
        {
            SerializableDictionary<string, double> sourcePrecisions = new SerializableDictionary<string, double>();
            _oracle.Threshold = 0;
            foreach (string sourceArtifact in _oracle.SourceArtifactsIds)
            {
                TLLinksList links = _matrix.GetLinksAboveThresholdForSourceArtifact(sourceArtifact);
                int correct = 0;
                foreach (TLSingleLink link in links)
                {
                    if (_oracle.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId))
                    {
                        correct++;
                    }
                }
                sourcePrecisions.Add(sourceArtifact, correct / (double)links.Count);
            }
            Results = sourcePrecisions;
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
