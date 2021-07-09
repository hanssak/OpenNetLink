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

        char GetClipBoardVKeyWhenNetOver(int groupId, int nIdx, int nMaxGroupID, int nMaxIdx);

        CLIPALM_TYPE GetClipAlarmType();
        bool GetClipAfterSend();
        bool GetURLAutoTrans(int nGroupID);
        bool GetURLAutoAfterMsg(int nGroupID);
        string GetURLAutoAfterBrowser(int nGroupID);

        string GetForwardUrl(int nGroupID);

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
        bool GetScreenLockUserChange();
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

        bool GetUseNetOverAllsend();

        bool GetFileForward();
        bool GetEmailApproveUse();
        bool GetClipboardApproveUse();
        bool GetShowAdminInfo();
        bool GetUseFileCheckException();
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
            string strKey = "";
            string strValue = "";
            strKey = String.Format($"{groupId}-{nIdx}");
            (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver ??= new Dictionary<string,string>();
            /*            (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver ??= new Dictionary<string, Dictionary<string, string>>();
                        Dictionary<string, string> dicIdxHotKey = new Dictionary<string, string>();

                        if (dicIdxHotKey.TryAdd(nIdx.ToString(), "N,Y,N,Y,Z"))
                            AppConfigInfo.ClipBoardHotKeyNetOver.TryAdd(groupId.ToString(), dicIdxHotKey);

                        return AppConfigInfo.ClipBoardHotKeyNetOver[groupId.ToString()][nIdx.ToString()];*/

            if ((AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver.TryGetValue(strKey, out strValue) == false)
                return "";

            return strValue;
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
            if (HotKeylist.Length == 5)
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

/*                if (HotKeylist[(int)HOTKEY_MOD.VKEY].Length > 0)
                    ValueList.Insert((int)HOTKEY_MOD.VKEY, true);
                else
                    ValueList.Insert((int)HOTKEY_MOD.VKEY, false);
*/

            }
            else /// default
            {
                ValueList.Insert((int)HOTKEY_MOD.WINDOW, false);
                ValueList.Insert((int)HOTKEY_MOD.CTRL, true);
                ValueList.Insert((int)HOTKEY_MOD.ALT, false);
                ValueList.Insert((int)HOTKEY_MOD.SHIFT, true);
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

        /**
        *@brief 3망에서 설정된 알파벳 Key 값 받아옴
        */
        public char GetClipBoardVKeyData(int groupId, int nIdx)
        {
            char cVKey;
            string strHotKey;
            String[] HotKeylist;
            strHotKey = GetClipBoardHotKeyWhenNetOver(groupId, nIdx);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            if (HotKeylist.Length == 5)
            {
                cVKey = char.Parse(HotKeylist[(int)HOTKEY_MOD.VKEY]);
            }
            else /// default
            {
                cVKey = ' ';
            }

            return cVKey;
        }


        public char GetClipBoardVKeyWhenNetOver(int groupId, int nIdx, int nMaxGroupID, int nMaxIdx)
        {
            char cVKey;
            char cVKeyUsed;
            char cVKeyUsedOther;
            string strHotKey;
            String[] HotKeylist;
            int nJdx = 0;
            int nKdx = 0;
            bool bIsalone = false;

            strHotKey = GetClipBoardHotKeyWhenNetOver(groupId, nIdx);
            HotKeylist = strHotKey.Split(",", StringSplitOptions.RemoveEmptyEntries);

            // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
            if (HotKeylist.Length == 5)
            {
                cVKey = char.Parse(HotKeylist[(int)HOTKEY_MOD.VKEY]);
            }
            else /// default
            {
                
                if (nMaxGroupID < 1 || nMaxIdx < 3)
                {
                    cVKey = 'V';
                }
                else
                {

                    // 등록되지 않은 단축키 찾아서 등록

                    cVKeyUsed = 'A';
                    for (; cVKeyUsed <= 'Z'; cVKeyUsed++)
                    {
                        for (nJdx = 0; nJdx < nMaxGroupID; nJdx++)
                        {
                            if (GetClipBoardVKey(nJdx) == cVKeyUsed)
                                break;
                        }

                        if (nJdx == nMaxGroupID)    // ClipBoardHotKey 에서 중복된 것 없음.
                        {
                            for (nKdx = 0; nKdx < nMaxIdx; nKdx++)
                            {
                                if (nKdx == 1) // 기본 단축키 검증은 위에서 했음
                                    continue;

                                cVKeyUsedOther = GetClipBoardVKeyData(groupId, nKdx);
                                if (cVKeyUsedOther != ' ' && cVKeyUsedOther == cVKeyUsed)
                                    break;
                            }

                            if (nKdx == nMaxIdx)
                                bIsalone = true;
                        }

                        if (bIsalone)
                            break;
                    }

                    if (cVKeyUsed > 'Z')
                        cVKeyUsed = 'V';

                    cVKey = cVKeyUsed;
                }

                //cVKey = GetClipBoardVKey(groupId);                
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
        public bool GetURLAutoTrans(int nGroupID)
        {
            //return AppConfigInfo.bURLAutoTrans;

            (AppConfigInfo as SGAppConfig).bURLAutoTrans ??= new List<bool>();
            /*            (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver ??= new Dictionary<string, Dictionary<string, string>>();
                        Dictionary<string, string> dicIdxHotKey = new Dictionary<string, string>();
                        if (dicIdxHotKey.TryAdd(nIdx.ToString(), "N,Y,N,Y,Z"))
                            AppConfigInfo.ClipBoardHotKeyNetOver.TryAdd(groupId.ToString(), dicIdxHotKey);
                        return AppConfigInfo.ClipBoardHotKeyNetOver[groupId.ToString()][nIdx.ToString()];*/

            if ((AppConfigInfo as SGAppConfig).bURLAutoTrans.Count >= nGroupID+1)
                return (AppConfigInfo as SGAppConfig).bURLAutoTrans[nGroupID];

            return true;    // 기본값
        }

        public bool GetURLAutoAfterMsg(int nGroupID)
        {
            //return AppConfigInfo.bURLAutoAfterMsg;
            (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg ??= new List<bool>();

            if ((AppConfigInfo as SGAppConfig).bURLAutoAfterMsg.Count >= nGroupID + 1)
                return (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg[nGroupID];

            return false;   // 기본값
        }

        public string GetURLAutoAfterBrowser(int nGroupID)
        {
            //return AppConfigInfo.strURLAutoAfterBrowser;
            (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser ??= new List<string>();

            if ((AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser.Count >= nGroupID + 1)
                return (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser[nGroupID];

            return "";
        }

        public string GetForwardUrl(int nGroupID)
        {
            //return AppConfigInfo.strForwardUrl;
            (AppConfigInfo as SGAppConfig).strForwardUrl ??= new List<string>();

            if ((AppConfigInfo as SGAppConfig).strForwardUrl.Count >= nGroupID + 1)
                return (AppConfigInfo as SGAppConfig).strForwardUrl[nGroupID];

            return "";
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
        public bool GetScreenLockUserChange()
        {
            return AppConfigInfo.bScreenLockUserChange;
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

        public bool GetUseNetOverAllsend()
        {
            return AppConfigInfo.bUseNetOverAllsend;
        }

        public bool GetFileForward()
        {
            return AppConfigInfo.bFileForward;
        }
        public bool GetEmailApproveUse()
        {
            return AppConfigInfo.bEmailApproveUse;
        }
        public bool GetClipboardApproveUse()
        {
            return AppConfigInfo.bClipboardApproveUse;
        }
        public bool GetShowAdminInfo()
        {
            return AppConfigInfo.bShowAdminInfo;
        }
        public bool GetUseFileCheckException()
        {
            return AppConfigInfo.bUseFileCheckException;
        }
    }
}