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
using TraceLabSDK.Types;
using TraceLab.Components.DevelopmentKit.Preprocessors.Splitters;
using System.ComponentModel;

namespace TraceLab.Components.Library.Preprocessors.Splitters
{
    [Component(Name = "SEMERU Splitter",
        Description = "Processes a TLArtifactsCollection by intelligently splitting Semeru terms.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(SemeruSplitterComponentConfig))]
    [IOSpec(IOSpecType.Input, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [Tag("Preprocessors.Splitters")]
    public class SemeruSplitterComponent : BaseComponent
    {
        private SemeruSplitterComponentConfig _config;

        public SemeruSplitterComponent(ComponentLogger log)
            : base(log)
        {
            _config = new SemeruSplitterComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("ListOfArtifacts");
            TLArtifactsCollection processed = SemeruSplitter.ProcessArtifacts(artifacts, _config.KeepCompoundIdentifier);
            Workspace.Store("ListOfArtifacts", processed);
        }
    }

    public class SemeruSplitterComponentConfig
    {
        [DisplayName("Keep compound identifiers?")]
        public bool KeepCompoundIdentifier { get; set; }
    }
}
