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
    public static class JPDA
    {
        /// <summary>
        /// Generates a BiGramCollection from a file
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>BiGram collection</returns>
        public static BiGramCollection GenerateBiGrams(string filename)
        {
            return GenerateBiGrams(filename, null);
        }

        /// <summary>
        /// Generates a BiGramCollection from a file,
        /// ignoring methods that do not appear in the given set
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <param name="includeOnly">Set of methods to include</param>
        /// <returns>BiGram collection</returns>
        public static BiGramCollection GenerateBiGrams(string filename, ISet<string> includeOnly)
        {
            TextReader traceFile = new StreamReader(filename);
            BiGramCollection bigrams = new BiGramCollection();
            Dictionary<string, List<string>> threadToMethodsStackTranslator = new Dictionary<string, List<string>>();
            int lineNumber = 0;
            string line;
            while ((line = traceFile.ReadLine()) != null)
			{
                lineNumber++;

                if (line.Equals("-- VM Started --"))
					continue;
				if (line.Equals("m-- VM Started --"))
					continue;
				if (line.Equals("-- The application exited --"))
					continue;
				if (line.Length == 1)
					continue;

				string[] traceLineSplit = line.Split('\t');

				if (traceLineSplit.Length >= 3)
				{
					throw new DevelopmentKitException("Error at line " + lineNumber + ": " + line);
				}

				if (traceLineSplit[1][0] == '=')
				{
					// do nothing. This is a report line like ===== main =====
					continue;
				}
					
				String threadName = traceLineSplit[0].Substring(0, traceLineSplit[0].IndexOf(':'));
//					if (threadName.equals("org.eclipse.jdt.internal.ui.text.JavaReconciler"))
//					{
//						continue;
//					}
					
				if (!threadToMethodsStackTranslator.ContainsKey(threadName))
				{
					threadToMethodsStackTranslator.Add(threadName, new List<string>());
				}

				int numberOfPipes = 0;
				// count number of pipes "|"
				for (int i = 0; i < traceLineSplit[0].Length; i++)
				{
					if (traceLineSplit[0][i] == '|')
					{
						numberOfPipes++;
					}
				}
					
				string[] buf = traceLineSplit[1].Split(new string[] {"  --  "}, StringSplitOptions.None);
				string methodName = buf[0];
				string methodPath = buf[1];

				//leave method names like method14, but eliminate everything after a dollar (e.g., method$1)
				int indexOfDollar = methodName.IndexOf('$');
				if (indexOfDollar >= 0)
				{
//						System.out.print(methodName+"->");
					methodName = methodName.Substring(0, indexOfDollar);
//						System.out.println(methodName);
				}

				indexOfDollar = methodPath.IndexOf('$');
				if (indexOfDollar >= 0)
				{	
//						System.out.print(methodPath+"->");
					methodPath = methodPath.Replace('$','.');
//						methodPath=methodPath.substring(0,indexOfDollar);
//						System.out.println(methodPath);
				}
					
				if (methodName.Equals("<init>"))
				{
//						System.out.print("<init>"+"->");
					methodName=methodPath.Substring(methodPath.LastIndexOf(".") + 1);
//						System.out.println(methodName);
				}
					
				if (methodName.Equals("<clinit>"))
				{
//						System.out.print("<cinit>"+"->");
					methodName=methodPath.Substring(methodPath.LastIndexOf(".") + 1);
//						System.out.println(methodName);
				}
					
				if (numberOfPipes == 0)	
				{
                    threadToMethodsStackTranslator[threadName].Clear();
					string fullMethodName = methodPath + "." + methodName;
					//string IDFullMethodName = inputOutput.getPositionOfMethodMappingInCorpus(fullMethodName);
						
					threadToMethodsStackTranslator[threadName].Insert(numberOfPipes, fullMethodName);
					//if its 0, it couldn't have been called by anything
					continue;
				}

                while (threadToMethodsStackTranslator[threadName].Count > numberOfPipes)
				{
                    threadToMethodsStackTranslator[threadName].RemoveAt(numberOfPipes);
				}
										
//					if (threadName.equals("org.eclipse.jdt.internal.ui.text.JavaReconciler"))
//					{
//						//code to deal with the Reconciler thread which sometimes has methods that start with 20-30 pipes and the size of the stack traces is less than that
//						//solution: add an unknownMethod
//						
//						if (currentThreadMethodsStack.size()<numberOfPipes)
//						{
//							int numberOfUnknownMethodsToAdd=numberOfPipes-currentThreadMethodsStack.size();
//							for (int i=0;i<numberOfUnknownMethodsToAdd;i++)
//								currentThreadMethodsStack.add("orgeclipsejdtinternaluitextjavareconciler#unknownMethod");
//						}
//					}

                if (threadToMethodsStackTranslator[threadName].Count < numberOfPipes)
				{
//					numberOfInconsistencies++;
//						System.out.println(currentLine);
					continue;
				}
				string fullMethodName2 = methodPath + "." + methodName;
				//String IDFullMethodName=inputOutput.getPositionOfMethodMappingInCorpus(fullMethodName);
                threadToMethodsStackTranslator[threadName].Insert(numberOfPipes, fullMethodName2);

				string parentMethod = threadToMethodsStackTranslator[threadName][numberOfPipes - 1];
				string childMethod = threadToMethodsStackTranslator[threadName][numberOfPipes];
				//if (parentMethod.id.equals("-1")||childMethod.id.equals("-1"))
				//	continue;

                if (includeOnly != null)
                {
                    if (!includeOnly.Contains(parentMethod))
                    {
                        //Console.WriteLine(String.Format("includeOnly.Contains({0}): {1}", parentMethod, includeOnly.Contains(parentMethod)));
                        //Console.WriteLine("Ignoring parent: " + parentMethod);
                        continue;
                    }
                    if (!includeOnly.Contains(childMethod))
                    {
                        //Console.WriteLine("Ignoring child: " + childMethod);
                        continue;
                    }
                }
					
				bigrams.Add(new BiGram(parentMethod, childMethod));
			}

            traceFile.Close();
            return bigrams;
        }

        /// <summary>
        /// Generates unique methods called in a trace
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <returns>Setof unique methods</returns>
        public static ISet<string> GenerateUniqueMethods(string filename)
        {
            TextReader traceFile = new StreamReader(filename);
            HashSet<string> uniqueMethods = new HashSet<string>();
            int lineNumber = 0;
            string line;
            while ((line = traceFile.ReadLine()) != null)
            {
                lineNumber++;

                if (line.Equals("-- VM Started --"))
                    continue;
                if (line.Equals("m-- VM Started --"))
                    continue;
                if (line.Equals("-- The application exited --"))
                    continue;
                if (line.Length == 1)
                    continue;

                string[] traceLineSplit = line.Split('\t');

                if (traceLineSplit.Length >= 3)
                {
                    throw new DevelopmentKitException("Error at line " + lineNumber + ": " + line);
                }

                if (traceLineSplit[1][0] == '=')
                {
                    // do nothing. This is a report line like ===== main =====
                    continue;
                }

                string[] buf = traceLineSplit[1].Split(new string[] { "  --  " }, StringSplitOptions.None);
                string methodName = buf[0];
                string methodPath = buf[1];

                //leave method names like method14, but eliminate everything after a dollar (e.g., method$1)
                int indexOfDollar = methodName.IndexOf('$');
                if (indexOfDollar >= 0)
                {
                    //						System.out.print(methodName+"->");
                    methodName = methodName.Substring(0, indexOfDollar);
                    //						System.out.println(methodName);
                }

                indexOfDollar = methodPath.IndexOf('$');
                if (indexOfDollar >= 0)
                {
                    //						System.out.print(methodPath+"->");
                    methodPath = methodPath.Replace('$', '.');
                    //						methodPath=methodPath.substring(0,indexOfDollar);
                    //						System.out.println(methodPath);
                }

                if (methodName.Equals("<init>"))
                {
                    //						System.out.print("<init>"+"->");
                    methodName = methodPath.Substring(methodPath.LastIndexOf(".") + 1);
                    //						System.out.println(methodName);
                }

                if (methodName.Equals("<clinit>"))
                {
                    //						System.out.print("<cinit>"+"->");
                    methodName = methodPath.Substring(methodPath.LastIndexOf(".") + 1);
                    //						System.out.println(methodName);
                }

                string fullMethodName = methodPath + "." + methodName;
                uniqueMethods.Add(fullMethodName);
            }

            traceFile.Close();
            return uniqueMethods;
        }
    }
}
