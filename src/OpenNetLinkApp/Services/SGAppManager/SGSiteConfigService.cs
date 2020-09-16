using System;
using OpenNetLinkApp.Models.SGConfig;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using HsNetWorkSG;
using OpenNetLinkApp.Models.SGNetwork;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSiteConfigService
    {
        public List<ISGSiteConfig> SiteConfigInfo { get;}

        public bool GetUseLoginIDSave(int groupID);
        public bool GetUseAutoLogin(int groupID);
        public bool GetUseApprLineLocalSave(int groupID);
        public int GetZipPWBlock(int groupID);
        public bool GetUseApprLineChkBlock(int groupID);
        public bool GetUseDlpInfoDisplay(int groupID);
        public bool GetUseApprDeptSearch(int groupID);
        public bool GetUseApprTreeSearch(int groupID);
        public int GetApprStepLimit(int groupID);
        public bool GetUseDeputyApprTerminateDel(int groupID);
        public bool GetUseUserPWChange(int groupID);
        public string GetPWChangeProhibitLimit(int groupID);
        public int GetPWChangeApplyCnt(int groupID);
        public bool GetUseURLListPolicyRecv(int groupID);
        public string GetInitPasswordInfo(int groupID);
    }
    internal class SGSiteConfigService : ISGSiteConfigService
    {
        public List<ISGSiteConfig> SiteConfigInfo { get; set; } = null;
        public SGSiteConfigService()
        {
            SiteConfigInfo = new List<ISGSiteConfig>();
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
            for(int i=0;i<count; i++)
            {
                SGSiteConfig sgSiteConfig = new SGSiteConfig();
                sgSiteConfig.m_bUserIDSave = true;                    // 로그인한 ID 저장 여부
                sgSiteConfig.m_bAutoLogin = true;                     // 자동로그인 사용 여부.
                sgSiteConfig.m_bAutoLoginCheck = false;                    // 자동로그인 체크박스 체크여부.
                sgSiteConfig.m_bApprLineLocalSave = false;            // 결재라인 로컬 저장 여부.
                sgSiteConfig.m_nZipPWBlock = 0;                       // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
                sgSiteConfig.m_bTitleDescSameChk = false;             // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
                sgSiteConfig.m_bApprLineChkBlock = false;              // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
                sgSiteConfig.m_bDlpInfoDisplay = false;                   // 전송/결재 관리 리스트에서 개인정보 검출 표시 유무 설정. ( true : 표시, false : 표시 안함 )
                sgSiteConfig.m_bApprDeptSearch = true;                // 결재자 검색 창의 타부서 수정 가능 여부.
                sgSiteConfig.m_nApprStepLimit = 0;                     // 결재자 Step 제한 설정. ( 0 : 무제한, 그외 양수 제한 Step )
                sgSiteConfig.m_bDeputyApprTerminateDel = false;           // 설정된 대결재자가 정보를 기한이 만료되면 삭제 할지 여부 ( true : 삭제, false : 삭제 안함)
                sgSiteConfig.m_bUserPWChange = false;                     // 사용자 패스워드 변경 사용 여부.
                sgSiteConfig.m_strPWChangeProhibitLimit = "";             // 패스워드 사용금지 문자열 지정.
                sgSiteConfig.m_nPWChangeApplyCnt = 9;                 // 패스워드 변경 시 허용되는 자리수 지정.
                sgSiteConfig.m_bURLListPolicyRecv = false;            // URL 리스트 정책 받기 사용 유무
                sgSiteConfig.m_strInitPasswd = "";

                SiteConfigInfo.Add(sgSiteConfig);
            }

            SetPWChangeApplyCnt(0, 9);
            SetInitPasswordInfo(0, "8xUHxpzSnsJgfVoSJthitg==");         // hsck@2301
            SetUseAutoLogin(0, false);
            SetUseApprLineLocalSave(0, true);
            SetUseLoginIDSave(0, true);                                 
        }
        public bool GetUseLoginIDSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUserIDSave;
            return false;
        }
        private void SetUseLoginIDSave(int groupID,bool bUserIDSave)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUserIDSave=bUserIDSave;
        }
        public bool GetUseAutoLogin(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bAutoLogin;
            return false;
        }
        private void SetUseAutoLogin(int groupID, bool bAutoLogin)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bAutoLogin = bAutoLogin;
        }

        public bool GetUseAutoLoginCheck(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bAutoLoginCheck;
            return false;
        }
        private void SetUseAutoLoginCheck(int groupID, bool bAutoLoginCheck)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bAutoLoginCheck = bAutoLoginCheck;
        }

        public bool GetUseApprLineLocalSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprLineLocalSave;
            return false;
        }
        private void SetUseApprLineLocalSave(int groupID, bool bApprLineLocalSave)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprLineLocalSave = bApprLineLocalSave;
        }
        public int GetZipPWBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nZipPWBlock;
            return 0;
        }
        private void SetZipPWBlock(int groupID,int nZipPWBlock)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nZipPWBlock = nZipPWBlock;
        }
        public bool GetUseApprLineChkBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprLineChkBlock;
            return false;
        }
        private void SetUseApprLineChkBlock(int groupID,bool bApprLineChkBlock)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprLineChkBlock = bApprLineChkBlock;
        }
        public bool GetUseDlpInfoDisplay(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bDlpInfoDisplay;
            return false;
        }
        private void SetUseDlpInfoDisplay(int groupID,bool bDlpInfoDisplay)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bDlpInfoDisplay = bDlpInfoDisplay;
        }

        public bool GetUseApprDeptSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprDeptSearch;
            return false;
        }
        private void SetUseApprDeptSearch(int groupID,bool bApprDeptSearch)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprDeptSearch = bApprDeptSearch;
        }
        public bool GetUseApprTreeSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bApprTreeSearch;
            return false;
        }
        private void SetUseApprTreeSearch(int groupID,bool bApprTreeSearch)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bApprTreeSearch= bApprTreeSearch;
        }
        public int GetApprStepLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nApprStepLimit;
            return 0;
        }
        private void SetApprStepLimit(int groupID, int nApprStepLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nApprStepLimit = nApprStepLimit;
        }
        public bool GetUseDeputyApprTerminateDel(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bDeputyApprTerminateDel;
            return false;
        }
        private void SetUseDeputyApprTerminateDel(int groupID, bool bDeputyApprTerminateDel)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bDeputyApprTerminateDel= bDeputyApprTerminateDel;
        }
        public bool GetUseUserPWChange(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bUserPWChange;
            return false;
        }
        private void SetUseUserPWChange(int groupID, bool bUserPWChange)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bUserPWChange= bUserPWChange;
        }
        public string GetPWChangeProhibitLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_strPWChangeProhibitLimit;
            return "";
        }
        private void SetPWChangeProhibitLimit(int groupID,string strPWChangeProhibitLimit)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_strPWChangeProhibitLimit = strPWChangeProhibitLimit;
        }

        public int GetPWChangeApplyCnt(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_nPWChangeApplyCnt;
            return 9;
        }
        private void SetPWChangeApplyCnt(int groupID, int nPWChangeApplyCnt)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_nPWChangeApplyCnt = nPWChangeApplyCnt;
        }
        public bool GetUseURLListPolicyRecv(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_bURLListPolicyRecv;
            return false;
        }
        private void SetUseURLListPolicyRecv(int groupID, bool bURLListPolicyRecv)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_bURLListPolicyRecv = bURLListPolicyRecv;
        }

        public string GetInitPasswordInfo(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].m_strInitPasswd;
            return "";
        }
        private void SetInitPasswordInfo(int groupID, string strInitPasswd)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                listSiteConfig[groupID].m_strInitPasswd = strInitPasswd;
        }

    }
}