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
using System.IO;
using System.Linq;
using System.Text;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Responsible for importing and exporting BiGramCollections to disk
    /// </summary>
    public static class BiGrams
    {
        /// <summary>
        /// Exports a BiGramCollection to disk
        /// </summary>
        /// <param name="bigrams">BiGram collection</param>
        /// <param name="filename">Output file</param>
        public static void Export(BiGramCollection bigrams, string filename)
        {
            TextWriter file = new StreamWriter(filename);
            foreach (BiGram bigram in bigrams)
            {
                file.WriteLine("{0}\t{1}", bigram.Caller, bigram.Callee);
            }
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// Imports a BiGramCollection from disk
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>BiGram collection</returns>
        public static BiGramCollection Import(string filename)
        {
            TextReader file = new StreamReader(filename);
            BiGramCollection bigrams = new BiGramCollection();
            int lineNumber = 0;
            string line;
            while ((line = file.ReadLine()) != null)
            {
                lineNumber++;
                if (String.IsNullOrWhiteSpace(line))
                    continue;
                string caller, callee;
                try
                {
                    string[] split = line.Split('\t');
                    caller = split[0];
                    callee = split[1];
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new DevelopmentKitException("Error in bigram file format on line " + lineNumber, e);
                }
                bigrams.Add(new BiGram(caller, callee));
            }
            return bigrams;
        }
    }
}
