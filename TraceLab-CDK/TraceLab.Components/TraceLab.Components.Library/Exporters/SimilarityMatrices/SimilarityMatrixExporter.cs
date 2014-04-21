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

namespace TraceLab.Components.Library.Exporters.SimilarityMatrices
{
    [Component(Name = "Similarity Matrix Exporter",
                Description = "Exports a TLSimilarityMatrix to the designated file.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(SimilarityMatrixExporterConfig))]
    [IOSpec(IOSpecType.Input, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("Exporters.TLSimilarityMatrix.To TXT")]
    public class SimilarityMatrixExporter : BaseComponent
    {
        private SimilarityMatrixExporterConfig _config;

        public SimilarityMatrixExporter(ComponentLogger log)
            : base(log)
        {
            _config = new SimilarityMatrixExporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Logger.Trace("Writing similarities to " + _config.File.Absolute);
            DevelopmentKit.IO.Similarities.Export((TLSimilarityMatrix)Workspace.Load("Similarities"), _config.File.Absolute);
            Logger.Trace("Write complete.");
        }
    }

    public class SimilarityMatrixExporterConfig
    {
        [DisplayName("File location")]
        [Description("File to import")]
        public FilePath File { get; set; }
    }
}
