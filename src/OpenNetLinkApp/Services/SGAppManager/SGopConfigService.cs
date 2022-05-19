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
    public interface ISGopConfigService
    {
        ref ISGopConfig AppConfigInfo { get; }

        PAGE_TYPE GetMainPageType();
        string GetMainPage(PAGE_TYPE enSiteMainPage, bool useDashBoard);
        bool GetClipAfterSend();
        bool GetURLAutoTrans(int nGroupID);
        bool GetURLAutoAfterMsg(int nGroupID);
        string GetURLAutoAfterBrowser(int nGroupID);

        string GetForwardUrl(int nGroupID);

        bool GetRMouseFileAddAfterTrans();
        bool GetAfterBasicChk();
        bool GetRecvDownPathChange();
        bool GetManualRecvDownChange();
        bool GetFileRecvTrayFix();
        bool GetApprTrayFix();
        bool GetUserApprActionTrayFix();
        bool GetUserApprRejectTrayFix();
        bool GetExitTrayMove();
        bool GetStartTrayMove();
        bool GetStartProgramReg();
        //bool GetUseScreenLock();
        bool GetScreenLock();
        bool GetScreenLockUserChange();
        bool GetUseApprWaitNoti();
        bool GetUseLogLevel();
        bool GetUseGPKILogin(int groupID);


        bool GetUseNetOverAllsend();
        bool GetFileDownloadBeforeReciving();
        //bool GetEmailManageApproveUse();

        bool GetShowAdminInfo();
        bool GetUseFileCheckException();

        bool GetUseAppLoginType();
        int GetAppLoginType();

    }
    internal class SGopConfigService : ISGopConfigService
    {
        private ISGopConfig _AppConfigInfo;
        public ref ISGopConfig AppConfigInfo => ref _AppConfigInfo;

        public SGopConfigService()
        {
            var serializer = new DataContractJsonSerializer(typeof(SGopConfig));
            string AppConfig = Environment.CurrentDirectory + "/wwwroot/conf/AppOPsetting.json";

            HsLogDel hsLog = new HsLogDel();
            hsLog.Delete(7);    // 7일이전 Log들 삭제

            Log.Information($"- AppOPsetting Path: [{AppConfig}]");
            if (File.Exists(AppConfig))
            {
                try
                {
                    Log.Information($"- AppOPsetting Loading... : [{AppConfig}]");
                    //Open the stream and read it back.
                    using (FileStream fs = File.OpenRead(AppConfig))
                    {
                        SGopConfig appConfig = (SGopConfig)serializer.ReadObject(fs);
                        _AppConfigInfo = appConfig;
                    }
                    Log.Information($"- AppOPsetting Load Completed : [{AppConfig}]");
                }
                catch (Exception ex)
                {
                    Log.Warning($"\nMessage ---\n{ex.Message}");
                    Log.Warning($"\nHelpLink ---\n{ex.HelpLink}");
                    Log.Warning($"\nStackTrace ---\n{ex.StackTrace}");
                    _AppConfigInfo = new SGopConfig();
                }
            }
            else
            {
                _AppConfigInfo = new SGopConfig();
            }
        }



        public PAGE_TYPE GetMainPageType()
        {
            return AppConfigInfo.enMainPageType;
        }
        public string GetMainPage(PAGE_TYPE enInitMainPage, bool useDashBoard)
        {
            string strPage = "/Welcome";
            PAGE_TYPE page;

            //사용자 선택이 NONE(초기값)이라면 프로그램에서 지정된 페이지로 설정
            page = (AppConfigInfo.enMainPageType == PAGE_TYPE.NONE) ? enInitMainPage : AppConfigInfo.enMainPageType;

            switch (page)
            {
                case PAGE_TYPE.NONE:
                case PAGE_TYPE.DASHBOARD:
                    strPage = useDashBoard ? "/Welcome" : "/Transfer";
                    break;

                case PAGE_TYPE.TRANSFER:
                    strPage = "/Transfer";
                    break;

                default:
                    strPage = "/Welcome";
                    break;
            }
            return strPage;
        }
        public bool GetClipAfterSend()
        {
            return AppConfigInfo.bClipCopyAutoSend;
        }
        public bool GetURLAutoTrans(int nGroupID)
        {
            //return AppConfigInfo.bURLAutoTrans;

            (AppConfigInfo as SGopConfig).bURLAutoTrans ??= new List<bool>();
            /*            (AppConfigInfo as SGopConfig).ClipBoardHotKeyNetOver ??= new Dictionary<string, Dictionary<string, string>>();
                        Dictionary<string, string> dicIdxHotKey = new Dictionary<string, string>();
                        if (dicIdxHotKey.TryAdd(nIdx.ToString(), "N,Y,N,Y,Z"))
                            AppConfigInfo.ClipBoardHotKeyNetOver.TryAdd(groupId.ToString(), dicIdxHotKey);
                        return AppConfigInfo.ClipBoardHotKeyNetOver[groupId.ToString()][nIdx.ToString()];*/

            if ((AppConfigInfo as SGopConfig).bURLAutoTrans.Count >= nGroupID + 1)
                return (AppConfigInfo as SGopConfig).bURLAutoTrans[nGroupID];

            return true;    // 기본값
        }

        public bool GetURLAutoAfterMsg(int nGroupID)
        {
            //return AppConfigInfo.bURLAutoAfterMsg;
            (AppConfigInfo as SGopConfig).bURLAutoAfterMsg ??= new List<bool>();

            if ((AppConfigInfo as SGopConfig).bURLAutoAfterMsg.Count >= nGroupID + 1)
                return (AppConfigInfo as SGopConfig).bURLAutoAfterMsg[nGroupID];

            return false;   // 기본값
        }

        public string GetURLAutoAfterBrowser(int nGroupID)
        {
            //return AppConfigInfo.strURLAutoAfterBrowser;
            (AppConfigInfo as SGopConfig).strURLAutoAfterBrowser ??= new List<string>();

            if ((AppConfigInfo as SGopConfig).strURLAutoAfterBrowser.Count >= nGroupID + 1)
                return (AppConfigInfo as SGopConfig).strURLAutoAfterBrowser[nGroupID];

            return "";
        }

        public string GetForwardUrl(int nGroupID)
        {
            //return AppConfigInfo.strForwardUrl;
            (AppConfigInfo as SGopConfig).strForwardUrl ??= new List<string>();

            if ((AppConfigInfo as SGopConfig).strForwardUrl.Count >= nGroupID + 1)
                return (AppConfigInfo as SGopConfig).strForwardUrl[nGroupID];

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
        public bool GetScreenLock()
        {
            return AppConfigInfo.bScreenLock;
        }
        public bool GetScreenLockUserChange()
        {
            return AppConfigInfo.bScreenLockUserChange;
        }
        public bool GetUseApprWaitNoti()
        {
            return AppConfigInfo.bUseApprWaitNoti;
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


        public bool GetUseNetOverAllsend()
        {
            return AppConfigInfo.bUseNetOverAllsend;
        }

        public bool GetFileDownloadBeforeReciving()
        {
            return AppConfigInfo.bFileDownloadBeforeReciving;
        }

        /*public bool GetEmailManageApproveUse()
        {
            return AppConfigInfo.bUseEmailManageApprove;
        }*/

        public bool GetShowAdminInfo()
        {
            return AppConfigInfo.bShowAdminInfo;
        }
        public bool GetUseFileCheckException()
        {
            return AppConfigInfo.bUseFileCheckException;
        }
        public bool GetUseAppLoginType()
        {
            return AppConfigInfo.bUseAppLoginType;
        }
        public int GetAppLoginType()
        {
            return AppConfigInfo.LoginType;
        }

    }
}