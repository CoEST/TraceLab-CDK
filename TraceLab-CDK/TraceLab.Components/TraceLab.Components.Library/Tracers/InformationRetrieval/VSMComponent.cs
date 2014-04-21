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
    [Component(Name = "Vector Space Model",
                Description = "Calculates the tf-idf weighted cosine similarities of two TLArtifactsCollections.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(VSMComponentConfig))]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("Tracers.InformationRetrieval")]
    public class VSMComponent : BaseComponent
    {
        private VSMComponentConfig _config;

        public VSMComponent(ComponentLogger log)
            : base(log)
        {
            _config = new VSMComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection sourceArtifacts = (TLArtifactsCollection)Workspace.Load("SourceArtifacts");
            TLArtifactsCollection targetArtifacts = (TLArtifactsCollection)Workspace.Load("TargetArtifacts");
            TLSimilarityMatrix sims = VSM.Compute(sourceArtifacts, targetArtifacts, _config.WeightingScheme);
            Workspace.Store("Similarities", sims);
        }
    }

    public class VSMComponentConfig
    {
        public VSMWeightEnum WeightingScheme { get; set; }
    }
}
