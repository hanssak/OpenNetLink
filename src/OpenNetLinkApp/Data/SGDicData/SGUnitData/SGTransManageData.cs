using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using OpenNetLinkApp.Common;
using System.Runtime.InteropServices;
using OpenNetLinkApp.Data.SGDicData.Approve;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eTransManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2,
        eTransCancelError = 3,
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
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
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

            for (int i = 0; i < dataCount; i++)
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


        public List<object> GetQuerySearchData()
        {
            List<object> listDicdata = m_DicTagData.GetTagDataObjectList("trans_req_list");

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
        public string GetTransKind(object obj)
        {
            string strTransKind = "";
            string reqNetId = obj.GetTagDataString("req_net_id");

            if (String.IsNullOrEmpty(reqNetId))
                strTransKind = "-";

            string strIE = reqNetId.Substring(0, 1);

            switch (strIE)
            {
                case "I":
                    strTransKind = xmlConf.GetTitle("T_COMMON_EXPORT");          // 반출
                    break;
                case "E":
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
        public string GetTransStatus(object obj)
        {
            string strTransStatus = obj.GetTagDataString("trans_state"); //전송상태
            string strApprStatus = obj.GetTagDataString("approval_state"); //승인상태

            if (strTransStatus.Equals("wait"))
            {
                if (strApprStatus.Equals("reject"))       // 반려
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
                else
                    strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSWAIT");        // 전송대기
            }
            else if (strTransStatus.Equals("cancel"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCANCLE");      // 전송취소
            else if (strTransStatus.Equals("received"))
                strTransStatus = xmlConf.GetTitle("T_TRANS_COMPLETE");      // 수신완료
            else if (strTransStatus.Equals("fail"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSFAIL");      // 전송실패
            else if (strTransStatus.Equals("scanning"))
                strTransStatus = xmlConf.GetTitle("T_COMMON_TRANSCHECKING");      // 검사중

            return strTransStatus;
        }

        /// <summary>
        /// 전송상태 원본데이터 정보를 반환한다.<br></br>
        /// return : 전송상태 원본데이터(C : 전송취소, W : 전송대기, S : 수신완료, F : 전송실패, V : 검사중)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransStatusCode(object obj)
        {
            return obj.GetTagDataString("trans_state");
        }

        /// <summary>
        /// 결재상태 정보를 반환한다.<br></br>
        /// return : 결재상태 정보(요청취소,승인대기,승인,반려)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprStatus(object obj)
        {
            string strTransStatus = obj.GetTagDataString("trans_state"); //전송상태
            string strApprStatus = obj.GetTagDataString("approval_state"); //승인상태

            switch (strApprStatus)
            {
                case "wait":
                    if (strTransStatus.Equals("cancel"))
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");      // 요청취소
                    else
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");      // 승인대기
                    break;
                case "confirm":
                    strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");      // 승인
                    break;
                case "reject":
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
        public string GetApprStatusCode(object obj)
        {
            return obj.GetTagDataString("approval_state");
        }

        /// <summary>
        /// 사용자가 파일 전송 시 입력한 제목을 반환한다.<br></br>
        /// return : 제목
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTitle(object obj)
        {
            return obj.GetTagDataString("title");
        }
        public string GetDesc(object obj)
        {
            return obj.GetTagDataString("description");
        }

        /// <summary>
        /// 전송요청일 정보를 반환한다.<br></br>
        /// return : 전송요청일(type : YYYY-MM-DD hh:mm:ss)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransReqDay(object obj)
        {
            string strTransReqDay = obj.GetTagDataString("req_datetime");
            string strYear = strTransReqDay.Substring(0, 4);
            string strMonth = strTransReqDay.Substring(4, 2);
            string strDay = strTransReqDay.Substring(6, 2);
            string strHour = strTransReqDay.Substring(8, 2);
            string strMinute = strTransReqDay.Substring(10, 2);
            string strSecond = strTransReqDay.Substring(12, 2);

            strTransReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strTransReqDay;
        }

        public string GetDataTypeCode(object obj)
        {
            return obj.GetTagDataString("data_type");
        }

        /// <summary>
        /// 전송요청일 정보를 반환한다.<br></br>
        /// return : 전송요청일(type : YYYY-MM-DD hh:mm:ss)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetQueryTransReqDay(object obj)
        {
            return obj.GetTagDataString("req_datetime");
        }

        /// <summary>
        /// 수신가능한 다운로드 횟수를 반환한다.<br></br>
        /// 다운로드 횟수
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDownloadCount(object obj)
        {
            string strDownloadCount = "";
            //download_count 임시로 설정

            return obj.GetTagDataString("download_count");
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
        public string GetExpiredDate(object obj)
        {
            return obj.GetTagDataString("expiration_datetime");
        }

        public string GetRequestUserName(object obj)
        {
            return obj.GetTagDataString("req_user_hr", "name");
        }

        /// <summary>
        /// 개인정보 검출 상태 정보를 반환한다.<br></br>
        /// return 개인정보 검출 상태 (미사용,포함,미포함,검출불가)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDlp(object obj)
        {
            //file info 리스트 정보를 가져온다.
            object fileInfo = obj.GetTagDataObject("file_info");
            List<object> listFileRecord = fileInfo.GetTagDataObjectList("file_record_list");

            string objData = "";
            foreach (object file in listFileRecord)
            {
                objData = file.GetTagDataString("dlp");
                if (objData == "unuse")
                    break;
                else if (objData == "detected")
                    break;
            }

            string strDlp = "";
            switch (objData)
            {
                case "unuse":
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
                    break;
                case "detected":
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");            // 포함
                    break;
                case "none":
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 미포함
                    break;
                case "undetectable":
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNKNOWN");            // 검출불가
                    break;
                default:
                    strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
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
        public string GetFileForwardKind(List<object> objList, string userSeq)
        {
            string strFileForwardKind = "";
            bool isReceive = false;

            if (objList == null || objList.Count == 0)
            {
                return "-";
            }

            foreach (object obj in objList)
            {
                string objSeq = obj.GetTagDataString("user_seq");
                if (objSeq == userSeq)
                {
                    isReceive = true;
                }
            }

            if (isReceive)
                strFileForwardKind = xmlConf.GetTitle("T_FILE_FORWARD_RECV");            // 수신
            else
                strFileForwardKind = xmlConf.GetTitle("T_FILE_FORWARD_SEND");            // 발송

            return strFileForwardKind;
        }

        /// <summary>
        /// 목적지망 정보를 반환한다.<br></br>
        /// return : 목적지망 정보
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="dicDestSysPos"></param>
        /// <returns></returns>
        public string GetDestNetworkName(object obj, List<SGNetOverData> destinationInfoList)
        {
            string strDestNetwork = "";

            List<object> idList = obj.GetTagDataObjectList("net_id_list");

            if (idList == null || idList.Count == 0)
                return strDestNetwork;

            foreach (object netId in idList)
            {
                foreach (SGNetOverData sgNetOverData in destinationInfoList)
                {
                    if (netId.ToString() == sgNetOverData.strDestSysid)
                    {
                        strDestNetwork += $"{sgNetOverData.strDestSysName},";
                        break;
                    }
                }
            }

            if (strDestNetwork.Length > 0)
                strDestNetwork = strDestNetwork.Substring(0, strDestNetwork.Length - 1);

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
            if ((dic.TryGetValue(15, out strRecvPos) != true) || (dic.TryGetValue(2, out strTransKind) != true))
                return strRecvPos;

            strRecvPos = dic[16];
            strTransKind = dic[2];

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

        /// <summary>
        /// TransSequence 정보를 반환한다.<br></br>
        /// return : TransSequence 정보
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransSeq(object obj)
        {
            string objData = obj.GetTagDataString("trans_seq");

            return objData;
        }

        /// <summary>
        /// 결재 정보를 반환한다.<br></br>
        /// return 결재 정보(선결,후결)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprKind(object obj)
        {
            string objData = obj.GetTagDataString("approval_proc_type");


            string strApprKind = "";

            switch (objData)
            {
                case "pre":
                    strApprKind = xmlConf.GetTitle("T_COMMON_APPROVE_BEFORE");        // 선결
                    break;
                case "post":
                    strApprKind = xmlConf.GetTitle("T_COMMON_APPROVE_AFTER");        // 후결
                    break;
                default:
                    break;
            }

            return strApprKind;
        }

        public string GetDataType(object obj)
        {
            string strDataType = "";
            string objData = obj.GetTagDataString("data_type");

            switch (objData)
            {
                case "file":
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_NORMAL");        // 텍스트
                    break;
                case "cliptxt":
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_TEXT");        // 텍스트
                    break;
                case "clipimg":
                    strDataType = xmlConf.GetTitle("T_DATA_TYPE_IMAGE");        // 이미지
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
            if ((strTransStatus.Equals("W") || strTransStatus.Equals("V"))
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
        public bool GetTransCancelEnableChk(string userSeq, object obj)
        {
            //수신자는 전송취소 불가, 발신자만 취소 가능
            string reqUserSeq = obj.GetTagDataString("req_uesr_seq");
            string transState = obj.GetTagDataString("trans_state");
            string approvalState = obj.GetTagDataString("approval_state");

            if (reqUserSeq != userSeq)
                return false;

            if ((transState == "scanning" || transState == "wait") && approvalState != "reject")
                return true;                                                                                           // 전송상태가 전송대기 또는 검사중이고 결재상태가 반려가 아니라면.

            return false;
        }

        public void GetFileInfo(object selectedInfo, out List<FileInfoData> fileListInfo)
        {
            //List<Dictionary<int, string>> listDicdata = GetRecordData("FILERECORD");
            List<object> fileRecordList = selectedInfo.GetTagDataObjectList("file_info", "file_record_list");
            List<object> scanResultList = selectedInfo.GetTagDataObjectList("scan_result");

            if (fileRecordList == null)
            {
                fileListInfo = null;
                HsLog.err($"GetFileInfo, FILERECORD null !");
                return;
            }
            if (fileRecordList.Count <= 0)
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
            string strVirusFlag = "";

            List<FileInfoData> m_ListData = new List<FileInfoData>();
            foreach (object record in fileRecordList)
            {
                //TODO 고도화 - DLP 및 Prework에 대한 정의 필요
                //DLP 포함여부(1:포함)
                stDLP = record.GetTagDataObject("dlp").ToString();

                //DLP DESC
                stDLPDesc = GetScanFileDescription(selectedInfo, "DLP", record.GetTagDataString("file_seq"));
                if (string.IsNullOrEmpty(stDLPDesc))
                    stDLPDesc = "-";

                // 파일이름 
                strFileName = record.GetTagDataObject("file_name").ToString();
                if (string.IsNullOrEmpty(strFileName))
                    strFileName = "-";
                else
                    strFileName = GetFileRename(false, strFileName);

                // 파일 유형 
                strFileType = record.GetTagDataObject("file_type").ToString(); // NetLink호환 : 언어별로 다 찾아서 넣어줘야함, dir이라는걸 알 수 있는 값이 없음
                if (string.IsNullOrEmpty(strFileType.Trim()))
                    strFileType = "-";

                if (strFileType.ToUpper().Equals("DIR"))
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

                // 파일 Size
                strFileSize = record.GetTagDataObject("file_size").ToString();
                if (string.IsNullOrEmpty(strFileSize))
                    strFileSize = "-";
                else
                {
                    Int64 nSize = Convert.ToInt64(strFileSize);
                    strFileSize = GetSizeStr(nSize);
                }

                // 바이러스 내역
                //TODO 고도화 - scan_record_list 확립 후 수정
                strVirus = "";

                // 바이러스 검사일 
                strVirusExamDay = "-";

                // VIRUSFLAG : VIRUS 및 파일 위변조 검출여부
                strVirusFlag = "0";

                string strFileSeq = record.GetTagDataObject("file_seq").ToString();
                m_ListData.Add(new FileInfoData(strFileName, strFileType, strFileSize, strVirus, strVirusExamDay, strFileSeq, stDLP, stDLPDesc, strVirusFlag));
            }
            fileListInfo = m_ListData;
            return;
        }

        public List<ApproverHist> GetApproverInfoHist(object selectedInfo, bool isVisibleApproveReason)
        {
            List<ApproverHist> approverHist = new List<ApproverHist>();
            ApproverHist tmpApprover;

            List<object> apprStepList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
            if (apprStepList == null)
                return null;

            if (apprStepList.Count <= 0)
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
            foreach (object apprStep in apprStepList)
            {
                // 결재자 ID 
                strApprUserID = apprStep.GetTagDataObject("approver_hr", "approver_id").ToString();
                if (string.IsNullOrEmpty(strApprUserID))
                    strApprUserID = "-";


                // 결재자 이름
                strApprName = apprStep.GetTagDataObject("approver_hr", "name").ToString();
                if (string.IsNullOrEmpty(strApprName))
                    strApprName = "-";


                // 결재자 직급 및 직위
                strApprPos = apprStep.GetTagDataObject("approver_hr", "rank").ToString();
                if (string.IsNullOrEmpty(strApprPos))
                    strApprPos = "-";

                // 결재일
                strApprDate = apprStep.GetTagDataObject("resp_datetime").ToString();
                if (string.IsNullOrEmpty(strApprDate))
                    strApprDate = "-";

                // 결재 상태
                strApprStatus = apprStep.GetTagDataObject("approval_state").ToString();
                if (string.IsNullOrEmpty(strApprStatus))
                    strApprStatus = "-";

                // 반려 사유
                strApprReason = apprStep.GetTagDataObject("description").ToString();
                if (string.IsNullOrEmpty(strApprReason))
                    strApprReason = "-";

                // 결재 Step
                strApprStep = apprStep.GetTagDataObject("approval_step", "approval_order").ToString();
                if (string.IsNullOrEmpty(strApprStep))
                    strApprStep = "-";

                // 승인자 정보 (이름 + 직위)
                strApprName = strApprName + " " + strApprPos;        // 승인자 정보 (이름 + 직위)                


                //보안결재에 대한 계산 여부 불필요
                //if (Dictemp.TryGetValue(7, out strPrivacyAppr))      // 보안결재자 여부.
                //    strPrivacyAppr = Dictemp[7];
                //else
                strPrivacyAppr = "-";

                switch (strApprStatus)
                {
                    case "pre":
                    case "wait":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                        break;
                    case "confirm":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");                   // 승인
                        break;
                    case "reject":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");                 // 반려
                        break;
                    case "skip":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");             //요청 취소
                        break;
                    default:
                        strApprStatus = "-";
                        break;
                }


                tmpApprover = new ApproverHist();
                tmpApprover.SetData(strApprUserID, strApprName, strApprStatus, strApprDate, strApprReason, strApprStep);
                if (strApprStep != "-")
                    tmpApprover.m_nApprStep = int.Parse(strApprStep);
                else
                    tmpApprover.m_nApprStep = 0;
                approverHist.Add(tmpApprover);
            }

            return approverHist;
        }

        public string GetFileRename(bool bMode, string strFileName) => SgExtFunc.hsFileRename(bMode, strFileName);
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

            rtn = CsFunction.GetSizeStr(size);
            return rtn;
        }

        public string GetScanFileDescription(object selectedInfo, string scanType, string fileSeq)
        {
            List<object> scanList = selectedInfo.GetTagDataObjectList("scan_list");
            if (scanList?.Count < 1)
                return "-";

            foreach (object scan in scanList)
            {
                //해당 scanType 종류 검색
                if (scan.GetTagDataString("type").ToUpper() != scanType.ToUpper())
                    continue;

                //해당 파일 검출 내용 검색
                List<object> scanFiles = scan.GetTagDataObjectList("result", "scan_record_list");
                foreach (object file in scanFiles)
                {
                    if (file.GetTagDataString("file_seq") == fileSeq)
                        return file.GetTagDataString("description");
                }
            }

            return "-";
        }

        public ApprovalInfo GetLastApprInfo(object selectedInfo, bool isVisibleFileApproveReason)
        {
            ApprovalInfo retValue = new ApprovalInfo("-", "-", "-", "-", "-", "-", "-");

            //if (GetRequestCancelChk(selectedInfo) != 0)
            //    return retValue;

            string strApprStatus = GetApprStatus(selectedInfo);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            //TODO 고도화 - 대결재자인 경우, 원결재자를 반환하도록 처리 필요
            List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
            if (approvalStepStatusList?.Count <= 0)
                return retValue;

            int lastApprOrder = 0;
            foreach (object status in approvalStepStatusList)
            {
                string apprStat = status.GetTagDataString("approval_state");
                if (apprStat != "confirm" && apprStat != "reject")
                    continue;

                if(apprStat == "reject")
                {
                    retValue.ApproveStatusCode = apprStat;
                    retValue.ApproveStatusName = GetApprStatusName(apprStat);
                    retValue.UserID = status.GetTagDataString("approver_hr", "approver_id");
                    retValue.UserName = status.GetTagDataString("approver_hr", "name");
                    retValue.UserSeq = status.GetTagDataString("approver_hr", "approver_seq");

                    if(isVisibleFileApproveReason)
                        retValue.ApproveReason = status.GetTagDataString("description");
                    else
                        retValue.ApproveReason = "-";
                    retValue.ApproveTime = status.GetTagDataString("resp_datetime");
                    break;
                }
                
                string apprOrderString = status.GetTagDataString("approval_step", "approval_order");
                int.TryParse(apprOrderString, out int apprOrder);

                if (lastApprOrder < apprOrder)
                {
                    lastApprOrder = apprOrder;
                    retValue.ApproveStatusCode = apprStat;
                    retValue.ApproveStatusName = GetApprStatusName(apprStat);
                    retValue.UserID = status.GetTagDataString("approver_hr", "approver_id");
                    retValue.UserName = status.GetTagDataString("approver_hr", "name");
                    retValue.UserSeq = status.GetTagDataString("approver_hr", "approver_seq");
                    if (isVisibleFileApproveReason)
                        retValue.ApproveReason = status.GetTagDataString("description");
                    else
                        retValue.ApproveReason = "-";
                    retValue.ApproveTime = status.GetTagDataString("resp_datetime");
                }
            }
            return retValue;    //마지막 결재자 반환

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strApprStatus">
        /// <para>scanning 검사중</para>
        /// <para>wait 전송대기</para>
        /// <para>cancel 전송취소</para>
        /// <para>received PC수신완료</para>
        /// <para>fail 전송실패</para>
        /// </param>
        /// <param name="strTransStatus">
        /// <para></para>
        /// </param>
        /// <returns>요청취소 판단 값. (0 : 요청취소 조건 아님, 1: 사용자가 전송취소 한 경우, 2: 이전 결재자가 반려한 경우</returns>
        public int GetRequestCancelChk(object selectedInfo)
        {
            string strTransStatus = selectedInfo.GetTagDataString("trans_req", "trans_state");
            string strApprStatus = selectedInfo.GetTagDataString("trans_req", "approval_state");

            if (string.IsNullOrEmpty(strApprStatus) || string.IsNullOrEmpty(strTransStatus))
                return 0;
            /*
            - "scanning 검사중"
            - "wait 전송대기"
            - "cancel 전송취소"
            - "received PC수신완료"
            - "fail 전송실패"
            ------------------------------
             pre(이전 단계 진행중), wait(결재 대기), confirm(승인), reject(반려), cancel(취소), skip(결재 스킵)
            - "pre 이전 단계 진행중"
            - "wait 결재 대기"
            - "confirm 승인"
            - "reject 반려"
            - "cancel 취소"
            - "skip 결재스킵(타 결재자 처리)"
            */
            if (strTransStatus == "cancel" && strApprStatus == "wait")     // 전송상태가 전송취소이면서, 결재상태가 승인대기일때
                return 1;

            if (strTransStatus == "wait" && strApprStatus == "reject")      //전송상태가 전송대기이면서, 결재상태가 반려인 경우
                return 2;

            return 0;
        }
        public string GetApprStatusName(string ApprStatusCode) => ApprStatusCode switch
        {
            "pre" or "wait" => xmlConf.GetTitle("T_COMMON_APPROVE_WAIT"),              // 승인대기
            "confirm" => xmlConf.GetTitle("T_COMMON_APPROVE"),                   // 승인
            "reject" => xmlConf.GetTitle("T_COMMON_REJECTION"),                   // 반려
            "skip" => xmlConf.GetTitle("T_COMMON_REQUESTCANCEL"),                   // 요청취소
            _ => "-"
        };
    }
}
