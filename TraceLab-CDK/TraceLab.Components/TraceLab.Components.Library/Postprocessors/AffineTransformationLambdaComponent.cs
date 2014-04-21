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
using TraceLab.Components.DevelopmentKit.Postprocessors;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Postprocessors
{
    [Component(Name = "Affine Tranformation (lambda input)",
                Description = "Performs an affine transformation combining two distributions.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Input, "LargeExpert", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "SmallExpert", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "Lambda", typeof(double))]
    [IOSpec(IOSpecType.Output, "CombinedSimilarities", typeof(TLSimilarityMatrix))]
    [Tag("Postprocessors")]
    public class AffineTransformationLambdaComponent : BaseComponent
    {
        public AffineTransformationLambdaComponent(ComponentLogger log)
            : base(log)
        {

        }

        public override void Compute()
        {
            TLSimilarityMatrix large = (TLSimilarityMatrix)Workspace.Load("LargeExpert");
            TLSimilarityMatrix small = (TLSimilarityMatrix)Workspace.Load("SmallExpert");
            double lambda = (double)Workspace.Load("Lambda");
            TLSimilarityMatrix combined = AffineTranformation.Transform(large, small, lambda);
            Workspace.Store("CombinedSimilarities", combined);
        }
    }
}
