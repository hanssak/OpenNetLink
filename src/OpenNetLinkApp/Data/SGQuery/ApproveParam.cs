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
        /// approval_state
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
    }
}
