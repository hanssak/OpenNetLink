using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

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
        public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
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

        public LinkedList<ApproverInfo> GetDeptApproverInfoData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            listDicdata = GetSvrRecordData("VALUE",false);
            if ((listDicdata == null) || (listDicdata.Count<=0))
                return null;

            ApproverSearch = new LinkedList<ApproverInfo>();

            string strDeptName = "";
            string strGrade = "";
            string strUserName = "";
            string strUserSeq = "";
            string strApprPos = "";
            string strDlpAppr = "";
            Dictionary<int, string> dic = null;
            ApproverInfo apprInfo = null;
            for (int i = 0; i < listDicdata.Count; i++)
            {
                dic = listDicdata[i];
                if (dic == null)
                    continue;

                apprInfo = new ApproverInfo();

                apprInfo.Index = String.Format("{0,3}", i + 1);                     // Index

                if (!dic.TryGetValue(1, out strUserName))                            // 이름
                    apprInfo.Name = "-";
                else
                    apprInfo.Name = dic[1];

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
        }

    }
}
