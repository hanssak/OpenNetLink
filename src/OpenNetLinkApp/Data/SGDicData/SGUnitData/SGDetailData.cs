using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Blazor.FileReader;
using System.IO;
using System.Runtime.InteropServices;
using Serilog;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class FileInfoData
    {
        public string strFileName = "";                 // 파일 이름
        public string strFileType = "";                 // 파일 유형
        public string strFileSize = "";                 // 파일 크기
        public string strVirusHistory = "";             // 바이러스 내역
        public string strVirusExamDay = "";             // 바이러스 검사일
        public bool bCheck = false;                  // 체크 상태
        public string fileNo = "";
        public bool bCheckDisable = false;                // 체크 가능 상태.
        public string stDLP = "";
        public string stDLPDesc = "";

        public FileInfoData()
        {
            strFileName = strFileType = strFileSize = strVirusHistory = strVirusExamDay = "";
        }
        public FileInfoData(string FileName, string FileType, string FileSize, string VirusHistory, string VirusExamDay, string fileno, string dlp, string dlpdesc)
        {
            strFileName = FileName;                     // 파일 이름
            strFileType = FileType;                     // 파일 유형
            strFileSize = FileSize;                     // 파일 크기
            strVirusHistory = VirusHistory;             // 바이러스 내역
            strVirusExamDay = VirusExamDay;             // 바이러스 검사일
            fileNo = fileno;
            stDLP = dlp;                                //DLP 여부
            stDLPDesc = dlpdesc;                        //DLP 상세 
        }
        public FileInfoData(string FileName, string FileType, string FileSize, string VirusHistory, string VirusExamDay, string fileno)
        {
            strFileName = FileName;                     // 파일 이름
            strFileType = FileType;                     // 파일 유형
            strFileSize = FileSize;                     // 파일 크기
            strVirusHistory = VirusHistory;             // 바이러스 내역
            strVirusExamDay = VirusExamDay;             // 바이러스 검사일
            fileNo = fileno;
        }
    }
    public class ApproverHist
    {
        public string m_strApproverID = "";                             // 결재자 ID
        public string m_strApproverName = "";                         // 승인자
        public string m_strApprStatus = "";                           // 승인상태
        public string m_strApprDay = "";                              // 승인일
        public string m_strRejectReason = "";                         // 반려사유
        public string m_strApprStep = "";                               // 결재 Step
        public string m_strPrivacyApprove = "";                         // 보안결재 여부 
        public int m_nApprStep = 0;

        public void SetData(string strApproverID, string strApproverName, string strApprStatus, string strApprDay, string strRejectReason,string strApprStep, string strPrivacyApprove="")
        {
            m_strApproverID = strApproverID;
            m_strApproverName = strApproverName;
            m_strApprStatus = strApprStatus;
            m_strApprDay = strApprDay;
            m_strRejectReason = strRejectReason;
            m_strApprStep = strApprStep;
            m_strPrivacyApprove = strPrivacyApprove;
        }
    }
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
        public bool m_bApprDetail = false;                         // 결재관리 상세보기 정보인지 아닌지 여부를 확인한다. (true : 결재관리 상세보기, false : 전송관리 상세보기)
        public string m_strTotalStatus = "";                       // 전체 전송상태
        public string m_strApprSeq = "";                           // ApproveSeq
        public int m_nDataForwarded = 0;                           // 데이터 포워딩 여부  0 : 포워딩한 사용자가 없음, 1 : 포워딩한 사용자가 있음. 2 : 포워딩받은 사용자.
        public bool m_bTransCancel = false;                          // 전송취소 가능 여부
        public bool m_bApprove = false;                              // 승인 가능 여부
        public bool m_bReject = false;                               // 반려 가능 여부
        public SGDetailData()
        {
            xmlConf = new XmlConfService();
        }
        
        ~SGDetailData()
        {

        }
        public void SetInit(bool bApprDetail, string strTotalStatusCode, string strApprSeq="",int nDataForwarded=0, bool bTransCancel=false, bool bApprove=false, bool bReject=false)
        {
            m_bApprDetail = bApprDetail;
            m_strTotalStatus = strTotalStatusCode;
            m_strApprSeq = strApprSeq;
            m_nDataForwarded = nDataForwarded;
            m_bTransCancel = bTransCancel;
            m_bApprove = bApprove;
            m_bReject = bReject;
        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public void DetailDataChange(HsNetWork hs, SGDetailData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);

            m_bApprDetail = data.m_bApprDetail;
            m_strTotalStatus = data.m_strTotalStatus;
            m_strApprSeq = data.m_strApprSeq;
            m_nDataForwarded = data.m_nDataForwarded ;
            m_bTransCancel = data.m_bTransCancel;
            m_bApprove = data.m_bApprove;
            m_bReject = data.m_bReject;
        }

        public static string ReturnMessage(eDetailManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eDetailManageMsg.eNone:
                    strMsg = "";
                    break;
                case eDetailManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eDetailManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0208");       // 상세보기 요청 중 오류가 발생되었습니다.
                    break;
                case eDetailManageMsg.eTransCancelError:
                    strMsg = xmlConf.GetErrMsg("E_0206");       // 전송 취소 중 오류가 발생되었습니다.
                    break;
                case eDetailManageMsg.eTransCancelSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0021");          // 전송취소가 완료되었습니다.
                    break;
                case eDetailManageMsg.eApproveError:
                    strMsg = xmlConf.GetErrMsg("E_0209");       // 승인 요청 중 오류가 발생되었습니다.
                    break;
                case eDetailManageMsg.eApproveSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0034");       // 승인이 완료되었습니다.
                    break;
                case eDetailManageMsg.eRejectError:
                    strMsg = xmlConf.GetErrMsg("E_0210");       // 반려 요청 중 오류가 발생되었습니다.
                    break;
                case eDetailManageMsg.eRejectSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0017");       // 반려가 완료되었습니다.
                    break;
            }

            return strMsg;
        }

        public string GetFowardTitle()
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();

            // m_nDataForwarded - 데이터 포워딩 여부  0 : 포워딩한 사용자가 없음, 1 : 포워딩한 사용자가 있음. 2 : 포워딩받은 사용자.
            switch (m_nDataForwarded)
            {
                case 1:
                    strMsg = xmlConf.GetTitle("T_ETC_RECIVER"); // 수신자
                    break;
                case 2:
                    strMsg = xmlConf.GetTitle("T_ETC_FORWARDSEND"); // 전송자
                    break;
                default:
                    strMsg = "";
                    break;
            }

            return strMsg;
        }

        /// <summary>
        /// 전송구분 정보를 반환(반출/반입)
        /// </summary>
        /// <returns></returns>
        public string GetTransKind()
        {
            string strTransKind = GetBasicTagData("TRANSKIND");

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
        /// 개인정보가 있는지 유무 정보 반환
        /// </summary>
        /// <returns></returns>
        public string GetDLP()
        {
            string DLPCode = GetBasicTagData("DLP");
            string stDLP = String.Empty;
            switch(DLPCode)
            {
                case "0":
                    stDLP = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");
                    break;
                case "1":
                    stDLP = xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");
                    break;
                case "2":
                    stDLP = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");
                    break;
                case "3":
                    stDLP = xmlConf.GetTitle("T_COMMON_DLP_UNKNOWN");
                    break;

            }
            return stDLP;
        }

        /// <summary>
        /// 전송상태 정보를 반환(전송취소,전송대기,수신완료,전송실패,검사중)
        /// </summary>
        /// <returns></returns>
        public string GetTransStatus()
        {
            string strTransStatus = GetBasicTagData("TRANSSTATUS");                          // 전송상태
            string strApprStatus = GetBasicTagData("APPROVESTATUS");                         // 승인상태

            //원소스
            //이렇게 한 이유가 뭔지 모르겠음 넷링크와 비교해볼 때 뭔가 작업이 완료되지 않은 느낌
            //비교 넷링크 : TransManageWnd.cpp 324라인  
            //if (m_nDataForwarded == 2)
            //    strTransStatus = m_strTotalStatus;

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

            if (m_nDataForwarded == 2)
            {
                string strTemp = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
                if (strTemp.Equals(strTransStatus))
                {
                    strTransStatus = xmlConf.GetTitle("T_ETC_CANCELRECIVE");        // 수신취소
                }
            }
            

            return strTransStatus;
        }

        /// <summary>
        /// 파일위치 정보를 반환(내부서버,외부서버,업무망PC,인터넷망PC)
        /// </summary>
        /// <returns></returns>
        public string GetFilePos()
        {
            string strTransStatus = GetBasicTagData("TRANSSTATUS");                          // 전송상태
            string strTransKind = GetBasicTagData("TRANSKIND");                              // 전송구분
            string strFilePos = GetBasicTagData("TRANSFILEPOS");                             // 파일위치

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

            if ((strTransStatus.Equals("C")) || (strTransStatus.Equals("F")))                       // 전송취소 이거나 전송실패인 경우 파일위치 "-" 으로 표시
                strFilePos = "-";

            return strFilePos;
        }

        /// <summary>
        /// 파일위치 정보를 반환한다.(3망상황일때,정보표시)
        /// </summary>
        /// <param name="dicDestSysPos"></param>
        /// <returns></returns>
        public string GetFilePosNetOver(Dictionary<string, SGNetOverData> dicDestSysPos)
        {

            if (dicDestSysPos == null || dicDestSysPos.Count < 2)
                return "";

            string strTransStatus = GetBasicTagData("TRANSSTATUS");                          // 전송상태
            if ((strTransStatus.Equals("C")) || (strTransStatus.Equals("F")))                // 전송취소 이거나 전송실패인 경우 파일위치 "-" 으로 표시
                return "-";

            string strTransKind = GetBasicTagData("TRANSKIND");                              // 전송구분
            string strFilePos = GetBasicTagData("TRANSFILEPOS");                             // 파일위치

            string strFilePosSysID = GetBasicTagData("NETOVERSYSTEM");                       // 3망 연계 상황에서 파일이 존재하는 system_id 위치정보
            if (strFilePosSysID.Length < 2)
            {
                return "";
            }

            string strFilePosText = "";

            // 파일이 위치한 망이름 설정
            foreach (var item in dicDestSysPos)
            {
                if (item.Value.strDestSysid == strFilePosSysID)
                {
                    strFilePosText = item.Key;
                    break;
                }
            }

            if (strFilePosText.Length < 1)
                return "";

            if (strFilePos.Equals("P"))
                strFilePosText += xmlConf.GetTitle("T_POS_PC");
            else
                strFilePosText += xmlConf.GetTitle("T_POS_SERVER");

            return strFilePosText;


            //////////////////////////////

            /*            if (m_nDataForwarded == 2)
                            strTransStatus = m_strTotalStatus;

                        if (strFilePos.Equals("I"))
                        {
                            if ((m_nDataForwarded == 2) && (strTransStatus.Equals("S")))                 // 수신자 이면서 전송상태가 전송완료라면
                            {
                                if (strTransKind.Equals("1"))                    // 반출이면
                                {
                                    //strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                                    strFilePos = strToServer + xmlConf.GetTitle("T_POS_PC");
                                }
                                else
                                {
                                    // strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                                    strFilePos = strFromServer + xmlConf.GetTitle("T_POS_PC");       // 업무망 PC
                                }

                            }
                            else
                            {
                                // strFilePos = xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
                                strFilePos = strFromServer + xmlConf.GetTitle("T_POS_SERVER");
                            }

                        }
                        else if (strFilePos.Equals("E"))
                        {
                            if ((m_nDataForwarded == 2) && (strTransStatus.Equals("S")))                 // 수신자 이면서 전송상태가 전송완료라면
                            {
                                if (strTransKind.Equals("1"))                    // 반출이면
                                {
                                    //strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                                    strFilePos = strToServer + xmlConf.GetTitle("T_POS_PC");
                                }
                                else
                                {
                                    //strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                                    strFilePos = strFromServer + xmlConf.GetTitle("T_POS_PC");       // 업무망 PC
                                }                        
                            }
                            else
                            {
                                // strFilePos = xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
                                strFilePos = strToServer + xmlConf.GetTitle("T_POS_SERVER");
                            }

                        }
                        else if (strFilePos.Equals("P"))
                        {
                            if (m_nDataForwarded == 2)                                         // 수신자
                            {
                                if (strTransKind.Equals("1"))                               // 반출이면
                                {
                                    if (strTransStatus.Equals("W"))
                                    {
                                        // strFilePos = xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
                                        strFilePos = strToServer + xmlConf.GetTitle("T_POS_SERVER");
                                    }                            
                                    else
                                    {
                                        // strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                                        strFilePos = strToServer + xmlConf.GetTitle("T_POS_PC");
                                    }                            
                                }
                                else
                                {
                                    if (strTransStatus.Equals("W"))
                                    {
                                        // strFilePos = xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
                                        strFilePos = strFromServer + xmlConf.GetTitle("T_POS_SERVER");
                                    }                            
                                    else
                                    {
                                        // strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                                        strFilePos = strFromServer + xmlConf.GetTitle("T_POS_PC");       // 업무망 PC
                                    }                            
                                }
                            }
                            else
                            {
                                if (strTransKind.Equals("1"))                            // 반출이면
                                {
                                    // strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                                    strFilePos = strToServer + xmlConf.GetTitle("T_POS_PC");
                                }                        
                                else
                                {
                                    // strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                                    strFilePos = strFromServer + xmlConf.GetTitle("T_POS_PC");       // 업무망 PC
                                }                        
                            }

                        }
                        else
                            strFilePos = xmlConf.GetTitle("T_DETAIL_ERROR");       // Unknown Position

                        return strFilePos;*/

        }

        /// <summary>
        /// 파일위치 정보를 반환한다.
        /// </summary>
        /// <returns>파일위치 정보(내부서버,외부서버,업체 지정업무망명, 업체 지정 인터넷망명)</returns>
        public string GetFilePosByRealName(int groupId)
        {
            string strTransStatus = GetBasicTagData("TRANSSTATUS");                          // 전송상태
            string strTransKind = GetBasicTagData("TRANSKIND");                              // 전송구분
            string strFilePos = GetBasicTagData("TRANSFILEPOS");                             // 파일위치

            string strInNet = "";
            string strExNet = "";
            string strServer = "";
            
            xmlConf.GetNetworkTitle(groupId, out strInNet, out strExNet);
            strServer = xmlConf.GetTitle("T_POS_SERVER");

            if (m_nDataForwarded == 2)
                strTransStatus = m_strTotalStatus;

            if (strFilePos.Equals("I"))
            {                
                if ((m_nDataForwarded == 2) && (strTransStatus.Equals("S")))                 // 수신자 이면서 전송상태가 전송완료라면
                {
                    if (strTransKind.Equals("1"))                    // 반출이면
                    {
                        strFilePos = strExNet;  //strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                        strFilePos = strInNet; // strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
                else
                    strFilePos = $"{strInNet} {strServer}"; //xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
            }
            else if (strFilePos.Equals("E"))
            {
                if ((m_nDataForwarded == 2) && (strTransStatus.Equals("S")))                 // 수신자 이면서 전송상태가 전송완료라면
                {
                    if (strTransKind.Equals("1"))                    // 반출이면
                    {
                        strFilePos = strExNet;  //strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                        strFilePos = strInNet;  //strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
                else
                    strFilePos = strFilePos = $"{strExNet} {strServer}"; //xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
            }
            else if (strFilePos.Equals("P"))
            {
                if (m_nDataForwarded == 2)                                         // 수신자
                {
                    if (strTransKind.Equals("1"))                               // 반출이면
                    {
                        if (strTransStatus.Equals("W"))
                            strFilePos = $"{strExNet} {strServer}"; //xmlConf.GetTitle("T_DETAIL_EX_SERVER");       // 외부서버
                        else
                            strFilePos = strExNet;  // strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    }
                    else
                    {
                        if (strTransStatus.Equals("W"))
                            strFilePos = xmlConf.GetTitle("T_DETAIL_IN_SERVER");       // 내부서버
                        else
                            strFilePos = strInNet;  //strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                    }
                }
                else
                {
                    if (strTransKind.Equals("1"))                            // 반출이면
                        strFilePos = strExNet;  // strFilePos = xmlConf.GetTitle("T_WATERMARK_OUT");       // 인터넷망 PC
                    else
                        strFilePos = strInNet;  // strFilePos = xmlConf.GetTitle("T_WATERMARK_IN");       // 업무망 PC
                }
            }
            else
                strFilePos = xmlConf.GetTitle("T_DETAIL_ERROR");       // Unknown Position

            if ((strTransStatus.Equals("C")) || (strTransStatus.Equals("F")))                       // 전송취소 이거나 전송실패인 경우 파일위치 "-" 으로 표시
                strFilePos = "-";

            return strFilePos;
        }

        /// <summary>
        /// TransSequence(요청번호) 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetTransSeq()
        {
            string strTransSeq = GetBasicTagData("TRANSSEQ");
            return strTransSeq;
        }

        /// <summary>
        /// Sequence(요청번호) 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetApprSeq()
        {
            return m_strApprSeq;
        }

        /// <summary>
        /// 결재 정보(선결,후결)반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetApprKind()
        {
            string strApprKind = GetBasicTagData("APPROVEKIND");

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

        /// <summary>
        /// 승인상태 정보(승인대기, 승인, 반려)를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetApprStatus()
        {
            string strTransStatus = GetBasicTagData("TRANSSTATUS");                           // 전송상태
            string strApprStatus = GetBasicTagData("APPROVESTATUS");                         // 승인상태

            if (m_nDataForwarded == 2)
                strTransStatus = m_strTotalStatus;

            if(strApprStatus.Equals("3"))
                strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");      // 반려
            else if(strApprStatus.Equals("2"))
                strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");      // 승인
            else
            {
                if( (strTransStatus.Equals("C")) || (strTransStatus.Equals("F")) )
                    strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
                else
                {
                    if (strApprStatus.Equals("1"))                           // 승인대기
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                    else
                        strApprStatus = "-";
                }
            }
            return strApprStatus;
        }
        public string GetApprStatusCode() //코드 리턴 
        {
            return GetBasicTagData("APPROVESTATUS");
        }

        public string GetExpiredDay()
        {
            string strApprReqDay = GetBasicTagData("EXPIREDDATE");
            return strApprReqDay;
        }

        /// <summary>
        /// 승인요청일 정보를 반환
        /// </summary>
        /// <returns></returns>
        public string GetApprReqDay()
        {
            string strApprReqDay = GetBasicTagData("REQDATE");
            string strYear = strApprReqDay.Substring(0, 4);                // 년도
            string strMonth = strApprReqDay.Substring(4, 2);               // 월
            string strDay = strApprReqDay.Substring(6, 2);                 // 일
            string strHour = strApprReqDay.Substring(8, 2);                // 시각
            string strMinute = strApprReqDay.Substring(10, 2);             // 분
            string strSecond = strApprReqDay.Substring(12, 2);             // 초

            strApprReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strApprReqDay;
        }

        /// <summary>
        /// 날짜 정보를 반환한다.
        /// </summary>
        /// <param name="strOrgDay"></param>
        /// <returns></returns>
        public string GetConvertDay(string strOrgDay)
        {
            string strConvertDay = strOrgDay;
            string strYear = strConvertDay.Substring(0, 4);                // 년도
            string strMonth = strConvertDay.Substring(4, 2);               // 월
            string strDay = strConvertDay.Substring(6, 2);                 // 일
            string strHour = strConvertDay.Substring(8, 2);                // 시각
            string strMinute = strConvertDay.Substring(10, 2);             // 분
            string strSecond = strConvertDay.Substring(12, 2);             // 초

            strConvertDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strConvertDay;
        }


        /// <summary>
        /// 승인 요청자 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetReqUser()
        {
            string strReqUserName = GetBasicTagData("REQNAME");                 // 결재 요청자 이름
            string strReqUserPos = GetBasicTagData("REQUSERPOS");               // 결재 요청자 직위

            if (strReqUserName.Equals(""))
                return strReqUserName;

            strReqUserName = strReqUserName + " " + strReqUserPos;
            return strReqUserName;
        }

        /// <summary>
        /// 제목을 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetTitle()
        {
            string strTitle = GetBasicTagData("TITLE");
            return strTitle;
        }

        /// <summary>
        /// 설명을 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetDesc()
        {
            string strDesc = GetBasicTagData("DESCRIPTION");
            return strDesc;
        }
        public string GetFileRename(bool bMode, string strFileName)
        {
            if (bMode == true)
            {
                strFileName=strFileName.Replace("`", "^TD^");
                strFileName=strFileName.Replace("&", "^AP^");
                strFileName=strFileName.Replace("%", "^PC^");
                strFileName=strFileName.Replace("!", "^EM^");
                strFileName=strFileName.Replace("@", "^AT^");

                strFileName = strFileName.Replace("#", "^SH^");
                strFileName = strFileName.Replace("$", "^DL^");
                strFileName = strFileName.Replace("*", "^AS^");
                strFileName = strFileName.Replace("(", "^LR^");
                strFileName = strFileName.Replace(")", "^RR^");

                strFileName = strFileName.Replace("-", "^DS^");
                strFileName = strFileName.Replace("+", "^PL^");
                strFileName = strFileName.Replace("=", "^EQ^");
                strFileName = strFileName.Replace(";", "^SC^");
                strFileName = strFileName.Replace("'", "^SQ^");
            }
            else
            {
                strFileName = strFileName.Replace("^TD^", "`");
                strFileName = strFileName.Replace("^AP^", "&");
                strFileName = strFileName.Replace("^PC^", "%");
                strFileName = strFileName.Replace("^EM^","!");
                strFileName = strFileName.Replace("^AT^", "@");

                strFileName = strFileName.Replace("^SH^","#");
                strFileName = strFileName.Replace( "^DL^","$");
                strFileName = strFileName.Replace("^AS^","*");
                strFileName = strFileName.Replace("^LR^","(");
                strFileName = strFileName.Replace("^RR^",")");

                strFileName = strFileName.Replace("^DS^","-");
                strFileName = strFileName.Replace("^PL^","+");
                strFileName = strFileName.Replace("^EQ^","=");
                strFileName = strFileName.Replace("^SC^",";");
                strFileName = strFileName.Replace("^SQ^","'");
            }

            return strFileName;
        }

        /// <summary>
        /// 파일미리보기 가능 여부를 반환한다.(bInner : 내부서버인지 외부서버인지 판단.)( true : 가능 , false :  불가능 )
        /// </summary>
        /// <param name="bInner"></param>
        /// <returns></returns>
        public bool GetFilePrevEnable(bool bInner)
        {
            if (m_bApprDetail != true)                      // 결재 상세보기가 아닐경우
                return false;

            if (m_bApprove != true)                         // 결재가 불가능한 경우
                return false;

            string strTransKind = GetBasicTagData("TRANSKIND");

            if(bInner)                                          // 내부
            {
                if (strTransKind.Equals("1"))
                    return true;
                else
                    return false;
            }
            else                                                // 외부 
            {
                if (strTransKind.Equals("1"))
                    return false;
                else
                    return true;
            }
        }


        /// <summary>
        /// 숫자를 입력받아 파일크기를 문자열로 줌
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetSizeStr(long size)
        {
            string rtn = "";
            if (size == 0)
            {
                rtn = "0 KB";
                return rtn;
            }

            if (size > 1024 * 1024 * 1024)
            {
                float nSize = (float)size / (1024 * 1024 * 1024);
                rtn = nSize.ToString("####0.0") + "GB";
            }
            else if (size > 1024 * 1024)
            {
                float nSize = (float)size / (1024 * 1024);
                rtn = nSize.ToString("####0.0") + "MB";
            }
            else if (size > 1024)
            {
                float nSize = (float)size / (1024);
                rtn = nSize.ToString("####0.0") + "KB";
            }
            else if (size > 0)
                rtn = size + " Byte";
            return rtn;
        }

        /// <summary>
        /// 해당 파일의 크기가 0kb 인지 확인(true: 0kb이거나 다운불가능, false: 다운가능)
        /// </summary>
        /// <returns></returns>
        public bool GetFileSizeEmpty(string strFIleIdx)
        {

            if (strFIleIdx.Length < 1)
            {
                Log.Information($"FILE-Index : Empty");
                return true;
            }

            List<Dictionary<int, string>> listDicdata = GetRecordData("FILERECORD");

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
            {
                Log.Information($"FILERECORD Count: 0");
                return true;
            }

            string strFileSize = "";
            string strFileIndexData = "";
            Dictionary<int, string> data = null;
            for (int i = 0; i < dataCount; i++)
            {
                data = listDicdata[i];
                if (data == null)
                    continue;

                if (data.TryGetValue(6, out strFileIndexData))                   // 파일 Index
                {
                    if (strFileIndexData == strFIleIdx)
                    {
                        strFileSize = "";
                        if (data.TryGetValue(3, out strFileSize))                   // 파일 Size
                        {
                            Log.Information($"FILE({strFileIndexData}) Size : {strFileSize}");

                            Int64 nSize = 0;
                            if (!strFileSize.Equals(""))
                                nSize = Convert.ToInt64(strFileSize);

                            return ((nSize > 0) ? false : true);
                        }

                    }
                }

            }

            Log.Information($"FILE Not Found !!!");
            return true;
        }

        public void GetFileInfo(out List<FileInfoData> fileListInfo)
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("FILERECORD");

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
            {
                fileListInfo = null;
                return;
            }

            string strFileName = "-";
            string strFileType = "-";
            string strFileSize = "-";
            string strVirus = "-";
            string strVirusExamDay = "-";
            string stDLP = "";
            string stDLPDesc = "";
            List<FileInfoData> m_ListData = new List<FileInfoData>();
            Dictionary<int, string> data = null;
            for (int i=0;i<dataCount;i++)
            {
                data = listDicdata[i];
                if (data == null)
                    continue;

                data.TryGetValue(0, out stDLP);         //DLP 포함여부(1:포함)
                
                if( data.TryGetValue(7, out stDLPDesc))     //DLP DESC
                {
                    if (stDLPDesc.Length < 1)
                        stDLPDesc = "-";
                }
                if (data.TryGetValue(1, out strFileName))                   // 파일이름 
                {
                    strFileName = data[1];
                    if (!strFileName.Equals(""))
                        strFileName = GetFileRename(false, strFileName);
                }
                else
                    strFileName = "-";

                if (data.TryGetValue(2, out strFileType))                   // 파일 유형
                {
                    strFileType = data[2];
                    if (strFileType == "파일 폴더")      // NetLink호환 : 언어별로 다 찾아서 넣어줘야함, dir이라는걸 알 수 있는 값이 없음
                        strFileType = "DIR";
                }
                else
                    strFileType = "-";

                if (strFileType.Equals("DIR"))
                {
                    int index = -1;
                    index = strFileName.LastIndexOf("\\");
                    if (index >= 0)
                    {
                        string strTemp = strFileName.Substring(0, index + 1);
                        string strTemp2 = strFileName.Replace(strTemp, "");
                        if (!strFileName.Equals("\\"))
                            strFileName = strFileName.Replace(strTemp, "");
                    }
                }
                else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        strFileName = strFileName.Replace("/", "\\");
                    }
                    else
                    {
                        strFileName = strFileName.Replace("\\", "/");
                    }
                    strFileName = System.IO.Path.GetFileName(strFileName);
                }

                if (data.TryGetValue(3, out strFileSize))                   // 파일 Size
                {
                    strFileSize = data[3];
                    Int64 nSize = 0;
                    if (!strFileSize.Equals(""))
                        nSize = Convert.ToInt64(strFileSize);

                    strFileSize = GetSizeStr(nSize);
                    /*if (nSize > 0)
                        nSize = nSize / 1024;

                    if (nSize <= 0)
                        nSize = 1;

                    strFileSize = String.Format("{0} KB", nSize);*/
                }
                else
                    strFileSize = "-";

                if (data.TryGetValue(4, out strVirus))                   // 바이러스 내역
                {
                    strVirus = data[4];
                    if(strVirus.Equals(""))
                        strVirus = "-";
                }
                else
                    strVirus = "-";

                if (data.TryGetValue(5, out strVirusExamDay))                   // 바이러스 검사일 
                {
                    strVirusExamDay = data[5];
                    if (strVirusExamDay.Equals(""))
                        strVirusExamDay = "-";
                }
                else
                    strVirusExamDay = "-";

                m_ListData.Add(new FileInfoData(strFileName, strFileType, strFileSize, strVirus, strVirusExamDay, data[6], stDLP, stDLPDesc));
            }
            fileListInfo = m_ListData;
            return;
        }

        public List<Dictionary<int, string>> GetApproverInfo()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }

        public List<Dictionary<int, string>> GetForwardUserInfo()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("FORWARDUSERRECORD");

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }

        public bool GetTransCancelEnable()
        {
            return m_bTransCancel;
        }
        public bool GetApproveEnable()
        {
            return m_bApprove;
        }
        public bool GetRejectEnable()
        {
            return m_bReject;
        }

        /// <summary>
        /// 결재 이력 정보를 반환한다
        /// </summary>
        /// <returns></returns>
        public List<ApproverHist> GetApproverInfoHist()
        {
            List<ApproverHist> approverHist = new List<ApproverHist>();
            ApproverHist tmpApprover;

            List<Dictionary<int, string>> DicData = null;
            DicData = GetApproverInfo();

            if (DicData == null)
                return null;

            int nApproverCount = DicData.Count;
            if (nApproverCount <= 0)
                return null;

            string strApprUserID = "";                // 결재자 ID
            string strApprName = "";                // 결재자 이름 
            string strApprPos = "";                 // 결재자 직급 및 직위
            string strApprDate = "";                // 결재일 
            string strApprStatus = "";              // 결재 상태
            string strApprReason = "";              // 반려사유
            string strApprStep = "";                 // 결재 Step
            string strPrivacyAppr = "";             // 보안결재자 여부.

            string strPreApprStatusCode = "";           // 이전 결재상태
            string strApprStatusCode = "";              // 결재 상태 원본코드
            for (int i = 0; i < nApproverCount; i++)
            {
                Dictionary<int, string> Dictemp = DicData[i];
                if (Dictemp.TryGetValue(0, out strApprUserID))      // 결재자 ID 
                    strApprUserID = Dictemp[0];
                else
                    strApprUserID = "-";

                if (Dictemp.TryGetValue(1, out strApprName))      // 결재자 이름
                    strApprName = Dictemp[1];
                else
                    strApprName = "-";

                if (Dictemp.TryGetValue(2, out strApprPos))      // 결재자 직급 및 직위
                    strApprPos = Dictemp[2];
                else
                    strApprPos = "-";

                if (Dictemp.TryGetValue(3, out strApprDate))      // 결재일
                    strApprDate = Dictemp[3];
                else
                    strApprDate = "-";

                if (Dictemp.TryGetValue(4, out strApprStatus))      // 결재 상태
                    strApprStatus = Dictemp[4];
                else
                    strApprStatus = "-";

                if (Dictemp.TryGetValue(5, out strApprReason))      // 반려 사유
                    strApprReason = Dictemp[5];
                else
                    strApprReason = "-";

                if (Dictemp.TryGetValue(6, out strApprStep))      // 결재 Step
                    strApprStep = Dictemp[6];
                else
                    strApprStep = "-";

                if (Dictemp.TryGetValue(7, out strPrivacyAppr))      // 보안결재자 여부.
                    strPrivacyAppr = Dictemp[7];
                else
                    strPrivacyAppr = "-";


                strApprName = strApprName + " " + strApprPos;        // 승인자 정보 (이름 + 직위)

                strApprStatusCode = strApprStatus;

                if (!strApprStatus.Equals(""))
                {
                    int nApprStatus = Convert.ToInt32(strApprStatus);
                    switch (nApprStatus)
                    {
                        case 1:                                  
                            strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                            break;
                        case 2:
                            strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");                   // 승인
                            break;
                        case 3:
                            strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");                 // 반려
                            break;
                        default:
                            strApprStatus = "-";
                            break;
                    }
                }

                if (strApprStatusCode.Equals("1"))
                    strApprDate = "-";
                else if (strApprStatusCode.Equals("2"))
                    strApprReason = "-";

                string strTransStatus = GetBasicTagData("TRANSSTATUS");
                if ((strTransStatus.Equals("C")) || (strTransStatus.Equals("F")))    // 전송취소 이거나 전송실패
                {
                    if ((!strApprStatusCode.Equals("2")) && (!strApprStatusCode.Equals("3")))         // 승인이 아니고 반려가 아니라면 
                    {
                        strApprDate = "-";
                        strApprReason = "-";
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
                    }
                }

                if (strPreApprStatusCode.Equals("3"))
                {
                    strApprDate = "-";
                    strApprReason = "-";
                    strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
                }
                else
                {
                    strPreApprStatusCode = strApprStatusCode;
                }

                if (strApprReason.Equals(""))
                    strApprReason = "-";
                else if (strApprDate.Equals(""))
                    strApprDate = "-";
                tmpApprover = new ApproverHist();
                tmpApprover.SetData(strApprUserID,strApprName, strApprStatus, strApprDate, strApprReason,strApprStep);
                if (strApprStep != "-")
                    tmpApprover.m_nApprStep = int.Parse(strApprStep);
                else
                    tmpApprover.m_nApprStep = 0;
                approverHist.Add(tmpApprover);
            }

            return approverHist;
        }

        /// <summary>
        /// 전송관리 마지막 결재자 이름/직위를 반환한다.
        /// </summary>
        /// <param name="ApprHist"></param>
        /// <returns></returns>
        public ApproverHist GetTransLastApproverHistData(ApproverHist ApprHist)
        {
            List<ApproverHist> approverHist = null;
            approverHist=GetApproverInfoHist();
            if ((approverHist == null) || (approverHist.Count <= 0))
            {
                ApprHist = null;
                return ApprHist;
            }

            string strApproveWait = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
            string strApporve = xmlConf.GetTitle("T_COMMON_APPROVE");                       // 승인
            string strReject = xmlConf.GetTitle("T_COMMON_REJECTION");                      // 반려
            string strCancel = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");                  // 요청취소
            string strCurApprStatus = "";
            string strPreApprStatus = "";
            ApproverHist temp = null;
            for (int i=0;i<approverHist.Count;i++)
            {
                temp = approverHist[i];
                if (temp == null)
                    continue;

                strCurApprStatus = temp.m_strApprStatus;
                if (strCurApprStatus.Equals(strReject))                                     // 반려이면 마지막 결재자 
                {
                    ApprHist = temp;
                    break;
                }

                if(
                    (strPreApprStatus.Equals(strReject))                                    // 반려
                    || (strPreApprStatus.Equals(strApproveWait))                                 // 승인대기 
                    || (strPreApprStatus.Equals(strCancel))                                 // 요청취소
                    )
                {
                    if (!strPreApprStatus.Equals(strApporve))
                        continue;
                }
                else
                    ApprHist = temp;

                strPreApprStatus = strCurApprStatus;
            }
            return ApprHist;
        }

        /// <summary>
        /// 결재관리 마지막 결재자 이름/직위 를  반환
        /// </summary>
        /// <param name="ApprHist"></param>
        /// <returns></returns>
        public ApproverHist GetApprLastApproverHistData(ApproverHist ApprHist)
        {
            List<ApproverHist> approverHist = null;
            approverHist = GetApproverInfoHist();
            if ((approverHist == null) || (approverHist.Count <= 0))
            {
                ApprHist = null;
                return ApprHist;
            }

            string strUserID = GetTagData("CLIENTID");
            ApproverHist temp = null;
            for (int i = 0; i < approverHist.Count; i++)
            {
                temp = approverHist[i];
                if (temp == null)
                    continue;

                if(temp.m_strApproverID.Equals(strUserID))
                {
                    ApprHist = temp;
                    break;
                }
            }
            return ApprHist;
        }

        /// <summary>
        /// 해당 조회 건에 대해 파일이 위치한 파일미리보기 실서버 ip 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetFilePreviewIP()
        {
            string strIP = GetBasicTagData("ORGSYSTEMIP");
            return strIP;
        }

        /// <summary>
        /// FileKey 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetFileKey()
        {
            string strFileKey = GetBasicTagData("FILEKEY");
            return strFileKey;
        }
    }

    public class transData
    {
        public bool bCheckDisable { get; set; }           //체크 가능 불가능
        public bool bCheck { get; set; }                  //체크 상태
        public string TransSeq { get; set; }                //TransSeq
        public string ApvType { get; set; }                 //결재종류
        public string TransferType { get; set; }            //전송구분
        public string TransferStatus { get; set; }          //전송상태
        public string ApvStatus { get; set; }               //승인상태
        public string IsFile { get; set; }                  //파일전달
        public string strDestNet { get; set; }                  //목적지 (목적망)
        public string Title { get; set; }                   //제목
        public string RequestTime { get; set; }             //전송요청일
        public string TransStatusCode { get; set; }         //전송상태 원본 ( W : 전송대기 , C : 전송취소 , S : 수신완료 , F : 전송실패 )
        public string ApprStatusCode { get; set; }          //승인상태 원본 ( 1 : 승인대기 , 2 : 승인 , 3 : 반려 )
        public int downloadCount { get; set; }              //다운로드 카운트
        public string stDLP { get; set; }                   //개인정보 포함여부

        public string expiredDate { get; set; }             //파일만료일

        public bool isPossibleDownload { get; set; }        //파일 다운로드 가능 여부

        public int isPossibleDownloadCount { get; set; }   //파일 다운로드 가능 Count
        public string DataType { get; set; }                // data Type ( 0 : , 1 : , 2 : ) 

        public transData()
        {
            bCheckDisable = bCheck = false;
            TransSeq = ApvType = TransferType = TransferStatus = ApvStatus = IsFile = Title = RequestTime = "";
            strDestNet = "";
        }

        public transData(bool bCheckDisable, bool bCheck, string seq, string apvtype, string transfertype, string transferstatus, string apvstatus, string isfile, string title, string reqtime, string strTransStatusCode, string strApprStatusCode, string strDestNetData)
        {
            TransSeq = seq;
            ApvType = apvtype;
            TransferType = transfertype;
            TransferStatus = transferstatus;
            ApvStatus = apvstatus;
            IsFile = isfile;
            Title = title;
            RequestTime = reqtime;
            TransStatusCode = strTransStatusCode;
            ApprStatusCode = strApprStatusCode;
            strDestNet = strDestNetData;
        }
    }



}
