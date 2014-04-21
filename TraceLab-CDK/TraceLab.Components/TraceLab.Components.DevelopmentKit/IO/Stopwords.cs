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

using System.IO;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Responsible for importing stopwords.
    /// </summary>
    public static class Stopwords
    {
        /// <summary>
        /// Imports stopwords from file. Each line is one stopword.
        /// </summary>
        /// <param name="filepath">Input file path</param>
        /// <returns>Stopwords collection</returns>
        public static TLStopwords Import(string filepath)
        {
            TLStopwords stopwords = new TLStopwords();
            TextReader reader = new StreamReader(filepath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                stopwords.Add(line);
            }
            return stopwords;
        }
    }
}
