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

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball
{
    /// <summary>
    /// Implements functionality for stemming artifacts using the Snowball stemming algorithm.
    /// </summary>
    public static class SnowballStemmer
    {
        /// <summary>
        /// Processes an artifacts collection using the Snowball stemming algorithm.
        /// </summary>
        /// <param name="artifacts">Artifacts collection</param>
        /// <param name="langauge">Stemmer language</param>
        /// <returns>Stemmed artifacts</returns>
        public static TLArtifactsCollection ProcessArtifacts(TLArtifactsCollection artifacts, SnowballStemmerEnum langauge)
        {
            TLArtifactsCollection processed = new TLArtifactsCollection();
            foreach (TLArtifact artifact in artifacts.Values)
            {
                TLArtifact processedArtifact = new TLArtifact(artifact.Id, String.Empty);
                processedArtifact.Text = ProcessText(artifact.Text, langauge);
                processed.Add(processedArtifact);
            }
            return processed;
        }

        /// <summary>
        /// Processes a string of terms using the Snowball stemming algorithm.
        /// </summary>
        /// <param name="text">Input string</param>
        /// <param name="language">Stemmer language</param>
        /// <returns>Stemmed terms</returns>
        public static string ProcessText(string text, SnowballStemmerEnum language)
        {
            StringBuilder builder = new StringBuilder();
            string result = string.Empty;
            string stemmedWord;
            char[] delimiterChars = { ' ' };
            string[] tokens = text.Split(delimiterChars);
            ISnowballStemmer stemmer = SnowballStemmerUtils.GetStemmer(language);
            foreach (string token in tokens)
            {
                stemmedWord = stemmer.Stem(token);
                builder.AppendFormat("{0} ", stemmedWord);
            }
            result = builder.ToString().Trim();
            return result;
        }
    }
}
