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
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.Library.Metrics.Controller
{
    [Component(Name = "(UI) Results Metrics Converter",
        Description = "Converts the results computed by previous metrics components into a TLExperimentsResultsCollection. Predominantly used by the Results Visualization component.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Output, "ExperimentResults", typeof(TLExperimentsResultsCollection))]
    [Tag("Metrics.Visualization")]
    public class ResultsControllerComponent : BaseComponent
    {
        public ResultsControllerComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLExperimentsResultsCollection results = ResultsController.Instance.GenerateSummaryResults();
            if (results.Count == 0)
            {
                throw new ComponentException("No results found.");
            }
            Workspace.Store("ExperimentResults", results);
        }
    }
}
