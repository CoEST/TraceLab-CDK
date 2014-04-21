// TraceLab - Software Traceability Instrument to Facilitate and Empower Traceability Research
// Copyright (C) 2012-2013 CoEST - National Science Foundation MRI-R2 Grant # CNS: 0959924
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
// along with this program.  If not, see<http://www.gnu.org/licenses/>.

using System;
using TraceLabSDK;
using TraceLabSDK.Types.Contests;
using System.Collections.Generic;
using TraceLab.Components.UI.WPF.Metrics.Visualization.ResultsCharts.Charts;

namespace TraceLab.Components.UI.WPF.Metrics.Visualization.ResultsCharts
{
    [Component(GuidIDString = "35a2e32e-530a-40a3-9015-42ac0360c32b",
                Name = "(WPF) Results Visualization",
                Description = "GUI Component that displays boxplot and line charts corresponding to the results calculated by the Metrics Computation Engine." +
                                "The charts are clasified by dataset and metric.",
                Author = "SAREC",
                Version = "1.0.0.0")]

    // Input & Outputs
    [IOSpec(IOSpecType.Input, "ExperimentResults", typeof(TLExperimentsResultsCollection))]

    // Tags
    [Tag("Metrics.Visualization")]
    [Tag("WPF.Metrics.Visualization")]

    public class ResultsVisualizationGUIComponent : BaseComponent
    {
        public ResultsVisualizationGUIComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLExperimentsResultsCollection allExperimentResults = (TLExperimentsResultsCollection)Workspace.Load("ExperimentResults");
            if (allExperimentResults == null)
            {
                throw new ComponentException("Received null results");
            }
            if ( allExperimentResults != null && allExperimentResults.Count > 0 )
            {
                SortedDictionary<string, Dictionary<string, Chart>> allResults = ResultsVisualizationHelper.ProcessResults(allExperimentResults);
                if (allResults != null && allResults.Count > 0)
                {
                    ResultsVisualizationWindow resultsWindow = new ResultsVisualizationWindow(allResults);
                    resultsWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    resultsWindow.ShowDialog();
                }
                else
                {
                    throw new ComponentException("Results could not be displayed because no metrics were found. Check selection on Metric Computation Component.");
                }
            }
            else
            {
                throw new ComponentException("Results could not be displayed because TLExperimentsResultsCollection is null or empty.");
            }
        }

        public override void PostCompute()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}
