using System;
using System.Collections.Generic;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Text;
using System.Text.Json;
using OpenNetLinkApp.Models.SGNetwork;
using System.IO;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services
{
    public class PageStatusService
    {
        public Dictionary<int, PageStatusData> m_DicPageStatusData;
        public PageStatusService()
        {
            m_DicPageStatusData = new Dictionary<int, PageStatusData>();

            string strNetworkFileName = "wwwroot/conf/NetWork.json";
            string jsonString = File.ReadAllText(strNetworkFileName);
            List<ISGNetwork> listNetworks = new List<ISGNetwork>();
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                JsonElement NetWorkElement = root.GetProperty("NETWORKS");
                //JsonElement Element;
                foreach (JsonElement netElement in NetWorkElement.EnumerateArray())
                {
                    SGNetwork sgNet = new SGNetwork();
                    string strJsonElement = netElement.ToString();
                    var options = new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                    };
                    sgNet = JsonSerializer.Deserialize<SGNetwork>(strJsonElement, options);
                    listNetworks.Add(sgNet);
                }
            }
            int count = listNetworks.Count;
            for (int i = 0; i < count; i++)
            {
                int groupID = listNetworks[i].GroupID;
                PageStatusData data = new PageStatusData();
                SetPageStatus(groupID, data);
            }
        }
        ~ PageStatusService()
        {

        }

        public PageStatusData GetPageStatus(int groupid)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupid, out tmpData) != true)
                return null;
            return m_DicPageStatusData[groupid];
        }

        public void SetPageStatus(int groupid, PageStatusData pageStatusdata)
        {
            if (pageStatusdata == null)
                return;

            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupid, out tmpData) == true)
            {
                m_DicPageStatusData.Remove(groupid);
                tmpData = null;
            }

            m_DicPageStatusData[groupid] = pageStatusdata;
        }
        public List<HsStream> GetFileList(int groupID)
        {
            PageStatusData data = null;
            data = GetPageStatus(groupID);
            if (data == null)
                return null;
            return data.GetFileDragListData();
        }

        public void SetFileList(int groupID, List<HsStream> hsList)
        {
            if (hsList == null)
                return;
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetFileDragListData(hsList);
        }

        public void SetFileAdd(int groupID, HsStream hs)
        {
            if (hs == null)
                return;
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetFileDragData(hs);
        }

        public void FileListClear(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].FileDragListClear();
        }

        public FileAddManage GetFileAddManage(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetFileAddManage();
        }

        public void SetAfterApprChkHIde(int groupID, bool bAfterApprCheckHide)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetAfterApprChkHIde(bAfterApprCheckHide);
        }
        public bool GetAfterApprChkHide(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetAfterApprChkHide();
        }
        public void SetAfterApprEnable(int groupID,bool bAfterApprEnable)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetAfterApprEnable(bAfterApprEnable);
        }
        public bool GetAfterApprEnable(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetAfterApprEnable();
        }

        public void SetSvrTime(int groupID, DateTime dt)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetSvrTime(dt);
        }

        public void SetAfterApprTimeEvent(int groupID, AfterApprTimeEvent afterApprTime)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetAfterApprTimeEvent(afterApprTime);
        }

        public DateTime GetAfterApprTime(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return DateTime.Now;
            }
            return m_DicPageStatusData[groupID].GetAfterApprTime();
        }

        public void SetDayFileAndClipMax(int groupID, Int64 fileMaxSize, int fileMaxCount, Int64 clipMaxSize, int clipMaxCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayFileAndClipMax(fileMaxSize, fileMaxCount,clipMaxSize,clipMaxCount);
        }

        public Int64 GetDayFileMaxSize(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayFileMaxSize();
        }

        public int GetDayFileMaxCount(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayFileMaxCount();
        }

        public Int64 GetDayClipMaxSize(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayClipMaxSize();
        }

        public int GetDayClipMaxCount(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayClipMaxCount();
        }

        public void SetDayUseFile(int groupID,Int64 fileSize, int fileCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayUseFile(fileSize, fileCount);
        }

        public void SetDayUseClip(int groupID,Int64 clipSize, int clipCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayUseClip(clipSize, clipCount);
        }

        public void SetDayRemainFile(int groupID,Int64 fileSize, int fileCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayRemainFile(fileSize, fileCount);
        }

        public void SetDayRemainClip(int groupID,Int64 clipSize, int clipCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayRemainClip(clipSize, clipCount);
        }
        public Int64 GetDayRemainFileSize(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileSize();
        }

        public int GetDayRemainFileCount(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileCount();
        }

        public Int64 GetDayRemainClipSize(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipSize();
        }

        public int GetDayRemainClipCount(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipCount();
        }
        public string GetDayRemainFileSizeString(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileSizeString();
        }

        public string GetDayRemainFileCountString(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileCountString();
        }

        public string GetDayRemainClipSizeString(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipSizeString();
        }

        public string GetDayRemainClipCountString(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipCountString();
        }

        public double GetDayRemainFileSizePercent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileSizePercent();
        }

        public double GetDayRemainFileCountPercent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainFileCountPercent();
        }

        public double GetDayRemainClipSizePercent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipSizePercent();
        }

        public double GetDayRemainClipCountPercent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetDayRemainClipCountPercent();
        }

        public bool GetDayFileTransSizeEnable(int groupID,Int64 nFileListSize)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileTransSizeEnable(nFileListSize);
        }
        public bool GetDayFileTransCountEnable(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileTransCountEnable();
        }

        public bool GetDayClipboardSizeEnable(int groupID, Int64 nClipSize)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileTransSizeEnable(nClipSize);
        }

        public bool GetDayClipboardCountEnable(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileTransCountEnable();
        }
    }
}
