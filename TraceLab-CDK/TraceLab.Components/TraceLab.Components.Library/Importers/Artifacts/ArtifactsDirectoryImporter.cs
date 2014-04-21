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
using TraceLabSDK;
using TraceLabSDK.Component.Config;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Importers.Artifacts
{
    [Component(Name = "Artifacts Directory Importer",
            Description = "Imports a corpus from a directory containing artifacts files.",
            Author = "SEMERU; Evan Moritz",
            Version = "1.0.0.0",
            ConfigurationType = typeof(ArtifactsDirectoryImporterConfig))]
    [IOSpec(IOSpecType.Output, "Artifacts", typeof(TLArtifactsCollection))]
    [Tag("Importers.TLArtifactsCollection.From DIR")]
    public class ArtifactsDirectoryImporter : BaseComponent
    {
        private ArtifactsDirectoryImporterConfig _config;

        public ArtifactsDirectoryImporter(ComponentLogger log)
            : base(log)
        {
            _config = new ArtifactsDirectoryImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("Artifacts", DevelopmentKit.IO.Artifacts.ImportDirectory(_config.Directory.Absolute, _config.Filter));
        }
    }

    public class ArtifactsDirectoryImporterConfig
    {
        [DisplayName("Artifacts directory")]
        [Description("Directory location containing artifacts files.")]
        public DirectoryPath Directory { get; set; }

        [DisplayName("Filter")]
        [Description("Only reads files with the given extension (no '.'). Leave blank to include all files.")]
        public string Filter { get; set; }
    }
}
