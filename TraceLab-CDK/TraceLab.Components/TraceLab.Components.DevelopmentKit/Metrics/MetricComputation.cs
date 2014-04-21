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
    /// Abstract class for defining new metrics.
    /// Includes common functionality for derived classes.
    /// Input should be defined in the constructor.
    /// </summary>
    public abstract class MetricComputation : IMetricComputation
    {
        /// <summary>
        /// Name of the metric
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Description of the metric
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Provides the results of the metrics computation
        /// </summary>
        public SerializableDictionary<string, double> Results
        {
            get;
            protected set;
        }

        /// <summary>
        /// Flag which describes whether the Compute() method has been run
        /// </summary>
        public bool HasRun
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetricComputation()
        {
            HasRun = false;
        }

        /// <summary>
        /// Main computation method.
        /// </summary>
        public void Compute()
        {
            ComputeImplementation();
            HasRun = true;
        }

        /// <summary>
        /// Computation method for derived classes
        /// </summary>
        protected abstract void ComputeImplementation();

        /// <summary>
        /// Generates a summarizing Metric for results analysis
        /// </summary>
        /// <returns>Metric object</returns>
        public Metric GenerateSummary()
        {
            if (!HasRun)
            {
                throw new DevelopmentKitException("Compute() method has not been run.");
            }
            return GenerateSummaryImplementation();
        }

        /// <summary>
        /// Metrics generation method for derived class
        /// </summary>
        /// <returns>Metric object</returns>
        protected abstract Metric GenerateSummaryImplementation();
    }
}
