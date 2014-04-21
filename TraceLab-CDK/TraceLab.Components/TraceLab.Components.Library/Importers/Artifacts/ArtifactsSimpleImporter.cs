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
using System.ComponentModel;
using System.Linq;
using System.Text;
using TraceLabSDK;
using TraceLabSDK.Component.Config;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Importers.Artifacts
{
    [Component(Name = "Simple Artifacts Importer",
            Description = "Imports corpus artifacts in simple file format.",
            Author = "SEMERU; Evan Moritz",
            Version = "1.0.0.0",
            ConfigurationType = typeof(ArtifactsSimpleImporterConfig))]
    [IOSpec(IOSpecType.Output, "Artifacts", typeof(TLArtifactsCollection))]
    [Tag("Importers.TLArtifactsCollection.From TXT")]
    public class ArtifactsSimpleImporter : BaseComponent
    {
        private ArtifactsSimpleImporterConfig _config;

        public ArtifactsSimpleImporter(ComponentLogger log) : base(log)
        {
            _config = new ArtifactsSimpleImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("Artifacts", DevelopmentKit.IO.Artifacts.ImportFile(_config.Artifacts.Absolute));
        }
    }

    public class ArtifactsSimpleImporterConfig
    {
        [DisplayName("Artifacts location")]
        [Description("Location of artifacts file")]
        public FilePath Artifacts { get; set; }
    }
}
