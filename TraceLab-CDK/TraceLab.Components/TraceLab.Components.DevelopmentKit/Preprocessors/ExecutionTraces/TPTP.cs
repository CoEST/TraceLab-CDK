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
using System.IO;
using System.Linq;
using System.Text;
using TraceLab.Components.Types.Preprocessors.ExecutionTraces;

namespace TraceLab.Components.DevelopmentKit.Preprocessors.ExecutionTraces
{
    /// <summary>
    /// Responsible for analyzing JPDA-formatted execution tracfes
    /// </summary>
    public static class TPTP
    {
        /// <summary>
        /// Generates a BiGramCollection from a file
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>BiGram collection</returns>
        public static BiGramCollection GenerateBiGrams(string filename)
        {
            TextReader brTrace = new StreamReader(filename);
			BiGramCollection bigrams = new BiGramCollection();

			int lineNumber = 0;
			Dictionary<string, string> idToClass = new Dictionary<string,string>();
			Dictionary<string, string> idToMethod = new Dictionary<string,string>();
            // Stack<methodID>
			Dictionary<string, Stack<string>> threadToMethodsStackTranslator = new Dictionary<string, Stack<string>>();
            string currentLine;

			while ((currentLine = brTrace.ReadLine()) != null)
			{
				lineNumber++;
                string[] traceLineSplit;
                // classDef
                if (currentLine.StartsWith("<classDef "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("name=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("sourceName=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("classId=") == false)
						throw new Exception();
                    string name = ExtractValueFromQuotes(traceLineSplit[1]);
					string classID = ExtractValueFromQuotes(traceLineSplit[3]);
					//System.out.println(classID+"\t"+name);
					idToClass.Add(classID, name);
					continue;
				}
				// methodDef
				if (currentLine.StartsWith("<methodDef "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("name=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("signature=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("startLineNumber=") == false)
						throw new Exception();
					if (traceLineSplit[4].StartsWith("endLineNumber=") == false)
						throw new Exception();
					if (traceLineSplit[5].StartsWith("methodId=") == false)
						throw new Exception();
					if (traceLineSplit[6].StartsWith("classIdRef=") == false)
						throw new Exception();
                    string name = ExtractValueFromQuotes(traceLineSplit[1]);
					string signature = ExtractValueFromQuotes(traceLineSplit[2]);
					string methodID = ExtractValueFromQuotes(traceLineSplit[5]);
					string classID = ExtractValueFromQuotes(traceLineSplit[6]);
					//System.out.println("#"+name+"\t"+signature+"\t"+methodID+"\t"+classID);
					//leave all the names in; the methods that are not in the corpus will be eliminated because they will not have an ID  
					if (name.Equals("-init-"))
					{
						string fullClassName = idToClass[classID];
						//System.out.println("\tfullClassName="+fullClassName);
						string className = fullClassName.Substring(fullClassName.LastIndexOf("/") + 1);
						// remove "outer" classes from inner classes
						if (className.IndexOf("$") > 0)
						{
							//System.out.println(currentLine);
							className = className.Substring(className.LastIndexOf("$") + 1);
						}
						name = className;
						//System.out.println("\tclassName="+className);
						//System.out.println("\tname="+name);
					}
                    if (name.EndsWith("$"))
                    {
                        name = name.Substring(0, name.LastIndexOf("$"));
                    }
					string fullMethodName = idToClass[classID] + "#" + name;
					fullMethodName = fullMethodName.Replace('/', '.');
					fullMethodName = fullMethodName.Replace('#','.');
					fullMethodName = fullMethodName.Replace('$','.');
					//System.out.println("MethodName="+fullMethodName);
					idToMethod.Add(methodID, fullMethodName);
					continue;
				}
				// methodEntry
				if (currentLine.StartsWith("<methodEntry "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("threadIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("time=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("methodIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[4].StartsWith("classIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[5].StartsWith("ticket=") == false)
						throw new Exception();
					if (traceLineSplit[6].StartsWith("stackDepth=") == false)
						throw new Exception();						
					string threadID = ExtractValueFromQuotes(traceLineSplit[1]);
					string methodIDTrace = ExtractValueFromQuotes(traceLineSplit[3]);
					//Stack<PairMethodIDTraceMethodIDCorpus> currentThreadMethodsStack=threadToMethodsStackTranslator.get(threadID);
					if (!threadToMethodsStackTranslator.ContainsKey(threadID))
					{
						threadToMethodsStackTranslator.Add(threadID, new Stack<string>());
					}
						
					//string fullMethodName = idToMethod[methodIDTrace];
					//string IDCorpusForFullMethodName = inputOutput.getPositionOfMethodMappingInCorpus(fullMethodName);
						
					//PairMethodIDTraceMethodIDCorpus parentMethod=null;
                    string parentMethodID = null;
					try
					{
						parentMethodID = threadToMethodsStackTranslator[threadID].Peek();
					}
					catch (InvalidOperationException) 
					{
                        // stack is empty
                        // do nothing, we will be adding childMethodID
					}
						
					//PairMethodIDTraceMethodIDCorpus childMethod=new PairMethodIDTraceMethodIDCorpus(methodIDTrace,fullMethodName,IDCorpusForFullMethodName);
                    //string childMethod = fullMethodName;
                    string childMethodID = methodIDTrace;
					threadToMethodsStackTranslator[threadID].Push(childMethodID);
						
					if (threadToMethodsStackTranslator[threadID].Count == 1)
					{
						//if its only 1 element, it couldn't have been called by anything
						continue;
					}

					//if (parentMethod.methodIDCorpus.equals("-1")||childMethod.methodIDCorpus.equals("-1"))
                    if (String.IsNullOrWhiteSpace(parentMethodID) || String.IsNullOrWhiteSpace(childMethodID))
						continue;

                    if (idToMethod[parentMethodID].EndsWith(".class")
                        || idToMethod[parentMethodID].EndsWith(".-clinit-")
                        || idToMethod[parentMethodID].EndsWith(".1")
                        || idToMethod[parentMethodID].Contains(".1.")
                    )
                        continue;

                    if (idToMethod[childMethodID].EndsWith(".class")
                        || idToMethod[childMethodID].EndsWith(".-clinit-")
                        || idToMethod[childMethodID].EndsWith(".1")
                        || idToMethod[childMethodID].Contains(".1.")
                    )
                        continue;

					//do not add "recursive calls". This is due to the fact that these are calls between overwritten methods 
                    if (parentMethodID.Equals(childMethodID) || idToMethod[parentMethodID].Equals(idToMethod[childMethodID]))
						continue;
						
                    bigrams.Add(new BiGram(idToMethod[parentMethodID], idToMethod[childMethodID]));						
					continue;
				}
				// methodExit
				if (currentLine.StartsWith("<methodExit "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("threadIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("methodIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("classIdRef=") == false)
						throw new Exception();
					if (traceLineSplit[4].StartsWith("ticket=") == false)
						throw new Exception();
					if (traceLineSplit[5].StartsWith("time=") == false)
						throw new Exception();
						
					string threadID = ExtractValueFromQuotes(traceLineSplit[1]);
					string methodIDTrace = ExtractValueFromQuotes(traceLineSplit[2]);
					
					//Stack<PairMethodIDTraceMethodIDCorpus> currentThreadMethodsStack=threadToMethodsStackTranslator.get(threadID);
					string topOfStack = threadToMethodsStackTranslator[threadID].Peek();
					if (topOfStack.Equals(methodIDTrace) == false)
						throw new Exception(currentLine + "\r\n" + threadToMethodsStackTranslator[threadID]);
					threadToMethodsStackTranslator[threadID].Pop();

					continue;
				}
			}
			brTrace.Close();
            return bigrams;
        }

        /// <summary>
        /// Generates unique methods called in a trace
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>Setof unique methods</returns>
        public static ISet<string> GenerateUniqueMethods(string filename)
        {
            TextReader brTrace = new StreamReader(filename);
			//System.out.println("Processing File: "+inputOutput.getFileNameTrace(issueID));
				
			HashSet<string> setOfUniqueMethodsTrace = new HashSet<string>();
			int lineNumber = 0;
			Dictionary<string, string> idToClass = new Dictionary<string,string>();
			Dictionary<string, string> idToMethod = new Dictionary<string, string>();

            string currentLine;
			while ((currentLine = brTrace.ReadLine()) !=null)
			{
				lineNumber++;
                string[] traceLineSplit;
                // classDef
				if (currentLine.StartsWith("<classDef "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("name=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("sourceName=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("classId=") == false)
						throw new Exception();

					string name = ExtractValueFromQuotes(traceLineSplit[1]);
					string classID = ExtractValueFromQuotes(traceLineSplit[3]);
					//System.out.println(classID+"\t"+name);
					idToClass.Add(classID, name);
					continue;
				}
				// methodDef
				if (currentLine.StartsWith("<methodDef "))
				{
					traceLineSplit = currentLine.Split(' ');
					if (traceLineSplit[1].StartsWith("name=") == false)
						throw new Exception();
					if (traceLineSplit[2].StartsWith("signature=") == false)
						throw new Exception();
					if (traceLineSplit[3].StartsWith("startLineNumber=") == false)
						throw new Exception();
					if (traceLineSplit[4].StartsWith("endLineNumber=") == false)
						throw new Exception();
					if (traceLineSplit[5].StartsWith("methodId=") == false)
						throw new Exception();
					if (traceLineSplit[6].StartsWith("classIdRef=") == false)
						throw new Exception();

					string name = ExtractValueFromQuotes(traceLineSplit[1]);
					string signature = ExtractValueFromQuotes(traceLineSplit[2]);
					string methodID = ExtractValueFromQuotes(traceLineSplit[5]);
					string classID = ExtractValueFromQuotes(traceLineSplit[6]);
					//System.out.println("#"+name+"\t"+signature+"\t"+methodID+"\t"+classID);
						
					//eliminate names such as "access$123"
					if (name.IndexOf("$") > 0)
						name = name.Substring(0, name.IndexOf("$"));
						
					// clinit are for static blocks
					if (name.Equals("-clinit-"))
					{
						continue;
					}
						
					if (traceLineSplit[1].Equals("name=\"class$\""))
					{
						continue;
					}

					if (name.Equals("-init-"))
					{
						string fullClassName = idToClass[classID];
						//System.out.println("\tfullClassName="+fullClassName);
						string className = fullClassName.Substring(fullClassName.LastIndexOf("/") + 1);

						// remove "outer" classes from inner classes
						if (className.IndexOf("$") > 0)
						{
							//System.out.println(currentLine);
							className = className.Substring(className.LastIndexOf("$") + 1);
						}

						name = className;

						//System.out.println("\tclassName="+className);
						//System.out.println("\tname="+name);
					}
					string fullMethodName = idToClass[classID] + "." + name;
					fullMethodName = fullMethodName.Replace('/','.');
					fullMethodName = fullMethodName.Replace('#','.');
					fullMethodName = fullMethodName.Replace('$','.');
					//System.out.println("MethodName="+fullMethodName);
					idToMethod.Add(methodID, fullMethodName);
					setOfUniqueMethodsTrace.Add(fullMethodName);
					continue;
				}
			}
			brTrace.Close();
            return setOfUniqueMethodsTrace;
        }

        // string: identifier="value"
        private static string ExtractValueFromQuotes(string entry)
        {
            int startPos = entry.IndexOf("\"") + 1;
            int length = entry.LastIndexOf("\"") - startPos;
            return entry.Substring(startPos, length);
        }
    }
}
