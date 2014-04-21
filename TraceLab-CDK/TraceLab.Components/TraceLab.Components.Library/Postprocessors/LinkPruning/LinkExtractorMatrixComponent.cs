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
using TraceLab.Components.DevelopmentKit.Utils;

namespace TraceLab.Components.Library.Postprocessors.LinkPruning
{
    [Component(Name = "Extract Specified Links (matrix)",
        Description = "Extracts links containing target matrix's artifact IDs from a similarity matrix.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Input, "OriginalMatrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Input, "TargetMatrix", typeof(TLSimilarityMatrix))]
    [IOSpec(IOSpecType.Output, "ExtractedLinks", typeof(TLSimilarityMatrix))]
    [Tag("Postprocessors.Link Pruning")]
    public class LinkExtractorMatrixComponent : BaseComponent
    {
        public LinkExtractorMatrixComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLSimilarityMatrix original = (TLSimilarityMatrix)Workspace.Load("OriginalMatrix");
            IEnumerable<string> artifactIDs = TLSimilarityMatrixUtil.GetSetOfTargetArtifacts((TLSimilarityMatrix)Workspace.Load("TargetMatrix"));
            TLSimilarityMatrix matrix = TLSimilarityMatrixUtil.CreateMatrix(TLSimilarityMatrixUtil.ExtractLinks(original, artifactIDs, true));
            Workspace.Store("ExtractedLinks", matrix);
        }
    }
}
