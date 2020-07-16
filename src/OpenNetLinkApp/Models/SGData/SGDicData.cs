using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;

namespace OpenNetLinkApp.Models.Data
{
    public class SGDicData
    {
        public Dictionary<int, SGData> m_DicSvrData;
        public Dictionary<int, SGData> m_DicLoginData;
        public Dictionary<int, SGData> m_DicUserData;
        public Dictionary<int, SGData> m_DicTransManageData;
        public Dictionary<int, SGData> m_DicApprManageData;
        public Dictionary<int, SGData> m_DicDetailData;
        public Dictionary<int, SGData> m_DicApprLineData;
        public SGDicData()
        {
            m_DicSvrData = new Dictionary<int, SGData>();
            m_DicLoginData = new Dictionary<int, SGData>();
            m_DicUserData = new Dictionary<int, SGData>();
            m_DicTransManageData = new Dictionary<int, SGData>();
            m_DicApprManageData = new Dictionary<int, SGData>();
            m_DicDetailData = new Dictionary<int, SGData>();
            m_DicApprLineData = new Dictionary<int, SGData>();
        }
        ~SGDicData()
        {

        }

        public SGData GetLoginData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicLoginData[groupid];
        }
        public void SetLoginData(int groupid,SGData data)
        {
            SGData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) == true)
                m_DicLoginData.Remove(groupid);

            m_DicLoginData[groupid]= data;
        }

        public SGData GetUserData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicUserData[groupid];
        }
        public void SetUserData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) == true)
                m_DicUserData.Remove(groupid);

            m_DicUserData[groupid]=data;
        }

        public SGData GetSvrData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicSvrData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicSvrData[groupid];
        }

        public void SetSvrData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicSvrData.TryGetValue(groupid, out tmpData) == true)
                m_DicSvrData.Remove(groupid);

            m_DicSvrData[groupid] = data;
        }

        public SGData GetTransManageData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicTransManageData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicTransManageData[groupid];
        }

        public void SetTransManageData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicTransManageData.TryGetValue(groupid, out tmpData) == true)
                m_DicTransManageData.Remove(groupid);

            m_DicTransManageData[groupid] = data;
        }

        public SGData GetApprManageData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicApprManageData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicApprManageData[groupid];
        }

        public void SetApprManageData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicApprManageData.TryGetValue(groupid, out tmpData) == true)
                m_DicApprManageData.Remove(groupid);

            m_DicApprManageData[groupid] = data;
        }

        public SGData GetDetailData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicDetailData[groupid];
        }

        public void SetDetailData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) == true)
                m_DicDetailData.Remove(groupid);

            m_DicDetailData[groupid] = data;
        }

        public SGData GetApprLineData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicApprLineData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicApprLineData[groupid];
        }

        public void SetApprLineData(int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicApprLineData.TryGetValue(groupid, out tmpData) == true)
                m_DicApprLineData.Remove(groupid);

            m_DicApprLineData[groupid] = data;
        }
    }
}
