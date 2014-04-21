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

namespace TraceLab.Components.Library.Importers.SimilarityMatrices
{
    [Component(Name = "Answer Matrix Importer",
                Description = "Imports an answer matrix in the form of {SOURCE TARGET1 TARGET2 ...\\n}",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(AnswerMatrixImporterConfig))]
    [IOSpec(IOSpecType.Output, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Importers.TLSimilarityMatrix.From TXT")]
    public class AnswerMatrixImporter : BaseComponent
    {
        private AnswerMatrixImporterConfig _config;

        public AnswerMatrixImporter(ComponentLogger log)
            : base(log)
        {
            _config = new AnswerMatrixImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("AnswerMatrix", DevelopmentKit.IO.Oracle.Import(_config.OracleDocument.Absolute));
        }
    }

    public class AnswerMatrixImporterConfig
    {
        [DisplayName("Oracle file")]
        [Description("Oracle file location")]
        public FilePath OracleDocument { get; set; }
    }
}
