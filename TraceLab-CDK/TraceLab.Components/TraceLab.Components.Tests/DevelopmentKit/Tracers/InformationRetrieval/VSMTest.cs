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

using NUnit.Framework;
using System.IO;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;
using TraceLab.Components.Tests.Properties;
using TraceLabSDK.Types;

namespace TraceLab.Components.Tests.DevelopmentKit.Tracers.InformationRetrieval
{
    [TestFixture]
    public class VSMTest
    {
        [Test]
        public void BooleanQueriesAndTFIDFCorpusTest()
        {
            string inputData = Settings.Default.SimpleCorpusDir;
            string outputData = Path.Combine(inputData, "VSM");
            TLArtifactsCollection source = Artifacts.ImportFile(Path.Combine(inputData, "source.txt"));
            TLArtifactsCollection target = Artifacts.ImportFile(Path.Combine(inputData, "target.txt"));
            TLSimilarityMatrix testsims = VSM.Compute(source, target, VSMWeightEnum.BooleanQueriesAndTFIDFCorpus);
            TLSimilarityMatrix realsims = Similarities.Import(Path.Combine(outputData, "output.txt"));
            Assert.AreEqual(testsims.Count, realsims.Count);
            TLLinksList testlinks = testsims.AllLinks;
            TLLinksList reallinks = realsims.AllLinks;
            testlinks.Sort();
            reallinks.Sort();
            for (int i = 0; i < reallinks.Count; i++)
            {
                Assert.AreEqual(testlinks[i].SourceArtifactId, reallinks[i].SourceArtifactId);
                Assert.AreEqual(testlinks[i].TargetArtifactId, reallinks[i].TargetArtifactId);
                Assert.AreEqual(testlinks[i].Score, reallinks[i].Score, Settings.Default.DoublePrecision);
            }
        }
    }
}
