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

        void SaveOpConfigSerialize();

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
        void SetScreenLock(bool screenLock);
        void SetScreenTime(int screenTime);
        void SetLastUpdated(string lastUPdated);
        void SetSWVersion(string swVersion);
        void SetSWCommitId(string swCommitId);
        void SetLogLevel(LogEventLevel logLevel);
        void SetUseApprWaitNoti(bool useApprWaitNoti);

        void SetUserSelectFirstNet(int nSelectNet);
    }
    public class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        private ISGAppConfig _AppConfigInfo;
        private Dictionary<int , ISGopConfig> _OpConfigInfo;
        private ISGVersionConfig _VersionConfigInfo;
        private List<ISGNetwork> _NetWorkInfo;
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGCtrlSideUIService>();
        public SGCtrlSideUIService(ref ISGAppConfig appConfigInfo, ref Dictionary<int , ISGopConfig> opConfigInfo, ref ISGVersionConfig verConfigInfo, List<ISGNetwork> netWorkInfo)
        {
            _AppConfigInfo = appConfigInfo;
            _OpConfigInfo = opConfigInfo;
            _VersionConfigInfo = verConfigInfo;
            _NetWorkInfo = netWorkInfo;
            SetLogLevel(AppConfigInfo.LogLevel);
        }

        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        public ref ISGAppConfig AppConfigInfo => ref _AppConfigInfo;

        public ref Dictionary<int, ISGopConfig> OpConfigInfo => ref _OpConfigInfo;

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
                CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
            }
        }

        public void SaveOpConfigSerialize()
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                var serializer = new DataContractJsonSerializer(typeof(SGopConfig));
                string AppConfig = Environment.CurrentDirectory + $"/wwwroot/conf/AppOPsetting_{sGNetwork.GroupID}_{sGNetwork.NetPos}.json";
                try
                {
                    using (var fs = new FileStream(AppConfig, FileMode.Create))
                    {
                        var encoding = Encoding.UTF8;
                        var ownsStream = false;
                        var indent = true;

                        using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                        {
                            serializer.WriteObject(writer, _OpConfigInfo[sGNetwork.GroupID]);
                        }
                    }

                    byte[] info = null;
                    using (FileStream fileStream = new FileStream(AppConfig, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            string str = streamReader.ReadToEnd();
                            byte[] byteInput = Encoding.UTF8.GetBytes(str);
                            byte[] masterKey = SGCrypto.GetMasterKey();
                            info = SGCrypto.AESEncrypt256(byteInput, masterKey);
                        }
                    }

                    using (FileStream fs = File.Create(AppConfig))
                    {
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (Exception ex)
                {
                    CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
                }
            }
        }
        public void SaveOpConfigSerialize(int groupId)
        {
            var serializer = new DataContractJsonSerializer(typeof(SGopConfig));
            string AppConfig = String.Empty;
            foreach(ISGNetwork sGNetwork in NetWorkInfo)
            {
                if(sGNetwork.GroupID == groupId)
                    AppConfig = Environment.CurrentDirectory + $"/wwwroot/conf/AppOPsetting_{groupId}_{sGNetwork.NetPos}.json";
            }

            if(AppConfig == String.Empty)
            {
                return;
            }
            try
            {
                using (var fs = new FileStream(AppConfig, FileMode.Create))
                {
                    var encoding = Encoding.UTF8;
                    var ownsStream = false;
                    var indent = true;

                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                    {
                        serializer.WriteObject(writer, _OpConfigInfo[groupId]);
                    }
                }

                byte[] info = null;
                using (FileStream fileStream = new FileStream(AppConfig, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string str = streamReader.ReadToEnd();
                        byte[] byteInput = Encoding.UTF8.GetBytes(str);
                        byte[] masterKey = SGCrypto.GetMasterKey();
                        info = SGCrypto.AESEncrypt256(byteInput, masterKey);
                    }
                }

                using (FileStream fs = File.Create(AppConfig))
                {
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
            }
        }

        public void SaveVersionConfigSerialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGVersionConfig));
            string VersionConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppVersion.json";
            try
            {
                using (var fs = new FileStream(VersionConfig, FileMode.Create))
                {
                    var encoding = Encoding.UTF8;
                    var ownsStream = false;
                    var indent = true;

                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                    {
                        serializer.WriteObject(writer, (_VersionConfigInfo as SGVersionConfig));
                    }
                }
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
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].enMainPageType = pageType;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetClipAfterSend(bool clipAfterSend)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bClipCopyAutoSend = clipAfterSend;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoTrans(int nGroupID, bool urlAutoTrans)
        {
            try
            {
                OpConfigInfo[nGroupID].bURLAutoTrans = urlAutoTrans;
            }
            catch(Exception e)
            {
                CLog.Here().Information($"FindSubMenu-Exception(Msg) : {e.Message}");
            }

            SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoAfterMsg(int nGroupID, bool urlAutoAfterMsg)
        {
            try
            {
                OpConfigInfo[nGroupID].bURLAutoAfterMsg = urlAutoAfterMsg;
            }
            catch (Exception e)
            {
                //e.Message;
                string strMsg = e.Message;
            }

            SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetURLAutoAfterBrowser(int nGroupID, string urlAutoAfterBrowser)
        {
            OpConfigInfo[nGroupID].strURLAutoAfterBrowser = urlAutoAfterBrowser;

            SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetForwardUrl(int nGroupID, string urlData)
        {
            OpConfigInfo[nGroupID].strForwardUrl = urlData;

            SaveOpConfigSerialize(nGroupID);
            NotifyStateChangedCtrlSide();
        }

        public void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans)
        {
            foreach(ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bRMouseFileAddAfterTrans = rmouseFileAddAfterTrans;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetAfterBasicChk(bool afterBasicChk)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bAfterBasicChk = afterBasicChk;
            }
            
            SaveOpConfigSerialize();
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
        public void SetManualRecvDownChange(bool manualRecvDownChange)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bManualRecvDownChange = manualRecvDownChange;
            }
            
            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvTrayFix(bool fileRecvTrayFix)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bFileRecvTrayFix = fileRecvTrayFix;
            }
            
            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetApprTrayFix(bool apprTrayFix)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bApprTrayFix = apprTrayFix;
            }
            
            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprActionTrayFix(bool userApprActionTrayFix)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bUserApprActionTrayFix = userApprActionTrayFix;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprRejectTrayFix(bool userApprRejectTrayFix)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bUserApprRejectTrayFix = userApprRejectTrayFix;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetExitTrayMove(bool exitTrayMove)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bExitTrayMove = exitTrayMove;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetStartTrayMove(bool startTrayMove)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bStartTrayMove = startTrayMove;
            }

            SaveOpConfigSerialize();
            NotifyStateChangedCtrlSide();
        }


        public void GetStartProgramReg()
        {

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
        public void SetScreenLock(bool screenLock)
        {
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bUseScreenLock = screenLock;
            }

            SaveOpConfigSerialize();
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
            (VersionConfigInfo as SGVersionConfig).LastUpdated = lastUPdated;
            SaveVersionConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetSWVersion(string swVersion)
        {
            (VersionConfigInfo as SGVersionConfig).SWVersion = swVersion;
            SaveVersionConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
        public void SetSWCommitId(string swCommitId)
        {
            (VersionConfigInfo as SGVersionConfig).SWCommitId = swCommitId;
            SaveVersionConfigSerialize();
            NotifyStateChangedCtrlSide();
        }
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
            foreach (ISGNetwork sGNetwork in NetWorkInfo)
            {
                OpConfigInfo[sGNetwork.GroupID].bUseApprWaitNoti = useApprWaitNoti;
            }

            SaveOpConfigSerialize();
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