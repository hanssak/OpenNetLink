using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class BoardParam : BaseParam
    {
        public string UserID { get; set; }

        public BoardParam(string userid, int listcount, int viewno)
        {
            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
            UserID = userid;
        }
    }
}
