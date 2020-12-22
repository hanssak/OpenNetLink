using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class TransferParam : BaseParam
    {
        public TransferParam(string kind, string tstatus, string astatus, string title, string fday, string eday, string id, int listcount, int viewno)
        {
            TransKind = kind;
            TransStatus = tstatus;
            ApprStatus = astatus;
            Title = title;
            SearchFromDay = fday;
            SearchToDay = eday;
            UserID = id;
            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
        }
        public string TransKind { get; set; }   //전송구분(ioType)
        public string TransStatus { get; set; }
        public string ApprStatus { get; set; }
        public string Title { get; set; }
        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        public string UserID { get; set; }
        public int DataType { get; set; }
        public string Src_system_id { get; set; }
        public string Dest_system_id { get; set; }
    }
}
