using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class MailApproveParam : BaseParam
    {
        public string SearchFromDay { get; set; }//on
        public string SearchToDay { get; set; }//on
        public string ApprKind { get; set; } //on
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
    }
}
