using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class PCURLApproveParam : BaseParam
    {
        public PCURLApproveParam(string sday, string eday, string appstatus, string url, long seq, string reqname, string title, int listcount, int viewno)
        {
            SearchFromDay = sday;
            SearchToDay = eday;
            ApprStatus = appstatus;
            Url = url;
            UserSeq = seq;
            ReqUserName = reqname;
            Title = title;
            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
        }
        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        public string ApprStatus { get; set; }
        public string Url { get; set; }
        public long UserSeq { get; set; }
        public string ReqUserName { get; set; }
        public string Title { get; set; }
    }
}
