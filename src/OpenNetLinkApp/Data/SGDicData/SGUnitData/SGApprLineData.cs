using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Pages.Transfer;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class ApproverInfo
    {
        public string Index { get; set; }
        public string DeptName { get; set; }
        public string Grade { get; set; }
        public string Name { get; set; }
        public string UserSeq { get; set; }

        public int nApprPos { get; set; }
        public int nDlpApprove { get; set; }
        public ApproverInfo()
        {
            Index = DeptName = Grade = Name = UserSeq = "";
        }
        public ApproverInfo(string index, string deptname, string grade, string name, string userSeq, string apprPos,string dlpApprove)
        {
            Index = index;
            DeptName = deptname;
            Grade = grade;
            Name = name;
            UserSeq = userSeq;
            if (!apprPos.Equals(""))
                nApprPos = Convert.ToInt32(apprPos);
            if (!dlpApprove.Equals(""))
                nDlpApprove = Convert.ToInt32(dlpApprove);
        }

        public bool GetApprover()
        {
            if (nApprPos > 0)
                return true;
            return false;
        }
        public bool GetDlpApprover()
        {
            if (nDlpApprove > 0)
                return true;
            return false;
        }
    }
    public class SGApprLineData : SGData
    {
        public LinkedList<ApproverInfo> ApproverSelect = null;
        List<string> apprList = null;
        public SGApprLineData()
        {
            ApproverSelect = new LinkedList<ApproverInfo>();
        }
        ~SGApprLineData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetSessionKey(hs.GetSeedKey());
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        public void CopyApprLine(LinkedList<ApproverInfo> orgApprInfo)
        {
            ApproverSelect = new LinkedList<ApproverInfo>(orgApprInfo);
        }

        public List<Dictionary<int, string>> GetBaseApprLineData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            listDicdata = GetRecordData("APPROVERECORD");
            return listDicdata;
        }
        public List<string> GetBaseApprAndLineName()
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

        public List<string> GetBaseApprAndLineSeq()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 0; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
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

        public List<string> GetBaseApprAndLineDeptName()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 0; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(5, out tmpStr) == true)
                {
                    tmpStr = dic[5];
                    if (!tmpStr.Equals(""))
                        listApprLine.Add(tmpStr);
                }
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineRank()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 0; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(3, out tmpStr) == true)
                {
                    tmpStr = dic[3];
                    if (!tmpStr.Equals(""))
                        listApprLine.Add(tmpStr);
                }
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineOrder()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 0; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(6, out tmpStr) == true)
                {
                    tmpStr = dic[6];
                    if (!tmpStr.Equals(""))
                        listApprLine.Add(tmpStr);
                }
            }
            return listApprLine;
        }

        public LinkedList<ApproverInfo> GetConvertBaseApprAndLineData()
        {
            List<string> strListUserName = GetBaseApprAndLineName();                // 결재자 이름 List
            List<string> strListUserSeq = GetBaseApprAndLineSeq();                  // 결재자 Seq List
            List<string> strListDeptName = GetBaseApprAndLineDeptName();            // 결재자 부서이름 List
            List<string> strListUserRank = GetBaseApprAndLineRank();                // 결재자 이름 직위 List

            if ((strListUserName == null) && (strListUserName.Count <= 0))
                return null;

            ApproverInfo apprInfo = null;
            for (int i=0; i< strListUserName.Count;i++)
            {
                apprInfo = new ApproverInfo();

                apprInfo.Index = String.Format("{0,3}", i + 1);                     // Index

                if (!(strListUserName[i].Equals("")))                               // 결재자 이름 
                    apprInfo.Name = strListUserName[i];
                else
                    apprInfo.Name = "-";

                if (!(strListUserSeq[i].Equals("")))                               // 결재자 Seq
                    apprInfo.UserSeq = strListUserSeq[i];
                else
                    apprInfo.UserSeq = "-";

                if (!(strListDeptName[i].Equals("")))                               // 부서이름
                    apprInfo.DeptName = strListDeptName[i];
                else
                    apprInfo.DeptName = "-";

                if (!(strListUserRank[i].Equals("")))                               // 직위
                    apprInfo.Grade = strListUserRank[i];
                else
                    apprInfo.Grade = "-";

                apprInfo.nApprPos = 1;
                apprInfo.nDlpApprove = 0;

                ApproverSelect.AddLast(apprInfo);
            }

            return ApproverSelect;
        }

        public LinkedList<ApproverInfo> GetApprAndLineData()
        {
            return ApproverSelect;
        }

        public List<string> GetApprAndLineSeq(string strUserSeq)
        {
            LinkedList<ApproverInfo> apprLineData = GetApprAndLineData();
            if ((apprLineData == null) || (apprLineData.Count <= 0))
                return null;

            apprList = new List<string>();
            foreach (var item in apprLineData)
            {
                if (strUserSeq.Equals(item.UserSeq))
                    continue;

                apprList.Add(item.UserSeq);
            }

            return apprList;

        }

        public int GetApprAndLineSeqCount(string strUserSeq)
        {
            LinkedList<ApproverInfo> apprLineData = GetApprAndLineData();
            if ((apprLineData == null) || (apprLineData.Count <= 0))
                return 0;

            foreach (var item in apprLineData)
            {
                if (strUserSeq.Equals(item.UserSeq))
                    apprLineData.Remove(item);
            }
            return apprLineData.Count;
        }

        public void SetApprAndLindData(LinkedList<ApproverInfo> LinkedApprInfo)
        {
            ApproverSelect = null;
            CopyApprLine(LinkedApprInfo);
        }
        public static string DeleteApprLineData(string strUserSeq, string strSaveApprLine)
        {
            string[] strOriginApprLine = strSaveApprLine.Split('\u0002');
            if (strOriginApprLine.Length == 1)
            {
                return "";
            }

            char sep = (char)':';
            for (int i = 0; i < strOriginApprLine.Length; i++)
            {
                string[] strSplit = strOriginApprLine[i].Split(sep);
                if (strSplit[0].Equals(strUserSeq))
                {
                    strOriginApprLine[i] = "";
                    break;
                }
            }

            strSaveApprLine = "";
            for (int i = 0; i < strOriginApprLine.Length; i++)
            {
                strSaveApprLine += strOriginApprLine[i];
                if (i < (strOriginApprLine.Length - 1))
                    strSaveApprLine += '\u0002';
            }
            return strSaveApprLine;
        }
        public static string LocalSaveANDApprLineData(LinkedList<ApproverInfo> LinkedApprInfo, string strUserSeq, string strSaveApprLine)
        {
            if (LinkedApprInfo == null)
            {
                return "";
            }

            if(LinkedApprInfo.Count <= 0)
            {
                return DeleteApprLineData(strUserSeq, strSaveApprLine);
            }

            string strApprLineData = "";
            foreach (var item in LinkedApprInfo)
            {

                string strApprLineName = item.Name;
                string strApprLineRank = item.Grade;
                string strApprLineDeptName = item.DeptName;
                string strApprLineSeq = item.UserSeq;
                string strApprLineOrder = item.Index;
                int ApprPos = item.nApprPos;
                int DlpApprove = item.nDlpApprove;
                strApprLineData = strApprLineData + String.Format("{0}\u0001{1}\u0001{2}\u0001{3}\u0001{4}\u0001{5}\u0001{6}\u0003", strApprLineName, strApprLineRank, strApprLineDeptName, strApprLineSeq, strApprLineOrder, ApprPos, DlpApprove);
            }

            strApprLineData = strApprLineData.Substring(0, strApprLineData.Length - 1);
            strApprLineData = strUserSeq + ':' + strApprLineData;

            char sep = (char)':';

            if (strSaveApprLine.Equals(""))
                return strApprLineData;

            string[] strOriginApprLine = strSaveApprLine.Split('\u0002');
            if (strOriginApprLine.Length == 1)
            {
                string[] strSplit = strSaveApprLine.Split(sep);
                if (strSplit.Length == 1)
                    return strApprLineData;

                if (strSplit[0].Equals(strUserSeq))
                    return strApprLineData;

                strSaveApprLine += '\u0002';
                strSaveApprLine += strApprLineData;
                return strSaveApprLine;
            }

            int index = -1;
            for(int i=0; i < strOriginApprLine.Length;i++)
            {
                string[] strSplit = strOriginApprLine[i].Split(sep);
                if(strSplit[0].Equals(strUserSeq))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                strSaveApprLine = "";
                strOriginApprLine[index] = strApprLineData;
                for (int i = 0; i < strOriginApprLine.Length; i++)
                {
                    strSaveApprLine += strOriginApprLine[i];
                    if (i < (strOriginApprLine.Length - 1))
                        strSaveApprLine += '\u0002';
                }
            }
            else
            {
                strSaveApprLine += '\u0002';
                strSaveApprLine += strApprLineData;
            }
            return strSaveApprLine;
        }

        public bool LocalLoadANDApprLineData(string strApprLineData, string strUserSeq)
        {
            LinkedList<ApproverInfo> apprInfo = new LinkedList<ApproverInfo>();
            if (strApprLineData.Equals(""))
            {
                return false;
            }

            if(!strApprLineData.Contains(strUserSeq))
            {
                return false;
            }

            char sep = (char)':';
            string[] strApprList;
            string[] strApprLineDataList = strApprLineData.Split('\u0002');
            if(strApprLineDataList.Length==1)
            {
                string[] strSplit = strApprLineData.Split(sep);
                if(strSplit.Length==1)
                {
                    return false;
                }

                if(!strSplit[0].Equals(strUserSeq))
                {
                    return false;
                }

                strApprList = strSplit[1].Split('\u0003');
                if (strApprList.Length <= 0)
                {
                    return false;
                }

                for (int i = 0; i < strApprList.Length; i++)
                {
                    string[] strApprData = strApprList[i].Split('\u0001');
                    if (strApprData.Length <= 0)
                        continue;
                    ApproverInfo apprdata = new ApproverInfo();
                    apprdata.Name = strApprData[0];
                    apprdata.Grade = strApprData[1];
                    apprdata.DeptName = strApprData[2];
                    apprdata.UserSeq = strApprData[3];
                    apprdata.Index = strApprData[4];
                    if (!strApprData[5].Equals(""))
                        apprdata.nApprPos = Convert.ToInt32(strApprData[5]);
                    if (!strApprData[6].Equals(""))
                        apprdata.nDlpApprove = Convert.ToInt32(strApprData[6]);

                    apprInfo.AddLast(apprdata);
                }

                CopyApprLine(apprInfo);
                return true;
            }

            bool bFind = false;
            string strApprLine = "";
            for(int i=0;i< strApprLineDataList.Length;i++)
            {
                string[] strSplit = strApprLineDataList[i].Split(sep);
                if (strSplit.Length <= 1)
                    continue;

                if(strSplit[0].Equals(strUserSeq))
                {
                    strApprLine = strSplit[1];
                    bFind = true;
                    break;
                }
            }

            if(!bFind)
            {
                apprInfo = null;
                return false;
            }

            strApprList = strApprLine.Split('\u0003');
            if (strApprList.Length <= 0)
            {
                return false;
            }
            
            for(int i=0;i<strApprList.Length;i++)
            {
                string [] strApprData = strApprList[i].Split('\u0001');
                if (strApprData.Length <= 0)
                    continue;
                ApproverInfo apprdata = new ApproverInfo();
                apprdata.Name = strApprData[0];
                apprdata.Grade = strApprData[1];
                apprdata.DeptName = strApprData[2];
                apprdata.UserSeq = strApprData[3];
                apprdata.Index = strApprData[4];
                if (!strApprData[5].Equals(""))
                    apprdata.nApprPos = Convert.ToInt32(strApprData[5]);
                if(!strApprData[6].Equals(""))
                    apprdata.nDlpApprove = Convert.ToInt32(strApprData[6]);

                apprInfo.AddLast(apprdata);
            }

            CopyApprLine(apprInfo);
            return true;
        }

    }
}
