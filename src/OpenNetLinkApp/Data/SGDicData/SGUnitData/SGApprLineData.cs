using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Pages.Transfer;
using Serilog;
using AgLogManager;

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

        /// <summary>
        /// false:일반사용자, true:추가 결재권자.
        /// </summary>
        public bool bOtherApproveUse { get; set; }

        /// <summary>
        /// false:일반사용자, true:vip사용자
        /// </summary>
        public bool bVip { get; set; }



        public int ORDER { get; set; } //대결재 순서....
        public ApproverInfo(int index, string name, string rank, string deptname, string deptseq, string seq, string apvorder, string id)
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

            if (String.IsNullOrEmpty(id))
                APPR_USERID = "-";
            else
                APPR_USERID = id;

            nApvOrder = Int32.Parse(apvorder);
        }
        public ApproverInfo()
        {
            Index = DeptName = DeptSeq = Grade = Name = UserSeq = APPR_USERID = "";
        }
        public ApproverInfo(string index, string deptname, string deptseq, string grade, string name, string userSeq, string apprPos, string dlpApprove, string id)
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
            APPR_USERID = id;
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
            this.APPR_USERID = info.APPR_USERID;
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

        public ApproverInfo Copy()
        {
            ApproverInfo CopyValue = new ApproverInfo()
            {
                Index = this.Index,
                selectIndex = this.selectIndex,
                DeptName = this.DeptName,
                DeptSeq = this.DeptSeq,
                Grade = this.Grade,
                Name = this.Name,
                UserSeq = this.UserSeq,
                nApprPos = this.nApprPos,
                nDlpApprove = this.nDlpApprove,
                nApvOrder = this.nApvOrder,
                STARTDATE = this.STARTDATE,
                ENDDATE = this.ENDDATE,
                APPR_TEAMCODE = this.APPR_TEAMCODE,
                APPR_TEAMNAME = this.APPR_TEAMNAME,
                APPR_USERID = this.APPR_USERID,
                APPR_USERNAME = this.APPR_USERNAME,
                POSITION = this.POSITION,
                RANK = this.RANK,
                bOtherApproveUse = this.bOtherApproveUse,
                bVip = this.bVip
            };

            return CopyValue;
        }


        public ProxyApprover ToProxyApprover(string strUserSeq)
        {
            ProxyApprover CopyValue = new ProxyApprover();

            try
            {
                CopyValue.ApproverHr.strId = this.APPR_USERID;
                CopyValue.ApproverHr.strSeq = this.UserSeq;
                CopyValue.ApproverHr.strDeptName = this.DeptName;
                CopyValue.ApproverHr.deptSeq = this.DeptSeq;
                CopyValue.ApproverHr.strName = this.Name;
                CopyValue.ApproverHr.strRank = this.RANK;
                CopyValue.ApproverHr.strPosition = this.POSITION;
                
                CopyValue.User_Seq = Convert.ToInt64(strUserSeq);
                CopyValue.Approver_Seq = Convert.ToInt64(this.UserSeq);

                CopyValue.ApproverType.nAuthority = this.nApprPos;
                CopyValue.ApproverType.nDlpAuthority = this.nDlpApprove;
                CopyValue.ApproverType.bOtherUse = this.bOtherApproveUse;
                CopyValue.ApproverType.bVip = this.bVip;

                //CopyValue.strApproverType = this.GetDlpApprover() ? "security" : "common";
                CopyValue.nApproverOrder = this.nApvOrder;
                CopyValue.STARTDATE = this.STARTDATE;
                CopyValue.ENDDATE = this.ENDDATE;
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"ToProxyApprover, Exception(MSG) : {ex.Message}");
            }

            return CopyValue;
        }

    }


    public class UserApproveRightType
    {
        /// <summary>
        /// 결재 권한 : 0:일반사용자, 1:결재자, 2:전결자
        /// </summary>
        public int nAuthority { get; set; }

        /// <summary>
        /// 0:일반사용자, 1:정보 보안 결재권자
        /// </summary>
        public int nDlpAuthority { get; set; }

        /// <summary>
        /// false:일반사용자, true:추가 결재권자.
        /// </summary>
        public bool bOtherUse { get; set; }

        /// <summary>
        /// false:일반사용자, true:vip사용자
        /// </summary>
        public bool bVip { get; set; }

        public UserApproveRightType()
        {
            nAuthority = 0;
            nDlpAuthority = 0;
            bOtherUse = false;
            bVip = false;
        }



    }


    /// <summary>
    /// 
    /// </summary>
    public class UserHRinfo
    {
        public string strId { get; set; }
        public string strSeq { get; set; }
        public string strName { get; set; }
        public string strPosition { get; set; }

        public string strRank { get; set; }
        public string deptSeq { get; set; }
        public string strDeptName { get; set; }

        public UserHRinfo()
        {
            strId = "";
            strSeq = "";
            strName = "";
            strPosition = "";
            strRank = "";
            deptSeq = "";
            strDeptName = "";
        }

        public UserHRinfo Copy()
        {
            return new UserHRinfo()
            {
                strId = this.strId,
                strSeq = this.strSeq,
                strName = this.strName,
                strPosition = this.strPosition,
                strRank = this.strRank,
                deptSeq = this.deptSeq,
                strDeptName = this.strDeptName
            };
        }
    }



    public class ApproverUiData
    {
        public Int64 Approver_Seq { get; set; }
        public UserHRinfo ApproverHr { get; set; }
        public UserApproveRightType ApproverType { get; set; }

    }

    public class ProxyApprover
    {

        public Int64 User_Seq { get; set; }
        public Int64 Approver_Seq { get; set; }
        public UserHRinfo ApproverHr { get; set; }
        public UserApproveRightType ApproverType { get; set; }

        /// <summary>
        /// 결재순서
        /// </summary>
        public int nApproverOrder { get; set; }

        public string STARTDATE { get; set; }
        public string ENDDATE { get; set; }


        public ProxyApprover()
        {
            User_Seq = 0;
            Approver_Seq = 0;
            ApproverHr = new UserHRinfo();
            ApproverType = new UserApproveRightType();
            nApproverOrder = 0;
            STARTDATE = ENDDATE = "";
        }

        public ProxyApprover Copy()
        {
            ProxyApprover CopyValue = new ProxyApprover()
            {

                STARTDATE = this.STARTDATE,
                ENDDATE = this.ENDDATE,

            };

            return CopyValue;
        }
    }

    /// <summary>
    /// default_approver_reg_list
    /// </summary>
    public class SGApprLineData : SGData
    {

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGApprLineData>();

        public UserHRinfo UserHr = null;
        public LinkedList<ApproverInfo> ApproverSelect = null;
        List<string> apprList = null;


        public SGApprLineData()
        {
            ApproverSelect = new LinkedList<ApproverInfo>();
            UserHr = new UserHRinfo();
        }
        ~SGApprLineData()
        {

        }
        public void Copy(HsNetWork hs, SGData data, UserHRinfo userHr)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);

            UserHr = userHr;
            ApproverSelect = GetConvertBaseApprAndLineData();

        }

        public void CopyApprLine(LinkedList<ApproverInfo> orgApprInfo)
        {
            ApproverSelect = new LinkedList<ApproverInfo>(orgApprInfo);
        }

        public List<string> GetBaseApprAndLineName()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.strName);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");
            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineName, listdefaultAppr == null !");
                return listApprLine;
            }

            foreach (object person in listdefaultAppr)
            {
                listApprLine.Add(person.GetTagDataObject("approver_hr", "name").ToString());
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineID()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.strId);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");
            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineID, listdefaultAppr == null !");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                listApprLine.Add(person.GetTagDataObject("approver_hr", "approver_id").ToString());
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineSeq()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.strSeq);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");

            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineSeq, listDicdata == null !");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                listApprLine.Add(person.GetTagDataObject("approver_hr", "approver_seq").ToString());
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineDeptName()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.strDeptName);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");

            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineSeq, listDicdata == null !");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                string deptName = person.GetTagDataObject("approver_hr", "dept_name").ToString();
                listApprLine.Add(string.IsNullOrEmpty(deptName) ? "-" : deptName);
            }
            return listApprLine;
        }
        public List<string> GetBaseApprAndLineDeptSeq()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.deptSeq);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");
            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineSeq, listDicdata == null !");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                string deptSeq = person.GetTagDataObject("approver_hr", "dept_seq").ToString();
                listApprLine.Add(string.IsNullOrEmpty(deptSeq) ? "-" : deptSeq);
            }
            return listApprLine;
        }
        public List<string> GetBaseApprAndLineRank()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add(UserHr.strRank);//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");
            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineSeq, listDicdata == null !");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                string rank = person.GetTagDataObject("approver_hr", "rank").ToString();
                listApprLine.Add(string.IsNullOrEmpty(rank) ? "-" : rank);
            }
            return listApprLine;
        }

        public List<string> GetBaseApprAndLineOrder()
        {
            List<string> listApprLine = new List<string>();
            listApprLine.Add("-99");//자기 자신 포함

            List<object> listdefaultAppr = GetTagDataObjectList("default_approver_reg_list");

            if (listdefaultAppr == null)
            {
                Log.Logger.Here().Error($"GetBaseApprAndLineOrder, listDicdata == null ");
                return listApprLine;
            }
            foreach (object person in listdefaultAppr)
            {
                listApprLine.Add(person.GetTagDataObject("approver_order").ToString());
            }
            return listApprLine;
        }

        public LinkedList<ApproverInfo> GetConvertBaseApprAndLineData()
        {
            LinkedList<ApproverInfo> ApproverSelect = new LinkedList<ApproverInfo>();

            try
            {
                List<string> listApprLineName = GetBaseApprAndLineName();                // 결재자 이름 List
                List<string> listApprLineSeq = GetBaseApprAndLineSeq();                  // 결재자 Seq List
                List<string> listApprLineDeptName = GetBaseApprAndLineDeptName();            // 결재자 부서이름 List
                List<string> listApprLineDeptSeq = GetBaseApprAndLineDeptSeq();              // 결재자 부서Seq List
                List<string> listApprLineRank = GetBaseApprAndLineRank();                // 결재자 이름 직위 List
                List<string> listApprLineOrder = GetBaseApprAndLineOrder();
                List<string> listApprLineID = GetBaseApprAndLineID();                   // 결재자 ID List

                if (listApprLineName == null || listApprLineName.Count <= 0)
                {
                    Log.Logger.Here().Error($"GetConvertBaseApprAndLineData, listApprLineName == null !!!");
                    return null;
                }

                if (listApprLineName.Count != listApprLineDeptSeq.Count)
                {
                    int NameCount = listApprLineName.Count;
                    int DeptSeqCount = listApprLineDeptSeq.Count;
                    for (int i = 0; i < NameCount - DeptSeqCount; i++)
                    {
                        listApprLineDeptSeq.Add("-");
                    }
                }

                Dictionary<int, List<string>> checkUserSeqByOrder = new Dictionary<int, List<string>>();

                for (int i = 0; i < listApprLineName.Count; i++)
                {
                    int order = Convert.ToInt32(listApprLineOrder[i]);

                    //Log.Logger.Here().Information($"GetConvertBaseApprAndLineData, index: {i}, order : {order}");
                    if (order < 0)
                    {
                        Log.Logger.Here().Error($"GetConvertBaseApprAndLineData, ApproveLine -  index: {i}, order : {order}, MSG: order Value is less than 0.");
                        order = i;
                    }

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

                    ApproverInfo apprInfo = new ApproverInfo(i, listApprLineName[i], listApprLineRank[i], listApprLineDeptName[i], listApprLineDeptSeq[i], listApprLineSeq[i], listApprLineOrder[i], listApprLineID[i]);
                    apprInfo.nApprPos = 1;
                    apprInfo.nDlpApprove = 0;
                    ApproverSelect.AddLast(apprInfo);
                }

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetConvertBaseApprAndLineData-Exception(MSG) : {ex.Message}");
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

            foreach (ApproverInfo info in apprLineData)
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

            char Sep = (char)'\u0002'; //(STX)
            char orSep = (char)'|';
            if (apprLineData != null && apprLineData.Count > 0)
            {
                LinkedListNode<ApproverInfo> last = apprLineData.Last;
                LinkedListNode<ApproverInfo> curNode = apprLineData.First;


                if (apprStep == "0")    //AND
                {
                    foreach (ApproverInfo item in apprLineData)
                    {
                        if (item.UserSeq.Equals(strUserSeq))
                            continue;

                        //AND 조건 : USERID[STX]USERID[STX]USERID[STX]USERID[STX]USERID[STX]USERID[STX]
                        rtn += item.UserSeq;
                        rtn += Sep;
                    }
                }
                if (apprStep == "1")    //OR
                {
                    foreach (ApproverInfo item in apprLineData)
                    {
                        if (item.UserSeq.Equals(strUserSeq))
                            continue;

                        //OR 조건 : USERID|USERID......|USERID|USERID[STX]
                        rtn += item.UserSeq;
                        if (last.Value.UserSeq.Equals(item.UserSeq))
                            rtn += Sep;
                        else
                            rtn += orSep;
                    }
                }
                if (apprStep == "2")        //ANDOR
                {
                    while (true)
                    {
                        if (curNode == null)
                            break;

                        if (curNode.Value.UserSeq.Equals(strUserSeq))
                        {
                            curNode = curNode.Next;
                            continue;
                        }
                        LinkedListNode<ApproverInfo> next = curNode.Next;
                        rtn += curNode.Value.UserSeq;
                        if (next != null)
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
            return apprLineData.Count - nDisCount;
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

            if (LinkedApprInfo.Count <= 0)
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
                string strApprLineID = item.APPR_USERID;
                strApprLineData = strApprLineData + String.Format("{0}\u0001{1}\u0001{2}\u0001{3}\u0001{4}\u0001{5}\u0001{6}\u0001{7}\u0001{8}\u0003", strApprLineName, strApprLineRank, strApprLineDeptName, strApprLineDeptSeq, strApprLineSeq, strApprLineOrder, ApprPos, DlpApprove, strApprLineID);
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
            for (int i = 0; i < strOriginApprLine.Length; i++)
            {
                string[] strSplit = strOriginApprLine[i].Split(sep);
                if (strSplit[0].Equals(strUserSeq))
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
        /// <param name="apvStep">APPROVESTEP (0: AND, 1: OR, 2: ANDOR</param>
        /// <returns></returns>
        public bool LocalLoadANDApprLineData(string strApprLineData, string strUserSeq, int apvStep)
        {
            LinkedList<ApproverInfo> apprInfo = new LinkedList<ApproverInfo>();
            if (strApprLineData.Equals(""))
            {
                return false;
            }

            if (!strApprLineData.Contains(strUserSeq))
            {
                return false;
            }

            char sep = (char)':';
            string[] strApprList;
            string[] strApprLineDataList = strApprLineData.Split('\u0002');

            bool bFind = false;
            string strApprLine = "";
            for (int i = 0; i < strApprLineDataList.Length; i++)
            {
                string[] strSplit = strApprLineDataList[i].Split(sep);
                if (strSplit.Length <= 1)
                    continue;

                if (strSplit[0].Equals(strUserSeq))
                {
                    strApprLine = strSplit[1];
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                apprInfo = null;
                return false;
            }

            strApprList = strApprLine.Split('\u0003');
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

                //ANDOR 결재는 Local 저장 시 5번째에 ApprOrder 저장하므로 해당 정보로 GROUP 구성에 활용
                if (apvStep == 2 && int.TryParse(strApprData[5], out int apprStep))
                    apprdata.nApvOrder = apprStep;

                if (!strApprData[6].Equals(""))
                    apprdata.nApprPos = Convert.ToInt32(strApprData[6]);
                if (!strApprData[7].Equals(""))
                    apprdata.nDlpApprove = Convert.ToInt32(strApprData[7]);
                if (strApprData.Length >= 9 && !strApprData[8].Equals(""))
                    apprdata.APPR_USERID = strApprData[8];

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
