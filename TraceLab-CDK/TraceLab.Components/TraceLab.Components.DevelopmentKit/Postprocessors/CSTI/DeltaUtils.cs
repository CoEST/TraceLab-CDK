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
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Postprocessors.CSTI
{
    /// <summary>
    /// Calculations for computing delta in O-CSTI and UD-CSTI
    /// </summary>
    public static class DeltaUtils
    {
        /// <summary>
        /// Compute the delta value for each source artifact, then return the median of the delta values
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>delta</returns>
        public static double Compute(TLSimilarityMatrix matrix)
        {
            List<double> DeltaLookup = new List<double>();
            foreach (string source in matrix.SourceArtifactsIds)
            {
                DeltaLookup.Add(ComputeForSourceArtifact(matrix, source));
            }
            DeltaLookup.Sort();
            if (DeltaLookup.Count % 2 == 0)
            {
                return (DeltaLookup[DeltaLookup.Count / 2] + DeltaLookup[(DeltaLookup.Count / 2) + 1]) / 2.0;
            }
            else
            {
                return DeltaLookup[Convert.ToInt32(Math.Ceiling(DeltaLookup.Count / 2.0))];
            }
        }

        /// <summary>
        /// Computes the delta value for an individual artifact
        /// </summary>
        /// <param name="matrix">Similarities</param>
        /// <param name="source">Source artifact id</param>
        /// <returns>delta</returns>
        public static double ComputeForSourceArtifact(TLSimilarityMatrix matrix, string source)
        {
            matrix.Threshold = double.MinValue;
            double min = Double.MaxValue;
            double max = Double.MinValue;
            foreach (TLSingleLink link in matrix.GetLinksAboveThresholdForSourceArtifact(source))
            {
                if (link.Score < min)
                {
                    min = link.Score;
                }
                if (link.Score > max)
                {
                    max = link.Score;
                }
            }
            double delta = (max - min) / 2.0;
            // according to R scripts
            if (delta < 0.05)
            {
                delta = Math.Pow(delta, 4) / 4;
            }
            return delta;
        }
    }
}
