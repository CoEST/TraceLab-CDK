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
using TraceLab.Components.DevelopmentKit.Preprocessors.Splitters;

namespace TraceLab.Components.Tests.DevelopmentKit.Preprocessors.Splitters
{
    [TestFixture]
    public class CamelCaseSplitterTest
    {
        [Test]
        public void SimpleSplitterTest()
        {
            String textToProcess = "ThisShouldGetSplittedToSeperateWords thisshouldnot thisShouldBe";
            String expectedResult = "This Should Get Splitted To Seperate Words thisshouldnot this Should Be";
            String result = CamelCaseSplitter.ProcessText(textToProcess, false);
            #if Verbose
            Console.WriteLine("CamelCaseSplitterTest.SimpleSplitterTest()");
            Console.WriteLine("Original: " + textToProcess);
            Console.WriteLine("Expected: " + expectedResult);
            Console.WriteLine("Result:   " + result);
            #endif
            Assert.AreEqual(expectedResult, result); 
        }

        [Test]
        public void SimpleLowercaseTest()
        {
            String textToProcess = "ThisShouldGetSplittedToSeperateWords thisshouldnot thisShouldBe";
            String expectedResult = "this should get splitted to seperate words thisshouldnot this should be";
            String result = CamelCaseSplitter.ProcessText(textToProcess, true);
            #if Verbose
            Console.WriteLine("CamelCaseSplitterTest.SimpleLowercaseTest()");
            Console.WriteLine("Original: " + textToProcess);
            Console.WriteLine("Expected: " + expectedResult);
            Console.WriteLine("Result:   " + result);
            #endif
            Assert.AreEqual(expectedResult, result);
        }
    }
}
