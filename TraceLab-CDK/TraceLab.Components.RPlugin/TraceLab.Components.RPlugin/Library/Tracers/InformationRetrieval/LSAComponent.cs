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
using TraceLabSDK;
using TraceLabSDK.Types;
using RPlugin.Core;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;

namespace TraceLab.Components.Library.Tracers.InformationRetrieval
{
    [Component(Name = "Latent Semantic Analysis",
                Description = "Performs LSA on a set of artifacts.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(LSAConfig))]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("RPlugin.Tracers.InformationRetrieval")]
    [Tag("Tracers.InformationRetrieval")]
    public class LSAComponent : BaseComponent
    {
        private LSAConfig _config;

        public LSAComponent(ComponentLogger log)
            : base(log)
        {
            _config = new LSAConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection source = (TLArtifactsCollection) Workspace.Load("SourceArtifacts");
            TLArtifactsCollection target = (TLArtifactsCollection) Workspace.Load("TargetArtifacts");
            REngine engine = new REngine(_config.RScriptPath);
            TLSimilarityMatrix sims = (TLSimilarityMatrix)engine.Execute(new LSAScript(source, target, _config));
            Workspace.Store("Similarities", sims);
        }
    }
}