using HsNetWorkSG;
using HsNetWorkSGData;
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
            ChildrenInfo = new List<DeptTreeInfo>();
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
            m_DicTagData = new Dictionary<string, string>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        public List<DeptTreeInfo> GetDeptInfoTree(string SelectDeptSeq)// List<DeptInfo> GetDeptInfoTree()
        {
            if (deptTreeInfoValues == null)
                deptTreeInfoValues = new List<DeptTreeInfo>();

            deptTreeInfoValues.Clear();

            List<Dictionary<int, string>> listRecord = GetRecordData("DEPTRECORD");
            foreach (Dictionary<int, string> record in listRecord)
            {
                deptTreeInfoValues.Add(new DeptTreeInfo(record[0], record[1], record[2]));
            }

            List<DeptTreeInfo> topTree = deptTreeInfoValues.FindAll(dept => dept.ParentDeptSeq == "0"); //Top Tree
            foreach (DeptTreeInfo top in topTree)
            {
                if (SelectDeptSeq == top.DeptSeq)        //TOP에서 선택 부서를 찾은 경우 (이후 Parent 들은 Expand 전부 False (찾을필요도 없음)
                {
                    top.IsExpanded = false;
                    //Prent 없으므로 별도 세팅 없음
                    //parentDept.ExistsIntoChildren = true;   //부모 Tree는 Expanded 하도록
                }

                List<DeptTreeInfo> children = getChildren(top, SelectDeptSeq);

                if (top.ExistsIntoChildren) //내 자식 중 선택 dept가 존재하는 경우
                    top.IsExpanded = true;

                top.ChildrenInfo.AddRange(children);
            }

            return topTree;

        }

        List<DeptTreeInfo> getChildren(DeptTreeInfo parentDept, string getSelectDeptSeq)
        {
            List<DeptTreeInfo> children = deptTreeInfoValues.FindAll(dept => dept.ParentDeptSeq == parentDept.DeptSeq);   //Parent의 하위 조회
            foreach (DeptTreeInfo child in children)
            {
                if (getSelectDeptSeq == child.DeptSeq)
                {
                    //찾음
                    child.IsExpanded = false;               //해당 Tree는 Expanded 하지 않음
                    parentDept.ExistsIntoChildren = true;   //부모 Tree는 Expanded 하도록
                }

                //자식 세팅
                List<DeptTreeInfo> children2 = getChildren(child, getSelectDeptSeq);
                child.ChildrenInfo.AddRange(children2);

                if (child.ExistsIntoChildren)   //내 자식 중 선택 Dept가 존재하는 경우
                {
                    child.IsExpanded = true;
                    parentDept.ExistsIntoChildren = true;
                }
            }

            return children;
        }

    }
}
