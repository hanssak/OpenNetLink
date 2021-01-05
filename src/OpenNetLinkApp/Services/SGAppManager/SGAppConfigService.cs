using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;
using OpenNetLinkApp.Models.SGConfig;
using HsNetWorkSG;

using Serilog;
using Serilog.Events;
using AgLogManager;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGAppConfigService
    {
        ref ISGAppConfig AppConfigInfo { get; }
        string GetClipBoardHotKey(int groupId);
        List<bool> GetClipBoardModifier(int groupId);

        List<bool> GetClipBoardModifierWhenNetOver(int groupId, int nIdx);

        char GetClipBoardVKey(int groupId);

        char GetClipBoardVKeyWhenNetOver(int groupId, int nIdx);

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
        //bool GetUseScreenLock();
        bool GetScreenLock();
        int GetScreenTime();
        string GetLastUpdated();
        string GetSWVersion();
        string GetSWCommitId();
        LogEventLevel GetLogLevel();
        bool GetUseApprWaitNoti();
        string GetUpdateSvcIP();
        string GetUpdatePlatform();
        void SetUpdatePlatform(string strPlatFrom);
        bool GetUseLogLevel();
        bool GetUseGPKILogin(int groupID);
        bool GetUseOverNetwork2();
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
                    Log.Information($"- AppEnvSetting Load Completed : [{AppConfig}]");
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


        public string GetClipBoardHotKeyWhenNetOver(int groupId, int nIdx)
        {
            (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver ??= new List<string>() { "N,Y,N,Y,F", "N,Y,N,Y,F" };

/*            List<string> listIdxNetOver = new List<string>() { "N,Y,N,Y,V,2" };
            Dictionary<int, List<string>> dicIdxHotKey = new Dictionary<int, List<string>>();
            dicIdxHotKey.TryAdd(nIdx, listIdxNetOver);*/

            return AppConfigInfo.ClipBoardHotKeyNetOver[groupId];
        }


        public List<bool> GetClipBoardModifier(int groupId)
        {
            string strHotKey;
            String[] HotKeylist;

            strHotKey = GetClipBoardHotKey(groupId);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            List<bool> ValueList = new List<bool>();
            if(HotKeylist.Length == 5)
            {
                if(HotKeylist[(int)HOTKEY_MOD.WINDOW].Equals("Y")) ValueList.Insert((int)HOTKEY_MOD.WINDOW,true);
                else ValueList.Insert((int)HOTKEY_MOD.WINDOW,false);
                if(HotKeylist[(int)HOTKEY_MOD.CTRL].Equals("Y")) ValueList.Insert((int)HOTKEY_MOD.CTRL,true);
                else ValueList.Insert((int)HOTKEY_MOD.CTRL,false);
                if(HotKeylist[(int)HOTKEY_MOD.ALT].Equals("Y")) ValueList.Insert((int)HOTKEY_MOD.ALT,true);
                else ValueList.Insert((int)HOTKEY_MOD.ALT,false);
                if(HotKeylist[(int)HOTKEY_MOD.SHIFT].Equals("Y")) ValueList.Insert((int)HOTKEY_MOD.SHIFT,true);
                else ValueList.Insert((int)HOTKEY_MOD.SHIFT,false);
            }
            else /// default
            {
                ValueList.Insert((int)HOTKEY_MOD.WINDOW,false);
                ValueList.Insert((int)HOTKEY_MOD.CTRL,true);
                ValueList.Insert((int)HOTKEY_MOD.ALT,false);
                ValueList.Insert((int)HOTKEY_MOD.SHIFT,true);
            }

            return ValueList;
        }

        /**
        *@brief 3망에서 제일 끝단 단축키 설정된 값 받아옴
        */
        public List<bool> GetClipBoardModifierWhenNetOver(int groupId, int nIdx)
        {
            string strHotKey;
            String[] HotKeylist;

            strHotKey = GetClipBoardHotKeyWhenNetOver(groupId, nIdx);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            List<bool> ValueList = new List<bool>();
            if (HotKeylist.Length == 6)
            {
                if (HotKeylist[(int)HOTKEY_MOD.WINDOW].Equals("Y")) 
                    ValueList.Insert((int)HOTKEY_MOD.WINDOW, true);
                else 
                    ValueList.Insert((int)HOTKEY_MOD.WINDOW, false);

                if (HotKeylist[(int)HOTKEY_MOD.CTRL].Equals("Y")) 
                    ValueList.Insert((int)HOTKEY_MOD.CTRL, true);
                else 
                    ValueList.Insert((int)HOTKEY_MOD.CTRL, false);

                if (HotKeylist[(int)HOTKEY_MOD.ALT].Equals("Y")) 
                    ValueList.Insert((int)HOTKEY_MOD.ALT, true);
                else 
                    ValueList.Insert((int)HOTKEY_MOD.ALT, false);

                if (HotKeylist[(int)HOTKEY_MOD.SHIFT].Equals("Y")) 
                    ValueList.Insert((int)HOTKEY_MOD.SHIFT, true);
                else 
                    ValueList.Insert((int)HOTKEY_MOD.SHIFT, false);

                if (HotKeylist[(int)HOTKEY_MOD.VKEY].Length > 0)
                    ValueList.Insert((int)HOTKEY_MOD.VKEY, true);
                else
                    ValueList.Insert((int)HOTKEY_MOD.VKEY, false);

                if (HotKeylist[(int)HOTKEY_MOD.NETOVER_IDX] == nIdx.ToString() && 
                    HotKeylist[(int)HOTKEY_MOD.NETOVER_IDX].Length > 0)
                    ValueList.Insert((int)HOTKEY_MOD.NETOVER_IDX, true);
                else
                    ValueList.Insert((int)HOTKEY_MOD.NETOVER_IDX, false);


            }
            else /// default
            {
                ValueList.Insert((int)HOTKEY_MOD.WINDOW, false);
                ValueList.Insert((int)HOTKEY_MOD.CTRL, true);
                ValueList.Insert((int)HOTKEY_MOD.ALT, false);
                ValueList.Insert((int)HOTKEY_MOD.SHIFT, true);
                ValueList.Insert((int)HOTKEY_MOD.VKEY, true);
                ValueList.Insert((int)HOTKEY_MOD.NETOVER_IDX, false);
            }

            return ValueList;
        }


        public char GetClipBoardVKey(int groupId)
        {
            char cVKey;
            string strHotKey;
            String[] HotKeylist;

            strHotKey = GetClipBoardHotKey(groupId);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            if(HotKeylist.Length == 5)
            {
                cVKey = char.Parse(HotKeylist[(int)HOTKEY_MOD.VKEY]);
            }
            else /// default
            {
                cVKey = 'V';
            }

            return cVKey;
        }

        public char GetClipBoardVKeyWhenNetOver(int groupId, int nIdx)
        {
            char cVKey;
            string strHotKey;
            String[] HotKeylist;

            strHotKey = GetClipBoardHotKeyWhenNetOver(groupId, nIdx);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            if (HotKeylist.Length == 6 && nIdx.ToString() == HotKeylist[(int)HOTKEY_MOD.NETOVER_IDX])
            {
                cVKey = char.Parse(HotKeylist[(int)HOTKEY_MOD.VKEY]);
            }
            else /// default
            {
                cVKey = 'V';
            }

            return cVKey;
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

            int count = AppConfigInfo.RecvDownPath.Count;
            for (int i = 0; i < count; i++)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    AppConfigInfo.RecvDownPath[i] = AppConfigInfo.RecvDownPath[i].Replace("/", "\\");
                }
                else
                {
                    AppConfigInfo.RecvDownPath[i] = AppConfigInfo.RecvDownPath[i].Replace("\\", "/");
                }
            }
            string strDownPath = AppConfigInfo.RecvDownPath[groupId];
            strDownPath = ConvertRecvDownPath(strDownPath);
            return strDownPath;
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
        //public bool GetUseScreenLock()
        //{
        //    return AppConfigInfo.bUseScreenLock;
        //}
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
        public string GetSWCommitId()
        {
            return AppConfigInfo.SWCommitId;
        }
        public LogEventLevel GetLogLevel()
        {
            return AppConfigInfo.LogLevel;
        }
        public bool GetUseApprWaitNoti()
        {
            return AppConfigInfo.bUseApprWaitNoti;
        }
        public string GetUpdateSvcIP()
        {
            return AppConfigInfo.UpdateSvcIP;
        }
        public string GetUpdatePlatform()
        {
            return AppConfigInfo.UpdatePlatform;
        }

        public void SetUpdatePlatform(string strPlatForm)
        {
            AppConfigInfo.UpdatePlatform = strPlatForm;
        }
        public bool GetUseLogLevel()
        {
            return AppConfigInfo.bUseLogLevel;
        }
        public string ConvertRecvDownPath(string DownPath)
        {
            string strDownPath = "";
            if (DownPath.Contains("%USERPATH%"))
            {
                string strFullHomePath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                strDownPath = DownPath.Replace("%USERPATH%", strFullHomePath);
            }
            else if (DownPath.Contains("%MODULEPATH%"))
            {
                string strModulePath = System.IO.Directory.GetCurrentDirectory();
                strDownPath = DownPath.Replace("%MODULEPATH%", strModulePath);
            }
            else
                strDownPath = DownPath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strDownPath = strDownPath.Replace("/", "\\");
            }
            else
            {
                strDownPath = strDownPath.Replace("\\", "/");
            }
            return strDownPath;
        }
        public bool GetUseGPKILogin(int groupID)
        {
            return AppConfigInfo.listUseGpkiLogin[groupID];
        }

        public bool GetUseOverNetwork2()
        {
            return AppConfigInfo.bUseOverNetwork2;
        }
    }
}