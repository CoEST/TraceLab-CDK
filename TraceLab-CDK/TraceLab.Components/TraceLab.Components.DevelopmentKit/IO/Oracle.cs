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
using System.IO;
using TraceLabSDK.Types;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Responsible for importing answer sets
    /// </summary>
    public static class Oracle
    {
        /// <summary>
        /// Imports an answer set from file in the form (each line):
        /// SOURCE TARGET1 TARGET2 ...
        /// </summary>
        /// <param name="filename">File location</param>
        /// <returns>Similarity matrix (link score 1)</returns>
        public static TLSimilarityMatrix Import(String filename)
        {
            StreamReader file = new StreamReader(filename);
            TLSimilarityMatrix answer = new TLSimilarityMatrix();
            String line;
            while ((line = file.ReadLine()) != null)
            {
                String[] artifacts = line.Split();
                String source = artifacts[0];
                for (int i = 1; i < artifacts.Length; i++)
                {
                    String target = artifacts[i].Trim();
                    if (target != "")
                    {
                        answer.AddLink(source, target, 1);
                    }
                }
            }
            return answer;
        }

        /// <summary>
        /// Exports an answer matrix to a file.
        /// </summary>
        /// <param name="answerMatrix">Answer matrix</param>
        /// <param name="filename">Output file path</param>
        public static void Export(TLSimilarityMatrix answerMatrix, string filename)
        {
            TextWriter tw = null;
            try
            {
                tw = new StreamWriter(filename);
                foreach (string sourceID in answerMatrix.SourceArtifactsIds)
                {
                    tw.Write(sourceID);
                    foreach (string targetID in answerMatrix.GetSetOfTargetArtifactIdsAboveThresholdForSourceArtifact(sourceID))
                    {
                        tw.Write(" " + targetID);
                    }
                    tw.WriteLine();
                }
                tw.Flush();
                tw.Close();
            }
            catch (Exception e)
            {
                if (tw != null)
                {
                    tw.Close();
                }
                throw new DevelopmentKitException("There was an exception writing to file (" + filename + ")", e);
            }
        }

        /// <summary>
        /// Imports an oracle from a directory of files.
        /// Each file is a source artifact containing targets on each line.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static TLSimilarityMatrix ImportDirectory(string directory)
        {
            TLSimilarityMatrix oracle = new TLSimilarityMatrix();
            foreach (string file in Directory.GetFiles(directory))
            {
                string id = Path.GetFileName(file);
                TextReader fReader = new StreamReader(file);
                string line;
                while ((line = fReader.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    oracle.AddLink(id, line, 1);
                }
            }
            return oracle;
        }

        #region XML I/O

        /// <summary>
        /// Exports an answer matrix to XML format
        /// </summary>
        /// <param name="answerSet">Answer matrix</param>
        /// <param name="sourceId">ID of source artifacts</param>
        /// <param name="targetId">ID of target artifacts</param>
        /// <param name="outputPath">Output file path</param>
        public static void ExportXML(TLSimilarityMatrix answerSet, string sourceId, string targetId, string outputPath)
        {
            if (answerSet == null)
            {
                throw new TraceLabSDK.ComponentException("Received null answer similarity matrix");
            }
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.CheckCharacters = true;
            //create file
            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(outputPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("answer_set");
                WriteAnswerSetXMLInfo(writer, sourceId, targetId);
                WriteXMLLinks(answerSet, writer);
                writer.WriteEndElement(); //answer_set
                writer.WriteEndDocument();
                writer.Close();
            }
            //System.Diagnostics.Trace.WriteLine("File created , you can find the file " + outputPath);
        }

        /// <summary>
        /// Private method to write a link to an XmlWriter
        /// </summary>
        /// <param name="writer">XmlWriter object</param>
        /// <param name="sourceId">Source artifact ID</param>
        /// <param name="targetId">Target artifact ID</param>
        private static void WriteAnswerSetXMLInfo(System.Xml.XmlWriter writer, string sourceId, string targetId)
        {
            writer.WriteStartElement("answer_info");
            writer.WriteElementString("source_artifacts_collection", sourceId.Trim());
            writer.WriteElementString("target_artifacts_collection", targetId.Trim());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Private method to write a TLSimilarityMatrix to an XmlWriter
        /// </summary>
        /// <param name="answerSet">Answer matrix</param>
        /// <param name="writer">XmlWriter object</param>
        private static void WriteXMLLinks(TLSimilarityMatrix answerSet, System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("links");
            foreach (TLSingleLink link in answerSet.AllLinks)
            {
                writer.WriteStartElement("link");
                writer.WriteElementString("source_artifact_id", link.SourceArtifactId.Trim());
                writer.WriteElementString("target_artifact_id", link.TargetArtifactId.Trim());
                writer.WriteElementString("confidence_score", link.Score.ToString().Trim());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // artifacts
        }

        /// <summary>
        /// Imports the answer set without validation against source and target artifacts
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="trimValues">if set to <c>true</c> [trim values].</param>
        /// <returns></returns>
        public static TLSimilarityMatrix ImportXML(string filepath, bool trimValues)
        {
            string friendlyAnswerSetFilename = System.IO.Path.GetFileName(filepath);

            TLSimilarityMatrix answerSet = new TLSimilarityMatrix();

            XPathDocument doc = new XPathDocument(filepath);
            XPathNavigator nav = doc.CreateNavigator();

            //read collection info
            XPathNavigator iter = nav.SelectSingleNode("/answer_set/answer_info/source_artifacts_collection");
            string source_artifacts_collection_id = iter.Value;

            iter = nav.SelectSingleNode("/answer_set/answer_info/target_artifacts_collection");
            string target_artifacts_collection_id = iter.Value;

            XPathNodeIterator linksIterator = nav.Select("/answer_set/links/link");

            string source_artifact_id;
            string target_artifact_id;
            double confidence_score;
            while (linksIterator.MoveNext())
            {
                // Parse Source Artifact Id
                iter = linksIterator.Current.SelectSingleNode("source_artifact_id");
                if (iter == null)
                {
                    throw new XmlException(String.Format("The source_artifact_id has not been provided for the link. File location: {0}", filepath));
                }

                source_artifact_id = iter.Value;
                if (trimValues)
                {
                    source_artifact_id = source_artifact_id.Trim();
                }

                // Parse Target Artifact Id
                iter = linksIterator.Current.SelectSingleNode("target_artifact_id");
                if (iter == null)
                {
                    throw new XmlException(String.Format("The target_artifact_id has not been provided for the link. File location: {0}", filepath));
                }

                target_artifact_id = iter.Value;
                if (trimValues)
                {
                    target_artifact_id = target_artifact_id.Trim();
                }

                //Parse confidence score
                iter = linksIterator.Current.SelectSingleNode("confidence_score");
                if (iter == null)
                {
                    //if confidence score is not provided set it to default value 1
                    confidence_score = 1.0;
                }
                else
                {
                    string tmpValue = iter.Value;
                    if (trimValues) tmpValue = tmpValue.Trim();

                    if (double.TryParse(tmpValue, out confidence_score) == false)
                    {
                        throw new XmlException(String.Format("The confidence score provided for link from source artifact {0} to target artifact is in incorrect format {1}. File location: {2}", source_artifact_id, target_artifact_id, filepath));
                    }
                }

                answerSet.AddLink(source_artifact_id, target_artifact_id, confidence_score);
            }

            return answerSet;
        }

        #endregion
    }
}
