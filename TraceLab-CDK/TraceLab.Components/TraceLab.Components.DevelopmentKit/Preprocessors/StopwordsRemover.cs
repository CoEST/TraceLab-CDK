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
using TraceLabSDK;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Preprocessors
{
    /// <summary>
    /// Provides stopwords removal functionality
    /// </summary>
    public static class StopwordsRemover
    {
        /// <summary>
        /// Processes a TLArtifactsCollection by removing terms that match the given list of stopwords
        /// </summary>
        /// <param name="listOfArtifacts">Artifacts collection</param>
        /// <param name="stopwords">Stopwords collection</param>
        /// <param name="minWordLength">Minimum word length</param>
        /// <param name="removeNumbers">Flag to remove numbers</param>
        /// <returns>Processed artifacts</returns>
        public static TLArtifactsCollection ProcessArtifacts(TLArtifactsCollection listOfArtifacts, TLStopwords stopwords, int minWordLength, bool removeNumbers)
        {
            TLArtifactsCollection processed = new TLArtifactsCollection();
            foreach (TLArtifact artifact in listOfArtifacts.Values)
            {
                TLArtifact processedArtifact = new TLArtifact(artifact.Id, String.Empty);
                processedArtifact.Text = ProcessText(artifact.Text, stopwords, minWordLength, removeNumbers);
                processed.Add(processedArtifact);
            }
            return processed;
        }

        /// <summary>
        /// Processes a string by removing terms that match the given list of stopwords
        /// </summary>
        /// <param name="textToProcess">Input text</param>
        /// <param name="stopwords">Stopwords collection</param>
        /// <param name="minWordLength">Minimum word length</param>
        /// <param name="removeNumbers">Flag to remove numbers</param>
        /// <returns>Processed text</returns>
        public static string ProcessText(string textToProcess, TLStopwords stopwords, int minWordLength, bool removeNumbers)
        {
            StringBuilder builder = new StringBuilder();
            string result = string.Empty;
            string[] tokens = textToProcess.Split();
            foreach (string token in tokens)
            {
                if (!stopwords.Contains(token) && token.Length >= minWordLength)
                {
                    if (removeNumbers && IsNumber(token))
                    {
                        continue;
                    }
                    else
                    {
                        builder.AppendFormat("{0} ", token);
                    }
                }
            }
            result = builder.ToString().Trim();
            return result;
        }

        private static bool IsNumber(string text)
        {
            int len = text.Length;
            for (int i = 0; i < len; ++i)
            {
                char c = text[i];
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
