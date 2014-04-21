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

using System.ComponentModel;
using TraceLab.Components.DevelopmentKit.Preprocessors.Splitters;
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Preprocessors.Splitters
{
    [Component(Name = "CamelCase Splitter",
        Description = "Processes a TLArtifactsCollection by splitting CamelCase terms.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(CamelCaseSplitterComponentConfig))]
    [IOSpec(IOSpecType.Input, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [IOSpec(IOSpecType.Output, "ListOfArtifacts", typeof(TLArtifactsCollection))]
    [Tag("Preprocessors.Splitters")]
    public class CamelCaseSplitterComponent : BaseComponent
    {
        private CamelCaseSplitterComponentConfig _config;

        public CamelCaseSplitterComponent(ComponentLogger log)
            : base(log)
        {
            _config = new CamelCaseSplitterComponentConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("ListOfArtifacts");
            TLArtifactsCollection processed = CamelCaseSplitter.ProcessArtifacts(artifacts, _config.ConvertLowercase);
            Workspace.Store("ListOfArtifacts", processed);
        }
    }

    public class CamelCaseSplitterComponentConfig
    {
        [DisplayName("Convert to lowercase?")]
        [Description("Option to convert resulting terms to lowercase.")]
        public bool ConvertLowercase { get; set; }
    }
}
