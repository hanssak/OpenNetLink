using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{

    public class SGeMailApproveManageData : SGData
    {
        XmlConfService xmlConf;
        public SGeMailApproveManageData()
        {
            xmlConf = new XmlConfService();
        }
        ~SGeMailApproveManageData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        /*public int GetSearchResultCount()
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
        }*/


        public List<Dictionary<int, string>> GetQuerySearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetSvrRecordData("RECORD");

            int dataCount = 0;
            if (listDicdata != null)
                dataCount = listDicdata.Count;

            if (dataCount <= 0)
                return null;

            return listDicdata;
        }


        public static string ReturnMessage(eTransManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eTransManageMsg.eNone:
                    strMsg = "";
                    break;
                case eTransManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eTransManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
                case eTransManageMsg.eTransCancelError:
                    strMsg = xmlConf.GetErrMsg("E_0206");       // 전송 취소 중 오류가 발생되었습니다.
                    break;
                case eTransManageMsg.eTransCancelSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0021");          // 전송취소가 완료되었습니다.
                    break;
            }

            return strMsg;
        }

        /// <summary>
        /// 전송구분 정보를 반환한다.<br></br>
        /// return : 전송구분 정보(반출/반입)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransKind(Dictionary<int, string> dic)
        {
            string strTransKind = "";
            if (dic.TryGetValue(3, out strTransKind) != true)
                return strTransKind;

            strTransKind = dic[3];

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

        /// <summary>
        /// 반출 : "1", 반입 : "2" return
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransKindRawData(Dictionary<int, string> dic)
        {
            string strTransKind = "";
            if (dic.TryGetValue(3, out strTransKind) != true)
                return strTransKind;

            strTransKind = dic[3];

            return strTransKind;
        }

        /// <summary>
        /// 전송상태 정보를 반환한다.<br></br>
        /// 전송상태 정보(전송취소,전송대기,수신완료,전송실패,검사중)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransStatus(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ( (dic.TryGetValue(6, out strTransStatus) != true) || (dic.TryGetValue(7, out strApprStatus) != true) )
                return strTransStatus;

            strTransStatus = dic[6];            // 전송상태
            strApprStatus = dic[7];             // 승인상태

            return GetTransStatusDisplayData(strTransStatus, strApprStatus);

            /*if (strTransStatus.Equals("W"))
            {
                if (strApprStatus.Equals("3"))       // 반려
                    strTransStatus = xmlConf.GetTitle("T_MAIL_TRANSCANCLE");      // 발송취소
                else
                    strTransStatus = xmlConf.GetTitle("T_MAIL_TRANSWAIT");        // 발송대기
            }
            else if (strTransStatus.Equals("C"))
                strTransStatus = xmlConf.GetTitle("T_MAIL_TRANSCANCLE");      // 발송취소
            else if (strTransStatus.Equals("S"))
                strTransStatus = xmlConf.GetTitle("T_MAIL_TRANS_SUCCESS");      // 발송완료
            else if (strTransStatus.Equals("F"))
                strTransStatus = xmlConf.GetTitle("T_MAIL_TRANSFRFAILED");      // 발송실패
            else if (strTransStatus.Equals("V"))
                strTransStatus = xmlConf.GetTitle("T_MAIL_INSPECT");      // 검사중
            else
                strTransStatus = "";

            return strTransStatus;*/
        }

        public string GetTransStatusDisplayData(string strTransStatus, string strApprStatus)
        {

            string strRet = "";

            if (strTransStatus.Equals("W"))
            {
                if (strApprStatus.Equals("3"))       // 반려
                    strRet = xmlConf.GetTitle("T_MAIL_TRANSCANCLE");      // 발송취소
                else
                    strRet = xmlConf.GetTitle("T_MAIL_TRANSWAIT");        // 발송대기
            }
            else if (strTransStatus.Equals("C"))
                strRet = xmlConf.GetTitle("T_MAIL_TRANSCANCLE");      // 발송취소
            else if (strTransStatus.Equals("S"))
                strRet = xmlConf.GetTitle("T_MAIL_TRANS_SUCCESS");      // 발송완료
            else if (strTransStatus.Equals("F"))
                strRet = xmlConf.GetTitle("T_MAIL_TRANSFRFAILED");      // 발송실패
            else if (strTransStatus.Equals("V"))
                strRet = xmlConf.GetTitle("T_MAIL_INSPECT");      // 검사중
            else
                strRet = "";

            return strRet;
        }

        /// <summary>
        /// 대결재(sfm2)에서 실제 승인을 한 승인자 정보를 알려준다.
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetQueryApprName(Dictionary<int, string> dic)
        {
            string strApprName = "";
            if (!dic.ContainsKey(14)) // KKW - 승인자 찾기
                return "-";

            strApprName = dic[14];

            string strApprStatus = GetApprStaus(dic);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
                return strApprName;
            else
                return "-";
        }

        /// <summary>
        /// 결재상태 정보를 반환한다.<br></br>
        /// return : 결재상태 정보(요청취소,승인대기,승인,반려)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprStaus(Dictionary<int,string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ((dic.TryGetValue(6, out strTransStatus) != true) || (dic.TryGetValue(7, out strApprStatus) != true))
                return strApprStatus;

            strTransStatus = dic[6];            // 전송상태
            strApprStatus = dic[7];             // 승인상태

            return GetApprStausDisplayData(strApprStatus, strTransStatus);
        }


        public string GetApprStausDisplayData(string strApprStatus, string strTransStatus)
        {
            string strRet = "";
            if (strApprStatus == "1")
            {
                if (strTransStatus.Equals("C"))
                    strRet = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");      // 요청취소
                else
                    strRet = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");      // 승인대기
            }
            else if (strApprStatus == "2")
                strRet = xmlConf.GetTitle("T_COMMON_APPROVE");      // 승인
            else if (strApprStatus == "3")
                strRet = xmlConf.GetTitle("T_COMMON_REJECTION");      // 반려
            else
                strRet = "-";

            return strRet;
        }


        /// <summary>
        /// 승인대기 상태 인지 유무 값 (true : 승인/반려가능, false:불가능)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool GetApproveCanDo(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            string strApprStatus = "";
            if ((dic.TryGetValue(6, out strTransStatus) != true) || (dic.TryGetValue(7, out strApprStatus) != true))
                return false;

            strTransStatus = dic[6];            // 전송상태
            strApprStatus = dic[7];             // 승인상태

            return (strApprStatus == "1" && strTransStatus != "C");
        }


        /// <summary>
        /// 메일발송시 지정된 수신자 목록을 반환한다.<br></br>
        /// return : 수신자들 목록
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetRecvUser(Dictionary<int, string> dic)
        {
            string strRecvUser = "";
            if (dic.TryGetValue(9, out strRecvUser) != true)
                return strRecvUser;
            strRecvUser = dic[9];
            return strRecvUser;
        }

        public string GetMailSender(Dictionary<int, string> dic)
        {
            string strSender = "";
            if (dic.TryGetValue(8, out strSender) != true)
                return strSender;
            strSender = dic[8];

            int nPos = -1;
            if( (nPos = strSender.IndexOf('/')) != -1)
            {
                if (nPos == 0)
                    strSender = "";
                else
                    strSender = strSender.Substring(0, nPos);
            }

            return strSender;
        }

        /// <summary>
        /// 사용자가 메일발송시 입력한 제목을 반환한다.<br></br>
        /// return : 제목
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTitle(Dictionary<int, string> dic)
        {
            string strTitle = "";
            if (dic.TryGetValue(11, out strTitle) != true)
                return strTitle;
            strTitle = dic[11];
            return strTitle;
        }

        /// <summary>
        /// 전송요청일 정보를 반환한다.<br></br>
        /// return : 전송요청일(type : YYYY-MM-DD hh:mm:ss)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(12, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[12];

            if (strTransReqDay.Length >= 14)
            {
                string strYear = strTransReqDay.Substring(0, 4);
                string strMonth = strTransReqDay.Substring(4, 2);
                string strDay = strTransReqDay.Substring(6, 2);
                string strHour = strTransReqDay.Substring(8, 2);
                string strMinute = strTransReqDay.Substring(10, 2);
                string strSecond = strTransReqDay.Substring(12, 2);
                strTransReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            }

            return strTransReqDay;
        }


        public string GetApproveDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(13, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[13];

            if (strTransReqDay.Length >= 14)
            {
                string strYear = strTransReqDay.Substring(0, 4);
                string strMonth = strTransReqDay.Substring(4, 2);
                string strDay = strTransReqDay.Substring(6, 2);
                string strHour = strTransReqDay.Substring(8, 2);
                string strMinute = strTransReqDay.Substring(10, 2);
                string strSecond = strTransReqDay.Substring(12, 2);
                strTransReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            }

            return strTransReqDay;
        }

        

        /// <summary>
        /// 개인정보 검출 상태 정보를 반환한다.<br></br>
        /// return 개인정보 검출 상태 (미사용,포함,미포함,검출불가)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDlp(Dictionary<int, string> dic)
        {
            string strDlp = "";
            if (dic.TryGetValue(4, out strDlp) != true)
                return strDlp;
            strDlp = dic[4];

            return GetDlpDisplayData(strDlp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDlp"></param>
        /// <returns></returns>
        public string GetDlpDisplayData(string strDlp)
        {
            string strRet = "";

            if (strDlp == "0")
                strRet = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
            else if (strDlp == "1")
                strRet = xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");            // 포함
            else if (strDlp == "2")
                strRet = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 미포함
            else if (strDlp == "3")
                strRet = xmlConf.GetTitle("T_COMMON_DLP_UNKNOWN");            // 알수없음
            else
                strRet = "-";

            return strRet;
        }


        /// <summary>
        /// 파일 첨부 유무 정보를 반환한다.<br></br>
        /// return : 파일 첨부 구분 정보 (첨부파일 있음, 없음)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetFileAdd(Dictionary<int, string> dic)
        {
            string strFileAdd = "";
            if (dic.TryGetValue(5, out strFileAdd) != true)
                return strFileAdd;
            strFileAdd = dic[5];

            if (string.Compare(strFileAdd, "Y", true) == 0)
                return xmlConf.GetTitle("T_EMAIL_FILEATTACH"); // 첨부
            else if (string.Compare(strFileAdd, "N", true) == 0)
                return xmlConf.GetTitle("T_EMAIL_FILENOTATTACH"); // 미첨부

            return "-";
        }


        /// <summary>
        /// 송신망 정보를 반환한다.<br></br>
        /// return : 송신망 정보
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetSrcNetworkName(Dictionary<int, string> dic)
        {

            string strSrcNetwork = "";
            if (dic.TryGetValue(17, out strSrcNetwork) != true)
                return strSrcNetwork;

            strSrcNetwork = dic[17];

            return strSrcNetwork;
        }


        /// <summary>
        /// 목적지망 정보를 반환한다.<br></br>
        /// return : 목적지망 정보
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="dicDestSysPos"></param>
        /// <returns></returns>
        public string GetDestNetworkName(Dictionary<int, string> dic, Dictionary<string, SGNetOverData> dicDestSysPos)
        {
            return "";
        }

        /// <summary>
        /// 파일 수신위치 정보를 반환한다.<br></br>
        /// 파일 수신위치(보안웹하드, 업무PC/인터넷PC)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetRecvPos(Dictionary<int, string> dic)
        {
            string strRecvPos = "";
            string strTransKind = "";
            if ( (dic.TryGetValue(15, out strRecvPos) != true) || (dic.TryGetValue(2, out strTransKind) != true) )
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


        /// <summary>
        /// TransSequence 정보를 반환한다.<br></br>
        /// return : TransSequence 정보
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetEmailSeq(Dictionary<int, string> dic)
        {
            string strTransSeq = "";
            if (dic.TryGetValue(0, out strTransSeq) != true)
                return strTransSeq;
            strTransSeq = dic[0];
            return strTransSeq;
        }


        /// <summary>
        /// 승인 / 반려가 가능한 항목인지를 검사하는 함수
        /// Return false : 승인/반려불가능, true : 승인/반려가능
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool GetApprEnableChk(Dictionary<int, string> dic)
        {
            //if (GetRequestCancelChk(dic) != 0)
            //  return false;

            string strApprPossible = "";
            string strApprStepStatus = "";

            string strTransStatus = "";
            string strApprStatus = "";
            string strApprKind = "";
            if (dic.TryGetValue(6, out strTransStatus) != true ||
                dic.TryGetValue(7, out strApprStatus) != true ||
                dic.TryGetValue(2, out strApprKind) != true)
            {
                return false;
            }

            strTransStatus = dic[6];                            // 전송상태 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
            strApprStatus = dic[7];                             // 결재상태 (1:승인대기,2:승인,3:반려)
            strApprKind = dic[2];                               // , 결재 데이터 위치 (C:결재테이블, H:결재 이력 테이블)


            if (dic.TryGetValue(15, out strApprPossible) != true)   // AND 사전결재, STEP별 결재가능유무 파악 data, 없으면 일단 결재하게 적용
                strApprPossible = "0";

            if (strTransStatus.Equals("C") && strApprStatus.Equals("1"))     // 사용자가 전송취소, 요청취소 
                return false;

            if (strApprStatus.Equals("1") && strApprPossible != "0")    // 승인대기 && 승인불가능
                return false;

            if (strApprKind=="1")   // 사후결재
            {
                return strApprStatus.Equals("1");
            }
            else    // 사전결재
            {
                return (strApprStatus.Equals("1") && (strTransStatus.Equals("W"))); //  || strTransStatus.Equals("V")
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApproveSeq(Dictionary<int, string> dic)
        {
            string strTransSeq = "";
            if (dic.TryGetValue(1, out strTransSeq) != true)
                return strTransSeq;
            strTransSeq = dic[1];
            return strTransSeq;
        }


        /// <summary>
        /// 사전/사후 data, DB에 있는 값 그대로 받음.
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprKindRawData(Dictionary<int, string> dic)
        {
            string strApprKind = "";
            if (dic.TryGetValue(2, out strApprKind) != true)
                return strApprKind;
            strApprKind = dic[2];

            return strApprKind;
        }


        /// <summary>
        /// 결재 정보를 반환한다.<br></br>
        /// return 결재 정보(선결,후결)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprKind(Dictionary<int, string> dic)
        {
            string strApprKind = "";
            if (dic.TryGetValue(2, out strApprKind) != true)
                return strApprKind;
            strApprKind = dic[2];

            return GetApprKindDisplayData(strApprKind);
        }


        public string GetApprKindDisplayData(string strApprKind)
        {
            string strRet = "";

            if (strApprKind == "0")
                strRet = xmlConf.GetTitle("T_COMMON_APPROVE_BEFORE");        // 선결
            else if (strApprKind == "1")
                strRet = xmlConf.GetTitle("T_COMMON_APPROVE_AFTER");        // 후결

            return strRet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDataType(Dictionary<int, string> dic)
        {
            string strDataType = "";
            if (dic.TryGetValue(17, out strDataType) != true)
                return strDataType;
            strDataType = dic[17];

            int nIndex = 0;
            if (!strDataType.Equals(""))
                nIndex = Convert.ToInt32(strDataType);

            switch (nIndex)
            {
                case 1:
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_TEXT");        // 텍스트
                    break;
                case 2:
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_IMAGE");        // 이미지
                    break;
                case 4:
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_OBJECT");        // 객체
                    break;
                default:
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_NORMAL");        // 일반파일
                    break;
            }

            return strDataType;
        }

        /// <summary>
        /// 선택된 리스트 아이템의 전송취소 가능 여부를 판별한다.<br></br>
        /// 전송취소 가능 여부( true : 가능, false : 불가능)
        /// </summary>
        /// <param name="strTransStatus"></param>
        /// <param name="strApprStatus"></param>
        /// <returns></returns>
        public static bool GetTransCancelEnableChk(string strTransStatus, string strApprStatus)
        {
            if ( (strTransStatus.Equals("W") || strTransStatus.Equals("V")) 
                && (!strApprStatus.Equals("3")) 
                )                                                                                               // 전송상태가 전송대기 또는 검사중이고 결재상태가 반려가 아니라면.
                return true;
            return false;
        }

        /// <summary>
        /// 선택된 리스트 아이템의 전송취소 가능 여부를 판별한다.<br></br>
        /// 전송취소 가능 여부( true : 가능, false : 불가능)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool GetTransCancelEnableChk(string userId, Dictionary<int, string> dic)
        {

            string strTransStatus = "";
            string strApprStatus = "";
            if ((dic.TryGetValue(5, out strTransStatus) != true) || (dic.TryGetValue(6, out strApprStatus) != true))
                return false;

            strTransStatus = dic[5];            // 전송상태
            strApprStatus = dic[6];             // 승인상태

            if ((strTransStatus.Equals("W") || strTransStatus.Equals("V"))
                && (strApprStatus.Equals("1"))
                )                                                                                               // 전송상태가 전송대기 또는 검사중이고 결재상태가 반려가 아니라면.
                return true;
            return false;
        }
    }

}
