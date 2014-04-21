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
using System.IO;
using TraceLabSDK;
using TraceLabSDK.Types;
using RPlugin.Core;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;

namespace TraceLab.Components.Library.Tracers.InformationRetrieval
{
    [Component(Name = "Relational Topic Model",
                Description = "Computes similarities between artifacts using RTM",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(RTMConfig))]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("RPlugin.Tracers.InformationRetrieval")]
    [Tag("Tracers.InformationRetrieval")]
    public class RTMComponent : BaseComponent
    {
        private RTMConfig _config;

        public RTMComponent(ComponentLogger log)
            : base(log)
        {
            _config = new RTMConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection source = (TLArtifactsCollection) Workspace.Load("SourceArtifacts");
            TLArtifactsCollection target = (TLArtifactsCollection) Workspace.Load("TargetArtifacts");
            REngine engine = new REngine(_config.RScriptPath);
            TLSimilarityMatrix sims = (TLSimilarityMatrix)engine.Execute(new RTMScript(source, target, _config));
            Workspace.Store("Similarities", sims);
        }
    }
}