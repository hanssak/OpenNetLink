using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGApproveData
    {
        public bool bCheckDisable { get; set; }           // 체크 가능 불가능
        public bool bCheck { get; set; }                  // 체크 상태
        public string TransSeq { get; set; }              // TransSeq
        /// <summary>
        /// 고도화 시 필요한지 확인
        /// </summary>
        public string ApprSeq { get; set; }               // ApproveSeq
        public string ApvType { get; set; }               // 결재 종류 (선결/후결)
        public string TransferType { get; set; }          // 전송 구분 (반출/반입)
        //public string TransStatus { get; set; }           // 전송 상태 (전송대기, 전송취소, 전송실패)
        public string RequesterName { get; set; }         // 승인요청자

        /// <summary>
        /// 승인자
        /// </summary>
        public string ApproverName { get; set; }          // 승인자
        public string ApvStatus { get; set; }             // 결재상태(승인대기/승인/반려)
        //public string IsFile { get; set; }                // 파일 전달 유무
        public string strDestNet { get; set; }            // 전송하고자하는 목적지 망이름
        public string Title { get; set; }                 // 제목
        public string RequestTime { get; set; }           // 전송요청일
        public string ApvTime { get; set; }               // 승인일

        public string DataType { get; set; }              //전송파일 타입
        public string TransStatusCode { get; set; }         //전송상태 원본 ( W : 전송대기 , C : 전송취소 , S : 수신완료 , F : 전송실패 )
        /// <summary>
        /// 승인상태 원본
        /// <para>pre, wait, confirm, rejcet, skip</para>
        /// </summary>
        public string ApprStatusCode { get; set; }          
        //public string ApprTablePos { get; set; }            // 결재 데이터 위치 ( C : 결재 테이블 , H : 결재 이력 테이블 )
        public string ApprPossible { get; set; }            // 결재 가능 여부 ( 1: 가능 , 0 : 불가능)
        //public string ApprStepStatus { get; set; }          // 결재자가 포함된 결재단계의 결재 상태 ( 1 : 승인가능상태, 2 : 승인불가능상태 )
        public string stDLP { get; set; }           //개인정보 

        public SGApproveData()
        {
            bCheckDisable = bCheck = false;
            TransSeq = ApvType = TransferType = RequesterName = ApproverName = ApvStatus = Title = RequestTime = ApvTime = TransStatusCode = ApprStatusCode = ApprPossible = strDestNet = "";
        }
        public SGApproveData(bool bChkDisable, bool bChk, string seq, string apprseq, string apvtype, string transfertype, string reqname, string apvstatus, string isfile, string title, string reqtime, string apvtime,
            string strTransStatusCode, string strApprStatusCode, string strApprTablePos, string strApprPossible, string strApprStepStatus, string approverName)
        {
            bCheckDisable = bChkDisable;
            bCheck = bChk;
            TransSeq = seq;
            ApvType = apvtype;
            TransferType = transfertype;
            RequesterName = reqname;
            ApvStatus = apvstatus;
            //IsFile = isfile;
            Title = title;
            RequestTime = reqtime;
            ApvTime = apvtime;
            TransStatusCode = strTransStatusCode;
            ApprStatusCode = strApprStatusCode;
            //ApprTablePos = strApprTablePos;
            ApprPossible = strApprPossible;
            //ApprStepStatus = strApprStepStatus;
            ApproverName = approverName;
        }
    }
}
