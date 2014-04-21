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

using TraceLab.Components.DevelopmentKit.Utils.TermDocumentMatrixUtils;
using TraceLabSDK.Types;
using System;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Responsible for computing VSM similarities
    /// </summary>
    public static class VSM
    {
        /// <summary>
        /// Computes cosine similarities between documents via the Vector Space Model.
        /// </summary>
        /// <param name="source">Source artifacts</param>
        /// <param name="target">Target artifacts</param>
        /// <param name="weight">Weighting scheme</param>
        /// <returns>Similarity matrix</returns>
        public static TLSimilarityMatrix Compute(TLArtifactsCollection source, TLArtifactsCollection target, VSMWeightEnum weight)
        {
            switch (weight)
            {
                case VSMWeightEnum.TFIDF:
                    return SimilarityUtil.ComputeCosine(WeightUtil.ComputeTFIDF(new TermDocumentMatrix(source, target)), source.Keys, target.Keys);
                case VSMWeightEnum.BooleanQueriesAndTFIDFCorpus:
                    return SimilarityUtil.ComputeCosine(WeightUtil.ComputeBinaryTF(new TermDocumentMatrix(source)), WeightUtil.ComputeTFIDF(new TermDocumentMatrix(target)));
                case VSMWeightEnum.NoWeight:
                    return SimilarityUtil.ComputeCosine(new TermDocumentMatrix(source, target), source.Keys, target.Keys);
                default:
                    throw new NotImplementedException("Unknown weighting scheme \"" + weight + "\"");
            }
        }
    }

    /// <summary>
    /// Weights for VSM
    /// </summary>
    public enum VSMWeightEnum
    {
        /// <summary>
        /// tf-idf (entire matrix)
        /// </summary>
        TFIDF,

        /// <summary>
        /// boolean queries
        /// </summary>
        BooleanQueriesAndTFIDFCorpus,

        /// <summary>
        /// no weighting scheme
        /// </summary>
        NoWeight,
    }
}
