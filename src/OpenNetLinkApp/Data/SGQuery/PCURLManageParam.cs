using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class PCURLManageParam : BaseParam
    {
        public PCURLManageParam(string sday, string eday, long seq, string appstatus, string title, string url, int listcount, int viewno)
        {
            SearchFromDay = sday;
            SearchToDay = eday;
            UserSeq = seq;
            ApprStatus = appstatus;
            Title = title;
            Url = url;
            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
        }

        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        public long UserSeq { get; set; }
        public string ApprStatus { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
