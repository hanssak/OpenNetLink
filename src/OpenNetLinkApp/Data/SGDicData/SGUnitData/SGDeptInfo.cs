using HsNetWorkSG;
using HsNetWorkSGData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using AgLogManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    /// <summary>
    /// 부서정보 조회 요청에 대한 응답 데이터 ($DEPTINFO)
    /// </summary>
    public class DeptTreeInfo
    {
        public DeptTreeInfo(string deptSeq, string deptName, string parentDeptSeq)
        {
            DeptSeq = deptSeq;
            DeptName = deptName;
            ParentDeptSeq = parentDeptSeq;
            IsExpanded = false;
            ExistsIntoChildren = false;
            ChildrenInfo = null;
        }

        public DeptTreeInfo() { }
        /// <summary>
        /// 사용자 SEQ
        /// </summary>
        public string DeptSeq { get; set; }

        /// <summary>
        /// 사용자명
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 사용자ID
        /// </summary>
        public string ParentDeptSeq { get; set; }

        public bool IsExpanded { get; set; }

        public bool ExistsIntoChildren { get; set; }
        public List<DeptTreeInfo> ChildrenInfo { get; set; }
    }

    public class SGDeptInfo : SGData
    {

        List<DeptTreeInfo> deptTreeInfoValues = null;

        public override void Copy(HsNetWork hs, SGData data)
        {
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }

        bool GetDeptinfoNodeData(string strData)
        {

            if (string.IsNullOrEmpty(strData))
                return false;

            if (deptTreeInfoValues == null)
                return false;

            bool bRet = false;

            try
            {

                var dataList = JsonConvert.DeserializeObject<List<dynamic>>(strData);
                string strItemData = "";
                bRet = (dataList?.Count ?? 0) > 0;
                foreach (var dataItem in dataList)
                {
                    strItemData = Convert.ToString(dataItem);
                    JObject jO = JObject.Parse(strItemData);

                    string strTmpData = jO.ContainsKey("parent_dept_seq") ? (string)jO["parent_dept_seq"] : "0";
                    deptTreeInfoValues.Add(new DeptTreeInfo((string)jO["dept_seq"], (string)jO["dept_name"], strTmpData));

                    Log.Logger.Here().Error($"GetDeptinfoNodeData, Dept-Name : {(string)jO["dept_name"]}, Dept-Seq : {(string)jO["dept_seq"]}, parent_dept_seq : {strTmpData}, sub_dept : {(jO.ContainsKey("sub_dept_list") ? "O" : "X")}");

                    if (jO.ContainsKey("sub_dept_list"))
                    {
                        GetDeptinfoNodeData(Convert.ToString(jO["sub_dept_list"]));
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"GetDeptinfoNodeData, Exception(MSG) : {ex.Message}");
            }

            return bRet;
        }

        /// <summary>
        /// Tree 컨트롤에 사용하기 위한 List 객체 별도 생성
        /// </summary>
        /// <param name="SelectDeptSeq"></param>
        /// <returns></returns>
        public List<DeptTreeInfo> GetDeptInfoTree(string SelectDeptSeq)// List<DeptInfo> GetDeptInfoTree()
        {
            if (deptTreeInfoValues == null)
                deptTreeInfoValues = new List<DeptTreeInfo>();

            deptTreeInfoValues.Clear();

            // 예전
            /*List<Dictionary<int, string>> listRecord = GetRecordData("DEPTRECORD");
            foreach (Dictionary<int, string> record in listRecord)
            {
                deptTreeInfoValues.Add(new DeptTreeInfo(record[0], record[1], record[2]));
            }*/

            // Advanced
            GetDeptinfoNodeData(GetTagData("dept_list"));


            List<DeptTreeInfo> topTree = deptTreeInfoValues.FindAll(dept => dept.ParentDeptSeq == "0"); //Top Tree
            foreach (DeptTreeInfo top in topTree)
            {
                if (SelectDeptSeq == top.DeptSeq)       //선택된 Dept 인 경우
                {
                    top.IsExpanded = false;             //해당 Tree는 Expanded 하지 않음
                }

                //하위 세팅
                List<DeptTreeInfo> addTree = getChildren(top, SelectDeptSeq);

                top.ChildrenInfo = addTree;

                if (top.ExistsIntoChildren) //Children에 선택 Dept가 존재하는 경우, Expand
                    top.IsExpanded = true;
            }
            return topTree;
        }

        List<DeptTreeInfo> getChildren(DeptTreeInfo parentDept, string getSelectDeptSeq)
        {
            List<DeptTreeInfo> children = deptTreeInfoValues.FindAll(dept => dept.ParentDeptSeq == parentDept.DeptSeq);   //Parent에 대한 Children 조회
            if (children.Count <= 0)
                return null;

            foreach (DeptTreeInfo child in children)
            {
                if (getSelectDeptSeq == child.DeptSeq)      //선택된 Dept 인 경우
                {
                    child.IsExpanded = false;               //해당 Tree는 Expanded 하지 않음
                    parentDept.ExistsIntoChildren = true;   //부모 Tree는 Expanded 하도록
                }

                //하위 세팅
                List<DeptTreeInfo> addTree = getChildren(child, getSelectDeptSeq);
                child.ChildrenInfo = addTree;

                if (child.ExistsIntoChildren)   ///Children에 선택 Dept가 존재하는 경우, Expand
                {
                    child.IsExpanded = true;
                    parentDept.ExistsIntoChildren = true;
                }
            }
            return children;
        }
    }
}
