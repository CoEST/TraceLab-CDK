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

namespace TraceLab.Components.Library.Importers.SimilarityMatrices
{
    [Component(Name = "Answer Matrix Directory Importer",
                Description = "Imports an answer matrix from a directory of files. Each file name is a source artifact, and each line of the file is a target.",
                Author = "SEMERU; Evan Moritz",
                Version = "1.0.0.0",
                ConfigurationType = typeof(AnswerMatrixDirectoryImporterConfig))]
    [IOSpec(IOSpecType.Output, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Importers.TLSimilarityMatrix.From DIR")]
    public class AnswerMatrixDirectoryImporter : BaseComponent
    {
        private AnswerMatrixDirectoryImporterConfig _config;

        public AnswerMatrixDirectoryImporter(ComponentLogger log)
            : base(log)
        {
            _config = new AnswerMatrixDirectoryImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("AnswerMatrix", DevelopmentKit.IO.Oracle.ImportDirectory(_config.OracleDirectory.Absolute));
        }
    }

    public class AnswerMatrixDirectoryImporterConfig
    {
        [DisplayName("Oracle directory")]
        public DirectoryPath OracleDirectory { get; set; }
    }
}
