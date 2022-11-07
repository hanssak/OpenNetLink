using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Collections.Concurrent;

namespace OpenNetLinkApp.Data.SGDicData
{
    public class SGDicRecvData
    {
        // Dictionary / ConcurrentDictionary
        public ConcurrentDictionary<int, SGSvrData> m_DicSvrData;
        public ConcurrentDictionary<int, SGLoginData> m_DicLoginData;
        public ConcurrentDictionary<int, SGUserData> m_DicUserData;
        public ConcurrentDictionary<int, SGTransManageData> m_DicTransManageData;
        public ConcurrentDictionary<int, SGApprManageData> m_DicApprManageData;
        public ConcurrentDictionary<int, SGDetailData> m_DicDetailData;
        public ConcurrentDictionary<int, SGApprLineData> m_DicApprLineData;
        public ConcurrentDictionary<int, SGDeptApprLineSearchData> m_DicDeptApprLineSearchData;
        public ConcurrentDictionary<int, SGDeptInfo> m_DicDeptInfoData; //부서정보

        public ConcurrentDictionary<int, SGData> m_DicFileRecvNoti;
        public ConcurrentDictionary<int, SGData> m_DicBoardNoti;
        public ConcurrentDictionary<int, SGData> m_DicGpkiData;
        public ConcurrentDictionary<int, SGUrlListData> m_UrlListData;   // SGData
        public ConcurrentDictionary<int, SGData> m_DicSFMListData; // 자신이 지정된 대결재 정보 관리

        public SGDicRecvData()
        {
            m_DicSvrData = new ConcurrentDictionary<int, SGSvrData>();
            m_DicLoginData = new ConcurrentDictionary<int, SGLoginData>();
            m_DicUserData = new ConcurrentDictionary<int, SGUserData>();
            m_DicTransManageData = new ConcurrentDictionary<int, SGTransManageData>();
            m_DicApprManageData = new ConcurrentDictionary<int, SGApprManageData>();
            m_DicDetailData = new ConcurrentDictionary<int, SGDetailData>();
            m_DicApprLineData = new ConcurrentDictionary<int, SGApprLineData>();
            m_DicDeptApprLineSearchData = new ConcurrentDictionary<int, SGDeptApprLineSearchData>();
            m_DicFileRecvNoti = new ConcurrentDictionary<int, SGData>();
            m_DicBoardNoti = new ConcurrentDictionary<int, SGData>();
            m_DicGpkiData = new ConcurrentDictionary<int, SGData>();
            m_UrlListData = new ConcurrentDictionary<int, SGUrlListData>();
            m_DicSFMListData = new ConcurrentDictionary<int, SGData>();
            m_DicDeptInfoData = new ConcurrentDictionary<int, SGDeptInfo>();
        }
        ~SGDicRecvData()
        {

        }

        public int GetLoginDataCount()
        {
            return m_DicLoginData.Count;
        }
        public SGData GetLoginData(int groupid)
        {
            SGLoginData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicLoginData[groupid];
        }
        public void SetLoginData(HsNetWork hs, int groupid, SGData data)
        {
            SGLoginData tmpData = null;
            if (m_DicLoginData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicLoginData.TryRemove(groupid, out tmpData);
                //m_DicLoginData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGLoginData();
            tmpData.Copy(hs, data);
            //m_DicLoginData[groupid]= tmpData;
            m_DicLoginData.TryAdd(groupid, tmpData);
        }

        public SGData GetUserData(int groupid)
        {
            SGUserData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicUserData[groupid];
        }
        public void SetUserData(HsNetWork hs, int groupid, SGData data)
        {
            SGUserData tmpData = null;
            if (m_DicUserData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicUserData.TryRemove(groupid, out tmpData);
                //m_DicUserData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGUserData();
            tmpData.Copy(hs, data);
            //m_DicUserData[groupid]= tmpData;
            m_DicUserData.TryAdd(groupid, tmpData);
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
                m_DicSvrData.TryRemove(groupid, out tmpData);
                //m_DicSvrData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGSvrData();
            tmpData.Copy(data);

            m_DicSvrData.TryAdd(groupid, tmpData);
            //m_DicSvrData[groupid] = tmpData;
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
                m_DicTransManageData.TryRemove(groupid, out tmpData);
                //m_DicTransManageData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGTransManageData();
            tmpData.Copy(hs, data);

            m_DicTransManageData.TryAdd(groupid, tmpData);
            // m_DicTransManageData[groupid] = tmpData;
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
                m_DicApprManageData.TryRemove(groupid, out tmpData);
                //m_DicApprManageData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGApprManageData();
            tmpData.Copy(hs, data);

            m_DicApprManageData.TryAdd(groupid, tmpData);
            //m_DicApprManageData[groupid] = tmpData;
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
                m_DicDetailData.TryRemove(groupid, out tmpData);
                //m_DicDetailData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDetailData();
            tmpData.Copy(hs, data);

            m_DicDetailData.TryAdd(groupid, tmpData);
            //m_DicDetailData[groupid] = tmpData;
        }
        public void SetDetailDataChange(HsNetWork hs, int groupid, SGDetailData data)
        {
            SGDetailData tmpData = null;
            if (m_DicDetailData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicDetailData.TryRemove(groupid, out tmpData);
                //m_DicDetailData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDetailData();
            tmpData.DetailDataChange(hs, data);

            m_DicDetailData.TryAdd(groupid, tmpData);
            //m_DicDetailData[groupid] = tmpData;
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
                //m_DicApprLineData.Remove(groupid);
                m_DicApprLineData.TryRemove(groupid, out tmpData);
                tmpData = null;
            }

            tmpData = new SGApprLineData();
            tmpData.Copy(hs, data);

            m_DicApprLineData.TryAdd(groupid, tmpData);
            //m_DicApprLineData[groupid] = tmpData;
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
                m_DicDeptApprLineSearchData.TryRemove(groupid, out tmpData);
                //m_DicDeptApprLineSearchData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGDeptApprLineSearchData();
            tmpData.Copy(hs, data);

            m_DicDeptApprLineSearchData.TryAdd(groupid, tmpData);
            //m_DicDeptApprLineSearchData[groupid] = tmpData;
        }

        public SGData GetFileRecvNoti(int groupid)
        {
            SGData tmpData = null;
            if (m_DicFileRecvNoti.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicFileRecvNoti[groupid];
        }

        public void SetFileRecvNoti(HsNetWork hs, int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicFileRecvNoti.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicFileRecvNoti.TryRemove(groupid, out tmpData);
                //m_DicFileRecvNoti.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGData();
            tmpData.Copy(hs, data);

            m_DicFileRecvNoti.TryAdd(groupid, tmpData);
            //m_DicFileRecvNoti[groupid] = tmpData;
        }
        public SGData GetBoardNoti(int groupid)
        {
            SGData tmpData = null;
            if (m_DicBoardNoti.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicBoardNoti[groupid];
        }

        public void SetBoardNoti(HsNetWork hs, int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicBoardNoti.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicBoardNoti.TryRemove(groupid, out tmpData);
                //m_DicBoardNoti.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGData();
            tmpData.Copy(hs, data);

            m_DicBoardNoti.TryAdd(groupid, tmpData);
            //m_DicBoardNoti[groupid] = tmpData;
        }

        public SGData GetGpkiData(int groupid)
        {
            SGData tmpData = null;
            if (m_DicGpkiData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicGpkiData[groupid];
        }

        public void SetGpkiData(HsNetWork hs, int groupid, SGData data)
        {
            SGData tmpData = null;
            if (m_DicGpkiData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicGpkiData.TryRemove(groupid, out tmpData);
                //m_DicGpkiData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGData();
            tmpData.Copy(hs, data);

            m_DicGpkiData.TryAdd(groupid, tmpData);
            //m_DicGpkiData[groupid] = tmpData;
        }

        public SGData GetUrlListData(int groupid)
        {
            SGUrlListData tmpData = null;
            if (m_UrlListData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_UrlListData[groupid];
        }

        public void SetUrlListData(HsNetWork hs, int groupid, SGData data)
        {
            SGUrlListData tmpData = null;
            if (m_UrlListData.TryGetValue(groupid, out tmpData) == true)
            {
                m_UrlListData.TryRemove(groupid, out tmpData);
                //m_UrlListData.Remove(groupid);
                tmpData = null;
            }
            tmpData = new SGUrlListData();
            tmpData.Copy(hs, data);

            m_UrlListData.TryAdd(groupid, tmpData);
            //m_UrlListData[groupid] = tmpData;
        }

        public SGData GetSFMListData(int groupId)
        {
            if (m_DicSFMListData.ContainsKey(groupId))
                return m_DicSFMListData[groupId];
            else
                return null;
        }

        public void SetSFMListData(int groupId, SGData data)
        {
            if (m_DicSFMListData.ContainsKey(groupId))
            {
                SGData tmpData = null;
                if (m_DicSFMListData.TryRemove(groupId, out tmpData))
                    m_DicSFMListData.TryAdd(groupId, data);
                //m_DicSFMListData[groupId] = data;
            }
            else
            {
                m_DicSFMListData.TryAdd(groupId, data);
                //m_DicSFMListData.Add(groupId, data);
            }
        }

        public SGData GetDeptInfoData(int groupid)
        {
            SGDeptInfo tmpData = null;
            if (m_DicDeptInfoData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicDeptInfoData[groupid];
        }

        public void SetDeptInfoData(HsNetWork hs, int groupid, SGData data)
        {
            SGDeptInfo tmpData = null;
            if (m_DicDeptInfoData.TryGetValue(groupid, out tmpData) == true)
            {
                //m_DicApprLineData.Remove(groupid);
                m_DicDeptInfoData.TryRemove(groupid, out tmpData);
                tmpData = null;
            }

            tmpData = new SGDeptInfo();
            tmpData.Copy(hs, data);

            m_DicDeptInfoData.TryAdd(groupid, tmpData);
            //m_DicApprLineData[groupid] = tmpData;
        }
    }
}
