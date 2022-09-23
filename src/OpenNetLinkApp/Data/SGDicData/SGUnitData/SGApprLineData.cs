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
        public string selectIndex { get; set; }
        public string Index { get; set; }
        public string DeptName { get; set; }
        public string DeptSeq { get; set; }
        public string Grade { get; set; }
        public string Name { get; set; }
        public string UserSeq { get; set; }
        public int nApprPos { get; set; }
        public int nDlpApprove { get; set; }
        public int nApvOrder { get; set; }
        public string STARTDATE { get; set; }
        public string ENDDATE { get; set; }
        public string APPR_TEAMCODE { get; set; }
        public string APPR_TEAMNAME { get; set; }
        public string APPR_USERID { get; set; }
        public string APPR_USERNAME { get; set; }
        public string POSITION { get; set; }
        public string RANK { get; set; }

        public int ORDER { get; set; } //대결재 순서....
        public ApproverInfo(int index, string name, string rank, string deptname, string deptseq, string seq, string apvorder)
        {
            Index = index.ToString();

            if (String.IsNullOrEmpty(name))
                Name = "-";
            else
                Name = name;

            if (String.IsNullOrEmpty(rank))
                Grade = "-";
            else
                Grade = rank;

            if (String.IsNullOrEmpty(deptname))
                DeptName = "-";
            else
                DeptName = deptname;

            if (String.IsNullOrEmpty(deptseq))
                DeptSeq = "-";
            else
                DeptSeq = deptseq;

            if (String.IsNullOrEmpty(seq))
                UserSeq = "-";
            else
                UserSeq = seq;

            nApvOrder = Int32.Parse(apvorder);
        }
        public ApproverInfo()
        {
            Index = DeptName = DeptSeq = Grade = Name = UserSeq = "";
        }
        public ApproverInfo(string index, string deptname, string deptseq, string grade, string name, string userSeq, string apprPos,string dlpApprove)
        {
            Index = index;
            DeptName = deptname;
            DeptSeq = deptseq;
            Grade = grade;
            Name = name;
            UserSeq = userSeq;
            if (!apprPos.Equals(""))
                nApprPos = Convert.ToInt32(apprPos);
            if (!dlpApprove.Equals(""))
                nDlpApprove = Convert.ToInt32(dlpApprove);
        }
        public ApproverInfo(int index, ApproverInfo info)
        {
            this.Index = index.ToString();
            this.Name = info.Name;
            this.Grade = info.Grade;
            this.DeptName = info.DeptName;
            this.DeptSeq = info.DeptSeq;
            this.UserSeq = info.UserSeq;
            this.nApvOrder = info.nApvOrder;
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

            ApproverSelect = GetConvertBaseApprAndLineData();
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
        public List<string> GetBaseApprAndLineDeptSeq()
        {
            List<string> listApprLine = new List<string>();
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            int nTotalCount = listDicdata.Count;
            for (int i = 0; i < nTotalCount; i++)                       // 파일 전송 시 사용하기 위해 자기 자신을 제외하기 위해서 i = 1 부터 시작.
            {
                Dictionary<int, string> dic = listDicdata[i];
                string tmpStr = "";
                if (dic.TryGetValue(4, out tmpStr) == true)
                {
                    tmpStr = dic[4];
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
            List<string> listApprLineName = GetBaseApprAndLineName();                // 결재자 이름 List
            List<string> listApprLineSeq = GetBaseApprAndLineSeq();                  // 결재자 Seq List
            List<string> listApprLineDeptName = GetBaseApprAndLineDeptName();            // 결재자 부서이름 List
            List<string> listApprLineDeptSeq = GetBaseApprAndLineDeptSeq();              // 결재자 부서Seq List
            List<string> listApprLineRank = GetBaseApprAndLineRank();                // 결재자 이름 직위 List
            List<string> listApprLineOrder = GetBaseApprAndLineOrder();

            LinkedList<ApproverInfo> ApproverSelect = new LinkedList<ApproverInfo>();

            if ((listApprLineName == null) && (listApprLineName.Count <= 0))
                return null;
            if(listApprLineName.Count != listApprLineDeptSeq.Count)
            {
                int NameCount = listApprLineName.Count;
                int DeptSeqCount = listApprLineDeptSeq.Count;
                for (int i = 0; i < NameCount - DeptSeqCount; i++)
                {
                    listApprLineDeptSeq.Add("-");
                }
            }


            Dictionary<int, List<string>> checkUserSeqByOrder = new Dictionary<int, List<string>>();

            for (int i=0; i< listApprLineName.Count;i++)
            {
                int order = Convert.ToInt32(listApprLineOrder[i]);
                if (checkUserSeqByOrder.ContainsKey(order))
                {
                    if (checkUserSeqByOrder[order].Contains(listApprLineSeq[i]))
                        continue;
                    else
                    {
                        checkUserSeqByOrder[order].Add(listApprLineSeq[i]);
                    }
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(listApprLineSeq[i]);
                    checkUserSeqByOrder.Add(order, list);
                }
                ApproverInfo apprInfo = new ApproverInfo(i, listApprLineName[i], listApprLineRank[i], listApprLineDeptName[i], listApprLineDeptSeq[i], listApprLineSeq[i], listApprLineOrder[i]);
                apprInfo.nApprPos = 1;
                apprInfo.nDlpApprove = 0;
                ApproverSelect.AddLast(apprInfo);
            }

            return ApproverSelect;
        }

        /// <summary>
        /// ApproverSelect
        /// </summary>
        /// <returns></returns>
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

        public string GetApprCommaSeqString(string strUserSeq)
        {
            LinkedList<ApproverInfo> apprLineData = GetApprAndLineData();
            if ((apprLineData == null) || (apprLineData.Count <= 0))
                return null;

            string apprLine = String.Empty;

            foreach(ApproverInfo info in apprLineData)
            {
                if (info.UserSeq.Equals(strUserSeq))
                    continue;
                else
                {
                    apprLine += $"{info.UserSeq},";
                }
            }

            if (!String.IsNullOrEmpty(apprLine))
                apprLine = apprLine.Substring(0, apprLine.Length - 1);

            return apprLine;

        }


        /// <summary>
        /// 결재 라인 String으로 반환 ( \u0002, | ) 사용
        /// </summary>
        /// <param name="strUserSeq"></param>
        /// <param name="apprStep"></param>
        /// <returns></returns>
        public string GetApprAndLineSeqString(string strUserSeq, string apprStep)
        {
            string rtn = string.Empty;
            LinkedList<ApproverInfo> apprLineData = GetApprAndLineData();
            if ((apprLineData == null) || (apprLineData.Count <= 0))
                return null;


            ///이걸하면 변경된 사항이 적용이 안되는데... 왜 하는건지 모르겠음..
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            foreach (ApproverInfo item in apprLineData)
            {
                for( int i=0; i<listDicdata.Count; i++)
                {
                    Dictionary<int, string> dic = listDicdata[i];
                    if( item.UserSeq == dic[0])
                    {
                        item.nApvOrder = Int32.Parse(dic[6]);
                        break;
                    }
                }
            }

            char Sep = (char)'\u0002';
            char orSep = (char)'|';
            if(apprLineData != null && apprLineData.Count > 0)
            {
                LinkedListNode<ApproverInfo> last = apprLineData.Last;
                LinkedListNode<ApproverInfo> curNode = apprLineData.First;

                if (apprStep == "0")
                {
                    foreach (ApproverInfo item in apprLineData)
                    {
                        if (item.UserSeq.Equals(strUserSeq))
                            continue;
                        rtn += item.UserSeq;
                        rtn += Sep;
                    }
                }
                if (apprStep == "1")
                {
                    foreach (ApproverInfo item in apprLineData)
                    {
                        if (item.UserSeq.Equals(strUserSeq))
                            continue;
                        rtn += item.UserSeq;
                        if (last.Value.UserSeq.Equals(item.UserSeq))
                            rtn += Sep;
                        else
                            rtn += orSep;
                    }
                }
                if(apprStep == "2")
                { 
                    while (true)
                    {
                        if (curNode == null)
                            break;

                        if( curNode.Value.UserSeq.Equals(strUserSeq) )
                        {
                            curNode = curNode.Next;
                            continue;
                        }
                        LinkedListNode<ApproverInfo> next = curNode.Next;
                        rtn += curNode.Value.UserSeq;
                        if(next != null)
                        {
                            if (curNode.Value.nApvOrder == next.Value.nApvOrder)
                                rtn += orSep;
                            else
                                rtn += Sep;
                        }
                        else
                            rtn += Sep;

                        curNode = curNode.Next;
                    }
                }
            }
            return rtn;
        }

        public int GetApprAndLineSeqCount(string strUserSeq)
        {
            LinkedList<ApproverInfo> apprLineData = GetApprAndLineData();
            if ((apprLineData == null) || (apprLineData.Count <= 0))
                return 0;
            int nDisCount = 0;
            foreach (var item in apprLineData)
            {
                if (strUserSeq.Equals(item.UserSeq))
                    nDisCount++;
            }
            return apprLineData.Count-nDisCount;
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
        public static string LocalSaveANDApprLineData(LinkedList<ApproverInfo> LinkedApprInfo, string strUserSeq, string strSaveApprLine, int apvStep)
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
                string strApprLineDeptSeq = item.DeptSeq;
                string strApprLineSeq = item.UserSeq;
                string strApprLineOrder = item.Index;
                if (apvStep == 2)
                    strApprLineOrder = item.nApvOrder.ToString();
                int ApprPos = item.nApprPos;
                int DlpApprove = item.nDlpApprove;
                strApprLineData = strApprLineData + String.Format("{0}\u0001{1}\u0001{2}\u0001{3}\u0001{4}\u0001{5}\u0001{6}\u0001{7}\u0003", strApprLineName, strApprLineRank, strApprLineDeptName, strApprLineDeptSeq, strApprLineSeq, strApprLineOrder, ApprPos, DlpApprove);
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

        /// <summary>
        /// 해당 데이터를 ApproverSelect 으로 저장
        /// </summary>
        /// <param name="strApprLineData"></param>
        /// <param name="strUserSeq"></param>
        /// <returns></returns>
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
                    apprdata.DeptSeq = strApprData[3];
                    apprdata.UserSeq = strApprData[4];
                    apprdata.Index = strApprData[5];
                    if (!strApprData[6].Equals(""))
                        apprdata.nApprPos = Convert.ToInt32(strApprData[6]);
                    if (!strApprData[7].Equals(""))
                        apprdata.nDlpApprove = Convert.ToInt32(strApprData[7]);

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
                apprdata.DeptSeq = strApprData[3];
                apprdata.UserSeq = strApprData[4];
                apprdata.Index = strApprData[5];
                if (!strApprData[6].Equals(""))
                    apprdata.nApprPos = Convert.ToInt32(strApprData[6]);
                if(!strApprData[7].Equals(""))
                    apprdata.nDlpApprove = Convert.ToInt32(strApprData[7]);

                //DeptSeq가 비어있으면 유효성 검증 넘어가기 위해 "-"로 저장
                if (apprdata.DeptSeq.Equals(""))
                    apprdata.DeptSeq = "-";

                apprInfo.AddLast(apprdata);
            }

            CopyApprLine(apprInfo);
            return true;
        }

    }
}
