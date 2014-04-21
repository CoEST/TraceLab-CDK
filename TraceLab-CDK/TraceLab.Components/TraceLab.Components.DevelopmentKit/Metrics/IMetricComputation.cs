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

using TraceLabSDK;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.DevelopmentKit.Metrics
{
    /// <summary>
    /// Interface for defining new metrics.
    /// </summary>
    public interface IMetricComputation
    {
        /// <summary>
        /// Name of the metric
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the metric
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Provides the results of the metrics computation
        /// </summary>
        SerializableDictionary<string, double> Results { get; }

        /// <summary>
        /// Flag which describes whether the Compute() method has been run
        /// </summary>
        bool HasRun { get; }

        /// <summary>
        /// Main computation method.
        /// </summary>
        void Compute();

        /// <summary>
        /// Generates a summarizing Metric for results analysis
        /// </summary>
        /// <returns>Metric object</returns>
        Metric GenerateSummary();
    }
}
