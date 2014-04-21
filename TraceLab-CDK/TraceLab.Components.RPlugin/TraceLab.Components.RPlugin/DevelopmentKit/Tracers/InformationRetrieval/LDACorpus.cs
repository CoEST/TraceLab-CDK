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
using TraceLabSDK.Types;
using TraceLab.Components.DevelopmentKit;
using RPlugin.Core;

namespace TraceLab.Components.DevelopmentKit.Tracers.InformationRetrieval
{
    /// <summary>
    /// Representation of corpus in package `lda`
    /// </summary>
    public class LDACorpus
    {
        #region Private members

        private TermDocumentMatrix _matrix;
        private IEnumerable<string> _sourceDocs;
        private IEnumerable<string> _targetDocs;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Corpus name</param>
        /// <param name="source">Source artifacts</param>
        /// <param name="target">Target artifacts</param>
        public LDACorpus(string name, TLArtifactsCollection source, TLArtifactsCollection target)
        {
            Name = name;
            TermDocumentMatrix sMatrix = new TermDocumentMatrix(source);
            TermDocumentMatrix tMatrix = new TermDocumentMatrix(target);
            _sourceDocs = sMatrix.DocMap;
            _targetDocs = tMatrix.DocMap;
            _matrix = TermDocumentMatrix.Combine(sMatrix, tMatrix);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Corpus name</param>
        /// <param name="source">Source matrix</param>
        /// <param name="target">Target matrix</param>
        public LDACorpus(string name, TermDocumentMatrix source, TermDocumentMatrix target)
        {
            Name = name;
            _sourceDocs = source.DocMap;
            _targetDocs = target.DocMap;
            _matrix = TermDocumentMatrix.Combine(source, target);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Corpus name</param>
        /// <param name="matrix">Input matrix</param>
        /// <param name="sourceIDs">Collection of source artifacts ids</param>
        /// <param name="targetIDs">Collection of target artifacts ids</param>
        public LDACorpus(string name, TermDocumentMatrix matrix, IEnumerable<string> sourceIDs, IEnumerable<string> targetIDs)
        {
            Name = name;
            _sourceDocs = sourceIDs;
            _targetDocs = targetIDs;
            _matrix = matrix;
        }

        #endregion

        #region Public accessors

        /// <summary>
        /// Corpus name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Edge list in R format
        /// </summary>
        public string Edges
        {
            get
            {
                StringBuilder edges = new StringBuilder();
                foreach (string sourceID in _sourceDocs)
                {
                    foreach (string targetID in _targetDocs)
                    {
                        edges.AppendFormat("{0} {1}", _matrix.GetDocumentIndex(sourceID), _matrix.GetDocumentIndex(targetID));
                        edges.AppendLine();
                    }
                }
                return edges.ToString();
            }
        }

        /// <summary>
        /// Links in R format
        /// </summary>
        public string Links
        {
            get
            {
                StringBuilder list = new StringBuilder("list(");
                List<string> links = new List<string>();
                for (int i = 0; i < _matrix.NumDocs; i++)
                {
                    links.Add("integer(0)");
                }
                list.Append(String.Join(",", links));
                list.Append(")");
                return list.ToString();
            }
        }

        /// <summary>
        /// Term list in R format
        /// </summary>
        public string Vocab
        {
            get
            {
                return "c(\"" + String.Join("\",\"", _matrix.TermMap) + "\")";
            }   
        }

        /// <summary>
        /// Matrix in R format
        /// 
        /// From R Documentation of lda.collapsed.gibbs.sampler v1.3.1
        /// documents: A list whose length is equal to the number of documents, D.
        /// Each element of documents is an integer matrix with two rows.
        /// Each column of documents[[i]] (i.e., document i) represents a word occurring in the document.
        /// documents[[i]][1, j] is a 0-indexed word identifier for the jth word in document i. That is, this should be an index - 1 into vocab.
        /// documents[[i]][2, j] is an integer specifying the number of times that word appears in the document.
        /// 
        /// However, this is also written in the documentation of lda.collapsed.gibbs.sampler  v1.3.1 
        /// "WARNING: This function does not compute precisely the correct thing when the count associated 
        /// with a word in a document is not 1 (this is for speed reasons currently). A workaround when a word
        /// appears multiple times is to replicate the word across several columns of a document. This will
        /// likely be fixed in a future version."
        /// 
        /// We implement the workaround in this instance.
        /// </summary>
        public string Matrix
        {
            get
            {
                StringBuilder tdoc = new StringBuilder("list(");
                tdoc.AppendLine();
                for (int i = 0; i < _matrix.NumDocs; i++)
                {
                    tdoc.Append("structure(c(");
                    List<string> entries = new List<string>();
                    for (int j = 0; j < _matrix.NumTerms; j++)
                    {
                        int freq = Convert.ToInt32(_matrix[i, j]);
                        for (int k = 1; k <= freq; k++)
                        {
                            entries.Add(j + "L,1L");
                        }
                    }
                    tdoc.Append(String.Join(",", entries));
                    tdoc.Append("), .Dim = c(2L, " + Convert.ToInt32(_matrix.GetDocument(i).Sum()) + "L))");
                    if (i < _matrix.NumDocs - 1)
                    {
                        tdoc.AppendLine(",");
                    }
                }
                tdoc.AppendLine();
                tdoc.AppendLine(")");
                return tdoc.ToString();
            }
        }

        /// <summary>
        /// Map of artifacts ids
        /// </summary>
        public List<string> Map
        {
            get
            {
                return _matrix.DocMap;
            }
        }

        #endregion

        #region I/O

        /// <summary>
        /// Saves corpus to cache.
        /// Overwrites existing files with the same name.
        /// </summary>
        /// <returns>Corpus base path + name</returns>
        public LDACorpusInfo Save()
        {
            LDACorpusInfo info = new LDACorpusInfo();
            info.Name = Name;
            // write matrix
            FileStream cFS = RUtil.CreateCacheFile(Name + ".corpus");
            info.Corpus = cFS.Name;
            TextWriter corpus = new StreamWriter(cFS);
            corpus.Write(Matrix);
            corpus.Flush();
            corpus.Close();
            // write vocab
            FileStream vFS = RUtil.CreateCacheFile(Name + ".vocab");
            info.Vocab = vFS.Name;
            TextWriter vocab = new StreamWriter(vFS);
            vocab.Write(Vocab);
            vocab.Flush();
            vocab.Close();
            // write edges
            FileStream eFS = RUtil.CreateCacheFile(Name + ".tableWriter");
            info.Edges = eFS.Name;
            TextWriter edges = new StreamWriter(eFS);
            edges.Write(Edges);
            edges.Flush();
            edges.Close();
            // write links
            FileStream lFS = RUtil.CreateCacheFile(Name + ".links");
            info.Links = lFS.Name;
            TextWriter links = new StreamWriter(lFS);
            links.Write(Links);
            links.Flush();
            links.Close();
            // return info
            return info;
        }

        #endregion
    }

    /// <summary>
    /// Corpus cache information
    /// </summary>
    public class LDACorpusInfo
    {
        /// <summary>
        /// Corpus name
        /// </summary>
        public string Name;

        /// <summary>
        /// Corpus cache file
        /// </summary>
        public string Corpus;

        /// <summary>
        /// Vocab cache file
        /// </summary>
        public string Vocab;

        /// <summary>
        /// Edges cache file
        /// </summary>
        public string Edges;

        /// <summary>
        /// Links cache file
        /// </summary>
        public string Links;
    }
}
