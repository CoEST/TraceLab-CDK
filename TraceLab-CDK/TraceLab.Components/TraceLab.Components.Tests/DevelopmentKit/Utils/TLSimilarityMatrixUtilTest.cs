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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TraceLab.Components.DevelopmentKit.Utils;
using TraceLabSDK.Types;

namespace TraceLab.Components.Tests.DevelopmentKit.Utils
{
    [TestFixture]
    public class TLSimilarityMatrixUtilTest
    {
        #region Setup

        private TLSimilarityMatrix sims;
        private TLSimilarityMatrix oracle;

        [SetUp]
        public void RunBeforeEachTest()
        {
            oracle = new TLSimilarityMatrix();
            oracle.AddLink("A", "B*", 1);
            oracle.AddLink("A", "C*", 1);
            oracle.AddLink("A", "D*", 1);
            sims = new TLSimilarityMatrix();
            /* Sorted order:
             * sims.AddLink("A", "B*", 10);
             * sims.AddLink("A", "E",  9);
             * sims.AddLink("A", "F",  8);
             * sims.AddLink("A", "C*", 7);
             * sims.AddLink("A", "G",  6);
             * sims.AddLink("A", "H",  5);
             * sims.AddLink("A", "I",  4);
             * sims.AddLink("A", "J",  3);
             * sims.AddLink("A", "D*", 2);
             * sims.AddLink("A", "K",  1);
             */
            sims.AddLink("A", "G", 6);
            sims.AddLink("A", "K", 1);
            sims.AddLink("A", "B*", 10);
            sims.AddLink("A", "E", 9);
            sims.AddLink("A", "J", 3);
            sims.AddLink("A", "F", 8);
            sims.AddLink("A", "C*", 7);
            sims.AddLink("A", "H", 5);
            sims.AddLink("A", "D*", 2);
            sims.AddLink("A", "I", 4);
        }

        #endregion

        #region Link pruning tests

        [Test]
        public void GetLinksAtRecall100()
        {
            TLLinksList list = TLSimilarityMatrixUtil.GetLinksAtRecall(sims, oracle, 1.0);
            #if Verbose
            Console.WriteLine("TLSimilarityMatrixUtilTest.GetLinksAtRecall100()");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    list[i].SourceArtifactId,
                    list[i].TargetArtifactId,
                    list[i].Score
                );
            }
            #endif
            Assert.AreEqual(9, list.Count);
            TLLinksList expected = new TLLinksList();
            expected.Add(new TLSingleLink("A", "B*", 10));
            expected.Add(new TLSingleLink("A", "E",  9));
            expected.Add(new TLSingleLink("A", "F",  8));
            expected.Add(new TLSingleLink("A", "C*", 7));
            expected.Add(new TLSingleLink("A", "G",  6));
            expected.Add(new TLSingleLink("A", "H",  5));
            expected.Add(new TLSingleLink("A", "I",  4));
            expected.Add(new TLSingleLink("A", "J",  3));
            expected.Add(new TLSingleLink("A", "D*", 2));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], list[i]);
            }
        }

        [Test]
        public void GetLinksAtRecall23()
        {
            TLLinksList list = TLSimilarityMatrixUtil.GetLinksAtRecall(sims, oracle, 2.0 / 3);
            #if Verbose
            Console.WriteLine("TLSimilarityMatrixUtilTest.GetLinksAtRecall23()");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    list[i].SourceArtifactId,
                    list[i].TargetArtifactId,
                    list[i].Score
                );
            }
            #endif
            Assert.AreEqual(4, list.Count);
            TLLinksList expected = new TLLinksList();
            expected.Add(new TLSingleLink("A", "B*", 10));
            expected.Add(new TLSingleLink("A", "E", 9));
            expected.Add(new TLSingleLink("A", "F", 8));
            expected.Add(new TLSingleLink("A", "C*", 7));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], list[i]);
            }
        }

        [Test]
        public void GetTopNLinks()
        {
            TLLinksList list = TLSimilarityMatrixUtil.GetTopNLinks(sims, 4);
            #if Verbose
            Console.WriteLine("TLSimilarityMatrixUtilTest.GetTopNLinks()");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    list[i].SourceArtifactId,
                    list[i].TargetArtifactId,
                    list[i].Score
                );
            }
            #endif
            Assert.AreEqual(4, list.Count);
            TLLinksList expected = new TLLinksList();
            expected.Add(new TLSingleLink("A", "B*", 10));
            expected.Add(new TLSingleLink("A", "E", 9));
            expected.Add(new TLSingleLink("A", "F", 8));
            expected.Add(new TLSingleLink("A", "C*", 7));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], list[i]);
            }
        }

        [Test]
        public void GetLinksAboveThresholdDefault()
        {
            sims.Threshold = 4;
            TLLinksList list = TLSimilarityMatrixUtil.GetLinksAboveThreshold(sims);
            list.Sort();
            #if Verbose
            Console.WriteLine("TLSimilarityMatrixUtilTest.GetLinksAboveThresholdDefault()");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    list[i].SourceArtifactId,
                    list[i].TargetArtifactId,
                    list[i].Score
                );
            }
            #endif
            Assert.AreEqual(6, list.Count);
            TLLinksList expected = new TLLinksList();
            expected.Add(new TLSingleLink("A", "B*", 10));
            expected.Add(new TLSingleLink("A", "E", 9));
            expected.Add(new TLSingleLink("A", "F", 8));
            expected.Add(new TLSingleLink("A", "C*", 7));
            expected.Add(new TLSingleLink("A", "G", 6));
            expected.Add(new TLSingleLink("A", "H", 5));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], list[i]);
            }
        }

        [Test]
        public void GetLinksAboveThresholdProvided()
        {
            TLLinksList list = TLSimilarityMatrixUtil.GetLinksAboveThreshold(sims, 4);
            list.Sort();
            #if Verbose
            Console.WriteLine("TLSimilarityMatrixUtilTest.GetLinksAboveThresholdProvided()");
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("{0}\t{1}\t{2}",
                    list[i].SourceArtifactId,
                    list[i].TargetArtifactId,
                    list[i].Score
                );
            }
            #endif
            Assert.AreEqual(6, list.Count);
            TLLinksList expected = new TLLinksList();
            expected.Add(new TLSingleLink("A", "B*", 10));
            expected.Add(new TLSingleLink("A", "E", 9));
            expected.Add(new TLSingleLink("A", "F", 8));
            expected.Add(new TLSingleLink("A", "C*", 7));
            expected.Add(new TLSingleLink("A", "G", 6));
            expected.Add(new TLSingleLink("A", "H", 5));
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], list[i]);
            }
        }

        #endregion
    }
}
