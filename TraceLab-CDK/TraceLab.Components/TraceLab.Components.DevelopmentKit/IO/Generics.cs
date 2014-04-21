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

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Importers for lists of generic types
    /// </summary>
    public static class Generics
    {
        /// <summary>
        /// Imports each line of a file as a string
        /// </summary>
        /// <param name="filename">File to import</param>
        /// <returns>Collection of strings</returns>
        public static string[] ImportStrings(string filename)
        {
            return File.ReadAllLines(filename);
        }

        /// <summary>
        /// Imports each line of a file as a 32bit integer
        /// </summary>
        /// <param name="filename">File to import</param>
        /// <param name="skipInvalid">Flag to skip invalid entries, throws exception otherwise</param>
        /// <returns>Collection of integers</returns>
        public static int[] ImportIntegers(string filename, bool skipInvalid = true)
        {
            List<int> list = new List<int>();
            TextReader file = new StreamReader(filename);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                int number;
                // failure
                if (!Int32.TryParse(line, out number))
                {
                    // skip
                    if (skipInvalid)
                    {
                        continue;
                    }
                    // error
                    else
                    {
                        throw new DevelopmentKitException("Line in invalid format (" + filename + ")");
                    }
                }
                // success
                else
                {
                    list.Add(number);
                }
            }
            file.Close();
            return list.ToArray();
        }

        /// <summary>
        /// Imports each line of a file as a double
        /// </summary>
        /// <param name="filename">File to import</param>
        /// <param name="skipInvalid">Flag to skip invalid entries, throws exception otherwise</param>
        /// <returns>Collection of integers</returns>
        public static double[] ImportDoubles(string filename, bool skipInvalid = true)
        {
            List<double> list = new List<double>();
            TextReader file = new StreamReader(filename);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                double number;
                // failure
                if (!Double.TryParse(line, out number))
                {
                    // skip
                    if (skipInvalid)
                    {
                        continue;
                    }
                    // error
                    else
                    {
                        throw new DevelopmentKitException("Line in invalid format (" + filename + ")");
                    }
                }
                // success
                else
                {
                    list.Add(number);
                }
            }
            file.Close();
            return list.ToArray();
        }
    }
}
