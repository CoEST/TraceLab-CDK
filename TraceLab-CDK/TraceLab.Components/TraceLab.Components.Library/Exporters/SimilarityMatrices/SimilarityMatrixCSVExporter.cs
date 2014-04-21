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

using System;
using System.ComponentModel;
using TraceLabSDK;
using TraceLabSDK.Component.Config;
using TraceLabSDK.Types;

namespace TraceLab.Components.Library.Exporters.SimilarityMatrices
{
    [Component(Name = "Similarity Matrix CSV Exporter",
                Description = "Exports similarity matrix to csv text file.",
                Author = "SAREC",
                Version = "1.0.0.0",
                ConfigurationType = typeof(SimilarityMatrixCSVExporterConfig))]
    [IOSpec(IOSpecType.Input, "SimilarityMatrix", typeof(TLSimilarityMatrix))]
    [Tag("Exporters.TLSimilarityMatrix.To CSV")]
    public class SimilarityMatrixCSVExporter : BaseComponent
    {
        private SimilarityMatrixCSVExporterConfig Config;

        public SimilarityMatrixCSVExporter(ComponentLogger log)
            : base(log)
        {
            Config = new SimilarityMatrixCSVExporterConfig();
            Configuration = Config;
        }

        public override void Compute()
        {
            TLSimilarityMatrix similarityMatrix = (TLSimilarityMatrix)Workspace.Load("SimilarityMatrix");
            DevelopmentKit.IO.Similarities.ExportCSV(similarityMatrix, Config.Path.Absolute);
            Logger.Info(String.Format("Matrix has been saved into csv file '{0}'", Config.Path.Absolute));
        }
    }

    public class SimilarityMatrixCSVExporterConfig
    {
        [DisplayName("File name")]
        [Description("Path of file to save to")]
        public FilePath Path { get; set; }
    }
}
