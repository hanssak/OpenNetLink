using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eApprManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2,
        eApprBatchError = 3,
        eApprBatchActionSuccess = 4,
        eApprBatchRejectSuccess = 5
    }
    public class SGApprManageData : SGData
    {
        XmlConfService xmlConf;
        public SGApprManageData()
        {
            xmlConf = new XmlConfService();
        }

        ~SGApprManageData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        public int GetSearchResultCount()
        {
            string strData = GetTagData("APPROVECOUNT");
            int count = 0;
            if (!strData.Equals(""))
                count = Convert.ToInt32(strData);
            return count;
        }
        public static string ReturnMessage(eApprManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eApprManageMsg.eNone:
                    strMsg = "";
                    break;
                case eApprManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eApprManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchError:
                    strMsg = xmlConf.GetErrMsg("E_0207");       // 결재 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchActionSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0034");       // 승인이 완료되었습니다.
                    break;
                case eApprManageMsg.eApprBatchRejectSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0017");       // 반려가 완료되었습니다.
                    break;
            }

            return strMsg;
        }
        public List<Dictionary<int, string>> GetSearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            return listDicdata;
        }

        /// <summary>
        /// 삭제예정
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<int, string>> GetQuerySearchData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            listDicdata = GetSvrRecordData("RECORD");
            if (listDicdata == null)
                return null;

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }

        public int GetTotalPageCount()
        {
            string page = GetTagData("total_page_count");
            if (int. TryParse(page, out int retValue) == false)
                return 1;
            else 
                return retValue;
        }

    }
}
