using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using AgLogManager;
using Serilog;
using HsNetWorkSG;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using IWshRuntimeLibrary;
using System.IO.Compression;
using SharpCompress.Common;
using SharpCompress.Readers;
using OpenNetLinkApp.Data.SGDicData.SGAlz;

namespace OpenNetLinkApp.Common
{

    public sealed class AsyncLock
    {

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new Handler(_semaphore);
        }

        private sealed class Handler : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed = false;

            public Handler(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore.Release();
                    _disposed = true;
                }
            }

        }

    }


    public class CsFunction
    {


        private static XmlConfService _xmlConfInstance = new XmlConfService();

        public static XmlConfService XmlConf
        {
            get
            {
                if(_xmlConfInstance != null)
                    return _xmlConfInstance;
                else
                {
                    _xmlConfInstance = new XmlConfService();
                    return _xmlConfInstance;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static string GetSizeStr(long Size)
        {
            string rtn = "";
            if (Size == 0)
            {
                rtn = "0 Byte";
            }
            if (Size > 1024 * 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024 * 1024);
                rtn = nSize.ToString("####0.0") + "GB";
            }
            else if (Size > 1024 * 1024)
            {
                float nSize = (float)Size / (1024 * 1024);
                rtn = nSize.ToString("####0.0") + "MB";
            }
            else if (Size > 1024)
            {
                float nSize = (float)Size / (1024);
                rtn = nSize.ToString("####0.0") + "KB";
            }
            else if (Size > 0)
                rtn = Size + " Byte";
            return rtn;
        }

        public static string GetChangeNewLineToN(string sql)
        {
            return sql.Replace(Environment.NewLine, "\n");
        }

        public static string GetFileRename(bool bMode, string strFileName)
        {
            string strRet = "";
            strRet = SgExtFunc.hsFileRename(bMode, strFileName);
            return strRet;
        }

        public static void SetFilterString(ref string str, string value, Func<Task> function)
        {
            str = value;

            Task.Run(async () => { await function(); });
        }

        public static List<string> GetOptionValue(string option, Dictionary<string, SGNetOverData> dic = null)
        {
            List<string> values = new List<string>();
            if (option == "TransKind")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_IMPORT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_EXPORT"));
            }
            else if (option == "TransStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSWAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSCANCLE"));
                values.Add(_xmlConfInstance.GetTitle("T_TRANS_COMPLETE"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSFAIL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_TRANSCHECKING"));
            }
            else if (option == "MailTransStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_MAIL_TRANSWAIT"));          //발송대기
                values.Add(_xmlConfInstance.GetTitle("T_MAIL_TRANSCANCLE"));        //발송취소
                values.Add(_xmlConfInstance.GetTitle("T_MAIL_TRANS_SUCCESS"));      //발송완료
                values.Add(_xmlConfInstance.GetTitle("T_MAIL_TRANSFRFAILED"));      //발송실패
                values.Add(_xmlConfInstance.GetTitle("T_MAIL_INSPECT"));            //검사중
            }
            else if (option == "ApproveStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE_WAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REJECTION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REQUESTCANCEL"));
            }
            else if (option == "MailApproveStatus")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE_WAIT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_APPROVE"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REJECTION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_REQUESTCANCEL"));
            }
            else if (option == "ApproveKind")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_DETAIL_BEFORE_APPROVE"));
                values.Add(_xmlConfInstance.GetTitle("T_DETAIL_AFTER_APPROVE"));
            }
            else if (option == "TransType")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_IMPORT"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_EXPORT"));
            }
            else if (option == "DlpValue")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_DLP_NOTINCLUSION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_DLP_INCLUSION"));
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_DLP_UNKNOWN"));
            }
            else if (option == "DataType")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                values.Add(_xmlConfInstance.GetTitle("T_DATA_TYPE_TEXT"));
                values.Add(_xmlConfInstance.GetTitle("T_DATA_TYPE_IMAGE"));
            }
            else if(option == "DestNetwork")
            {
                values.Add(_xmlConfInstance.GetTitle("T_COMMON_ALL"));
                if (dic != null)
                {
                    foreach (string str in dic.Keys)
                    {
                        values.Add(str);
                    }
                }
            }

            return values;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        /// <summary>
        /// 입력된 확장자가 string 상에 있는지 확인하는 함수(대소문자구분안함) <br></br>
        /// bFileNameIsOnlyExt - true : 확장자만 입력, false : 파일이름과 함께 입력 <br></br>
        /// (strFileName에서 확장자만 뽑아낸후에 검색함) <br></br>
        /// strFileName : 확장자 or 파일이름.확장자 <br></br>
        /// strExtData : 확장자목록(ex. txt;exe;pptx;zip  )
        /// </summary>
        /// <param name="bFileNameIsOnlyExt"></param>
        /// <param name="strFileName"></param>
        /// <param name="strExtData"></param>
        /// <returns></returns>
        public static bool isFileExtinListStr(bool bFileNameIsOnlyExt, string strFileName, string strExtData)
        {
            if (strExtData.Length < 1 || strExtData == ";")
            {
                Log.Logger.Here().Information($"isFileExtinListStr - false (Wrong ExtList) - strFileName : {strFileName}, Ext List : {strExtData}");
                return false;
            }
                
            if (strFileName.Length < 1)
            {
                Log.Logger.Here().Information($"isFileExtinListStr - false (Wrong FileExt) - strFileName : {strFileName}, Ext List : {strExtData}");
                return false;
            }             
            
            if (bFileNameIsOnlyExt == false)
            {
                // strFileName 예서 확장자만 구해서 입력하기
                int nPos = -1;
                nPos = strFileName.LastIndexOf(".");
                if (nPos < 0)
                    return false;

                strFileName = strFileName.Substring(nPos + 1);

            }

            strFileName = strFileName.ToUpper();
            strExtData = strExtData.ToUpper();

            if (strExtData.IndexOf(strFileName) >= 0)
                return true;

            return false;
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }
        /// <summary>
        /// Base64String To UTF8 String
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        public static string ConvertBase64StringToUTF8(string strVal)
        {
            string tempVal = strVal;
            byte[] temp = Convert.FromBase64String(tempVal);
            tempVal = Encoding.UTF8.GetString(temp);
            return tempVal;
        }
        /// <summary>
        /// gz파일 압축해제
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destPath"></param>
        public static void GzFileDecompress(string filePath, string destPath)
        {
            using (FileStream originalFileStream = System.IO.File.OpenRead(filePath))
            {
                string currentFileName = filePath;
                string newFileName = Path.Combine(destPath, Path.GetFileName(currentFileName.Remove(currentFileName.Length - Path.GetExtension(filePath).Length)));

                using (FileStream decompressedFileStream = System.IO.File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }
        /// <summary>
        /// tar, tgz, tar.gz 파일 압축해제
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destPath"></param>
        public static void TarFileDecompress(string filePath, string destPath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(destPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }
        }

        public static int AlzFileDecompress(string filePath, string destPath)
        {
            //alz file 압축풀기
            SGUnAlz lib = new SGUnAlz();
            int ret = 0;

#if _WINDOWS
            ret = lib.UnAlzExtractWDll(filePath, destPath);
#else
            ret = lib.UnAlzExtractDll(filePath, destPath);
#endif
            return ret;
        }
    }

    public class CsSeqFunc
    {
        public static bool isDeptSeq(string strSeq)
        {
            if ((strSeq?.Length ?? 0) != 18)
                return false;

            if (strSeq.Substring(8, 2) != "16")
                return false;



            return true;
        }

    }

    public class CsFileFunc
    {

        public static bool GetFileStrData(string filePath, out string strData)
        {
            strData = "";
            bool bRet = false;

            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                try
                {
                    long lSize = fileStream.Length;
                    if (lSize < 1)
                    {
                        bRet = false;
                        Log.Logger.Here().Information($"GetFileStrData - Size(Error) : {lSize}, path : {filePath}");
                    }
                    else
                    {
                        bRet = true;

                        byte[] pbyte = null;
                        pbyte = new byte[lSize];
                        fileStream.Read(pbyte, 0, (int)lSize);
                        strData = Encoding.UTF8.GetString(pbyte);
                    }
                }
                catch (Exception e)
                {
                    Log.Logger.Here().Information($"GetFileStrData - Exception - msg : {e.Message}, path : {filePath}");
                }
                finally
                {
                    fileStream?.Dispose();
                }
            }

            return bRet;
        }

        public static bool SetFileStrData(string filePath, string strData)
        {

            if (strData.Length < 1)
            {
                Log.Logger.Here().Information($"SetFileStrData, Write Data Empty!");
                return false;
            }

            bool bRet = false;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    byte[] utf8Bytes = Encoding.UTF8.GetBytes(strData);
                    fileStream.Write(utf8Bytes, 0, utf8Bytes.Length);
                    bRet = true;
                }
                catch (Exception e)
                {
                    Log.Logger.Here().Information($"SetFileStrData - Exception - msg : {e.Message}, path : {filePath}");
                    bRet = false;
                }
                finally
                {
                    fileStream?.Dispose();
                }
            }

            return bRet;
        }


        public static long GetFileSize(string filePath)
        {
            long lSize = 0;

            using (FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                try
                {
                    lSize = fileStream.Length;
                    return lSize;
                }
                catch (Exception e)
                {
                    Log.Logger.Here().Information($"GetFileSize - Exception - msg : {e.Message}, path : {filePath}");
                    lSize = -1;
                }
                finally
                {
                    fileStream?.Dispose();
                }
            }

            return lSize;
        }

        /// <summary>
        /// strFilePath : [IN] 파일경로(FullPath)
        /// chSep : [IN] Folder 간의 구분문자( '\\' : 윈도우, '/' : Linux, Mac 등등 )
        /// strFolderPath : [OUT] 파일경로상에 있는 Path
        /// bCreateFolder : [IN] 파일경로상에 있는 Path 를 생성할지 유무
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strFolderPath"></param>
        /// <param name="bCreateFolder"></param>
        /// <returns></returns>
        public static bool GetPathByFilePath(string strFilePath, char chSep, out string strFolderPath, bool bCreateFolder)
        {
            strFolderPath = "";
            int nLastPos = strFilePath.LastIndexOf(chSep);
            if (nLastPos < 0)
            {
                Log.Logger.Here().Information($"GetPathByFilePath, input Path Error : {strFilePath}");
                return false;
            }

            strFolderPath = strFilePath.Substring(0, nLastPos);
            if (bCreateFolder && Directory.Exists(strFolderPath) == false)
                Directory.CreateDirectory(strFolderPath);

            return true;
        }

        /// <summary>
        /// Windows / Linux / Mac OSx 에서 다 지원하는 파일명인지 확인하는 함수<br></br>
        /// return : true - 지원함
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strItem"></param>
        /// <returns></returns>
        public static bool isSupportFileName(string fileName, out string strItem)
        {

            Log.Logger.Here().Information($"isSupportFileName - fileName : {fileName}");

            // 빈문자인지 확인
            if ((fileName?.Length ?? 0) == 0)
            {
                strItem = "$EMPTY";
                return false;
            }

            fileName = fileName.Replace(" ", "");
            if ((fileName?.Length ?? 0) == 0)
            {
                strItem = "$EMPTY";
                return false;
            }


            // File / Folder 이름으로 정해질 수 없는 문자 있는지 확인

            // Windows
            string strNotSupportData = "\\/:*?\"<>";

            // Linux
            // "/"

            // Mac ( Finder)


            // 문자 1개라도 허용불가능 
            char[] chNotSupport = strNotSupportData.ToCharArray();

            for (int i = 0; i < chNotSupport.Length; i++)
            {
                if (fileName.IndexOf(chNotSupport[i]) >= 0)
                {
                    strItem = chNotSupport[i].ToString();
                    Log.Logger.Here().Information($"isSupportFileName - Not Support Char(###-Char)(Windows) : {strItem}");
                    return false;
                }
            }

            // 단어전체 동일할때 허용불가능
            if (fileName == "." ||  // Linux
                fileName == "..")
            {
                strItem = fileName;
                Log.Logger.Here().Information($"isSupportFileName - Not Support FileName(###-Word)(Linux) : {strItem}");
                return false;
            }

            // 특정문자로 시작될때 허용불가능 
            if (fileName.IndexOf('.') == 0)
            {
                strItem = ".";
                Log.Logger.Here().Information($"isSupportFileName - Not Support Start Char(###-StartChar)(MacOSx) : {"."}");
                return false;
            }

            strItem = "";
            return true;
        }


        /// <summary>
        /// 지정한 Path에 있는 지정한 확장자 및 파일들을 삭제하는 함수( ex. *.txt
        /// </summary>
        /// <param name="strTargetPath"></param>
        /// <param name="strDelFilePattern"></param>
        /// <returns></returns>
        public static bool DeleteFilesByPathExt(string strTargetPath, string strDelFilePattern)
        {
            if ( (strDelFilePattern?.Length ?? 0) < 3 )
            {
                Log.Logger.Here().Information($"DeleteFilesByPathExt, FilePattern-Error : {strDelFilePattern}");
                return false;
            }

            if ((strTargetPath?.Length ?? 0) < 3 || (Directory.Exists(strTargetPath) == false))
            {
                Log.Logger.Here().Information($"DeleteFilesByPathExt, Path-Error : {strTargetPath}");
                return false;
            }

            bool bRet = true;
            try
            {
                foreach (string filePath in Directory.GetFiles(strTargetPath, strDelFilePattern))
                {
                    try
                    {
                        System.IO.File.Delete(filePath); // 파일 삭제
                        Log.Logger.Here().Information($"DeleteFilesByPathExt, DeleteFile : {filePath}");
                    }
                    catch (IOException e)
                    {
                        Log.Logger.Here().Information($"DeleteFilesByPathExt, DeleteFile(Pcf)(Exception-Msg:{e.Message}) : {filePath}");
                        bRet = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Information($"DeleteFilesByPathExt, Exception-Msg : {ex.Message}, Ret: {bRet}");
                bRet = false;
            }

            return bRet;
        }

    }

    public class CsHashFunc
    {

        public static string SHA256CheckSum(string filePath)
        {
            try
            {
                using (SHA256 SHA256 = SHA256Managed.Create())
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                        return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
                }
            }
            catch (Exception e)
            {
                Log.Logger.Here().Information($"SHA256CheckSum - Exception - msg : {e.Message}, path : {filePath}");
                //CLog.Here().Information($"FileInfo Get(#####) - Exception - msg : {e.Message}, path : {filePath}");
            }

            return "";
        }

        public static string SHA384BinBase64(string filePath)
        {
            try
            {
                using (SHA384 sha384 = SHA384Managed.Create())
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                    {
                        byte[] pbyte = null;
                        pbyte = new byte[fileStream.Length];
                        fileStream.Read(pbyte, 0, (int)fileStream.Length);

                        return Convert.ToBase64String(SHA384.HashData(pbyte));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Logger.Here().Information($"SHA384BinBase64 - Exception - msg : {e.Message}, path : {filePath}");
                //CLog.Here().Information($"FileInfo Get(#####) - Exception - msg : {e.Message}, path : {filePath}");
            }

            return "";
        }

    }

    public class CsSystemFunc
    {

        public static string GetCurrentModulePath()
        {
            string strAgentPath = "";
            string[] strArgumentArry = System.Environment.GetCommandLineArgs();
            strAgentPath = strArgumentArry[0];

            int nIdex = strArgumentArry[0].LastIndexOf("\\");
            if (nIdex > 0)
            {
                strAgentPath = strArgumentArry[0].Substring(0, nIdex);
            }

            Log.Logger.Here().Information($"GetCurrentModulePath : {strAgentPath}");

            return strAgentPath;
        }



        public static string GetCurrentProcessName(bool bGetExePath = true)
        {
            string strAgentPath = "";
            string[] strArgumentArry = System.Environment.GetCommandLineArgs();
            strAgentPath = strArgumentArry[0];

            int nIdex = strArgumentArry[0].LastIndexOf(".");
            if (bGetExePath && nIdex > 0)
            {
                strAgentPath = strArgumentArry[0].Substring(0, nIdex);
                strAgentPath += ".exe";
            }

            Log.Logger.Here().Information($"GetCurrentProcessName : {strAgentPath}");

            return strAgentPath;
        }

        /// <summary>
        /// 부팅시 자동실행되도록 시작프로그램에 Lnk를 등록/제거하는 기능<br></br>
        /// bStartReg : 시작프로그램으로 등록시킬지 유무(true:생성,false:삭제)<br></br>
        /// bUsePublicPath : false만 사용, 공통계정용폴더에 Lnk 생성하려면 관리자권한 필요(Exception발생함)<br></br>
        /// strOrgPath : EXE의 FullPath 경로<br></br>
        /// strLnkFIleName : 생성할 Lnk 파일의 FullPath 경로<br></br>
        /// </summary>
        /// <param name="bStartReg"></param>
        /// <param name="bUsePublicPath"></param>
        /// <param name="strOrgPath"></param>
        /// <param name="strLnkFIleName"></param>
        /// <returns></returns>
        public static bool makeAgentBootStartOSwindow(bool bStartReg,bool bUsePublicPath, string strOrgPath, string strLnkFIleName)
        {
            bUsePublicPath = false;

            string strStartDir = Environment.GetFolderPath(bUsePublicPath?Environment.SpecialFolder.CommonStartup:Environment.SpecialFolder.Startup);
            string LinkFullPath = strStartDir.ToString() + @$"\{strLnkFIleName}"; // OpenNetLink.lnk
            FileInfo LinkFile = new FileInfo(LinkFullPath);
            if (LinkFile.Exists)
            {                
                if (bStartReg == false)
                    LinkFile.Delete();

                Log.Logger.Here().Information($"makeAgentBootStartOSwindow - Lnk File exist : {LinkFullPath},  {(bStartReg?"Lnk Create Skip!":"Lnk Delete Done!")}");
                return true;
            }
            else
            {
                if (bStartReg == false)
                {
                    Log.Logger.Here().Information($"makeAgentBootStartOSwindow - Lnk File isn't exist(Lnk Delete Skip!) : {LinkFullPath}");
                    return true;
                }
            }

            Log.Logger.Here().Information($"makeAgentBootStartOSwindow - WorkingPath(#####) : {Environment.CurrentDirectory}");

            return CsLnkFunc.makeLnkShortCut(strOrgPath, LinkFullPath, "", Environment.CurrentDirectory);
        }

        public static bool makeAgentBootStartOSX(bool bStartReg, string strOrgPath, string strLnkPath)
        {

            return false;
        }

        public static bool makeAgentBootStartLinux(bool bStartReg, string strOrgPath, string strLnkPath)
        {

            return false;
        }


    }

    public class CsLnkFunc
    {
        public static bool makeLnkShortCut(string strOrgPath, 
            string strLnkPath, string strIconPath="", string strWorkingPath="", 
            string Description="", string strArguments="")
        {

            string strErrMsg = "";
            bool bRet = true;
            try
            {
                // 바로가기 생성
                WshShell wsh = new WshShell();
                // IWshShell3 BEforeLink = (IWshShell3)wsh.CreateShortcut(strLnkPath);
                IWshShortcut Link = (IWshShortcut)wsh.CreateShortcut(strLnkPath); // IWshShortcut

                // 원본 파일의 경로 
                Link.TargetPath = strOrgPath;
                Link.WorkingDirectory = strWorkingPath;
                //Link.IconLocation = strIconPath;  // Exception 발생시킴
                Link.Description = Description;
                Link.Arguments = strArguments;
                Link.Save();
            }
            catch (Exception e)
            {
                strErrMsg = e.Message;
                bRet = false;
            }

            Log.Logger.Here().Information(@$"makeAgentBootStart OSwindow - Make Lnk File {(bRet?"SUCCESS":("FAILED+ERRmsg:"+ strErrMsg))} : {strLnkPath}");

            return bRet;
        }

    }

    public class CsWASfunc
    {

        public static string GetErrorCodeToStr(string strData)
        {
            string strMsg = "";

            if (strData == "-1")
                strMsg = "ID 정보를 수신받지 못했습니다.";
            else if (strData == "-2")
                strMsg = "PW 정보를 수신받지 못했습니다.";
            else if (strData == "-3")
                strMsg = "사용자 인증 실패 되었습니다.";
            else if (strData == "-4")
                strMsg = "사용자가 존재하지 않습니다.";
            else if (strData == "-5")
                strMsg = "입력한 PW가 틀립니다.";
            else if (strData == "-6")
                strMsg = "내부시스템(서버오류 및 DB 접근 오류 등)에서 오류가 발생되었습니다.";
            else
                strMsg = "사용자 인증과정 중 알수없는 오류가 발생하였습니다.";

            return strMsg;
        }

        public static string stringIDpwJsonString(string strID, string strPW)
        {
            string json = "{\n\"id\":\"" + strID + "\",\n\"pw\":\"" + strPW + "\"\n}";
            return json;
        }

    }

}
