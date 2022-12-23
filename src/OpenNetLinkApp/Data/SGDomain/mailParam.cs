using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNetLinkApp.Services;
using Microsoft.JSInterop;
namespace OpenNetLinkApp.Data.SGDomain
{
    class MailParam
    {
        public XmlConfService XmlConf = null;  //초기설정 필요
        public IJSRuntime jsRuntime = null;    //초기설정 필요
        public string SystemId { get; set; }
        public string UserID { get; set; }
        public string SearchStartDate = "";
        public string SearchEndDate = "";

        /// <summary>
        /// 값 직접사용금지, 반드시 함수를 통해 사용(선결,후결)
        /// </summary>
        public string ApproveKind = "";     //선결,후결

        /// <summary>
        /// 값 직접사용금지, 반드시 함수를 통해 사용(반입,반출)
        /// </summary>
        public string TransKind = "";

        /// <summary>
        /// 값 직접사용금지, 반드시 함수를 통해 사용(...)
        /// </summary>
        public string ApprStatus = "";

        /// <summary>
        /// 값 직접사용금지, 반드시 함수를 통해 사용(...)
        /// </summary>
        public string TransStatus = "";

        /// <summary>
        /// 값 직접사용금지, 반드시 함수를 통해 사용(...)
        /// </summary>
        public string DlpValue = "";

        public string Receiver = "";
        public string Title = "";   //UTF-8변환 필요한듯(?)
        public int PageListCount = 10;
        public int ViewPageNo = 1;


        public async Task<string> GetSearchStartDate(string pickerId)   //datepicker, datepicker2
        {
            string rtn = "";
            object[] param = { pickerId };
            string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

            char sep = '-';
            string[] splitFrom = vStr.Split(sep);
            rtn = String.Format("{0}{1}{2}000000", splitFrom[0], splitFrom[1], splitFrom[2]);
            return rtn;
        }

        public void SetSearchStartDate(string Value)
        {
            SearchStartDate = Value;
        }

        public void SetSearchEndDate(string Value)
        {
            SearchEndDate = Value;
        }

        #region [사용안함]
        //public async Task<string> SetSearchStartDate(string pickerId)   //datepicker, datepicker2
        //{
        //    string rtn = "";
        //    object[] param = { pickerId };
        //    string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

        //    char sep = '-';
        //    string[] splitFrom = vStr.Split(sep);
        //    rtn = String.Format("{0}{1}{2}000000", splitFrom[0], splitFrom[1], splitFrom[2]);
        //    SearchStartDate = rtn;
        //    return rtn;
        //}
        //public async Task<string> SetSearchEndDate(string pickerId)   //datepicker, datepicker2
        //{
        //    string rtn = "";
        //    object[] param = { pickerId };
        //    string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

        //    char sep = '-';
        //    string[] splitFrom = vStr.Split(sep);
        //    rtn = String.Format("{0}{1}{2}999999", splitFrom[0], splitFrom[1], splitFrom[2]);
        //    SearchEndDate = rtn;
        //    return rtn;
        //}

        #endregion
        public string GetTransKindCode()
        {
            return TransKind;
        }
        public void SetTransKindCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_ALL"))
                TransKind = "";
            else if (value == XmlConf.GetTitle("T_COMMON_IMPORT"))  // 반입
                TransKind = "2";
            else if (value == XmlConf.GetTitle("T_COMMON_EXPORT"))  // 반출
                TransKind = "1";
            else
                TransKind = "";
        }

        public string GetApproveKindCode()
        {
            string rtn = "";
            if (ApproveKind == null || ApproveKind.Length == 0)
                return rtn;
            if (ApproveKind == XmlConf.GetTitle("T_COMMON_APPROVE_BEFORE")) //선결
                return "0";
            if (ApproveKind == XmlConf.GetTitle("T_COMMON_APPROVE_AFTER")) //후결
                return "1";
            return rtn;
        }

        public void SetApprKindCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_ALL"))
                ApproveKind = "";
            else if (value == XmlConf.GetTitle("T_DETAIL_BEFORE_APPROVE"))
                ApproveKind = "0";
            else if (value == XmlConf.GetTitle("T_DETAIL_AFTER_APPROVE"))
                ApproveKind = "1";
        }

        /// <summary>
        /// 저장할때 꼭 이함수를 사용해서 저장해야 정상적으로 Query 가능함.
        /// </summary>
        /// <param name="strValue"></param>
        public void SetApprStatusCode(string strValue)
        {
            if (strValue == XmlConf.GetTitle("T_DASH_APPROVE_COMPLETE")) //승인
                ApprStatus = "2";
            else if (strValue == XmlConf.GetTitle("T_COMMON_APPROVE_WAIT")) //승인대기
                ApprStatus = "1";
            else if (strValue == XmlConf.GetTitle("T_DASH_APPROVE_REJECT")) //반려
                ApprStatus = "3";
        }

        public string GetApprStatusCode()
        {
            return ApprStatus;
        }

        public void SetTransStatusCode(string strValue)
        {
            if (strValue == XmlConf.GetTitle("T_MAIL_TRANSWAIT")) //발송대기
                TransStatus = "W";
            else if (strValue == XmlConf.GetTitle("T_MAIL_TRANSCANCLE")) //발송취소
                TransStatus = "C";
            else if (strValue == XmlConf.GetTitle("T_MAIL_TRANS_SUCCESS")) //발송완료
                TransStatus = "S";
            else if (strValue == XmlConf.GetTitle("T_MAIL_TRANSFRFAILED")) //발송실패 
                TransStatus = "F";
            else if (strValue == XmlConf.GetTitle("T_MAIL_INSPECT")) //검사중
                TransStatus = "V";
            else
                TransStatus = "";   // 전체
        }

        /// <summary>
        /// TransStatus 값 return, 전달인자 true : 기존 code 호환 값 return
        /// </summary>
        /// <param name="bOldStyle"></param>
        /// <returns></returns>
        public string GetTransStatusCode(bool bOldStyle = true)
        {
            if (TransStatus == null || TransStatus.Length == 0)
                return "";

            if (bOldStyle)
            {
                if (TransStatus == "S") //발송완료
                    return "7";
                if (TransStatus == "W") //발송대기
                    return "9";
                if (TransStatus == "C") //발송취소
                    return "5";
                if (TransStatus == "F") //발송실패 
                    return "8";
            }

            return TransStatus;
            
        }

        /// <summary>
        /// DLP 검색조건 값 설정
        /// </summary>
        /// <param name="strDlpValue"></param>
        public void SetDlpValue(string strDlpValue)
        {
            if (strDlpValue == XmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION")) //미포함
                DlpValue = "2";
            else if (strDlpValue == XmlConf.GetTitle("T_COMMON_DLP_INCLUSION")) //포함
                DlpValue = "1";
            else if (strDlpValue == XmlConf.GetTitle("T_COMMON_DLP_UNKNOWN")) //검출불가
                DlpValue = "3";
            else
                DlpValue = "";
        }

        public string GetDlpValue()
        {
            return DlpValue;
        }

        /// <summary>
        /// 예전 Code 호환 위해 남겨 놓음
        /// </summary>
        /// <returns></returns>
        public string GetDlpValueCode()
        {
            if (DlpValue == null || DlpValue.Length == 0)
                return "";

            return DlpValue;
        }


        public void SetReceiver(string strValue)
        {
            Receiver = strValue;
        }

        public string GetReceiver()
        {
            return Receiver;
        }

        public void SetTitle(string strValue)
        {
            Title = strValue;
        }

        public string GetTitle()
        {
            return Title;
        }


    }
}
