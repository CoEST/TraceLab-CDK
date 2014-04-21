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
using TraceLab.Components.DevelopmentKit.Metrics;
using TraceLab.Components.DevelopmentKit.Metrics.FeatureLocation;
using TraceLab.Components.Library.Metrics.Controller;
using TraceLabSDK;
using TraceLabSDK.Types;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.Library.Metrics.FeatureLocation
{
    [Component(Name = "Feature Location Metrics",
        Description = "Computes various feature location metrics on a similarity matrix.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(FeatureLocationMetricsConfig))]
    [IOSpec(IOSpecType.Input, "CandidateMatrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Metrics.FeatureLocation")]
    public class FeatureLocationMetricsComponent : BaseComponent
    {
        private FeatureLocationMetricsConfig _config;

        public FeatureLocationMetricsComponent(ComponentLogger log)
            : base(log)
        {
            _config = new FeatureLocationMetricsConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Logger.Trace("Starting metrics computation: " + _config.TechniqueName);
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)Workspace.Load("CandidateMatrix");
            TLSimilarityMatrix oracle = (TLSimilarityMatrix)Workspace.Load("AnswerMatrix");
            TLExperimentResults exResults = new TLExperimentResults(_config.TechniqueName);
            #region Effectiveness Best Measure
            if (_config.EffectivenessBestMeasure)
            {
                Logger.Trace("Computing effectiveness best measure...");
                IMetricComputation computation = new EffectivenessBestMeasureComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped effectiveness best measure computation.");
            }
            #endregion
            #region Effectiveness All Measure
            if (_config.EffectivenessAllMeasure)
            {
                Logger.Trace("Computing effectiveness all measure...");
                IMetricComputation computation = new EffectivenessAllMeasureComputation(matrix, oracle);
                computation.Compute();
                ResultsController.Instance.AddResult(_config.TechniqueName, _config.DatasetName, computation);
            }
            else
            {
                Logger.Trace("Skipped effectiveness all measure computation.");
            }
            #endregion
        }
    }
}
