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
using TraceLabSDK.Types;
using TraceLab.Components.DevelopmentKit.Preprocessors;

namespace TraceLab.Components.Tests.DevelopmentKit.Preprocessors
{
    [TestFixture]
    class StopwordsRemoverTest
    {
        [Test]
        public void CleanArtifactsWithStopwords()
        {
            TLArtifactsCollection artifacts = new TLArtifactsCollection();
            artifacts.Add(new TLArtifact("id1", "clean these words"));
            artifacts.Add(new TLArtifact("id2", "this has a stopword"));
            artifacts.Add(new TLArtifact("id3", "an expression"));

            TLStopwords stopwords = new TLStopwords();
            stopwords.Add("these");
            stopwords.Add("this");

            TLArtifactsCollection processedArtifacts = StopwordsRemover.ProcessArtifacts(artifacts, stopwords, 4, false);

            Assert.AreEqual(processedArtifacts["id1"].Text, "clean words");
            Assert.AreEqual(processedArtifacts["id2"].Text, "stopword");
            Assert.AreEqual(processedArtifacts["id3"].Text, "expression");
        }
    }
}
