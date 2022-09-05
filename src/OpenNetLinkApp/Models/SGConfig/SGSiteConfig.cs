using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace OpenNetLinkApp.Models.SGConfig
{
    public class SGSiteConfig : ISGSiteConfig
    {
        public bool m_bUserIDSave { get; set; } = false;                    // 로그인한 ID 저장 여부
        public bool m_bAutoLogin { get; set; } = false;                     // 자동로그인 사용 여부.
        public bool m_bAutoLoginCheck { get; set; } = false;                // 자동로그인 체크박스 체크여부.
        public bool m_bApprLineLocalSave { get; set; } = false;             // 결재라인 로컬 저장 여부.
        public int m_nZipPWBlock { get; set; } = 0;                         // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
        public bool m_bTitleDescSameChk { get; set; } = false;              // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
        public bool m_bApprLineChkBlock { get; set; } = false;              // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        public bool m_bDlpInfoDisplay { get; set; } = false;                // 전송/결재 관리 리스트에서 개인정보 검출 표시 유무 설정. ( true : 표시, false : 표시 안함 )
        public bool m_bApprDeptSearch { get; set; } = true;                 // 결재자 검색 창의 타부서 수정 가능 여부.
        public bool m_bApprTreeSearch { get; set; } = false;                // 결재자 검색 부서트리 사용 유무.
        public int m_nApprStepLimit { get; set; } = 0;                      // 결재자 Step 제한 설정. ( 0 : 무제한, 그외 양수 제한 Step )
        public bool m_bDeputyApprTerminateDel { get; set; } = false;        // 설정된 대결재자가 정보를 기한이 만료되면 삭제 할지 여부 ( true : 삭제, false : 삭제 안함)
        public bool m_bUserPWChange { get; set; } = false;                  // 사용자 패스워드 변경 사용 여부.
        public string m_strPWChangeProhibitLimit { get; set; } = "";        // 패스워드 사용금지 문자열 지정.
        public int m_nPWChangeApplyCnt { get; set; } = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
        public bool m_bURLListPolicyRecv { get; set; } = false;             // URL 리스트 정책 받기 사용 유무,
        public string m_strInitPasswd { get; set; } = "";                   // 초기 패스워드 정보.
        public bool m_bUseScreenLock { get; set; } = true;                  // 화면잠금 사용 여부
        public bool m_bUseClipBoard { get; set; } = true;                   // 클립보드 사용 여부
        public bool m_bUseURLRedirection { get; set; } = true;              // URL 리다이렉션 사용 여부
        public bool m_bUseFileSend { get; set; } = true;                    // 파일 전송 사용 여부
        public bool m_bRecvFolderChange { get; set; } = true;               // 수신 폴더 변경 사용 여부.
        public bool m_bUseUserRecvDownPath { get; set; } = false;           // 로그인 유저별 다운로드 경로 사용 여부
        public bool m_bUseEmail { get; set; } = false;               // 메일 관리/결재 사용 유무.
        public bool m_bUsePCURL { get; set; } = false;                      // PCURL 사용 유무.
        public bool m_bUseClipApprove { get; set; } = false;                // 클립보드 결재 사용 유무
        public bool m_bUsePublicBoard { get; set; } = false;                // 공지사항 사용 유무.
        public bool m_bUseCertSend { get; set; } = false;                   // 인증서 전송 사용 유무.
        //public bool m_bUseOSMaxFilePath { get; set; } = true;               // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 

        public bool m_bUseDenyPasswordZip { get; set; } = false;         // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool m_bFileForward { get; set; } = false;               // 파일포워드기능 사용할지 유무
        public bool m_bUseClipBoardFileTrans { get; set; } = false;         // 파일형태로보내는 클립보드 사용 유무
        public bool m_bUseFileClipManageUI { get; set; } = false;         // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool m_bUseFileClipApproveUI { get; set; } = false;          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무

        public bool m_bUseClipTypeSelectSend { get; set; } = true;         // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무

        public bool m_bUseClipTypeTextFirstSend { get; set; } = false;         // 클립보드를 보낼때, Text 및 image Mixed 상태일때 Text를 우선적으로 보내도록 설정

        public bool m_bUseApproveAfterLimit { get; set; } = true;             // 파일전송시 사후결재 Count 제한 사용유무

        public bool m_bUseClipBoardApproveAfterLimit { get; set; } = true;       // 클립보드 파일전송시 사후결재 Count 제한 사용유무


    }
}
