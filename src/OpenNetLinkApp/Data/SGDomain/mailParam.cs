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
        public async Task<string> SetSearchStartDate(string pickerId)   //datepicker, datepicker2
        {
            string rtn = "";
            object[] param = { pickerId };
            string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

            char sep = '-';
            string[] splitFrom = vStr.Split(sep);
            rtn = String.Format("{0}{1}{2}000000", splitFrom[0], splitFrom[1], splitFrom[2]);
            SearchStartDate = rtn;
            return rtn;
        }
        public async Task<string> SetSearchEndDate(string pickerId)   //datepicker, datepicker2
        {
            string rtn = "";
            object[] param = { pickerId };
            string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

            char sep = '-';
            string[] splitFrom = vStr.Split(sep);
            rtn = String.Format("{0}{1}{2}999999", splitFrom[0], splitFrom[1], splitFrom[2]);
            SearchEndDate = rtn;
            return rtn;
        }

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
        public string GetDlpValueCode()
        {
            string rtn = "";
            if (DlpValue == null || DlpValue.Length == 0)
                return rtn;
            if (DlpValue == XmlConf.GetTitle("T_COMMON_EXIST")) //있음
                return "1";
            if (DlpValue == XmlConf.GetTitle("T_COMMON_NOTEXIST")) //없음
                return "2";
            return rtn;
        }
        public string GetTransStatusCode()
        {
            string rtn = "";
            if (TransStatus == null || TransStatus.Length == 0)
                return rtn;
            if (TransStatus == XmlConf.GetTitle("T_DETAIL_TRANS_SUCCESS")) //전송완료
                return "3";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSWAIT")) //전송대기
                return "1";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSCANCLE")) //전송취소
                return "5";
            return rtn;
        }
        public string GetApprStatusCode()
        {
            string rtn = "";
            if (ApprStatus == null || ApprStatus.Length == 0)
                return rtn;
            if (ApprStatus == XmlConf.GetTitle("T_COMMON_APPROVE")) //승인
                return "2";
            if (ApprStatus == XmlConf.GetTitle("T_COMMON_APPROVE_WAIT")) //승인대기
                return "1";
            if (ApprStatus == XmlConf.GetTitle("T_COMMON_REJECTION")) //반려
                return "3";
            if (ApprStatus == XmlConf.GetTitle("T_COMMON_REQUESTCANCEL")) //요청취소
                return "5";
            return rtn;
        }
    }
}
