using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using Microsoft.AspNetCore.Components.Web;
using OpenNetLinkApp.Models.SGConfig;
using OpenNetLinkApp.Services.SGAppManager;

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
    }
    internal class SGCtrlSideUIService : ISGCtrlSideUIService
    {
        private ISGAppConfig _AppConfigInfo;
        public SGCtrlSideUIService(ref ISGAppConfig appConfigInfo)
        {
            _AppConfigInfo = appConfigInfo;
        }

        /* To Manage ControlSide State */
        /// <summary>
        /// Application Environment Config Info.
        /// </summary>
        public ref ISGAppConfig AppConfigInfo => ref _AppConfigInfo;
        public event Action OnChangeCtrlSide;
        private void NotifyStateChangedCtrlSide() => OnChangeCtrlSide?.Invoke();
        public void SetClipBoardHotKey(int groupId, bool bWin, bool bCtrl, bool bAlt, bool bShift, char chVKCode)
        {
            char cWin, cCtrl, cAlt, cShift;
            (AppConfigInfo as SGAppConfig).ClipBoardHotKey ??= new List<string>();
            cWin    = bWin?'Y':'N';
            cCtrl   = bCtrl?'Y':'N';
            cAlt    = bAlt?'Y':'N';
            cShift  = bShift?'Y':'N';
            AppConfigInfo.ClipBoardHotKey.Insert(groupId, String.Format($"{cWin},{cCtrl},{cAlt},{cShift},{chVKCode}"));
            NotifyStateChangedCtrlSide();
        }
        public void SetClipAlarmType(CLIPALM_TYPE alamType)
        {
            (AppConfigInfo as SGAppConfig).enClipAlarmType = alamType;
            NotifyStateChangedCtrlSide();
        }
        public void SetClipAfterSend(bool clipAfterSend)
        {
            (AppConfigInfo as SGAppConfig).bClipAfterSend = clipAfterSend;
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoTrans(bool urlAutoTrans)
        {
            (AppConfigInfo as SGAppConfig).bURLAutoTrans = urlAutoTrans;
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoAfterMsg(bool urlAutoAfterMsg)
        {
            (AppConfigInfo as SGAppConfig).bURLAutoAfterMsg = urlAutoAfterMsg;
            NotifyStateChangedCtrlSide();
        }
        public void SetURLAutoAfterBrowser(string urlAutoAfterBrowser)
        {
            (AppConfigInfo as SGAppConfig).strURLAutoAfterBrowser = urlAutoAfterBrowser;
            NotifyStateChangedCtrlSide();
        }
        public void SetRMouseFileAddAfterTrans(bool rmouseFileAddAfterTrans)
        {
            (AppConfigInfo as SGAppConfig).bRMouseFileAddAfterTrans = rmouseFileAddAfterTrans;
            NotifyStateChangedCtrlSide();
        }
        public void SetAfterBasicChk(bool afterBasicChk)
        {
            (AppConfigInfo as SGAppConfig).bAfterBasicChk = afterBasicChk;
            NotifyStateChangedCtrlSide();
        }
        public void SetRecvDownPath(int groupId, string recvDownPath)
        {
            (AppConfigInfo as SGAppConfig).RecvDownPath ??= new List<string>();
            AppConfigInfo.RecvDownPath.Insert(groupId, recvDownPath);
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvFolderOpen(bool fileRecvFolderOpen)
        {
            (AppConfigInfo as SGAppConfig).bFileRecvFolderOpen = fileRecvFolderOpen;
            NotifyStateChangedCtrlSide();
        }
        public void SetRecvDownPathChange(bool recvDownPathChange)
        {
            (AppConfigInfo as SGAppConfig).bRecvDownPathChange = recvDownPathChange;
            NotifyStateChangedCtrlSide();
        }
        public void SetManualRecvDownChange(bool manualRecvDownChange)
        {
            (AppConfigInfo as SGAppConfig).bManualRecvDownChange = manualRecvDownChange;
            NotifyStateChangedCtrlSide();
        }
        public void SetFileRecvTrayFix(bool fileRecvTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bFileRecvTrayFix = fileRecvTrayFix;
            NotifyStateChangedCtrlSide();
        }
        public void SetApprTrayFix(bool apprTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bApprTrayFix = apprTrayFix;
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprActionTrayFix(bool userApprActionTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprActionTrayFix = userApprActionTrayFix;
            NotifyStateChangedCtrlSide();
        }
        public void SetUserApprRejectTrayFix(bool userApprRejectTrayFix)
        {
            (AppConfigInfo as SGAppConfig).bUserApprRejectTrayFix = userApprRejectTrayFix;
            NotifyStateChangedCtrlSide();
        }
        public void SetExitTrayMove(bool exitTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bExitTrayMove = exitTrayMove;
            NotifyStateChangedCtrlSide();
        }
        public void SetStartTrayMove(bool startTrayMove)
        {
            (AppConfigInfo as SGAppConfig).bStartTrayMove = startTrayMove;
            NotifyStateChangedCtrlSide();
        }
        public void SetStartProgramReg(bool startProgramReg)
        {
            (AppConfigInfo as SGAppConfig).bStartProgramReg = startProgramReg;
            NotifyStateChangedCtrlSide();
        }
        public void SetLanguage(string language)
        {
            (AppConfigInfo as SGAppConfig).strLanguage = language;
            NotifyStateChangedCtrlSide();
        }
        public void SetScreenLock(bool screenLock)
        {
            (AppConfigInfo as SGAppConfig).bScreenLock = screenLock;
            NotifyStateChangedCtrlSide();
        }
        public void SetScreenTime(int screenTime)
        {
            (AppConfigInfo as SGAppConfig).tScreenTime = screenTime;
            NotifyStateChangedCtrlSide();
        }
    }
}