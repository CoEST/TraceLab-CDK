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
using NUnit.Framework;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using TraceLab.Components.DevelopmentKit.Preprocessors.ExecutionTraces;

namespace TraceLab.Components.Tests.DevelopmentKit.Preprocessors.ExecutionTraces.Formats
{
    [TestFixture]
    class JPDATest
    {
        [Test]
        public void GenerateBiGramWithExclusion()
        {
            string dataRoot = @"../../Data/ExecutionTraces/JPDA/";
            string traceFile = dataRoot + "trace.jdpa";
            string oracleFile = dataRoot + "trace.bigrams";
            string methodFile = dataRoot + "trace.include";

            IEnumerable<string> rawMethods = Generics.ImportStrings(methodFile);
            HashSet<string> prunedMethods = new HashSet<string>();
            foreach (string rawMethod in rawMethods)
            {
                prunedMethods.Add(rawMethod.Substring(0, rawMethod.IndexOf('(')));
            }
            //Console.WriteLine("Number of methods: {0}", prunedMethods.Count);

            BiGramCollection bigrams = JPDA.GenerateBiGrams(traceFile, prunedMethods);
            BiGramCollection oracle = BiGrams.Import(oracleFile);

            Assert.AreEqual(oracle.Count, bigrams.Count);

            for (int i = 0; i < oracle.Count; i++)
            {
                //Console.WriteLine(String.Format("{0} -> {1} : {2} -> {3}", oracle[i].Caller, oracle[i].Callee, bigrams[i].Caller, bigrams[i].Callee));
                Assert.AreEqual(oracle[i].Caller, bigrams[i].Caller);
                Assert.AreEqual(oracle[i].Callee, bigrams[i].Callee);
            }
        }

        [Test]
        public void GenerateUniqueMethods()
        {
            string dataRoot = @"../../Data/ExecutionTraces/JPDA/";
            string traceFile = dataRoot + "trace.jdpa";
            string oracleFile = dataRoot + "trace.unique";

            ISet<string> unique = JPDA.GenerateUniqueMethods(traceFile);
            IEnumerable<string> oracle = Generics.ImportStrings(oracleFile);

            Assert.AreEqual(oracle.Count(), unique.Count);

            foreach (string method in oracle)
            {
                Assert.IsTrue(unique.Contains(method));
            }
        }
    }
}
