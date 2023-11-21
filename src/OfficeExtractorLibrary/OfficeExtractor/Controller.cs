using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OfficeExtractor.Exceptions;
using System.IO;

namespace OfficeExtractor
{
    public class Controller
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<Controller>();
        //[DllExport]
        public static int ExcuteExtractor([MarshalAs(UnmanagedType.LPWStr)] string strSource, [MarshalAs(UnmanagedType.LPWStr)] string strDest)
        {
            int result = 0;
            try
            {
                OfficeExtractor.Extractor extractor = new OfficeExtractor.Extractor();
                var files = extractor.Extract(strSource, strDest);
                result = files.Count;
            }
            catch (XmlParsingException ex)
            {
                result = -11;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (DocumentCorrupt ex)
            {
                result = -10;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (PathTooLongException ex)
            {
                result = -9;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (OEObjectTypeNotSupported ex)
            {
                result = -8;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (OEFileIsCorrupt ex)
            {
                result = -7;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch(ArgumentNullException ex)
            {
                result = -6;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch(FileNotFoundException ex)
            {
                result = -5;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch(DirectoryNotFoundException ex)
            {
                result = -4;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch(OEFileTypeNotSupported ex)
            {
                result = -3;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (OEFileIsPasswordProtected ex)
            {

                result = -2;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            catch (Exception ex)
            {
                result = -1;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            finally
            {
                GC.Collect();
            }
            return result;
        }
        public static int ExcuteExtractor(Stream inputFile, string inputFileName, string strDest)
        {
            int result = 0;
            try
            {
                OfficeExtractor.Extractor extractor = new OfficeExtractor.Extractor();
                var files = extractor.Extract(inputFile, inputFileName, strDest);
                result = files.Count;
            }
            catch (XmlParsingException ex)
            {
                result = -11;
            }
            catch (DocumentCorrupt ex)
            {
                result = -10;
            }
            catch (PathTooLongException ex)
            {
                result = -9;
            }
            catch (OEObjectTypeNotSupported ex)
            {
                result = -8;
            }
            catch (OEFileIsCorrupt ex)
            {
                result = -7;
            }
            catch (ArgumentNullException ex)
            {
                result = -6;
            }
            catch (FileNotFoundException ex)
            {
                result = -5;
            }
            catch (DirectoryNotFoundException ex)
            {
                result = -4;
            }
            catch (OEFileTypeNotSupported ex)
            {
                result = -3;
            }
            catch (OEFileIsPasswordProtected ex)
            {

                result = -2;
            }
            catch (Exception ex)
            {
                result = -1;
            }
            finally
            {
                GC.Collect();
            }
            return result;
        }

        public static int ExcuteBinaryCheck([MarshalAs(UnmanagedType.LPWStr)] string strSource)
        {
            int result = 0;
            try
            {
                OfficeExtractor.AppendCheck appendChecker = new OfficeExtractor.AppendCheck();
                result = appendChecker.FileAppendCheck(strSource);
            }
            catch (Exception ex)
            {
                result = -2;
                CLog.Error($"ExcuteExtractor Exception (code:{result})- {ex.ToString()}");
            }
            finally
            {
                GC.Collect();
            }
            return result;
        }

    }
}
