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
            catch(ArgumentNullException ex)
            {
                result = -6;
            }
            catch(FileNotFoundException ex)
            {
                result = -5;
            }
            catch(DirectoryNotFoundException ex)
            {
                result = -4;
            }
            catch(OEFileTypeNotSupported ex)
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

        public static int ExcuteExtractor(Stream inputFile, string inputFileName, string strDest)
        {
            int result = 0;
            try
            {
                OfficeExtractor.Extractor extractor = new OfficeExtractor.Extractor();
                var files = extractor.Extract(inputFile, inputFileName, strDest);
                result = files.Count;
            }
            catch(DocumentCorrupt ex)
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
    }
}
