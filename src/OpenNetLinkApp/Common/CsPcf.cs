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
using System.Runtime.InteropServices;
using static OpenNetLinkApp.Common.Enums;
using OpenNetLinkApp.Data.SGDicData.SGNpki;
using OpenNetLinkApp.Data.SGDicData.SGGpki;

namespace OpenNetLinkApp.Common
{


    public class CsPkiFile
    {

        SGGpkiLib sgGpkiLib = new SGGpkiLib();

        public CsPkiFile()
        {
        }

        public bool init()
        {
            sgGpkiLib.GPKI_Init();
            return true;
        }

        public bool GetSearchNpkiDerFiles(List<string> listNpki, string strSearchPath)
        {
            //string strSearchPathFile = strSearchPath + "\\*.*";

            DirectoryInfo di = new DirectoryInfo(strSearchPath);
            string strUpperData = "";
            bool bIsuser = false;

            foreach (var item in di.GetFiles())
            {

                strUpperData = item.FullName.ToUpper();
                bIsuser = strUpperData.Contains("\\USER\\");

                if (string.Compare(item.Name, "signCert.der", true) == 0 &&
                    NPKIFileInfo.isDerFile(item.FullName) && bIsuser)
                    //item.FullName.Contains("\\USER\\", StringComparison.OrdinalIgnoreCase))
                {
                    listNpki.Add(item.FullName);
                    Log.Logger.Here().Information($"GetSearchNpkiList, filesName : {item.FullName}, Ext:{item.Extension}, files-Attributes(###) : {item.Attributes.ToString()}");
                }
            }

            foreach (var item in di.GetDirectories())
            {
                if (string.Compare(item.Name, ".", true) != 0 &&
                    string.Compare(item.Name, "..", true) != 0)
                {
                    GetSearchNpkiDerFiles(listNpki, item.FullName);
                    //Log.Logger.Here().Information($"GetSearchNpkiList, FolderName : {item.FullName}, files-Attributes(###) : {item.Attributes.ToString()}");
                }
            }

            // C:\Users\kwkim\AppData\LocalLow\NPKI\CrossCert 내부 뒤지도록 작업
            /*foreach (var item in Directory.GetDirectories(sDir))
            {
                System.IO.FileInfo fInfo = new System.IO.FileInfo(item);
                temp.Add(fInfo);
                DirSearch(item, temp);
                Log.Logger.Here().Information($"### - DirSearch, Directories : {item}");
            }*/

            return true;
        }

        /// <summary>
        /// pki fileList를 얻는 함수<br></br>
        /// 
        /// </summary>
        /// <param name="listNpki"></param>
        /// <param name="eSystemPos"></param>
        /// <returns></returns>
        public bool GetNpkiList(List<string> listNpki, EnumSysPos eSystemPos, string strVolume = "")
        {
            int nPos = -1;
            //string strTmpPath = "";
            //string strDriveName = "C:\\";
            string strPkiSearchPath = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (EnumSysPos.UserData == eSystemPos)
                {
                    //strTmpPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    strPkiSearchPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    nPos = strPkiSearchPath.LastIndexOf('\\');
                    if (nPos != -1)
                        strPkiSearchPath = strPkiSearchPath.Substring(0, nPos);

                    strPkiSearchPath += "\\LocalLow\\NPKI";
                }
                else if (EnumSysPos.ProgramFiles == eSystemPos)
                {

                }
                else if (EnumSysPos.ProgramFilesX86 == eSystemPos)
                {

                }
                else if (EnumSysPos.OtherDrive == eSystemPos)
                {

                    if ((strVolume?.Length ?? 0) == 0)
                    {
                        Log.Logger.Here().Information($"GetNpkiList, Position : {eSystemPos}, strVolume : {strVolume}");
                        return false;
                    }

                    strPkiSearchPath = strVolume + "\\NPKI";
                }

            }

            Log.Logger.Here().Information($"GetNpkiList, Position : {eSystemPos}, Path : {strPkiSearchPath}");

            if (strPkiSearchPath.Length > 0 && Directory.Exists(strPkiSearchPath))
                GetSearchNpkiDerFiles(listNpki, strPkiSearchPath);

            return ((listNpki?.Count ?? 0) > 0 );
        }

        /// <summary>
        /// signCert.der 파일이 있는 경로의 npki 관련 정보 display
        /// </summary>
        /// <param name="listNpkiPath"></param>
        /// <param name="listNpkiInfo"></param>
        /// <returns></returns>
        public bool GetNpkiFileinfoList(List<string> listNpkiPath, List<NPKIFileInfo> listNpkiInfo, EnumSysPos eSystemPos)
        {
            if (listNpkiPath.Count < 1)
                return false;

            
            bool bGetNpkiDatainfo = false;
            foreach(string strFilePath in listNpkiPath)
            {
                if(File.Exists(strFilePath))
                {
                    NPKIFileInfo infoData = new NPKIFileInfo();

                    if (sgGpkiLib.LoadDERfileInfo(strFilePath, infoData))
                    {
                        bGetNpkiDatainfo = true;
                        infoData.m_strFileName = strFilePath;
                        infoData.ePos = eSystemPos;
                        listNpkiInfo.Add(infoData);
                    }
                }
            }

            return bGetNpkiDatainfo;
        }


        public bool GetNpkiinfoList(List<NPKIFileInfo> listNpkiinfo, EnumSysPos eSystemPos, string strDriveVolume = "")
        {
            Log.Logger.Here().Information($"GetNpkiinfoList, Position : {eSystemPos}");     // , Path : {strPkiSearchPath}

            //string strDriveName = "C:\\";
            List<string> listNpkiPath = new List<string>();

            if (eSystemPos == EnumSysPos.OtherDrive)
                GetNpkiList(listNpkiPath, eSystemPos, strDriveVolume);
            else
                GetNpkiList(listNpkiPath, eSystemPos);

            if (listNpkiPath.Count < 1)
                return false;

            GetNpkiFileinfoList(listNpkiPath, listNpkiinfo, eSystemPos);

            return (listNpkiinfo.Count > 0);
        }

        

    }


    public class CsPcfFile
    {


        private int nVer = 0;
        private SGRSACrypto sgRSACrypto = new SGRSACrypto();
        UTF8Encoding utf8Enc = new UTF8Encoding();

        public CsPcfFile(int nSetrVer = 1)
        {
            nVer = nSetrVer;
        }


        private bool GetEncNeedString(string strFIlePath, out string strData)
        {
            strData = "";
            int nPos = strFIlePath.IndexOf("\\NPKI\\");
            if (nPos < 1)
                return false;

            string strPathData = strFIlePath.Substring(nPos + "\\NPKI\\".Length);
            sgRSACrypto.ValueEncrypt(strPathData, out strData); // 수정필요
            Log.Logger.Here().Information($"GetEncNeedString, input-Path : {strPathData}, output-Path : {strData}");

            return true;
        }


        public bool makeNpkiFIlesToPcf(List<NPKIFileInfo> listnpkifiles, string strPcfFilePath)
        {

            Log.Logger.Here().Information($"makeNpkiFIlesToPcf, Start - Make Pcf File !");

            if ( (listnpkifiles?.Count() ?? 0) < 1)
            {
                Log.Logger.Here().Information($"makeNpkiFIlesToPcf, npki fileList Empty!");
                return false;
            }

            using (FileStream fs = File.Create(strPcfFilePath))
            {
                try
                {
                    /*Buf = new byte[FileAddManage.MaxBufferSize];
                    if (fs.Length > 2)
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        if (fs.Read(Buf, 0, Buf.Length) > 2)
                        {
                            bRet = FileAddManage.IsDER(Buf, "");
                        }
                    }*/

                    int nFilePos = 0;

                    // 1. Ver정보 Write
                    byte[] byteArrayVer = BitConverter.GetBytes(nVer);
                    fs.Write(byteArrayVer, 0, byteArrayVer.Length);
                    nFilePos += byteArrayVer.Length;

                    // List file들 읽어서 pcf 파일만들기
                    int nDerFileCount = 0;
                    foreach (NPKIFileInfo fileinfo in listnpkifiles)
                    {

                        Log.Logger.Here().Information($"makeNpkiFIlesToPcf, Pki File : {fileinfo.m_strFileName}");
                        if (File.Exists(fileinfo.m_strFileName) == false)
                        {
                            Log.Logger.Here().Information($"makeNpkiFIlesToPcf, Not Found Pki File : {fileinfo.m_strFileName}");
                            return false;
                        }

                        // Der file List 받아옴
                        if (fileinfo.GetDerFileList())
                        {
                            // pcf 파일 만드는 작업
                            nDerFileCount += fileinfo.m_listDerFilePath.Count;
                        }
                    }

                    Log.Logger.Here().Information($"makeNpkiFIlesToPcf, DerFile-Count : {nDerFileCount}");
                    if (nDerFileCount < 1)
                        return false;

                    // 2. data개수 Write
                    byte[] byteArrFileCount = BitConverter.GetBytes(nDerFileCount);
                    fs.Write(byteArrFileCount, 0, byteArrFileCount.Length);
                    nFilePos += byteArrFileCount.Length;

                    // 3. 개별 파일마다 Write
                    foreach (NPKIFileInfo fileinfo in listnpkifiles)
                    {
                        string strEncData = "";
                        foreach (string strDerfilePath in fileinfo.m_listDerFilePath)
                        {

                            // 3.1. Type data Write
                            byte[] byteArrType = BitConverter.GetBytes((int)fileinfo.ePos);
                            //fs.Seek(nFilePos, SeekOrigin.Current);
                            fs.Write(byteArrType, 0, byteArrType.Length);
                            nFilePos += byteArrType.Length;

                            strEncData = "";
                            if (GetEncNeedString(strDerfilePath, out strEncData) == false)
                            {
                                Log.Logger.Here().Information($"makeNpkiFIlesToPcf, EncCode Error(#) : {strDerfilePath}");
                                return false;
                            }

                            // 3.2. File Name data Write
                            // utf8로 변환후 저장
                            byte[] byteArrayPathEnc = utf8Enc.GetBytes(strEncData);
                            byte[] byteArrayLength = BitConverter.GetBytes(byteArrayPathEnc.Length);

                            // 3.2.1 FileName Length Write
                            fs.Write(byteArrayLength, 0, byteArrayLength.Length);
                            nFilePos += byteArrayLength.Length;

                            // 3.2.2 FileName Data Write
                            fs.Write(byteArrayPathEnc, 0, byteArrayPathEnc.Length);
                            nFilePos += byteArrayPathEnc.Length;

                            // 3.3. File Data Read / Write
                            using (FileStream fsDer = File.OpenRead(strDerfilePath))
                            {
                                try
                                {
                                    if(fsDer.Length > 0)
                                    {
                                        byte[] Buf = null;
                                        Buf = new byte[fsDer.Length];
                                        fsDer.Seek(0, SeekOrigin.Begin);
                                        fsDer.Read(Buf, 0, Buf.Length);

                                        // 3.3.1 FileData Length Write
                                        byte[] byteArrDataLength = BitConverter.GetBytes(Buf.Length);
                                        fs.Write(byteArrDataLength, 0, byteArrDataLength.Length);
                                        nFilePos += byteArrDataLength.Length;

                                        // 3.3.2 FileData Write
                                        fs.Write(Buf, 0, Buf.Length);
                                        nFilePos += Buf.Length;
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    Log.Logger.Here().Information($"makeNpkiFIlesToPcf, OpenRead - Exception(MSG) : {ex1.Message}");
                                }
                                finally
                                {
                                    fsDer?.Dispose();
                                }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.Logger.Here().Information($"makeNpkiFIlesToPcf, Exception(MSG) : {ex.Message}");
                }
                finally
                {
                    fs?.Dispose();
                }
            }


            Log.Logger.Here().Information($"makeNpkiFIlesToPcf, End - Make Pcf File !");

            return true;
        }


    }

}

