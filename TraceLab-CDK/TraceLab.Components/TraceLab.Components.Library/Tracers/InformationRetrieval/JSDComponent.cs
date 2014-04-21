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

using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Tracers.InformationRetrieval
{
    [Component(Name = "Jensen-Shannon divergence",
        Description = "Computes Jensen-Shannon divergence between two sets of artifacts.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("Tracers.InformationRetrieval")]
    public class JSDComponent : BaseComponent
    {
        public JSDComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLArtifactsCollection source = (TLArtifactsCollection)Workspace.Load("SourceArtifacts");
            TLArtifactsCollection target = (TLArtifactsCollection)Workspace.Load("TargetArtifacts");
            TLSimilarityMatrix sims = JSD.Compute(source, target);
            Workspace.Store("Similarities", sims);
        }
    }
}
