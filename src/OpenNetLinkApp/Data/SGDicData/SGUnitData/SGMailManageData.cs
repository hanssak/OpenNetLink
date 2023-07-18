using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    
    class SGMailManageData : SGData
    {
        XmlConfService xmlConf;
        public SGMailManageData()
        {
            xmlConf = new XmlConfService();
        }
        ~SGMailManageData()
        {

        }
        public override void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public int GetSearchResultCount()
        {
            string strData = GetTagData("SEQCOUNT");
            int count = 0;
            if (!strData.Equals(""))
                count = Convert.ToInt32(strData);
            return count;
        }

        public List<Dictionary<int, string>> GetSearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("TRANSRECORD");
            return listDicdata;
        }

        public List<Dictionary<int, string>> GetSearchData(string strSelTransStatus, string strSelApprStatus)
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("TRANSRECORD");

            List<Dictionary<int, string>> resultDicData = new List<Dictionary<int, string>>();

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return listDicdata;

            for (int i = 0; i < dataCount; i++)
            {
                Dictionary<int, string> temp = listDicdata[i];
                string strTransStatus = "";
                string strApprStatus = "";
                if ((temp.TryGetValue(3, out strTransStatus) != true) || (temp.TryGetValue(5, out strApprStatus) != true))
                    continue;

                strTransStatus = temp[3];
                strApprStatus = temp[5];

                if ((strSelTransStatus.Equals("W")) && (strApprStatus.Equals("3")))
                    continue;

                if ((strSelApprStatus.Equals("1")) && (strTransStatus.Equals("C")))
                    continue;

                resultDicData.Add(temp);
            }

            return resultDicData;
        }

        public List<Dictionary<int, string>> GetQuerySearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetSvrRecordData("RECORD");

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }
        public static string ReturnMessage(eTransManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eTransManageMsg.eNone:
                    strMsg = "";
                    break;
                case eTransManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eTransManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
                case eTransManageMsg.eTransCancelError:
                    strMsg = xmlConf.GetErrMsg("E_0206");       // 전송 취소 중 오류가 발생되었습니다.
                    break;
                case eTransManageMsg.eTransCancelSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0021");          // 전송취소가 완료되었습니다.
                    break;
            }

            return strMsg;
        }




    }
}
