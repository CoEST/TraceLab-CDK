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

namespace TraceLab.Components.Library.Metrics.Traceability
{
    [Component(Name = "Traceability Metrics - Query granularity",
        Description = "Computes various traceability metrics per source artifact in a similarity matrix.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(TraceabilityMetricsQueryConfig))]
    [IOSpec(IOSpecType.Input, "CandidateMatrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Metrics.Traceability")]
    public class TraceabilityMetricsQueryComponent : BaseComponent
    {
        private TraceabilityMetricsQueryConfig _config;

        public TraceabilityMetricsQueryComponent(ComponentLogger log)
            : base(log)
        {
            _config = new TraceabilityMetricsQueryConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Logger.Trace("Starting metrics computation: " + _config.TechniqueName);
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)Workspace.Load("CandidateMatrix");
            TLSimilarityMatrix oracle = (TLSimilarityMatrix)Workspace.Load("AnswerMatrix");
            #region Precision
            if (_config.Precision)
            {
                Logger.Trace("Computing precision...");
                IMetricComputation computation = new PrecisionQueryComputation(matrix, oracle);
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
                IMetricComputation computation = new RecallQueryComputation(matrix, oracle);
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
                IMetricComputation computation = new AveragePrecisionQueryComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped average precision computation.");
            }
            #endregion
            #region Mean Average Precision
            if (_config.MeanAveragePrecision)
            {
                Logger.Trace("Computing mean average precision...");
                IMetricComputation computation = new MeanAveragePrecisionComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped mean average precision computation.");
            }
            #endregion
        }
    }
}

