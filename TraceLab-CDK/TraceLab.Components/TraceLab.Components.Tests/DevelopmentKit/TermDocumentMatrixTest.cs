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
using TraceLab.Components.DevelopmentKit;
using TraceLab.Components.DevelopmentKit.IO;
using TraceLab.Components.Tests.Properties;

namespace TraceLab.Components.Tests
{
    [TestFixture]
    public class TermDocumentMatrixTest
    {
        [Test]
        public void ConstructorTest_Artifacts()
        {
            string inputData = Settings.Default.SimpleCorpusDir;
            string outputData = Path.Combine(inputData, "TermDocumentMatrix");
            TermDocumentMatrix matrix = new TermDocumentMatrix(Artifacts.ImportFile(Path.Combine(inputData, "target.txt")));
            TermDocumentMatrix answer = TermDocumentMatrix.Load(Path.Combine(outputData, "output.txt"));
            // counts
            Assert.AreEqual(matrix.NumDocs, answer.NumDocs);
            Assert.AreEqual(matrix.NumTerms, answer.NumTerms);
            // matrix
            for (int i = 0; i < answer.NumDocs; i++)
            {
                Assert.AreEqual(matrix.GetDocumentName(i), answer.GetDocumentName(i));
                Assert.AreEqual(matrix.GetDocument(i).Length, answer.NumTerms);
                for (int j = 0; j < answer.NumTerms; j++)
                {
                    Assert.AreEqual(matrix.GetTermName(j), answer.GetTermName(j));
                    Assert.AreEqual(matrix[i, j], answer[i, j], 0.0);
                }
            }
        }
    }
}
