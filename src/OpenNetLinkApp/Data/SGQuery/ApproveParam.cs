using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    public class ApproveParam : BaseParam
    {
        XmlConfService XmlConf;
        string strTotal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromday"></param>
        /// <param name="today"></param>
        /// <param name="apprkind">pre, post</param>
        /// <param name="transkind"></param>
        /// <param name="apprstatus"></param>
        /// <param name="reqname"></param>
        /// <param name="title"></param>
        /// <param name="userid"></param>
        /// <param name="listcount"></param>
        /// <param name="viewno"></param>
        public ApproveParam(string fromday, string today, string ApprKindValue, string TransKindValue, string ApprStatusValue, string reqname, string title, string userid, int listcount, int viewno, XmlConfService xmlConf)
        {
            XmlConf = xmlConf;
            strTotal = XmlConf.GetTitle("T_COMMON_ALL");

            SearchFromDay = fromday;
            SearchToDay = today;
            ApprKind = GetApprKind(ApprKindValue);
            TransKind = GetTransKind(TransKindValue);
            ApprStatus = GetApprStatus(ApprStatusValue);
            ReqUserName = reqname;
            Title = title;
            UserID = userid;
            this.NetWorkType = EnumNetWorkType.Single;
            this.IsSecurity = false;

            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
        }

        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        /// <summary>
        /// pre(사전), post(사후)
        /// </summary>
        public List<string> ApprKind { get; set; }  //approval_proc_type_list (pre, post)
        /// <summary>
        /// I(반입), E(반출)
        /// </summary>
        public List<string> TransKind { get; set; }

        /// <summary>
        /// pre(이전 단계 진행중), wait(결재 대기), confirm(승인), reject(반려), cancel(취소), skip(결재 스킵)"
        /// </summary>
        public List<string> ApprStatus { get; set; }

        /// <summary>
        /// 발송상태
        /// </summary>
        public List<string> MailDeliveryStatus { get; set; }

        /// <summary>
        /// 수신자
        /// </summary>
        public string MailReceiver { get; set; }

        /// <summary>
        /// 개인정보 검출 여부
        /// </summary>
        public List<string> PrivacyDetectFlag { get; set; }

        /// <summary>
        /// 승인요청자 OR 발신자
        /// </summary>
        public string ReqUserName { get; set; }
        public string Title { get; set; }
        public string UserID { get; set; }
        public string APPROVE_TYPE_SFM { get; set; }    //1:대결자기준, 2:결재자기준
        public string SystemId { get; set; }            //사용자가 접근한 시스템
        /// <summary>
        /// 데이터 타입 
        /// (file, email, cliptxt, clipbmp, npki, pcurl)
        /// </summary>
        public List<string> DataType { get; set; }

        /// <summary>
        /// 전송 시작점
        /// </summary>
        public string SrcSGNetType { get; set; }    //전송 시작점
        /// <summary>
        /// 전송 목적지
        /// </summary>
        public List<string> DestSGNetType { get; set; } //전송 목적지

        //public string Src_system_id { get; set; }
        //public string Dest_system_id { get; set; }

        public EnumNetWorkType NetWorkType { get; set; } //단일망, 다중망

        public bool IsSecurity { get; set; }

        /// <summary>
        /// 선결/후결
        /// </summary>
        /// <param name="strApprKindValue"></param>
        /// <returns></returns>
        public List<string> GetApprKind(string strApprKindValue)
        {
            List<string> strValue = new List<string>();

            if (strApprKindValue.Equals(strTotal))                         // 전체
            {
                strValue.Add("pre");
                strValue.Add("post");
            }
            else if (strApprKindValue.Equals(XmlConf.GetTitle("T_DETAIL_BEFORE_APPROVE")))                  // 선결
                strValue.Add("pre");
            else if (strApprKindValue.Equals(XmlConf.GetTitle("T_DETAIL_AFTER_APPROVE")))                // 후결
                strValue.Add("post");

            return strValue;
        }

        /// <summary>
        /// 반입/반출
        /// </summary>
        /// <param name="strTransKindValue"></param>
        /// <returns></returns>
        public List<string> GetTransKind(string strTransKindValue)
        {
            List<string> strValue = new List<string>();

            if (strTransKindValue.Equals(strTotal))                         // 전체
            {
                strValue.Add("I");
                strValue.Add("E");
            }
            else if (strTransKindValue.Equals(XmlConf.GetTitle("T_COMMON_EXPORT")))                 // 반출
                strValue.Add("E");
            else if (strTransKindValue.Equals(XmlConf.GetTitle("T_COMMON_IMPORT")))                  // 반입
                strValue.Add("I");

            return strValue;
        }

        /// <summary>
        /// approval_state (승인대기,승인...
        /// </summary>
        /// <returns></returns>
        public List<string> GetApprStatus(string strApprStatusValue)
        {
            //pre(이전 단계 진행중), wait(결재 대기), confirm(승인), reject(반려), cancel(취소), skip(결재 스킵)"
            List<string> strValue = new List<string>();

            if (strApprStatusValue.Equals(strTotal))                        // 전체
            {
                strValue.Add("pre");
                strValue.Add("wait");
                strValue.Add("confirm");
                strValue.Add("reject");
                strValue.Add("cancel");
                strValue.Add("skip");
            }
            else if (strApprStatusValue.Equals(XmlConf.GetTitle("T_COMMON_APPROVE_WAIT")))             // 승인대기
            {
                strValue.Add("pre");
                strValue.Add("wait");
            }
            else if (strApprStatusValue.Equals(XmlConf.GetTitle("T_COMMON_APPROVE")))                 // 승인
            {
                strValue.Add("confirm");
                strValue.Add("skip");
            }
            else if (strApprStatusValue.Equals(XmlConf.GetTitle("T_COMMON_REJECTION")))                  // 반려
                strValue.Add("reject");
            else if (strApprStatusValue.Equals(XmlConf.GetTitle("T_COMMON_REQUESTCANCEL")))           // 요청취소
                strValue.Add("cancel");

            return strValue;
        }

        /// <summary>
        /// 선택된 망 명칭을 기준으로 전송건 조회 중 목적지 조건 추가
        /// </summary>
        /// <param name="strDestinationValue"></param>
        /// <param name="DestInfo"></param>
        /// <returns></returns>
        public List<string> GetDestination(string strDestinationValue, Dictionary<string, SGNetOverData> DestInfo)
        {
            List<string> strValue = new List<string>();
            foreach (SGNetOverData Dest in DestInfo?.Values)
            {
                if (DestInfo?.Count == 1)
                    strValue.Add(Dest.strDestSysid);
                else if (strDestinationValue == strTotal)
                    strValue.Add(Dest.strDestSysid);
                else if (strDestinationValue == Dest.strDestSysName)
                    strValue.Add(Dest.strDestSysid);
            }
            return strValue;
        }

        /// <summary>
        /// 메일 발송상태 조건 추가
        /// </summary>
        /// <param name="strStatusText">전체, 발송대기, 발송취소 등등</param>
        public void SetMailDeliveryStatus(string strStatusText) => MailDeliveryStatus = GetMailDeliveryStatus(strStatusText);
        public List<string> GetMailDeliveryStatus(string strStatusText)
        {
            //TODO 고도화 - 메일 결재 조회 시, 발송상태를 조회 조건에 추가 
            List<string> strValue = new List<string>();

            if (strStatusText == XmlConf.GetTitle("T_MAIL_TRANSWAIT")) //발송대기
                strValue.Add("발송대기");
            else if (strStatusText == XmlConf.GetTitle("T_MAIL_TRANSCANCLE")) //발송취소
                strValue.Add("발송취소");
            else if (strStatusText == XmlConf.GetTitle("T_MAIL_TRANS_SUCCESS")) //발송완료
                strValue.Add("발송완료");
            else if (strStatusText == XmlConf.GetTitle("T_MAIL_TRANSFRFAILED")) //발송실패 
                strValue.Add("발송실패");
            else if (strStatusText == XmlConf.GetTitle("T_MAIL_INSPECT")) //검사중
                strValue.Add("검사중");
            else
                strValue.Add("전체");

            return strValue;
        }

        /// <summary>
        /// 개인정보 검출 여부 추가
        /// </summary>
        /// <param name="strStatusTest">전체, 미포함, 포함, 검출불가</param>
        public void SetPrivacyDetectFlag(string strStatusTest) => PrivacyDetectFlag = GetPrivacyDetectFlag(strStatusTest);
        /// <summary>
        /// 개인정보 검출 여부 조건 추가
        /// </summary>
        /// <param name="strDlpValue">전체, 미포함, 포함, 검출불가</param>
        public List<string> GetPrivacyDetectFlag(string strDlpStatusText)
        {
            //TODO 고도화 - 메일 결재 조회 시, 개인정보 검출 여부를 조회 조건에 추가 
            List<string> strValue = new List<string>();

            if (strDlpStatusText == XmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION")) //미포함
                strValue.Add("미포함");
            else if (strDlpStatusText == XmlConf.GetTitle("T_COMMON_DLP_INCLUSION")) //포함
                strValue.Add("포함");
            else if (strDlpStatusText == XmlConf.GetTitle("T_COMMON_DLP_UNKNOWN")) //검출불가
                strValue.Add("검출불가");
            else
                strValue.Add("전체");

            return strValue;
        }
    }
}
