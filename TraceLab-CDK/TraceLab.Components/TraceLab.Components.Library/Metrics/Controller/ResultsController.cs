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
using TraceLabSDK;
using TraceLabSDK.Types.Contests;

namespace TraceLab.Components.Library.Metrics.Controller
{
    public sealed class ResultsController
    {
        #region Singleton utilities

        private static readonly ResultsController _instance;

        /// <summary>
        /// Static constructor
        /// Creates singleton instance
        /// </summary>
        static ResultsController()
        {
            _instance = new ResultsController();
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ResultsController Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Creates an instance for use on its own.
        /// Changes to the instance will not affect the singleton.
        /// </summary>
        /// <returns></returns>
        public static ResultsController CreatePrivateInstance()
        {
            return new ResultsController();
        }

        #endregion

        #region Class definition

        #region Members

        private Object _lock = new Object();

        /// <summary>
        /// Prevent direct instantiation
        /// </summary>
        private ResultsController()
        {
            _techniques = new HashSet<string>();
            _datasets = new HashSet<string>();
            _results = new Dictionary<string, List<IMetricComputation>>();
        }

        private HashSet<string> _techniques;
        private HashSet<string> _datasets;
        private Dictionary<string, List<IMetricComputation>> _results;

        /// <summary>
        /// Collection of techniques in results
        /// </summary>
        public IEnumerable<string> Techniques
        {
            get
            {
                lock (_lock)
                {
                    return new HashSet<string>(_techniques);
                }
            }
        }

        /// <summary>
        /// Collection of datasets in results
        /// </summary>
        public IEnumerable<string> Datasets
        {
            get
            {
                lock (_lock)
                {
                    return new HashSet<string>(_datasets);
                }
            }
        }

        #endregion

        /// <summary>
        /// Adds the results for the given computation.
        /// This operation is thread-safe.
        /// </summary>
        /// <param name="technique">Technique name</param>
        /// <param name="dataset">Dataset name</param>
        /// <param name="computation">Computation object</param>
        public void AddResult(string technique, string dataset, IMetricComputation computation)
        {
            if (String.IsNullOrWhiteSpace(technique))
            {
                throw new ComponentException("Technique name cannot be empty.");
            }
            if (String.IsNullOrWhiteSpace(dataset))
            {
                throw new ComponentException("Dataset name cannot be empty.");
            }
            if (computation == null)
            {
                throw new ComponentException("Computation cannot be null.");
            }
            lock (_lock)
            {
                _techniques.Add(technique);
                _datasets.Add(dataset);
                string key = ComputeKey(technique, dataset);
                if (!_results.ContainsKey(key))
                {
                    _results.Add(key, new List<IMetricComputation>());
                }
                _results[key].Add(computation);
            }
        }

        /// <summary>
        /// Returns a collection of IMetricCompuatations for the given technique and dataset.
        /// This operation is thread-safe.
        /// </summary>
        /// <param name="technique">Technique name</param>
        /// <param name="dataset">Dataset name</param>
        /// <returns>IMetricComputation collection</returns>
        public IEnumerable<IMetricComputation> GetResults(string technique, string dataset)
        {
            if (String.IsNullOrWhiteSpace(technique))
            {
                throw new ComponentException("Technique name cannot be empty.");
            }
            if (String.IsNullOrWhiteSpace(dataset))
            {
                throw new ComponentException("Dataset name cannot be empty.");
            }
            lock (_lock)
            {
                if (!_techniques.Contains(technique))
                {
                    throw new ComponentException("Collection does not contain technique \"" + technique + "\"");
                }
                if (!_datasets.Contains(dataset))
                {
                    throw new ComponentException("Collection does not contain dataset \"" + dataset + "\"");
                }
                List<IMetricComputation> list = null;
                string key = ComputeKey(technique, dataset);
                _results.TryGetValue(key, out list);
                if (list == null)
                {
                    throw new ComponentException("Could not retrieve data from collection: (" + technique + ", " + dataset + ")");
                }
                return new List<IMetricComputation>(list);
            }
        }

        /// <summary>
        /// Generates a TLExperimentsResultsCollection containing summary data of results.
        /// This operation is thread-safe.
        /// </summary>
        /// <returns>Results summaries</returns>
        public TLExperimentsResultsCollection GenerateSummaryResults()
        {
            lock (_lock)
            {
                TLExperimentsResultsCollection ExperimentsResultsCollection = new TLExperimentsResultsCollection();
                // iterate over techniques
                foreach (string technique in _techniques)
                {
                    TLExperimentResults TechniqueResults = new TLExperimentResults(technique);
                    // iterate over datasets
                    foreach (string dataset in _datasets)
                    {
                        // get list of results for technique + dataset
                        List<IMetricComputation> list = null;
                        string key = ComputeKey(technique, dataset);
                        _results.TryGetValue(key, out list);
                        if (list != null)
                        {
                            DatasetResults data = new DatasetResults(dataset);
                            // add results to dataset
                            foreach (IMetricComputation computation in list)
                            {
                                if (!computation.HasRun)
                                {
                                    computation.Compute();
                                }
                                data.AddMetric(computation.GenerateSummary());
                            }
                            // add dataset to technique
                            if (data.Metrics.Count() > 0)
                            {
                                TechniqueResults.AddDatasetResult(data);
                            }
                        }
                    }
                    // add technique to collection
                    if (TechniqueResults.DatasetsResults.Count() > 0)
                    {
                        ExperimentsResultsCollection.Add(TechniqueResults);
                    }
                }
                return ExperimentsResultsCollection;
            }
        }

        /// <summary>
        /// Computes the key used in the results collection
        /// </summary>
        /// <param name="technique">Technique name</param>
        /// <param name="dataset">Dataset name</param>
        /// <returns>Key</returns>
        private string ComputeKey(string technique, string dataset)
        {
            return technique + "_" + dataset;
        }

        #endregion
    }
}
