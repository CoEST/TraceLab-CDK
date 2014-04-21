﻿// TraceLab - Software Traceability Instrument to Facilitate and Empower Traceability Research
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
    [Component(Name = "Answer Matrix XML Importer",
                Description = "Imports an answer matrix from a standard CoEST XML file.",
                Author = "SAREC",
                Version = "1.0.0.0",
                ConfigurationType = typeof(AnswerMatrixImporterConfig))]
    [IOSpec(IOSpecType.Output, "AnswerMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Importers.TLSimilarityMatrix.From XML")]
    public class AnswerMatrixXMLImporter : BaseComponent
    {
        private AnswerMatrixXMLImporterConfig _config;

        public AnswerMatrixXMLImporter(ComponentLogger log)
            : base(log)
        {
            _config = new AnswerMatrixXMLImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            Workspace.Store("AnswerMatrix", DevelopmentKit.IO.Oracle.ImportXML(_config.OracleDocument.Absolute, true));
        }
    }

    public class AnswerMatrixXMLImporterConfig
    {
        [DisplayName("Oracle file")]
        [Description("Oracle file location")]
        public FilePath OracleDocument { get; set; }
    }
}
