using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OpenNetLinkApp.Services;
using Microsoft.JSInterop;
namespace OpenNetLinkApp.Data.SGQuery
{
    class TransferParam : BaseParam
    {
        public XmlConfService XmlConf = null;  //초기설정 필요
        public IJSRuntime jsRuntime = null;    //초기설정 필요
        public TransferParam()
        {
            TransKind = String.Empty;
            TransStatus = String.Empty;
            ApprStatus = String.Empty;
            Title = String.Empty;
            SearchFromDay = String.Empty;
            SearchToDay = String.Empty;
            UserID = String.Empty;
            this.PageListCount = 20;
            this.ViewPageNo = 1;
        }
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

        public TransferParam(string kind, string tstatus, string astatus, string dtype, string title, string fday, string eday, string id, int listcount, int viewno)
        {
            TransKind = kind;
            TransStatus = tstatus;
            ApprStatus = astatus;
            DataType = Convert.ToInt32(dtype);
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

        public async Task<string> SetSearchStartDate(string pickerId)   //datepicker, datepicker2
        {
            string rtn = "";
            object[] param = { pickerId };
            string vStr = await jsRuntime.InvokeAsync<string>("getElementValue", param);

            char sep = '-';
            string[] splitFrom = vStr.Split(sep);
            rtn = String.Format("{0}{1}{2}000000", splitFrom[0], splitFrom[1], splitFrom[2]);
            SearchFromDay = rtn;
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
            SearchToDay = rtn;
            return rtn;
        }
    }
}
