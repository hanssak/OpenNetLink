using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using OpenNetLinkApp.Models.SGConfig;
using HsNetWorkSG;

using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppConfigService
    {
        ref ISGAppConfig AppConfigInfo { get; }
        string GetClipBoardHotKey(int groupId);
        CLIPALM_TYPE GetClipAlarmType();
        bool GetClipAfterSend();
        bool GetURLAutoTrans();
        bool GetURLAutoAfterMsg();
        string GetURLAutoAfterBrowser();
        bool GetRMouseFileAddAfterTrans();
        bool GetAfterBasicChk();
        string GetRecvDownPath(int groupId);
        bool GetFileRecvFolderOpen();
        bool GetRecvDownPathChange();
        bool GetManualRecvDownChange();
        bool GetFileRecvTrayFix();
        bool GetApprTrayFix();
        bool GetUserApprActionTrayFix();
        bool GetUserApprRejectTrayFix();
        bool GetExitTrayMove();
        bool GetStartTrayMove();
        bool GetStartProgramReg();
        string GetLanguage();
        bool GetScreenLock();
        int GetScreenTime();
        string GetLastUpdated();
        string GetSWVersion();
    }
    internal class SGAppConfigService : ISGAppConfigService
    {
        private ISGAppConfig _AppConfigInfo;
        public ref ISGAppConfig AppConfigInfo => ref _AppConfigInfo;
        public SGAppConfigService()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGAppConfig));
            string AppConfig = Environment.CurrentDirectory+"/wwwroot/conf/AppEnvSetting.json";

            Log.Information($"- AppEnvSetting Path: [{AppConfig}]");
            if(File.Exists(AppConfig))
            {
                try
                {
                    Log.Information($"- AppEnvSetting Loading... : [{AppConfig}]");
                    //Open the stream and read it back.
                    using (FileStream fs = File.OpenRead(AppConfig))
                    {
                        SGAppConfig appConfig = (SGAppConfig)serializer.ReadObject(fs);
                        _AppConfigInfo = appConfig;
                    }
                }
                catch(Exception ex)
                {
                    Log.Warning($"\nMessage ---\n{ex.Message}");
                    Log.Warning($"\nHelpLink ---\n{ex.HelpLink}");
                    Log.Warning($"\nStackTrace ---\n{ex.StackTrace}");
                    _AppConfigInfo = new SGAppConfig();
                }
            }
            else
            {
                _AppConfigInfo = new SGAppConfig();
            }
        }
    
        public string GetClipBoardHotKey(int groupId)
        {
            (AppConfigInfo as SGAppConfig).ClipBoardHotKey ??= new List<string>(){"N,Y,N,Y,V","N,Y,N,Y,V"};
            return AppConfigInfo.ClipBoardHotKey[groupId];
        }
        public CLIPALM_TYPE GetClipAlarmType()
        {
            return AppConfigInfo.enClipAlarmType;
        }
        public bool GetClipAfterSend()
        {
            return AppConfigInfo.bClipAfterSend;
        }
        public bool GetURLAutoTrans()
        {
            return AppConfigInfo.bURLAutoTrans;
        }
        public bool GetURLAutoAfterMsg()
        {
            return AppConfigInfo.bURLAutoAfterMsg;
        }
        public string GetURLAutoAfterBrowser()
        {
            return AppConfigInfo.strURLAutoAfterBrowser;
        }
        public bool GetRMouseFileAddAfterTrans()
        {
            return AppConfigInfo.bRMouseFileAddAfterTrans;
        }
        public bool GetAfterBasicChk()
        {
            return AppConfigInfo.bAfterBasicChk;
        }
        public string GetRecvDownPath(int groupId)
        {
            (AppConfigInfo as SGAppConfig).RecvDownPath ??= new List<string>(){
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)};
            return AppConfigInfo.RecvDownPath[groupId];
        }
        public bool GetFileRecvFolderOpen()
        {
            return AppConfigInfo.bFileRecvFolderOpen;
        }
        public bool GetRecvDownPathChange()
        {
            return AppConfigInfo.bRecvDownPathChange;
        }
        public bool GetManualRecvDownChange()
        {
            return AppConfigInfo.bManualRecvDownChange;
        }
        public bool GetFileRecvTrayFix()
        {
            return AppConfigInfo.bFileRecvTrayFix;
        }
        public bool GetApprTrayFix()
        {
            return AppConfigInfo.bApprTrayFix;
        }
        public bool GetUserApprActionTrayFix()
        {
            return AppConfigInfo.bUserApprActionTrayFix;
        }
        public bool GetUserApprRejectTrayFix()
        {
            return AppConfigInfo.bUserApprRejectTrayFix;
        }
        public bool GetExitTrayMove()
        {
            return AppConfigInfo.bExitTrayMove;
        }
        public bool GetStartTrayMove()
        {
            return AppConfigInfo.bStartTrayMove;
        }
        public bool GetStartProgramReg()
        {
            return AppConfigInfo.bStartProgramReg;
        }
        public string GetLanguage()
        {
            return AppConfigInfo.strLanguage;
        }
        public bool GetScreenLock()
        {
            return AppConfigInfo.bScreenLock;
        }
        public int GetScreenTime()
        {
            return AppConfigInfo.tScreenTime;
        }
        public string GetLastUpdated()
        {
            return AppConfigInfo.LastUpdated;
        }
        public string GetSWVersion()
        {
            return AppConfigInfo.SWVersion;
        }
    }
}