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
    /// User-Driven Combination of Structural and Textual Information
    /// </summary>
    public static class UDCSTI
    {
        /// <summary>
        /// Computes UD-CSTI for known relationships
        /// </summary>
        /// <param name="sims">Similarities</param>
        /// <param name="relationships">Known relationships</param>
        /// <param name="feedback">User feedback</param>
        /// <returns>Updated similarities</returns>
        public static TLSimilarityMatrix Compute(TLSimilarityMatrix sims, TLSimilarityMatrix relationships, TLSimilarityMatrix feedback)
        {
            // new matrix
            TLSimilarityMatrix newMatrix = new TLSimilarityMatrix();
            // compute delta
            double delta = DeltaUtils.Compute(sims);
            // make sure the entire list is sorted
            TLLinksList links = sims.AllLinks;
            links.Sort();
            // end condition
            int correct = 0;
            // iterate over each source-target pair
            while (links.Count > 0 && correct < feedback.Count)
            {
                // get link at top of list
                TLSingleLink link = links[0];
                // check feedback
                if (feedback.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId))
                {
                    correct++;
                    // update related links
                    for (int i = 1; i < links.Count; i++)
                    {
                        if (link.SourceArtifactId.Equals(links[i].SourceArtifactId)
                            && relationships.IsLinkAboveThreshold(link.TargetArtifactId, links[i].TargetArtifactId))
                        {
                            links[i].Score += links[i].Score * delta;
                        }
                    }
                }
                // remove link
                newMatrix.AddLink(link.SourceArtifactId, link.TargetArtifactId, link.Score);
                links.RemoveAt(0);
                // reorder links
                links.Sort();
            }
            return newMatrix;
        }
    }
}
