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

namespace TraceLab.Components.Library.Exporters.Artifacts
{
    [Component(Name = "Simple Artifacts Exporter",
                Description = "Exports a TLArtifactsCollection to a tab-delimited text file.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(ArtifactsSimpleExporterConfig))]
    [IOSpec(IOSpecType.Input, "Artifacts", typeof(TLArtifactsCollection))]
    [Tag("Exporters.TLArtifactsCollection.To TXT")]
    public class ArtifactsSimpleExporter : BaseComponent
    {
        private ArtifactsSimpleExporterConfig _config;

        public ArtifactsSimpleExporter(ComponentLogger log)
            : base(log)
        {
            _config = new ArtifactsSimpleExporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("Artifacts");
            DevelopmentKit.IO.Artifacts.ExportFile(artifacts, _config.File.Absolute);
        }
    }

    public class ArtifactsSimpleExporterConfig
    {
        [DisplayName("File location")]
        [Description("File name to export to")]
        public FilePath File { get; set; }
    }
}
