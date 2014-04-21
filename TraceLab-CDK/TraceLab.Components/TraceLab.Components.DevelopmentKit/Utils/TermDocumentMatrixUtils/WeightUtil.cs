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
    /// Provides weighting scheme utilities for TermDocumentMatrix
    /// </summary>
    public static class WeightUtil
    {
        /// <summary>
        /// Computes binary (0|1) terms in documents.
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <returns>Term-by-document matrix with 1s for terms that are in the document and 0s for terms that are not.</returns>
        public static TermDocumentMatrix ComputeBinaryTF(TermDocumentMatrix matrix)
        {
            for (int i = 0; i < matrix.NumDocs; i++)
            {
                for (int j = 0; j < matrix.NumTerms; j++)
                {
                    matrix[i, j] = (matrix[i, j] > 0.0) ? 1.0 : 0.0;
                }
            }
            return matrix;
        }

        /// <summary>
        /// Computes the term frequencies of each document.
        /// Each term in a vector is divided by the max term in that vector.
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <returns>tf-weighted term-by-document matrix</returns>
        public static TermDocumentMatrix ComputeTF(TermDocumentMatrix matrix)
        {
            for (int i = 0; i < matrix.NumDocs; i++)
            {
                double max = matrix.GetDocument(i).Max();
                for (int j = 0; j < matrix.NumTerms; j++)
                {
                    matrix[i, j] = matrix[i, j] / max;
                }
            }
            return matrix;
        }

        /// <summary>
        /// Computes the document frequencies of each term
        /// </summary>
        /// <param name="matrix">Term-by-document matrix</param>
        /// <returns>df-weighted term distribution</returns>
        public static double[] ComputeDF(TermDocumentMatrix matrix)
        {
            double[] df = new double[matrix.NumTerms];
            for (int j = 0; j < matrix.NumTerms; j++)
            {
                df[j] = 0.0;
                for (int i = 0; i < matrix.NumDocs; i++)
                {
                    df[j] += (matrix[i, j] > 0.0) ? 1.0 : 0.0;
                }
            }
            return df;
        }

        /// <summary>
        /// Computes the inverse document frequencies of a TermDocumentMatrix
        /// </summary>
        /// <param name="matrix">TDM</param>
        /// <returns>IDF vector</returns>
        public static double[] ComputeIDF(TermDocumentMatrix matrix)
        {
            return ComputeIDF(ComputeDF(matrix), matrix.NumDocs);
        }

        /// <summary>
        /// Computes the inverse document frequencies of a document frequencies vector
        /// </summary>
        /// <param name="df">Document frequencies vector</param>
        /// <param name="numDocs">Number of documents in corpus</param>
        /// <returns>Inverse document frequencies vector</returns>
        public static double[] ComputeIDF(double[] df, int numDocs)
        {
            double[] idf = new double[df.Length];
            for (int i = 0; i < df.Length; i++)
            {
                if (df[i] <= 0.0)
                {
                    idf[i] = 0.0;
                }
                else
                {
                    idf[i] = Math.Log(numDocs / df[i]);
                }
            }
            return idf;
        }

        /// <summary>
        /// Computes tf-idf weights on a TermDocumentMatrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static TermDocumentMatrix ComputeTFIDF(TermDocumentMatrix matrix)
        {
            return ComputeTFIDF(ComputeTF(matrix), ComputeIDF(matrix));
        }

        /// <summary>
        /// Computes tf-idf weights on a TDM that has been TF() and an IDF vector
        /// </summary>
        /// <param name="tf">Term-frequency weighted matrix</param>
        /// <param name="idf">Inverse document frequencies vector</param>
        /// <returns>tf-idf weighted TermDocumentMatrix</returns>
        public static TermDocumentMatrix ComputeTFIDF(TermDocumentMatrix tf, double[] idf)
        {
            for (int i = 0; i < tf.NumDocs; i++)
            {
                for (int j = 0; j < tf.NumTerms; j++)
                {
                    tf[i, j] = tf[i, j] * idf[j];
                }
            }
            return tf;
        }

        /// <summary>
        /// Computes the average term vector of the matrix
        /// </summary>
        /// <param name="matrix">Artifacts</param>
        /// <returns>Average vector</returns>
        public static double[] ComputeAverageVector(TermDocumentMatrix matrix)
        {
            double[] avg = new double[matrix.NumTerms];
            for (int j = 0; j < matrix.NumTerms; j++)
            {
                for (int i = 0; i < matrix.NumDocs; i++)
                {
                    avg[j] += matrix[i, j];
                }
                avg[j] = avg[j] / matrix.NumDocs;
            }
            return avg;
        }

        /// <summary>
        /// Computes a vector of the average weight for each term in the matrix
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <param name="IDs">Collection of artifacts ids</param>
        /// <returns>Average vector</returns>
        public static double[] ComputeAverageVector(TermDocumentMatrix matrix, IEnumerable<string> IDs)
        {
            double[] avg = new double[matrix.NumTerms];
            for (int j = 0; j < matrix.NumTerms; j++)
            {
                foreach (string docID in IDs)
                {
                    int docIndex = matrix.GetDocumentIndex(docID);
                    avg[j] += matrix[docIndex, j];
                }
                avg[j] = avg[j] / IDs.Count();
            }
            return avg;
        }
    }
}
