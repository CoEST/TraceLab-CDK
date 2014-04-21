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

namespace TraceLab.Components.DevelopmentKit.IO
{
    /// <summary>
    /// Responsible for TLSimilarityMatrix I/O
    /// </summary>
    public static class Similarities
    {
        /// <summary>
        /// Imports a file in the form (each line):
        /// SOURCE TARGET SCORE
        /// </summary>
        /// <param name="filename">Similarities file</param>
        /// <returns>Similarity matrix</returns>
        public static TLSimilarityMatrix Import(String filename)
        {
            StreamReader file = new StreamReader(filename);
            TLSimilarityMatrix answer = new TLSimilarityMatrix();
            String line;
            int num = 0;
            while ((line = file.ReadLine()) != null)
            {
                num++;
                if (String.IsNullOrWhiteSpace(line))
                    continue;
                try
                {
                    String[] artifacts = line.Split('\t');
                    String source = artifacts[0];
                    String target = artifacts[1];
                    double score = Convert.ToDouble(artifacts[2]);
                    answer.AddLink(source, target, score);
                }
                catch (IndexOutOfRangeException e)
                {
                    file.Close();
                    throw new DevelopmentKitException("Invalid data format on line " + num + " of file:" + filename, e);
                }
            }
            file.Close();
            return answer;
        }

        /// <summary>
        /// Exports TLSimilarityMatrix to file in the form (each line):
        /// SOURCE TARGET SCORE
        /// </summary>
        /// <param name="matrix">Similarity matrix</param>
        /// <param name="filename">Output file</param>
        public static void Export(TLSimilarityMatrix matrix, string filename)
        {
            TextWriter file = new StreamWriter(filename);
            TLLinksList links = matrix.AllLinks;
            links.Sort();
            foreach (TLSingleLink link in links)
            {
                file.WriteLine("{0}\t{1}\t{2}", link.SourceArtifactId, link.TargetArtifactId, link.Score);
            }
            file.Flush();
            file.Close();
        }

        #region Export to CSV

        /// <summary>
        /// Exports a TLSimilarityMatrix to CSV format
        /// </summary>
        /// <param name="similarityMatrix">Matrix</param>
        /// <param name="outputPath">Output file path</param>
        public static void ExportCSV(TLSimilarityMatrix similarityMatrix, string outputPath)
        {
            if (similarityMatrix == null)
            {
                throw new DevelopmentKitException("Received similarity matrix is null!");
            }
            if (outputPath == null)
            {
                throw new DevelopmentKitException("Output path cannot be null.");
            }
            if (!System.IO.Path.IsPathRooted(outputPath))
            {
                throw new DevelopmentKitException(String.Format("Absolute output path is required. Given path is '{0}'", outputPath));
            }
            if (outputPath.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                outputPath = outputPath + ".csv";
            }
            using (System.IO.TextWriter writeFile = new StreamWriter(outputPath))
            {
                WriteMatrixCSV(similarityMatrix, writeFile);
                writeFile.Flush();
                writeFile.Close();
            }
        }

        /// <summary>
        /// Private method to write TLSimilarityMatrix to CSV
        /// </summary>
        /// <param name="similarityMatrix">Matrix</param>
        /// <param name="writeFile">Open TextWriter stream</param>
        private static void WriteMatrixCSV(TLSimilarityMatrix similarityMatrix, System.IO.TextWriter writeFile)
        {
            //header
            writeFile.WriteLine("Source Artifact Id,Target Artifact Id,Probability");
            TLLinksList traceLinks = similarityMatrix.AllLinks;
            traceLinks.Sort();
            foreach (TLSingleLink link in traceLinks)
            {
                writeFile.WriteLine("{0},{1},{2}", link.SourceArtifactId, link.TargetArtifactId, link.Score);
            }
        }

        /// <summary>
        /// Exports a TLSimilarityMatrix to CSV with an additional column for correct links
        /// 0 - incorrect link
        /// 1 - correct link
        /// </summary>
        /// <param name="similarityMatrix">Candidate Matrix</param>
        /// <param name="answerMatrix">Answer Matrix</param>
        /// <param name="outputPath">Output file path</param>
        public static void ExportCSVWithCorrectness(TLSimilarityMatrix similarityMatrix, TLSimilarityMatrix answerMatrix, string outputPath)
        {
            if (similarityMatrix == null)
            {
                throw new DevelopmentKitException("Received similarity matrix is null!");
            }
            if (answerMatrix == null)
            {
                throw new DevelopmentKitException("Received answer similarity matrix is null!");
            }
            if (outputPath == null)
            {
                throw new DevelopmentKitException("Output path cannot be null.");
            }
            if (!System.IO.Path.IsPathRooted(outputPath))
            {
                throw new DevelopmentKitException(String.Format("Absolute output path is required. Given path is '{0}'", outputPath));
            }
            if (outputPath.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                outputPath = outputPath + ".csv";
            }
            using (System.IO.TextWriter writeFile = new StreamWriter(outputPath))
            {
                WriteMatrixCSVWithCorrectness(similarityMatrix, answerMatrix, writeFile);
                writeFile.Flush();
                writeFile.Close();
            }
        }

        /// <summary>
        /// Private method to export a TLSimilarityMatrix to CSV with an additional column for correct links
        /// 0 - incorrect link
        /// 1 - correct link
        /// </summary>
        /// <param name="similarityMatrix">Candidate Matrix</param>
        /// <param name="answerMatrix">Answer Matrix</param>
        /// <param name="writeFile">Open TextWriter stream</param>
        private static void WriteMatrixCSVWithCorrectness(TLSimilarityMatrix similarityMatrix, TLSimilarityMatrix answerMatrix, System.IO.TextWriter writeFile)
        {
            //header
            writeFile.WriteLine("Source Artifact Id,Target Artifact Id,Probability,Is correct");
            TLLinksList traceLinks = similarityMatrix.AllLinks;
            traceLinks.Sort();
            foreach (TLSingleLink link in traceLinks)
            {
                writeFile.WriteLine("{0},{1},{2},{3}", link.SourceArtifactId, link.TargetArtifactId, link.Score, (answerMatrix.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId)) ? "1" : "0");
            }
        }

        #endregion

        #region Export to Excel spreadsheet

        /// <summary>
        /// Exports a TLSimilarityMatrix to an MS Excel spreadsheet (with correctness).
        /// Much slower than ExportCSV()
        /// </summary>
        /// <param name="similarityMatrix">Candidate matrix</param>
        /// <param name="answerMatrix">Answer matrix</param>
        /// <param name="outputPath">Output file path</param>
        public static void ExportExcelSpreadsheet(TLSimilarityMatrix similarityMatrix, TLSimilarityMatrix answerMatrix, string outputPath)
        {
            if (similarityMatrix == null)
            {
                throw new DevelopmentKitException("Received similarity matrix is null!");
            }
            if (answerMatrix == null)
            {
                throw new DevelopmentKitException("Received answer similarity matrix is null!");
            }
            if (outputPath == null)
            {
                throw new DevelopmentKitException("Output path cannot be null.");
            }
            if (!System.IO.Path.IsPathRooted(outputPath))
            {
                throw new DevelopmentKitException(String.Format("Absolute output path is required. Given path is '{0}'", outputPath));
            }
            if (outputPath.EndsWith(".xsl", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                outputPath = outputPath + ".xsl";
            }
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            ReadSimilarityMatrixToExcelWorksheet(similarityMatrix, answerMatrix, xlWorkSheet);
            xlWorkBook.SaveAs(outputPath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            ReleaseExcelObject(xlWorkSheet);
            ReleaseExcelObject(xlWorkBook);
            ReleaseExcelObject(xlApp);
        }

        /// <summary>
        /// Private method to export a TLSimilarityMatrix to an MS Excel spreadsheet (with correctness).
        /// </summary>
        /// <param name="similarityMatrix">Candidate matrix</param>
        /// <param name="answerMatrix">Answer matrix</param>
        /// <param name="xlWorkSheet">Excel Worksheet object</param>
        private static void ReadSimilarityMatrixToExcelWorksheet(TLSimilarityMatrix similarityMatrix, TLSimilarityMatrix answerMatrix, Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet)
        {
            //header
            int row = 1;
            xlWorkSheet.Cells[row, 1] = "Source Artifact Id";
            xlWorkSheet.Cells[row, 2] = "Target Artifact Id";
            xlWorkSheet.Cells[row, 3] = "Probability";
            xlWorkSheet.Cells[row, 4] = "Is correct";
            row++;
            foreach (string sourceArtifact in similarityMatrix.SourceArtifactsIds)
            {
                var traceLinks = similarityMatrix.GetLinksAboveThresholdForSourceArtifact(sourceArtifact);
                traceLinks.Sort();
                foreach (TLSingleLink link in traceLinks)
                {
                    xlWorkSheet.Cells[row, 1] = link.SourceArtifactId;
                    xlWorkSheet.Cells[row, 2] = link.TargetArtifactId;
                    xlWorkSheet.Cells[row, 3] = link.Score;
                    xlWorkSheet.Cells[row, 4] = (answerMatrix.IsLinkAboveThreshold(link.SourceArtifactId, link.TargetArtifactId)) ? "1" : "0";
                    row++;
                }
            }
        }

        /// <summary>
        /// Releases an MS Excel object
        /// </summary>
        /// <param name="obj">Object to release</param>
        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new Exception("Exception occured while releasing excel object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }
}
