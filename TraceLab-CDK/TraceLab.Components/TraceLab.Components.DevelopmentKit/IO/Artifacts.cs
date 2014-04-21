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
using System.Xml;
using System.Xml.XPath;
using TraceLabSDK.Types;

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// SEMERU corpus artifacts importer class
    /// To be replaced by standard TraceLab components
    /// </summary>
    public static class Artifacts
    {
        #region Import

        /// <summary>
        /// Imports a corpus in the form (each line):
        /// ID TEXT TEXT TEXT TEXT TEXT ...
        /// </summary>
        /// <param name="filename">Corpus file location</param>
        /// <returns>Artifacts collection</returns>
        public static TLArtifactsCollection ImportFile(string filename)
        {
            StreamReader file = new StreamReader(filename);
            TLArtifactsCollection answer = new TLArtifactsCollection();
            String line;
            while ((line = file.ReadLine()) != null)
            {
                List<string> artifacts = new List<string>(line.Split());
                String id = artifacts[0].Trim();
                artifacts.RemoveAt(0);
                String doc = String.Join(" ", artifacts);
                answer.Add(new TLArtifact(id, doc));
            }
            return answer;
        }

        /// <summary>
        /// Imports a SEMERU corpus in the form (each line):
        /// ID TEXT TEXT TEXT TEXT TEXT ...
        /// Stores a mapping to the IDs in the order they were read in.
        /// </summary>
        /// <param name="filename">Corpus file location</param>
        /// <param name="map">ID mapping</param>
        /// <returns>Artifacts collection</returns>
        public static TLArtifactsCollection ImportFile(string filename, out List<string> map)
        {
            map = new List<string>();
            StreamReader file = new StreamReader(filename);
            TLArtifactsCollection answer = new TLArtifactsCollection();
            String line;
            while ((line = file.ReadLine()) != null)
            {
                List<string> artifacts = new List<string>(line.Split());
                String id = artifacts[0].Trim();
                artifacts.RemoveAt(0);
                String doc = String.Join(" ", artifacts);
                answer.Add(new TLArtifact(id, doc));
                map.Add(id);
            }
            return answer;
        }

        /// <summary>
        /// Imports a corpus from a directory containing artifacts files.
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="filter">Extension filter</param>
        /// <returns>Artifacts collection</returns>
        public static TLArtifactsCollection ImportDirectory(string path, string filter)
        {
            TLArtifactsCollection artifacts = new TLArtifactsCollection();
            if (String.IsNullOrWhiteSpace(filter))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    artifacts.Add(new TLArtifact(Path.GetFileName(file), File.ReadAllText(file)));
                }
            }
            else
            {
                foreach (string file in Directory.GetFiles(path, "*." + filter))
                {
                    artifacts.Add(new TLArtifact(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file)));
                }
            }
            return artifacts;
        }

        /// <summary>
        /// Imports a corpus from a file containing artifact names that correspond
        /// to the matching line of a file containg the artifact body.
        /// </summary>
        /// <param name="mappingFile">Mapping file</param>
        /// <param name="rawFile">Raw file</param>
        /// <returns>Artifacts collection</returns>
        public static TLArtifactsCollection ImportFromMapping(string mappingFile, string rawFile)
        {
            TextReader map = new StreamReader(mappingFile);
            TextReader raw = new StreamReader(rawFile);
            TLArtifactsCollection artifacts = new TLArtifactsCollection();
            string mapLine, rawLine;
            while ((mapLine = map.ReadLine()) != null)
            {
                if ((rawLine = raw.ReadLine()) == null)
                {
                    rawLine = "";
                }
                artifacts.Add(new TLArtifact(mapLine, rawLine));
            }
            return artifacts;
        }

        /// <summary>
        /// Imports artifacts from an XML file in standard CoEST format.
        /// </summary>
        /// <param name="filepath">Input file path</param>
        /// <param name="trimValues">Trim whitespace from entries?</param>
        /// <returns>Artifacts collection</returns>
        public static TLArtifactsCollection ImportXMLFile(string filepath, bool trimValues)
        {
            TLArtifactsCollection artifacts = new TLArtifactsCollection();
            XPathDocument doc = new XPathDocument(filepath);
            XPathNavigator nav = doc.CreateNavigator();
            //read collection info
            artifacts.CollectionId = ReadSingleXMLNode(filepath, nav, "/artifacts_collection/collection_info/id");
            artifacts.CollectionName = ReadSingleXMLNode(filepath, nav, "/artifacts_collection/collection_info/name");
            artifacts.CollectionVersion = ReadSingleXMLNode(filepath, nav, "/artifacts_collection/collection_info/version");
            artifacts.CollectionDescription = ReadSingleXMLNode(filepath, nav, "/artifacts_collection/collection_info/description");
            if (trimValues)
            {
                artifacts.CollectionId = artifacts.CollectionId.Trim();
                artifacts.CollectionName = artifacts.CollectionName.Trim();
                artifacts.CollectionVersion = artifacts.CollectionVersion.Trim();
                artifacts.CollectionDescription = artifacts.CollectionDescription.Trim();
            }
            //check what type of content location the file has
            XPathNavigator iter = nav.SelectSingleNode("/artifacts_collection/collection_info/content_location");
            string content_location_type = "internal"; //default content location is internal
            //if content location has been sprecified read it
            if (iter != null)
            {
                content_location_type = iter.Value;
            }
            //root dir is going to be needed to external content type, to determine absolute paths of the files
            string rootDir = System.IO.Path.GetDirectoryName(filepath);
            XPathNodeIterator artifactsIterator = nav.Select("/artifacts_collection/artifacts/artifact");
            string artifactId;
            string content;
            while (artifactsIterator.MoveNext())
            {
                iter = artifactsIterator.Current.SelectSingleNode("id");
                artifactId = iter.InnerXml;
                iter = artifactsIterator.Current.SelectSingleNode("content");
                if (content_location_type.Equals("external"))
                {
                    content = System.IO.File.ReadAllText(System.IO.Path.Combine(rootDir, iter.InnerXml.Trim()));
                }
                else
                {
                    content = iter.InnerXml;
                }
                if (trimValues)
                {
                    artifactId = artifactId.Trim();
                    content = content.Trim();
                }
                // Checking if ID is already in Artifacts List
                if (!artifacts.ContainsKey(artifactId))
                {
                    TLArtifact artifact = new TLArtifact(artifactId, content);
                    artifacts.Add(artifactId, artifact);
                }
                else
                {
                    /*
                     CoestDatasetImporterHelper.Logger.Warn(
                        String.Format("Repeated artifact ID '{0}' found in file '{1}'.", artifactId, filepath)
                     );
                    */
                }
            }
            return artifacts;
        }

        /// <summary>
        /// Private method to read single XML node.
        /// </summary>
        /// <param name="filepath">Input file path</param>
        /// <param name="nav">XPath navigator object</param>
        /// <param name="xpath">XPath</param>
        /// <returns>Node value</returns>
        private static string ReadSingleXMLNode(string filepath, XPathNavigator nav, string xpath)
        {
            XPathNavigator iter = nav.SelectSingleNode(xpath);
            if (iter == null)
            {
                throw new XmlException(String.Format("The format of the given file {0} is not correct. The xml node {1} has not been found in the file.", filepath, xpath));
            }
            return iter.InnerXml;
        }

        #endregion

        #region Export

        /// <summary>
        /// Exports a corpus in the form (each line):
        /// ID TEXT TEXT TEXT TEXT TEXT ...
        /// </summary>
        /// <param name="artifacts">Artifacts collection</param>
        /// <param name="outputfile">Output file path</param>
        public static void ExportFile(TLArtifactsCollection artifacts, string outputfile)
        {
            TextWriter tw = new StreamWriter(outputfile);
            foreach (TLArtifact artifact in artifacts.Values)
            {
                tw.WriteLine(artifact.Id + " " + artifact.Text.Replace("\n", " ").Replace("\r", String.Empty));
            }
            tw.Flush();
            tw.Close();
        }

        /// <summary>
        /// Exports a TLArtifactsCollection to standard CoEST XML format.
        /// </summary>
        /// <param name="artifactsCollection">Artifacts collection</param>
        /// <param name="outputPath">Output file path</param>
        /// <param name="collectionId">Collection ID</param>
        /// <param name="name">Collection name</param>
        /// <param name="version">Collection version</param>
        /// <param name="description">Collection description</param>
        public static void ExportXML(TLArtifactsCollection artifactsCollection, string outputPath, string collectionId, string name, string version, string description)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.CheckCharacters = true;
            //create file
            using (XmlWriter writer = XmlWriter.Create(outputPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("artifacts_collection");
                WriteXMLCollectionInfo(writer, collectionId, name, version, description);
                WriteXMLArtifacts(artifactsCollection, writer);
                writer.WriteEndElement(); //artifacts_collection
                writer.WriteEndDocument();
                writer.Close();
            }
            System.Diagnostics.Trace.WriteLine("File created, you can find the file " + outputPath);
        }

        /// <summary>
        /// Private method to write collection information to XmlWriter
        /// </summary>
        /// <param name="writer">XmlWriter object</param>
        /// <param name="collectionId">Collection ID</param>
        /// <param name="name">Collection name</param>
        /// <param name="version">Collection version</param>
        /// <param name="description">Collection description</param>
        private static void WriteXMLCollectionInfo(XmlWriter writer, string collectionId, string name, string version, string description)
        {
            writer.WriteStartElement("collection_info");
            writer.WriteElementString("id", collectionId.Trim());
            writer.WriteElementString("name", name.Trim());
            writer.WriteElementString("version", version.Trim());
            writer.WriteElementString("description", description.Trim());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Private method to write collection to XmlWriter
        /// </summary>
        /// <param name="artifactsCollection">Artifacts collection</param>
        /// <param name="writer">XmlWriter object</param>
        private static void WriteXMLArtifacts(TLArtifactsCollection artifactsCollection, XmlWriter writer)
        {
            writer.WriteStartElement("artifacts");
            foreach (KeyValuePair<string, TLArtifact> artifact in artifactsCollection)
            {
                writer.WriteStartElement("artifact");
                writer.WriteElementString("id", artifact.Value.Id.Trim());
                writer.WriteElementString("content", artifact.Value.Text.Trim());
                writer.WriteElementString("parent_id", String.Empty);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // artifacts
        }

        #endregion
    }
}
