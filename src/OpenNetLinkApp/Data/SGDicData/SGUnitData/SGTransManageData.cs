using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eTransManageMsg
    {
        eNone=0,
        eNotData = 1,
        eSearchError = 2,
        eTransCancelError= 3,
        eTransCancelSuccess = 4
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
        override public void Copy(HsNetWork hs, SGData data)
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

        /// <summary>
        /// 전송상태 원본데이터 정보를 반환한다.<br></br>
        /// return : 전송상태 원본데이터(C : 전송취소, W : 전송대기, S : 수신완료, F : 전송실패, V : 검사중)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransStatusCode(Dictionary<int, string> dic)
        {
            string strTransStatus = "";
            if (dic.TryGetValue(3, out strTransStatus) != true)
                return strTransStatus;

            strTransStatus = dic[3];            // 전송상태

            return strTransStatus;
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

        /// <summary>
        /// 결재상태 원본 데이터 정보를 반환한다.<br></br>
        /// 결재상태 원본 데이터(1: 승인대기, 2:승인, 3: 반려)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 사용자가 파일 전송 시 입력한 제목을 반환한다.<br></br>
        /// return : 제목
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTitle(Dictionary<int, string> dic)
        {
            string strTitle = "";
            if (dic.TryGetValue(6, out strTitle) != true)
                return strTitle;
            strTitle = dic[6];
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

        /// <summary>
        /// 전송요청일 정보를 반환한다.<br></br>
        /// return : 전송요청일(type : YYYY-MM-DD hh:mm:ss)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetQueryTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(9, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[9];
            return strTransReqDay;
        }

        /// <summary>
        /// 수신가능한 다운로드 횟수를 반환한다.<br></br>
        /// 다운로드 횟수
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDownloadCount(Dictionary<int, string> dic)
        {
            string strDownloadCount = "";
            if (dic.TryGetValue(12, out strDownloadCount) != true)
                return strDownloadCount;
            strDownloadCount = dic[12];
            return strDownloadCount;
        }

        /// <summary>
        /// 수신가능한 다운로드 횟수를 반환한다.<br></br>
        /// 다운로드 횟수
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDownloadPossible(Dictionary<int, string> dic)
        {
            string strDownloadCount = "0";
            if (dic.TryGetValue(10, out strDownloadCount) != true)
                return strDownloadCount;
            strDownloadCount = dic[10];
            return strDownloadCount;
        }

        /// <summary>
        /// 파일 만료일 반환
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetExpiredDate(Dictionary<int, string> dic)
        {
            if (dic.ContainsKey(11))
            {
                return dic[11];
            }
            else
                return String.Empty;
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

        /// <summary>
        /// 파일 포워딩 전송 구분 정보를 반환한다.<br></br>
        /// return : 파일 포워딩 전송 구분 정보 (발송, 수신)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
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
                    strFileForwardKind = xmlConf.GetTitle("T_FILE_FORWARD_RECV");            // 수신
                    break;
                default:
                    strFileForwardKind = "-";
                    break;
            }
            return strFileForwardKind;
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
            string strDestNetwork = "";
            if (dic.TryGetValue(18, out strDestNetwork) != true)        // 전송관리 error 확인
                return strDestNetwork;

            strDestNetwork = dic[18];

            if (strDestNetwork.Length < 1 || dicDestSysPos == null || dicDestSysPos.Count < 1)
                return strDestNetwork;

            // 해당망 이름을 return;
            foreach(var item in dicDestSysPos)
            {
                if (item.Value.strDestSysid == strDestNetwork)
                {
                    return item.Key;
                }
            }

            return strDestNetwork;
        }

        /// <summary>
        /// 파일 수신위치 정보를 반환한다.<br></br>
        /// return 파일 수신위치(보안웹하드, 업무PC/인터넷PC)
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
        public string GetTransSeq(Dictionary<int, string> dic)
        {
            string strTransSeq = "";
            if (dic.TryGetValue(0, out strTransSeq) != true)
                return strTransSeq;
            strTransSeq = dic[0];
            return strTransSeq;
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
            //수신자는 전송취소 불가, 발신자만 취소 가능
            string stSender = String.Empty;
            dic.TryGetValue(14, out stSender);
            string[] arrSender = stSender.Split("|");
            if (arrSender[0] != userId)
                return false;

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
