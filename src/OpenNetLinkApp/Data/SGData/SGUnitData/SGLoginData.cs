using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Models.Data
{
    public class SGLoginData : SGData
    {
        public SGLoginData()
        {

        }
        
        ~SGLoginData()
        {

        }
        public void Copy(SGData data)
        {
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
    }
}
