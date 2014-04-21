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

namespace TraceLab.Components.Library.Importers.Stopwords
{
    [Component(Name = "Stopwords Importer",
           Author = "SAREC",
           Version = "1.0.0.0",
           Description = "Imports list of stopwords from text file. Each stopword should be in new line. \n" +
                         "Sample file can be found in Package 'Importers Sample Files', file 'Stopwords Importer Format.txt'",
           ConfigurationType = typeof(StopwordsImporterConfig))]
    [IOSpec(IOSpecType.Output, "stopwords", typeof(TraceLabSDK.Types.TLStopwords))]
    [Tag("Importers.TLStopwords.From TXT")]
    public class StopwordsImporter : BaseComponent
    {
        private StopwordsImporterConfig _config;

        public StopwordsImporter(ComponentLogger log)
            : base(log)
        {
            _config = new StopwordsImporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            TLStopwords stopwords = DevelopmentKit.IO.Stopwords.Import(_config.Path.Absolute);
            Workspace.Store("stopwords", stopwords);

            Logger.Info("Stopwords has been imported from " + _config.Path);
        }
    }

    public class StopwordsImporterConfig
    {
        [DisplayName("File to import")]
        public FilePath Path { get; set; }
    }
}
