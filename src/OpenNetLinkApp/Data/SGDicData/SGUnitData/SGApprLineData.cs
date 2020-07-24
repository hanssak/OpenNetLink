using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGApprLineData : SGData
    {
        public SGApprLineData()
        {

        }
        ~SGApprLineData()
        {

        }
        public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public List<string> GetApprAndLineName()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for(int i=0;i<nTotalCount;i++)                              // UI 에서 사용하기 위해 자기 자신을 포함하기 위해 i = 0 부터 시작.                  
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(2, out tmpStr) == true)
                {
                    tmpStr = dic[2];
                    if(!tmpStr.Equals(""))
                        listApprLine.Add(tmpStr);
                }
            }
            return listApprLine;
        }

        public List<string> GetApprAndLineSeq()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 1; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(0, out tmpStr) == true)
                {
                    tmpStr = dic[0];
                    if (!tmpStr.Equals(""))
                        listApprLine.Add(tmpStr);
                }
            }
            return listApprLine;
        }
    }
}
