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

namespace TraceLab.Components.DevelopmentKit.Preprocessors.Splitters
{
    /// <summary>
    /// SEMERU identifier splitting algorithm
    /// </summary>
    public static class SemeruSplitter
    {
        /// <summary>
        /// Splits identifiers for each artifact in an artifacts collection
        /// </summary>
        /// <param name="listOfArtifacts"></param>
        /// <param name="keepCompoundIdentifier"></param>
        /// <returns>Splitted artifacts</returns>
        public static TLArtifactsCollection ProcessArtifacts(TLArtifactsCollection listOfArtifacts, bool keepCompoundIdentifier)
        {
            TLArtifactsCollection processed = new TLArtifactsCollection();
            foreach (TLArtifact artifact in listOfArtifacts.Values)
            {
                TLArtifact processedArtifact = new TLArtifact(artifact.Id, String.Empty);
                processedArtifact.Text = ProcessText(artifact.Text, keepCompoundIdentifier);
                processed.Add(processedArtifact);
            }
            return processed;
        }

        /// <summary>
        /// Splits identifiers in a string.
        /// </summary>
        /// <param name="originalBuffer">Input string</param>
        /// <param name="keepCompoundIdentifier">Flag to keep original term in string alongside split terms</param>
        /// <returns>Splitted string</returns>
        public static string ProcessText(string originalBuffer, bool keepCompoundIdentifier)
	    {
		    string[] words = originalBuffer.Split();
		
		    StringBuilder newBuffer = new StringBuilder();
		    bool isCompoundIdentifier;
		
		    foreach (string word in words)
		    {
			    string originalWord = word;
			    if (word.Length == 0)
				    continue;

                StringBuilder newWord;

			    isCompoundIdentifier = false;
                if (word.IndexOf('_') >= 0)
                {
                    isCompoundIdentifier = true;
                    newWord = new StringBuilder(word.Replace("_", " "));
                }
                else
                {
                    newWord = new StringBuilder(word);
                }

			    for (int i = newWord.Length - 1; i >= 0; i--)
			    {
				    if (Char.IsUpper(newWord.ToString()[i]))
				    {
					    if (i > 0)
						    if (Char.IsLower(newWord.ToString()[i-1]))
						    {
							    newWord.Insert(i, ' ');
							    isCompoundIdentifier=true;
						    }
				    }
				    else
					if (Char.IsLower(newWord.ToString()[i]))
					{
						if (i > 0)
							if (Char.IsUpper(newWord.ToString()[i-1]))
							{
								newWord.Insert(i - 1, ' ');
								isCompoundIdentifier = true;
							}
					}					
			    }
			
			    newBuffer.Append(newWord.ToString().ToLower());
			    newBuffer.Append(' ');
			    if (keepCompoundIdentifier)
			    {
				    if (isCompoundIdentifier)
				    {
					    newBuffer.Append(originalWord.ToLower());
					    newBuffer.Append(' ');
				    }
			    }
		    }
		    return newBuffer.ToString();
	    }
    }
}
