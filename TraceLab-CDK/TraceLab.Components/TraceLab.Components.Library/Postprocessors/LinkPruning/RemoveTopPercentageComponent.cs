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
using TraceLab.Components.DevelopmentKit.Utils;

namespace TraceLab.Components.Library.Postprocessors.LinkPruning
{
    [Component(Name = "Remove Top Percentage",
        Description = "Removes top percent of links from a TLSimilarityMatrix",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(RemoveTopPercentageComponentConfig))]
    [IOSpec(IOSpecType.Input, "Matrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "PrunedMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Postprocessors.Link Pruning")]
    public class RemoveTopPercentageComponent : BaseComponent
    {
        private RemoveTopPercentageComponentConfig _config;

        public RemoveTopPercentageComponent(ComponentLogger log)
            : base(log)
        {
            _config = new RemoveTopPercentageComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)Workspace.Load("Matrix");
            TLSimilarityMatrix pruned = TLSimilarityMatrixUtil.CreateMatrix(TLSimilarityMatrixUtil.RemoveTopPercentage(matrix, _config.Percentage));
            Workspace.Store("PrunedMatrix", pruned);
        }
    }

    public class RemoveTopPercentageComponentConfig
    {
        public double Percentage { get; set; }
    }
}
