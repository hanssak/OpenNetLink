using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Linq;

using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Models.SGConfig;
using OpenNetLinkApp.Services.SGAppManager;

using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGCtrlSideUIService
    {
        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        ref ISGAppConfig AppConfigInfo { get; }
        /// <summary>
        /// Control Right SideBar Menu Delegate, Related App Environment Setting.
        /// </summary>
        event Action OnChangeCtrlSide;
        void EmitNotifyStateChangedCtrlSide();
        void SaveAppConfigSerialize();
        void SetClipBoardHotKey(int groupId, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode);
        void SetClipAlarmType(CLIPALM_TYPE alamType);
        void SetClipAfterSend(bool clipAfterSend);
        void SetURLAutoTrans(bool urlAutoTrans);
        void SetURLAutoAfterMsg(bool urlAutoAfterMsg);
        void SetURLAutoAfterBrowser(string urlAutoAfterBrowser);
        void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans);
        void SetAfterBasicChk(bool afterBasicChk);
        void SetRecvDownPath(int groupId, string recvDownPath);
        void SetFileRecvFolderOpen(bool fileRecvFolderOpen);
        void SetRecvDownPathChange(bool recvDownPathChange);
        void SetManualRecvDownChange(bool manualRecvDownChange);
        void SetFileRecvTrayFix(bool fileRecvTrayFix);
        void SetApprTrayFix(bool apprTrayFix);
        void SetUserApprActionTrayFix(bool userApprActionTrayFix);
        void SetUserApprRejectTrayFix(bool userApprRejectTrayFix);
        void SetExitTrayMove(bool exitTrayMove);
        void SetStartTrayMove(bool startTrayMove);
        void SetStartProgramReg(bool startProgramReg);
        void SetLanguage(string language);
        void SetScreenLock(bool screenLock);
        void SetScreenTime(int screenTime);
        void SetLastUpdated(string lastUPdated);
        void SetSWVersion(string swVersion);
        void SetSWCommitId(string swCommitId);
        void SetLogLevel(LogEventLevel logLevel);
        void SetUseApprWaitNoti(bool useApprWaitNoti);
    }
    public class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        private ISGAppConfig _AppConfigInfo;
        public SGCtrlSideUIService(ref ISGAppConfig appConfigInfo)
        {
            _AppConfigInfo = appConfigInfo;
            SetLogLevel(AppConfigInfo.LogLevel);
        }

        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        public ref ISGAppConfig AppConfigInfo => ref _AppConfigInfo;
        public event Action OnChangeCtrlSide;
        private void NotifyStateChangedCtrlSide() => OnChangeCtrlSide?.Invoke();
        public void EmitNotifyStateChangedCtrlSide() => NotifyStateChangedCtrlSide();
        public void SaveAppConfigSerialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGAppConfig));
            string AppConfig = Environment.CurrentDirectory+"/wwwroot/conf/AppEnvSetting.json";
            try
            {
                using (var fs = new FileStream(AppConfig, FileMode.Create))
                {
                    var encoding = Encoding.UTF8;
                    var ownsStream = false;
                    var indent = true;
    
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                    {
                        serializer.WriteObject(writer, (_AppConfigInfo as SGAppConfig));
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error($"\nMessage ---\n{ex.Message}");
                Log.Error($"\nHelpLink ---\n{ex.HelpLink}");
                Log.Error($"\nStackTrace ---\n{ex.StackTrace}");
            }
        }
        public void SetClipBoardHotKey(int groupId, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode)
        {
            char cWin, cCtrl, cAlt, cShift;
            (AppConfigInfo as SGAppConfig).ClipBoardHotKey ??= new List<string>();
            cWin    = bWin?'Y':'N';
            cCtrl   = bCtrl?'Y':'N';
            cAlt    = bAlt?'Y':'N';
            cShift  = bShift?'Y':'N';
            if(AppConfigInfo.ClipBoardHotKey.ElementAtOrDefault(groupId) != null)
            {
                AppConfigInfo.ClipBoardHotKey.RemoveAt(groupId);
                AppConfigInfo.ClipBoardHotKey.Insert(groupId, String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}"));
            }
            else
            {
                AppConfigInfo.ClipBoardHotKey.Insert(groupId, String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}"));
            }

            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetClipBoardHotKeyNetOver(int groupId, int nIDx, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode)
        {
            char cWin, cCtrl, cAlt, cShift;
            //(AppConfigInfo as SGAppConfig).ClipBoardHotKey ??= new List<string>();
/*            if ((AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver == null)
            {
                (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver = new Dictionary<string, Dictionary<string, string>>();                
                // 추가 생성
            }*/

            cWin = bWin ? 'Y' : 'N';
            cCtrl = bCtrl ? 'Y' : 'N';
            cAlt = bAlt ? 'Y' : 'N';
            cShift = bShift ? 'Y' : 'N';

            string strHotKey = String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}");
            //(AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver.TryAdd(groupId, );

            if (AppConfigInfo.ClipBoardHotKey.ElementAtOrDefault(groupId) != null)
            {
                AppConfigInfo.ClipBoardHotKey.RemoveAt(groupId);
                //AppConfigInfo.ClipBoardHotKey.Insert(groupId, );
            }
            else
            {
                AppConfigInfo.ClipBoardHotKey.Insert(groupId, String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}"));
            }

            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetClipAlarmType(CLIPALM_TYPE alamType)
        {
            (AppConfigInfo as SGAppConfig).enClipAlarmType = alamType;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetClipAfterSend(bool clipAfterSend)
        {
            (AppConfigInfo as SGAppConfig).bClipAfterSend = clipAfterSend;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoTrans(bool urlAutoTrans)
        {
            (AppConfigInfo as SGAppConfig).bURLAutoTrans = urlAutoTrans;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoAfterMsg(bool urlAutoAfterMsg)
        {
            (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg = urlAutoAfterMsg;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoAfterBrowser(string urlAutoAfterBrowser)
        {
            (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser = urlAutoAfterBrowser;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans)
        {
            (AppConfigInfo as SGAppConfig).bRMouseFileAddAfterTrans = rmouseFileAddAfterTrans;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetAfterBasicChk(bool afterBasicChk)
        {
            (AppConfigInfo as SGAppConfig).bAfterBasicChk = afterBasicChk;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetRecvDownPath(int groupId, string recvDownPath)
        {
            (AppConfigInfo as SGAppConfig).RecvDownPath ??= new List<string>();
            if(AppConfigInfo.RecvDownPath.ElementAtOrDefault(groupId) != null)
            {
                AppConfigInfo.RecvDownPath.RemoveAt(groupId);
                AppConfigInfo.RecvDownPath.Insert(groupId, recvDownPath);
            }
            else
            {
                AppConfigInfo.RecvDownPath.Insert(groupId, recvDownPath);
            }

            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvFolderOpen(bool fileRecvFolderOpen)
        {
            (AppConfigInfo as SGAppConfig).bFileRecvFolderOpen = fileRecvFolderOpen;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetRecvDownPathChange(bool recvDownPathChange)
        {
            (AppConfigInfo as SGAppConfig).bRecvDownPathChange = recvDownPathChange;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetManualRecvDownChange(bool manualRecvDownChange)
        {
            (AppConfigInfo as SGAppConfig).bManualRecvDownChange = manualRecvDownChange;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvTrayFix(bool fileRecvTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bFileRecvTrayFix = fileRecvTrayFix;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetApprTrayFix(bool apprTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bApprTrayFix = apprTrayFix;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprActionTrayFix(bool userApprActionTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprActionTrayFix = userApprActionTrayFix;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprRejectTrayFix(bool userApprRejectTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprRejectTrayFix = userApprRejectTrayFix;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetExitTrayMove(bool exitTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bExitTrayMove = exitTrayMove;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetStartTrayMove(bool startTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bStartTrayMove = startTrayMove;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetStartProgramReg(bool startProgramReg)
        {
            (AppConfigInfo as SGAppConfig).bStartProgramReg = startProgramReg;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetLanguage(string language)
        {
            (AppConfigInfo as SGAppConfig).strLanguage = language;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetScreenLock(bool screenLock)
        {
            (AppConfigInfo as SGAppConfig).bScreenLock = screenLock;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetScreenTime(int screenTime)
        {
            (AppConfigInfo as SGAppConfig).tScreenTime = screenTime;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetLastUpdated(string lastUPdated)
        {
            (AppConfigInfo as SGAppConfig).LastUpdated = lastUPdated;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetSWVersion(string swVersion)
        {
            (AppConfigInfo as SGAppConfig).SWVersion = swVersion;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetSWCommitId(string swCommitId)
        {
            (AppConfigInfo as SGAppConfig).SWCommitId = swCommitId;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        private void ChangeLogLevel(LogEventLevel logLevel)
        {
            AgLog.LogLevelSwitch.MinimumLevel = logLevel;
            Log.Fatal($"Changed LogLevel to {logLevel.ToString()}");
        }
        public void SetLogLevel(LogEventLevel logLevel)
        {
            (AppConfigInfo as SGAppConfig).LogLevel = logLevel;
            ChangeLogLevel(logLevel);
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUseApprWaitNoti(bool useApprWaitNoti)
        {
            (AppConfigInfo as SGAppConfig).bUseApprWaitNoti = useApprWaitNoti;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
    }
}