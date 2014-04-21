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

namespace TraceLab.Components.DevelopmentKit.Utils.TermDocumentMatrixUtils
{
    /// <summary>
    /// Provides similarity score computation utilities for TermDocumentMatrix
    /// </summary>
    public static class SimilarityUtil
    {
        /// <summary>
        /// Computes the dot product of two equal-length vectors.
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        /// <returns>Vector dot product</returns>
        public static double ComputeDotProduct(double[] vec1, double[] vec2)
        {
            if (vec1.Length != vec2.Length)
            {
                throw new DevelopmentKitException("Vectors must be of equal length.");
            }
            double product = 0.0;
            for (int i = 0; i < vec1.Length; i++)
            {
                product += (vec1[i] * vec2[i]);
            }
            return product;
        }

        /// <summary>
        /// Computes the Euclidean length of a vector.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Euclidean length</returns>
        public static double ComputeLength(double[] vector)
        {
            double length = 0.0;
            for (int i = 0; i < vector.Length; i++)
            {
                length += Math.Pow(vector[i], 2);
            }
            return Math.Sqrt(length);
        }

        /// <summary>
        /// Computes cosine similarities between two TermDocumentMatrices.
        /// Cosine similarity is defined as (dot product) / (length * length)
        /// </summary>
        /// <param name="m1">Binary document matrix</param>
        /// <param name="m2">tf-idf weighted document matrix</param>
        /// <returns>Similarity matrix</returns>
        public static TLSimilarityMatrix ComputeCosine(TermDocumentMatrix m1, TermDocumentMatrix m2)
        {
            TLSimilarityMatrix sims = new TLSimilarityMatrix();
            List<TermDocumentMatrix> matrices = TermDocumentMatrix.Equalize(m1, m2);
            for (int i = 0; i < m1.NumDocs; i++)
            {
                TLLinksList links = new TLLinksList();
                for (int j = 0; j < m2.NumDocs; j++)
                {
                    double lengthProduct = ComputeLength(matrices[0].GetDocument(i)) * ComputeLength(matrices[1].GetDocument(j));
                    if (lengthProduct == 0.0)
                    {
                        links.Add(new TLSingleLink(m1.GetDocumentName(i), m2.GetDocumentName(j), 0.0));
                    }
                    else
                    {
                        links.Add(new TLSingleLink(m1.GetDocumentName(i), m2.GetDocumentName(j), ComputeDotProduct(matrices[0].GetDocument(i), matrices[1].GetDocument(j)) / lengthProduct));
                    }
                }
                links.Sort();
                foreach (TLSingleLink link in links)
                {
                    sims.AddLink(link.SourceArtifactId, link.TargetArtifactId, link.Score);
                }
            }
            return sims;
        }

        /// <summary>
        /// Computes the cosine similarity between the given document pairs in the matrix
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <param name="sourceIDs">Collection of source artifacts ids</param>
        /// /// <param name="targetIDs">Collection of target artifacts ids</param>
        /// <returns>Similarity matrix</returns>
        public static TLSimilarityMatrix ComputeCosine(TermDocumentMatrix matrix, IEnumerable<string> sourceIDs, IEnumerable<string> targetIDs)
        {
            TLSimilarityMatrix sims = new TLSimilarityMatrix();
            foreach (string sourceID in sourceIDs)
            {
                double[] sourceDoc = matrix.GetDocument(sourceID);
                foreach (string targetID in targetIDs)
                {
                    // compute cosine similarity between source and target
                    double[] targetDoc = matrix.GetDocument(targetID);
                    double lengthProduct = ComputeLength(sourceDoc) * ComputeLength(targetDoc);
                    if (lengthProduct == 0.0)
                    {
                        sims.AddLink(sourceID, targetID, 0.0);
                    }
                    else
                    {
                        double score = ComputeDotProduct(sourceDoc, targetDoc) / lengthProduct;
                        sims.AddLink(sourceID, targetID, score);
                    }
                }
            }
            return sims;
        }
    }
}
