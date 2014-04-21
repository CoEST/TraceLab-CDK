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

using TraceLab.Components.DevelopmentKit;
using TraceLab.Components.DevelopmentKit.Preprocessors;
using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;
using TraceLabSDK;
using TraceLabSDK.Types;
using TraceLab.Components.DevelopmentKit.Utils.TermDocumentMatrixUtils;

namespace TraceLab.Components.Library.Tracers.InformationRetrieval
{
    [Component(Name = "Vector Space Model (with smoothing filter)",
                Description = "VSM applied with a smoothing filter",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("Tracers.InformationRetrieval")]
    public class VSMSmoothComponent : BaseComponent
    {
        public VSMSmoothComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLArtifactsCollection sourceArtifacts = (TLArtifactsCollection)Workspace.Load("SourceArtifacts");
            TLArtifactsCollection targetArtifacts = (TLArtifactsCollection)Workspace.Load("TargetArtifacts");
            TermDocumentMatrix matrix = new TermDocumentMatrix(sourceArtifacts, targetArtifacts);
            matrix = SmoothingFilter.Compute(matrix, sourceArtifacts.Keys);
            matrix = SmoothingFilter.Compute(matrix, targetArtifacts.Keys);
            TLSimilarityMatrix sims = SimilarityUtil.ComputeCosine(matrix, sourceArtifacts.Keys, targetArtifacts.Keys);
            Workspace.Store("Similarities", sims);
        }
    }
}
