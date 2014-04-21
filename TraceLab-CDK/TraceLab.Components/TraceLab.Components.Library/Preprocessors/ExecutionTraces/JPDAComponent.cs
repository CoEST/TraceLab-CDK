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
using TraceLabSDK.Component.Config;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using TraceLab.Components.DevelopmentKit.Preprocessors.ExecutionTraces;
using System.IO;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors.ExecutionTraces
{
    [Component(Name = "JPDA Trace Analyzer",
        Description = "Extracts a PDG and unique methods from a JPDA-format trace.",
        Author = "SEMERU; Bogdon Dit; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(JPDAComponentConfig))]
    [Tag("Preprocessors.Execution Traces")]
    [IOSpec(IOSpecType.Input, "TraceID", typeof(string))]
    [IOSpec(IOSpecType.Input, "Artifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "PDG", typeof(PDG))]
    [IOSpec(IOSpecType.Output, "UniqueMethods", typeof(IEnumerable<string>))]
    public class JPDAComponent : BaseComponent
    {
        private JPDAComponentConfig _config;

        public JPDAComponent(ComponentLogger log)
            : base(log)
        {
            _config = new JPDAComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            string traceID = (string)Workspace.Load("TraceID");
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("Artifacts");
            BiGramCollection bigrams = JPDA.GenerateBiGrams(Path.Combine(_config.TraceDirectory, traceID), new HashSet<string>(artifacts.Keys));
            PDG pdg = PDG.Convert(bigrams);
            ISet<string> unique = JPDA.GenerateUniqueMethods(Path.Combine(_config.TraceDirectory, traceID));
            Workspace.Store("PDG", pdg);
            Workspace.Store("UniqueMethods", unique);
        }
    }

    public class JPDAComponentConfig
    {
        public DirectoryPath TraceDirectory { get; set; }
    }
}
