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
using TraceLab.Components.DevelopmentKit.Metrics;
using System.IO;
using TraceLabSDK.Types.Contests;
using System.Xml;

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Writes classes implenting IMetricComputation to disk
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// Exports an IMetricComputation to disk
        /// </summary>
        /// <param name="computation">Computation</param>
        /// <param name="filename">Destination file</param>
        /// <param name="summary">Option to write summary data</param>
        public static void Export(IMetricComputation computation, string filename, bool summary = false)
        {
            // compute file names
            string basefilename = (filename.EndsWith(".txt"))
                ? filename.Substring(0, filename.Length - 4)
                : filename;
            string longfilename = basefilename + ".csv";
            string summaryfilename = basefilename + ".summary.xml";
            // write results
            TextWriter file = new StreamWriter(longfilename);
            file.WriteLine("\"{0}\",\"{1}\"", computation.Name.Replace("\"", "\\\"\\"), computation.Description.Replace("\"", "\\\"\\"));
            foreach (KeyValuePair<string, double> result in computation.Results)
            {
                file.WriteLine("\"{0}\",{1}", result.Key.Replace("\"", "\\\"\\"), result.Value);
            }
            file.Flush();
            file.Close();
            // write summary
            if (summary)
            {
                using (XmlWriter writer = XmlWriter.Create(summaryfilename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Metric");
                    Metric metric = computation.GenerateSummary();
                    metric.WriteXml(writer);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }
    }
}
