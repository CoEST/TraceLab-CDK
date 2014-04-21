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

namespace TraceLab.Components.DevelopmentKit.Utils
{
    /// <summary>
    /// TLSimilarityMatrix utility functions
    /// </summary>
    public static class TLSimilarityMatrixUtil
    {
        #region Simple statistics

        /// <summary>
        /// Computes the average similarity score of a matrix
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Average similarity score</returns>
        public static double AverageSimilarity(TLSimilarityMatrix matrix)
        {
            return AverageSimilarity(matrix.AllLinks);
        }

        /// <summary>
        /// Computes the average similarity score of a links list
        /// </summary>
        /// <param name="list">Links list</param>
        /// <returns>Average similarity score</returns>
        public static double AverageSimilarity(TLLinksList list)
        {
            double sum = 0;

            foreach (TLSingleLink link in list)
            {
                sum += link.Score;
            }

            return sum / list.Count;
        }

        /// <summary>
        /// Computes the standard deviation of similarity scores for a matrix
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>Standard deviation</returns>
        public static double SimilarityStandardDeviation(TLSimilarityMatrix matrix)
        {
            return SimilarityStandardDeviation(matrix.AllLinks);
        }

        /// <summary>
        /// Computes the standard deviation of similarity scores for a links list
        /// </summary>
        /// <param name="list">Links list</param>
        /// <returns>Standard deviation</returns>
        public static double SimilarityStandardDeviation(TLLinksList list)
        {
            double average = AverageSimilarity(list);
            double sumOfDerivation = 0;

            foreach (TLSingleLink link in list)
            {
                sumOfDerivation += link.Score * link.Score;
            }

            double sumOfDerivationAverage = sumOfDerivation / list.Count;
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new TLSimilarityMatrix out of a links list
        /// </summary>
        /// <param name="list">Links list</param>
        /// <returns>New TLSimilarityMatrix</returns>
        public static TLSimilarityMatrix CreateMatrix(TLLinksList list)
        {
            TLSimilarityMatrix matrix = new TLSimilarityMatrix();
            foreach (TLSingleLink link in list)
            {
                matrix.AddLink(link.SourceArtifactId, link.TargetArtifactId, link.Score);
            }
            return matrix;
        }

        #endregion

        #region Link pruning

        /// <summary>
        /// Returns all links above the matrix set threshold property.
        /// Similar to TLSimilarityMatrix.GetLinksAboveThreshold()
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <returns>List of links above threshold</returns>
        public static TLLinksList GetLinksAboveThreshold(TLSimilarityMatrix matrix)
        {
            TLLinksList links = new TLLinksList();
            foreach (TLSingleLink link in matrix.AllLinks)
            {
                if (link.Score > matrix.Threshold)
                    links.Add(link);
            }
            return links;
        }

        /// <summary>
        /// Returns all links above the given threshold.
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="threshold">Score threshold</param>
        /// <returns>List of links above threshold</returns>
        public static TLLinksList GetLinksAboveThreshold(TLSimilarityMatrix matrix, double threshold)
        {
            TLLinksList links = new TLLinksList();
            foreach (TLSingleLink link in matrix.AllLinks)
            {
                if (link.Score > threshold)
                    links.Add(link);
            }
            return links;
        }

        /// <summary>
        /// Returns the top N scoring links in a matrix.
        /// </summary>
        /// <param name="matrix">Matrix</param>
        /// <param name="topN">Number of links to return</param>
        /// <returns>List of top N links</returns>
        public static TLLinksList GetTopNLinks(TLSimilarityMatrix matrix, int topN)
        {
            if (matrix.AllLinks.Count < topN)
            {
                throw new DevelopmentKitException("Matrix only has " + matrix.AllLinks.Count + " links (" + topN + " requested).");
            }
            if (topN < 1)
            {
                throw new DevelopmentKitException("topN must be greater than 0.");
            }
            TLLinksList links = matrix.AllLinks;
            links.Sort();
            TLLinksList newLinks = new TLLinksList();
            for (int i = 0; i < topN; i++)
            {
                newLinks.Add(links[i]);
            }
            return newLinks;
        }

        /// <summary>
        /// Returns links for the desired recall level.
        /// </summary>
        /// <param name="matrix">Candidate matrix</param>
        /// <param name="answerMatrix">Answer matrix</param>
        /// <param name="level">Desired recall level</param>
        /// <returns>List of links at desired recall</returns>
        public static TLLinksList GetLinksAtRecall(TLSimilarityMatrix matrix, TLSimilarityMatrix answerMatrix, double level)
        {
            if (level <= 0.0 || level > 1.0)
            {
                throw new DevelopmentKitException("Recall level must be between 0 and 1.");
            }
            double totalCorrect = answerMatrix.Count * level;
            int numCorrect = 0;
            TLLinksList links = matrix.AllLinks;
            links.Sort();
            TLLinksList newLinks = new TLLinksList();
            while (links.Count > 0 && numCorrect < totalCorrect)
            {
                TLSingleLink link = links[0];
                if (answerMatrix.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId))
                {
                    numCorrect++;
                }
                newLinks.Add(link);
                links.RemoveAt(0);
            }
            return newLinks;
        }

        /// <summary>
        /// Removes a percentage of links from the top of the list.
        /// </summary>
        /// <param name="matrix">Ranklist</param>
        /// <param name="percent">Percentage to remove</param>
        /// <returns>Trimmed ranklist</returns>
        public static TLLinksList RemoveTopPercentage(TLSimilarityMatrix matrix, double percent)
        {
            return RemoveTopPercentage(matrix.AllLinks, percent);
        }

        /// <summary>
        /// Removes a percentage of links from the top of the list.
        /// </summary>
        /// <param name="links">Ranklist</param>
        /// <param name="percent">Percentage to remove</param>
        /// <returns>Trimmed ranklist</returns>
        public static TLLinksList RemoveTopPercentage(TLLinksList links, double percent)
        {
            if (percent <= 0.0 || percent >= 1.0)
            {
                throw new DevelopmentKitException("Percentage level must be between 0 and 1.");
            }
            TLLinksList remaining = new TLLinksList();
            links.Sort();
            int startIndex = Convert.ToInt32(Math.Ceiling(links.Count * percent)) - 1;
            for (int i = startIndex; i < links.Count; i++)
            {
                TLSingleLink link = links[i];
                remaining.Add(new TLSingleLink(link.SourceArtifactId, link.TargetArtifactId, link.Score));
            }
            return remaining;
        }

        /// <summary>
        /// Removes a percentage of links from the bottom of the list.
        /// </summary>
        /// <param name="matrix">Ranklist</param>
        /// <param name="percent">Percentage to remove</param>
        /// <returns>Trimmed ranklist</returns>
        public static TLLinksList RemoveBottomPercentage(TLSimilarityMatrix matrix, double percent)
        {
            return RemoveBottomPercentage(matrix.AllLinks, percent);
        }

        /// <summary>
        /// Removes a percentage of links from the bottom of the list.
        /// </summary>
        /// <param name="links">Ranklist</param>
        /// <param name="percent">Percentage to remove</param>
        /// <returns>Trimmed ranklist</returns>
        public static TLLinksList RemoveBottomPercentage(TLLinksList links, double percent)
        {
            if (percent <= 0.0 || percent >= 1.0)
            {
                throw new DevelopmentKitException("Percentage level must be between 0 and 1.");
            }
            TLLinksList remaining = new TLLinksList();
            links.Sort();
            int endIndex = Convert.ToInt32(Math.Floor(links.Count * (1 - percent))) - 1;
            for (int i = 0; i < endIndex; i++)
            {
                TLSingleLink link = links[i];
                remaining.Add(new TLSingleLink(link.SourceArtifactId, link.TargetArtifactId, link.Score));
            }
            return remaining;
        }

        /// <summary>
        /// Extracts links containing the given artifact IDs from a similarity matrix.
        /// </summary>
        /// <param name="original">Original matrix</param>
        /// <param name="artifactIDs">List of artifact IDs</param>
        /// <param name="ignoreParameters">Flag to ignore parameter overloads and compare only method names.</param>
        /// <returns>Extracted links</returns>
        public static TLLinksList ExtractLinks(TLSimilarityMatrix original, IEnumerable<string> artifactIDs, bool ignoreParameters)
        {
            return ExtractLinks(original.AllLinks, artifactIDs, ignoreParameters);
        }

        /// <summary>
        /// Extracts links containing the given artifact IDs from a similarity matrix.
        /// </summary>
        /// <param name="original">Original matrix</param>
        /// <param name="artifactIDs">List of artifact IDs</param>
        /// <param name="ignoreParameters">Flag to ignore parameter overloads and compare only method names.</param>
        /// <returns>Extracted links</returns>
        public static TLLinksList ExtractLinks(TLLinksList original, IEnumerable<string> artifactIDs, bool ignoreParameters)
        {
            TLSimilarityMatrix matrix = new TLSimilarityMatrix();
            foreach (TLSingleLink link in original)
            {
                string sourceID = (ignoreParameters && link.SourceArtifactId.IndexOf('(') > 0)
                    ? link.SourceArtifactId.Substring(0, link.SourceArtifactId.IndexOf('('))
                    : link.SourceArtifactId;
                string targetID = (ignoreParameters && link.TargetArtifactId.IndexOf('(') > 0)
                    ? link.TargetArtifactId.Substring(0, link.TargetArtifactId.IndexOf('('))
                    : link.TargetArtifactId;
                if (artifactIDs.Contains(sourceID) || artifactIDs.Contains(targetID))
                {
                    matrix.AddLink(link.SourceArtifactId, link.TargetArtifactId, link.Score);
                }
            }
            return matrix.AllLinks;
        }

        /// <summary>
        /// Gets the target artifact ids present in the ranklist.
        /// </summary>
        /// <param name="matrix">Input matrix</param>
        /// <returns>Set of target artifacts ids</returns>
        public static ISet<string> GetSetOfTargetArtifacts(TLSimilarityMatrix matrix)
        {
            return GetSetOfTargetArtifacts(matrix.AllLinks);
        }

        /// <summary>
        /// Gets the target artifact ids present in the ranklist.
        /// </summary>
        /// <param name="links">Input matrix</param>
        /// <returns>Set of target artifacts ids</returns>
        public static ISet<string> GetSetOfTargetArtifacts(TLLinksList links)
        {
            HashSet<string> artifacts = new HashSet<string>();
            foreach (TLSingleLink link in links)
            {
                artifacts.Add(link.TargetArtifactId);
            }
            return artifacts;
        }

        /// <summary>
        /// Collapses overloaded source artifacts, assigning the best score.
        /// </summary>
        /// <param name="matrix">Similarities</param>
        /// <returns>Collapsed artifacts</returns>
        public static TLSimilarityMatrix CollapseOverloadedTargets(TLSimilarityMatrix matrix)
        {
            Dictionary<string, Dictionary<string, double>> pseudomatrix = new Dictionary<string, Dictionary<string, double>>();
            foreach (TLSingleLink link in matrix.AllLinks)
            {
                if (!pseudomatrix.ContainsKey(link.SourceArtifactId))
                {
                    pseudomatrix.Add(link.SourceArtifactId, new Dictionary<string,double>());
                }
                int startIndex = link.TargetArtifactId.IndexOf('(');
                string target = (startIndex > 0)
                    ? link.TargetArtifactId.Substring(0, startIndex)
                    : link.TargetArtifactId;
                if (!pseudomatrix[link.SourceArtifactId].ContainsKey(target))
                {
                    pseudomatrix[link.SourceArtifactId].Add(target, link.Score);
                }
                else
                {
                    if (link.Score > pseudomatrix[link.SourceArtifactId][target])
                    {
                        pseudomatrix[link.SourceArtifactId][target] = link.Score;
                    }
                }
            }
            TLSimilarityMatrix collapsedMatrix = new TLSimilarityMatrix();
            foreach (string sourceID in pseudomatrix.Keys)
            {
                foreach (string targetID in pseudomatrix[sourceID].Keys)
                {
                    collapsedMatrix.AddLink(sourceID, targetID, pseudomatrix[sourceID][targetID]);
                }
            }
            return collapsedMatrix;
        }

        #endregion
    }
}
