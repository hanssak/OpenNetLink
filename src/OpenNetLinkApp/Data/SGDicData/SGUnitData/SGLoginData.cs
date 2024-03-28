using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using System.Data;
using Serilog;
using AgLogManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eIDPWresultJob
    {
        eNone = 0,
        /// <summary>
        /// 강제 변경
        /// </summary>
        eEnforced = 1,
        /// <summary>
        /// 선택 변경
        /// </summary>
        eAfterWard = 2,
        /// <summary>
        /// 초기 비밀번호 설정
        /// </summary>
        eInitPW = 3
    }

    public enum ePassWDChgType
    {
        eNone = 0,
        /// <summary>강제 정책</summary>
        eEnforce = 1,
        /// <summary>선택 변경</summary>
        eAfterward = 2
    }

    public enum eUrlType
    {
        eNone = 0,
        eWhiteList = 1,
        eBlackList = 2
    }


    public class SGNetOverData
    {
        /// <summary>
        /// SGNetType
        /// (SecureGate 망 종류. (ex) I(내부), E(외부), IA(내부 A망), ...)
        /// </summary>
        public string strDestSysid = "";

        /// <summary>
        /// SGNetName
        /// (망 명칭 (ex) 내부망, 외부망, 내부 A망, ...)
        /// </summary>
        public string strDestSysName = "";

        public string strPolicy = "";
        public int nIdx = 0;
        public bool bUseFileTrans = false;
        public bool bUseClipTrans = false;
        public bool bUseUrlTrans = false;
        public bool bUseApprove = false;
        public bool bUseinterlock = false;

        public SGNetOverData()
        {

        }

        ~SGNetOverData()
        {

        }


    }

    /// <summary>
    /// (구 Bind / 신 PostLogin)
    /// </summary>
    public class SGLoginData : SGData
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGLoginData>();
        private readonly XmlConfService xconf = new XmlConfService();
        /// <summary>
        /// SGData 저장될 시, Ready에서 받아놨던, sg_net_type 내부  저장
        /// </summary>
        public string MySgNetType = "";

        public SGLoginData()
        {

        }

        ~SGLoginData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        /// <summary>
        /// MySgNetType - 현재 내부망에 접속되어 있는지 여부를 반환
        /// <br/>LoginData 세팅 시, Ready의 값을 가지고 와서 저장
        /// <br/>(SGReadyData의 동명의 함수와 동일한 값 반환)
        /// </summary>
        /// <returns>true 내부, false 외부</returns>
        public bool GetSystemPosition() => MySgNetType.StartsWith("I");

        public List<object> GetDestinationSgNetList() => GetTagDataObjectList("user_policy", "server_policy", "destination_sg_net_list");

        /// <summary>
        /// 목적지에 대한 Net 정보 반환
        /// ("server_policy", "destination_sg_net_list")
        /// </summary>
        /// <param name="exceptMe">true:현재 접속중인 서버의 정보는 제외</param>
        /// <returns></returns>
        public List<SGNetOverData> GetDestinationInfo(bool exceptMe = true)
        {
            List<SGNetOverData> retValue = new List<SGNetOverData>();
            List<object> sgNetList = GetDestinationSgNetList();

            int idx = 0;
            foreach (object destSgNet in sgNetList)
            {
                SGNetOverData dest = new SGNetOverData();
                dest.nIdx = idx;
                dest.strDestSysid = destSgNet.GetTagDataObject("type").ToString();
                dest.strDestSysName = destSgNet.GetTagDataObject("name").ToString();

                List<object> destPolicy = destSgNet.GetTagDataObjectList("policy_list");
                dest.bUseFileTrans = destPolicy.Contains("file");
                dest.bUseClipTrans = destPolicy.Contains("clip");
                dest.bUseUrlTrans = destPolicy.Contains("url");
                dest.bUseApprove = destPolicy.Contains("approval");
                dest.bUseinterlock = destPolicy.Contains("interlock");

                if (dest.strDestSysid == MySgNetType)
                {
                    if (exceptMe == false) retValue.Add(dest);
                }
                else retValue.Add(dest);

                idx++;
            }
            return retValue;
        }

        public Dictionary<string, object> GetUserHRDic()
        {
            JObject userHrJObj = (JObject)GetTagDataObject("user_hr");
            Dictionary<string, object> userHrDic = (userHrJObj != null) ? userHrJObj.ToObject<Dictionary<string, object>>() : new Dictionary<string, object>();
            return userHrDic;
        }

        public UserHRinfo GetUserHRInfo()
        {
            UserHRinfo userHr = new UserHRinfo()
            {
                strId = GetTagData("user_hr", "user_id"),
                strSeq = GetTagData("user_hr", "user_seq"),
                strName = GetTagData("user_hr", "name"),
                strRank = GetTagData("user_hr", "rank"),
                strDeptName = GetTagData("user_hr", "dept_name"),
                deptSeq = GetTagData("user_hr", "dept_seq"),
                strPosition = GetTagData("user_hr", "position")
            };
            return userHr;
        }

        public static string LoginFailMessage(int nRet)
        {
            string strLoginFailMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (nRet)
            {
                case 1:
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0037");                                         // 사용자 인증에 실패 하였습니다.
                    break;

                case 2: // 중복로그인
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0039");                                         // 이미 다른곳에서 사용자가 로그인 되어있습니다.
                    break;

                case 3: //사용자 계정 일시 잠김
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0241");                                         // 사용자 계정이 일시적으로 잠겼습니다./r/n/r/n3분후 다시 시도하여 주십시요.
                    break;

                case 4: //사용자 계정 영구 잠김
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0041");                                         // 사용자 계정이 잠겼습니다.
                    break;

                case 5: //등록되지 않은 IP 또는 MAC 정보
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0042");                                         // 등록되지 않은 IP 또는 MAC 정보입니다.
                    break;

                case 6: //계정정지
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0090");                                         // 계정이 정지된 사용자입니다./r/n관리자에게 문의 해주십시오.
                    break;

                case 7: //계정만료
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0091");                                         // 계정이 만료된 사용자입니다./r/n관리자에게 문의 해주십시오.
                    break;

                case 8:
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0155");                                         // 프로그램의 무결성 검사가 실패하였습니다.&#10;&#10;프로그램을 재설치하여 주십시오.
                    break;

                case 9: // OTP 등록 취소
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0159");                                         // OTP 등록을 취소하시겠습니까? -> Google OTP
                    break;

                case 10: // OTP 인증 취소
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0160");                                         // OTP 인증을 취소하였습니다. -> Google OTP
                    break;

                case 11: // OTP 인증 실패
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0161");                                         // OTP 인증에 실패하였습니다. -> Google OTP
                    break;

                case 12: // 등록되지 않은 IP 
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0203");                                         // 등록되지 않은 IP 정보입니다.
                    break;
                case 13: // 등록되지 않은 MAC
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0204");                                         // 등록되지 않은 MAC 정보입니다.
                    break;
                case 14: //  장기 미접속 차단
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0301");                                         // 장기간 접속하지 않아 차단되었습니다./r/n접속 차단을 해제하시려면 관리자에게 요청하시기 바랍니다.
                    break;
                default: //사용자 인증에 실패
                    strLoginFailMsg = xmlConf.GetWarnMsg("W_0037");                                         // 사용자 인증에 실패 하였습니다.
                    break;
            }
            return strLoginFailMsg;
        }


        /// <summary>
        /// 현재 내부망에 접속되어 있는지 여부를 문자열로 반환(GetSystemPosition 기반)
        /// </summary>
        /// <returns>I or E (내부/외부)</returns>
        public string GetSysID()
        {
            //string strSysID = "I";
            //int nConnNetWork = GetConnNetwork();
            //if (nConnNetWork == 0)
            //	strSysID = "E";
            string strSysID = (GetSystemPosition()) ? "I" : "E";
            return strSysID;
        }

        /// <summary>
        /// 수신받은 서버 시간 정보를 반환한다.
        /// <br>주의:실시간 증가하는 시간이 아닌, 마지막 응답에 저장된 서버시간</br>
        /// </summary>
        /// <returns>서버 시간</returns>
        public string GetSvrTime()
        {
            string strTime = GetTagData("user_policy", "server_policy", "time");
            return strTime;
        }

        /// <summary>
        /// 서버 시간정보를 얻는다(GetSvrTime을 내부적에서 사용)
        /// <br>주의:실시간 증가하는 시간이 아닌, 마지막 응답에 저장된 서버시간</br>
        /// </summary>
        /// <returns></returns>
        public DateTime GetSvrTimeConvert()
        {
            string strTime = GetSvrTime();
            if (strTime.Equals(""))
                return System.DateTime.Now;

            string strD = strTime.Substring(0, 8);
            string strT = strTime.Substring(8);


            string strYear = strTime.Substring(0, 4);
            string strMonth = strTime.Substring(4, 2);
            string strDay = strTime.Substring(6, 2);
            string strHour = strTime.Substring(8, 2);
            string strMinute = strTime.Substring(10, 2);
            string strSecond = strTime.Substring(12, 2);

            string strConvertDay = String.Format("{0}/{1}/{2}", strYear, strMonth, strDay);
            string strConvertTime = String.Format("{0}:{1}:{2}", strHour, strMinute, strSecond);

            DateTime dt = Convert.ToDateTime(strConvertDay);
            dt = Convert.ToDateTime(strConvertTime);
            return dt;
        }

        /// <summary>
        /// 사용자의 이름을 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            string strData = GetTagData("user_hr", "name");
            return strData;
        }

        /// <summary>
        /// 사용자의 Sequence 정보를 반환한다.(return 사용자 Sequence)
        /// </summary>
        /// <returns></returns>
        public string GetUserSequence()
        {
            string strData = GetTagData("user_hr", "user_seq");
            return strData;
        }
        /// <summary>
        /// 사용자의 ID 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetUserID()
        {
            string strData = GetTagData("user_hr", "user_id");
            return strData;
        }

        /// <summary>
        /// 사용자의 팀 이름을 반환한다..
        /// </summary>
        /// <returns></returns>
        public string GetTeamName()
        {
            string strData = GetTagData("user_hr", "dept_name");
            return strData;
        }
        /// <summary>
        /// 사용자의 팀 코드를 반환한다..
        /// </summary>
        /// <returns></returns>
        public string GetTeamCode()
        {
            string strData = GetTagData("user_hr", "dept_seq");
            return strData;
        }

        /// <summary>
        /// 사용자의 Position 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetUserPosition()
        {
            string strData = GetTagData("user_hr", "position");
            return strData;
        }

        /// <summary>
        /// 사용자의 Rank 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetRank()
        {
            string strData = GetTagData("user_hr", "rank");
            return strData;
        }

        /// <summary>
        /// 사용자의 실 결재권한 여부를 반환한다. (return 0 : 일반사용자, 1: 결재권자, 2: 전결자)
        ///<br>[주의] userInfo Service에 삽입시에만 사용</br> 
        /// <br>(OpenNetLink의 동작 시 권한은 UserInfo의 GetUserApprPos 사용)</br>
        /// </summary>
        /// <returns></returns>
        public int GetUserApprpos()
        {
            string strData = GetTagData("approver_type", "authority");
            int.TryParse(strData, out int nApprPos);
            return nApprPos;
        }
        /// <summary>
        /// GetUserApprpos 확장함수로 문자열을 직접 제공한다
        /// </summary>
        /// <returns></returns>
        public string GetUserApprposString() => GetUserApprpos() switch
        {
            0 => xconf.GetTitle("T_INFO_NORMAL_USER"),
            1 => xconf.GetTitle("T_INFO_APPROVER_USER"),
            2 => xconf.GetTitle("T_INFO_APPROVE_FREE_USER"),
            _ => "",
        };

        /// <summary>
        /// 사용자의 개인정보,보안결재권한 여부를 반환한다. (return true : 보안결재자)
        /// </summary>
        /// <returns>true: 보안결재자 / false : 일반사용자</returns>
        public bool GetExApprovalUse()
        {

            string strData = GetTagData("approver_type", "dlp_authority");
            return (strData == "1");
        }
        public string GetUserPrivacyApprPosString() => GetExApprovalUse() switch
        {
            true => xconf.GetTitle("T_APPROVE_PRIVACY"),
            false => xconf.GetTitle("T_INFO_NORMAL_USER")
        };
        /// <summary>
        /// 수신파일 폴더 삭제 주기를 반환한다.
        /// </summary>
        /// <returns>시간 단위</returns>
        public int GetFileRemoveCycle()
        {
            string strData = GetTagData("user_policy", "file_delete_cycle");
            if (strData.Equals(""))
                return 0;
            int.TryParse(strData, out int size);
            return size;
        }

        /// <summary>
        /// 한번에 전송가능한 파일의 최대 사이즈를 반환한다.
        /// </summary>
        /// <returns>MB 단위</returns>
        public Int64 GetFileLimitSize()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_one_time", "total_size");
            if (strData.Equals(""))
                return 0;
            int.TryParse(strData, out int size);
            return size;
        }

        /// <summary>
        /// 한번에 전송가능한 파일의 최대 개수를 반환한다.
        /// </summary>
        /// <returns>전송가능한 파일의 최대 개수</returns>
        public int GetFileLimitCount()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_one_time", "total_count");
            if (strData.Equals(""))
                return 0;
            int.TryParse(strData, out int count);
            return count;
        }


        /// <summary>
        /// 클립보드 사용 여부를 반환한다.
        /// 0:금지, 1:전체허용, 2:반입 계속 가능, 3:반출 계속 가능"
        /// </summary>
        /// <returns>true  : 클립보드 전송 가능</returns>
        public bool GetClipboard()
        {
            string strData = GetTagData("user_login", "user_policy", "clip_policy_flag", "type");
            bool bInner = GetSystemPosition();

            if (strData == "0") //금지
                return false;
            if (strData == "1") //전체 허용
                return true;
            if (strData == "2")  //반입 가능
                return (!bInner);
            if (strData == "3") //반출 가능
                return (bInner);

            return false;
        }

        /// <summary>
        /// 수동다운로드 사용 여부를 반환한다.
        /// </summary>
        /// <returns>true : 수동다운로드 사용.</returns>
        public bool GetManualDownload()
        {
            string strData = GetTagData("user_policy", "manual_download_use");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// OTP 발급불가/가능. 타망에서 OTP 로그인을 할 경우 사용. OTP 생성 메뉴 활성화
        /// </summary>
        /// <returns>true : OTP발급가능(발급 Popup 표시)</returns>
        public bool GetOTPPublishUse()
        {
            string strData = GetTagData("user_policy", "otp_publishing_use");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// 결재 사용 여부를 반환한다. (3망연계 설정 사용시 3망연계 설정값도 반영함)
        /// <para>서버 정책의 approve_use 사용</para>
        /// </summary>
        /// <returns>true : 결재 사용</returns>		
        public bool GetApprove()
        {
            bool bRet = true;
            string strData = GetTagData("user_policy", "approval_policy", "approve_use");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// 결재 사용 시 결재자 편집 사용 여부를 반환한다
        /// <para>user_login.user_policy.approval_policy.action 사용</para>
        /// </summary>
        /// <returns>결재자 편집 사용</returns>
        public bool GetApproveAppend()
        {
            if (GetApprove() == false)  // 3망연계정보 상관없음
                return false;

            string strData = GetTagData("user_policy", "approval_policy", "action");
            return (strData == "all" || strData == "select");
        }

        /// <summary>
        /// 결재 사용 시 대결재자 편집 사용여부를 반환
        /// </summary>
        /// <returns>true : 대결재자 편집 사용</returns>
        public bool GetDeputyApprove()
        {
            if (GetApprove() == false)  // 3망연계정보 상관없음
                return false;

            string strData = GetTagData("user_policy", "approval_policy", "action");
            return (strData == "all" || strData == "fix");
        }

        /// <summary>
        /// 결재 사용 시, 결재 대결재자/결재자 추가 정보 (과거 APPROVEPROXY)
        /// </summary>
        /// <returns>0:사용안함 / 1:고정결재(대결재) / 2:선택 결재 / 3: 1+2</returns>
        public int GetApproveProxyInteger()
        {
            if (GetApprove() == false)  // 3망연계정보 상관없음
                return -1;

            string strData = GetTagData("user_policy", "approval_policy", "action");
            if (strData == "unuse")
                return 0;
            if (strData == "fix")
                return 1;
            if (strData == "select")
                return 2;
            if (strData == "all")
                return 3;

            return 0;
        }

        /// <summary>
        /// 파일 확장자 제한 타입을 반환한다.
        /// </summary>
        /// <returns></returns>
        public bool GetFileFilterType()
        {
            string strData = GetTagData("user_policy", "file_filter_type");
            if (strData.Equals("w"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 파일 확장자 제한 정보를 반환
        /// </summary>
        /// <returns>파일확장자 정보</returns>
        public string GetFileFilter()
        {
            List<string> filterList = GetTagDataList("user_policy", "file_filter_list");

            if (filterList == null || filterList.Count <= 0)
                return ";";
            else
                return string.Join(";", filterList);
        }

        /// <summary>
        /// VIP 권한을 갖고 있는 사용자인지 여부를 반환
        /// </summary>
        /// <returns>true : VIP 사용자</returns>
        public bool IsVipUser()
        {
            string strData = GetTagData("approver_type", "vip");
            return (strData.ToUpper() == "TRUE");
        }
        /// <summary>
        /// 화면잠금 시간 정보를 반환
        /// </summary>
        /// <returns>분단위</returns>
        public int GetSCRLimit()
        {
            string strData = GetTagData("user_policy", "screen_lock_cycle");
            int.TryParse(strData, out int nValue);
            return nValue;
        }

        /// <summary>
        /// 비밀번호 변경 시 복잡도 확인 여부를 반환
        /// </summary>
        /// <returns>true : 비밀번호 복잡도 사용</returns>
        public bool GetPasswordRule()
        {
            string strData = GetTagData("user_policy", "server_policy", "pw_complexity_check");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// 파일 Part Size 값을 반환한다.
        /// (Rest-전송 패킷 최대 사이즈)
        /// </summary>
        /// <returns>KB 단위</returns>
        public Int64 GetFilePartSize()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_packet_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }

        /// <summary>
        /// 파일 전송 대역폭을 반환
        /// </summary>
        /// <returns>BPS(bit per Second) 단위</returns>
        public Int64 GetFileBandWidth()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_bandwith");
            if (!strData.Equals(""))
                return 0;
            Int64 bandwidth = Convert.ToInt64(strData);
            return bandwidth;
        }
        /// <summary>
        /// 서버에 업데이트 대기 중인 Client Version 을 반환
        /// </summary>
        /// <returns>Client Version 정보. ( ex) NetLink 2.03 )</returns>
        public string GetServClientVersion()
        {
            //TODO 고도화 - 업데이트 확인 체크 프로세스 변경 예정
            string strData = GetTagData("CLIENTVERSION");
            return strData;
        }

        /// <summary>
        /// 서버에 업데이트 대기 중인 Client 패치 파일 존재 여부를 반환
        /// </summary>
        /// <returns>true : Client 패치파일 존재</returns>
        public bool GetClientUpgrade()
        {
            //TODO 고도화 - 업데이트 확인 체크 프로세스 변경 예정
            string strData = GetTagData("CLIENTUPGRADE");
            int nValue = 0;
            if (!strData.Equals(""))
                nValue = Convert.ToInt32(strData);
            if (nValue == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 일반 사용자도 대결재 권한 및 결재 권한이 있는지 여부를 반환한다.(구 ApproveProxyRight)
        /// </summary>
        /// <returns>true : 대결재 권한 및 결재 권한 존재</returns>
        public bool GetApproveProxyRight()
        {
            string strData = GetTagData("user_policy", "server_policy", "common_proxy_right_use");
            return (strData.ToUpper() == "TRUE");
        }

        /// <summary>
        /// 파일 전송 사용 여부를 확인한다..
        /// 0:금지, 1:전체허용, 2:반입 계속 가능, 3:반출 계속 가능
        /// </summary>
        /// <returns>파일 전송 가능</returns>
        public bool GetFileTrans()
        {
            string strData = GetTagData("user_policy", "file_upload_time", "type");
            bool bInner = GetSystemPosition();

            //Test용 -> 삭제 예정
            return true;
            if (strData == "0") //금지
                return false;
            if (strData == "1")  //전체 허용
                return true;
            if (strData == "2")  //반입 가능
                return (!bInner);
            if (strData == "3")   //반출 가능
                return (bInner);

            return false;
        }

        /// <summary>
        /// 파일전송 사용 Mode 값을 return (0,1,2)
        /// </summary>
        /// <returns></returns>
        public int GetModeValueForFileTrans()
        {
            //TODO 고도화 - 프로세스 확인 필요
            string strData = GetTagData("FILEUPLOADTIME");

            int nRet = -1;
            try
            {
                if ((strData?.Length ?? 0) > 0)
                {
                    strData = strData.Substring(0, 1);
                    return Convert.ToInt32(strData);
                }
            }
            catch (Exception ex)
            {
                nRet = -2;
                Log.Logger.Here().Information($"GetModeValueForFileTrans, Exception(MSG) : {ex.Message}");
            }

            return nRet;
        }


        /// <summary>
        /// Server에서 요일별로 받은 파일전송 동작하도록할지 유무 Low data들 Dictionary화 해서 반환함<br></br>
        /// 0: 일요일,.. 6:토요일, 7:공휴일<br></br>
        /// Mode 값에 상관없이 동일한 형식으로 변환해서 반환함
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string[]> GetModeWeekDataForFileTrans()
        {
            //TODO 고도화 - 프로세스 확인 필요
            Dictionary<int, string[]> dicWeekData = new Dictionary<int, string[]>();

            string strData = GetTagData("FILEUPLOADTIME");
            // Test)
            //strData = "2/0|0|1|7~8.17~23|2|-1|3|7~8.17~23|4|0|5|0~24|6|1~23|7|1~18";
            //strData = "1/0,6/!9~18";

            try
            {
                if (string.IsNullOrEmpty(strData) == false)
                {
                    int nModeValue = Convert.ToInt32(strData.Substring(0, 1));
                    int nPos = strData.IndexOf('/');
                    if (nPos < 1)
                    {
                        Log.Logger.Here().Information($"GetModeWeekDataForFileTrans, Error : syntax !!!");
                        return dicWeekData;
                    }

                    strData = strData.Substring(nPos + 1);

                    if (nModeValue == 1)
                    {

                        // ex) strData = "1/0,1,2,6/!9~18";
                        string[] arrAfter = strData.Split("/");

                        if ((arrAfter?.Length ?? 0) == 2)
                        {

                            string[] strArrUseOneDay = new string[1] { "0" };
                            string[] strArrUseNoOneDay = new string[1] { "-1" };

                            if (string.Compare(arrAfter[0], "all", true) == 0 ||
                                 string.Compare(arrAfter[1], "all", true) == 0)
                            {
                                for (int iDx = 0; iDx < 8; iDx++)
                                {
                                    dicWeekData.TryAdd(iDx, strArrUseOneDay);
                                }
                            }
                            else if (string.Compare(arrAfter[0], "none", true) == 0 &&
                                    string.Compare(arrAfter[1], "none", true) == 0)
                            {
                                for (int iDx = 0; iDx < 7; iDx++)
                                {
                                    dicWeekData.TryAdd(iDx, strArrUseNoOneDay);
                                }

                                dicWeekData.TryAdd(7, (GetTodayIsHoliday()) ? strArrUseOneDay : strArrUseNoOneDay);
                            }
                            else
                            {
                                // 요일별설정
                                if (string.Compare(arrAfter[0], "none", true) != 0)
                                {
                                    arrAfter[0] = arrAfter[0].Replace('.', ',');
                                    string[] strWeekUseDay = arrAfter[0].Split(',');
                                    foreach (string strWeekNum in strWeekUseDay)
                                    {
                                        dicWeekData.TryAdd(Convert.ToInt32(strWeekNum), strArrUseOneDay);
                                    }
                                }

                                string[] strArrDayData = null;
                                for (int iDx = 0; iDx < 7; iDx++)
                                {
                                    if (dicWeekData.TryGetValue(iDx, out strArrDayData) == false)
                                    {
                                        dicWeekData.TryAdd(iDx, strArrUseNoOneDay);
                                    }
                                }

                                // 휴일설정
                                dicWeekData.TryAdd(7, (GetTodayIsHoliday()) ? strArrUseOneDay : strArrUseNoOneDay);

                                // 시간별설정
                                if (string.Compare(arrAfter[1], "none", true) != 0)
                                {
                                    string[] strArrUseTime = new string[1] { arrAfter[1] };
                                    for (int iDx = 0; iDx < 8; iDx++)
                                    {
                                        if (dicWeekData.TryGetValue(iDx, out strArrDayData))
                                        {
                                            if (strArrDayData[0] == "-1")
                                            {
                                                dicWeekData.Remove(iDx);
                                                dicWeekData.TryAdd(iDx, strArrUseTime);
                                            }


                                        }
                                        else
                                        {
                                            dicWeekData.TryAdd(iDx, strArrUseTime);
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            throw new Exception("FILEUPLOADTIME syntax Error");
                        }
                    }
                    else if (nModeValue == 2)
                    {

                        // ex) 2/0|0|1|-1|2|-1|3|7~8.17~23|4|0|5|0~24|6|1~23|7|1~18
                        string[] strWeekData = strData.Split('|');
                        int nDx = 0;
                        for (int iDx = 0; iDx < 16;)
                        {
                            if (dicWeekData.ContainsKey(nDx))
                                dicWeekData.Remove(nDx);

                            string[] strArrDayUseinfo = strWeekData[iDx + 1].Split('.');

                            dicWeekData.TryAdd(nDx, strArrDayUseinfo);
                            iDx += 2;
                            nDx++;
                        }

                    }

                } // if (string.IsNullOrEmpty(strData) == false)

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Information($"GetModeWeekDataForFileTrans, Exception(MSG) : {ex.Message}");
            }

            return dicWeekData;
        }

        /// <summary>
        /// 요일별 파일전송 기능 사용때에 display할 서버의 정책값을 문구로 만들어내는 함수
        /// </summary>
        /// <param name="strUseOneday"></param>
        /// <param name="strUseNoday"></param>
        /// <param name="strDisplay"></param>
        /// <param name="strDisplayOutData"></param>
        /// <returns></returns>
        public bool GetFileTransWeekTimeDisplay(string strUseOneday, string strUseNoday, string strDisplay, string strTimeConvertMsg, out string strDisplayOutData)
        {
            bool bRet = false;
            strDisplayOutData = "";

            try
            {
                int nMode = GetModeValueForFileTrans();

                if (nMode == 1 || nMode == 2)
                {
                    Dictionary<int, string[]> dicWeekData = GetModeWeekDataForFileTrans();
                    string[][] strArrayWeek = new string[8][];

                    for (int nDx = 0; nDx < 8; nDx++)
                    {
                        if (dicWeekData.TryGetValue(nDx, out strArrayWeek[nDx]) == false)
                        {
                            throw new Exception("FileTrans Week Time Data, Wrong!");
                        }

                        string strTmpData2 = "";
                        if ((strArrayWeek[nDx]?.Length ?? 0) == 1)
                        {
                            if (strArrayWeek[nDx][0] == "0")
                                strArrayWeek[nDx][0] = strUseOneday;
                            else if (strArrayWeek[nDx][0] == "-1")
                                strArrayWeek[nDx][0] = strUseNoday;
                            else if ((strArrayWeek[nDx][0]?.Length ?? 0) > 1 && strArrayWeek[nDx][0][0] == '!')
                            {
                                strTmpData2 = strArrayWeek[nDx][0].Substring(1);
                                strTmpData2 += strTimeConvertMsg;

                                strArrayWeek[nDx][0] = strTmpData2;
                            }
                            // time Data 1개는 그대로 사용
                        }
                        else if ((strArrayWeek[nDx]?.Length ?? 0) > 1)
                        {
                            string strTmpData = "";
                            foreach (string strTime in strArrayWeek[nDx])
                            {
                                if (string.IsNullOrEmpty(strTmpData) == false)
                                    strTmpData += ",";

                                if ((strTime?.Length ?? 0) > 1 && strTime[0] == '!') //  && nMode == 1
                                {
                                    strTmpData2 = strTime.Substring(1);
                                    strTmpData2 += strTimeConvertMsg;

                                    strTmpData += strTmpData2;
                                }
                                else
                                    strTmpData += strTime;
                            }

                            if (string.IsNullOrEmpty(strTmpData) == false)
                                strArrayWeek[nDx][0] = strTmpData;
                            else
                                throw new Exception("FileTrans Week Time Data, Any Empty!");
                        }

                    } // for (int nDx = 0; ; nDx++)

                    strDisplayOutData = string.Format(strDisplay,
                        strArrayWeek[0][0], strArrayWeek[1][0], strArrayWeek[2][0], strArrayWeek[3][0],
                        strArrayWeek[4][0], strArrayWeek[5][0], strArrayWeek[6][0], strArrayWeek[7][0]);

                    bRet = true;

                } // if (nMode == 1 || nMode == 2)

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetFileTransWeekTimeDisplayData, Exception-MSG:{ex.Message}");
            }

            return bRet;
        }


        /// <summary>
        /// 현재(서버시간 기준) 자료전송을 사용할 수 있는 시간대인지 유무를 확인하는 함수
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileTransForTime(DateTime nowTime)
        {
            try
            {
                bool todayIsHolidy = GetTodayIsHoliday(); //금일이 휴일(사내휴일&공휴일) 인지 여부
                string dayOfWeekString = (todayIsHolidy) ? "holiday" : nowTime.DayOfWeek.ToString().ToLower();

                List<string> todayPolicy = GetTagDataList("user_policy", "file_upload_time", "schedule", dayOfWeekString);

                if (todayPolicy == null || todayPolicy.Count < 1)
                {
                    Log.Logger.Here().Error($"Today file transfer time Policy is Empty! (Day of Week-{dayOfWeekString}");
                    return false;
                }

                Log.Logger.Here().Information($"File Upload Time: Day of Week-{dayOfWeekString}, Today Policy : {string.Join(", ", todayPolicy)}");

                //type: array
                //-1:사용 불가, 0:하루 종일 사용, A~B:A시부터 B시까지 사용
                //[{9~12}, {13~18}]
                if (todayPolicy.Contains("0"))  //하루 종일 사용
                    return true;

                if (todayPolicy.Contains("-1"))  //사용 불가
                    return false;

                string hourString = nowTime.ToString("HH");
                int nowHour = int.Parse(hourString);

                foreach (string timePeriod in todayPolicy)
                {
                    //9~12
                    //13~18
                    if (!timePeriod.Contains("~"))
                        continue;

                    string start = timePeriod.Split("~")[0];
                    string end = timePeriod.Split("~")[1];

                    if (!int.TryParse(start, out int startHour))    //시작 시간 미기입되었다면 0시간부터
                        startHour = 0;
                    if (!int.TryParse(end, out int endHour))        //종료 시간 미기입되었다면 23시간까지
                        endHour = 23;

                    if (startHour <= nowHour && startHour >= nowHour)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetUseFileTransForTime Exception - {ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 파일 일일 전송 가능 최대 Size 제한 정보를 반환한다.
        /// </summary>
        /// <returns>파일 일일 전송 가능 최대 Size 제한 정보.(MB 단위)</returns>
        public Int64 GetDayFileTransferLimitSize()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_daily", "total_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }

        /// <summary>
        /// 파일 일일 전송 횟수 제한 정보를 반환
        /// </summary>
        /// <returns>파일 일일 전송 횟수 제한 정보</returns>
        public int GetDayFileTransferLimitCount()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_daily", "total_number");
            if (strData.Equals(""))
                return 0;
            int Count = Convert.ToInt32(strData);
            return Count;
        }

        /// <summary>
        /// 클립보드 일일 전송 횟수 제한 정보를 반환
        /// </summary>
        /// <returns>클립보드 일일 전송 횟수 제한 정보</returns>
        public int GetDayClipboardLimitCount()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_daily", "clipboard_total_number");
            if (strData.Equals(""))
                return 0;
            int Count = Convert.ToInt32(strData);
            return Count;
        }

        /// <summary>
        /// 클립보드 일일 전송 가능한 최대 SIZE 제한 정보를 반환한다.
        /// </summary>
        /// <returns>클립보드 일일 전송 가능한 최대 SIZE 제한 정보(MB 단위)</returns>
        public Int64 GetDayClipboardLimitSize()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_daily", "clipboard_total_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }

        /// <summary>
        /// 한번에 전송 가능한 클립보드 최대 Size 제한 정보를 반환
        /// </summary>
        /// <returns>한번에 전송 가능한 클립보드 최대 Size 제한 정보(MB 단위)</returns>
        public Int64 GetClipboardLimitSize()
        {
            string strData = GetTagData("user_policy", "transfer_policy", "allowed_one_time", "clipboard_total_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }


        /// <summary>
        /// 파일 일일 이미 전송한 Size 정보를 반환한다. 
        /// </summary>
        /// <returns>MB 단위</returns>
        public Int64 GetDayFileTransferUsedSize()
        {
            string strData = GetTagData("used_transfer", "used_daily", "total_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }

        /// <summary>
        /// 파일 일일 이미 전송한 횟수 정보를 반환
        /// </summary>
        /// <returns>MB 단위</returns>
        public int GetDayFileTransferUsedCount()
        {
            string strData = GetTagData("used_transfer", "used_daily", "total_number");
            if (strData.Equals(""))
                return 0;
            int Count = Convert.ToInt32(strData);
            return Count;
        }

        

        /// <summary>
        /// 클립보드 일일 이미 전송한 SIZE 정보를 반환한다.
        /// </summary>
        /// <returns>(MB 단위)</returns>
        public Int64 GetDayClipboardUsedSize()
        {
            string strData = GetTagData("used_transfer", "used_daily", "clipboard_total_size");
            if (strData.Equals(""))
                return 0;
            Int64 size = Convert.ToInt64(strData);
            return size;
        }

        /// <summary>
        /// 클립보드 일일 이미 전송한 횟수 정보를 반환
        /// </summary>
        /// <returns>MB 단위</returns>
        public int GetDayClipboardUsedCount()
        {
            string strData = GetTagData("used_transfer", "used_daily", "clipboard_total_number");
            if (strData.Equals(""))
                return 0;
            int Count = Convert.ToInt32(strData);
            return Count;
        }
        /// <summary>
         /// 다운로드 가능한 횟수를 반환(여러번 다운로드에서 사용)
         /// </summary>
         /// <returns>다운로드 가능한 횟수</returns>
        public int GetMaxDownCount()
        {
            string strData = GetTagData("user_policy", "download_limit_count");
            if (!int.TryParse(strData, out int count))
                return 0;
            else return count;
        }

        /// <summary>
        /// 환경변수 HSZDEFAULTOPTION 값을 10진수로 반환한다.
        /// </summary>
        /// <returns>HSZDEFAULTOPTION 값을 10진수</returns>
        public int GetHszDefaultDec()
        {
            //긴파일명, utf8은 기본사용 + 64Bit도 기본사용
            int iHszOpt = (10 + 1 + 2);

            //압축유무, 암호화유무 설정값
            string compress = GetTagData("user_policy", "hsz_default_option", "compress");
            string encode = GetTagData("user_policy", "hsz_default_option", "encode");

            if (compress == "1")
                iHszOpt += 4;
            if (encode == "1")
                iHszOpt += 8;

            return Convert.ToInt32(iHszOpt.ToString(), 16);
        }

        /// <summary>
        /// 대결재 사용 방식에 대해 반환 (무조건 2반환)
        /// </summary>
        /// <returns>2</returns>
        public int GetApproveTypeSFM() => 2;

        /// <summary>
        /// 서버에서 바이러스검사를 하는지 유무(INTERLOCKFLAG 값 받은걸로 확인)
        /// </summary>
        /// <returns></returns>
        public bool GetServerVirusExam()
        {
            //TODO 고도화 - UI에서 필요한지 프로세스 확인 필요
            //string strData = GetTagData("INTERLOCKFLAG");
            //int nValue = 0;
            //if (!strData.Equals(""))
            //    nValue = Convert.ToInt32(strData);

            //if ((nValue & 2) > 0)
            //    return true;
            //else
            return false;
        }

        /// <summary>
        /// [사용안함] PCURL 사용 유무를 반환한다.
        /// </summary>
        /// <returns>false</returns>
        public bool GetPCURLUse() => false;

        public string GetSystemPeriod()
        {
            //TODO 고도화 - UI에서 필요한지 프로세스 확인 필요
            string strPeriod = GetTagData("SYSTEMPERIOD");
            return strPeriod;
        }


        /// <summary>
        /// 현재 서버에서 설정한 휴일 인지 정보를 얻는다. (user_policy.holiday)
        /// </summary>
        /// <returns></returns>
        public bool GetTodayIsHoliday()
        {
            string strHoliday = GetTagData("user_policy", "holiday");
            return (strHoliday == "1");
        }

        /// <summary>
        /// 사후결재를 사용할 수 있는 상태(checkBox View)인지 유무(By:서버정책) 
        /// </summary>
        /// <returns>true : 사용못함, false : 사용함</returns>
        public bool GetAfterChkHide(DateTime nowTime)
        {
            try
            {
                //string strPolicy = GetTagData("FILEUPLOADTIME");
                bool todayIsHolidy = GetTodayIsHoliday(); //금일이 휴일(사내휴일&공휴일) 인지 여부
                string dayOfWeekString = (todayIsHolidy) ? "holiday" : nowTime.DayOfWeek.ToString().ToLower();

                List<string> todayPolicy = GetTagDataList("user_policy", "file_upload_time", "schedule", dayOfWeekString);

                if (todayPolicy == null || todayPolicy.Count < 1)
                {
                    Log.Logger.Here().Error($"Today file transfer time Policy is Empty! (Day of Week-{dayOfWeekString}");
                    return true;
                }

                Log.Logger.Here().Information($"File Upload Time: Day of Week-{dayOfWeekString}, Today Policy : {string.Join(", ", todayPolicy)}");

                //type: array
                //-1:사용 불가, 0:하루 종일 사용, A~B:A시부터 B시까지 사용
                //[{9~12}, {13~18}]
                if (todayPolicy.Contains("0"))  //하루 종일 사용
                    return false;

                if (todayPolicy.Contains("-1"))  //사용 불가
                    return true;

                string hourString = nowTime.ToString("HH");
                int nowHour = int.Parse(hourString);

                foreach (string timePeriod in todayPolicy)
                {
                    //9~12
                    //13~18
                    if (!timePeriod.Contains("~"))
                        continue;

                    string start = timePeriod.Split("~")[0];
                    string end = timePeriod.Split("~")[1];

                    if (!int.TryParse(start, out int startHour))    //시작 시간 미기입되었다면 0시간부터
                        startHour = 0;
                    if (!int.TryParse(end, out int endHour))        //종료 시간 미기입되었다면 23시간까지
                        endHour = 23;

                    if (startHour <= nowHour && startHour >= nowHour)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetAfterChkHide Exception - {ex.ToString()}");
                return true;
            }
        }

        /// <summary>
        /// param  dt: 기준 시각,
        /// 사후결재를 사용 가능 유무(By:서버정책, true : 사용, false : 사용못함) 
        /// </summary>
        public bool GetUseAfterApprove(DateTime dt)
        {
            return !GetAfterChkHide(dt);
        }

        /// <summary>
        /// 서버에 설정된 결재유형을 알려준다.(0:AND, 1:OR, 2:ANDOR)
        /// </summary>
        /// <returns>서버에 설정된 결재유형값(0:AND, 1:OR, 2:ANDOR)</returns>
        public int GetApproveStep()
        {
            string strData = GetTagData("user_policy", "approval_policy", "approve_step");
            if (strData == "and")
                return 0;
            if (strData == "or")
                return 1;
            if (strData == "andor")
                return 2;
            return 0;
        }

        /// <summary>
        /// 환경변수 HSZDEFAULTOPTION 값을 반환한다.
        /// </summary>
        /// <returns>환경변수 HSZDEFAULTOPTION 값</returns>
        public string GetPassWDExpiredDay()
        {
            //TODO 고도화 - UI에서 필요한지 프로세스 확인 필요
            string strData = GetTagData("PASSWDEXPIREDDAYS");
            return strData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void AddRunSystemEnvData(SGData data)
        {
            //TODO 고도화 - UI에서 필요한지 프로세스 확인 필요
            AddRunSystemData("HSZDEFAULTOPTION", data);          // 긴파일, 압축, 암호화 지원여부
            AddRunSystemData("INTERLOCKEMAIL", data);            // 이메일용 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
            AddRunSystemData("APPROVETYPESFM", data);            // 대결재 방식(1:고정, 2:유동)
            if (GetPCURLUse() == true)
                AddRunSystemData("PCURLHTTPPROXY", data);            // PCURLHTTPPROXY 설정 정보

            AddRunSystemData("INTERLOCKFLAG", data);               // 서버 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)

            AddRunSystemData("PASSWDEXPIREDDAYS", data);
            AddRunSystemData("PASSWDEXPIREDMETHOD", data);
            AddRunSystemData("LOGINTYPE", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="data"></param>
        public void AddRunSystemData(string strKey, SGData data)
        {
            string strValue = data.GetSystemRunTagData(strKey);
            EncAdd(strKey, strValue);
        }

        /// <summary>
        /// 압축파일 내부검사 Depth
        /// </summary>
        /// <returns></returns>
        public int GetZipDepthCount()
        {
            string count = GetTagData("user_policy", "zip_depth", "depth_count");

            if (!int.TryParse(count, out int DepthCount))
                return 3;
            return DepthCount;
        }

        /// <summary>
        /// 압축파일 내부검사가 모두 완료후에도 압축파일이 잔여할 경우, 처리 타입
        /// </summary>
        /// <returns>0: 내부에 압축이 발견되면 차단 / 1: 허용</returns>
        public int GetZipDepthRemainProcType()
        {
            string procType = GetTagData("user_policy", "zip_depth", "reamaining_zip_proc_type");

            if (!int.TryParse(procType, out int typeCode))
                return 1;
            return typeCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTagName"></param>
        /// <returns></returns>
        public string GetTagDataBySystemEnvName(string strTagName)
        {
            string strRet = "";
            string strTagRealName = "";
            strTagRealName = GetSystemPosition() ? "I_" : "E_";
            strTagRealName += strTagName.ToUpper();
            strRet = GetTagData(strTagRealName);
            return strRet;
        }

        /// <summary>
        /// 서버 ENV테이블에 설정된 문서파일 내부 검사유형 정보를 반환한다. (서버ENV 테이블에서 GET)
        /// <br>0 : 문서 검사 안함.</br>
        /// <br>1 : 모듈검사 AND OLE객체 마임리스트검사.</br>
        /// <br>2 : 모듈검사 AND  위변조 체크</br>
        /// <br>3 : 모듈검사 AND OLE마임리스트검사 AND 위변조체크</br>
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetDocumentExtractType(bool bSystem)
            => (bSystem) ? GetTagData("I_CLIENT_OLE_EXTRACT") : GetTagData("E_CLIENT_OLE_EXTRACT");

        public string GetPreViewerExt()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_PREVIEW_VIEWER_EXT") : GetTagData("E_CLIENT_PREVIEW_VIEWER_EXT");

        public string GetDocumentExtractExt()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_OLE_EXTRACT_EXT") : GetTagData("E_CLIENT_OLE_EXTRACT_EXT");
        public string GetForwardAutoDown()
            => (GetSystemPosition()) ? GetTagData("I_FORWARD_AUTODOWN") : GetTagData("E_FORWARD_AUTODOWN");
        public string GetBinaryCheckType(bool bSystem)
            => (bSystem) ? GetTagData("I_CLIENT_BINARY_CHECK") : GetTagData("E_CLIENT_BINARY_CHECK");
        public string GetBinaryCheckExt()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_BINARY_CHECK_EXT") : GetTagData("E_CLIENT_BINARY_CHECK_EXT");
        public string GetBinaryInnerCheck(bool bSystem)
            => (bSystem) ? GetTagData("I_CLIENT_CHECK_PE") : GetTagData("E_CLIENT_CHECK_PE");
        /// <summary>
        /// 제목 최대 길이 값
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetTitleMaxLength()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_TITLE_MAX_LENGTH") : GetTagData("E_CLIENT_TITLE_MAX_LENGTH");

        /// <summary>
        /// 제목 최소 길이 값
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetTitleMinLength()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_TITLE_MIN_LENGTH") : GetTagData("E_CLIENT_TITLE_MIN_LENGTH");

        /// <summary>
        /// 설명 최대 길이 값
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetDescMaxLength()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_DESC_MAX_LENGTH") : GetTagData("E_CLIENT_DESC_MAX_LENGTH");

        /// <summary>
        /// 설명 최소 길이 값
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetDescMinLength()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_DESC_MIN_LENGTH") : GetTagData("E_CLIENT_DESC_MIN_LENGTH");

        /// <summary>
        /// 개인정보 검출 시 차단 문구에 추가할 문구
        /// </summary>
        /// <param name="bSystem"></param>
        /// <returns></returns>
        public string GetPrivacyComment()
            => (GetSystemPosition()) ? GetTagData("I_CLIENT_PRIVACY_COMMENT") : GetTagData("E_CLIENT_PRIVACY_COMMENT");

        /// <summary>
        /// 패스워드 변경 유무 또는 변경 타입을 반환한다.
        /// </summary>
        /// <returns>패스워드 변경 유무 또는 변경 타입.</returns>
        public ePassWDChgType GetPasswordExpired()
        {
            ePassWDChgType ePassType = ePassWDChgType.eNone;
            string strData = GetTagData("user_policy", "pw_expire_noti_type");
            switch (strData)
            {
                case "0":
                    ePassType = ePassWDChgType.eNone;
                    break;
                case "1":
                    ePassType = ePassWDChgType.eAfterward;
                    break;
                case "2":
                    ePassType = ePassWDChgType.eEnforce;
                    break;
                default:
                    ePassType = ePassWDChgType.eNone;
                    break;
            }
            return ePassType;
        }

        /// <summary>
        /// 환경설정 변수(RUNTIME) 패스워드 변경 타입을 반환한다.
        /// </summary>
        /// <returns>환경설정 변수(RUNTIME) 패스워드 변경 타입.</returns>
        public ePassWDChgType GetPasswordExpiredMethodSystemRunEnv()
        {
            //TODO 고도화 - UI에서 필요한지 프로세스 확인 필요
            ePassWDChgType ePassType = ePassWDChgType.eNone;
            string strData = GetTagData("PASSWDEXPIREDMETHOD");

            if ((strData.Equals("")) || (strData == null))
                return ePassType;
            int nValue = Convert.ToInt32(strData);

            switch (nValue)
            {
                case 0:
                    ePassType = ePassWDChgType.eNone;
                    break;
                case 1:
                    ePassType = ePassWDChgType.eEnforce;
                    break;
                case 2:
                    ePassType = ePassWDChgType.eAfterward;
                    break;
                default:
                    ePassType = ePassWDChgType.eNone;
                    break;
            }
            return ePassType;
        }

        /// <summary>
        /// 패스워드 변경하지 않은 날짜 정보를 반환한다.
        /// </summary>
        /// <returns>패스워드 변경하지 않은 날짜 정보</returns>
        public int GetPasswordExpiredDay()
        {
            string strData = GetTagData("user_policy", "pw_not_changed_days");
            if (!int.TryParse(strData, out int days))
                return 0;
            else
                return days;
        }


        ///// <summary>
        ///// 문서형식 검사 시 참고할 확장자 타입
        ///// <para>DOCUMENTFILEFILTERTYPE</para>
        ///// </summary>
        ///// <returns></returns>
        //public bool GetDocumentFileFilterType()
        //{
        //    string strData = GetTagData("DOCUMENTFILEFILTERTYPE");
        //    if (strData.Equals("W"))
        //        return true;
        //    else
        //        return false;
        //}

        ///// <summary>
        ///// 문서형식 검사 시 참고할 확장자명
        ///// <para>DOCUMENTFILEFILTER</para>
        ///// </summary>
        ///// <returns></returns>
        //public string GetDocumentFileFilter()
        //{
        //    string strData = GetTagData("DOCUMENTFILEFILTER");
        //    if ((strData.Equals("") == true) || (strData.Equals("HS_ALL_FILE") == true))
        //        return ";";

        //    return strData;
        //}

        /// <summary>
        /// 3망 정책값을 받아서 상세하게 설정
        /// </summary>
        /// <param name="DataNet"></param>
        /// <returns></returns>
        public bool SetPolicyDataWithParsing(ref SGNetOverData DataNet)
        {
            uint nPolicyVal = 0;

            if (DataNet.strPolicy.Length < 1)
                return false;

            nPolicyVal = Convert.ToUInt32(DataNet.strPolicy);

            if ((nPolicyVal & 0b_00001) > 0)
                DataNet.bUseFileTrans = true;

            if ((nPolicyVal & 0b_00010) > 0)
                DataNet.bUseClipTrans = true;

            if ((nPolicyVal & 0b_00100) > 0)
                DataNet.bUseUrlTrans = true;

            if ((nPolicyVal & 0b_01000) > 0)
                DataNet.bUseApprove = true;

            if ((nPolicyVal & 0b_10000) > 0)
                DataNet.bUseinterlock = true;

            return true;
        }

        /// <summary>
        /// 3망 전송기능 사용유무
        /// </summary>
        /// <returns></returns>
        public bool GetUseOverNetwork2()
        {
            List<string> strData = GetTagDataList("user_login", "user_policy", "net_over_mode_list");
            if (strData?.Count >= 3)
            {
                CLog.Here().Information($"NETOVERMODE(Get from Server-###) : {string.Join(",", strData)}");
                return true;
            }
            else
                return false;

            ////CLog.Here().Information("NETOVERMODE(Get from Server-###) : {0}", strData);
            ////strData = "0";	// (3중망연계,사용하지 않는다고 판단)
            //// strData = "단말망,I001/업무망,E001,31/인터넷망,E101,28";	// ex) (인터넷망, 파일/클립보드 사용안함)
            ////strData = "단말망,I001/업무망,E001,31/인터넷망,E101,31";  // ex) (인터넷망, 전부 사용)
            //if (strData == null || strData.Length == 0 || strData == "0")
            //    return false;

            //if (strData.Length < 6)
            //    return false;

            //String[] listNetOver2 = strData.Split("/");
            //if (listNetOver2.Count() < 3)
            //    return false;

            //return true;
        }

        /// <summary>
        /// 목적지망 정보를 가지고 오는 함수
        /// </summary>
        /// <param name="dicSysIdName">망이름, 망정책정보  형태의 Dic Data 받아옴</param>
        /// <param name="bIsMultiNetWork"></param>
        /// <returns></returns>
        public Dictionary<string, SGNetOverData> GetOverNetwork2Data(bool exceptMe = true)
        {
            Dictionary<string, SGNetOverData> retValue = new Dictionary<string, SGNetOverData>();
            try
            {
                foreach (SGNetOverData dest in GetDestinationInfo(exceptMe))
                {
                    retValue.Add(dest.strDestSysName, dest);
                }
            }
            catch (Exception e)
            {
                CLog.Here().Information("GetOverNetwork2Data-Exception(Msg) : {0}", e.Message);
            }
            return retValue;
        }


        /// <summary>
        /// 로그인할때 받은 EXCEPTIONEXT 값을 받음
        /// </summary>
        /// <returns></returns>
        public string GetExceptionExt()
        {
            List<string> strData = GetTagDataList("user_policy", "exception_handling", "approve_policy", "ext_list");

            if (strData == null || strData.Count < 1)
                return ";";
            else
                return string.Join(",", strData);
        }

        public bool GetURLRedirect()
        {
            //택 1: UNUSE:미사용, IN->EX:내부->외부, IN<-EX:내부<-외부
            string strData = GetTagData("user_policy", "url_redirection");
            if (strData == "UNUSE")
                return false;
            if (strData == "IN->EX" && MySgNetType == "IN")
                return true;
            else if (strData == "EX->IN" && MySgNetType == "EX")
                return true;
            else
                return false;
        }
        /**
      *@breif url redirection 사용 type 정보
      *@return 0 : error, 1 : White, 2 : Black
       */
        public eUrlType GetURLuseType()
        {
            string strData = GetTagData("mime_ole_url_info", "redirection_url", "filter_type");
            if (strData.Equals(""))
                return eUrlType.eNone;

            strData = strData.ToUpper();

            if (strData == "WHITE")
                return eUrlType.eWhiteList;
            else
                return eUrlType.eBlackList;
        }

        /**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
        public int GetURLlistCount()
        {
            List<string> strData = GetTagDataList("mime_ole_url_info", "redirection_url", "list");

            if (strData == null)
                return 0;
            else
                return strData.Count;
        }

        /**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
        public bool GetURLlist(ref List<string> listUrlData)
        {
            int nIdx = 0;
            List<string> listurl = GetTagDataList("mime_ole_url_info", "redirection_url", "list");

            if (listurl == null)
                return false;

            listUrlData.Clear();
            for (; nIdx < listurl.Count(); nIdx++)
            {
                listUrlData.Add(listurl[nIdx]);
            }

            return (listUrlData.Count > 0);
        }

        /**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
        public int GetURLexceptionlistCount()
        {
            List<string> strData = GetTagDataList("mime_ole_url_info", "redirection_url", "exception_list");

            if (strData == null)
                return 0;
            else
                return strData.Count;
        }

        /**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
        public bool GetURLexceptionlist(ref List<string> listUrlData)
        {
            //int nCount = GetURLexceptionlistCount();
            int nIdx = 0;
            List<string> listurl = GetTagDataList("mime_ole_url_info", "redirection_url", "exception_list");

            if (listurl == null)
                return false;

            listUrlData.Clear();
            for (; nIdx < listurl.Count(); nIdx++)
            {
                listUrlData.Add(listurl[nIdx]);
            }

            return (listUrlData.Count > 0);
        }

        /// <summary>
        /// 나를 대결재자로 부여한 사용자 목록
        /// </summary>
        /// <returns></returns>
        public List<(string Name, string Rank, int Right)> GetApproverWhoGaveMeProxyList()
        {
            List<object> listData = GetTagDataObjectList("approve_line", "approver_who_gave_me_proxy_list");
            List<(string name, string rank, int right)> ret = new List<(string name, string rank, int right)>();
            if (listData == null || listData.Count <= 0)
            {
                return ret;
            }
            else
            {
                foreach (object item in listData)
                {
                    JObject jObjItem = (JObject)item;
                    Dictionary<string, object> person = jObjItem.ToObject<Dictionary<string, object>>();
                    string name = person.GetTagData("approver_hr", "name");
                    string rank = person.GetTagData("approver_hr", "rank");
                    int.TryParse(person.GetTagData("approver_type", "authority"), out int right);

                    ret.Add((name, rank, right));
                }
                return ret;

            }
        }

        public void SetNewUserPolicy(SGData newPolicy)
        {
            //정책 업데이트 갱신
            m_DicTagData["user_policy"] = newPolicy.m_DicTagData["user_policy"];
            m_DicTagData["mime_ole_url_info"] = newPolicy.m_DicTagData["mime_ole_url_info"];
            m_DicTagData["agent_patch_policy"] = newPolicy.m_DicTagData["agent_patch_policy"];
            m_DicTagData["approve_line"] = newPolicy.m_DicTagData["approve_line"];
        }
    }
}


