using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class ApproveParam : BaseParam
    {
        public ApproveParam(string fromday, string today, string apprkind, string transkind, string apprstatus, string reqname, string title, string userid, int listcount, int viewno)
        {
            SearchFromDay = fromday;
            SearchToDay = today;
            ApprKind = apprkind;
            TransKind = transkind;
            ApprStatus = apprstatus;
            ReqUserName = reqname;
            Title = title;
            this.PageListCount = listcount;
            this.ViewPageNo = viewno;
            UserID = userid;
        }
        
        public string SearchFromDay { get; set; }
        public string SearchToDay { get; set; }
        public string ApprKind { get; set; }
        public string TransKind { get; set; }
        public string ApprStatus { get; set; }
        public string ReqUserName { get; set; }
        public string Title { get; set; }
        public string UserID { get; set; }
        public string APPROVE_TYPE_SFM { get; set; }    //1:대결자기준, 2:결재자기준
        public string SystemId { get; set; }            //사용자가 접근한 시스템
        public int DataType { get; set; }
    }
}
