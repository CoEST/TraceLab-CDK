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
using System.ComponentModel;
using TraceLabSDK.Component.Config;

namespace TraceLab.Components.Library.Importers.Artifacts
{
    [Component(Name = "Artifacts Mapping Importer",
            Description = "Imports corpus artifacts from two files: (map) each line is an ID, and (raw) each line is the artifact.",
            Author = "SEMERU; Evan Moritz",
            Version = "1.0.0.0",
            ConfigurationType = typeof(ArtifactsMappingImporterConfig))]
    [IOSpec(IOSpecType.Output, "Artifacts", typeof(TLArtifactsCollection))]
    [Tag("Importers.TLArtifactsCollection.From TXT")]
    public class ArtifactsMappingImporter : BaseComponent
    {
        private ArtifactsMappingImporterConfig _config;

        public ArtifactsMappingImporter(ComponentLogger log)
            : base(log)
        {
            _config = new ArtifactsMappingImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("Artifacts", DevelopmentKit.IO.Artifacts.ImportFromMapping(_config.Mapping.Absolute, _config.Raw.Absolute));
        }
    }

    public class ArtifactsMappingImporterConfig
    {
        [DisplayName("Mapping file location")]
        public FilePath Mapping { get; set; }

        [DisplayName("Raw file location")]
        public FilePath Raw { get; set; }
    }
}
