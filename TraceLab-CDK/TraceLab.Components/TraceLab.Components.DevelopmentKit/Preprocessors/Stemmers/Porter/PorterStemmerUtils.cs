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
using System.Text;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Porter
{
    /// <summary>
    /// Provides TraceLab functionality for the Porter Stemmer
    /// </summary>
    public static class PorterStemmerUtils
    {
        /// <summary>
        /// Processes a TLArtifactsCollection by stemming terms via the Porter stemming algorithm
        /// </summary>
        /// <param name="listOfArtifacts">Artifacts collection</param>
        /// <returns>Stemmed artifacts collection</returns>
        public static TLArtifactsCollection ProcessArtifacts(TLArtifactsCollection listOfArtifacts)
        {
            TLArtifactsCollection processed = new TLArtifactsCollection();
            foreach (TLArtifact artifact in listOfArtifacts.Values)
            {
                TLArtifact processedArtifact = new TLArtifact(artifact.Id, String.Empty);
                processedArtifact.Text = ProcessText(artifact.Text);
                processed.Add(processedArtifact);
            }
            return processed;
        }

        /// <summary>
        /// Processes a string by stemming terms via the Porter stemming algorithm
        /// </summary>
        /// <param name="textToProcess">Input text</param>
        /// <returns>Stemmed text</returns>
        public static string ProcessText(string textToProcess)
        {
            StringBuilder builder = new StringBuilder();
            string result = string.Empty;
            string stemmedWord;
            char[] delimiterChars = { ' ' };
            string[] tokens = textToProcess.Split(delimiterChars);
            PorterStemmer porterStemmer = new PorterStemmer();
            foreach (string token in tokens)
            {
                stemmedWord = porterStemmer.stemTerm(token);
                builder.AppendFormat("{0} ", stemmedWord);
            }
            result = builder.ToString().Trim();
            return result;
        }
    }
}
