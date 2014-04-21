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
using TraceLab.Components.DevelopmentKit.Preprocessors.Stemmers.Snowball;

namespace TraceLab.Components.Tests.DevelopmentKit.Preprocessors.Stemmers
{
    [TestFixture]
    public class SnowballStemmerTest
    {
        [Test]
        public void EnglishStemmerTest()
        {
            string terms = "jump jumping jumps jumped";
            string[] stem = SnowballStemmer.ProcessText(terms, SnowballStemmerEnum.English).Split();
            #if Verbose
            Console.WriteLine("SnowballStemmerTest.EnglishStemmerTest()");
            Console.WriteLine("Original: {0}", terms);
            Console.WriteLine("Stemmed:  {0}", String.Join(" ", stem));
            #endif
            foreach (string term in stem)
            {
                Assert.AreEqual("jump", term);
            }
        }

        [Test]
        public void GermanStemmerTest()
        {
            string terms = "mochte mochtest mochten mochtet";
            string[] stem = SnowballStemmer.ProcessText(terms, SnowballStemmerEnum.German).Split();
            #if Verbose
            Console.WriteLine("SnowballStemmerTest.GermanStemmerTest()");
            Console.WriteLine("Original: {0}", terms);
            Console.WriteLine("Stemmed:  {0}", String.Join(" ", stem));
            #endif
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual("mocht", stem[i]);
            }
            Assert.AreEqual("mochtet", stem[3]);
        }
    }
}
