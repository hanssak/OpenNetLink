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
using OpenNetLinkApp.Models.SGNetwork;
using System.Text.Json;
using System.Security.Cryptography;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGopConfigService
    {
        //ref Dictionary<int, ISGopConfig> AppConfigInfo { get; }
        Dictionary<int, ISGopConfig> AppConfigInfo { get { return GetSGopConfigService(); } }
        Dictionary<int, ISGopConfig> GetSGopConfigService();

        public bool GetPocMode(int groupId);

        public bool GetUseAppLoginType(int groupId);

        public int GetAppLoginType(int groupId);
        public int GetCustomLoginType(int groupId);

        public string GetCustomLoginHttpUrl(int groupId);
        public string GetCustomLoginSecurityKey(int groupId);

        public string GetCustomLoginSecurityIV(int groupId);

        public bool GetUseGPKILogin(int groupId);

        public int GetNACLoginType(int groupId);

        public string GetNACLoginEncryptKey(int groupId);

        public bool GetUseLoginIDSave(int groupId);

        public bool GetUseLoginIDSaveCheck(int groupId);

        public bool GetUseAutoLogin(int groupId);

        public bool GetUseAutoLoginCheck(int groupId);

        public bool GetUseSelectFirstConnectNetServer(int groupId);

        public bool GetUseOneToMultiLogin(int groupId);

        public bool GetUseOneToMultiLoginButoneBYoneLogout(int groupId);

        public bool GetUseOver1auth(int groupId);

        public bool GetUseUserPWChange(int groupId);

        public bool GetUseGoogleOtp2FactorAuth(int groupId);

        public string GetPWChangeProhibitLimit(int groupId);

        public int GetPWChangeApplyCnt(int groupId);

        public string GetInitPasswordInfo(int groupId);

        public bool GetUseIDAsInitPassword(int groupId);

        public bool GetRFileAutoSend(int groupId);

        //public bool GetRMouseFileAddAfterTrans(int groupId);

        public bool GetUseNetOverAllsend(int groupId);

        public bool GetFileDownloadBeforeReciving(int groupId);

        public bool GetNoApproveManageUI(int groupId);
        public bool GetUseApproveManageUIForce(int groupId);

        public bool GetEmptyfileTrans(int groupId);

        public bool GetUseTitleDescSameCharCheck(int groupId);

        public bool GetUseApprLineChkBlock(int groupId);
        //public bool GetRecvFolderOpen(int groupId);
        public bool GetUseRecvFolderChange(int groupId);
        public bool GetManualDownFolderChange(int groupId);
        //public bool GetManualRecvDownChange(int groupId);
        public bool GetUseUserRecvDownPath(int groupId);
        public bool GetUseDenyPasswordZip(int groupId);
        public bool GetUseFileForward(int groupId);
        public bool GetFileForward(int groupId);
        public bool GetUsePartialFileAddInTransfer(int groupId);
        public bool GetUseCheckHardSpace(int groupId);
        public bool GetUseFileApproveReason(int groupId);
        public bool GetUseClipBoardApproveReason(int groupId);
        public bool GetUseFileSelectDelete(int groupId);
        public bool GetUseFileAllDelete(int groupId);
        public bool GetUseCrossPlatformOSforFileName(int groupId);
        public bool GetUseMinLengthTitleDesc(int groupId);
        public bool GetUseMaxLengthTitleDesc(int groupId);
        public bool GetUseAgentBlockValueChange(int groupId);
        public bool GetUseOSMaxFilePath(int groupId);
        public bool GetUseFileForwardDownNotRecv(int groupId);
        public bool GetUseClipBoard(int groupId);
        public bool GetUseClipCopyAndSend(int groupId);
        //public bool GetClipCopyAutoSend(int groupId);
        public bool GetUseClipApprove(int groupId);
        public bool GetUseClipBoardNoApproveButFileTrans(int groupId);
        public int GetClipUseAfterApprove(int groupId);
        public bool GetUseClipBoardFileTrans(int groupId);
        public bool GetUseFileClipManageUI(int groupId);
        public bool GetUseFileClipApproveUI(int groupId);
        public bool GetUseClipTypeSelectSend(int groupId);
        public bool GetUseClipTypeTextFirstSend(int groupId);
        public bool GetUseClipTypeText(int groupId);
        public bool GetUseClipTypeImage(int groupId);
        public bool getUseClipBoardPasteHotKey(int groupId);
        public bool GetUseEmailManageApprove(int groupId);
        public bool GetUseEmailApprovePreviewPopup(int groupId);

        public bool GetUseUIdlpData(int groupId);
        //public bool GetURLAutoTrans(int groupId);
        public bool GetUseURLRedirectionAlarm(int groupId);
        //public bool GetURLAutoAfterMsg(int groupId);
        public bool GetUseURLRedirectionAlarmType(int groupId);
        //public string GetURLAutoAfterBrowser(int groupId);
        //public string GetForwardUrl(int groupId);
        public bool GetAfterApprAutoCheck(int groupId);
        //public bool GetAfterBasicChk(int groupId);
        public bool GetUseApprLineLocalSave(int groupId);
        public bool GetUseApprDeptSearch(int groupId);
        public bool GetViewDlpApproverMyDept(int groupId);
        public bool GetUseOneAClockChangeAgentTimer(int groupId);
        public bool GetUserLineShowNameAndID(int groupId);
        public bool GetFileRecvAlarmRetain(int groupId);
        //public bool GetFileRecvTrayFix(int groupId);
        public bool GetApprCountAlarmRetain(int groupId);
        //public bool GetApprTrayFix(int groupId);
        public bool GetApprCompleteAlarmRetain(int groupId);
        //public bool GetUserApprActionTrayFix(int groupId);
        public bool GetApprRejectAlarmRetain(int groupId);
        //public bool GetUserApprRejectTrayFix(int groupId);
        public bool GetUseApprCountAlaram(int groupId);
        //public bool GetUseApprWaitNoti(int groupId);
        public bool GetUseClipAlarmTypeChange(int groupId);
        public bool GetUseInitAlarmPerDay(int groupId);
        public bool GetUseInitMessagePerDay(int groupId);
        public bool GetUseMainPageTypeChange(int groupId);
        //public PAGE_TYPE GetMainPageType(int groupId);
        //public string GetMainPage(int groupId, PAGE_TYPE enInitMainPage, bool useDashBoard);
        //public string GetMainPage(int groupId);
        public bool GetUseCloseTrayMove(int groupId);
        //public bool GetExitTrayMove(int groupId);
        public bool GetUseStartTrayMove(int groupId);
        public bool GetUseLoginAfterTray(int groupId);
        public bool GetUseLoginAfterShow(int groupId);
        //public bool GetStartTrayMove(int groupId);
        public bool GetUseStartProgramReg(int groupId);
        //public bool GetStartProgramReg(int groupId);
        //public bool GetScreenLockUserChange(int groupId);
        //public bool GetUseScreenLock(int groupId);
        public bool GetUseLogLevel(int groupId);
        public bool GetShowAdminInfo(int groupId);
        public bool GetUseFileCheckException(int groupId);
        public bool GetUsePCURL(int groupId);
        public bool GetUseFileTrans(int groupId);
        public bool GetUsePublicBoard(int groupId);
        public bool GetUseCertSend(int groupId);
        public bool GetUseApproveAfterLimit(int groupId);
        public bool GetUseClipBoardApproveAfterLimit(int groupId);
        public bool GetUseAllProxyAuthority(int groupId);
        public bool GetUseWebLinkPreviewer(int groupId);
        public string GetWebLinkPreviewerURL(int groupId);
        public bool GetUseLanguageSet(int groupId);
        public bool GetViewFileFilter(int groupId);
        public bool GetViewSGSideBarUIBadge(int groupId);
        public bool GetViewSGHeaderUIAlarmNoriAllDel(int groupId);
        public bool GetUseForceUpdate(int groupId);
        public bool GetUseForceBackgroundUpdate(int groupId);
        public bool GetVisiblePolicyUpdateButton(int groupId);
        public string GetApproverSearchType(int groupId);
        public bool GetUseInputSearchInApproverTree(int groupId);
        public string GetReceiverSearchType(int groupId);
        public bool GetUseInputSearchInReceiverTree(int groupId);
        public string GetProxySearchType(int groupId);
        public bool GetUseInputSearchInProxyTree(int groupId = 0);
        public string GetSecurityApproverSearchType(int groupId);
        public bool GetUseInputSearchInSecurityApproverTree(int groupId);
        public string GetApproveExtSelectType(int groupId);
        public bool GetUseInputSearchApproveExtTree(int groupId);
        public bool GetUseApproveExt(int groupId);
        public bool GetUseApproveExtRegardlessApprove(int groupId);
        public int GetMethodApproveExtRegardlessApprove(int groupId);
        public bool GetUseFileExceptionDescCheck(int groupId);
        public bool GetUsePKIsendRecv(int groupId);
        public bool GetPKIsendType(int groupId);
        public bool GetUseApprTreeSearch(int groupId);
        public string GetInitTransferFileExplorerPathInWindow(int groupId);
        public bool GetUseToastInsteadOfOSNotification(int groupId);
        public bool GetUiFileExpiredDateShow(int groupId);

        public bool GetShowInnerFileErrDetail(int groupId);


        public bool GetUseDlpFoundSendContinue(int groupId);

        public string GetScrTimeoutLockType(int groupId);

        public string GetOKTAUrl(int groupId);

        public bool GetVisibleLogOutButton(int groupId);

        /// <summary>
        /// -1 : 로그인때 session중복시 강제차단, 0 : 사용자가 Msg에서 결정, 1 : 강제로그인 진행
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int GetSessionDuplicateBlock(int groupId);

        public bool GetAllowDRM(int groupId);

        public bool GetHiddenLoginLogo(int groupId);
        public bool GetUseLoginCI(int groupId);
        public string GetLoginTextFontSize(int groupId);

        public int GetLoginConnectLimitCount(int groupId);
        public int GetLoginConnectDelaySecond(int groupId);

        public bool GetUseDrmAfterFileReceive(int groupId);

        public int GetDrmType(int groupId);
        public string GetSoftCampGrade(int groupId);
        public int GetDlpType(int groupId);

        public bool GetUseDlpCheck(int groupId);

        public bool GetUseHideApprLine(int groupId);


        public bool GetUseHideTitleDesc(int groupId);
        public bool GetUseHideTitle(int groupId);
        public bool GetUseHideDesc(int groupId);

        public int GetTransferTemplate(int groupId);

        public bool GetUseDashBoard(int groupId);

        public bool GetUseEmailApprUIwait(int groupId);

        public bool GetUseHideToastPopup(int groupId);

        public bool GetUseUnZipForTransfer(int groupId);

        public bool GetUseAskFileSendAlert(int groupId);


        public bool GetUseCommonEnvNetNameRevert(int groupId);


        public bool GetUseCheckZipFileInnerFileCount(int groupId);

        public bool GetMakeRecvDownPathShortCut(int groupId);

        public bool GetUseFromNameRecvDownPathShortCut(int groupId);

        public bool GetEmptyExtFileTrans(int groupid);

        public bool GetDeleteUploadFile(int groupId);

        public bool GetUseDlpAfterApproveToNormal(int groupId);
    }


    internal class SGopConfigService : ISGopConfigService
    {
        /// <summary>ISGopConfigService 에서 사용</summary>
        public Dictionary<int, ISGopConfig> GetSGopConfigService() => AppConfigInfo;

        private static Dictionary<int, ISGopConfig> _AppConfigInfo;
        /// <summary>
        /// AppOPsetting
        /// </summary>
        public static Dictionary<int, ISGopConfig> AppConfigInfo
        {
            get
            {
                if (_AppConfigInfo == null) LoadFile();
                return _AppConfigInfo;
            }
        }// => ref _AppConfigInfo;

        //private Dictionary<int, ISGopConfig> _AppConfigInfo;
        ///// <summary>
        ///// AppOPsetting
        ///// </summary>
        //public ref Dictionary<int, ISGopConfig> AppConfigInfo => ref _AppConfigInfo;

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGopConfigService>();

        private static void LoadFile()
        {
            string AppConfig = "";
            try
            {
                //로그 삭제
                HsLogDel hsLog = new HsLogDel();
                hsLog.Delete(7);    // 7일이전 Log들 삭제

                List<ISGNetwork> listNetworks = SGNetworkService.NetWorkInfo;

                if (!Directory.Exists(Environment.CurrentDirectory + $"/wwwroot/conf"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + $"/wwwroot/conf");

                _AppConfigInfo = new Dictionary<int, ISGopConfig>();
                var serializer = new DataContractJsonSerializer(typeof(SGopConfig));
                foreach (SGNetwork sgNetwork in listNetworks)
                {
                    AppConfig = Environment.CurrentDirectory + $"/wwwroot/conf/AppOPsetting_{sgNetwork.GroupID}_{sgNetwork.NetPos}.json";

                    CLog.Here().Information($"- AppOPsetting Path: [{AppConfig}]");

                    if (File.Exists(AppConfig))
                    {
                        try
                        {
                            CLog.Here().Information($"- AppOPsetting Loading... : [{AppConfig}]");

                            byte[] hsckByte = File.ReadAllBytes(AppConfig);
                            byte[] masterKey = SGCrypto.GetMasterKey();

                            bool isDeCrypt = true;
                            string strData = String.Empty;
                            try
                            {
                                byte[] dData = SGCrypto.AESDecrypt256(hsckByte, masterKey, PaddingMode.PKCS7);
                                strData = Encoding.UTF8.GetString(dData);
                            }
                            catch (Exception ex)
                            {
                                CLog.Here().Information($"- AppOPsetting Loading... : Decrypt Fail {AppConfig}]");
                                //디크립션 실패
                                isDeCrypt = false;
                            }

                            if (isDeCrypt)
                            {
                                SGopConfig appConfig = JsonSerializer.Deserialize<SGopConfig>(strData);
                                _AppConfigInfo.Add(sgNetwork.GroupID, appConfig);

                            }
                            else
                            {
                                //Open the stream and read it back.
                                using (FileStream fs = File.OpenRead(AppConfig))
                                {
                                    SGopConfig appConfig = (SGopConfig)serializer.ReadObject(fs);
                                    _AppConfigInfo.Add(sgNetwork.GroupID, appConfig);
                                }
                            }

                            CLog.Here().Information($"- AppOPsetting Load Completed : [{AppConfig}]");
                        }
                        catch (Exception ex)
                        {
                            CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
                            _AppConfigInfo.Add(sgNetwork.GroupID, new SGopConfig());
                        }
                    }
                    else
                    {
                        SGopConfig sgOpConfig = new SGopConfig();
                        _AppConfigInfo.Add(sgNetwork.GroupID, sgOpConfig);
                        //파일이 없으면 생성
                        try
                        {
                            using (var fs = new FileStream(AppConfig, FileMode.Create))
                            {
                                var encoding = Encoding.UTF8;
                                var ownsStream = false;
                                var indent = true;

                                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, encoding, ownsStream, indent))
                                {
                                    serializer.WriteObject(writer, sgOpConfig);
                                }
                            }

#if !DEBUG
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
#endif
                        }
                        catch (Exception ex)
                        {
                            CLog.Here().Warning($"Exception - Message : {ex.Message}, HelpLink : {ex.HelpLink}, StackTrace : {ex.StackTrace}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"SGopConfigService LoadFile(Path:{AppConfig}) Exception :{ex.ToString()}");
            }
        }

        public bool GetPocMode(int groupId)
        {
            return AppConfigInfo[groupId].PocMode;
        }

        public bool GetUseAppLoginType(int groupId)
        {
            return AppConfigInfo[groupId].bUseAppLoginType;
        }
        public int GetAppLoginType(int groupId)
        {
            return AppConfigInfo[groupId].LoginType;
        }
        public int GetCustomLoginType(int groupId)
        {
            return AppConfigInfo[groupId].CustomLoginType;
        }
        public string GetCustomLoginHttpUrl(int groupId)
        {
            return AppConfigInfo[groupId].CustomLoginHttpUrl;
        }
        public string GetCustomLoginSecurityKey(int groupId)
        {
            return AppConfigInfo[groupId].CustomLoginSecurityKey;
        }
        public string GetCustomLoginSecurityIV(int groupId)
        {
            return AppConfigInfo[groupId].CustomLoginSecurityIV;
        }
        public bool GetUseGPKILogin(int groupId)
        {
            return AppConfigInfo[groupId].bUseGpkiLogin;
        }
        public int GetNACLoginType(int groupId)
        {
            return AppConfigInfo[groupId].NACLoginType;
        }

        public string GetNACLoginEncryptKey(int groupId)
        {
            return AppConfigInfo[groupId].NACLoginEncryptKey;
        }

        public bool GetUseLoginIDSave(int groupId)
        {
            return AppConfigInfo[groupId].bUserIDSave;
        }
        public bool GetUseLoginIDSaveCheck(int groupId)
        {
            return AppConfigInfo[groupId].bUserIDSaveCheck;
        }
        public bool GetUseAutoLogin(int groupId)
        {
            return AppConfigInfo[groupId].bAutoLogin;
        }
        public bool GetUseAutoLoginCheck(int groupId)
        {
            return AppConfigInfo[groupId].bAutoLoginCheck;
        }
        public bool GetUseSelectFirstConnectNetServer(int groupId)
        {
            return AppConfigInfo[groupId].bUseUserSelectFirstServer;
        }
        public bool GetUseOneToMultiLogin(int groupId)
        {
            return AppConfigInfo[groupId].bUseOneToMultiLogin;
        }
        public bool GetUseOneToMultiLoginButoneBYoneLogout(int groupId)
        {
            return AppConfigInfo[groupId].bUseOneByOneLogOut;
        }
        public bool GetUseOver1auth(int groupId)
        {
            return AppConfigInfo[groupId].bUseOver1Auth;    // 기본값
        }
        public bool GetUseUserPWChange(int groupId)
        {
            return AppConfigInfo[groupId].bUserPWChange;
        }

        public bool GetUseGoogleOtp2FactorAuth(int groupId)
        {
            return AppConfigInfo[groupId].bUseGoogleOtp2FactorAuth;  // 구글 Otp를사용한 2차인증기능사용
        }


        public string GetPWChangeProhibitLimit(int groupId)
        {
            return AppConfigInfo[groupId].strPWChangeProhibitLimit;
        }
        public int GetPWChangeApplyCnt(int groupId)
        {
            return AppConfigInfo[groupId].nPWChangeApplyCnt;
        }
        public string GetInitPasswordInfo(int groupId)
        {
            return AppConfigInfo[groupId].strInitPasswd;
        }

        public bool GetUseIDAsInitPassword(int groupId)
        {
            return AppConfigInfo[groupId].bUseIDAsInitPassword;
        }

        public bool GetRFileAutoSend(int groupId)
        {
            return AppConfigInfo[groupId].bRFileAutoSend;
        }
        //public bool GetRMouseFileAddAfterTrans(int groupId)
        //{
        //    return AppConfigInfo[groupId].bRMouseFileAddAfterTrans;
        //}
        public bool GetUseNetOverAllsend(int groupId)
        {
            return AppConfigInfo[groupId].bUseNetOverAllsend;
        }
        public bool GetFileDownloadBeforeReciving(int groupId)
        {
            return AppConfigInfo[groupId].bFileDownloadBeforeReciving;
        }
        public bool GetNoApproveManageUI(int groupId)
        {
            return AppConfigInfo[groupId].bNoApproveManageUI;
        }
        public bool GetUseApproveManageUIForce(int groupId)
        {
            return AppConfigInfo[groupId].bUseApproveManageUIForce;
        }
        public bool GetEmptyfileTrans(int groupId)
        {
            return AppConfigInfo[groupId].bEmptyfileTrans;
        }
        public bool GetUseTitleDescSameCharCheck(int groupId)
        {
            return AppConfigInfo[groupId].bTitleDescSameChk;
        }
        public bool GetUseApprLineChkBlock(int groupId)
        {
            return AppConfigInfo[groupId].bApprLineChkBlock;
        }
        //public bool GetRecvFolderOpen(int groupId)
        //{
        //    return AppConfigInfo[groupId].bRecvFolderOpen;
        //}
        public bool GetUseRecvFolderChange(int groupId)
        {
            return AppConfigInfo[groupId].bRecvFolderChange;
        }
        public bool GetManualDownFolderChange(int groupId)
        {
            return AppConfigInfo[groupId].bManualDownFolderChange;
        }

        public bool GetUseFileTrans(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileTrans;
        }
        //public bool GetManualRecvDownChange(int groupId)
        //{
        //    return AppConfigInfo[groupId].bManualRecvDownChange;
        //}
        public bool GetUseUserRecvDownPath(int groupId)
        {
            return AppConfigInfo[groupId].bUseUserRecvDownPath;
        }
        public bool GetUseDenyPasswordZip(int groupId)
        {
            return AppConfigInfo[groupId].bUseDenyPasswordZip;
        }
        public bool GetUseFileForward(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileForward;
        }
        public bool GetFileForward(int groupId)
        {
            return AppConfigInfo[groupId].bFileForward;
        }
        public bool GetUsePartialFileAddInTransfer(int groupId)
        {
            return AppConfigInfo[groupId].bUsePartialFileAddInTransfer;
        }
        public bool GetUseCheckHardSpace(int groupId)
        {
            return AppConfigInfo[groupId].bUseChkHardSpace;
        }
        public bool GetUseFileApproveReason(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileApproveReason;
        }
        public bool GetUseClipBoardApproveReason(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipBoardApproveReason;
        }
        public bool GetUseFileSelectDelete(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileSelectDelete;
        }
        public bool GetUseFileAllDelete(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileAllDelete;
        }
        public bool GetUseCrossPlatformOSforFileName(int groupId)
        {
            return AppConfigInfo[groupId].bUseCrossPlatformOSforFileName;
        }
        public bool GetUseMinLengthTitleDesc(int groupId)
        {
            return AppConfigInfo[groupId].bUseTitleDescMinLength;
        }
        public bool GetUseMaxLengthTitleDesc(int groupId)
        {
            return AppConfigInfo[groupId].bUseTitleDescMaxLength;
        }
        public bool GetUseAgentBlockValueChange(int groupId)
        {
            return AppConfigInfo[groupId].bUseAgentBlockValueChange;
        }
        public bool GetUseOSMaxFilePath(int groupId)
        {
            return AppConfigInfo[groupId].bUseOSMaxFilePath;
        }
        public bool GetUseFileForwardDownNotRecv(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileForwardDownNotRecv;
        }
        public bool GetUseClipBoard(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipBoard;
        }
        public bool GetUseClipCopyAndSend(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipCopyAndSend;
        }
        //public bool GetClipCopyAutoSend(int groupId)
        //{
        //    return AppConfigInfo[groupId].bClipCopyAutoSend;
        //}
        public bool GetUseClipApprove(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipApprove;
        }
        public bool GetUseClipBoardNoApproveButFileTrans(int groupId)
        {
            return AppConfigInfo[groupId].bClipBoardNoApproveButFileTrans;
        }
        public int GetClipUseAfterApprove(int groupId)
        {
            return AppConfigInfo[groupId].nClipAfterApproveUseType;
        }
        public bool GetUseClipBoardFileTrans(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipBoardFileTrans;
        }
        public bool GetUseFileClipManageUI(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileClipManageUI;
        }
        public bool GetUseFileClipApproveUI(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileClipApproveUI;
        }
        public bool GetUseClipTypeSelectSend(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipTypeSelectSend;
        }
        public bool GetUseClipTypeTextFirstSend(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipTypeTextFirstSend;
        }
        public bool GetUseClipTypeText(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipTypeText;
        }
        public bool GetUseClipTypeImage(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipTypeImage;
        }
        public bool getUseClipBoardPasteHotKey(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipBoardPasteHotKey;
        }
        public bool GetUseEmailManageApprove(int groupId)
        {
            return AppConfigInfo[groupId].bUseEmail;
        }

        public bool GetUseEmailManageApproveOnly(int groupId)
        {
            return AppConfigInfo[groupId].bUseEmailOnly;
        }

        public bool GetUseEmailApprovePreviewPopup(int groupId)
        {
            return AppConfigInfo[groupId].bUseEmailApprovePreviewPopup;
        }

        public bool GetUseUIdlpData(int groupId)
        {
            return AppConfigInfo[groupId].bUiDlpShow;
        }
        //public bool GetURLAutoTrans(int groupId)
        //{
        //    return AppConfigInfo[groupId].bURLAutoTrans; 
        //}
        public bool GetUseURLRedirectionAlarm(int groupId)
        {
            return AppConfigInfo[groupId].bUseURLRedirectionAlarm;
        }
        //public bool GetURLAutoAfterMsg(int groupId)
        //{
        //    return AppConfigInfo[groupId].bURLAutoAfterMsg;   
        //}
        public bool GetUseURLRedirectionAlarmType(int groupId)
        {
            return AppConfigInfo[groupId].bUseURLRedirectionAlarmType;
        }
        //public string GetURLAutoAfterBrowser(int groupId)
        //{
        //    return AppConfigInfo[groupId].strURLAutoAfterBrowser;
        //}
        //public string GetForwardUrl(int groupId)
        //{
        //    return AppConfigInfo[groupId].strForwardUrl;
        //}
        public bool GetAfterApprAutoCheck(int groupId)
        {
            return AppConfigInfo[groupId].bShowAfterApprAutoCheck;
        }
        //public bool GetAfterBasicChk(int groupId)
        //{
        //    return AppConfigInfo[groupId].bAfterBasicChk;
        //}
        public bool GetUseApprLineLocalSave(int groupId)
        {
            return AppConfigInfo[groupId].bApprLineLocalSave;
        }
        public bool GetUseApprDeptSearch(int groupId)
        {
            return AppConfigInfo[groupId].bApprDeptSearch;
        }
        public bool GetViewDlpApproverMyDept(int groupId)
        {
            return AppConfigInfo[groupId].bViewDlpApproverSelectMyDept;
        }
        public bool GetUseOneAClockChangeAgentTimer(int groupId)
        {
            return AppConfigInfo[groupId].bUseAgentTime1aClock;
        }
        public bool GetUserLineShowNameAndID(int groupId)
        {
            return AppConfigInfo[groupId].bUserLineShowNameAndID;
        }

        public bool GetFileRecvAlarmRetain(int groupId)
        {
            return AppConfigInfo[groupId].bFileRecvAlarmRetain;
        }
        //public bool GetFileRecvTrayFix(int groupId)
        //{
        //    return AppConfigInfo[groupId].bFileRecvTrayFix;
        //}
        public bool GetApprCountAlarmRetain(int groupId)
        {
            return AppConfigInfo[groupId].bApprCountAlarmRetain;
        }
        //public bool GetApprTrayFix(int groupId)
        //{
        //    return AppConfigInfo[groupId].bApprTrayFix;
        //}
        public bool GetApprCompleteAlarmRetain(int groupId)
        {
            return AppConfigInfo[groupId].bApprCompleteAlarmRetain;
        }
        //public bool GetUserApprActionTrayFix(int groupId)
        //{
        //    return AppConfigInfo[groupId].bUserApprActionTrayFix;
        //}
        public bool GetApprRejectAlarmRetain(int groupId)
        {
            return AppConfigInfo[groupId].bApprRejectAlarmRetain;
        }
        //public bool GetUserApprRejectTrayFix(int groupId)
        //{
        //    return AppConfigInfo[groupId].bUserApprRejectTrayFix;
        //}
        public bool GetUseApprCountAlaram(int groupId)
        {
            return AppConfigInfo[groupId].bUseApprCountAlaram;
        }
        //public bool GetUseApprWaitNoti(int groupId)
        //{
        //    return AppConfigInfo[groupId].bUseApprWaitNoti;
        //}
        public bool GetUseClipAlarmTypeChange(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipAlarmType;
        }
        public bool GetUseInitAlarmPerDay(int groupId)
        {
            return AppConfigInfo[groupId].bUseInitAlarmPerDay;
        }
        public bool GetUseInitMessagePerDay(int groupId)
        {
            return AppConfigInfo[groupId].bUseInitMessagePerDay;
        }
        public bool GetUseMainPageTypeChange(int groupId)
        {
            return AppConfigInfo[groupId].bUseMainPageType;
        }
        //public PAGE_TYPE GetMainPageType(int groupId)
        //{
        //    return AppConfigInfo[groupId].enMainPageType;
        //}
        //public string GetMainPage(int groupId, PAGE_TYPE enInitMainPage, bool useDashBoard)
        //{
        //    string strPage = "/Welcome";
        //    PAGE_TYPE page;

        //    //사용자 선택이 NONE(초기값)이라면 프로그램에서 지정된 페이지로 설정
        //    page = (AppConfigInfo[groupId].enMainPageType == PAGE_TYPE.NONE) ? enInitMainPage : AppConfigInfo[groupId].enMainPageType;

        //    switch (page)
        //    {
        //        case PAGE_TYPE.NONE:
        //        case PAGE_TYPE.DASHBOARD:
        //            strPage = useDashBoard ? "/Welcome" : "/Transfer";
        //            break;

        //        case PAGE_TYPE.TRANSFER:
        //            strPage = "/Transfer";
        //            break;

        //        default:
        //            strPage = "/Welcome";
        //            break;
        //    }
        //    return strPage;
        //}

        //public string GetMainPage(int groupId)
        //{
        //    string strPage = "/Welcome";
        //    PAGE_TYPE page = AppConfigInfo[groupId].enMainPageType;
        //    switch (page)
        //    {
        //        case PAGE_TYPE.NONE:
        //        case PAGE_TYPE.DASHBOARD:
        //            strPage = "/Welcome";
        //            break;

        //        case PAGE_TYPE.TRANSFER:
        //            strPage = "/Transfer";
        //            break;

        //        default:
        //            strPage = "/Welcome";
        //            break;
        //    }
        //    return strPage;
        //}
        public bool GetUseCloseTrayMove(int groupId)
        {
            return AppConfigInfo[groupId].bUseCloseTrayMove;
        }
        //public bool GetExitTrayMove(int groupId)
        //{
        //    return AppConfigInfo[groupId].bExitTrayMove;
        //}
        public bool GetUseStartTrayMove(int groupId)
        {
            return AppConfigInfo[groupId].bUseStartTrayMove;
        }
        public bool GetUseLoginAfterTray(int groupId)
        {
            return AppConfigInfo[groupId].bUseLoginAfterTray;
        }

        public bool GetUseLoginAfterShow(int groupId)
        {
            return AppConfigInfo[groupId].bUseLoginAfterShow;
        }
        //public bool GetStartTrayMove(int groupId)
        //{
        //    return AppConfigInfo[groupId].bStartTrayMove;
        //}
        public bool GetUseStartProgramReg(int groupId)
        {
            return AppConfigInfo[groupId].bUseStartProgramReg;
        }
        //public bool GetStartProgramReg(int groupId)
        //{
        //    return AppConfigInfo[groupId].bStartProgramReg;
        //}
        //public bool GetScreenLockUserChange(int groupId)
        //{
        //    return AppConfigInfo[groupId].bScreenLockUserChange;
        //}
        //public bool GetUseScreenLock(int groupId)
        //{
        //    return AppConfigInfo[groupId].bUseScreenLock;
        //}
        public bool GetUseLogLevel(int groupId)
        {
            return AppConfigInfo[groupId].bUseLogLevel;
        }
        public bool GetShowAdminInfo(int groupId)
        {
            return AppConfigInfo[groupId].bShowAdminInfo;
        }
        public bool GetUseFileCheckException(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileCheckException;
        }
        public bool GetUsePCURL(int groupId)
        {
            return AppConfigInfo[groupId].bUsePCURL;
        }
        public bool GetUsePublicBoard(int groupId)
        {
            return AppConfigInfo[groupId].bUsePublicBoard;
        }
        public bool GetUseCertSend(int groupId)
        {
            return AppConfigInfo[groupId].bUseCertSend;
        }
        public bool GetUseApproveAfterLimit(int groupId)
        {
            return AppConfigInfo[groupId].bUseApproveAfterLimit;
        }
        public bool GetUseClipBoardApproveAfterLimit(int groupId)
        {
            return AppConfigInfo[groupId].bUseClipBoardApproveAfterLimit;
        }
        public bool GetUseAllProxyAuthority(int groupId)
        {
            return AppConfigInfo[groupId].bUseAllProxyAuthority;
        }
        public bool GetUseWebLinkPreviewer(int groupId)
        {
            return AppConfigInfo[groupId].bUseWebLinkPreviewer;
        }
        public string GetWebLinkPreviewerURL(int groupId)
        {
            return AppConfigInfo[groupId].strWebLinkPreviewerURL;
        }
        public bool GetUseLanguageSet(int groupId)
        {
            return AppConfigInfo[groupId].bUseLanguageSet;
        }
        public bool GetViewFileFilter(int groupId)
        {
            return AppConfigInfo[groupId].bViewFileFilter;
        }
        public bool GetViewSGSideBarUIBadge(int groupId)
        {
            return AppConfigInfo[groupId].bViewSGSideBarUIBadge;
        }
        public bool GetViewSGHeaderUIAlarmNoriAllDel(int groupId)
        {
            return AppConfigInfo[groupId].bViewSGHeaderUIAlarmNoriAllDel;
        }
        public bool GetUseForceUpdate(int groupId)
        {
            return AppConfigInfo[groupId].bUseForceUpdate;
        }
        public bool GetUseForceBackgroundUpdate(int groupId)
        {
            return AppConfigInfo[groupId].bUseForceBackgroundUpdate;
        }
        public bool GetVisiblePolicyUpdateButton(int groupId)
        {
            return AppConfigInfo[groupId].bVisiblePolicyUpdateButton;
        }
        public string GetApproverSearchType(int groupId)
        {
            return AppConfigInfo[groupId].strApproverSearchType;
        }
        public bool GetUseInputSearchInApproverTree(int groupId)
        {
            return AppConfigInfo[groupId].bUseInputSearchInApproverTree;
        }
        public string GetReceiverSearchType(int groupId)
        {
            return AppConfigInfo[groupId].strReceiverSearchType;
        }
        public bool GetUseInputSearchInReceiverTree(int groupId)
        {
            return AppConfigInfo[groupId].bUseInputSearchInReceiverTree;
        }
        public string GetProxySearchType(int groupId)
        {
            return AppConfigInfo[groupId].strProxySearchType;
        }
        public bool GetUseInputSearchInProxyTree(int groupId = 0)
        {
            return AppConfigInfo[groupId].bUseInputSearchInProxyTree;
        }
        public string GetSecurityApproverSearchType(int groupId)
        {
            return AppConfigInfo[groupId].strSecurityApproverSearchType;
        }
        public bool GetUseInputSearchInSecurityApproverTree(int groupId)
        {
            return AppConfigInfo[groupId].bUseInputSearchInSecurityApproverTree;
        }
        public string GetApproveExtSelectType(int groupId)
        {
            return AppConfigInfo[groupId].strApproveExtApproverSearchType;
        }
        public bool GetUseInputSearchApproveExtTree(int groupId)
        {
            return AppConfigInfo[groupId].bUseInputSearchApproveExtTree;
        }
        public bool GetUseApproveExt(int groupId)
        {
            return AppConfigInfo[groupId].bUseApproveExt;
        }
        public bool GetUseApproveExtRegardlessApprove(int groupId)
        {
            return AppConfigInfo[groupId].bUseApproveExtRegardlessApprove;
        }
        public int GetMethodApproveExtRegardlessApprove(int groupId)
        {
            return AppConfigInfo[groupId].nMethodApproveExtRegardlessApprove;
        }
        public bool GetUseFileExceptionDescCheck(int groupId)
        {
            return AppConfigInfo[groupId].bUseFileExceptionDescCheck;
        }
        public bool GetUsePKIsendRecv(int groupId)
        {
            return AppConfigInfo[groupId].bUsePKIsendRecv;    // 기본값
        }

        public bool GetPKIsendType(int groupId)
        {
            return AppConfigInfo[groupId].bPkiSendByFileTrans;
        }

        public bool GetUseApprTreeSearch(int groupId)
        {
            return false;
        }
        /// <summary>전송화면에서 초기 표시할 기본 경로 ("ROOT" / 명시된 경로 / "")</summary>        
        public string GetInitTransferFileExplorerPathInWindow(int groupId)
        {
            return AppConfigInfo[groupId].strInitTransferFileExplorerPathInWindow;
        }
        /// <summary>레지스트리 차단으로 OS 사용 불가한 경우, OS노티 대신 Toast 사용할지 여부</summary>        
        public bool GetUseToastInsteadOfOSNotification(int groupId)
        {
            return AppConfigInfo[groupId].bUseToastInsteadOfOSNotification;
        }
        /// <summary>
        /// 전송 관리에서 파일 만료일 표시 여부 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool GetUiFileExpiredDateShow(int groupId)
        {
            return AppConfigInfo[groupId].bUiFileExpiredDateShow;
        }
        /// <summary>
        /// 파일 추가에 제외된 파일 리스트에 내부에 걸린 파일 리스트 전부 보여주기 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool GetShowInnerFileErrDetail(int groupId)
        {
            return AppConfigInfo[groupId].bShowInnerFileErrDetail;
        }
        /// <summary>
        /// 서버DLP에서 개인정보 검출 됐을때, 정보보호 결재자 없이 현재결재자에게 결재받고 송신되도록 할지 유무
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool GetUseDlpFoundSendContinue(int groupId)
        {
            return AppConfigInfo[groupId].bDlpFoundSendContinue;
        }

        public string GetScrTimeoutLockType(int groupId)
        {
            return AppConfigInfo[groupId].strScrTimeoutLockType;
        }
        public string GetOKTAUrl(int groupId)
        {
            return AppConfigInfo[groupId].strOKTAUrl;
        }

        public bool GetVisibleLogOutButton(int groupId)
        {
            return AppConfigInfo[groupId].bVisibleLogOutButton;
        }


        public int GetSessionDuplicateBlock(int groupId)
        {
            return AppConfigInfo[groupId].nSessionDuplicate;        // 세션중복일때 사용자에게 강제 접속 여부를 묻지 않고, 바로 차단
        }

        public bool GetAllowDRM(int groupId)
        {
            return AppConfigInfo[groupId].bAllowDRM;
        }

        public bool GetHiddenLoginLogo(int groupId)
        {
            return AppConfigInfo[groupId].bHiddenLoginLogo;
        }

        public bool GetUseLoginCI(int groupId)
        {
            return AppConfigInfo[groupId].bUseLoginCI;
        }

        public string GetLoginTextFontSize(int groupId)
        {
            return AppConfigInfo[groupId].strLoginTextFontSize;
        }
        public int GetLoginConnectLimitCount(int groupId)
        {
            return AppConfigInfo[groupId].nLoginConnectLimitCount;
        }
        public int GetLoginConnectDelaySecond(int groupId)
        {
            return AppConfigInfo[groupId].nLoginConnectDelaySecond;
        }

        public bool GetUseDrmAfterFileReceive(int groupId)
        {
            return AppConfigInfo[groupId].bUseDrmAfterFileReceive;
        }
        public int GetDrmType(int groupId)
        {
            return AppConfigInfo[groupId].nDrmType;
        }
        public string GetSoftCampGrade(int groupId)
        {
            return AppConfigInfo[groupId].strSoftCampGrade;
        }
        public int GetDlpType(int groupId)
        {
            return AppConfigInfo[groupId].nDlpType;
        }
        public bool GetUseDlpCheck(int groupId)
        {
            return AppConfigInfo[groupId].bUseDlpCheck;
        }

        public bool GetUseHideApprLine(int groupId)
        {
            return AppConfigInfo[groupId].bHideApprLine;
        }

        public bool GetUseHideTitleDesc(int groupId)
        {
            return AppConfigInfo[groupId].bHideTitleDesc;
        }
        public bool GetUseHideTitle(int groupId)
        {
            return AppConfigInfo[groupId].bHideTitle;
        }
        public bool GetUseHideDesc(int groupId)
        {
            return AppConfigInfo[groupId].bHideDesc;
        }

        public bool GetUseDashBoard(int groupId)
        {
            return AppConfigInfo[groupId].bUseDashBoard;
        }

        public bool GetUseEmailApprUIwait(int groupId)
        {
            return AppConfigInfo[groupId].bUseEmailApprUIwait;
        }


        public int GetTransferTemplate(int groupId)
        {
            return AppConfigInfo[groupId].nTransferTemplate;
        }

        public bool GetUseHideToastPopup(int groupId)
        {
            return AppConfigInfo[groupId].bHideMoveTrayMsgPopup;
        }

        public bool GetUseUnZipForTransfer(int groupId)
        {
            return AppConfigInfo[groupId].bUseUnZipForTransfer;
        }

        public bool GetUseAskFileSendAlert(int groupId)
        {
            return AppConfigInfo[groupId].bUseAskFileSendAlert;
        }

        public bool GetUseCommonEnvNetNameRevert(int groupId)
        {
            return AppConfigInfo[groupId].bUseCommonEnvNetNameRevert;
        }
        public bool GetUseCheckZipFileInnerFileCount(int groupId)
        {
            return AppConfigInfo[groupId].bUseCheckZipFileInnerFileCount;
        }

        public bool GetMakeRecvDownPathShortCut(int groupId)
        {
            return AppConfigInfo[groupId].bMakeRecvDownPathShortCut;
        }

        public bool GetUseFromNameRecvDownPathShortCut(int groupId)
        {
            return AppConfigInfo[groupId].bUseFromNameRecvDownPathShortCut;
        }

        public bool GetEmptyExtFileTrans(int groupId)
        {
            return AppConfigInfo[groupId].bIsBlockEmptyExt;
        }

        public bool GetDeleteUploadFile(int groupId)
        {
            return AppConfigInfo[groupId].bDeleteUploadFile;
        }

        public bool GetUseDlpAfterApproveToNormal(int groupId)
        {
            return AppConfigInfo[groupId].bUseDlpAfterApproveToNormal;
        }

    }

}