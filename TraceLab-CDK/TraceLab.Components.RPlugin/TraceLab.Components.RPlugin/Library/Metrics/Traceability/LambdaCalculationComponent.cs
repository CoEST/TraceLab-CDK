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

namespace TraceLab.Components.Library.Metrics.Traceability
{
    [Component(Name = "Lambda Calculation",
        Description = "Calculates lambda for an affine transformation based on the results of PCA.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0")]
    [Tag("Metrics.Traceability")]
    [Tag("RPlugin.Metrics.Traceability")]
    [IOSpec(IOSpecType.Input, "Technique1PCA", typeof(double))]
    [IOSpec(IOSpecType.Input, "Technique2PCA", typeof(double))]
    [IOSpec(IOSpecType.Output, "Technique1to2Lambda", typeof(double))]
    public class LambdaCalculationComponent : BaseComponent
    {
        public LambdaCalculationComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            double t1 = (double)Workspace.Load("Technique1PCA");
            double t2 = (double)Workspace.Load("Technique2PCA");
            Workspace.Store("Technique1to2Lambda", t1 / (t1 + t2));
        }
    }
}
