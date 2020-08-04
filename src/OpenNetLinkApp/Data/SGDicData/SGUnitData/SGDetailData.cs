using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eDetailManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2,
        eTransCancelError = 3,
        eTransCancelSuccess = 4,
        eApproveError = 5,
        eApproveSuccess = 6,
        eRejectError = 7,
        eRejectSuccess = 8
    }
    public class SGDetailData : SGData
    {
        XmlConfService xmlConf;

        int m_nDataForwarded = 0;                           // 데이터 포워딩 여부  0 : 포워딩한 사용자가 없음, 1 : 포워딩한 사용자가 있음. 2 : 포워딩받은 사용자.
        string m_strTotalStatus = "";                       // 전체 전송상태
        public SGDetailData()
        {
            xmlConf = new XmlConfService();
        }
        
        ~SGDetailData()
        {

        }
        public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        /**
		 * @breif 전송구분 정보를 반환한다.
		 * @return 전송구분 정보(반출/반입)
		 */
        public string GetTransKind()
        {
            string strTransKind = GetTagData("TRANSKIND");

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
            string strTransStatus = GetTagData("TRANSSTATUS");                          // 전송상태
            string strApprStatus = "";
            

            if (strTransStatus.Equals("W"))
            {
                if (strApprStatus.Equals("3"))       // 반려
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
                else
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSWAIT");        // 전송대기
            }
            else if (strTransStatus.Equals("C"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
            else if (strTransStatus.Equals("S"))
                strTransStatus = xmlConf.GetTitle("T_TRANS_COMPLETE");      // 수신완료
            else if (strTransStatus.Equals("F"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSFAIL");      // 전송실패
            else
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCHECKING");      // 검사중

            return strTransStatus;
        }

        /**
		 * @breif 파일위치 정보를 반환한다.
		 * @return 파일위치 정보(내부서버,외부서버,업무망PC,인터넷망PC)
		 */
        public string GetFilePos(Dictionary<int, string> dic)
        {
            string strTransStatus = GetTagData("TRANSSTATUS");                          // 전송상태
            string strTransKind = GetTagData("TRANSKIND");                              // 전송구분
            string strFilePos = GetTagData("TRANSFILEPOS");                             // 파일위치

            if (m_nDataForwarded == 2)
                strTransStatus = m_strTotalStatus;

            if (strFilePos.Equals("I"))
            {
                if( (m_nDataForwarded==2) && (strTransStatus.Equals("S")) )                 // 수신자 이면서 전송상태가 전송완료라면
                {
                    if(strTransKind.Equals("1"))                    // 반출이면
                    {
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
                else 
                    strFilePos = xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
            }
            else if(strFilePos.Equals("E"))
            {
                if ((m_nDataForwarded == 2) && (strTransStatus.Equals("S")))                 // 수신자 이면서 전송상태가 전송완료라면
                {
                    if (strTransKind.Equals("1"))                    // 반출이면
                    {
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
                else
                    strFilePos = xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
            }
            else if(strFilePos.Equals("P"))
            {
                if(m_nDataForwarded==2)                                         // 수신자
                {
                    if (strTransKind.Equals("1"))                               // 반출이면
                    {
                        if(strTransStatus.Equals("W"))
                            strFilePos = xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
                        else
                            strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                    {
                        if (strTransStatus.Equals("W"))
                            strFilePos = xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
                        else
                            strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                    }
                }
                else
                {
                    if(strTransKind.Equals("1"))                            // 반출이면
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    else
                        strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
            }
            else
                strFilePos = xmlConf.GetTitle("T_DETAIL_ERROR");       // Unknown Position

            return strFilePos;
        }
    }
}
