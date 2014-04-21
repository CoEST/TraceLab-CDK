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
using NUnit.Framework;
using RPlugin.Core;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.DevelopmentKit.Tracers.WebMining;
using TraceLab.Components.RPlugin.Tests.Properties;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;
using TraceLab.Components.Types.Tracers.WebMining;
using TraceLabSDK.Types;

namespace TraceLab.Components.Tests.DevelopmentKit.Tracers.WebMining
{
    [TestFixture]
    class PageRankTest
    {
        string dataRoot;
        string corpusMethodsFile;
        string traceRoot;
        string mapFile;
        string traceFile;
        double beta;
        double epsilon;
        IEnumerable<string> rawMethods;

        [SetUp]
        public void RunBefore()
        {
            dataRoot = @"../../Data/PageRankAndHITS/";
            corpusMethodsFile = dataRoot + "corpus.methods";
            traceRoot = dataRoot + "trace950961";
            mapFile = traceRoot + ".mapping";
            traceFile = traceRoot + ".log";
            beta = 0.85;
            epsilon = 0.001;
            rawMethods = Generics.ImportStrings(corpusMethodsFile);
        }

        [Test]
        public void FrequencyTest()
        {
            string rankFile = traceRoot + ".PageRank.frequency.ranks.Beta85";
            TLSimilarityMatrix oracle = WebMiningTestUtils.GenerateOracle(rankFile, mapFile);
            PDG pdg = WebMiningTestUtils.GeneratePDG(traceFile, rawMethods);
            Console.WriteLine("Executing script...");
            REngine engine = new REngine(Settings.Default.RScriptEXE);
            RScript script = new PageRankScript("trace", pdg,
                new PageRankConfig
                {
                    Epsilon = epsilon,
                    Beta = beta,
                    Weight = WebMiningWeightEnum.Frequency,
                }
            );
            TLSimilarityMatrix results = (TLSimilarityMatrix)engine.Execute(script);
            WebMiningTestUtils.CompareResults(oracle, results, rawMethods);
        }

        [Test]
        public void BinaryTest()
        {
            string rankFile = traceRoot + ".PageRank.binary.ranks.Beta85";
            TLSimilarityMatrix oracle = WebMiningTestUtils.GenerateOracle(rankFile, mapFile);
            PDG pdg = WebMiningTestUtils.GeneratePDG(traceFile, rawMethods);
            Console.WriteLine("Executing script...");
            REngine engine = new REngine(Settings.Default.RScriptEXE);
            RScript script = new PageRankScript("trace", pdg,
                new PageRankConfig
                {
                    Epsilon = epsilon,
                    Beta = beta,
                    Weight = WebMiningWeightEnum.Binary,
                }
            );
            TLSimilarityMatrix results = (TLSimilarityMatrix)engine.Execute(script);
            WebMiningTestUtils.CompareResults(oracle, results, rawMethods);
        }
    }
}
