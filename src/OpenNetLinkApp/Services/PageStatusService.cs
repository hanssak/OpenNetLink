using System;
using System.Collections.Generic;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Text;
using System.Text.Json;
using OpenNetLinkApp.Models.SGNetwork;
using System.IO;
using HsNetWorkSG;
using Microsoft.EntityFrameworkCore.Storage;
using OpenNetLinkApp.Models.SGSideBar;
using System.Collections.Concurrent;
using System.Timers;

namespace OpenNetLinkApp.Services
{
    public class PageStatusService
    {
        public static ConcurrentDictionary<int, PageStatusData> m_DicPageStatusData;
        public static ConcurrentDictionary<int, eLoginType> m_DicGroupIDloginType; // 다중망, m_bMultiLoginDo가 true일때 Server별로그인타입

        public bool m_bClipBoardSending = false;
        public bool m_bFileRecving = false;
        public bool m_bFileSending = false;
        public bool m_bFilePrevRecving = false;
        public bool m_bFileExaming = false;
        public bool m_bSFMChecking = false;                             // SFM 체크중
        public bool m_bApproveAfterMyCountChecking = false;             // 자신의 사후결재 카운트 조회중 
        public bool m_bSFMLogOut = false;                               // SFM 체크로 인한 로그아웃 여부

        public bool m_bMultiLoginDo = true;                             // 0각서버 로그인타입대로 로그인 진행할지 유무 (true : 각서버별 설정대로 로그인함-사용자가클릭했을때로그인진행, false: 0 번 GroupID 정보대로 1,2번에도 로그인함(추가해야함) )


        static Timer timer = null;
        static DateTime svrTime;
        //public delegate bool LoginAfterChkHide();
        //LoginAfterChkHide FuncLoginAfterChkHide;

        //사후결재 노티 발생 시 호출할 Login정보와 PageData 정보 보관
        static ConcurrentDictionary<int, AfterApproveNotiData> sDicAfterApproveNoti = new ConcurrentDictionary<int, AfterApproveNotiData>();

        /// <summary>
        /// 다중망 혹은 단일망 상관없이, 첫서버(GroupID=0)의 로그인타입, m_bMultiLoginDo 값과 무관하게 사용
        /// <para>0 : ORIGIN</para>
        /// <para>1 : AD</para>
        /// <para>2 : AD_LDAP</para>
        /// <para>3 : LDAP</para>
        /// <para>4 : PW_OTP</para>
        /// <para>5 : OTP</para>
        /// <para>6 : SSO</para>
        /// </summary>
        public int m_nLoginType = 0;                                     // 다중망 혹은 단일망 상관없이, 첫서버(GroupID=0)의 로그인타입, m_bMultiLoginDo 값과 무관하게 사용
        public bool m_bScrLock = false;

        public int m_nCurViewPageGroupID = 0;               // 현재 보여질 UI page가 속할 GroupID

        public int m_nLastViewPageGroupID = 0;               // 직전에 본 UI page의 GroupID

        public string m_strLoginToastTitle = "";                 // Login Title 저장
        public string m_strLoginToastMsg = "";                   // Login Message 저장

        public string m_str3NetDestSysID = "";                  // 3망 연계에서 보낼 정보저장

        public bool m_bIsMultiNetWork = false;                    // 다중접속상황 유무

        public ISGSideBarUI[] m_approveMenuArray = null;            // nGroupID 순서대로 결재관리 메뉴들 저장
        public ISGSideBarUI[] m_TransMenuArray = null;              // nGroupID 순서대로 전송관리 메뉴들 저장

        public List<ISGNetwork> listNetWork = null;         // 접속하는 망정보

        public bool m_bScreenLock = false;

        public PageStatusService()
        {
            m_DicPageStatusData = new ConcurrentDictionary<int, PageStatusData>();
            m_DicGroupIDloginType = new ConcurrentDictionary<int, eLoginType>();

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
                PageStatusData data = new PageStatusData(groupID);
                SetPageStatus(groupID, data);
            }
        }
        ~PageStatusService()
        {

        }

        public PageStatusData GetPageStatus(int groupid)
        {

            if (!m_DicPageStatusData.ContainsKey(groupid))
                return null;
            return m_DicPageStatusData[groupid];
        }

        public void SetPageStatus(int groupid, PageStatusData pageStatusdata)
        {
            if (pageStatusdata == null)
                return;

            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupid, out tmpData))
            {
                m_DicPageStatusData.TryUpdate(groupid, pageStatusdata, tmpData);
            }
            else
            {
                m_DicPageStatusData.TryAdd(groupid, pageStatusdata);
            }
        }

        public eLoginType GetGroupIDLoginType(int groupid)
        {
            if (!m_DicGroupIDloginType.ContainsKey(groupid))
            {
                return (eLoginType)100;
            }
            return m_DicGroupIDloginType[groupid];
        }

        public void SetGroupIDLoginType(int groupid, eLoginType groupIDloginType)
        {
            eLoginType tmpData = (eLoginType)100;
            if (m_DicGroupIDloginType.TryGetValue(groupid, out tmpData))
            {
                m_DicGroupIDloginType.TryUpdate(groupid, groupIDloginType, tmpData);
            }
            else
            {
                m_DicGroupIDloginType.TryAdd(groupid, groupIDloginType);
            }
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

        /// <summary>
        /// 기존 Stream에서 신규 Stream 항목들 '추가'
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="listHS"></param>
        public void SetFileAdd(int groupID, List<HsStream> listHS)
        {
            if (listHS == null || listHS.Count < 1)
                return;
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetFileDragData(listHS);
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

        public void SetTargetSystemList(int groupID, Dictionary<string, SGNetOverData> dicSystemIdName)
        {
            if (dicSystemIdName == null)
                return;

            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
                return;

            m_DicPageStatusData[groupID].SetTargetSystemListData(dicSystemIdName);
        }

        public Dictionary<string, SGNetOverData> GetTargetSystemList(int groupID)
        {
            PageStatusData data = null;
            data = GetPageStatus(groupID);
            if (data == null)
                return null;
            return data.GetTargetSystemListData();
        }


        /**
		 * @breif 원래 사용하던 결재정책에 3 망연계 결재 정책까지 겸해서 결정(3망정책, 결재전부안쓸때에만 결재 안쓴다고 판단)
		 * @return true : 결재 사용
		 */
        /// <summary>
        /// 결제 사용 여부
        /// <para>[서버 정책] 없으면 결제 사용 못함</para>
        /// <para>[서버 정책-결제사용여부] false 면 사용 못함</para>
        /// <para>3망연계가 없다면 [서버 정책-결제사용여부]에 따라 사용 가능함.</para>
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bAfterApprCheckHide"><para>true:사후결제 불가 / false:사후결제 가능</para></param>
        public bool GetUseApproveNetOver(int groupID, SGLoginData sgLoginData)
        {

            if (sgLoginData == null)                //[서버 정책] 없으면 결제 사용 못함
                return false;

            if (sgLoginData.GetApprove() == false)  //[서버 정책-결제사용여부] false 면 사용 못함
                return false;

            // 3망연계 사용하지 않으면 기존 정책만 반영
            if (sgLoginData.GetUseOverNetwork2() == false)  //3망연계가 존재하지 않다면 [서버 정책-결제사용여부]에 따라 사용 가능함.
                return true;


            //3망 연계를 사용하는 경우.
            PageStatusData data = null;
            data = GetPageStatus(groupID);
            if (data == null)
                return true;

            Dictionary<string, SGNetOverData> dicSysIdName = data.GetTargetSystemListData();
            if (dicSysIdName == null || dicSysIdName.Count < 3)
                return true;

            // 3망 정보에 의한 결재사용 결정
            foreach (var item in dicSysIdName)
            {
                // 한곳이라도 결재를 쓰면 결재를 사용하는 걸로
                if (item.Value.bUseApprove)
                    return true;

                // 다중망 상황에서는 바로 맞은 편망에 결재 사용 설정 정보까지만 본다.
                if (listNetWork != null && listNetWork.Count > 1 && item.Value.nIdx == 1)
                    break;
            }

            return false;
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

        /// <summary>
        /// 사후 결제 사용 가능 여부
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="bAfterApprCheckHide"><para>true:사후결제 불가 / false:사후결제 가능</para></param>
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
        public void SetIsPolicyUpdate(int groupID, bool bPolicyUpdate)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetPolicyUpdate(bPolicyUpdate);
        }
        public bool GetIsPolicyUpdate(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetPolicyUpdate();
        }
        public void SetAfterApprEnable(int groupID, bool bAfterApprEnable)
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

        public void SetAfterApproveCheck(int groupID, bool bUserCheckAfterApprove)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetAfterApproveCheck(bUserCheckAfterApprove);
        }

        public bool GetAfterApproveCheck(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetAfterApproveCheck();
        }

        public void SetSvrTime(int groupID, DateTime dt, SGLoginData sgLoginData)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
                return;

            //호출할 이벤트 선언
            AfterApproveNotiData afterApprove;
            if (sDicAfterApproveNoti.TryGetValue(groupID, out afterApprove))
            {
                afterApprove.AfterApprovePageStatusData = tmpData;
                afterApprove.AfterApproveSGLoginData = sgLoginData;
            }
            else
            {
                sDicAfterApproveNoti[groupID] = new AfterApproveNotiData(tmpData, sgLoginData);
            }

            svrTime = dt;
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 1000;              // 1초
                timer.Elapsed += new ElapsedEventHandler(AfterApprTimer);
                timer.Start();
            }
        }

        public static void AfterApprTimer(object sender, ElapsedEventArgs e)
        {
            svrTime = svrTime.AddSeconds(1);

            if ((svrTime.Minute == 00) && (svrTime.Second == 0))
            {
                //매시간 정각마다 실행
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //사후결재 정보 groupID별 정보 재설정 함수 호출
                foreach (AfterApproveNotiData notiData in sDicAfterApproveNoti.Values)
                {
                    SGLoginData sgLogin = notiData.AfterApproveSGLoginData;
                    PageStatusData pageStatus = notiData.AfterApprovePageStatusData;

                    //실시간 사후결재 체크 함수 호출 값을 GroupId 별로 m_bAfterApprCheckHide 갱신                    
                    bool bAfterApprChkHIde = sgLogin.GetAfterChkHide();
                    bool bAfterAppr = sgLogin.GetUseAfterApprove(svrTime);
                    pageStatus.SetAfterApprChkHIde(bAfterApprChkHIde);
                    pageStatus.SetAfterApprEnable(bAfterAppr);
                }

                //[사후결재 정보 갱신] 작업 Page 노티 호출
                if (PageStatusData.SNotiEvent != null)
                    PageStatusData.SNotiEvent();

                //[사후결재 정보 갱신] 공통UI 노티 호출
                if (PageStatusData.SCommonNotiEvent != null)
                    PageStatusData.SCommonNotiEvent();

                //매일 자정마다 '일일 전송 가능' 정보 갱신 by 2022.08.
                if (svrTime.Hour == 0 && PageStatusData.RefreshInfoEvent != null)  //매일 자정마다 실행
                    PageStatusData.RefreshInfoEvent();

                //매일 자정마다 Alram & Message 정보 삭제
                if (svrTime.Hour == 0 && PageStatusData.DeleteAlramAndMessage != null)  //매일 자정마다 실행
                    PageStatusData.DeleteAlramAndMessage();

                //매일 자정마다 오래된 로그 삭제
                if (svrTime.Hour == 0)
                {
                    AgLogManager.HsLogDel hsLog = new AgLogManager.HsLogDel();
                    hsLog.Delete(7);    // 7일이전 Log들 삭제
                }
            }
        }

        /// <summary>
        /// 매시 정각에 한번씩 동작
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="afterApprTime"></param>
        public void SetAfterApprTimeEvent(int groupID, AfterApprTimeEvent afterApprTime)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            //m_DicPageStatusData[groupID].SetAfterApprTimeEvent(afterApprTime);
            PageStatusData.SNotiEvent = afterApprTime;
        }

        public DateTime GetAfterApprTime(int groupID)
        {
            //PageStatusData tmpData = null;
            //if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            //{
            //    return DateTime.Now;
            //}
            //return m_DicPageStatusData[groupID].GetAfterApprTime();
            return svrTime;
        }

        public void SetAfterApprCommonNotiEvent(AfterApprTimeEvent afterApprTime)
        {
            //PageStatusData tmpData = null;
            //if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            //{
            //    return;
            //}
            ////m_DicPageStatusData[groupID].SetAfterApprTimeEvent(afterApprTime);
            PageStatusData.SCommonNotiEvent = afterApprTime;
        }

        public void SetAlramAndMessageDeleteEvent(DeleteAlramAndMessageEvent deleteEvent)
        {
            PageStatusData.DeleteAlramAndMessage = deleteEvent;
        }
        /// <summary>
        ///  공통환경설정의 사후결재 컨트롤 체크여부 변경 노티 (공통환경설정 -> 파일전송)
        /// </summary>
        /// <param name="a"></param>
        public void SetAfterApprControlCheckEvent(int groupID, AfterApprTimeEvent afterApprControlCheck)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            PageStatusData.SAfterApprControlCheckEvent = afterApprControlCheck;
        }

        public void SetLogOutFileListClearEvent(int groupID, LogOutFileListClearEvent Event)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetLogoutFileListClearEvent(Event);            
        }
        public LogOutFileListClearEvent GetLogOutFileListClearEvent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetLogoutFileListClearEvent();
        }


        public void SetDayFileAndClipMax(int groupID, Int64 fileMaxSize, int fileMaxCount, Int64 clipMaxSize, int clipMaxCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayFileAndClipMax(fileMaxSize, fileMaxCount, clipMaxSize, clipMaxCount);
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

        public void SetDayUseFile(int groupID, Int64 fileSize, int fileCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayUseFile(fileSize, fileCount);
        }

        public void SetDayUseClip(int groupID, Int64 clipSize, int clipCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayUseClip(clipSize, clipCount);
        }

        public void SetDayRemainFile(int groupID, Int64 fileSize, int fileCount)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayRemainFile(fileSize, fileCount);
        }

        public void SetDayRemainClip(int groupID, Int64 clipSize, int clipCount)
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

        public bool GetDayFileTransSizeEnable(int groupID, Int64 nFileListSize)
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
            return m_DicPageStatusData[groupID].GetDayClipboardSizeEnable(nClipSize);
        }

        public bool GetDayClipboardCountEnable(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayClipboardCountEnable();
        }

        public void SetLoginComplete(int groupID, bool bLoginComplete)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetLoginComplete(bLoginComplete);
        }
        public bool GetLoginComplete(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetLoginComplete();
        }
        public bool GetDayInfoPrev(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayInfoPrev();
        }

        public void SetDayInfoPrev(int groupID, bool bFileView)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayInfoPrev(bFileView);
        }

        public bool GetDayFileSizeUnLimited(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileSizeUnLimited();
        }

        public bool GetDayFileCountUnLimited(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayFileCountUnLimited();
        }

        public bool GetDayClipSizeUnLimited(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayClipSizeUnLimited();
        }

        public bool GetDayClipCountUnLimited(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetDayClipCountUnLimited();
        }

        public ePassWDType GetPassWDChgType(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return ePassWDType.eNone;
            }
            return m_DicPageStatusData[groupID].GetPassWDChgType();
        }
        public void SetPassWDChgType(int groupID, ePassWDType ePassWDChgType)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetPassWDChgType(ePassWDChgType);
        }
        public InitPassWDCHGEvent GetInitPassWDCHGEvent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetInitPassWDCHGEvent();
        }
        public void SetInitPassWDCHGEvent(int groupID, InitPassWDCHGEvent initPasswdChgEvent)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetInitPassWDCHGEvent(initPasswdChgEvent);
        }

        public DayPassWDCHGEvent GetDayPassWDCHGEvent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetDayPassWDCHGEvent();
        }
        public void SetDayPassWDCHGEvent(int groupID, DayPassWDCHGEvent dayPasswdChgEvent)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetDayPassWDCHGEvent(dayPasswdChgEvent);
        }

        public UserPassWDCHGEvent GetUserPassWDCHGEvent(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetUserPassWDCHGEvent();
        }
        public void SetUserPassWDCHGEvent(int groupID, UserPassWDCHGEvent userPasswdChgEvent)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetUserPassWDCHGEvent(userPasswdChgEvent);
        }

        public void SetCurUserPassWD(int groupID, string strPW)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetCurUserPassWD(strPW);
        }
        public string GetCurUserPassWD(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetCurUserPassWD();
        }

        public string GetEncCurUserPassWD(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetEncCurUserPassWD();
        }

        public void SetSessionKey(int groupID, string strSessionKey)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetSessionKey(strSessionKey);
        }

        public int GetConnectCount(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return 0;
            }
            return m_DicPageStatusData[groupID].GetConnectCount();
        }
        public void ConnectCountAdd(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].ConnectCountAdd();
        }
        public bool GetConnectStatus(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetConnectStatus();
        }
        public void SetConnectStatus(int groupID, bool bConnect)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetConnectStatus(bConnect);
        }

        public bool GetUseClipBoard(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetUseClipBoard();
        }
        public void SetUseClipBoard(int groupID, bool bUse)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetUseClipBoard(bUse);
        }


        public void SetFileRecving(bool bFileRecving)
        {
            m_bFileRecving = bFileRecving;
        }
        public bool GetFileRecving()
        {
            return m_bFileRecving;
        }
        public void SetFileSending(bool bFileSending)
        {
            m_bFileSending = bFileSending;
        }

        public bool GetFileSending()
        {
            return m_bFileSending;
        }
        public void SetFilePrevRecving(bool bFilePrevRecving)
        {
            m_bFilePrevRecving = bFilePrevRecving;
        }
        public bool GetFilePrevRecving()
        {
            return m_bFilePrevRecving;
        }
        public bool GetClipboardSending()
        {
            return m_bClipBoardSending;
        }
        public void SetClipboardSending(bool bClipboardSending)
        {
            m_bClipBoardSending = bClipboardSending;
        }
        public void SetFileExaming(bool bFileExaming)
        {
            m_bFileExaming = bFileExaming;
        }
        public bool GetFileExaming()
        {
            return m_bFileExaming;
        }

        public void SetSFMChecking(bool bSFMChecking)
        {
            this.m_bSFMChecking = bSFMChecking;
        }

        public bool GetSFMChecking()
        {
            return this.m_bSFMChecking;
        }

        public void SetSFMLogOut(bool bSFMLogOut)
        {
            this.m_bSFMLogOut = bSFMLogOut;
        }

        public bool GetSFMLogOut()
        {
            return this.m_bSFMLogOut;
        }

        public void SetApproveAfterMyCountChecking(bool bApproveAfterMyCountChecking)
        {
            this.m_bApproveAfterMyCountChecking = bApproveAfterMyCountChecking;
        }

        public bool GetApproveAfterMyCountChecking()
        {
            return this.m_bApproveAfterMyCountChecking;
        }

        public void SetScrLocking(bool bScrLock)
        {
            m_bScrLock = bScrLock;
        }
        public bool GetScrLocking()
        {
            return m_bScrLock;
        }

        public bool GetLoadApprBaseLine(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetLoadApprBaseLine();
        }
        public void SetLoadApprBaseLine(int groupID, bool bLoadApprBaseLine)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetLoadApprBaseLine(bLoadApprBaseLine);
        }

        public bool GetInitApprLine(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetInitApprLine();
        }
        public void SetInitApprLine(int groupID, bool bInitApprLine)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetInitApprLine(bInitApprLine);
        }

        public void SetCurFileSendInfo(int groupID, string strFileSendInfo)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetCurFileSendInfo(strFileSendInfo);
        }
        public string GetCurFileSendInfo(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetCurFileSendInfo();
        }

        public bool GetLogoutStatus(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetLogoutStatus();
        }
        public void SetLogoutStatus(int groupID, bool bLogout)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetLogoutStatus(bLogout);
        }
        public void SetBoardHash(int groupID, string strHash)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetBoardHash(strHash);
        }
        public string GetBoardHash(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return "";
            }
            return m_DicPageStatusData[groupID].GetBoardHash();
        }
        public string GetFileTransPage(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return null;
            }
            return m_DicPageStatusData[groupID].GetFileTransPage();
        }
        public void SetFileTransPage(int groupID, string strPage)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }
            m_DicPageStatusData[groupID].SetFileTransPage(strPage);
        }

        public void SetDayInfoRefreshEvent(int groupID, DayInfoRefreshEvent refreshInfo)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
                return;
            m_DicPageStatusData[groupID].SetDayInfoRefreshEvent(refreshInfo);
        }

        public void SetApproveExtFileTransWithApprove(int groupID, bool bUseApprove)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return;
            }

            m_DicPageStatusData[groupID].SetApproveExtFileTransWithApprove(bUseApprove);
        }
        public bool GetApproveExtFileTransWithApprove(int groupID)
        {
            PageStatusData tmpData = null;
            if (m_DicPageStatusData.TryGetValue(groupID, out tmpData) != true)
            {
                return false;
            }
            return m_DicPageStatusData[groupID].GetApproveExtFileTransWithApprove();
        }
    }
}
