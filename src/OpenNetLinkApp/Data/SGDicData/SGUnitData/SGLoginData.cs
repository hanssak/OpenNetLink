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

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
	public enum eIDPWresultJob
	{
		eNone = 0,
		eEnforced = 1,
		eAfterWard = 2,
		eInitPW = 3
	}

	public enum ePassWDChgType
    {
		eNone = 0,
		eEnforce = 1,
		eAfterward = 2
    }

	public enum eUrlType
	{
		eNone = 0,
		eWhiteList = 1,
		eBlackList = 2
	}

	public class SGUrlListData : SGData
	{

		private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGUrlListData>();

		public SGUrlListData()
		{

		}

		~SGUrlListData()
		{

		}

		override public void Copy(HsNetWork hs, SGData data)
		{
			SetSessionKey(hs.GetSeedKey());
			m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
			m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
		}

		/**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
		public eUrlType GetURLuseType()
        {
			string strData = GetBasicTagData("URLUSETYPE"); // GetBasicTagData - GetTagData - GetEncTagData
			if (strData.Equals(""))
				return eUrlType.eNone;

			//int nType = Convert.ToInt32(strData);
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
			string strData = GetBasicTagData("URLLISTCOUNT");
			if (strData.Equals(""))
				return 0;
			int nCount = Convert.ToInt32(strData);
			return nCount;
		}

		/**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
		public bool GetURLlist(ref List<string> listUrlData)
		{
			int nCount = GetURLlistCount();
			int nIdx = 0;
			string strData = GetEncTagData("URLLIST");

			String[] listurl = strData.Split("\r\n");

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
			string strData = GetBasicTagData("URL_EXCEPTION_LIST_COUNT");
			if (strData.Equals(""))
				return 0;
			int nCount = Convert.ToInt32(strData);
			return nCount;
		}

		/**
		*@breif url redirection 사용 type 정보
		*@return 0 : error, 1 : White, 2 : Black
		 */
		public bool GetURLexceptionlist(ref List<string> listUrlData)
		{
			//int nCount = GetURLexceptionlistCount();
			int nIdx = 0;
			string strData = GetEncTagData("URL_EXCEPTION_LIST");

			String[] listurl = strData.Split("\r\n");

			if (listurl == null)
				return false;

			listUrlData.Clear();
			for (; nIdx < listurl.Count(); nIdx++)
			{
				listUrlData.Add(listurl[nIdx]);
			}

			return (listUrlData.Count > 0);
		}

	}


	public class SGNetOverData
    {

		public string strDestSysid = "";
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

	public class SGLoginData : SGData
	{
		private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGLoginData>();

		public SGLoginData()
		{

		}

		~SGLoginData()
		{

		}
		override public void Copy(HsNetWork hs, SGData data)
		{
			SetSessionKey(hs.GetSeedKey());
			m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
			m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
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

				default: //사용자 인증에 실패
					strLoginFailMsg = xmlConf.GetWarnMsg("W_0037");                                         // 사용자 인증에 실패 하였습니다.
					break;
			}
			return strLoginFailMsg;
		}
		/**
		*@breif 접속망에대한 정보를 리턴한다.
		*@return 0:업무-인터넷망 1:운영-업무망
		 */
		public int GetConnNetwork()
		{
			string strData = GetTagData("CONNNETWORK");
			if (strData.Equals(""))
				return 0;
			int nSysID = Convert.ToInt32(strData);
			return nSysID;
		}

		/**
		*@breif 접속망에대한 정보를 문자열로 리턴한다.
		*@return 접속망에대한 정보
		 */
		public string GetConnNetworkString()
		{
			string strData = GetTagData("CONNNETWORK");
			return strData;
		}

		/**
		*@breif 접속망에대한 시스템 ID 정보를 리턴한다.
		*@return I or E (내부/외부)
		 */
		public string GetSysID()
		{
			string strSysID = "I";
			int nConnNetWork = GetConnNetwork();
			if (nConnNetWork == 0)
				strSysID = "E";
			return strSysID;
		}
		/**
		 * @breif Link Check Time 주기를 반환한다.
		 * @return 초단위
		 */
		public int GetLinkCheckTime()
		{
			string strData = GetTagData("LINKCHECKTIME");
			if (strData.Equals(""))
				return 0;
			int size = Convert.ToInt32(strData);
			return size;
		}
		/**
		 * @breif 수신파일 폴더 삭제 주기를 반환한다.
		 * @return 일 단위
		 */
		public int GetFileRemoveCycle()
		{
			string strData = GetTagData("DELETECYCLE");
			if (strData.Equals(""))
				return 0;
			int size = Convert.ToInt32(strData);
			return size;
		}
		/**
		 * @breif 한번에 전송가능한 파일의 최대 사이즈를 반환한다.
		 * @return MB 단위
		 */
		public Int64 GetFileLimitSize()
		{
			string strData = GetTagData("FILELIMITSIZE");
			if (strData.Equals(""))
				return 0;
			Int64 size = Convert.ToInt64(strData);
			return size;
		}
		/**
		 * @breif 한번에 전송가능한 파일의 최대 개수를 반환한다.
		 * @return 전송가능한 파일의 최대 개수
		 */
		public int GetFileLimitCount()
		{
			string strData = GetTagData("MAXFILETRANSFERCOUNT");
			if (strData.Equals(""))
				return 0;
			int count = Convert.ToInt32(strData);
			return count;
		}

		/**
		 * @breif 클립보드 사용 여부를 반환한다.
		 * @return true  : 클립보드 전송 가능 
		 */
		public bool GetClipboard()
		{
			string strData = GetTagData("CLIPPOLICYFLAG");
			bool bInner = GetSystemPosition();
			int nClipPolicyFlag = 0;
			if (strData.Equals(""))
				return false;
			nClipPolicyFlag = Convert.ToInt32(strData);
			bool bResult = false;
			if (bInner == true)
			{
				if (nClipPolicyFlag == 1)
					bResult = false;
				else if (nClipPolicyFlag == 2)
					bResult = true;
			}
			else
			{
				if (nClipPolicyFlag == 1)
					bResult = true;
				else if (nClipPolicyFlag == 2)
					bResult = false;
			}

			if (nClipPolicyFlag == 3)
				bResult = true;
			else if (nClipPolicyFlag == 4)
				bResult = false;

			return bResult;
		}
		/**
		 * @breif 수동다운로드 사용 여부를 반환한다.
		 * @return true : 수동다운로드 사용.
		 */
		public bool GetManualDownload()
		{
			string strData = GetTagData("MANUALDOWNLOAD");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue != 0)
				return true;
			else
				return false;
		}
		/**
		 * @breif 결재 사용 여부를 반환한다. (3망연계 설정 사용시 3망연계 설정값도 반영함)
		 * @return true : 결재 사용.
		 */
		public bool GetApprove()
		{
			bool bRet = true;
			string strData = GetTagData("APPROVEUSETYPE");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue != 0)
				bRet = true;
			else
				bRet = false;

			return bRet;
		}


		/**
		 * @breif 결재 사용 시 결재자 편집 사용 여부를 반환한다.
		 * @return true : 결재자 편집 사용.
		 */
		public bool GetApproveAppend()
		{
			if (GetApprove() == false)	// 3망연계정보 상관없음
				return false;

			string strData = GetTagData("APPROVEPROXY");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if ((nValue == 2) || (nValue == 3))
				return true;
			else
				return false;
		}
		/**
		 * @breif 결재 사용 시 대결재자 편집 사용여부를 반환한다.
		 * @return true : 대결재자 편집 사용.
		 */
		public bool GetDeputyApprove()
		{
			if (GetApprove() == false)  // 3망연계정보 상관없음
				return false;

			string strData = GetTagData("APPROVEPROXY");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if ((nValue == 1) || (nValue == 3))
				return true;
			else
				return false;
		}
		/**
		 * @breif 파일 확장자 제한 타입을 반환한다.
		 * @return true : White List, false : Black List
		 */
		public bool GetFileFilterType()
		{
			string strData = GetTagData("FILEFILTERTYPE");
			if (strData.Equals("W"))
				return true;
			else
				return false;
		}

		/**
		 * @breif 파일 확장자 제한 정보를 반환한다.
		 * @return 파일확장자 정보.
		 */
		public string GetFileFilter()
		{
			string strData = GetTagData("FILEFILTER");
			if ((strData.Equals("") == true) || (strData.Equals("HS_ALL_FILE") == true))
			{
				SetTagData("FILEFILTER", ";");
				return ";";
			}

			return strData;
		}
		/**
		 * @breif VIP 권한을 갖고 있는 사용자인지 여부를 반환한다.
		 * @return true : VIP 사용자
		 */
		public bool IsVipUser()
		{
			string strData = GetTagData("FILEFILTER");
			if (strData.Equals("HS_ALL_FILE"))
				return true;

			return false;
		}
		/**
		 * @breif 서버명을 반환한다.
		 * @return 서버명
		 */
		public string GetServName()
		{
			string strData = GetTagData("SERVERNAME");
			return strData;
		}

		/**
		 * @breif 화면잠금 시간 정보를 반환한다.
		 * @return 분단위
		 */
		public int GetSCRLimit()
		{
			string strData = GetTagData("SCRLOCK");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			return nValue;
		}

		/**
		 * @breif 비밀번호 변경 시 복잡도 확인 여부를 반환한다.
		 * @return true : 비밀번호 복잡도 사용.
		 */
		public bool GetPasswordRule()
		{
			string strData = GetTagData("AUTHUSER");
			strData = strData.Substring(0, 1);
			if (strData.Equals("Y"))
				return true;
			return false;
		}

		/**
		 * @breif 파일 Part Size 값을 반환한다.
		 * @return KB 단위
		 */
		public Int64 GetFilePartSize()
		{
			string strData = GetTagData("FILEPARTSIZE");
			if (strData.Equals(""))
				return 0;
			Int64 size = Convert.ToInt64(strData);
			return size;
		}

		/**
		 * @breif Dummy Packet 사용 여부를 반환한다.
		 * @return true : Dummy Packet 사용.
		 */
		public bool GetUseDummyPacket()
		{
			string strData = GetTagData("DUMMYPACKETFLAG");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 1)
				return true;
			else
				return false;
		}
		/**
		*@biref 파일 전송 대역폭을 반환한다.
		*@return BPS(bit per Second) 단위
		*/
		public Int64 GetFileBandWidth()
		{
			string strData = GetTagData("FILEBANDWIDTH");
			if (!strData.Equals(""))
				return 0;
			Int64 bandwidth = Convert.ToInt64(strData);
			return bandwidth;
		}

		/**
		*@biref 현재 내부망에 접속되어 있는지 여부를 반환한다.
		*@return true 내부, false 외부
		*/
		public bool GetSystemPosition()
		{
			string strData = GetTagData("SYSTEMTYPE");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 1)
				return true;
			return false;
		}
		/**
		*@biref URL 리다이렉션 사용 여부를 반환한다.
		*@return BPS(bit per Second) 단위
		*/
		public bool GetURLRedirect()
		{
			string strData = GetTagData("URLREDIRECTION");
			bool bInner = GetSystemPosition();
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if ((nValue == 1) && (bInner == true))
				return true;
			else if ((nValue == 2) && (bInner == false))
				return true;
			else
				return false;
		}
		/**
		*@biref 서버에 업데이트 대기 중인 Client Version 을 반환한다.
		*@return Client Version 정보. ( ex) NetLink 2.03 )
		*/
		public string GetServClientVersion()
		{
			string strData = GetTagData("CLIENTVERSION");
			return strData;
		}
		/**
		*@biref 서버에 업데이트 대기 중인 Client 패치 파일 존재 여부를 반환한다.
		*@return true : Client 패치파일 존재.
		*/
		public bool GetClientUpgrade()
		{
			string strData = GetTagData("CLIENTUPGRADE");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 1)
				return true;
			return false;
		}

		/**
		*@biref 일반 사용자도 대결재 권한 및 결재 권한이 있는지 여부를 반환한다.
		*@return true : 대결재 권한 및 결재 권한 존재.
		*/
		public bool GetApproveProxyRight()
		{
			string strData = GetTagData("APPROVEPROXYRIGHT");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 2)
				return true;
			return false;
		}
		/**
		*@biref 파일 전송 사용 여부를 확인한다..
		*@return true : 파일 전송 가능.
		*/
		public bool GetFileTrans()
		{
			string strData = GetTagData("POLICYFLAG");
			bool bInner = GetSystemPosition();

			//1:반입허용, 2:반출허용, 3:전체허용, 4:전체금지
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 3)
				return true;
			else if (nValue == 4)
				return false;

			if (bInner == true)
			{
				if (nValue == 2)
					return true;
				else if (nValue == 1)
					return false;
				else
					return false;
			}
			else
			{
				if (nValue == 2)
					return false;
				else if (nValue == 1)
					return true;
				else
					return false;
			}
		}

		/**
		*@biref OTP 번호 발급 가능 여부를 반환한다.
		*@return true : OTP 번호 발급 가능.
		*/
		public bool GetCreateOtpNo()
		{
			string strData = GetTagData("OTP");
			if (strData.Equals(""))
				strData = GetTagData("OPT");

			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue == 1)
				return true;
			return false;
		}
		/**
		*@biref 파일 일일 전송 가능 최대 Size 제한 정보를 반환한다.
		*@return 파일 일일 전송 가능 최대 Size 제한 정보.
		*/
		public Int64 GetDayFileTransferLimitSize()
		{
			string strData = GetTagData("DAYFILETRANSFERLIMITSIZE");
			if (strData.Equals(""))
				return 0;
			Int64 size = Convert.ToInt64(strData);
			return size;
		}
		/**
		*@biref 파일 일일 전송 횟수 제한 정보를 반환한다.
		*@return 파일 일일 전송 횟수 제한 정보.
		*/
		public int GetDayFileTransferLimitCount()
		{
			string strData = GetTagData("DAYFILETRANSFERLIMITCOUNT");
			if (strData.Equals(""))
				return 0;
			int Count = Convert.ToInt32(strData);
			return Count;
		}

		/**
		*@biref 클립보드 일일 전송 횟수 제한 정보를 반환한다.
		*@return 클립보드 일일 전송 횟수 제한 정보.
		*/
		public int GetDayClipboardLimitCount()
		{
			string strData = GetTagData("DAYCLIPBOARDCOUNT");
			if (strData.Equals(""))
				return 0;
			int Count = Convert.ToInt32(strData);
			return Count;
		}

		/**
		*@biref 클립보드 일일 전송 가능한 최대 SIZE 제한 정보를 반환한다.
		*@return 클립보드 일일 전송 가능한 최대 SIZE 제한 정보.
		*/
		public Int64 GetDayClipboardLimitSize()
		{
			string strData = GetTagData("DAYCLIPBOARDSIZE");
			if (strData.Equals(""))
				return 0;
			Int64 size = Convert.ToInt64(strData);
			return size;
		}

		/**
		*@biref 한번에 전송 가능한 클립보드 최대 Size 제한 정보를 반환한다.
		*@return 한번에 전송 가능한 클립보드 최대 Size 제한 정보.
		*/
		public Int64 GetClipboardLimitSize()
		{
			string strData = GetTagData("CLIPBOARDSIZE");
			if (strData.Equals(""))
				return 0;
			Int64 size = Convert.ToInt64(strData);
			return size;
		}
		/**
		*@biref 다운로드 가능한 횟수를 반환한다.
		*@return 다운로드 가능한 횟수.
		*/
		public int GetMaxDownCount()
		{
			string strData = GetTagData("DOWNLIMITCOUNT");
			if (strData.Equals(""))
				return 0;
			int count = 0;
			if (!strData.Equals(""))
				count = Convert.ToInt32(strData);
			return count;
		}

		/**
		*@biref 환경변수 HSZDEFAULTOPTION 값을 반환한다.
		*@return 환경변수 HSZDEFAULTOPTION 값
		*/
		public string GetHszDefaultOption()
		{
			string strData = GetTagData("HSZDEFAULTOPTION");
			return strData;
		}

		/**
		*@biref 환경변수 HSZDEFAULTOPTION 값을 10진수로 반환한다.
		*@return 환경변수 HSZDEFAULTOPTION 값
		*/
		public int GetHszDefaultDec()
		{
			string strData = GetHszDefaultOption();
			int len = 0;

			if (strData.Equals(""))
				strData = "00";
			else
			{
				int pos = -1;
				pos = strData.IndexOf("x");
				if (pos > 0)
					strData = strData.Substring(pos + 1 + 8, strData.Length - (pos + 1 + 8));

				len = strData.Length;
				if ((len % 2) != 0)  // 홀수 일 경우
					strData.Insert(0, "0");
			}
			len = strData.Length;
			int iHszOpt = Convert.ToInt32(strData, 16);

			return iHszOpt;
		}

		/**
		*@biref 대결재 사용 방식에 대해 반환한다.
		*@return 대결재 방식(1:고정, 2:유동)
		*/
		public int GetApproveTypeSFM()
		{
			string strData = GetTagData("APPROVETYPESFM");
			int nValue = 1;
			if (strData.Equals(""))
				return nValue;
			nValue = Convert.ToInt32(strData);
			return nValue;
		}

		/**
		*@biref 환경변수 INTERLOCKEMAIL 값을 반환한다.
		*@return 환경변수 INTERLOCKEMAIL 값
		*/
		public string GetInterLockEmail()
		{
			string strData = GetTagData("INTERLOCKEMAIL");
			return strData;
		}

		public bool GetServerVirusExam()
		{
			string strData = GetTagData("INTERLOCKFLAG");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);

			if ((nValue & 2) > 0)
				return true;
			else
				return false;
		}

		/**
		*@biref PCURL 사용 유무를 반환한다.
		*@return PCURL 사용 유무 ( true : 사용, false : 사용 안함 )
		*/
		public bool GetPCURLUse()
		{
			string strData = GetTagData("PCTOURLUSE");
			int nValue = 0;
			if (strData.Equals(""))
				return false;
			nValue = Convert.ToInt32(strData);
			if (nValue > 0)
				return true;
			return false;
		}
		/**
		*@biref 서버 시간 정보를 반환한다.
		*@return 서버 시간 
		*/
		public string GetSvrTime()
		{
			string strTime = GetTagData("SVRTIME");
			return strTime;
		}
		public DateTime GetSvrTimeDayConvert()
		{
			string strTime = GetSvrTime();
			if (strTime.Equals(""))
				return System.DateTime.Now;

			string strD = strTime.Substring(0, 8);

			string strYear = strTime.Substring(0, 4);
			string strMonth = strTime.Substring(4, 2);
			string strDay = strTime.Substring(6, 2);

			string strConvertDay = String.Format("{0}/{1}/{2}", strYear, strMonth, strDay);

			DateTime dt = Convert.ToDateTime(strConvertDay);
			return dt;
		}
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
			string strSecond = strTime.Substring(12);

			string strConvertDay = String.Format("{0}/{1}/{2}", strYear, strMonth, strDay);
			string strConvertTime = String.Format("{0}:{1}:{2}", strHour, strMinute, strSecond);

			DateTime dt = Convert.ToDateTime(strConvertDay);
			dt = Convert.ToDateTime(strConvertTime);
			return dt;

		}

		/**
		*@biref 휴일 정보를 반환한다.
		*@return 휴일정보
		*/
		public string GetHoliday()
		{
			string strHoliday = GetTagData("HOLIDAY");
			return strHoliday;
		}
		/**
		*@biref 서버 시간 정보를 반환한다.
		*@return 서버 시간 
		*/
		public string GetAfterApprove()
		{
			string strAfterApprove = GetTagData("AFTERAPPROVE");
			return strAfterApprove;
		}
		/**
		*@biref 사후결재 체크박스 숨김여부를 반환한다.
		*@return 사후결재 체크박스 
		*/
		public bool GetAfterChkHide()
		{
			bool bRet = false;
			string strAfterApprove = GetAfterApprove();
			if (strAfterApprove[0] == '0')
				bRet = true;
			else if (strAfterApprove[0] == '1')
			{
				strAfterApprove = strAfterApprove.Substring(2);
				if (strAfterApprove.Equals("none/none"))
					bRet = true;
			}
			else if (strAfterApprove[0] == '2')
			{
				bool bSubRet = true;
				strAfterApprove = strAfterApprove.Substring(2);
				string[] str1 = strAfterApprove.Split("/");
				for (int i = 0; i < str1.Length; i++)
				{
					string[] str2 = str1[i].Split(":");
					if (!str2[1].Equals("-1"))
						bSubRet = false;
				}
				bRet = bSubRet;
			}
			else
			{

			}

			return bRet;
		}
		/**
		*@biref 특정날짜의 요일값을 반환한다.
		*@return 특정날짜의 요일값. (0 : 일요일, 1: 
		*/
		private string GetDay(DateTime dt)
		{
			string strDay = "";
			switch (dt.DayOfWeek)
			{
				case DayOfWeek.Monday:
					strDay = "1";
					break;

				case DayOfWeek.Tuesday:
					strDay = "2";
					break;

				case DayOfWeek.Wednesday:
					strDay = "3";
					break;

				case DayOfWeek.Thursday:
					strDay = "4";
					break;

				case DayOfWeek.Friday:
					strDay = "5";
					break;

				case DayOfWeek.Saturday:
					strDay = "6";
					break;

				case DayOfWeek.Sunday:
					strDay = "0";
					break;

				default:
					strDay = "";
					break;

			}
			return strDay;
		}
		/**
		*@biref 특정날짜의 요일(한글)값을 반환한다.
		*@return 특정날짜의 요일(한글)값.
		*/
		private string GetDayConvertHangul(string strDay)
		{
			string strHanDay = "";
			switch (strDay)
			{
				case "0":
					strHanDay = "일";
					break;

				case "1":
					strHanDay = "월";
					break;

				case "2":
					strHanDay = "화";
					break;

				case "3":
					strHanDay = "수";
					break;

				case "4":
					strHanDay = "목";
					break;

				case "5":
					strHanDay = "금";
					break;

				case "6":
					strHanDay = "토";
					break;

				default:
					strHanDay = "";
					break;

			}
			return strHanDay;
		}
		/**
		*@biref 사후결재 설정 요일 검사 후 사용 가능 여부를 반환한다.
		*@return 사후결재 사용 가능 여부(true : 사용 가능, false : 사용 불가능)
		*/
		public bool GetWeekApprove(string strWeek, DateTime dt)
		{
			if (strWeek.Equals(""))
				return false;

			string[] strListWeek = strWeek.Split(",");

			if (strListWeek.Length <= 0)
				return false;

			string strCurDay = GetDay(dt);
			string strCurHanDay = GetDayConvertHangul(strCurDay);
			for (int i = 0; i < strListWeek.Length; i++)
			{
				if ((strListWeek[i].Equals(strCurDay)) || (strListWeek[i].Equals(strCurDay)))
					return true;
			}
			return false;
		}
		/**
		*@biref 사후결재 설정 시각 검사 후 사용 가능 여부를 반환한다.
		*@return 사후결재 사용 가능 여부(true : 사용 가능, false : 사용 불가능)
		*/
		public bool GetTimeApprove(string strTime, DateTime dt)
		{
			bool bNot = false;
			if (strTime[0] == '!')
			{
				strTime = strTime.Substring(1);
				bNot = true;
			}

			int nCurHour = dt.Hour;
			bool bRet = false;
			if (strTime.Contains(","))
			{
				string[] strTime1 = strTime.Split(",");
				for (int i = 0; i < strTime1.Length; i++)
				{
					string[] strTime2 = strTime1[i].Split("~");
					int nStartTime = 0;
					int nEndTime = 0;
					if (!strTime2[0].Equals(""))
						nStartTime = Convert.ToInt32(strTime2[0]);
					if (!strTime2[1].Equals(""))
						nEndTime = Convert.ToInt32(strTime2[1]);

					if (nStartTime > nEndTime)
						nEndTime += 24;

					if ((nCurHour >= nStartTime) && (nCurHour <= nEndTime))
					{
						bRet = true;
						break;
					}
				}
			}
			else
			{
				string[] strTime2 = strTime.Split("~");
				int nStartTime = 0;
				int nEndTime = 0;
				if (!strTime2[0].Equals(""))
					nStartTime = Convert.ToInt32(strTime2[0]);
				if (!strTime2[1].Equals(""))
					nEndTime = Convert.ToInt32(strTime2[1]);

				if (nStartTime > nEndTime)
					nEndTime += 24;

				if ((nCurHour >= nStartTime) && (nCurHour <= nEndTime))
				{
					bRet = true;
				}
			}

			if (bNot)
				bRet = !bRet;
			return bRet;
		}

		/**
		*@biref 사후결재 사용 가능 여부를 반환한다.
		*@param  dt 기준 시각
		*@return 사후결재 사용 가능 여부(true : 사용 가능, false : 사용 불가능)
		*/
		public bool GetUseAfterApprove(DateTime dt)
		{
			string strAfterApprove = GetAfterApprove();
			strAfterApprove = strAfterApprove.Replace(" ", "");
			strAfterApprove = strAfterApprove.Replace(".", ",");

			string strHoliday = GetHoliday();
			char chAfterApproveUse = strAfterApprove[0];
			strAfterApprove = strAfterApprove.Substring(2);
			if (chAfterApproveUse == '0')
				return false;
			else if (chAfterApproveUse == '1')
			{
				if (strAfterApprove.Equals("none/none"))
					return false;

				if (strHoliday.Equals("1"))
					return true;

				if (strAfterApprove.Equals("all/all"))
					return true;

				int pos = -1;
				pos = strAfterApprove.IndexOf("/");
				if (pos <= 0)
					return false;

				string strWeek = strAfterApprove.Substring(0, pos);
				string strTime = strAfterApprove.Substring(pos + 1);

				if ((strWeek.Equals("all")) || (strTime.Equals("all")))
					return true;

				if (GetWeekApprove(strWeek, dt))
					return true;
				if (GetTimeApprove(strTime, dt))
					return true;
				else
					return false;
			}
			else if (chAfterApproveUse == '2')
			{

			}
			else
			{
				return false;
			}

			return false;
		}

		/**
		*@biref PCURL 사용 유무를 반환한다.
		*@return PCURL 사용 유무 ( true : 사용, false : 사용 안함 )
		*/
		public int GetApproveStep()
		{
			string strData = GetTagData("APPROVESTEP");
			int nValue = 0;
			if (strData.Equals(""))
				return nValue;
			nValue = Convert.ToInt32(strData);
			return nValue;
		}
		/**
		*@biref PCURL PROXY 설정 정보를 반환한다.
		*@return PCURL PROXY 설정 정보.
		*/
		public string GetPCURLHTTPPROXY()
		{
			string strData = GetTagData("PCURLHTTPPROXY");
			return strData;
		}

		/**
		*@biref 환경변수 HSZDEFAULTOPTION 값을 반환한다.
		*@return 환경변수 HSZDEFAULTOPTION 값
		*/
		public string GetPassWDExpiredDay()
		{
			string strData = GetTagData("PASSWDEXPIREDDAYS");
			return strData;
		}

		public void AddRunSystemEnvData(SGData data)
		{
			AddRunSystemData("HSZDEFAULTOPTION", data);          // 긴파일, 압축, 암호화 지원여부
			AddRunSystemData("INTERLOCKEMAIL", data);            // 이메일용 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
			AddRunSystemData("APPROVETYPESFM", data);            // 대결재 방식(1:고정, 2:유동)
			if (GetPCURLUse() == true)
				AddRunSystemData("PCURLHTTPPROXY", data);            // PCURLHTTPPROXY 설정 정보

			AddRunSystemData("INTERLOCKFLAG", data);               // 서버 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)

			AddRunSystemData("PASSWDEXPIREDDAYS", data);
			AddRunSystemData("PASSWDEXPIREDMETHOD", data);
		}
		public void AddRunSystemData(string strKey, SGData data)
		{
			string strValue = data.GetSystemRunTagData(strKey);
			EncAdd(strKey, strValue);
		}
		public void AddSystemEnvData(SGData data)
		{
			AddSystemData("HSZDEFAULTOPTION", data);          // 긴파일, 압축, 암호화 지원여부
			AddSystemData("INTERLOCKEMAIL", data);            // 이메일용 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
			AddSystemData("APPROVETYPESFM", data);            // 대결재 방식(1:고정, 2:유동)
			AddSystemData("PCURLHTTPPROXY", data);            // PCURLHTTPPROXY 설정 정보
			AddSystemData("INTERLOCKFLAG", data);               // 서버 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
		}
		public void AddSystemData(string strKey, SGData data)
		{
			List<Dictionary<int, string>> listDicdata = data.GetRecordData("TAGRECORD");
			if (listDicdata == null)
				return;
			int nTotalCount = listDicdata.Count;
			for (int i = 0; i < nTotalCount; i++)                              // UI 에서 사용하기 위해 자기 자신을 포함하기 위해 i = 0 부터 시작.                  
			{
				Dictionary<int, string> dic = listDicdata[i];
				string strTmpKey = "";
				string strValue = "";
				if (dic.TryGetValue(0, out strTmpKey) == true)
				{
					strTmpKey = dic[0];
					if (strTmpKey.Equals(strKey))
					{
						if (dic.TryGetValue(1, out strValue) == true)
						{
							strValue = dic[1];
							EncAdd(strKey, strValue);
						}
					}

				}
			}
		}

		public void AddZipDepthInfo(SGData data)
		{
			string strData = data.GetBasicTagData("RECORD");

			char sep = (char)'\u0003';
			string[] strList = strData.Split(sep);

			for (int i = 0; i < strList.Length; i++)
			{
				char sep2 = (char)'\u0001';
				string[] str = strList[i].Split(sep2);
				EncAdd(str[0], str[1]);
			}
		}

		/**
		*@biref 서버로 부터 수신 받은 zip 파일 내부 검사 설정 정보를 반환한다.
		*@param bSystem ( true : 내부, false : 외부)
		*@return zip 파일 내부 검사 설정 정보.
		*/
		public string GetZipDepthInfo(bool bSystem)
		{
			string strRet = "";
			if (bSystem)
				strRet = GetTagData("I_CLIENT_ZIP_DEPTH");
			else
				strRet = GetTagData("E_CLIENT_ZIP_DEPTH");
			return strRet;
		}

		public void AddData(string strKey, string strValue)
		{
			EncAdd(strKey, strValue);
		}
		/**
		*@biref 패스워드 변경 유무 또는 변경 타입을 반환한다.
		*@return 패스워드 변경 유무 또는 변경 타입.
		*/
		public ePassWDChgType GetPasswordExpired()
		{
			ePassWDChgType ePassType = ePassWDChgType.eNone;
			//ePassWDChgType ePassType = ePassWDChgType.eEnforce;
			//ePassWDChgType ePassType = ePassWDChgType.eAfterward;
			//return ePassType;
			string strData = GetTagData("PASSWORDEXPIRED");

			char sep = (char)',';
			string[] strPassWordExpired = strData.Split(sep);
			if (strPassWordExpired.Length <= 0)
				return ePassType;

			strData = strPassWordExpired[0];
			int nValue = 0;
			if (strData.Equals(""))
				return ePassType;

			nValue = Convert.ToInt32(strData);

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

		/**
		*@biref 환경설정 변수(RUNTIME) 패스워드 변경 타입을 반환한다.
		*@return 환경설정 변수(RUNTIME) 패스워드 변경 타입.
		*/
		public ePassWDChgType GetPasswordExpiredMethodSystemRunEnv()
		{
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

		/**
		*@biref 패스워드 변경하지 않은 날짜 정보를 반환한다.
		*@return 패스워드 변경하지 않은 날짜 정보.
		*/
		public int GetPasswordExpiredDay()
		{
			string strData = GetTagData("PASSWORDEXPIRED");

			char sep = (char)',';
			string[] strPassWordExpired = strData.Split(sep);
			if (strPassWordExpired.Length <= 1)
				return 0;

			strData = strPassWordExpired[1];
			int nValue = 0;
			if (strData.Equals(""))
				return nValue;

			nValue = Convert.ToInt32(strData);
			return nValue;
		}

		/**
		*@biref 3망 정책값을 받아서 상세하게 설정
		*/
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

		/**
		*@biref 3망 전송기능 사용유무
		*/
		public bool GetUseOverNetwork2()
		{
			string strData = GetTagData("NETOVERMODE");
			//CLog.Here().Information("NETOVERMODE(Get from Server-###) : {0}", strData);
			//strData = "0";	// KKW-Sample (3중망연계,사용하지 않는다고 판단)
			// strData = "단말망,I001/업무망,E001,31/인터넷망,E101,28";	// ex) (인터넷망, 파일/클립보드 사용안함)
			//strData = "단말망,I001/업무망,E001,31/인터넷망,E101,31";  // ex) (인터넷망, 전부 사용)
			if (strData == null || strData.Length == 0 || strData == "0")
				return false;

			if(strData.Length < 6)
				return false;

			String[] listNetOver2 = strData.Split("/");
			if (listNetOver2.Count() < 3)
				return false;

			return true;
		}

		/**
		*@biref 3망 관련 정책정보를 받아오는 함수 (<망이름, 망정책정보>  형태의 Dic Data 받아옴.) 
		*/
		public bool GetOverNetwork2Data(ref Dictionary<string, SGNetOverData> dicSysIdName, bool bIsMultiNetWork)
		{
			string strData = GetTagData("NETOVERMODE");

			CLog.Here().Information("NETOVERMODE(Get from Server-###) : {0}", strData);

			// strData = "단말망,I001/업무망,E001,31/인터넷망,E101,28";	// KKW-Sample (인터넷망, 파일/클립보드 사용안함)
			//strData = "단말망,I001/업무망,E001,31/인터넷망,E101,31";	// KKW-Sample (인터넷망, 전부 사용)
			if (strData == null || strData.Length == 0 || strData == "0")
				return false;

			String[] listNetOver2 = strData.Split("/");
			if (listNetOver2.Count() < 3)
				return false;

			int nIdx = 0;   

			try
            {

				for (; nIdx < listNetOver2.Count(); nIdx++)
				{

					// 다중망 접속일때에는 3번째 망 정보는 현재 접속한쪽 반대망이 아니므로 넣지 않음.(dicSysIdName)
					// if (nIdx <= 1 || (bIsMultiNetWork == false && nIdx > 1))
					{

						String[] listOneNet = listNetOver2[nIdx].Split(",");
						if (listOneNet.Count() > 1)     // 첫번째 - 자신 제외한 타망들 정보 넣음
						{
							SGNetOverData DataNet = new SGNetOverData();

							DataNet.nIdx = nIdx;
							int nJdx = 1;
							for (; nJdx < listOneNet.Count(); nJdx++)
							{
								if (nJdx == 1)
									DataNet.strDestSysid = listOneNet[nJdx];
								else if (nJdx == 2)
								{
									DataNet.strPolicy = listOneNet[nJdx];
									SetPolicyDataWithParsing(ref DataNet);
								}
							}

							dicSysIdName.Add(listOneNet[0], DataNet);
						}

					}

				}

			}
			catch (Exception e)
            {
				CLog.Here().Information("GetOverNetwork2Data-Exception(Msg) : {0}", e.Message);
				return false;
			}

			return true;
		}



	}
}
