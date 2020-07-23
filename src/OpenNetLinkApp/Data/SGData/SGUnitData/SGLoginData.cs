using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data
{
    public class SGLoginData : SGData
    {
        public SGLoginData()
        {

        }
        
        ~SGLoginData()
        {

        }
        public void Copy(HsNetWork hs,SGData data)
        {
			SetSessionKey(hs.GetSeedKey());
			m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public static string LoginFailMessage(int nRet)
        {
            string strLoginFailMsg = "";
			XmlConfService xmlConf = new XmlConfService();
            switch(nRet)
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
			int nClipPolicyFlag = Convert.ToInt32(strData);
			bool bResult = false;
			if(bInner==true)
            {
				if (nClipPolicyFlag == 1)
					bResult = true;
				else if (nClipPolicyFlag == 2)
					bResult = false;
            }
			else
            {
				if (nClipPolicyFlag == 1)
					bResult = false;
				else if (nClipPolicyFlag == 2)
					bResult = true;
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
		 * @breif 결재 사용 여부를 반환한다.
		 * @return true : 결재 사용.
		 */
		public bool GetApprove()
        {
			string strData = GetTagData("APPROVEUSETYPE");
			int nValue = 0;
			if (!strData.Equals(""))
				nValue = Convert.ToInt32(strData);
			if (nValue != 0)
				return true;
			else
				return false;
		}
		/**
		 * @breif 결재 사용 시 결재자 편집 사용 여부를 반환한다.
		 * @return true : 결재자 편집 사용.
		 */
		public bool GetApproveAppend()
        {
			if (GetApprove() == false)
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
			if (GetApprove() == false)
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
			if((strData.Equals("")==true) || (strData.Equals("HS_ALL_FILE")==true))
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
			if ( (nValue == 1) && (bInner == true) )
				return true;
			else if ( (nValue == 2) && (bInner == false))
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
			else if(nValue==4)
				return false;

			if(bInner==true)
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
					strData = strData.Substring(pos + 1 +8, strData.Length - (pos + 1 +8));

				len = strData.Length;
				if ((len % 2) != 0)  // 홀수 일 경우
					strData.Insert(0, "0");
			}
			len = strData.Length;
			int iHszOpt = Convert.ToInt32(strData,16);			

			return iHszOpt;
		}

		/**
		*@biref 대결재 사용 방식에 대해 반환한다.
		*@return 대결재 방식(1:고정, 2:유동)
		*/
		public int GetApproveTypeSFM()
		{
			string strData = GetTagData("APPROVETYPESFM");
			int nValue = Convert.ToInt32(strData);
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

		public void AddRunSystemEnvData(SGData data)
		{
			AddRunSystemData("HSZDEFAULTOPTION", data);          // 긴파일, 압축, 암호화 지원여부
			AddRunSystemData("INTERLOCKEMAIL", data);            // 이메일용 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
			AddRunSystemData("APPROVETYPESFM", data);            // 대결재 방식(1:고정, 2:유동)
			AddRunSystemData("PCURLHTTPPROXY", data);            // PCURLHTTPPROXY 설정 정보
		}
		public void AddRunSystemData(string strKey, SGData data)
        {
			string strValue = data.GetSystemRunTagData(strKey);
			EncAdd(strKey, strValue);
			/*
			string strValue = "";
			if (data.m_DicTagData.TryGetValue(strKey, out strValue) == true)
			{
				strValue = data.m_DicTagData[strKey];
				Add(strKey, strValue);
			}
			*/
		}
		public void AddSystemEnvData(SGData data)
        {
			AddSystemData("HSZDEFAULTOPTION", data);          // 긴파일, 압축, 암호화 지원여부
			AddSystemData("INTERLOCKEMAIL", data);            // 이메일용 INTERLOCKFLAG ( DLP/DRM/VIRUS/APT)
			AddSystemData("APPROVETYPESFM", data);            // 대결재 방식(1:고정, 2:유동)
			AddSystemData("PCURLHTTPPROXY", data);            // PCURLHTTPPROXY 설정 정보
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
	}
}
