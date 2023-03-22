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

namespace OpenNetLinkApp.Common
{

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

