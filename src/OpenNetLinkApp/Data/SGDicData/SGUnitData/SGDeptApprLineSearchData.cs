using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using Serilog;
using AgLogManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eDeptApprLineSearchManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2
    }
    public class SGDeptApprLineSearchData : SGData
    {
        LinkedList<ApproverInfo> ApproverSearch = new LinkedList<ApproverInfo>();
        public SGDeptApprLineSearchData()
        {
        }
        ~SGDeptApprLineSearchData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public static string ReturnMessage(eDeptApprLineSearchManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eDeptApprLineSearchManageMsg.eNone:
                    strMsg = "";
                    break;
                case eDeptApprLineSearchManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eDeptApprLineSearchManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0211");       // 결재자 정보 요청 중 오류가 발생되었습니다.
                    break;
            }

            return strMsg;
        }

        /*public LinkedList<ApproverInfo> GetDeptApproverInfoData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            //listDicdata = GetSvrRecordData("VALUE");
            listDicdata = GetSvrRecordData("RECORD");
            if ((listDicdata == null) || (listDicdata.Count<=0))
                return null;

            ApproverSearch = new LinkedList<ApproverInfo>();

            string strDeptName = "";
            string strGrade = "";
            string strUserName = "";
            string strUserSeq = "";
            string strApprPos = "";
            string strDlpAppr = "";
            string stUserId = "";
            string stDeptCode = "";
            Dictionary<int, string> dic = null;
            ApproverInfo apprInfo = null;
            for (int i = 0; i < listDicdata.Count; i++)
            {
                dic = listDicdata[i];
                if (dic == null)
                    continue;

                apprInfo = new ApproverInfo();

                apprInfo.Index = String.Format("{0,3}", i + 1);                     // Index

                if (!dic.TryGetValue(0, out stUserId))                            //유저ID
                    apprInfo.APPR_USERID = "-";
                else
                    apprInfo.APPR_USERID = dic[0];

                if (!dic.TryGetValue(1, out strUserName))                            // 이름
                    apprInfo.Name = "-";
                else
                    apprInfo.Name = dic[1];

                if (!dic.TryGetValue(2, out stDeptCode))                            // 부서Seq
                    apprInfo.APPR_TEAMCODE = apprInfo.DeptSeq = "-";
                else
                    apprInfo.APPR_TEAMCODE = apprInfo.DeptSeq = dic[2];

                if (!dic.TryGetValue(3, out strDeptName))                            // 부서이름 
                    apprInfo.DeptName = "-";
                else
                    apprInfo.DeptName = dic[3];

                if (!dic.TryGetValue(5, out strGrade))                              // 직위
                    apprInfo.Grade = "-";
                else
                    apprInfo.Grade = dic[5];

                if (!dic.TryGetValue(7, out strApprPos))                              // ApprovePos
                    apprInfo.nApprPos = 0;
                else
                {
                    string temp = dic[7];
                    if (!temp.Equals(""))
                        apprInfo.nApprPos = Convert.ToInt32(temp);
                }

                if (!dic.TryGetValue(8, out strUserSeq))                              // User Sequence
                    apprInfo.UserSeq = "-";
                else
                    apprInfo.UserSeq = dic[8];

                if (!dic.TryGetValue(9, out strDlpAppr))                              // 보안결재자 여부
                    apprInfo.nDlpApprove = 0;
                else
                {
                    string temp = dic[9];
                    if (!temp.Equals(""))
                        apprInfo.nDlpApprove = Convert.ToInt32(temp);
                }

                ApproverSearch.AddLast(apprInfo);
            }

            return ApproverSearch;
        }*/


        public LinkedList<ApproverInfo> GetDeptApproverInfoDataAdvanced()
        {

            if (ApproverSearch == null)
                ApproverSearch = new LinkedList<ApproverInfo>();

            try
            {

                ApproverSearch.Clear();
                //List<string> liststrData = GetTagDataList("approver_list");

                //string strData = GetTagData("approver_list", "approver_seq");
                /*List<Dictionary<int, string>> listDicdata = null;
                //listDicdata = GetSvrRecordData("VALUE");
                listDicdata = GetSvrRecordData("RECORD");
                if ((listDicdata == null) || (listDicdata.Count <= 0))
                    return null;

                GetTagData("approver_list");*/

                //string strData = GetTagData("approver_list");
                //var dataList = JsonConvert.DeserializeObject<List<dynamic>>(strData);

                List<object> dataList =GetTagDataObjectList("approver_list");
                Log.Logger.Here().Information($"GetDeptApproverInfoDataAdvanced, Search User - Count : {dataList?.Count}");

                if (dataList?.Count < 1)
                    return ApproverSearch;

                ApproverInfo apprInfo = null;
                string strItemData = "";
                string strApproverHr = "";
                string strApproverType = "";
                int i = 0;

                foreach (object dataItem in dataList)
                {
                    JObject jO = (JObject)dataItem;

                    apprInfo = new ApproverInfo();
                    apprInfo.Index = String.Format("{0,3}", i + 1);                     // Index
                    apprInfo.APPR_USERID = dataItem.GetTagDataObject(new List<string>() { "approver_hr", "approver_id" }).ToString();
                    apprInfo.Name = dataItem.GetTagDataObject(new List<string>() { "approver_hr", "name" }).ToString();                       // 이름
                    apprInfo.APPR_TEAMCODE = dataItem.GetTagDataObject(new List<string>() { "approver_hr", "dept_seq" }).ToString();    // 부서Code
                    apprInfo.DeptName = dataItem.GetTagDataObject(new List<string>() { "approver_hr", "dept_name" }).ToString();    // 부서이름
                    apprInfo.Grade = dataItem.GetTagDataObject(new List<string>() { "approver_hr", "rank" }).ToString();   // 직위
                    apprInfo.nApprPos = int.Parse(dataItem.GetTagDataObject(new List<string>() { "approver_type", "authority" }).ToString());// ApprovePos
                    apprInfo.UserSeq = dataItem.GetTagDataObject(new List<string>() { "approver_seq"}).ToString();                   // userSeq
                    apprInfo.nDlpApprove = int.Parse(dataItem.GetTagDataObject(new List<string>() { "approver_type", "dlp_authority" }).ToString());                              // 보안결재자 여부

                    ApproverSearch.AddLast(apprInfo);
                    i++;

                    Log.Logger.Here().Information($"GetDeptApproverInfoDataAdvanced, Search User - Index : {i}");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Information($"GetDeptApproverInfoDataAdvanced, Exception(MSG) : {ex.Message}");
            }
            return ApproverSearch;
        }



    }


}
