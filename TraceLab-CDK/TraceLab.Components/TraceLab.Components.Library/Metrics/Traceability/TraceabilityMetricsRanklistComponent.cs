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
using TraceLab.Components.DevelopmentKit.Metrics;
using TraceLab.Components.DevelopmentKit.Metrics.Traceability;
using TraceLab.Components.Library.Metrics.Controller;
using TraceLabSDK;
using TraceLabSDK.Types;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.Library.Metrics.Traceability
{
    [Component(Name = "Traceability Metrics - Ranklist granularity",
        Description = "Computes various traceability metrics on a similarity matrix at the ranklist granularity.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(TraceabilityMetricsRanklistConfig))]
    [IOSpec(IOSpecType.Input, "CandidateMatrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Metrics.Traceability")]
    public class TraceabilityMetricsRanklistComponent : BaseComponent
    {
        private TraceabilityMetricsRanklistConfig _config;

        public TraceabilityMetricsRanklistComponent(ComponentLogger log)
            : base(log)
        {
            _config = new TraceabilityMetricsRanklistConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Logger.Trace("Starting metrics computation: " + _config.TechniqueName);
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)Workspace.Load("CandidateMatrix");
            TLSimilarityMatrix oracle = (TLSimilarityMatrix)Workspace.Load("AnswerMatrix");
            TLExperimentResults exResults = new TLExperimentResults(_config.TechniqueName);
            #region Precision
            if (_config.Precision)
            {
                Logger.Trace("Computing precision...");
                IMetricComputation computation = new PrecisionRanklistComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped precision computation.");
            }
            #endregion
            #region Recall
            if (_config.Recall)
            {
                Logger.Trace("Computing recall...");
                IMetricComputation computation = new RecallRanklistComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped recall computation.");
            }
            #endregion
            #region Average Precision
            if (_config.AveragePrecision)
            {
                Logger.Trace("Computing average precision...");
                IMetricComputation computation = new AveragePrecisionRanklistComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped average precision computation.");
            }
            #endregion
            #region PR curve
            if (_config.PRCurve)
            {
                Logger.Trace("Computing precision-recall curve...");
                IMetricComputation computation = new PrecisionRecallCurveComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped precision-recall curve computation.");
            }
            #endregion
        }
    }
}
