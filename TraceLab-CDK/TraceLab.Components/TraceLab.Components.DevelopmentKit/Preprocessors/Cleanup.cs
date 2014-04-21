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
using System.Text.RegularExpressions;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.Preprocessors
{
    /// <summary>
    /// Provides "cleanup" functionality for preprocessing a raw corpus
    /// </summary>
    public static class Cleanup
    {
        /// <summary>
        /// Processes a TLArtifactsCollection by removing all non-alphanumeric symbols and characters
        /// </summary>
        /// <param name="listOfArtifacts">Artifacts collection</param>
        /// <param name="convertToLowercase">Flag to convert the string to lowercase</param>
        /// <returns>Processed artifacts</returns>
        public static TLArtifactsCollection ProcessArtifacts(TLArtifactsCollection listOfArtifacts, bool convertToLowercase)
        {
            TLArtifactsCollection processed = new TLArtifactsCollection();
            foreach (TLArtifact artifact in listOfArtifacts.Values)
            {
                TLArtifact processedArtifact = new TLArtifact(artifact.Id, String.Empty);
                processedArtifact.Text = ProcessText(artifact.Text, convertToLowercase);
                processed.Add(processedArtifact);
            }
            return processed;
        }

        /// <summary>
        /// Processes a string by removing all non-alphanumeric symbols and characters
        /// </summary>
        /// <param name="text">Input string</param>
        /// /// <param name="convertToLowercase">Flag to convert the string to lowercase</param>
        /// <returns>Processed text</returns>
        public static string ProcessText(string text, bool convertToLowercase)
        {
            string result = string.Empty;
            StringBuilder builder = new StringBuilder();
            // Reduces the text to only characters - using Regular Expressions - TODO: test if this is correct
            string cleanText = Regex.Replace(text, "[^A-Za-z0-9 ]", " ");

            // remove duplicate white spaces... 
            // this method is apparently faster than Regex.Replace(input, "[\s]+", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // for significantly larger files
            string[] parts = cleanText.Split(new char[] { ' ', '\n', '\t', '\r', '\f', '\v' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                builder.AppendFormat("{0} ", part);
            }
            result = builder.ToString().Trim();
            //convert to lower case
            if (convertToLowercase)
            {
                result = result.ToLower();
            }
            return result;

        }
    }
}
