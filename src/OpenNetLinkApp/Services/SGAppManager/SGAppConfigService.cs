using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;

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
    }
    internal class SGAppConfigService : ISGAppConfigService
    {
        private ISGAppConfig _AppConfigInfo;
        public ref ISGAppConfig AppConfigInfo => ref _AppConfigInfo;
        public SGAppConfigService()
        {
            _AppConfigInfo = new SGAppConfig();
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
            (AppConfigInfo as SGAppConfig).RecvDownPath ??= new List<string>(){"",""};
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
    }
}