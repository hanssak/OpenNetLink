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
        public string TransKind = "";
        public string TransStatus = "";
        public string ApprStatus = "";
        public string DlpValue = "";
        public string Title = "";   //UTF-8변환 필요한듯(?)
        public string Receiver = "";
        public int PageListCount = 10;
        public int ViewPageNo = 1;
        public string ApproveKind = "";     //선결,후결


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
            string rtn = "";
            if (TransKind == null || TransKind.Length == 0)
                return rtn;
            if (TransKind == XmlConf.GetTitle("T_COMMON_IMPORT")) //반입
                return "R";
            if (TransKind == XmlConf.GetTitle("T_COMMON_EXPORT")) //반출
                return "S";
            return rtn;
        }
        public void SetTransKindCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_ALL"))
                TransKind = "";
            else if (value == XmlConf.GetTitle("T_COMMON_IMPORT"))
                TransKind = "R";
            else if (value == XmlConf.GetTitle("T_COMMON_EXPORT"))
                TransKind = "S";
        }

        public string GetDlpValueCode()
        {
            string rtn = "";
            if (DlpValue == null || DlpValue.Length == 0)
                return rtn;
            if (DlpValue == XmlConf.GetTitle("T_COMMON_DLP_INCLUSION")) //포함
                return "1";
            if (DlpValue == XmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION")) //미포함
                return "2";
            return rtn;
        }
        public string GetTransStatusCode()
        {
            string rtn = "";
            if (TransStatus == null || TransStatus.Length == 0)
                return rtn;
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANS_SUCCESS")) //전송완료
                return "7";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSWAIT")) //전송대기
                return "9";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSCANCLE")) //전송취소
                return "5";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSFAIL")) //전송실패 
                return "8";
            return rtn;
        }
        public string GetApprStatusCode()
        {
            string rtn = "";
            if (ApprStatus == null || ApprStatus.Length == 0)
                return rtn;
            if (ApprStatus == XmlConf.GetTitle("T_DASH_APPROVE_COMPLETE")) //승인
                return "2";
            if (ApprStatus == XmlConf.GetTitle("T_COMMON_APPROVE_WAIT")) //승인대기
                return "1";
            if (ApprStatus == XmlConf.GetTitle("T_DASH_APPROVE_REJECT")) //반려
                return "3";
            return rtn;
        }
    }
}
