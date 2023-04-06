using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.JSInterop;
using OpenNetLinkApp.Services;
using System.Threading.Tasks;
namespace OpenNetLinkApp.Data.SGQuery
{
    class MailApproveParam : BaseParam
    {
        public XmlConfService XmlConf = null;  //초기설정 필요
        public IJSRuntime jsRuntime = null;    //초기설정 필요
        public string SearchFromDay { get; set; }//on
        public string SearchToDay { get; set; }//on
        public string TransKind { get; set; }
        public string ApprStatus { get; set; }
        public string ApproveFlag { get; set; } //on
        public string TransStatus { get; set; } //on
        public string ReqUserName { get; set; }
        public string Sender { get; set; }  //on
        public string Receiver { get; set; }    //on
        public string Title { get; set; } //on
        public string UserID { get; set; }//on
        public string APPROVE_TYPE_SFM { get; set; }    //1:대결자기준, 2:결재자기준  //on
        public string SystemId { get; set; }            //사용자가 접근한 시스템  //on
        public string DlpValue = "";
        public string ApproveKind = "";     //선결,후결

        /// <summary>
        /// 선결, 후결 설정값을 알려준다.
        /// </summary>
        /// <returns></returns>
        public string GetApproveKindCode()
        {
            if (ApproveKind == null || ApproveKind.Length == 0)
                return "";

            return ApproveKind;
        }

        public void SetApproveKindCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_APPROVE_BEFORE"))
                ApproveKind = "0";
            else if (value == XmlConf.GetTitle("T_COMMON_APPROVE_AFTER"))
                ApproveKind = "1";
            else
                ApproveKind = "";
        }

        public void SetSearchStartDate(string Value)
        {
            SearchFromDay = Value;
        }
        public void SetSearchEndDate(string Value)
        {
            SearchToDay = Value;
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
        public void SetDlpValueCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_DLP_INCLUSION"))
                DlpValue = "1";
            else if (value == XmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION"))
                DlpValue = "2";
            else
                DlpValue = "";
        }
        public string GetTransStatusCode()
        {
            string rtn = "";
            if (TransStatus == null || TransStatus.Length == 0)
                return rtn;
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANS_SUCCESS")) //전송완료
                return "7";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSWAIT")) //전송대기
                return "1";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSCANCLE")) //전송취소
                return "5";
            if (TransStatus == XmlConf.GetTitle("T_COMMON_TRANSFAIL")) //전송실패 
                return "8";
            return rtn;
        }
        public void SetTransStatusCode(string value)
        {
            if (value == XmlConf.GetTitle("T_COMMON_TRANS_SUCCESS"))
                TransStatus = "7";
            else if (value == XmlConf.GetTitle("T_COMMON_TRANSWAIT"))
                TransStatus = "1";
            else if (value == XmlConf.GetTitle("T_COMMON_TRANSCANCLE"))
                TransStatus = "5";
            else if (value == XmlConf.GetTitle("T_COMMON_TRANSFAIL"))
                TransStatus = "8";
            else
                TransStatus = "";
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
        public void SetApprStatusCode(string value)
        {
            if (value == XmlConf.GetTitle("T_DASH_APPROVE_COMPLETE"))
                ApprStatus = "2";
            else if (value == XmlConf.GetTitle("T_COMMON_APPROVE_WAIT"))
                ApprStatus = "1";
            else if (value == XmlConf.GetTitle("T_DASH_APPROVE_REJECT"))
                ApprStatus = "3";
            else
                ApprStatus = "";
        }

        public void SetReceiver(string strValue)
        {
            Receiver = strValue;
        }
        public void SetTitle(string strValue)
        {
            Title = strValue;
        }

        public void SetSender(string strValue)
        {
            Sender = strValue;
        }
        

    }
}
