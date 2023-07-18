using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eApprManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2,
        eApprBatchError = 3,
        eApprBatchActionSuccess = 4,
        eApprBatchRejectSuccess = 5
    }
    public class SGApprManageData : SGData
    {
        XmlConfService xmlConf;
        public SGApprManageData()
        {
            xmlConf = new XmlConfService();
        }
        
        ~SGApprManageData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        public int GetSearchResultCount()
        {
            string strData = GetTagData("APPROVECOUNT");
            int count = 0;
            if (!strData.Equals(""))
                count = Convert.ToInt32(strData);
            return count;
        }
        public static string ReturnMessage(eApprManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eApprManageMsg.eNone:
                    strMsg = "";
                    break;
                case eApprManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eApprManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchError:
                    strMsg = xmlConf.GetErrMsg("E_0207");       // 결재 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchActionSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0034");       // 승인이 완료되었습니다.
                    break;
                case eApprManageMsg.eApprBatchRejectSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0017");       // 반려가 완료되었습니다.
                    break;
            }

            return strMsg;
        }
        public List<Dictionary<int, string>> GetSearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            return listDicdata;
        }

        public List<Dictionary<int, string>> GetQuerySearchData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            listDicdata = GetSvrRecordData("RECORD");
            if (listDicdata == null)
                return null;

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }

        /**
		 * @breif 결재 정보를 반환한다.
		 * @return 결재 정보(선결,후결)
		 */
        public string GetApprKind(Dictionary<int, string> dic)
        {
            string strApprKind = "";
            if (dic.TryGetValue(8, out strApprKind) != true)
                return strApprKind;
            strApprKind = dic[8];

            int nIndex = 0;
            if (!strApprKind.Equals(""))
                nIndex = Convert.ToInt32(strApprKind);

            switch (nIndex)
            {
                case 0:
                    strApprKind = xmlConf.GetTitle("T_COMMON_APPROVE_BEFORE");        // 선결
                    break;
                case 1:
                    strApprKind = xmlConf.GetTitle("T_COMMON_APPROVE_AFTER");        // 후결
                    break;
                default:
                    break;
            }

            return strApprKind;
        }
        /**
		 * @breif 개인정보 검출 상태 정보를 반환한다.
		 * @return 개인정보 검출 상태 (미사용,포함,미포함,검출불가)
		 */
        public string GetDlp(Dictionary<int, string> dic)
        {
            string strDlp = "";
            if (dic.TryGetValue(2, out strDlp) != true)
                return strDlp;
            strDlp = dic[2];

            int nIndex = 0;
            if (!strDlp.Equals(""))
                nIndex = Convert.ToInt32(strDlp);

            switch (nIndex)
            {
                case 0:
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
                    break;
                case 1:
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");            // 포함
                    break;
                case 2:
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 미포함
                    break;
                case 3:
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNKNOWN");            // 검출불가
                    break;
                default:
                    strDlp = "0";
                    break;
            }
            return strDlp;
        }
        /**
		 * @breif 전송구분 정보를 반환한다.
		 * @return 전송구분 정보(반출/반입)
		 */
        public string GetTransKind(Dictionary<int, string> dic)
        {
            string strTransKind = "";
            if (dic.TryGetValue(6, out strTransKind) != true)
                return strTransKind;

            strTransKind = dic[6];

            int nIndex = 0;
            if (!strTransKind.Equals(""))
                nIndex = Convert.ToInt32(strTransKind);

            switch (nIndex)
            {
                case 1:
                    strTransKind = xmlConf.GetTitle("T_COMMON_EXPORT");          // 반출
                    break;
                case 2:
                    strTransKind = xmlConf.GetTitle("T_COMMON_IMPORT");          // 반입
                    break;
                default:
                    strTransKind = "-";
                    break;
            }

            return strTransKind;
        }

        /**
		 * @breif 결재요청자 정보를 반환한다.
		 * @return 결재요청자 정보
		 */
        public string GetApproveReqUser(Dictionary<int, string> dic)
        {
            string strApproveReqUser = "";
            if (dic.TryGetValue(4, out strApproveReqUser) != true)
                return strApproveReqUser;

            strApproveReqUser = dic[4];
            return strApproveReqUser;
        }

        /**
		 * @breif TransSequence 정보를 반환한다.
		 * @return TransSequence 정보
		 */
        public string GetTransSeq(Dictionary<int, string> dic)
        {
            string strTransSeq = "";
            if (dic.TryGetValue(0, out strTransSeq) != true)
                return strTransSeq;
            strTransSeq = dic[0];
            return strTransSeq;
        }

        /**
		 * @breif ApproveSequence 정보를 반환한다.
		 * @return ApproveSequence 정보
		 */
        public string GetApproveSeq(Dictionary<int, string> dic)
        {
            string strApproveSeq = "";
            if (dic.TryGetValue(1, out strApproveSeq) != true)
                return strApproveSeq;
            strApproveSeq = dic[1];
            return strApproveSeq;
        }
        /**
		 * @breif 승인대기 상태가 요청취소로 변경 할지 여부를 판단하기 위한 값을 반환한다.
		 * @return 요청취소 판단 값. (0 : 요청취소 조건 아님, 1: 사용자가 전송취소 한 경우, 2: 이전 결재자가 반려한 경우
		 */
        public int GetRequestCancelChk(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            string strApprDataPos = "";
            if(
                (dic.TryGetValue(7, out strTransStatus)!=true)
                || (dic.TryGetValue(9, out strApprStatus) != true)
                || (dic.TryGetValue(13, out strApprDataPos) != true)
                )
            {
                return 0;
            }

            strTransStatus = dic[7];                            // 전송상태 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
            strApprStatus = dic[9];                             // 결재상태 (1:승인대기,2:승인,3:반려)
            strApprDataPos = dic[13];                           // 결재 데이터 위치 (C:결재테이블, H:결재 이력 테이블)

            if((strTransStatus.Equals("C") == true) && (strApprStatus.Equals("1") == true))     // 전송상태가 전송취소이면서 결재상태가 승인대기일때
                return 1;

            if (
                (strTransStatus.Equals("W") == true)
                && (strApprStatus.Equals("1") == true)
                && (strApprDataPos.Equals("H") == true)
                )                                                                               // 전송상태가 전송대기이고 결재상태가 승인대기 일때 결재 데이터 위치가 결재 이력 테이블에 존재하면
                return 2;

            return 0;
        }
        /**
		 * @breif 전송상태 원본데이터 정보를 반환한다.
		 * @return 전송상태 원본데이터(C : 전송취소, W : 전송대기, S : 수신완료, F : 전송실패, V : 검사중)
		 */
        public string GetTransStatusCode(Dictionary<int, string> dic)
        {
            string strTransStatusCode = "";
            if (dic.TryGetValue(7, out strTransStatusCode) != true)
                return strTransStatusCode;

            strTransStatusCode = dic[7];            // 전송상태

            return strTransStatusCode;
        }
        /**
		 * @breif 결재상태 원본 데이터 정보를 반환한다.
		 * @return 결재상태 원본 데이터(1: 승인대기, 2:승인, 3: 반려)
		 */
        public string GetApprStausCode(Dictionary<int, string> dic)
        {
            string strApprStatusCode = "";
            if (dic.TryGetValue(9, out strApprStatusCode) != true)
                return strApprStatusCode;

            strApprStatusCode = dic[9];             // 승인상태

            int nIndex = 0;
            if (!strApprStatusCode.Equals(""))
                nIndex = Convert.ToInt32(strApprStatusCode);
            return strApprStatusCode;
        }

        /**
		 * @breif 결재 테이블 위치 정보를 반환한다.
		 * @return 결재 테이블 위치 정보(C : 결재 테이블, H : 결재 이력테이블)
		 */
        public string GetApprDataPos(Dictionary<int, string> dic)
        {
            string strApprDataPos = "";
            if (dic.TryGetValue(13, out strApprDataPos) != true)
                return strApprDataPos;

            strApprDataPos = dic[13];             // 결재테이블 위치 정보
            return strApprDataPos;
        }

        /**
		 * @breif 결재 가능 여부를 반환한다.
		 * @return 결재 가능 여부(0: 결재불가능, 1:결재가능)
		 */
        public string GetApprPossible(Dictionary<int, string> dic)
        {
            string strApprPossible = "";
            if (dic.TryGetValue(14, out strApprPossible) != true)
                return strApprPossible;

            strApprPossible = dic[14];             // 결재가능 여부

            int nIndex = 0;
            if (!strApprPossible.Equals(""))
                nIndex = Convert.ToInt32(strApprPossible);
            return strApprPossible;
        }

        /**
		 * @breif 결재자가 포함된 결재단계의 결재상태 정보를 반환한다.
		 * @return 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
		 */
        public string GetApprStepPossible(Dictionary<int, string> dic)
        {
            string strApprStepPossible = "";
            if (dic.TryGetValue(15, out strApprStepPossible) != true)
                return strApprStepPossible;

            strApprStepPossible = dic[15];             // 결재자가 포함된 결재단계의 결재상태 정보

            int nIndex = 0;
            if (!strApprStepPossible.Equals(""))
                nIndex = Convert.ToInt32(strApprStepPossible);
            return strApprStepPossible;
        }
        /**
		 * @breif 결재상태 정보를 반환한다.
		 * @return 결재상태 정보(요청취소,승인대기,승인,반려)
		 */
        public string GetApprStaus(Dictionary<int, string> dic)
        {
            string strTempApprStatus = "";
            if (GetRequestCancelChk(dic) != 0)
            {
                strTempApprStatus=xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
                return strTempApprStatus;
            }

            string strApprStatus = "-";
            string strApprStepStatus = "";
            if ( (dic.TryGetValue(9, out strApprStatus) != true) || (dic.TryGetValue(15, out strApprStepStatus) != true) )
                return strApprStatus;

            strApprStatus = dic[9];           // 1: 승인 대기, 2: 승인, 3: 반려
            strApprStepStatus = dic[15];      // 1: 승인 가능 상태, 2 : 승인 불가능한 상태.
            strTempApprStatus = strApprStatus;

            if (
                (strApprStatus.Equals("1") == true)
                && (strApprStepStatus.Equals("2") == true)
                && (strTempApprStatus.Equals("4") != true)
                )
            {
                strTempApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
                return strTempApprStatus;
            }
            else
            {
                int nIndex = 0;
                if (!strApprStatus.Equals(""))
                    nIndex = Convert.ToInt32(strApprStatus);

                switch (nIndex)
                {
                    case 1:
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                        break;
                    case 2:
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");                   // 승인
                        break;
                    case 3:
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");                   // 반려
                        break;
                    case 4:
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");                   // 요청취소
                        break;
                    default:
                        strApprStatus = "-";
                        break;
                }
            }
            return strApprStatus;
        }

        /**
		 * @breif 파일 포워딩 사용 여부를 반환한다.
		 * @return 파일 포워딩 사용 여부 (사용, 미사용)
		 */
        public string GetUseFileForward(Dictionary<int, string> dic)
        {
            string strUseFileForward = "-";
            if (dic.TryGetValue(16, out strUseFileForward) != true)
                return strUseFileForward;

            strUseFileForward = dic[16];            // 파일 포워딩 사용 여부 ( 0 : 포워딩한 사용자가 없음, 1: 포워딩한 사용자가 있음)

            int nIndex = 0;
            if (!strUseFileForward.Equals(""))
                nIndex = Convert.ToInt32(strUseFileForward);

            switch (nIndex)
            {
                case 0:
                    strUseFileForward = xmlConf.GetTitle("T_COMMON_FORWARD_UNUSE");              // 미사용
                    break;
                case 1:
                    strUseFileForward = xmlConf.GetTitle("T_COMMON_FORWARD_USE");                   // 사용
                    break;
                default:
                    strUseFileForward = "-";
                    break;
            }
            return strUseFileForward;
        }

        /**
        * @breif 파일 수신위치 정보를 반환한다.
        * @return 파일 수신위치(보안웹하드, 업무PC/인터넷PC)
        */
        public string GetRecvPos(Dictionary<int, string> dic)
        {
            string strRecvPos = "";
            string strTransKind = "";
            if ((dic.TryGetValue(18, out strRecvPos) != true) || (dic.TryGetValue(6, out strTransKind) != true))
                return strRecvPos;

            strRecvPos = dic[18];
            strTransKind = dic[6];

            int nIndex = 0;
            if (!strRecvPos.Equals(""))
                nIndex = Convert.ToInt32(strRecvPos);

            switch (nIndex)
            {
                case 0:
                    if (strTransKind.Equals("1"))                // 반출이면
                        strRecvPos = xmlConf.GetTitle("T_RECV_INTERNETPC");     // 인터넷 PC
                    else                                        // 반입이면
                        strRecvPos = xmlConf.GetTitle("T_RECV_BUSINESSPC");     //  PC
                    break;
                case 1:
                    strRecvPos = xmlConf.GetTitle("T_SECURITY_WEBHARD");        // 보안웹하드
                    break;
                default:
                    strRecvPos = "-";
                    break;
            }
            return strRecvPos;
        }
        /**
		 * @breif 사용자가 파일 전송 시 입력한 제목을 반환한다.
		 * @return 제목
		 */
        public string GetTitle(Dictionary<int, string> dic)
        {
            string strTitle = "";
            if (dic.TryGetValue(10, out strTitle) != true)
                return strTitle;
            strTitle = dic[10];
            return strTitle;
        }
        /**
		 * @breif 전송요청일 정보를 반환한다.
		 * @return 전송요청일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(11, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[11];
            string strYear = strTransReqDay.Substring(0, 4);
            string strMonth = strTransReqDay.Substring(4, 2);
            string strDay = strTransReqDay.Substring(6, 2);
            string strHour = strTransReqDay.Substring(8, 2);
            string strMinute = strTransReqDay.Substring(10, 2);
            string strSecond = strTransReqDay.Substring(12, 2);

            strTransReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strTransReqDay;
        }
        /**
		 * @breif 결재일 정보를 반환한다.
		 * @return 결재일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetApprDay(Dictionary<int, string> dic)
        {
            string strApprDay = "";
            if (dic.TryGetValue(12, out strApprDay) != true)
                return "-";

            strApprDay = dic[12];
            string strYear = strApprDay.Substring(0, 4);                // 년도
            string strMonth = strApprDay.Substring(4, 2);               // 월
            string strDay = strApprDay.Substring(6, 2);                 // 일
            string strHour = strApprDay.Substring(8, 2);                // 시각
            string strMinute = strApprDay.Substring(10, 2);             // 분
            string strSecond = strApprDay.Substring(12, 2);             // 초

            strApprDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strApprDay;
        }
        /**
		 * @breif 전송요청일 정보를 반환한다.
		 * @return 전송요청일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetQueryTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(11, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[11];
            return strTransReqDay;
        }
        /**
		 * @breif 결재일 정보를 반환한다.
		 * @return 결재일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetQueryApprDay(Dictionary<int, string> dic)
        {
            string strApprDay = "";
            if (dic.TryGetValue(12, out strApprDay) != true)
                return "-";

            strApprDay = dic[12];

            if (GetRequestCancelChk(dic) !=0)
                return "-";


            string strApprStatus = GetApprStaus(dic);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
                return strApprDay;
            else
                return "-";
        }

        /**
		 * @breif 결재자 정보를 반환한다.
		 * @return 결재자
		 */
        public string GetQueryApprName(Dictionary<int, string> dic)
        {
            string strApprName = "";
            if (!dic.ContainsKey(19))
                return "-";

            strApprName = dic[19];

            if (GetRequestCancelChk(dic) != 0)
                return "-";


            string strApprStatus = GetApprStaus(dic);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
                return strApprName;
            else
                return "-";
        }
        /// <summary>
        /// 데이타 타입 결과 리턴
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDataType(Dictionary<int, string> dic)
        {
            string strDataType = String.Empty;
            string resultValue = String.Empty;

            if (!dic.ContainsKey(20))
                return "";

            strDataType = dic[20];
            
            switch (strDataType)
            {
                case "1":
                    resultValue = xmlConf.GetTitle("T_DATA_TYPE_TEXT");
                    break;
                case "2":
                    resultValue = xmlConf.GetTitle("T_DATA_TYPE_IMAGE");
                    break;
                default:
                    break;
            }
            return resultValue;
        }

        /**
        * @breif 리스트 아이템의 결재 가능 여부를 판별한다.
        * @param strTransStatusCode : 전송상태 원본 코드 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
        * @param strApprStatusCode : 결재상태 원본코드 (1:승인대기,2:승인,3:반려)
        * @param strApprPossible : 결재 가능 불가능 (0 : 불가능 , 1: 가능)
        * @param strApprStepStatus : 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public static bool GetApprEnableChk(string strTransStatusCode, string strApprStatusCode,string strApprPossible, string strApprStepStatus)
        {
            if(strApprStatusCode.Equals("1"))                           // 승인대기
            {
                if (strApprPossible.Equals("0"))                         // 승인불가능
                    return false;
                else if (strTransStatusCode.Equals("V") == true)
                {
                    return false;
                }
                else
                {
                    if ((strApprStatusCode.Equals("1") == true) && (strApprStepStatus.Equals("2") == true))         // 승인대기 이지만 strApprStepStatus 값이 결재 불가능일 때
                        return false;
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        /**
        * @breif 리스트 아이템의 승인 대기 여부를 판별한다.
        * @return 승인대기 여부( true : 승인대기, false : 승인대기 아님)
        */
        public bool GetApproveWait(Dictionary<int, string> dic)
        {
            string strTransStatusCode = "";
            string strApprKind = "";
            string strApprStatusCode = "";
            string strApprDataPos = "";


            if (
                (dic.TryGetValue(7, out strTransStatusCode) != true)
                || (dic.TryGetValue(8, out strApprKind) != true)
                || (dic.TryGetValue(9, out strApprStatusCode) != true)
                || (dic.TryGetValue(13, out strApprDataPos) != true)
                )
                return false;

            strTransStatusCode = dic[7];
            strApprKind = dic[8];
            strApprStatusCode = dic[9];
            strApprDataPos = dic[13];

            if ((strApprStatusCode.Equals("1"))
                && (!strTransStatusCode.Equals("C"))
                && (!strTransStatusCode.Equals("F"))
                )
            {
                if ((strApprKind.Equals("0")) && (strApprDataPos.Equals("H")))
                    return false;
                return true;
            }
            return false;
        }
        /**
        * @breif 리스트 아이템의 결재 가능 여부를 판별한다.
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public bool GetApprEnableChk(Dictionary<int, string> dic)
        {
            if (GetRequestCancelChk(dic) != 0)
                return false;

            string strTransStatusCode = "";
            string strApprStatusCode = "";
            string strApprPossible = "";
            string strApprStepStatus = "";
            if (
                (dic.TryGetValue(7, out strTransStatusCode) != true)
                || (dic.TryGetValue(9, out strApprStatusCode) != true)
                || (dic.TryGetValue(14, out strApprPossible) != true)
                || (dic.TryGetValue(15, out strApprStepStatus) != true)
                )
                return false;

            strTransStatusCode = dic[7];                // 전송상태  ( W : 전송대기, C : 전송취소, S : 전송완료, F : 전송실패, V : 검사중 )
            strApprStatusCode = dic[9];                 // 결재상태  ( 1 : 승인대기, 2 : 승인, 3: 반려 )
            strApprPossible = dic[14];              // 결재 가능/불가능 
            strApprStepStatus = dic[15];            // 결재단계가 포함된 결재 가능 /불가능 ( 1 : 승인가능 , 2 : 승인 불가능 )

            if (strApprStatusCode.Equals("1"))                           // 승인대기
            {
                if (strApprPossible.Equals("0"))                         // 승인불가능
                    return false;
                else if (strTransStatusCode.Equals("V") == true)
                {
                    return false;
                }
                else
                {
                    if ((strApprStatusCode.Equals("1") == true) && (strApprStepStatus.Equals("2") == true))         // 승인대기 이지만 strApprStepStatus 값이 결재 불가능일 때
                        return false;
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /**
        * @breif 선택된 리스트 아이템의 승인 또는 반려 가능 여부를 판별한다.
        * @param bApprAction 승인 가능 여부(out)
        * @param bApprReject 반려 가능 여부(out)
        * @param strTransStatusCode : 전송상태 원본 코드 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
        * @param strApprStatusCode : 결재상태 원본코드 (1:승인대기,2:승인,3:반려)
        * @param strApprPossible : 결재 가능 불가능 (0 : 불가능 , 1: 가능)
        * @param strApprStepStatus : 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public static void GetApprActionRejectChk(out bool bApprAction, out bool bApprReject, string strTransStatusCode, string strApprStatusCode, string strApprPossible, string strApprStepStatus)
        {
            if (strApprStatusCode.Equals("1"))                           // 승인대기
            {
                if (strApprPossible.Equals("0"))                         // 승인불가능
                {
                    bApprAction = bApprReject = false;
                }
                else
                {
                    if ((strApprStatusCode.Equals("1") == true) && (strApprStepStatus.Equals("2") == true))         // 승인대기 이지만 strApprStepStatus 값이 결재 불가능일 때
                    {
                        bApprAction = bApprReject = false;
                    }
                    else
                    {
                        bApprAction = true;
                        if (strApprPossible.Equals("2"))
                            bApprReject = false;
                        else
                            bApprReject = true;
                    }
                }
                if (strTransStatusCode.Equals("V"))
                {
                    bApprAction = bApprReject = false;
                }
            }
            else
            {
                bApprAction = bApprReject = false;
            }

            return;
        }


        /**
		 * @breif 목적지망 정보를 반환한다.
		 * @return 목적지망 정보
		 */
        public string GetDestNetworkName(Dictionary<int, string> dic, Dictionary<string, SGNetOverData> dicDestSysPos)
        {
            string strDestNetwork = "";
            if (dic.TryGetValue(18, out strDestNetwork) != true)
                return strDestNetwork;

            strDestNetwork = dic[18];

            if (strDestNetwork.Length < 1 || dicDestSysPos == null || dicDestSysPos.Count < 1)
                return strDestNetwork;

            // 해당망 이름을 return;
            foreach (var item in dicDestSysPos)
            {
                if (item.Value.strDestSysid == strDestNetwork)
                {
                    return item.Key;
                }
            }

            return strDestNetwork;
        }

    }
}
