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
using TraceLab.Components.DevelopmentKit.Preprocessors.ExecutionTraces;
using TraceLab.Components.RPlugin.Tests.Properties;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using TraceLabSDK.Types;

namespace TraceLab.Components.Tests.DevelopmentKit.Tracers.WebMining
{
    internal static class WebMiningTestUtils
    {
        internal static TLSimilarityMatrix GenerateOracle(string rankFile, string mapFile)
        {
            Console.WriteLine("Generating oracle...");
            IEnumerable<double> ranks = Generics.ImportDoubles(rankFile, false);
            IEnumerable<string> map = Generics.ImportStrings(mapFile);
            Assert.AreEqual(map.Count(), ranks.Count());
            TLSimilarityMatrix oracle = new TLSimilarityMatrix();
            for (int i = 0; i < map.Count(); i++)
            {
                oracle.AddLink("trace", map.ElementAt(i), ranks.ElementAt(i));
            }
            return oracle;
        }

        internal static PDG GeneratePDG(string traceFile, IEnumerable<string> rawMethods)
        {
            Console.WriteLine("Generating input PDG...");
            HashSet<string> prunedMethods = new HashSet<string>();
            foreach (string rawMethod in rawMethods)
            {
                prunedMethods.Add(rawMethod.Substring(0, rawMethod.IndexOf('(')));
            }
            return PDG.Convert(JPDA.GenerateBiGrams(traceFile, prunedMethods));
        }

        internal static void CompareResults(TLSimilarityMatrix oracle, TLSimilarityMatrix results, IEnumerable<string> rawMethods)
        {
            Console.WriteLine("Comparing results...");
            Assert.AreEqual(oracle.Count, results.Count);
            foreach (string oracleMethod in oracle.GetSetOfTargetArtifactIdsAboveThresholdForSourceArtifact("trace"))
            {
                string rawMethod = rawMethods.ElementAt(Convert.ToInt32(oracleMethod) - 1);
                string method = rawMethod.Substring(0, rawMethod.IndexOf('('));
                //Console.WriteLine(oracleMethod + ": " + method);
                Assert.IsTrue(results.IsLinkAboveThreshold("trace", method));
                Assert.AreEqual(oracle.GetScoreForLink("trace", oracleMethod), results.GetScoreForLink("trace", method), Settings.Default.DoublePrecision);
            }
        }
    }
}
