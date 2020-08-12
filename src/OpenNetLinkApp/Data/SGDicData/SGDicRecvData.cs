using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;

namespace OpenNetLinkApp.Data.SGDicData
{
    public class SGDicRecvData
    {
        public Dictionary<int, SGSvrData> m_DicSvrData;
        public Dictionary<int, SGLoginData> m_DicLoginData;
        public Dictionary<int, SGUserData> m_DicUserData;
        public Dictionary<int, SGTransManageData> m_DicTransManageData;
        public Dictionary<int, SGApprManageData> m_DicApprManageData;
        public Dictionary<int, SGDetailData> m_DicDetailData;
        public Dictionary<int, SGApprLineData> m_DicApprLineData;
        public Dictionary<int, SGDeptApprLineSearchData> m_DicDeptApprLineSearchData;

        public Dictionary<int, SGData> m_DicFileSendProgressNoti;
        public Dictionary<int, SGData> m_DicFileRecvProgressNoti;
        public SGDicRecvData()
        {
            m_DicSvrData = new Dictionary<int, SGSvrData>();
            m_DicLoginData = new Dictionary<int, SGLoginData>();
            m_DicUserData = new Dictionary<int, SGUserData>();
            m_DicTransManageData = new Dictionary<int, SGTransManageData>();
            m_DicApprManageData = new Dictionary<int, SGApprManageData>();
            m_DicDetailData = new Dictionary<int, SGDetailData>();
            m_DicApprLineData = new Dictionary<int, SGApprLineData>();
            m_DicDeptApprLineSearchData = new Dictionary<int, SGDeptApprLineSearchData>();
            m_DicFileSendProgressNoti = new Dictionary<int, SGData>();
            m_DicFileRecvProgressNoti = new Dictionary<int, SGData>();
        }
        ~SGDicRecvData()
        {

        }
        public SGData GetLoginData(int groupid)
        {
            SGLoginData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicLoginData[groupid];
        }
        public void SetLoginData(HsNetWork hs,int groupid,SGData data)
        {
            SGLoginData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicLoginData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGLoginData();
            tmpData.Copy(hs,data);
            m_DicLoginData[groupid]= tmpData;
        }

        public SGData GetUserData(int groupid)
        {
            SGUserData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicUserData[groupid];
        }
        public void SetUserData(HsNetWork hs,int groupid, SGData data)
        {
            SGUserData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) == true)
            { 
                m_DicUserData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGUserData();
            tmpData.Copy(hs,data);

            m_DicUserData[groupid]= tmpData;
        }

        public SGData GetSvrData(int groupid)
        {
            SGSvrData tmpData = null;
            if (m_DicSvrData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicSvrData[groupid];
        }

        public void SetSvrData(int groupid, SGData data)
        {
            SGSvrData tmpData = null;
            if (m_DicSvrData.TryGetValue(groupid, out tmpData) == true)
            { 
                m_DicSvrData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGSvrData();
            tmpData.Copy(data);

            m_DicSvrData[groupid] = tmpData;
        }

        public SGData GetTransManageData(int groupid)
        {
            SGTransManageData tmpData = null;
            if (m_DicTransManageData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicTransManageData[groupid];
        }

        public void SetTransManageData(HsNetWork hs, int groupid, SGData data)
        {
            SGTransManageData tmpData = null;
            if (m_DicTransManageData.TryGetValue(groupid, out tmpData) == true)
            { 
                m_DicTransManageData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGTransManageData();
            tmpData.Copy(hs, data);

            m_DicTransManageData[groupid] = tmpData;
        }

        public SGData GetApprManageData(int groupid)
        {
            SGApprManageData tmpData = null;
            if (m_DicApprManageData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicApprManageData[groupid];
        }

        public void SetApprManageData(HsNetWork hs, int groupid, SGData data)
        {
            SGApprManageData tmpData = null;
            if (m_DicApprManageData.TryGetValue(groupid, out tmpData) == true)
            { 
                m_DicApprManageData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGApprManageData();
            tmpData.Copy(hs, data);

            m_DicApprManageData[groupid] = tmpData;
        }

        public SGData GetDetailData(int groupid)
        {
            SGDetailData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicDetailData[groupid];
        }

        public void SetDetailData(HsNetWork hs, int groupid, SGData data)
        {
            SGDetailData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) == true)
            { 
                m_DicDetailData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDetailData();
            tmpData.Copy(hs, data);

            m_DicDetailData[groupid] = tmpData;
        }
        public void SetDetailDataChange(HsNetWork hs, int groupid, SGDetailData data)
        {
            SGDetailData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicDetailData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDetailData();
            tmpData.DetailDataChange(hs, data);

            m_DicDetailData[groupid] = tmpData;
        }

        public SGData GetApprLineData(int groupid)
        {
            SGApprLineData tmpData = null;
            if (m_DicApprLineData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicApprLineData[groupid];
        }

        public void SetApprLineData(HsNetWork hs, int groupid, SGData data)
        {
            SGApprLineData tmpData = null;
            if (m_DicApprLineData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicApprLineData.Remove(groupid);
                tmpData = null;
            }

            tmpData = new SGApprLineData();
            tmpData.Copy(hs, data);

            m_DicApprLineData[groupid] = tmpData;
        }

        public void SetApprLineList(int groupid, LinkedList<ApproverInfo> LinkedApprInfo)
        {
            SGApprLineData tmpData = null;
            if (!m_DicApprLineData.TryGetValue(groupid, out tmpData))
            {
                return;
            }
            tmpData = m_DicApprLineData[groupid];
            if (tmpData != null)
                tmpData.SetApprAndLindData(LinkedApprInfo);
        }

        public SGData GetDeptApprLineSearchData(int groupid)
        {
            SGDeptApprLineSearchData tmpData = null;
            if (m_DicDeptApprLineSearchData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicDeptApprLineSearchData[groupid];
        }

        public void SetDeptApprLineSearchData(HsNetWork hs, int groupid, SGData data)
        {
            SGDeptApprLineSearchData tmpData = null;
            if (m_DicDeptApprLineSearchData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicDeptApprLineSearchData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDeptApprLineSearchData();
            tmpData.Copy(hs, data);

            m_DicDeptApprLineSearchData[groupid] = tmpData;
        }

        public SGData GetFileSendProgressNotiData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicFileSendProgressNoti.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicFileSendProgressNoti[groupid];
        }

        public void SetFileSendProgressNotiData(HsNetWork hs, int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicFileSendProgressNoti.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicFileSendProgressNoti.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDeptApprLineSearchData();
            tmpData.Copy(hs, data);

            m_DicFileSendProgressNoti[groupid] = tmpData;
        }

        public SGData GetFileRecvProgressNotiData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicFileRecvProgressNoti.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicFileRecvProgressNoti[groupid];
        }

        public void SetFileRecvProgressNotiData(HsNetWork hs, int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicFileRecvProgressNoti.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicFileRecvProgressNoti.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDeptApprLineSearchData();
            tmpData.Copy(hs, data);

            m_DicFileRecvProgressNoti[groupid] = tmpData;
        }
    }
}
