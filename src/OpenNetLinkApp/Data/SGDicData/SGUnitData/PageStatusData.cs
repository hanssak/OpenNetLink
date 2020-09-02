using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class PageStatusData
    {
        public List<HsStream> hsStreamList = null;
        public PageStatusData()
        {
            hsStreamList = new List<HsStream>();
        }
        ~PageStatusData()
        {

        }
        public void FileDragListClear()
        {
            hsStreamList.Clear();
        }

        public void SetFileDragData(HsStream hs)
        {
            if (hs == null)
                return;
            hsStreamList.Add(hs);
        }
        public void SetFileDragListData(List<HsStream> hsList)
        {
            if (hsList == null)
                return;

            hsStreamList.Clear();
            hsStreamList = new List<HsStream>(hsList);
        }
        public List<HsStream> GetFileDragListData()
        {
            return hsStreamList;
        }
    }
}
