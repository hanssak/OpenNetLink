using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.JSInterop;
using OpenNetLinkApp.Services;
using System.Threading.Tasks;
namespace OpenNetLinkApp.Data.SGQuery
{
    class MailApproveEx1Param : MailParam
    {
        public string ApproveFlag { get; set; } //on
        public string ReqUserName { get; set; }
        public string Sender { get; set; }  //on


        /// <summary>
        /// 1:대결자기준, 2:결재자기준  //on
        /// </summary>
        public string APPROVE_TYPE_SFM { get; set; }

        /// <summary>
        /// 보안결재UI 검색인지 유무
        /// </summary>
        public bool bIsDlpPrivacyApprove { get; set; } = false;

        public void SetSender(string strValue)
        {
            Sender = strValue;
        }
        

    }
}
