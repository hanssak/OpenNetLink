using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class BaseParam
    {
        public int PageListCount { get; set; }  //한리스트에 뿌려질 총 리스트수
        public int ViewPageNo { get; set; }     //패이지 수
    }
}
