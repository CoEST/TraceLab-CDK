// TraceLab - Software Traceability Instrument to Facilitate and Empower Traceability Research
// Copyright © 2012-2013 CoEST - National Science Foundation MRI-R2 Grant # CNS: 0959924
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
    [Component(Name = "Artifacts XML Exporter",
                Description = "Exports a TLArtifactsCollection to an XML file.",
                Author = "SAREC",
                Version = "1.0.0.0",
                ConfigurationType = typeof(ArtifactsXMLExporterConfig))]
    [IOSpec(IOSpecType.Input, "Artifacts", typeof(TLArtifactsCollection))]
    [Tag("Exporters.TLArtifactsCollection.To XML")]
    public class ArtifactsXMLExporter : BaseComponent
    {
        private ArtifactsXMLExporterConfig _config;

        public ArtifactsXMLExporter(ComponentLogger log)
            : base(log)
        {
            _config = new ArtifactsXMLExporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLArtifactsCollection artifacts = (TLArtifactsCollection)Workspace.Load("Artifacts");
            DevelopmentKit.IO.Artifacts.ExportXML(artifacts, _config.Path.Absolute, _config.CollectionId, _config.CollectionName, _config.CollectionVersion, _config.CollectionDescription);
        }
    }

    public class ArtifactsXMLExporterConfig
    {
        [DisplayName("1. Collection Id")]
        public string CollectionId { get; set; }

        [DisplayName("2. Collection Name")]
        public string CollectionName { get; set; }

        [DisplayName("3. Collection Version")]
        public string CollectionVersion { get; set; }

        [DisplayName("4. Collection Description")]
        public string CollectionDescription { get; set; }

        [DisplayName("5. Output filepath")]
        public FilePath Path { get; set; }
    }
}
