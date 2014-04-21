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
    [Component(Name = "Remove Bottom Percentage",
        Description = "Removes bottom percent of links from a TLSimilarityMatrix",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(RemoveBottomPercentageComponentConfig))]
    [IOSpec(IOSpecType.Input, "Matrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "PrunedMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Postprocessors.Link Pruning")]
    public class RemoveBottomPercentageComponent : BaseComponent
    {
        private RemoveBottomPercentageComponentConfig _config;

        public RemoveBottomPercentageComponent(ComponentLogger log)
            : base(log)
        {
            _config = new RemoveBottomPercentageComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)Workspace.Load("Matrix");
            TLSimilarityMatrix pruned = TLSimilarityMatrixUtil.CreateMatrix(TLSimilarityMatrixUtil.RemoveBottomPercentage(matrix, _config.Percentage));
            Workspace.Store("PrunedMatrix", pruned);
        }
    }

    public class RemoveBottomPercentageComponentConfig
    {
        public double Percentage { get; set; }
    }
}
