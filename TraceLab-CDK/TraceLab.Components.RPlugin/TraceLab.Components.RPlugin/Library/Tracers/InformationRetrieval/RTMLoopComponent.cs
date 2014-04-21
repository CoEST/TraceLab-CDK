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
    [Component(Name = "(loop) Relational Topic Model",
                Description = "Computes similarities between artifacts using RTM. Imports configurations for use in a loop.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0")]
    [IOSpec(IOSpecType.Input, "SourceArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "TargetArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Input, "Config", typeof(RTMConfig))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("RPlugin.Tracers.InformationRetrieval")]
    [Tag("Tracers.InformationRetrieval")]
    public class RTMLoopComponent : BaseComponent
    {
        public RTMLoopComponent(ComponentLogger log) : base(log) { }

        public override void Compute()
        {
            TLArtifactsCollection source = (TLArtifactsCollection)Workspace.Load("SourceArtifacts");
            TLArtifactsCollection target = (TLArtifactsCollection)Workspace.Load("TargetArtifacts");
            RTMConfig config = (RTMConfig)Workspace.Load("Config");
            REngine engine = new REngine(config.RScriptPath);
            TLSimilarityMatrix sims = (TLSimilarityMatrix) engine.Execute(new RTMScript(source, target, config));
            Workspace.Store("Similarities", sims);
        }
    }
}