using System;
using System.Collections.Generic;
using System.Text;

//목적 : 컴포넌트간 Drag & Drop 에서 사용되는 데이타 전달용 서비스 클래스
//제작 : YKH , 2020/06/11
namespace OpenNetLinkApp.Services
{
    class DragAndDropService
    {
        public string Data { get; set; }
        public string Zone { get; set; }
        private List<string> dataList = new List<string>();

        public List<string> getDataList()
        {
            return dataList;
        }
        public void AddDataList(string str)
        {
            bool bFind = false;
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i] == str)
                {
                    bFind = true;
                    break;
                }
            }
            if (bFind == false)
                dataList.Add(str);
        }
        public void removeDataList(string str)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i] == str)
                {
                    dataList.RemoveAt(i);
                    break;
                }
            }
        }
        public void clearDataList()
        {
            dataList.Clear();
        }
        public void StartDrag(string data, string zone)
        {
            this.Data = data;
            this.Zone = zone;
        }
        public bool Accepts(string zone)
        {
            return this.Zone == zone;
        }
        public void Clear()
        {
            this.Data = "";
            this.Zone = "";
        }
    }
}
