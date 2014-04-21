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
using TraceLab.Components.DevelopmentKit.Utils.TermDocumentMatrixUtils;
using TraceLabSDK.Types;
using System.Collections.Generic;

namespace TraceLab.Components.DevelopmentKit.Preprocessors
{
    /// <summary>
    /// Provides smoothing filter functionality for TermDocumentMatrix
    /// </summary>
    public static class SmoothingFilter
    {
        /// <summary>
        /// Smoothing filter from ICPC'11 paper "Improving IR-based Traceability Recovery Using Smoothing Filters"
        /// </summary>
        /// <param name="artifacts">Artifacts</param>
        /// <returns>Smoothed artifacts</returns>
        public static TermDocumentMatrix Compute(TLArtifactsCollection artifacts)
        {
            return Compute(new TermDocumentMatrix(artifacts));
        }

        /// <summary>
        /// Smoothing filter from ICPC'11 paper "Improving IR-based Traceability Recovery Using Smoothing Filters"
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <returns>Smoothed artifacts</returns>
        public static TermDocumentMatrix Compute(TermDocumentMatrix matrix)
        {
            double[] avg = WeightUtil.ComputeAverageVector(matrix);

            if (avg.Length != matrix.NumTerms)
                throw new ArgumentException("Average vector does not have the correct number of terms.");

            for (int i = 0; i < matrix.NumDocs; i++)
            {
                for (int j = 0; j < matrix.NumTerms; j++)
                {
                    matrix[i, j] -= avg[j];
                    if (matrix[i, j] < 0.0)
                    {
                        matrix[i, j] = 0.0;
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Smoothing filter from ICPC'11 paper "Improving IR-based Traceability Recovery Using Smoothing Filters"
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <param name="IDs">Collection of document ids to smooth.</param>
        /// <returns>Smoothed artifacts</returns>
        public static TermDocumentMatrix Compute(TermDocumentMatrix matrix, IEnumerable<string> IDs)
        {
            double[] avg = WeightUtil.ComputeAverageVector(matrix, IDs);

            if (avg.Length != matrix.NumTerms)
                throw new ArgumentException("Average vector does not have the correct number of terms.");

            foreach (string docID in IDs)
            {
                int i = matrix.GetDocumentIndex(docID);
                for (int j = 0; j < matrix.NumTerms; j++)
                {
                    matrix[i, j] -= avg[j];
                    if (matrix[i, j] < 0.0)
                    {
                        matrix[i, j] = 0.0;
                    }
                }
            }

            return matrix;
        }
    }
}
