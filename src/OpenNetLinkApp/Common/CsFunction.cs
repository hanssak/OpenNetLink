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
using System.Text.RegularExpressions;
using System.Drawing;
using QRCoder;
using System.Security.Cryptography;
using Google.Authenticator;

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
                Log.Information($"isFileExtinListStr - false (Wrong ExtList) - strFileName : {strFileName}, Ext List : {strExtData}");
                return false;
            }
                
            if (strFileName.Length < 1)
            {
                Log.Information($"isFileExtinListStr - false (Wrong FileExt) - strFileName : {strFileName}, Ext List : {strExtData}");
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

            // strFileName = strFileName.ToUpper();
            // strExtData = strExtData.ToUpper();

            string[] liststr = strExtData.Split(';');
            int nCount = liststr.Count();

            for (int nDx = 0; nDx < nCount; nDx++)
            {
                if (string.Compare(strFileName, liststr[nDx], true) == 0)
                {
                    Log.Logger.Here().Information($"isFileExtinListStr, Found : {strFileName}");
                    return true;
                }
            }

            Log.Logger.Here().Information($"isFileExtinListStr, FileExt : {strFileName}, is not in str LIST : {strExtData}");
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
        /// Windows / Linux / Mac OSx 에서 다 지원하는 파일명인지 확인하는 함수<br></br>
        /// return : true - 지원함
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strItem"></param>
        /// <returns></returns>
        public static bool isSupportFileName(string fileName, out string strItem, bool bIsForRecv=true)
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
            string strNotSupportData = "\\/:*?\"<>|";  // 차단문자, Test때에 사용 "\\/:*?\"<>|0"

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

            // 특정문자로 시작될때 허용불가능 (수신때만)
            if (bIsForRecv && fileName.IndexOf('.') == 0)
            {
                strItem = ".";
                Log.Logger.Here().Information($"isSupportFileName - Not Support Start Char(###-StartChar)(MacOSx) : {"."}");
                return false;
            }

            strItem = "";
            return true;
        }



        /// <summary>
        /// 지정한 Path에 있는 지정한 확장자 및 파일들을 삭제하는 함수
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

        public static bool DeleteFile(string strDelFilePath)
        {
            bool bRet = true;
            try
            {
                System.IO.File.Delete(strDelFilePath);
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Information($"DeleteFile, Exception(MSG) : {ex.Message}, TargetFile : {strDelFilePath}");
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
                Log.Information($"SHA256CheckSum - Exception - msg : {e.Message}, path : {filePath}");
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
                Log.Information($"SHA384BinBase64 - Exception - msg : {e.Message}, path : {filePath}");
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

            int nIdex = -1;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                nIdex = strArgumentArry[0].LastIndexOf("\\");
            else
                nIdex = strArgumentArry[0].LastIndexOf("/");

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

            Log.Information($"GetCurrentProcessName : {strAgentPath}");

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

                Log.Information($"makeAgentBootStartOSwindow - Lnk File exist : {LinkFullPath},  {(bStartReg?"Lnk Create Skip!":"Lnk Delete Done!")}");
                return true;
            }
            else
            {
                if (bStartReg == false)
                {
                    Log.Information($"makeAgentBootStartOSwindow - Lnk File isn't exist(Lnk Delete Skip!) : {LinkFullPath}");
                    return true;
                }
            }

            Log.Information($"makeAgentBootStartOSwindow - WorkingPath(#####) : {Environment.CurrentDirectory}");

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

            Log.Information(@$"makeAgentBootStart OSwindow - Make Lnk File {(bRet?"SUCCESS":("FAILED+ERRmsg:"+ strErrMsg))} : {strLnkPath}");

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


    public class CsGoogleQRcode
    {
        Bitmap qrCodeImage = null;
        //Bitmap convertedImage = null;

        public string strSecureKey = "";

        ~CsGoogleQRcode()
        {
            Dispose();
        }

        public bool GenerateQRsecureKey()
        {
            // 비밀키 길이 (20바이트)
            int keyLength = 20;
            bool bRet = true;

            using (var rng = new RNGCryptoServiceProvider())
            {
                try
                {
                    byte[] secretKeyBytes = new byte[keyLength];
                    rng.GetBytes(secretKeyBytes);

                    // Base32 인코딩을 사용하여 바이트 배열을 문자열로 변환
                    strSecureKey = Base32Encoding.ToString(secretKeyBytes);
                }
                catch (Exception ex)
                {
                    bRet = false;
                    Log.Logger.Here().Error($"GenerateQRsecureKey, Exception(MSG) : {ex.Message}");
                }
            }

            return bRet;
        }

        Bitmap ConvertTo24Bit(Bitmap image)
        {
            // 24 비트로 새로운 이미지 생성
            Bitmap convertedImage = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // 이미지 복사
            using (Graphics g = Graphics.FromImage(convertedImage))
            {
                g.DrawImage(image, 0, 0);
            }

            return convertedImage;
        }

        public bool GenerateQRimg(string strOtpUrl)
        {

            // OTP URL 생성
            try
            {
                // otpauth://totp/SecureGate:KS0002?secret=JNJTAMBQGJFVGMBQ&issuer=SecureGate
                // otpauth://totp/{issuer}:{accountName}?secret={secretKey}&issuer={issuer}
                //string otpUrl = $"otpauth://totp/{encodedIssuer}:{encodedAccountName}?secret={encodedSecretKey}&issuer={encodedIssuer}";

                Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-Start!");

                if (strOtpUrl.IndexOf("otpauth://") != 0)
                {
                    Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-End(Error)!");
                    return false;
                }
                
                // QR 코드 생성
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(strOtpUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                qrCodeImage = qrCode.GetGraphic(10); // 이미지 크기를 조정합니다.
                //qrCodeImage.SetPixel(240, 240, Color.White);

                // 24 비트로 이미지 변환
                //convertedImage = ConvertTo24Bit(qrCodeImage);

                string strQRImgPath = CsSystemFunc.GetCurrentModulePath();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    strQRImgPath += "\\wwwroot\\otp_qrcode.png";
                else
                    strQRImgPath += "/wwwroot/otp_qrcode.png";

                // 기존꺼 삭제
                //CsFileFunc.DeleteFile(strQRImgPath);
                //Thread.Sleep(500);

                // 32bit 그대로 사용
                qrCodeImage.Save(strQRImgPath, System.Drawing.Imaging.ImageFormat.Png);
                qrCodeImage.Dispose();
                qrCodeImage = null;

                //convertedImage.Dispose();
                //convertedImage = null;

                // 5초 TimeOut
                int nIdx = 0;
                for (; nIdx < 50; nIdx++)
                {
                    
                    if (System.IO.File.Exists(strQRImgPath))
                    {
                        long SizeFile = CsFileFunc.GetFileSize(strQRImgPath);
                        Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-Size : {SizeFile}");
                        if (SizeFile > 0)
                            break;
                    }
                    Thread.Sleep(100);
                }
                Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-End : {((nIdx < 50) ? "Success!" : "Failed!")}");

                // 순서 바꾸면 파일 이미지 안나오기 시작함.
                //Thread.Sleep(1000);
                if (nIdx < 50)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GenerateQRimg, exception(MSG) : {ex.Message}");
                return false;
            }
            finally
            {
            }

        }
        public bool GenerateQRimg(string issuer, string accountName, string secretKey)
        {

            // OTP URL 생성
            try
            {
                // otpauth://totp/SecureGate:KS0002?secret=JNJTAMBQGJFVGMBQ&issuer=SecureGate
                // otpauth://totp/{issuer}:{accountName}?secret={secretKey}&issuer={issuer}

                Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-Start!");

                string encodedIssuer = Uri.EscapeDataString(issuer);
                string encodedAccountName = Uri.EscapeDataString(accountName);
                string encodedSecretKey = Uri.EscapeDataString(secretKey);

                string otpUrl = $"otpauth://totp/{encodedAccountName}?secret={encodedSecretKey}&issuer={encodedIssuer}";
                //string otpUrl = $"otpauth://totp/{encodedIssuer}:{encodedAccountName}?secret={encodedSecretKey}&issuer={encodedIssuer}";

                // QR 코드 생성
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                qrCodeImage = qrCode.GetGraphic(10); // 이미지 크기를 조정합니다.
                //qrCodeImage.SetPixel(240, 240, Color.White);

                // 24 비트로 이미지 변환
                //convertedImage = ConvertTo24Bit(qrCodeImage);

                string strQRImgPath = CsSystemFunc.GetCurrentModulePath();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    strQRImgPath += "\\wwwroot\\otp_qrcode.png";
                else
                    strQRImgPath += "/wwwroot/otp_qrcode.png";

                // 기존꺼 삭제
                //CsFileFunc.DeleteFile(strQRImgPath);
                //Thread.Sleep(500);

                // 32bit 그대로 사용
                qrCodeImage.Save(strQRImgPath, System.Drawing.Imaging.ImageFormat.Png);
                qrCodeImage.Dispose();                
                qrCodeImage = null;

                //convertedImage.Dispose();
                //convertedImage = null;

                // 5초 TimeOut
                int nIdx = 0;
                for (; nIdx < 50; nIdx++)
                {
                    if (System.IO.File.Exists(strQRImgPath))
                        if (CsFileFunc.GetFileSize(strQRImgPath) > 15000)
                            break;
                    Thread.Sleep(100);
                }
                Log.Logger.Here().Information($"GenerateQRimg, GenerateQRimg-End : {((nIdx < 50) ? "Success!" : "Failed!")}");

                // 순서 바꾸면 파일 이미지 안나오기 시작함.
                // 파일 존재 확인하고도 약간의 sleep 필요
                //Thread.Sleep(1000);
                if (nIdx < 50)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GenerateQRimg, exception(MSG) : {ex.Message}");
                return false;
            }
            finally
            {
            }

        }

        public void Dispose()
        {
            strSecureKey = "";

            if (qrCodeImage != null)
            {
                qrCodeImage.Dispose();
                qrCodeImage = null;
            }
                
        }

        public bool GetSecureKeyFromOtpUrl(string strUrl, out string strRet)
        {
            strRet = "";
            if (strUrl.Length > 0)
            {
                int nPos = strUrl.IndexOf("?secret=");
                if (nPos < 0)
                    nPos = strUrl.IndexOf("&secret=");

                if (nPos > 0)
                {
                    // otpauth://totp/SecureGate:KS0002?secret=JNJTAMBQGJFVGMBQ&issuer=SecureGate
                    strRet = strUrl.Substring(nPos + 8);
                    nPos = strRet.IndexOf('&');
                    if (nPos > 0)
                        strRet = strRet.Substring(0, nPos);
                }

            }

            return (strRet.Length > 0);
        }


    }

    public class CsPasswdValidCheckfunc
    {
        Regex regex = new Regex(@"^.*([ ]+).*$");       //공백체크
        Regex regex2 = new Regex(@"^.*([A-Z]+).*$");    //대문자 존재 체크
        Regex regex3 = new Regex(@"^.*([a-z]+).*$");    //소문자 존재 체크
        Regex regex4 = new Regex(@"^.*([0-9]+).*$");    //숫자 존재 체크
        Regex regex5 = new Regex(@"^.*([^A-Za-z0-9]+).*$");    //특수문자 존재 체크
        string[] ArrayStrKeyBoard = new string[] { "`1234567890-=", "~!@#$%^&*()_+", "qwertyuiop[]\\", "QWERTYUIOP{}|",
            "asdfghjkl;'\"", "ASDFGHJKL:\"", "zxcvbnm,./", "ZXCVBNM<>?",
            "=-0987654321`", "+_)(*&^%$#@!~", "\\][poiuytrewq", "|}{POIUYTREWQ", 
            "\"';lkjhgfdsa", "\":LKJHGFDSA", "/.,mnbvcxz", "?><MNBVCXZ"};        

        /// <summary>
        /// 공백문자가 있는지 체크
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool GetEmptyString(string strData)
        {
            if (regex.IsMatch(strData))
                return true;

            return false;
        }

        /// <summary>
        /// 숫자, 대문자(영문), 소문자(영문), 특수문자가 각항목이 존재하면 +1, (전부다있으면 : 4)
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public int GetComplexCnt(string strData)
        {
            int nComplexCnt = 0;
            if (regex2.IsMatch(strData))
                nComplexCnt++;
            if (regex3.IsMatch(strData))
                nComplexCnt++;
            if (regex4.IsMatch(strData))
                nComplexCnt++;
            if (regex5.IsMatch(strData))
                nComplexCnt++;

            return nComplexCnt;
        }

        /// <summary>
        /// reture : True - 같은문자가 지정한 개수 만큼 반복되지 않음
        /// reture : false - 같은문자가 지정한 개수 만큼 반복됨
        /// </summary>
        /// <param name="chPasswd"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        bool CheckSameChar(string chPasswd, int iCount)
        {

            bool bFindSameChar = false;
            int nLength = chPasswd.Length;

            try
            {
                for (int n = 0; n < nLength - (iCount - 1); n++)
                {
                    if (chPasswd[n] == chPasswd[n + 1])
                    {
                        for (int i = 1; i < iCount - 1; i++)
                        {
                            if (chPasswd[n + i] != chPasswd[n + i + 1])
                            {
                                bFindSameChar = false;
                                break;
                            }
                            bFindSameChar = true;
                        }
                    }

                    if (bFindSameChar == true)
                        return false;
                }

            }
            catch(Exception ex)
            {
                Log.Logger.Here().Error($"CheckSameChar, exception(MSG) : {ex.Message}");
            }

            return true;
        }


        /// <summary>
        /// 동일한 문자·숫자의 연속적인 존재유무(true:존재함,false:존재X) <br></br>
        /// nCharKeyCount : 연속으로 존재해야하는 문자개수
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool GetSameCharCheck(string strData, int nCharKeyCount)
        {
            if (CheckSameChar(strData, nCharKeyCount))
                return false;

            return true;
        }

        /// <summary>
        /// true : 키보드상의 연속된 문자 또는 숫자의 순차적 입력이 입력된 문자열에 있음
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool GetKeyBoardContinuousWord(string strData, int nCharKeyCount)
        {

            try
            {
                foreach (string strKeyLine in ArrayStrKeyBoard)
                {
                    if (PasswdValiation(strKeyLine, strData, nCharKeyCount) == false)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetKeyBoardContinuousWord, Exception(MSG) : {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// passwd에 지정한 연속된 Key가 존재하는지 파악하는 함수
        /// </summary>
        /// <param name="strKeyLine"></param>
        /// <param name="strPWdata"></param>
        /// <param name="nCharKeyCount"></param>
        /// <returns></returns>
        bool PasswdValiation(string strKeyLine, string strPWdata, int nCharKeyCount)
        {

            string strTemp = "";
            int len = 0;

            if (nCharKeyCount > 0)
                len = strPWdata.Length;

            for (int n = 0; n < len; n++)
            {
                strTemp = "";
                if (nCharKeyCount > 0)
                {
                    // C#에서는 확인필요 - exception 발생
                    if (n + nCharKeyCount > len)
                        return true;

                    strTemp = strPWdata.Substring(n, nCharKeyCount);
                    if (strTemp.Length < nCharKeyCount)
                        return true;
                }
                else
                    strTemp = strPWdata;


                int pos = strKeyLine.IndexOf(strTemp);
                if (pos > -1)
                    return false;
            }

            return true;
        }

    }

}
