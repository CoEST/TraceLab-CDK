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
using RPlugin.Core;
using TraceLab.Components.DevelopmentKit;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval;
using TraceLab.Components.RPlugin.Tests.Properties;
using TraceLab.Components.Types.Tracers.InformationRetrieval;
using TraceLabSDK.Types;

namespace TraceLab.Components.Tests.DevelopmentKit.Tracers.InformationRetrieval
{
    [TestFixture]
    public class LSATest
    {
        [Test]
        public void ComputeLSA()
        {
            TLArtifactsCollection source = TermDocumentMatrix.Load(@"../../Data/LSA/source.txt").ToTLArtifactsCollection();
            TLArtifactsCollection target = TermDocumentMatrix.Load(@"../../Data/LSA/target.txt").ToTLArtifactsCollection();
            REngine engine = new REngine(Settings.Default.RScriptEXE);
            TLSimilarityMatrix matrix = (TLSimilarityMatrix)engine.Execute(new LSAScript(source, target, new LSAConfig { Dimensions = 3 }));
            TLSimilarityMatrix correct = Similarities.Import(@"../../Data/LSA/correct.txt");
            foreach (TLSingleLink link in matrix.AllLinks)
            {
                Assert.AreEqual(correct.GetScoreForLink(link.SourceArtifactId, link.TargetArtifactId),
                    link.Score,
                    Settings.Default.DoublePrecision
                );
            }
        }
    }
}
