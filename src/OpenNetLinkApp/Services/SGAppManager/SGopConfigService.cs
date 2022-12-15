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

        /// <summary>
        /// 로그인후 첫 Page Url(입력값에 의해 다르게 나오게 됨)
        /// </summary>
        /// <param name="enSiteMainPage"></param>
        /// <param name="useDashBoard"></param>
        /// <returns></returns>
        string GetMainPage(PAGE_TYPE enSiteMainPage, bool useDashBoard);

        /// <summary>
        /// 로그인후 첫 Page Url(AppOPsetting.json의 enMainPageType 값으로 결정, 2:파일전송화면, 나머지:DashBoard)
        /// </summary>
        /// <returns></returns>
        public string GetMainPage();

        bool GetClipCopyAutoSend();
        bool GetURLAutoTrans(int nGroupID);
        bool GetURLAutoAfterMsg(int nGroupID);
        string GetURLAutoAfterBrowser(int nGroupID);

        string GetForwardUrl(int nGroupID);

        bool GetRMouseFileAddAfterTrans();
        bool GetAfterBasicChk();

        bool GetManualRecvDownChange();
        bool GetFileRecvTrayFix();
        bool GetApprTrayFix();
        bool GetUserApprActionTrayFix();
        bool GetUserApprRejectTrayFix();
        bool GetExitTrayMove();
        bool GetStartTrayMove();
        bool GetStartProgramReg();

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

        bool GetNoApproveManageUI();

        /// <summary>
        /// 빈파일 송신 가능하게 할지 유무
        /// </summary>
        /// <returns></returns>
        bool GetEmptyfileTrans();


        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public bool GetUseLoginIDSave(int groupID);
        public bool GetUseAutoLogin(int groupID);
        public bool GetUseAutoLoginCheck(int groupID);
        public bool GetUseApprLineLocalSave(int groupID);

        public bool GetUseApprLineChkBlock(int groupID);

        public bool GetUseApprDeptSearch(int groupID);
        public bool GetUseApprTreeSearch(int groupID);
        public bool GetUseUserPWChange(int groupID);
        public string GetPWChangeProhibitLimit(int groupID);
        public int GetPWChangeApplyCnt(int groupID);
        public string GetInitPasswordInfo(int groupID);

        /// <summary>
        /// 수신 폴더 사용자 변경 여부 가져오기
        /// </summary>
        /// <param name="groupID">그룹ID</param>
        /// <returns>true : 사용자 변경 가능  false : 사용자 변경 불가능</returns>
        public bool GetUseRecvFolderChange(int groupID);

        /// <summary>
        /// 로그인 유저별 다운로드 경로 사용 여부 가져오기
        /// </summary>
        /// <param name="groupID">그룹ID</param>
        /// <returns>true : 로그인 유저별 수신경로 사용, false : 로그인 유저별 수신경로 미사용</returns>
        public bool GetUseUserRecvDownPath(int groupID);

        public bool GetUseUserRecvDownPathUserName(int groupID);
        

        public bool GetUseEmailManageApprove(int groupID);

        public bool GetUsePCURL(int groupID);

        public bool GetUsePublicBoard(int groupID);
        public bool GetUseCertSend(int groupID);
        public bool GetUseClipAlarmTypeChange();
        public bool GetUseMainPageTypeChange();
        public bool GetUseClipCopyAndSend();
        public bool GetRFileAutoSend();
        public bool GetAfterApprAutoCheck();
        public bool GetRecvFolderOpen();
        public bool GetManualDownFolderChange();
        public bool GetFileRecvAlarmRetain();
        public bool GetApprCountAlarmRetain();
        public bool GetApprCompleteAlarmRetain();
        public bool GetApprRejectAlarmRetain();
        public bool GetUseApprCountAlaram();
        public bool GetUseCloseTrayMove();
        public bool GetUseStartTrayMove();
        public bool GetUseStartProgramReg();
        public bool GetUseLanguageSet();
        public bool GetViewFileFilter();
        public bool GetUseForceUpdate();
        public bool GetUseForceBackgroundUpdate();

        public bool GetViewSGSideBarUIBadge();

        public bool GetViewSGHeaderUIAlarmNoriAllDel();

        public bool GetViewDlpApproverMyDept();

        /// <summary>
        /// 클립보드 파일전송형태 전송때, 무조건 결재없이  전송되게 함.
        /// </summary>
        /// <returns></returns>
        public bool GetUseClipBoardNoApproveButFileTrans();

        /// <summary>
        /// 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로(개발중...), 1:사전, 2:사후 로 전송되게 적용
        /// </summary>
        /// <returns></returns>
        public int GetClipUseAfterApprove();

        /// <summary>
        /// 처음 접속 Server(Network) 를 사용자가 선택할 수 있도록 할건지 유무(Network.json 파일에 2개이상있어야 가능)
        /// </summary>
        /// <returns></returns>
        public bool GetUseSelectFirstConnectNetServer();

        /// <summary>
        /// 3망전송기능에서 한번에 모든 망에 자료를 송신하는 기능 사용 유무
        /// </summary>
        /// <returns></returns>
        //public bool GetUseNetOverAllsend();

        /// <summary>
        /// 파일포워드 기능 사용할 것인지 유무
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileForward();


        /// <summary>
        /// PassWord 걸린 ZIP 파일에 대해 파일추가 거부하게 할지 유무(true:거부)
        /// </summary>
        /// <returns></returns>
        public bool GetUseDenyPasswordZip();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseFileClipManageUI(int groupID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseFileClipApproveUI(int groupID);

        /// <summary>
        /// ClipBoard를 파일전송 Type으로 전송
        /// </summary>
        /// <returns></returns>
        public bool GetUseClipBoardFileTrans(int groupID);

        /// <summary>
        /// 1번에 모든망 로그인
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>

        public bool GetUseOneToMultiLogin();

        /// <summary>
        /// 1번에 모든망 로그인하되 로그아웃은 선택한 망 1개만 진행여부
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool GetUseOneToMultiLoginButoneBYoneLogout();

        public bool GetUseOneAClockChangeAgentTimer();

        bool GetUseFileExceptionDescCheck();

        /// <summary>
        /// '파일전송' 화면에서 등록시도한 파일목록에 정상파일과 오류파일이 함께 존재할 시 정상 파일에 대한 부분 등록 가능여부(true, false)
        /// </summary>
        /// <returns></returns>
        public bool GetUsePartialFileAddInTransfer();


        /// <summary>
        /// 알람 초기화 매일 자정마다
        /// </summary>
        /// <returns></returns>
        public bool GetUseInitAlarmPerDay();
        /// <summary>
        /// 메세지 초기화 매일 자정마다
        /// </summary>
        /// <returns></returns>
        public bool GetUseInitMessagePerDay();
        /// <summary>
        /// 정책 수동 업데이트 버튼 보여주는 여부
        /// </summary>
        /// <returns></returns>
        public bool GetVisiblePolicyUpdateButton();
    }


    internal class SGopConfigService : ISGopConfigService
    {
        private ISGopConfig _AppConfigInfo;
        /// <summary>
        /// AppOPsetting
        /// </summary>
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

        public string GetMainPage()
        {
            string strPage = "/Welcome";
            PAGE_TYPE page = AppConfigInfo.enMainPageType;
            switch (page)
            {
                case PAGE_TYPE.NONE:
                case PAGE_TYPE.DASHBOARD:
                    strPage = "/Welcome";
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

        public bool GetClipCopyAutoSend()
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

        public bool GetUseEmailManageApprove(int nGroupID)
        {
            (AppConfigInfo as SGopConfig).blistUseEmail ??= new List<bool>();

            if ((AppConfigInfo as SGopConfig).blistUseEmail.Count >= nGroupID + 1)
                return (AppConfigInfo as SGopConfig).blistUseEmail[nGroupID];

            return false;    // 기본값
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
            if (AppConfigInfo.listUseGpkiLogin == null)
                return false;

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

        public bool GetNoApproveManageUI()
        {
            return AppConfigInfo.bNoApproveManageUI;
        }

        public bool GetEmptyfileTrans()
        {
            return AppConfigInfo.bEmptyfileTrans;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // siteConfig Code Level에서 동작하던거 AppOPsetting.json 으로 옮겨 온거

        public bool GetUseLoginIDSave(int groupID)
        {
            return AppConfigInfo.bUserIDSave;
        }
        public bool GetUseAutoLogin(int groupID)
        {
            return AppConfigInfo.bAutoLogin;
        }
        public bool GetUseAutoLoginCheck(int groupID)
        {
            return AppConfigInfo.bAutoLoginCheck;
        }

        public bool GetUseApprLineLocalSave(int groupID)
        {
            return AppConfigInfo.bApprLineLocalSave;
        }


        public bool GetUseTitleDescSameCharCheck(int groupID)
        {
            return AppConfigInfo.bTitleDescSameChk;
        }

        public bool GetUseApprLineChkBlock(int groupID)
        {
            return AppConfigInfo.bApprLineChkBlock;
        }

        public bool GetUseApprDeptSearch(int groupID)
        {
            return AppConfigInfo.bApprDeptSearch;
        }
        public bool GetUseApprTreeSearch(int groupID)
        {

            return false;
        }

        public bool GetUseUserPWChange(int groupID)
        {
            return AppConfigInfo.bUserPWChange;
        }

        public string GetPWChangeProhibitLimit(int groupID)
        {
            return AppConfigInfo.strPWChangeProhibitLimit;
        }

        public int GetPWChangeApplyCnt(int groupID)
        {
            return AppConfigInfo.nPWChangeApplyCnt;
        }

        public string GetInitPasswordInfo(int groupID)
        {
            return AppConfigInfo.strInitPasswd;
        }

        public bool GetUseRecvFolderChange(int groupID)
        {
            return AppConfigInfo.bRecvFolderChange;
        }
        public bool GetUseUserRecvDownPath(int groupID)
        {
            return AppConfigInfo.bUseUserRecvDownPath;
        }

        public bool GetUseUserRecvDownPathUserName(int groupID)
        {
            return AppConfigInfo.bUseUserRecvDownPathUserName;
        }

        public bool GetUsePCURL(int groupID)
        {
            return AppConfigInfo.bUsePCURL;
        }

        public bool GetUsePublicBoard(int groupID)
        {
            return AppConfigInfo.bUsePublicBoard;
        }

        public bool GetUseCertSend(int groupID)
        {
            return AppConfigInfo.bUseCertSend;
        }


        public bool GetUseClipCopyAndSend()
        {
            return AppConfigInfo.bUseClipCopyAndSend;
        }
        public bool GetRFileAutoSend()
        {
            return AppConfigInfo.bRFileAutoSend;
        }
        public bool GetAfterApprAutoCheck()
        {
            return AppConfigInfo.bShowAfterApprAutoCheck;
        }
        public bool GetRecvFolderOpen()
        {
            return AppConfigInfo.bRecvFolderOpen;
        }
        public bool GetManualDownFolderChange()
        {
            return AppConfigInfo.bManualDownFolderChange;
        }
        public bool GetFileRecvAlarmRetain()
        {
            return AppConfigInfo.bFileRecvAlarmRetain;
        }
        public bool GetApprCountAlarmRetain()
        {
            return AppConfigInfo.bApprCountAlarmRetain;
        }
        public bool GetApprCompleteAlarmRetain()
        {
            return AppConfigInfo.bApprCompleteAlarmRetain;
        }
        public bool GetApprRejectAlarmRetain()
        {
            return AppConfigInfo.bApprRejectAlarmRetain;
        }
        public bool GetUseApprCountAlaram()
        {
            return AppConfigInfo.bUseApprCountAlaram;
        }
        public bool GetUseCloseTrayMove()
        {
            return AppConfigInfo.bUseCloseTrayMove;
        }
        public bool GetUseStartTrayMove()
        {
            return AppConfigInfo.bUseStartTrayMove;
        }
        public bool GetUseStartProgramReg()
        {
            return AppConfigInfo.bUseStartProgramReg;
        }
        public bool GetUseLanguageSet()
        {
            return AppConfigInfo.bUseLanguageSet;
        }

        public bool GetUseMainPageTypeChange()
        {
            return AppConfigInfo.bUseMainPageType;
        }
        public bool GetViewFileFilter()
        {
            return AppConfigInfo.bViewFileFilter;
        }

        public bool GetViewSGSideBarUIBadge()
        {
            return AppConfigInfo.bViewSGSideBarUIBadge;
        }

        public bool GetViewSGHeaderUIAlarmNoriAllDel()
        {
            return AppConfigInfo.bViewSGHeaderUIAlarmNoriAllDel;
        }
        public bool GetUseForceUpdate()
        {
            return AppConfigInfo.bUseForceUpdate;
        }

        public bool GetUseForceBackgroundUpdate()
        {
            return AppConfigInfo.bUseForceBackgroundUpdate;
        }

        public bool GetViewDlpApproverMyDept()
        {
            return AppConfigInfo.bViewDlpApproverSelectMyDept;
        }
        public bool GetUseClipBoardNoApproveButFileTrans()
        {
            return AppConfigInfo.bClipBoardNoApproveButFileTrans;
        }
        public int GetClipUseAfterApprove()
        {
            return AppConfigInfo.nClipAfterApproveUseType;
        }

        public bool GetUseSelectFirstConnectNetServer()
        {
            return AppConfigInfo.bUseUserSelectFirstServer;
        }

        public bool GetUseFileForward()
        {
            return AppConfigInfo.bUseFileForward;
        }


        public bool GetUseDenyPasswordZip()
        {
            return AppConfigInfo.bUseDenyPasswordZip;
        }

        public bool GetUseFileClipManageUI(int groupID)
        {
            return AppConfigInfo.bUseFileClipManageUI;
        }

        public bool GetUseFileClipApproveUI(int groupID)
        {
            return AppConfigInfo.bUseFileClipApproveUI;
        }

        public bool GetUseClipBoardFileTrans(int groupID)
        {
            return AppConfigInfo.bUseClipBoardFileTrans;
        }

        public bool GetUseClipAlarmTypeChange()
        {
            return AppConfigInfo.bUseClipAlarmType;
        }

        public bool GetUseOneToMultiLogin()
        {
            return AppConfigInfo.bUseOneToMultiLogin;
        }

        public bool GetUseOneAClockChangeAgentTimer()
        {
            return AppConfigInfo.bUseAgentTime1aClock;
        }

        public bool GetUseFileExceptionDescCheck()
        {
            return AppConfigInfo.bUseFileExceptionDescCheck;
        }

        public bool GetUseOneToMultiLoginButoneBYoneLogout()
        {
            return AppConfigInfo.bUseOneByOneLogOut;
        }

        public bool GetUsePartialFileAddInTransfer()
        {
            return AppConfigInfo.bUsePartialFileAddInTransfer;
        }


        public bool GetUseInitAlarmPerDay()
        {
            return AppConfigInfo.bUseInitAlarmPerDay;
        }

        public bool GetUseInitMessagePerDay()
        {
            return AppConfigInfo.bUseInitMessagePerDay;
        }

        public bool GetVisiblePolicyUpdateButton()
        {
            return AppConfigInfo.bVisiblePolicyUpdateButton;
        }
    }
}