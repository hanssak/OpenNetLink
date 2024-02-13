using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using OfficeExtractor.Exceptions;
using OfficeExtractor.Helpers;
using OpenMcdf;
using PasswordProtectedChecker;

//
// Extractor.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2013-2022 Magic-Sessions. (www.magic-sessions.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace OfficeExtractor
{
    /// <summary>
    /// This class is used to extract embedded files from Word, Excel and PowerPoint files. It only extracts
    /// one level deep, for example when you have an Word file with an embedded Excel file that has an embedded
    /// PDF it will only extract the embedded Excel file from the Word file.
    /// </summary>
    public class Extractor
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<Extractor>();
        #region Fields
        /// <summary>
        ///     <see cref="Checker"/>
        /// </summary>
        private readonly Checker _passwordProtectedChecker = new Checker();

        /// <summary>
        ///     <see cref="Word"/>
        /// </summary>
        private Word _word;

        /// <summary>
        ///     <see cref="Excel"/>
        /// </summary>
        private Excel _excel;

        /// <summary>
        ///     <see cref="PowerPoint"/>
        /// </summary>
        private PowerPoint _powerPoint;

        /// <summary>
        ///     <see cref="Rtf"/>
        /// </summary>
        private Rtf _rtf;

        /// <summary>
        ///     <see cref="Extraction"/>
        /// </summary>
        private Extraction _extraction;
        #endregion

        #region Properties
        /// <summary>
        ///     An unique id that can be used to identify the logging of the converter when
        ///     calling the code from multiple threads and writing all the logging to the same file
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string InstanceId
        {
            get => Logger.InstanceId;
            set => Logger.InstanceId = value;
        }

        /// <summary>
        /// Returns a reference to the Word class when it already exists or creates a new one
        /// when it doesn't
        /// </summary>
        private Word Word
        {
            get
            {
                if (_word != null)
                    return _word;

                _word = new Word();
                return _word;
            }
        }

        /// <summary>
        /// Returns a reference to the Excel class when it already exists or creates a new one
        /// when it doesn't
        /// </summary>
        private Excel Excel
        {
            get
            {
                if (_excel != null)
                    return _excel;

                _excel = new Excel();
                return _excel;
            }
        }

        /// <summary>
        /// Returns a reference to the PowerPoint class when it already exists or creates a new one
        /// when it doesn't
        /// </summary>
        private PowerPoint PowerPoint
        {
            get
            {
                if (_powerPoint != null)
                    return _powerPoint;

                _powerPoint = new PowerPoint();
                return _powerPoint;
            }
        }

        /// <summary>
        /// Returns a reference to the RTF class when it already exists or creates a new one
        /// when it doesn't
        /// </summary>
        private Rtf Rtf
        {
            get
            {
                if (_rtf != null)
                    return _rtf;

                _rtf = new Rtf();
                return _rtf;
            }
        }

        /// <summary>
        /// Returns a reference to the Extraction class when it already exists or creates a new one
        /// when it doesn't
        /// </summary>
        private Extraction Extraction
        {
            get
            {
                if (_extraction != null)
                    return _extraction;

                _extraction = new Extraction();
                return _extraction;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        ///     Creates this object and sets it's needed properties
        /// </summary>
        /// <param name="logStream">When set then logging is written to this stream for all extractions. If
        /// you want a separate log for each conversion then set the logstream on the <see cref="Extract"/> method</param>
        public Extractor(Stream logStream = null)
        {
            Logger.LogStream = logStream;
        }
        #endregion

        #region CheckFileNameAndOutputFolder
        /// <summary>
        /// Checks if the <paramref name="inputFile"/> and <paramref name="outputFolder"/> is valid
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFolder"></param>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile"/> or <paramref name="outputFolder"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile"/> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder"/> does not exists</exception>
        private static void CheckFileNameAndOutputFolder(string inputFile, string outputFolder)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentNullException(inputFile);

            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(outputFolder);

            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            if (!Directory.Exists(outputFolder))
                throw new DirectoryNotFoundException(outputFolder);
        }
        /// <summary>
        /// Checks if the <paramref name="stream"/> and <paramref name="outputFolder"/> is valid
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="outputFolder"></param>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile"/> or <paramref name="outputFolder"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <paramref name="inputFile"/> does not exists</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder"/> does not exists</exception>
        private static void CheckFileNameAndOutputFolder(Stream stream, string outputFolder)
        {
            if (stream == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(outputFolder);

            if (!Directory.Exists(outputFolder))
                throw new DirectoryNotFoundException(outputFolder);
        }
        #endregion

        #region GetExtension
        /// <summary>
        ///     Get the extension from the file and checks if this extension is valid
        /// </summary>
        /// <param name="inputFile">The file to check</param>
        /// <returns></returns>
        private static string GetExtension(string inputFile)
        {
            var extension = Path.GetExtension(inputFile);
            extension = string.IsNullOrEmpty(extension) ? string.Empty : extension.ToUpperInvariant();

            switch (extension)
            {
                case ".RTF":
                case ".ODT":
                case ".ODS":
                case ".ODP":
                    break;

                default:

                    //using (var fileStream = File.OpenRead(inputFile))
                    //{
                    //    var header = new byte[2];
                    //    fileStream.Read(header, 0, 2);

                    //    // 50 4B = PK --> .doc = 4
                    //    if (header[0] == 0x50 && header[1] == 0x4B && extension.Length == 4)
                    //    {
                    //        extension += "X";
                    //    }
                    //    // D0 CF = DI --> .docx = 5
                    //    else if (header[0] == 0xD0 && header[1] == 0xCF)
                    //    {
                    //        extension = extension.Substring(0, 4);
                    //    }
                    //}
                    break;
            }

            return extension;
        }
        #endregion

        #region ThrowPasswordProtected
        private void ThrowPasswordProtected(string inputFile)
        {
            var message = $"The file '{Path.GetFileName(inputFile)}' is password protected";
            Logger.WriteToLog(message);
            throw new OEFileIsPasswordProtected(message);
        }
        #endregion

        #region Extract
        /// <summary>
        /// Extracts all the embedded object from the Microsoft Office <paramref name="inputFile"/> to the 
        /// <paramref name="outputFolder"/> and returns the files with full path as a list of strings
        /// </summary>
        /// <param name="inputFile">The Microsoft Office file</param>
        /// <param name="outputFolder">The output folder</param>
        /// <param name="logStream">When set then logging is written to this stream</param>
        /// <param name="attachmentsOnly">Sets whether all OLE objects shall be extracted or only attachment-like ones </param>
        /// <returns>List with files or en empty list when there are nog embedded files</returns>
        /// <exception cref="ArgumentNullException">Raised when the <paramref name="inputFile"/> or <paramref name="outputFolder"/> is null or empty</exception>
        /// <exception cref="FileNotFoundException">Raised when the <sparamref name="inputFile"/> does not exist</exception>
        /// <exception cref="DirectoryNotFoundException">Raised when the <paramref name="outputFolder"/> does not exists</exception>
        /// <exception cref="OEFileIsCorrupt">Raised when the <paramref name="inputFile" /> is corrupt</exception>
        /// <exception cref="OEFileTypeNotSupported">Raised when the <paramref name="inputFile"/> is not supported</exception>
        /// <exception cref="OEFileIsPasswordProtected">Raised when the <paramref name="inputFile"/> is password protected</exception>
        public List<string> Extract(string inputFile, string outputFolder, Stream logStream = null, bool attachmentsOnly = false)
        {
            if (logStream != null)
                Logger.LogStream = logStream;

            CheckFileNameAndOutputFolder(inputFile, outputFolder);

            var extension = GetExtension(inputFile);

            Logger.WriteToLog($"Checking if file '{inputFile}' contains any embeded objects");

            outputFolder = FileManager.CheckForDirectorySeparator(outputFolder);

            List<string> result;
            CLog.Information($"ExcuteExtractor.Extract Checking Data - extension[{extension}] inputFile[{inputFile}]");
            try
            {
                switch (extension)
                {
                    case ".ODT":
                    case ".ODS":
                    case ".ODP":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        result = ExtractFromOpenDocumentFormat(inputFile, outputFolder, "OpenOffice");
                        break;

                    case ".DOC":
                    case ".DOT":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        // Word 97 - 2003
                        result = Word.Extract(inputFile, outputFolder, attachmentsOnly);
                        break;

                    case ".DOCM":
                    case ".DOCX":
                    case ".DOTM":
                    case ".DOTX":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        //if (extension == ".DOCX")
                        //{
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");
                        //}

                        // Word 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/word/embeddings/", outputFolder, "Word");
                        break;

                    case ".RTF":
                        result = Rtf.Extract(inputFile, outputFolder);
                        break;

                    case ".XLS":
                    case ".XLT":
                    case ".XLW":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        // Excel 97 - 2003
                        result = Excel.Extract(inputFile, outputFolder);
                        break;

                    case ".XLSB":
                    case ".XLSM":
                    case ".XLSX":
                    case ".XLTM":
                    case ".XLTX":
                    case ".XLAM":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        //if (extension == ".XLSX")
                        //{
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");
                        //}

                        // Excel 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/xl/embeddings/", outputFolder, "Excel");
                        break;
                    case ".HML":
                        if (!CheckXmlParsing(inputFile))
                            throw new XmlParsingException("ERROR XML PARSING");

                        result = ExtractFromXmlFormat(inputFile, outputFolder, "text/xml");
                        break;
                    case ".HWP":
                    case ".HWT":
                        result = ExtractFromHwpFormat(inputFile, outputFolder, "Hwp");
                        break;
                    case ".HWTX":
                    case ".HWPX":
                        result = ExtractFromHwpxFormat(inputFile, outputFolder, "Hwpx");
                        break;
                    case ".POT":
                    case ".PPT":
                    case ".PPS":
                        if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFile);

                        // PowerPoint 97 - 2003
                        result = PowerPoint.Extract(inputFile, outputFolder);
                        break;

                    case ".POTM":
                    case ".POTX":
                    case ".PPSM":
                    case ".PPSX":
                    case ".PPAM":
                    case ".PPTM":
                    case ".PPTX":
                    case ".SLDM":
                    case ".SLDX":
                    case ".THMX":
                        //if (_passwordProtectedChecker.IsFileProtected(inputFile).Protected)
                        //    ThrowPasswordProtected(inputFile);

                        if(IsPowerPointPasswordProtected(inputFile))
                            ThrowPasswordProtected(inputFile);

                        //if (extension == ".PPTX" || extension == ".THMX" || extension == ".SLDX" || )
                        //{
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");
                        //}

                        // PowerPoint 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/ppt/embeddings/", outputFolder, "PowerPoint");
                        break;

                    default:
                        var message = "The file '" + Path.GetFileName(inputFile) +
                                      "' is not supported, only .ODT, .DOC, .DOCM, .DOCX, .DOT, .DOTM, .DOTX, .RTF, .XLS, .XLSB, .XLSM, .XLSX, .XLT, " +
                                      ".XLTM, .XLTX, .XLW, .POT, .PPT, .POTM, .POTX, .PPS, .PPSM, .PPSX, .PPTM, .PPTX, .SLDX, .SLDM, .THMX, .XLAM, .HML, .HWP and .HWPX are supported";

                        Logger.WriteToLog(message);
                        throw new OEFileTypeNotSupported(message);
                }
            }
            catch (CFCorruptedFileException)
            {
                CLog.Error($"Extract Exception - The file '{Path.GetFileName(inputFile)}' is corrupt");
                throw new OEFileIsCorrupt($"The file '{Path.GetFileName(inputFile)}' is corrupt");
            }
            catch (OEFileIsPasswordProtected ex)
            {
                throw ex;
            }
            catch (Exception exception)
            {
                CLog.Error($"Extract Exception - {exception.ToString()}");

                if (extension == ".HWP" || extension == ".HWT")
                    result = ExtractFromHwpxFormat(inputFile, outputFolder, "Hwpx");
                else if (extension == ".HWPX" || extension == ".HWTX")
                    result = ExtractFromHwpFormat(inputFile, outputFolder, "Hwp");
                else
                {
                    Logger.WriteToLog($"Cant check for embedded object because an error occured, error: {exception.Message}");
                    throw;
                }
            }

            var count = result.Count;
            Logger.WriteToLog($"Found {count} embedded object{(count == 1 ? string.Empty : "s")}");
            return result;
        }

        public List<string> Extract(Stream inputFile, string inputFileName, string outputFolder, Stream logStream = null, bool attachmentsOnly = false)
        {
            if (logStream != null)
                Logger.LogStream = logStream;

            CheckFileNameAndOutputFolder(inputFile, outputFolder);

            var extension = GetExtension(inputFileName);

            Logger.WriteToLog($"Checking if file '{inputFileName}' contains any embeded objects");

            outputFolder = FileManager.CheckForDirectorySeparator(outputFolder);

            List<string> result;
            CLog.Information($"ExcuteExtractor.Extract Checking Data - extension[{extension}] inputFile.Length[{inputFile?.Length}]");

            try
            {
                switch (extension)
                {
                    case ".ODT":
                    case ".ODS":
                    case ".ODP":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        result = ExtractFromOpenDocumentFormat(inputFile, inputFileName, outputFolder, "OpenOffice");
                        break;

                    case ".DOC":
                    case ".DOT":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        // Word 97 - 2003
                        result = Word.Extract(inputFile, outputFolder, attachmentsOnly);
                        break;

                    case ".DOCM":
                    case ".DOCX":
                    case ".DOTM":
                    case ".DOTX":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        //if (extension == ".DOCX")
                        //{
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");
                        //}

                        // Word 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/word/embeddings/", outputFolder, "Word");
                        break;

                    case ".RTF":
                        result = Rtf.Extract(inputFile, outputFolder);
                        break;

                    case ".XLS":
                    case ".XLT":
                    case ".XLW":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        // Excel 97 - 2003
                        result = Excel.Extract(inputFile, outputFolder);
                        break;

                    case ".XLSB":
                    case ".XLSM":
                    case ".XLSX":
                    case ".XLTM":
                    case ".XLTX":
                    case ".XLAM":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        //                        if (extension == ".XLSX")
                        //                        {
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");

                        //                        }
                        // Excel 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/xl/embeddings/", outputFolder, "Excel");
                        break;
                    case ".HML":
                        if (!CheckXmlParsing(inputFile))
                            throw new XmlParsingException("ERROR XML PARSING");

                        result = ExtractFromXmlFormat(inputFile, outputFolder, "text/xml");
                        break;
                    case ".HWP":
                    case ".HWT":
                        result = ExtractFromHwpFormat(inputFile, inputFileName, outputFolder, "Hwp");
                        break;
                    case ".HWPX":
                    case ".HWTX":
                        result = ExtractFromHwpxFormat(inputFile, inputFileName, outputFolder, "Hwpx");
                        break;

                    case ".POT":
                    case ".PPT":
                    case ".PPS":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        // PowerPoint 97 - 2003
                        result = PowerPoint.Extract(inputFile, outputFolder);
                        break;

                    case ".POTM":
                    case ".POTX":
                    case ".PPSM":
                    case ".PPSX":
                    case ".PPAM":
                    case ".PPTM":
                    case ".PPTX":
                    case ".SLDM":
                    case ".SLDX":
                    case ".THMX":
                        if (_passwordProtectedChecker.IsStreamProtected(inputFile).Protected)
                            ThrowPasswordProtected(inputFileName);

                        //                        if (extension == ".PPTX")
                        //                        {
                        if (DocumentCorruptCheck(inputFile, outputFolder))
                            throw new DocumentCorrupt("ERROR FILE CHANGE");

                        //                        }
                        // PowerPoint 2007 - 2013
                        result = ExtractFromOfficeOpenXmlFormat(inputFile, "/ppt/embeddings/", outputFolder, "PowerPoint");
                        break;

                    default:
                        var message = "The file '" + Path.GetFileName(inputFileName) +
                                      "' is not supported, only .ODT, .ODS, .ODP, .DOC, .DOCM, .DOCX, .DOT, .DOTM, .DOTX, .RTF, .XLS, .XLSB, .XLSM, .XLSX, .XLT, " +
                                      ".XLTM, .XLTX, .XLW, .POT, .PPT, .POTM, .POTX, .PPS, .PPSM, .PPSX, .PPTM, .PPTX, .SLDX, .SLDM, .THMX, .XLAM, .HML, .HWP and .HWPX are supported";

                        Logger.WriteToLog(message);
                        throw new OEFileTypeNotSupported(message);
                }
            }
            catch (CFCorruptedFileException)
            {
                throw new OEFileIsCorrupt($"The file '{Path.GetFileName(inputFileName)}' is corrupt");
            }
            catch (OEFileIsPasswordProtected ex)
            {
                throw ex;
            }
            catch (Exception exception)
            {
                CLog.Error($"Extract Exception - {exception.ToString()}");

                if (extension == ".HWP" || extension == ".HWT")
                    result = ExtractFromHwpxFormat(inputFile, inputFileName, outputFolder, "Hwpx");
                else if (extension == ".HWPX" || extension == ".HWTX")
                    result = ExtractFromHwpFormat(inputFile, inputFileName, outputFolder, "Hwp");
                else
                {
                    Logger.WriteToLog($"Cant check for embedded object because an error occured, error: {exception.Message}");
                    throw;
                }
            }


            var count = result.Count;
            Logger.WriteToLog($"Found {count} embedded object{(count == 1 ? string.Empty : "s")}");
            return result;
        }
        #endregion

        #region ExtractFromOfficeOpenXmlFormat
        /// <summary>
        /// Extracts all the embedded object from the Office Open XML <paramref name="inputFile"/> to the 
        /// <paramref name="outputFolder"/> and returns the files with full path as a list of strings
        /// </summary>
        /// <param name="inputFile">The Office Open XML format file</param>
        /// <param name="embeddingPartString">The folder in the Office Open XML format (zip) file</param>
        /// <param name="outputFolder">The output folder</param>
        /// <param name="programm"></param>
        /// <returns>List with files or an empty list when there are nog embedded files</returns>
        /// <exception cref="OEFileIsPasswordProtected">Raised when the Microsoft Office file is password protected</exception>
        private List<string> ExtractFromOfficeOpenXmlFormatBasic(MemoryStream inputFile, string embeddingPartString, string outputFolder, string programm)
        {
            var result = new List<string>();

            try
            {
                var package = Package.Open(inputFile);

                // Get the embedded files names. 
                foreach (var packagePart in package.GetParts())
                {
                    if (packagePart.Uri.ToString().StartsWith(embeddingPartString))
                    {
                        using (var packagePartStream = packagePart.GetStream())
                        using (var packagePartMemoryStream = new MemoryStream())
                        {
                            packagePartStream.CopyTo(packagePartMemoryStream);

                            var fileName = outputFolder +
                                           packagePart.Uri.ToString().Remove(0, embeddingPartString.Length);

                            if (fileName.ToUpperInvariant().Contains("OLEOBJECT"))
                            {
                                Logger.WriteToLog("OLEOBJECT found");

                                if (Extraction.IsCompoundFile(packagePartMemoryStream.ToArray()))
                                {
                                    using (var compoundFile = new CompoundFile(packagePartMemoryStream))
                                    {
                                        var resultFileName = Extraction.SaveFromStorageNode(compoundFile.RootStorage, outputFolder);
                                        if (resultFileName != null)
                                            result.Add(resultFileName);
                                    }
                                }
                                else
                                {
                                    fileName = FileManager.FileExistsMakeNew(fileName);
                                    File.WriteAllBytes(fileName, packagePartMemoryStream.ToArray());
                                    result.Add(fileName);
                                }
                            }
                            else
                            {
                                fileName = FileManager.FileExistsMakeNew(fileName);
                                File.WriteAllBytes(fileName, packagePartMemoryStream.ToArray());
                                result.Add(fileName);
                            }
                        }
                    }
                }
                package.Close();

                return result;
            }
            catch (FileFormatException fileFormatException)
            {
                throw new Exception(fileFormatException.Message);
            }
        }
        private List<string> ExtractFromOfficeOpenXmlFormat(string inputFile, string embeddingPartString, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type 'Open XML format'");

            var result = new List<string>();

            using (var inputFileMemoryStream = new MemoryStream(File.ReadAllBytes(inputFile)))
            {
                result = ExtractFromOfficeOpenXmlFormatBasic(inputFileMemoryStream, embeddingPartString, outputFolder, programm);
            }

            return result;
        }
        private List<string> ExtractFromOfficeOpenXmlFormat(Stream inputFile, string embeddingPartString, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type 'Open XML format'");

            var result = new List<string>();

            using (var inputFileMemoryStream = new MemoryStream(((MemoryStream)inputFile).ToArray()))
            {
                result = ExtractFromOfficeOpenXmlFormatBasic(inputFileMemoryStream, embeddingPartString, outputFolder, programm);
            }

            return result;
        }
        #endregion

        #region ExtractFromOpenDocumentFormat
        /// <summary>
        /// Searches for the first archive entry with the given name in the given archive.
        /// </summary>
        /// <param name="archive">The archive where the entry should be searched.</param>
        /// <param name="entryName">The name of the entry, which is the file or directory name.
        /// The search is done case insensitive.</param>
        /// <returns>Returns the reference of the entry if found and null if the entry doesn't exists in the archive.</returns>
        private SharpCompress.Archives.IArchiveEntry FindEntryByName(SharpCompress.Archives.IArchive archive, string entryName)
        {
            try
            {
                return
                    archive.Entries.First(
                        archiveEntry => archiveEntry.Key.Equals(entryName, StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts all the embedded object from the OpenDocument <paramref name="inputFile"/> to the 
        /// <paramref name="outputFolder"/> and returns the files with full path as a list of strings
        /// </summary>
        /// <param name="inputFile">The OpenDocument format file</param>
        /// <param name="outputFolder">The output folder</param>
        /// <param name="programm"></param>
        /// <returns>List with files or en empty list when there are nog embedded files</returns>
        /// <exception cref="OEFileIsPasswordProtected">Raised when the OpenDocument format file is password protected</exception>
        private List<string> ExtractFromOpenDocumentFormatBasic(SharpCompress.Archives.Zip.ZipArchive zipFile, string inputFileName, string outputFolder, string programm)
        {
            var result = new List<string>();
            // Check if the file is password protected
            var manifestEntry = FindEntryByName(zipFile, "META-INF/manifest.xml");
            if (manifestEntry != null)
            {
                using (var manifestEntryStream = manifestEntry.OpenEntryStream())
                using (var manifestEntryMemoryStream = new MemoryStream())
                {
                    manifestEntryStream.CopyTo(manifestEntryMemoryStream);
                    manifestEntryMemoryStream.Position = 0;
                    using (var streamReader = new StreamReader(manifestEntryMemoryStream))
                    {
                        var manifest = streamReader.ReadToEnd();
                        if (manifest.ToUpperInvariant().Contains("ENCRYPTION-DATA"))
                            throw new OEFileIsPasswordProtected($"The file '{Path.GetFileName(inputFileName)}' is password protected");
                    }
                }
            }

            foreach (var zipEntry in zipFile.Entries)
            {
                if (zipEntry.IsDirectory) continue;
                if (zipEntry.IsEncrypted)
                    throw new OEFileIsPasswordProtected($"The file '{Path.GetFileName(inputFileName)}' is password protected");

                var name = zipEntry.Key.ToUpperInvariant();
                if (!name.StartsWith("OBJECT") || name.Contains("/"))
                    continue;

                string fileName = null;

                var objectReplacementFile = FindEntryByName(zipFile, "ObjectReplacements/" + name);
                if (objectReplacementFile != null)
                    fileName = Extraction.GetFileNameFromObjectReplacementFile(objectReplacementFile);

                Logger.WriteToLog($"Extracting embedded object '{fileName}'");

                using (var zipEntryStream = zipEntry.OpenEntryStream())
                using (var zipEntryMemoryStream = new MemoryStream())
                {
                    zipEntryStream.CopyTo(zipEntryMemoryStream);

                    using (var compoundFile = new CompoundFile(zipEntryMemoryStream))
                        result.Add(Extraction.SaveFromStorageNode(compoundFile.RootStorage, outputFolder,
                            fileName));
                }
            }

            return result;
        }
        private List<string> ExtractFromOpenDocumentFormat(string inputFile, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type 'Open document format'");

            var result = new List<string>();
            using (var zipFile = SharpCompress.Archives.Zip.ZipArchive.Open(inputFile))
            {
                result = ExtractFromOpenDocumentFormatBasic(zipFile, inputFile, outputFolder, programm);
            }
            return result;
        }
        private List<string> ExtractFromOpenDocumentFormat(Stream inputFile, string inputFileName, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type 'Open document format'");

            var result = new List<string>();
            using (var zipFile = SharpCompress.Archives.Zip.ZipArchive.Open(inputFile))
            {
                result = ExtractFromOpenDocumentFormatBasic(zipFile, inputFileName, outputFolder, programm);
            }
            return result;
        }
        #endregion

        #region hwpx & hwp
        /// <summary>
        /// Extracts all the embedded object from the Hwpx <paramref name="inputFile"/> to the 
        /// <paramref name="outputFolder"/> and returns the files with full path as a list of strings
        /// </summary>
        /// <param name="inputFile">The OpenDocument format file</param>
        /// <param name="outputFolder">The output folder</param>
        /// <param name="programm"></param>
        /// <returns>List with files or en empty list when there are nog embedded files</returns>
        /// <exception cref="OEFileIsPasswordProtected">Raised when the OpenDocument format file is password protected</exception>
        private List<string> ExtractFromHwpxFormatBasic(SharpCompress.Archives.Zip.ZipArchive zipFile, string inputFileName, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            var result = new List<string>();

            // Check if the file is password protected
            var manifestEntry = FindEntryByName(zipFile, "META-INF/manifest.xml");
            if (manifestEntry != null)
            {
                using (var manifestEntryStream = manifestEntry.OpenEntryStream())
                using (var manifestEntryMemoryStream = new MemoryStream())
                {
                    manifestEntryStream.CopyTo(manifestEntryMemoryStream);
                    manifestEntryMemoryStream.Position = 0;
                    using (var streamReader = new StreamReader(manifestEntryMemoryStream))
                    {
                        var manifest = streamReader.ReadToEnd();
                        if (manifest.ToUpperInvariant().Contains("ENCRYPTION-DATA"))
                            throw new OEFileIsPasswordProtected($"The file '{Path.GetFileName(inputFileName)}' is password protected");
                    }
                }
            }

            var contentEntry = FindEntryByName(zipFile, "Contents/content.hpf");
            if (contentEntry == null)
            {
                throw new DocumentCorrupt("ERROR FILE CHANGE");
            }
            else
            {
                List<string> defaultContents = new List<string>() {
                    "mimetype", "settings.xml", "version.xml",
                    "Preview/PrvImage.png", "Preview/PrvText.txt",
                    "META-INF/container.rdf", "META-INF/container.xml","META-INF/manifest.xml",
                    "Contents/content.hpf"
                };

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(contentEntry.OpenEntryStream());

                XmlNode root = xmlDoc.FirstChild.NextSibling;
                string ns = root.GetNamespaceOfPrefix(root.Prefix);
                var nsManager = new XmlNamespaceManager(new NameTable());
                nsManager.AddNamespace(root.Prefix, ns);
                XmlNodeList nodeList = xmlDoc.SelectNodes($"/{root.Prefix}:package/{root.Prefix}:manifest/{root.Prefix}:item", nsManager);

                bool checkResult = false;
                foreach (var zipEntry in zipFile.Entries)
                {
                    if (zipEntry.IsDirectory) continue;

                    if (defaultContents.Contains(zipEntry.Key))
                        continue;

                    //if (zipEntry.LastModifiedTime.Value.TimeOfDay != TimeSpan.Zero)
                    //{
                    //    checkResult = true;
                    //    break;
                    //}

                    bool isExist = false;
                    foreach (XmlNode node in nodeList)
                    {
                        string href = node.Attributes.GetNamedItem("href").InnerText;
                        if (href == zipEntry.Key)
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist)
                    {
                        string fileName = outputFolder + Path.GetFileName(zipEntry.Key);
                        fileName = FileManager.FileExistsMakeNew(fileName);
                        byte[] bytes = new byte[zipEntry.Size];
                        zipEntry.OpenEntryStream().Read(bytes, 0, bytes.Length);
                        File.WriteAllBytes(fileName, bytes);
                        checkResult = true;
                    }
                }

                if (checkResult)
                {
                    throw new DocumentCorrupt("ERROR FILE CHANGE");
                }
            }


            foreach (var zipEntry in zipFile.Entries)
            {
                if (zipEntry.IsDirectory) continue;
                if (zipEntry.IsEncrypted)
                    throw new OEFileIsPasswordProtected($"The file '{Path.GetFileName(inputFileName)}' is password protected");

                var name = zipEntry.Key.ToUpperInvariant();
                if (!name.StartsWith("BINDATA"))
                    continue;
                if (!name.Contains("OLE"))
                    continue;

                using (var zipEntryStream = zipEntry.OpenEntryStream())
                using (var zipEntryMemoryStream = new MemoryStream())
                {
                    zipEntryStream.CopyTo(zipEntryMemoryStream);

                    using (var realEntryStream = new MemoryStream())
                    {
                        zipEntryMemoryStream.Position = 4;
                        zipEntryMemoryStream.CopyTo(realEntryStream);

                        using (CompoundFile compoundFile = new CompoundFile(realEntryStream))
                        {
                            var extractedFileName = Extraction.SaveFromStorageNode(compoundFile.RootStorage, outputFolder);
                            if (extractedFileName != null) result.Add(extractedFileName);
                        }
                    }
                }
            }

            return result;
        }

        private List<string> ExtractFromHwpxFormat(string inputFile, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            var result = new List<string>();
            using (var zipFile = SharpCompress.Archives.Zip.ZipArchive.Open(inputFile))
            {
                result = ExtractFromHwpxFormatBasic(zipFile, inputFile, outputFolder, programm);
            }

            return result;
        }
        private List<string> ExtractFromHwpxFormat(Stream inputFile, string inputFileName, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            var result = new List<string>();
            using (var zipFile = SharpCompress.Archives.Zip.ZipArchive.Open(inputFile))
            {
                result = ExtractFromHwpxFormatBasic(zipFile, inputFileName, outputFolder, programm);
            }
            return result;
        }
        private List<string> ExtractFromHwpFormatBasic(CompoundFile compoundFile, string inputFileName, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            var result = new List<string>();

            void EntriesStream(CFItem stream)
            {
                var childStream = stream as CFStream;
                if (childStream.Name.Contains("OLE"))
                {
                    byte[] decompressByte = MicrosoftDecompress(childStream.GetData());
                    using (MemoryStream zipMemoryStream = new MemoryStream(decompressByte))
                    {
                        using (MemoryStream realMemoryStream = new MemoryStream())
                        {
                            zipMemoryStream.Position = 4;
                            zipMemoryStream.CopyTo(realMemoryStream);

                            using (CompoundFile oleCompoundFile = new CompoundFile(realMemoryStream))
                            {
                                var extractedFileName = Extraction.SaveFromStorageNode(oleCompoundFile.RootStorage, outputFolder);
                                if (extractedFileName != null) result.Add(extractedFileName);
                            }
                        }
                    }
                }
            }

            void Entries(CFItem storage)
            {
                var childStorage = storage as CFStorage;

                if (childStorage != null && childStorage.Name.Equals("BinData"))
                {
                    childStorage.VisitEntries(EntriesStream, false);
                }
            }

            void CheckPassWord(CFItem storage)
            {
                var stream = storage as CFStream;
                if (stream != null && stream.Name.Contains("FileHeader"))
                {
                    using (var memoryStream = new MemoryStream(stream.GetData()))
                    {
                        int i = 0;
                        foreach (byte v in memoryStream.ToArray())
                        {
                            if (i < 36)
                            {
                                i++;
                                continue;
                            }
                            else
                            {
                                if ((v & 0x02) == 0x02)
                                {
                                    throw new OEFileIsPasswordProtected($"The file '{Path.GetFileName(inputFileName)}' is password protected");
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
            }
            compoundFile.RootStorage.VisitEntries(CheckPassWord, false);
            compoundFile.RootStorage.VisitEntries(Entries, false);
            return result;
        }
        private bool CheckXmlParsing(string inputFile)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(File.ReadAllText(inputFile));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool CheckXmlParsing(Stream inputFile)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(inputFile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private List<string> ExtractFromXmlFormat(string inputFile, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(inputFile));

            return ExtractFromXmlFormatBasic(xmlDoc, outputFolder, programm);
        }

        private List<string> ExtractFromXmlFormat(Stream inputFile, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(inputFile);

            return ExtractFromXmlFormatBasic(xmlDoc, outputFolder, programm);
        }

        private List<string> ExtractFromXmlFormatBasic(XmlDocument xmlDoc, string outputFolder, string programm)
        {
            var result = new List<string>();

            Dictionary<string, string> binDataFormat = new Dictionary<string, string>();
            XmlNodeList binDataeList = xmlDoc.SelectNodes("//BINITEM");
            foreach (XmlNode xml in binDataeList)
            {
                var format = xml.Attributes["Format"];
                var binData = xml.Attributes["BinData"];
                string formatValue = String.Empty;
                string binDataValue = String.Empty;
                if (format != null)
                {
                    formatValue = format.Value;
                }

                if (binData != null)
                {
                    binDataValue = binData.Value;
                }

                if (binDataValue != String.Empty && formatValue != String.Empty)
                {
                    binDataFormat.Add(binDataValue, formatValue);
                }
            }


            XmlNodeList binList = xmlDoc.SelectNodes("//BINDATA");
            foreach (XmlNode xml in binList)
            {
                var id = xml.Attributes["Id"];
                string idValue = String.Empty;
                if (id != null)
                {
                    idValue = id.Value;
                }
                else
                    continue;

                string format = String.Empty;
                if (!binDataFormat.TryGetValue(idValue, out format))
                    continue;

                switch (format)
                {
                    case "OLE":
                        {
                            byte[] byte64 = Convert.FromBase64String(xml.LastChild.Value);

                            using (MemoryStream memoryStream = new MemoryStream(byte64))
                            {
                                using (CompoundFile compoundFile = new CompoundFile(memoryStream))
                                {
                                    var extractedFileName = Extraction.SaveFromStorageNode(compoundFile.RootStorage, outputFolder);
                                    if (extractedFileName != null) result.Add(extractedFileName);

                                }
                            }
                        }
                        break;
                    default:
                        {
                            byte[] byte64 = Convert.FromBase64String(xml.LastChild.Value);
                            string fileName = $"EmbeddedOject.{format}";
                            string extractedFileName = Extraction.SaveByteArrayToFile(byte64, FileManager.FileExistsMakeNew(Path.Combine(outputFolder, fileName)));
                            if (extractedFileName != null) result.Add(extractedFileName);
                        }
                        break;

                }
            }

            return result;
        }
        private List<string> ExtractFromHwpFormat(string inputFile, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            if (CheckXmlParsing(inputFile))
            {
                return ExtractFromXmlFormat(inputFile, outputFolder, "text/xml");
            }

            var result = new List<string>();
            using (CompoundFile compoundFile = new CompoundFile(inputFile))
            {
                result = ExtractFromHwpFormatBasic(compoundFile, inputFile, outputFolder, programm);
            }
            return result;
        }

        private List<string> ExtractFromHwpFormat(Stream inputFile, string inputFileName, string outputFolder, string programm)
        {
            Logger.WriteToLog($"The {programm} file is of the type '{programm} format'");

            if (CheckXmlParsing(inputFile))
            {
                return ExtractFromXmlFormat(inputFile, outputFolder, "text/xml");
            }

            var result = new List<string>();
            using (CompoundFile compoundFile = new CompoundFile(inputFile))
            {
                result = ExtractFromHwpFormatBasic(compoundFile, inputFileName, outputFolder, programm);
            }
            return result;
        }
        /// <summary>
        /// docx, xlsx, pptx 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        private bool DocumentCorruptCheckBasic(MemoryStream inputFile, string outputFolder)
        {
            bool result = false;
            int zipCount = 0;
            try
            {
                using (var zipFile = SharpCompress.Archives.Zip.ZipArchive.Open(inputFile))
                {

                    long contentTypeTicks = 0;
                    foreach (var zipEntry in zipFile.Entries)
                    {
                        if (zipEntry.Key.Equals("[Content_Types].xml"))
                        {
                            contentTypeTicks = zipEntry.LastModifiedTime.Value.Ticks;
                            break;
                        }
                    }

                    foreach (var zipEntry in zipFile.Entries)
                    {
                        if (zipEntry.IsDirectory) continue;

                        if (zipEntry.Key.Equals("[trash]/0000.dat"))
                            continue;

                        if (zipEntry.Key.ToLower().EndsWith(".xml"))
                        {
                            if (zipEntry.LastModifiedTime.Value.Ticks != contentTypeTicks)
                            {
                                string fileName = outputFolder + Path.GetFileName(zipEntry.Key);
                                fileName = FileManager.FileExistsMakeNew(fileName);
                                byte[] bytes = new byte[zipEntry.Size];
                                zipEntry.OpenEntryStream().Read(bytes, 0, bytes.Length);
                                File.WriteAllBytes(fileName, bytes);
                                return true;
                            }
                        }

                        zipCount++;
                    }

                    using (var package = Package.Open(inputFile))
                    {
                        int packageCount = package.GetParts().Count();

                        if (packageCount == 0)
                        {
                            package.Close();
                            return result = true;
                        }

                        if (zipCount - 1 != packageCount)
                        {
                            foreach (var zipEntry in zipFile.Entries)
                            {
                                if (zipEntry.IsDirectory) continue;

                                if (zipEntry.Key.Equals("[Content_Types].xml"))
                                    continue;

                                bool isExist = false;
                                foreach (var packagePart in package.GetParts())
                                {
                                    if (packagePart.Uri.ToString() == "/" + zipEntry.Key)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }

                                if (!isExist)
                                {
                                    string fileName = outputFolder + Path.GetFileName(zipEntry.Key);
                                    fileName = FileManager.FileExistsMakeNew(fileName);
                                    byte[] bytes = new byte[zipEntry.Size];
                                    zipEntry.OpenEntryStream().Read(bytes, 0, bytes.Length);
                                    File.WriteAllBytes(fileName, bytes);
                                }
                            }
                            package.Close();
                            return result = true;
                        }
                    }
                }
            }
            catch (FileFormatException exception)
            {
                throw new Exception(exception.Message);
            }

            return result;
        }

        private bool DocumentCorruptCheck(string inputFile, string outputFolder)
        {
            bool result = false;

            using (var inputFileMemoryStream = new MemoryStream(File.ReadAllBytes(inputFile)))
            {
                result = DocumentCorruptCheckBasic(inputFileMemoryStream, outputFolder);
            }

            return result;
        }
        private bool DocumentCorruptCheck(Stream inputFile, string outputFolder)
        {
            bool result = false;

            using (var inputFileMemoryStream = new MemoryStream(((MemoryStream)inputFile).ToArray()))
            {
                result = DocumentCorruptCheckBasic(inputFileMemoryStream, outputFolder);
            }

            return result;
        }

        private bool IsPowerPointPasswordProtected(string filePath)
        {
            try
            {
                using (var compoundFile = new CompoundFile(filePath))
                {
                    if (compoundFile.RootStorage.TryGetStream("EncryptedPackage", out _)) return true;

                    return false;
                }
            }
            catch (CFFileFormatException)
            {
                // It seems the file is just a normal Microsoft Office 2007 and up Open XML file
                return false;
            }
        }

        //Use system IO. Decompress with compress
        public static byte[] MicrosoftDecompress(byte[] data)
        {
            MemoryStream compressed = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Decompress); //  Note: the first parameter here is also to fill in compressed data, but this time it is used as input data
            deflateStream.CopyTo(decompressed);
            byte[] result = decompressed.ToArray();
            return result;
        }
        #endregion
    }
}