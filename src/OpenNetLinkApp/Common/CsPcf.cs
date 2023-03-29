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

namespace OpenNetLinkApp.Common
{


    public class CsPkiFile
    {
        public CsPkiFile()
        {

        }



        static public bool GetSearchNpkiDerFiles(List<string> listNpki, string strSearchPath)
        {
            //string strSearchPathFile = strSearchPath + "\\*.*";

            DirectoryInfo di = new DirectoryInfo(strSearchPath);

            foreach (var item in di.GetFiles())
            {
                if (string.Compare(item.Name, "signCert.der", true) == 0)
                    listNpki.Add(item.FullName);
                Log.Logger.Here().Information($"GetSearchNpkiList, filesName : {item.FullName}, Ext:{item.Extension}, files-Attributes(###) : {item.Attributes.ToString()}");
            }

            foreach (var item in di.GetDirectories())
            {



                if (string.Compare(item.Name, ".", true) != 0)
                    GetSearchNpkiDerFiles(listNpki, item.FullName);                
                Log.Logger.Here().Information($"GetSearchNpkiList, filesName : {item.FullName}, Ext:{item.Extension}, files-Attributes(###) : {item.Attributes.ToString()}");
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
        static public bool GetNpkiList(List<string> listNpki, EnumSysPos eSystemPos)
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
                
            }

            Log.Logger.Here().Information($"GetNpkiList, Position : {eSystemPos}, Path : {strPkiSearchPath}");

            GetSearchNpkiDerFiles(listNpki, strPkiSearchPath);

            return true;
        }


        static public bool GetNpkiinfoList(List<NPKIFileInfo> listNpki, EnumSysPos eSystemPos)
        {
            Log.Logger.Here().Information($"GetNpkiinfoList, Position : {eSystemPos}");     // , Path : {strPkiSearchPath}

            string strDriveName = "C:\\";
            List<string> listNpkiPath = new List<string>();
            GetNpkiList(listNpkiPath, EnumSysPos.UserData);
            return true;
        }

        

    }


    public class CsPcfFile
    {


        private int nVer = 0;

        public CsPcfFile(int nSetrVer = 1)
        {
            nVer = nSetrVer;
        }

        public void SetNpkiFIles(List<string> listnpkifiles)
        {

        }

        public bool makeNpkiFIlesToPcf(List<string> listnpkifiles, string strPcfFilePath)
        {
            Log.Logger.Information($"makeNpkiFIlesToPcf, Start - Make Pcf File !");

            if ( (listnpkifiles?.Count() ?? 0) < 1)
            {
                Log.Logger.Information($"makeNpkiFIlesToPcf, npki fileList Empty!");
                return false;
            }

            // List file들 읽어서 pcf 파일만들기



            Log.Logger.Information($"makeNpkiFIlesToPcf, End - Make Pcf File !");

            return true;
        }


    }

}

