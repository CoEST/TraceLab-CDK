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
using TraceLab.Components.Types.Tracers.WebMining;
using TraceLabSDK.Types;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using RPlugin.Core;
using TraceLab.Components.DevelopmentKit.Tracers.WebMining;

namespace TraceLab.Components.Library.Tracers.WebMining
{
    [Component(Name = "PageRank",
        Description = "Computes PageRank scores for methods in a PDG.",
        Author = "SEMERU; Evan Moritz; Bogdan Dit",
        Version = "1.0.0.0",
        ConfigurationType = typeof(PageRankConfig))]
    [IOSpec(IOSpecType.Input, "TraceID", typeof(string))]
    [IOSpec(IOSpecType.Input, "PDG", typeof(PDG))]
    [IOSpec(IOSpecType.Output, "Ranks", typeof(TLSimilarityMatrix))]
    [Tag("Tracers.WebMining")]
    [Tag("RPlugin.Tracers.WebMining")]
    public class PageRankComponent : BaseComponent
    {
        private PageRankConfig _config;

        public PageRankComponent(ComponentLogger log)
            : base(log)
        {
            _config = new PageRankConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            string TraceID = (string)Workspace.Load("TraceID");
            PDG pdg = (PDG)Workspace.Load("PDG");
            REngine engine = new REngine(_config.RScriptPath.Absolute);
            RScript script = new PageRankScript(TraceID, pdg, _config);
            TLSimilarityMatrix ranks = (TLSimilarityMatrix)engine.Execute(script);
            Workspace.Store("Ranks", ranks);
        }
    }
}
