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
    class HITSTest
    {
        string dataRoot;
        string corpusMethodsFile;
        string traceRoot;
        string mapFile;
        string traceFile;
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
            epsilon = 0.001;
            rawMethods = Generics.ImportStrings(corpusMethodsFile);
        }

        [Test]
        public void FrequencyTest()
        {
            string authorityFile = traceRoot + ".HITS.frequency.ranks.authorities";
            string hubFile = traceRoot + ".HITS.frequency.ranks.hubs";
            TLSimilarityMatrix oracleAuthority = WebMiningTestUtils.GenerateOracle(authorityFile, mapFile);
            TLSimilarityMatrix oracleHub = WebMiningTestUtils.GenerateOracle(hubFile, mapFile);
            PDG pdg = WebMiningTestUtils.GeneratePDG(traceFile, rawMethods);
            Console.WriteLine("Executing script...");
            REngine engine = new REngine(Settings.Default.RScriptEXE);
            RScript script = new HITSScript("trace", pdg,
                new HITSConfig
                {
                    Epsilon = epsilon,
                    Weight = WebMiningWeightEnum.Frequency,
                }
            );
            HITSResult results = (HITSResult)engine.Execute(script);
            WebMiningTestUtils.CompareResults(oracleAuthority, results.Authorities, rawMethods);
            WebMiningTestUtils.CompareResults(oracleHub, results.Hubs, rawMethods);
        }

        [Test]
        public void BinaryTest()
        {
            string authorityFile = traceRoot + ".HITS.binary.ranks.authorities";
            string hubFile = traceRoot + ".HITS.binary.ranks.hubs";
            TLSimilarityMatrix oracleAuthority = WebMiningTestUtils.GenerateOracle(authorityFile, mapFile);
            TLSimilarityMatrix oracleHub = WebMiningTestUtils.GenerateOracle(hubFile, mapFile);
            PDG pdg = WebMiningTestUtils.GeneratePDG(traceFile, rawMethods);
            Console.WriteLine("Executing script...");
            REngine engine = new REngine(Settings.Default.RScriptEXE);
            RScript script = new HITSScript("trace", pdg,
                new HITSConfig
                {
                    Epsilon = epsilon,
                    Weight = WebMiningWeightEnum.Binary,
                }
            );
            HITSResult results = (HITSResult)engine.Execute(script);
            WebMiningTestUtils.CompareResults(oracleAuthority, results.Authorities, rawMethods);
            WebMiningTestUtils.CompareResults(oracleHub, results.Hubs, rawMethods);
        }
    }
}
