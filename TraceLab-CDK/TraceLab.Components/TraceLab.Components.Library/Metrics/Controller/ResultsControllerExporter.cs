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
using TraceLabSDK.Types.Contests;
using System.ComponentModel;
using TraceLabSDK.Component.Config;
using TraceLab.Components.DevelopmentKit.Metrics;
using System.IO;

namespace TraceLab.Components.Library.Metrics.Controller
{
    [Component(Name = "(UI) Results Metrics Exporter",
        Description = "Exports the metrics computed by previous metrics components to disk.",
        Author = "SEMERU; Evan Moritz",
        Version = "1.0.0.0",
        ConfigurationType = typeof(ResultsControllerExporterConfig))]
    [Tag("Metrics.Visualization")]
    public class ResultsControllerExporter : BaseComponent
    {
        private ResultsControllerExporterConfig _config;

        public ResultsControllerExporter(ComponentLogger log)
            : base(log)
        {
            _config = new ResultsControllerExporterConfig();
            Configuration = _config;
        }

        public override void Compute()
        {
            // get snapshot of entries in results
            // data is stale as soon as it is retrieved,
            // but there are no deletions
            IEnumerable<string> techniques = ResultsController.Instance.Techniques;
            IEnumerable<string> datasets = ResultsController.Instance.Datasets;
            // iterate over datasets
            foreach (string dataset in datasets)
            {
                Logger.Info("Exporting: " + dataset);
                // iterate over techniques
                foreach (string technique in techniques)
                {
                    // iterate over metrics
                    foreach (IMetricComputation computation in ResultsController.Instance.GetResults(technique, dataset))
                    {
                        try
                        {
                            // export metric to disk
                            DevelopmentKit.IO.Metrics.Export(computation,
                                GenerateFileName(_config.OutputDir.Absolute, dataset, technique, computation.Name),
                                _config.GenerateSummary
                            );
                        }
                        catch (ComponentException e)
                        {
                            Logger.Debug(e.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a file name
        /// </summary>
        private string GenerateFileName(string directory, string dataset, string technique, string computation)
        {
            return Path.Combine(directory, dataset + "." + technique + "." + computation);
        }
    }

    public class ResultsControllerExporterConfig
    {
        [DisplayName("Generate summaries?")]
        [Description("Option to additionally generate summary data.")]
        public bool GenerateSummary { get; set; }

        [DisplayName("Output directory")]
        [Description("Directory to write results files")]
        public DirectoryPath OutputDir { get; set; }
    }
}
