using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class MailParam : BaseParam
    {
        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        public string UserID { get; set; }
        public string TransStatus { get; set; }
        public string Title { get; set; }
        public string ApprStatus { get; set; }
        public string Receiver { get; set; }
        public string SystemId { get; set; }            //사용자가 접근한 시스템
    }
}
