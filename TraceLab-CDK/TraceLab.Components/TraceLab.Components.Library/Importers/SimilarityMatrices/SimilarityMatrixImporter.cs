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

namespace TraceLab.Components.Library.Importers.SimilarityMatrices
{
    [Component(Name = "Similarities Importer",
                Description = "Imports a TLSimilarityMatrix from file.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(SimilarityMatrixImporterConfig))]
    [IOSpec(IOSpecType.Output, "Similarities", typeof(TLSimilarityMatrix))]
    [Tag("Importers.TLSimilarityMatrix.From TXT")]
    public class SimilaritiesImporter : BaseComponent
    {
        private SimilarityMatrixImporterConfig _config;

        public SimilaritiesImporter(ComponentLogger log)
            : base(log)
        {
            _config = new SimilarityMatrixImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("Similarities", (TLSimilarityMatrix)DevelopmentKit.IO.Similarities.Import(_config.File.Absolute));
        }
    }

    public class SimilarityMatrixImporterConfig
    {
        [DisplayName("Similarities file")]
        [Description("Similarities file location")]
        public FilePath File { get; set; }
    }
}
