using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eTransManageFail
    {
        eNone=0,
        eNotData = 1,
        eSearchError = 2,
    }
    public class SGTransManageData : SGData
    {
        XmlConfService xmlConf;
        public SGTransManageData()
        {
            xmlConf = new XmlConfService();
        }
        ~SGTransManageData()
        {

        }
        public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public int GetSearchResultCount()
        {
            string strData = GetTagData("SEQCOUNT");
            int count = 0;
            if (!strData.Equals(""))
                count = Convert.ToInt32(strData);
            return count;
        }

        public List<Dictionary<int, string>> GetSearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("TRANSRECORD");
            return listDicdata;
        }

        public List<Dictionary<int, string>> GetSearchData(string strSelTransStatus, string strSelApprStatus)
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("TRANSRECORD");

            List<Dictionary<int, string>> resultDicData = new List<Dictionary<int, string>>();

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return listDicdata;

            for(int i=0;i<dataCount;i++)
            {
                Dictionary<int, string> temp = listDicdata[i];
                string strTransStatus = "";
                string strApprStatus = "";
                if ((temp.TryGetValue(3, out strTransStatus) != true) || (temp.TryGetValue(5, out strApprStatus) != true))
                    continue;

                strTransStatus = temp[3];
                strApprStatus = temp[5];

                if ((strSelTransStatus.Equals("W")) && (strApprStatus.Equals("3")))
                    continue;

                if ((strSelApprStatus.Equals("1")) && (strTransStatus.Equals("C")))
                    continue;

                resultDicData.Add(temp);
            }

            return resultDicData;
        }
        public static string FailMessage(eTransManageFail eType)
        {
            string strFailMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eTransManageFail.eNone:
                    strFailMsg = "";
                    break;
                case eTransManageFail.eNotData:
                    strFailMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eTransManageFail.eSearchError:
                    strFailMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
            }

            return strFailMsg;
        }

        /**
		 * @breif 전송구분 정보를 반환한다.
		 * @return 전송구분 정보(반출/반입)
		 */
        public string GetTransKind(Dictionary<int, string> dic)
        {
            string strTransKind = "";
            if (dic.TryGetValue(2, out strTransKind) != true)
                return strTransKind;

            strTransKind = dic[2];

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
		 * @breif 전송상태 정보를 반환한다.
		 * @return 전송상태 정보(전송취소,전송대기,수신완료,전송실패,검사중)
		 */
        public string GetTransStatus(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ( (dic.TryGetValue(3, out strTransStatus) != true) || (dic.TryGetValue(5, out strApprStatus) != true) )
                return strTransStatus;

            strTransStatus = dic[3];            // 전송상태
            strApprStatus = dic[5];             // 승인상태

            if(strTransStatus.Equals("W"))
            {
                if (strApprStatus.Equals("3"))       // 반려
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
                else
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSWAIT");        // 전송대기
            }
            else if(strTransStatus.Equals("C"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
            else if(strTransStatus.Equals("S"))
                strTransStatus = xmlConf.GetTitle("T_TRANS_COMPLETE");      // 수신완료
            else if(strTransStatus.Equals("F"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSFAIL");      // 전송실패
            else
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCHECKING");      // 검사중

            return strTransStatus;
        }
        /**
		 * @breif 전송상태 원본데이터 정보를 반환한다.
		 * @return 전송상태 원본데이터(C : 전송취소, W : 전송대기, S : 수신완료, F : 전송실패, V : 검사중)
		 */
        public string GetTransStatusCode(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            if (dic.TryGetValue(3, out strTransStatus) != true)
                return strTransStatus;

            strTransStatus = dic[3];            // 전송상태

            return strTransStatus;
        }
        /**
		 * @breif 결재상태 정보를 반환한다.
		 * @return 결재상태 정보(요청취소,승인대기,승인,반려)
		 */
        public string GetApprStaus(Dictionary<int,string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ((dic.TryGetValue(3, out strTransStatus) != true) || (dic.TryGetValue(5, out strApprStatus) != true))
                return strApprStatus;

            strTransStatus = dic[3];            // 전송상태
            strApprStatus = dic[5];             // 승인상태

            int nIndex = 0;
            if (!strApprStatus.Equals(""))
                nIndex = Convert.ToInt32(strApprStatus);

            switch(nIndex)
            {
                case 1:
                    if(strTransStatus.Equals("C"))
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");      // 요청취소
                    else
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");      // 승인대기
                    break;
                case 2:
                    strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");      // 승인
                    break;
                case 3:
                    strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");      // 반려
                    break;
                default:
                    strApprStatus = "-";
                    break;
            }
            return strApprStatus;
        }
        /**
		 * @breif 결재상태 원본 데이터 정보를 반환한다.
		 * @return 결재상태 원본 데이터(1: 승인대기, 2:승인, 3: 반려)
		 */
        public string GetApprStausCode(Dictionary<int, string> dic)
        {
            string strApprStatus = "";
            if (dic.TryGetValue(5, out strApprStatus) != true)
                return strApprStatus;

            strApprStatus = dic[5];             // 승인상태

            int nIndex = 0;
            if (!strApprStatus.Equals(""))
                nIndex = Convert.ToInt32(strApprStatus);
            return strApprStatus;
        }
        /**
		 * @breif 사용자가 파일 전송 시 입력한 제목을 반환한다.
		 * @return 제목
		 */
        public string GetTitle(Dictionary<int, string> dic)
        {
            string strTitle = "";
            if (dic.TryGetValue(6, out strTitle) != true)
                return strTitle;
            strTitle = dic[6];
            return strTitle;
        }
        /**
		 * @breif 전송요청일 정보를 반환한다.
		 * @return 전송요청일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(9, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[9];
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
		 * @breif 수신가능한 다운로드 횟수를 반환한다.
		 * @return 다운로드 횟수
		 */
        public string GetDownloadCount(Dictionary<int, string> dic)
        {
            string strDownloadCount = "";
            if (dic.TryGetValue(12, out strDownloadCount) != true)
                return strDownloadCount;
            strDownloadCount = dic[12];
            return strDownloadCount;
        }
        /**
		 * @breif 개인정보 검출 상태 정보를 반환한다.
		 * @return 개인정보 검출 상태 (미사용,포함,미포함,검출불가)
		 */
        public string GetDlp(Dictionary<int, string> dic)
        {
            string strDlp = "";
            if (dic.TryGetValue(1, out strDlp) != true)
                return strDlp;
            strDlp = dic[1];

            int nIndex = 0;
            if (!strDlp.Equals(""))
                nIndex = Convert.ToInt32(strDlp);

            switch(nIndex)
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
		 * @breif 파일 포워딩 전송 구분 정보를 반환한다.
		 * @return 파일 포워딩 전송 구분 정보 (발송, 수신)
		 */
        public string GetFileForwardKind(Dictionary<int, string> dic)
        {
            string strFileForwardKind = "";
            if (dic.TryGetValue(13, out strFileForwardKind) != true)
                return strFileForwardKind;
            strFileForwardKind = dic[13];

            int nIndex = 0;
            if (!strFileForwardKind.Equals(""))
                nIndex = Convert.ToInt32(strFileForwardKind);

            switch (nIndex)
            {
                case 0:
                    strFileForwardKind = "-";            
                    break;
                case 1:
                    strFileForwardKind = xmlConf.GetTitle("T_FILE_FORWARD_SEND");            // 발송
                    break;
                case 2:
                    strFileForwardKind = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 수신
                    break;
                default:
                    strFileForwardKind = "-";
                    break;
            }
            return strFileForwardKind;
        }
        /**
        * @breif 파일 수신위치 정보를 반환한다.
        * @return 파일 수신위치(보안웹하드, 업무PC/인터넷PC)
        */
        public string GetRecvPos(Dictionary<int, string> dic)
        {
            string strRecvPos = "";
            string strTransKind = "";
            if ( (dic.TryGetValue(16, out strRecvPos) != true) || (dic.TryGetValue(2, out strTransKind) != true) )
                return strRecvPos;

            strRecvPos = dic[16];
            strTransKind = dic[2];

            int nIndex = 0;
            if (!strRecvPos.Equals(""))
                nIndex = Convert.ToInt32(strRecvPos);

            switch(nIndex)
            {
                case 0:
                    if(strTransKind.Equals("1"))                // 반출이면
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
		 * @breif 결재 정보를 반환한다.
		 * @return 결재 정보(선결,후결)
		 */
        public string GetApprKind(Dictionary<int, string> dic)
        {
            string strApprKind = "";
            if (dic.TryGetValue(4, out strApprKind) != true)
                return strApprKind;
            strApprKind = dic[4];

            int nIndex = 0;
            if (!strApprKind.Equals(""))
                nIndex = Convert.ToInt32(strApprKind);

            switch(nIndex)
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
        * @breif 선택된 리스트 아이템의 전송취소 가능 여부를 판별한다.
        * @return 전송취소 가능 여부( true : 가능, false : 불가능)
        */
        public static bool GetTransCancelEnableChk(string strTransStatus, string strApprStatus)
        {
            if ( (strTransStatus.Equals("W") || strTransStatus.Equals("V")) 
                && (!strApprStatus.Equals("3")) 
                )                                                                                               // 전송상태가 전송대기 또는 검사중이고 결재상태가 반려가 아니라면.
                return true;
            return false;
        }

        /**
        * @breif 선택된 리스트 아이템의 전송취소 가능 여부를 판별한다.
        * @return 전송취소 가능 여부( true : 가능, false : 불가능)
        */
        public bool GetTransCancelEnableChk(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ((dic.TryGetValue(3, out strTransStatus) != true) || (dic.TryGetValue(5, out strApprStatus) != true))
                return false;

            strTransStatus = dic[3];            // 전송상태
            strApprStatus = dic[5];             // 승인상태

            if ((strTransStatus.Equals("W") || strTransStatus.Equals("V"))
                && (!strApprStatus.Equals("3"))
                )                                                                                               // 전송상태가 전송대기 또는 검사중이고 결재상태가 반려가 아니라면.
                return true;
            return false;
        }
    }
}
