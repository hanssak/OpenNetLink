using System;
using System.IO;
using System.Runtime.Serialization.Json;
using OpenNetLinkApp.Models.SGConfig;
using Serilog;
using AgLogManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGVersionConfigService
    {
        ref ISGVersionConfig VersionConfigInfo { get; }        
        string GetLastUpdated();
        string GetSWVersion();
        string GetSWCommitId();       
        string GetUpdatePlatform();
        void SetUpdatePlatform(string strPlatform);
        bool isUpperVersion(string localVer, string serverVer);
    }
    internal class SGVersionConfigService : ISGVersionConfigService
    {
        private ISGVersionConfig _VersionConfigInfo;
        public ref ISGVersionConfig VersionConfigInfo => ref _VersionConfigInfo;

        public SGVersionConfigService()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGVersionConfig));
            string VersionConfig = Environment.CurrentDirectory+"/wwwroot/conf/AppVersion.json";

            HsLogDel hsLog = new HsLogDel();
            hsLog.Delete(7);    // 7일이전 Log들 삭제

            Log.Information($"- VersionConfig Path: [{VersionConfig}]");
            if(File.Exists(VersionConfig))
            {
                try
                {
                    Log.Information($"- VersionConfig Loading... : [{VersionConfig}]");
                    //Open the stream and read it back.
                    using (FileStream fs = File.OpenRead(VersionConfig))
                    {
                        SGVersionConfig versionConfig = (SGVersionConfig)serializer.ReadObject(fs);
                        _VersionConfigInfo = versionConfig;
                    }
                    Log.Information($"- VersionConfig Load Completed : [{VersionConfig}]");
                }
                catch(Exception ex)
                {
                    Log.Warning($"\nMessage ---\n{ex.Message}");
                    Log.Warning($"\nHelpLink ---\n{ex.HelpLink}");
                    Log.Warning($"\nStackTrace ---\n{ex.StackTrace}");
                    _VersionConfigInfo = new SGVersionConfig();
                }
            }
            else
            {
                _VersionConfigInfo = new SGVersionConfig();
            }
        }    
        
        public string GetLastUpdated()
        {
            return VersionConfigInfo.LastUpdated;
        }
        public string GetSWVersion()
        {
            return VersionConfigInfo.SWVersion;
        }
        public string GetSWCommitId()
        {
            return VersionConfigInfo.SWCommitId;
        }       
        public string GetUpdatePlatform()
        {
            return VersionConfigInfo.UpdatePlatform;
        }     
        public void SetUpdatePlatform(string strPlatform)
        {
            VersionConfigInfo.UpdatePlatform = strPlatform;
        }
        private string[] GetVersion(string strVersion)
        {
            string str = strVersion.Replace("OPENNETLINK", "");
            str = str.Substring(0, str.LastIndexOf(":"));
            string[] tmpVer = str.Split(".");

            return tmpVer;
        }

        public bool isUpperVersion(string serverVer, string localVer)
        {
            string[] locVer = GetVersion(localVer);
            string[] svrVer = GetVersion(serverVer);
            bool res = false;

            for(int i = 0; i < locVer.Length; i++)
            {
                int loc = Convert.ToInt32(locVer[i]);
                int svr = Convert.ToInt32(svrVer[i]);

                if (loc < svr)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
    }
}