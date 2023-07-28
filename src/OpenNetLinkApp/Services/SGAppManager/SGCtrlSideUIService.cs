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
using System.Runtime.InteropServices;
using OpenNetLinkApp.Common;
using OpenNetLinkApp.Models.SGNetwork;
using HsNetWorkSG;
using System.Text.Json;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGCtrlSideUIService
    {
        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        ISGAppConfig AppConfigInfo { get; }
        /// <summary>
        /// Control Right SideBar Menu Delegate, Related App Environment Setting.
        /// </summary>
        event Action OnChangeCtrlSide;
        void EmitNotifyStateChangedCtrlSide();
        void SaveAppConfigSerialize();

        //void SaveOpConfigSerialize();

        //void SaveOpConfigSerialize(int groupId);

        void SetClipBoardHotKey(int groupId, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode);

        void SetClipBoardHotKeyNetOver(int groupId, int nIDx, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode);

        void SetClipAlarmType(CLIPALM_TYPE alamType);
        void SetMainPage(PAGE_TYPE pageType);
        void SetClipAfterSend(bool clipAfterSend);
        void SetURLAutoTrans(int nGroupID, bool urlAutoTrans);
        void SetURLAutoAfterMsg(int nGroupID, bool urlAutoAfterMsg);
        void SetURLAutoAfterBrowser(int nGroupID, string urlAutoAfterBrowser);

        void SetForwardUrl(int nGroupID, string urlData);

        void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans);
        void SetAfterBasicChk(bool afterBasicChk);
        void SetRecvDownPath(int groupId, string recvDownPath);
        void SetFileRecvFolderOpen(bool fileRecvFolderOpen);
        void SetManualRecvDownChange(bool manualRecvDownChange);
        void SetFileRecvTrayFix(bool fileRecvTrayFix);
        void SetApprTrayFix(bool apprTrayFix);
        void SetUserApprActionTrayFix(bool userApprActionTrayFix);
        void SetUserApprRejectTrayFix(bool userApprRejectTrayFix);
        void SetExitTrayMove(bool exitTrayMove);
        void SetStartTrayMove(bool startTrayMove);
        void SetStartProgramReg(bool startProgramReg);
        void SetLanguage(string language);
        //void SetScreenLock(bool screenLock);
        //void SetScreenTime(int screenTime);
        //void SetLastUpdated(string lastUPdated);
        //void SetSWVersion(string swVersion);
        //void SetSWCommitId(string swCommitId);
        void SetLogLevel(LogEventLevel logLevel);
        void SetUseApprWaitNoti(bool useApprWaitNoti);

        void SetUserSelectFirstNet(int nSelectNet);
    }
    public class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        //private ISGAppConfig _AppConfigInfo;
        //private Dictionary<int, ISGopConfig> _OpConfigInfo;
        private ISGVersionConfig _VersionConfigInfo;
        private List<ISGNetwork> _NetWorkInfo;
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGCtrlSideUIService>();
        public SGCtrlSideUIService(ref ISGVersionConfig verConfigInfo, List<ISGNetwork> netWorkInfo)
        {
            //_AppConfigInfo = appConfigInfo;
            //_OpConfigInfo = opConfigInfo;
            _VersionConfigInfo = verConfigInfo;
            _NetWorkInfo = netWorkInfo;
            SetLogLevel(AppConfigInfo.LogLevel);
        }

        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        public ISGAppConfig AppConfigInfo => SGAppConfigService.AppConfigInfo;

        public Dictionary<int, ISGopConfig> OpConfigInfo => SGopConfigService.AppConfigInfo;

        public ref ISGVersionConfig VersionConfigInfo => ref _VersionConfigInfo;

        public List<ISGNetwork> NetWorkInfo
        {
            get
            {
                return _NetWorkInfo;
            }
        }

        public event Action OnChangeCtrlSide;
        private void NotifyStateChangedCtrlSide() => OnChangeCtrlSide?.Invoke();
        public void EmitNotifyStateChangedCtrlSide() => NotifyStateChangedCtrlSide();
        public void SaveAppConfigSerialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGAppConfig));
            string AppConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppEnvSetting.json";
            try
            {
                //DEK로 암호화하여 재저장
                var opt = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                byte[] oriContents = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(this, opt);
                byte[] encContents= new byte[0];
                SGCrypto.AESEncrypt256WithDEK(oriContents, ref encContents);
                File.WriteAllBytes(AppConfig, encContents);

                //using (var fs = new FileStream(AppConfig, FileMode.Create))
                //{
                //    var encoding = Encoding.UTF8;
                //    var ownsStream = false;
                //    var indent = true;

                //    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                //    {
                //        serializer.WriteObject(writer, (_AppConfigInfo as SGAppConfig));
                //    }
                //}
            }
            catch (Exception ex)
            {
                CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
            }
        }
        
        public void SetClipBoardHotKey(int groupId, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode)
        {
            char cWin, cCtrl, cAlt, cShift;
            (AppConfigInfo as SGAppConfig).ClipBoardHotKey ??= new List<string>();
            cWin = bWin ? 'Y' : 'N';
            cCtrl = bCtrl ? 'Y' : 'N';
            cAlt = bAlt ? 'Y' : 'N';
            cShift = bShift ? 'Y' : 'N';
            if (AppConfigInfo.ClipBoardHotKey.ElementAtOrDefault(groupId) != null)
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

            if (nIDx == 1)
                return;

            char cWin, cCtrl, cAlt, cShift;
            cWin = bWin ? 'Y' : 'N';
            cCtrl = bCtrl ? 'Y' : 'N';
            cAlt = bAlt ? 'Y' : 'N';
            cShift = bShift ? 'Y' : 'N';

            string strHotKey = String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}");
            string strGroupIDidx = String.Format($"{groupId}-{nIDx}");

            if ((AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver == null)
                (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver = new Dictionary<string, string>();

            bool bAdded = false;

            if ((AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver != null)
                bAdded = (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver.TryAdd(strGroupIDidx, strHotKey);

            if (bAdded == false)
                (AppConfigInfo as SGAppConfig).ClipBoardHotKeyNetOver[strGroupIDidx] = strHotKey;

            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetClipAlarmType(CLIPALM_TYPE alamType)
        {
            (AppConfigInfo as SGAppConfig).enClipAlarmType = alamType;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetMainPage(PAGE_TYPE pageType)
        {
            (AppConfigInfo as SGAppConfig).enMainPageType = pageType;

            //SaveOpConfigSerialize();
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetClipAfterSend(bool clipAfterSend)
        {
            (AppConfigInfo as SGAppConfig).bClipCopyAutoSend = clipAfterSend;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoTrans(int nGroupID, bool urlAutoTrans)
        {
            try
            {
                (AppConfigInfo as SGAppConfig).bURLAutoTrans ??= new List<bool>();
                if (AppConfigInfo.bURLAutoTrans.Count >= nGroupID + 1)
                    (AppConfigInfo as SGAppConfig).bURLAutoTrans[nGroupID] = urlAutoTrans;
            }
            catch (Exception e)
            {
                CLog.Here().Information($"SGCtrlSideUIService-Exception(Msg) : {e.Message}");
            }

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoAfterMsg(int nGroupID, bool urlAutoAfterMsg)
        {
            try
            {
                (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg ??= new List<bool>();
                if (AppConfigInfo.bURLAutoAfterMsg.Count >= nGroupID + 1)
                    (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg[nGroupID] = urlAutoAfterMsg;

            }
            catch (Exception e)
            {
                //e.Message;
                CLog.Here().Information($"SGCtrlSideUIService-Exception(Msg) : {e.Message}");
            }
            SaveAppConfigSerialize();
            //SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoAfterBrowser(int nGroupID, string urlAutoAfterBrowser)
        {
            (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser ??= new List<string>();
            if (AppConfigInfo.strURLAutoAfterBrowser.Count >= nGroupID + 1)
                (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser[nGroupID] = urlAutoAfterBrowser;


            SaveAppConfigSerialize();
            //SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetForwardUrl(int nGroupID, string urlData)
        {
            (AppConfigInfo as SGAppConfig).strForwardUrl ??= new List<string>();
            if (AppConfigInfo.strForwardUrl.Count >= nGroupID + 1)
                (AppConfigInfo as SGAppConfig).strForwardUrl[nGroupID] = urlData;


            SaveAppConfigSerialize();
            //SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans)
        {
            (AppConfigInfo as SGAppConfig).bRMouseFileAddAfterTrans = rmouseFileAddAfterTrans;

            SaveAppConfigSerialize();

            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetAfterBasicChk(bool afterBasicChk)
        {
            (AppConfigInfo as SGAppConfig).bAfterBasicChk = afterBasicChk;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetRecvDownPath(int groupId, string recvDownPath)
        {
            (AppConfigInfo as SGAppConfig).RecvDownPath ??= new List<string>();
            if (AppConfigInfo.RecvDownPath.ElementAtOrDefault(groupId) != null)
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
        public void SetManualRecvDownChange(bool manualRecvDownChange)
        {
            (AppConfigInfo as SGAppConfig).bManualRecvDownChange = manualRecvDownChange;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvTrayFix(bool fileRecvTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bFileRecvTrayFix = fileRecvTrayFix;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetApprTrayFix(bool apprTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bApprTrayFix = apprTrayFix;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprActionTrayFix(bool userApprActionTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprActionTrayFix = userApprActionTrayFix;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprRejectTrayFix(bool userApprRejectTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprRejectTrayFix = userApprRejectTrayFix;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetExitTrayMove(bool exitTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bExitTrayMove = exitTrayMove;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetStartTrayMove(bool startTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bStartTrayMove = startTrayMove;

            SaveAppConfigSerialize();
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        /// <summary>
        /// Booting때 실행되도록 등록<br></br>
        /// startProgramReg : true(등록), false(삭제)
        /// </summary>
        /// <param name="startProgramReg"></param>
        public void SetStartProgramReg(bool startProgramReg)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string strAgentPath = CsSystemFunc.GetCurrentProcessName();
                CsSystemFunc.makeAgentBootStartOSwindow(startProgramReg, false, strAgentPath, "OpenNetLink.lnk"); // startProgramReg
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

            }
            else
            {
                CLog.Here().Information($"makeAgentBootStart - UnSupported OS Type - OSDescription : {RuntimeInformation.OSDescription}, OSArchitecture : {RuntimeInformation.OSArchitecture}");
            }

            // 일단 설정값을 운영하는 방식으로 적용
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
        //public void SetScreenLock(bool screenLock)
        //{
        //    (AppConfigInfo as SGAppConfig).bUseScreenLock = screenLock;

        //    SaveAppConfigSerialize();
        //    //SaveOpConfigSerialize();
        //    NotifyStateChangedCtrlSide();
        //}
        //public void SetScreenTime(int screenTime)
        //{
        //    (AppConfigInfo as SGAppConfig).tScreenTime = screenTime;
        //    SaveAppConfigSerialize();
        //    NotifyStateChangedCtrlSide();
        //}
        //public void SetLastUpdated(string lastUPdated)
        //{
        //    (VersionConfigInfo as SGVersionConfig).LastUpdated = lastUPdated;
        //    SaveVersionConfigSerialize();
        //    NotifyStateChangedCtrlSide();
        //}
        //public void SetSWVersion(string swVersion)
        //{
        //    (VersionConfigInfo as SGVersionConfig).SWVersion = swVersion;
        //    SaveVersionConfigSerialize();
        //    NotifyStateChangedCtrlSide();
        //}
        //public void SetSWCommitId(string swCommitId)
        //{
        //    (VersionConfigInfo as SGVersionConfig).SWCommitId = swCommitId;
        //    SaveVersionConfigSerialize();
        //    NotifyStateChangedCtrlSide();
        //}
        private void ChangeLogLevel(LogEventLevel logLevel)
        {
            AgLog.LogLevelSwitch.MinimumLevel = logLevel;
            CLog.Here().Fatal($"Changed LogLevel to {logLevel.ToString()}");
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
            //SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetUserSelectFirstNet(int nSelectNet)
        {
            (AppConfigInfo as SGAppConfig).nUserSelectFirstNet = nSelectNet;
            SaveAppConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

    }
}