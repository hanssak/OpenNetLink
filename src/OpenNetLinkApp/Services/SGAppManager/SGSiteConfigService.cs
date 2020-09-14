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
                sgSiteConfig.bUserIDSave = true;                    // 로그인한 ID 저장 여부
                sgSiteConfig.bAutoLogin = true;                     // 자동로그인 사용 여부.
                sgSiteConfig.bApprLineLocalSave = false;            // 결재라인 로컬 저장 여부.
                sgSiteConfig.nZipPWBlock = 0;                       // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
                sgSiteConfig.bTitleDescSameChk = false;             // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
                sgSiteConfig.bApprLineChkBlock = false;              // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
                sgSiteConfig.bDlpInfoDisplay = false;                   // 전송/결재 관리 리스트에서 개인정보 검출 표시 유무 설정. ( true : 표시, false : 표시 안함 )
                sgSiteConfig.bApprDeptSearch = true;                // 결재자 검색 창의 타부서 수정 가능 여부.
                sgSiteConfig.nApprStepLimit = 0;                     // 결재자 Step 제한 설정. ( 0 : 무제한, 그외 양수 제한 Step )
                sgSiteConfig.bDeputyApprTerminateDel = false;           // 설정된 대결재자가 정보를 기한이 만료되면 삭제 할지 여부 ( true : 삭제, false : 삭제 안함)
                sgSiteConfig.bUserPWChange = false;                     // 사용자 패스워드 변경 사용 여부.
                sgSiteConfig.strPWChangeProhibitLimit = "";             // 패스워드 사용금지 문자열 지정.
                sgSiteConfig.nPWChangeApplyCnt = 9;                 // 패스워드 변경 시 허용되는 자리수 지정.
                sgSiteConfig.bURLListPolicyRecv = false;            // URL 리스트 정책 받기 사용 유무

                SiteConfigInfo.Add(sgSiteConfig);
            }
        }
        public bool GetUseLoginIDSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUserIDSave;
            return false;
        }
        public bool GetUseAutoLogin(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bAutoLogin;
            return false;
        }
        public bool GetUseApprLineLocalSave(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bApprLineLocalSave;
            return false;
        }
        public int GetZipPWBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].nZipPWBlock;
            return 0;
        }
        public bool GetUseApprLineChkBlock(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bApprLineChkBlock;
            return false;
        }
        public bool GetUseDlpInfoDisplay(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bDlpInfoDisplay;
            return false;
        }

        public bool GetUseApprDeptSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bApprDeptSearch;
            return false;
        }
        public bool GetUseApprTreeSearch(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bApprTreeSearch;
            return false;
        }
        public int GetApprStepLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].nApprStepLimit;
            return 0;
        }
        public bool GetUseDeputyApprTerminateDel(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bDeputyApprTerminateDel;
            return false;
        }
        public bool GetUseUserPWChange(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bUserPWChange;
            return false;
        }
        public string GetPWChangeProhibitLimit(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].strPWChangeProhibitLimit;
            return "";
        }

        public int GetPWChangeApplyCnt(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].nPWChangeApplyCnt;
            return 9;
        }
        public bool GetUseURLListPolicyRecv(int groupID)
        {
            List<ISGSiteConfig> listSiteConfig = SiteConfigInfo;
            if (groupID < listSiteConfig.Count)
                return listSiteConfig[groupID].bURLListPolicyRecv;
            return false;
        }
    }
}