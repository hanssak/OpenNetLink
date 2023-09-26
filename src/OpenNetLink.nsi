; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "OpenNetLink"
!define PRODUCT_MAIN_FILE_NAME "OpenNetLinkApp.exe"
!define PRODUCT_KILL_FILE_NAME "SGClean.exe"


; !define PRODUCT_VERSION "1.0.0"
!define PRODUCT_PUBLISHER "Hanssak, Inc."
!define PRODUCT_WEB_SITE "http://www.hanssak.co.kr"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\ContextTransferClient.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "x64.nsh"
!include "FileFunc.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "Licence.txt"
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
;;!define MUI_FINISHPAGE_RUN "$INSTDIR\${PRODUCT_MAIN_FILE_NAME}"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "Korean"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
; OutFile "OpenNetLinkSetup.exe"
;OutFile ".\artifacts\installer\windows\packages\OpenNetLinkSetup_v${PRODUCT_VERSION}.exe"

!if ${IS_PATCH} == "TRUE"
  OutFile "${OUTPUT_DIRECTORY}\OpenNetLink-Windows-${PRODUCT_VERSION}.exe"
!else

    !if ${IS_SILENT} == "TRUE"
      OutFile "${OUTPUT_DIRECTORY}\[${CUSTOM_NAME}] OpenNetLink_${STORAGE_NAME}_${NETWORK_FLAG}_Windows_Silent_${PRODUCT_VERSION}.exe"
    !else
      OutFile "${OUTPUT_DIRECTORY}\[${CUSTOM_NAME}] OpenNetLink_${STORAGE_NAME}_${NETWORK_FLAG}_Windows_${PRODUCT_VERSION}.exe"
    !endif

!endif

InstallDir "C:\HANSSAK\OpenNetLink"
!define INSTALLPATH "C:\HANSSAK\OpenNetLink"

InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

; 설치파일정보
VIProductVersion	"${PRODUCT_VERSION}"							; 파일버전
VIAddVersionKey		FileVersion		"${PRODUCT_VERSION}"				; 파일버전
VIAddVersionKey		FileDescription	"PC Data Transmission System"		; 파일설명
VIAddVersionKey		ProductName		"SecureGate"						; 제품이름
VIAddVersionKey		ProductVersion	"3.5.1.0"							; 제품버전
VIAddVersionKey		LegalCopyright	"Hanssaksystem Co., Ltd."			; 저작권
VIAddVersionKey		CompanyName		"Hanssaksystem Co., Ltd."			; 회사명
VIAddVersionKey		Comments		"http://hanssak.co.kr"

; Global Variable
Var /GLOBAL g_AddFileRM
Var /GLOBAL g_AddFileRM0
Var /GLOBAL g_AddFileRM1
Var /GLOBAL g_AddFileRM2

Var /GLOBAL g_bAddFileRMFind  
Var /GLOBAL g_iAddFileRMCount
Var /GLOBAL g_strAddFileRMCompareStr
Var /GLOBAL g_strAddFileRM0CompareStr
Var /GLOBAL g_strAddFileRM1CompareStr
Var /GLOBAL g_strAddFileRM2CompareStr
Var /GLOBAL g_iCount
Var /GLOBAL g_UpdateFilePath
Var /Global g_UpdateResult
Var /Global g_UpdateReason

Var /GLOBAL g_strNetPos			; 3망중에 다중망(중간망)인지 여부 확인(NETPOS 값 IN일때:1, CN일때:2, EX일때: 3, NCI일때:4, 없으면 0)
Var /GLOBAL g_iNetPos			; 3망중에 다중망(중간망)인지 여부 ("IN": 중요단말, "CN": 중간망-다중망(업무망), "NCI": 중간망-인터넷망(업무망), "EX" : 인터넷망), IN(1) / CN(2) / OUT(3) / NCI(4) / NotFound(0)
Var /GLOBAL g_iPatchEdge	        ; edge(wwwroot\edge)-patch진행여부
Var /GLOBAL g_UseStartProgram	        ; Booting시에 agent 자동시작 되게할 지 여부


Var /GLOBAL out_day 		; "01"      day
Var /GLOBAL out_month		; "04"      month
Var /GLOBAL out_year		; "2005"    year
Var /GLOBAL out_dayName	; "Friday"  day of week name	
Var /GLOBAL out_hour		; "16"      hour
Var /GLOBAL out_min		; "05"      minute
Var /GLOBAL out_sec		; "50"      seconds

; ---------------------------- StrContains 함수(Start) -----------------------------------
!macro _StrContains un
	Function ${un}_StrContains
		Push $R0 # 찾을 문자열
		Exch
		Pop $R0
		Push $R1 # 원본 문자열
		Exch 2
		Pop $R1
		Push $R2 # 리턴 문자열
		Push $R3
		Push $R4
		Push $R5
		Push $R6
		;MessageBox MB_OK "찾을 문자열[$R0], 원본 문자열[$R1]"
		StrCpy $R2 ""
		StrCpy $R3 -1
		StrLen $R4 $R0
		StrLen $R6 $R1
		loop:
			IntOp $R3 $R3 + 1
			StrCpy $R5 $R1 $R4 $R3
			StrCmp $R5 $R0 found
			StrCmp $R3 $R6 done
			Goto loop
		found:
			StrCpy $R2 $R0
			Goto done
		done:
		Pop $R6
		Pop $R5
		Pop $R4
		Pop $R3
		Exch $R2
		Exch
		Pop $R0
		Exch
		Pop $R1
	FunctionEnd
!macroend
!insertmacro _StrContains ""
!insertmacro _StrContains "un."
!macro StrContains OUTPUT str_find str_origin
	Push "${str_origin}"
	Push "${str_find}"
	!ifndef __UNINSTALL__
		Call _StrContains
	!else
		Call un._StrContains
	!endif
	Pop "${OUTPUT}"
!macroend
!define StrContains "!insertmacro StrContains"
; ---------------------------- StrContains 함수(End) -----------------------------------

; ---------------------------- trim 함수(Start) -----------------------------------

 
!macro Trim ResultVar String
  Push "${String}"
  Call Trim
  Pop "${ResultVar}"
!macroend

Function Trim
	Exch $R1 ; Original string
	Push $R2
 
	Loop:
		StrCpy $R2 "$R1" 1
		StrCmp "$R2" " " TrimLeft
		StrCmp "$R2" "$\r" TrimLeft
		StrCmp "$R2" "$\n" TrimLeft
		StrCmp "$R2" "$\t" TrimLeft
		GoTo Loop2
	TrimLeft:	
		StrCpy $R1 "$R1" "" 1
		Goto Loop
	 
	Loop2:
		StrCpy $R2 "$R1" 1 -1
		StrCmp "$R2" " " TrimRight
		StrCmp "$R2" "$\r" TrimRight
		StrCmp "$R2" "$\n" TrimRight
		StrCmp "$R2" "$\t" TrimRight
		GoTo Done
	TrimRight:	
		StrCpy $R1 "$R1" -1
		Goto Loop2
	 
	Done:
		Pop $R2
		Exch $R1
FunctionEnd
!define Trim "!insertmacro Trim"
; ---------------------------- trim 함수(End) -----------------------------------

; ---------------------------- SetPatchLog 함수(Start) -----------------------------------
!macro SetPatchLog ResultVar Reason
  StrCpy $g_UpdateResult "${ResultVar}"
  StrCpy $g_UpdateReason "${Reason}"
  
  Call SetPatchLog
!macroend

Function SetPatchLog 
	;패치 결과 포맷 : SUCCESS(FAIL)/2023.08.23 13:50:59/reason
	${GetTime} "" "L" $out_day $out_month $out_year $out_dayName $out_hour $out_min $out_sec
	
	; FileOpen $9 "C:\HANSSAK\OpenNetLink\wwwroot\conf\UpdateResult.txt" a ;Append File and fills it
	; FileSeek $9 0 END
	FileOpen $9 "C:\HANSSAK\OpenNetLink\wwwroot\conf\UpdateResult.txt" w ;Append File and fills it
	
	${If} $g_UpdateResult == "TRUE"
		FileWrite $9 "SUCCESS/$out_year.$out_month.$out_day $out_hour:$out_min:$out_sec $\r$\n"
	${Else}
		FileWrite $9 "FAIL/$out_year.$out_month.$out_day $out_hour:$out_min:$out_sec/$g_UpdateReason $\r$\n"
	${EndIf}
	FileClose $9 ;Closes the filled file
FunctionEnd
!define SetPatchLog "!insertmacro SetPatchLog"
; ---------------------------- SetPatchLog 함수(End) -----------------------------------

; Patch Mode 일때에만 사용
!macro FUNC_GETNETPOS UN

    StrCpy $g_strNetPos ""

    ClearErrors
    FileOpen $0 "${INSTALLPATH}\wwwroot\conf\NetWork.json" r
    IfErrors exit_loop
    LOOP:
    
    ClearErrors
    FileRead $0 $1		; Line 단위로 읽음
    StrCpy $g_strNetPos $1
    
    ; 확인용도
    ;MessageBox MB_ICONINFORMATION|MB_OK $g_strNetPos
    IfErrors notfoundNETPOS
    
    ${StrContains} $1 "NETPOS" $g_strNetPos    
    StrCmp $1 "" LOOP
    
    ; 확인용도
    ;MessageBox MB_ICONINFORMATION|MB_OK "'NETPOS' 발견함!"    

    ${StrContains} $1 "CN" $g_strNetPos 
    StrCmp $1 "" notfoundCn
    StrCpy $g_iNetPos 2 
    
    ; 확인용도    
    ;CreateDirectory "${INSTALLPATH}\22222"
    ;MessageBox MB_ICONINFORMATION|MB_OK "#CN# 발견함!"    
    
    Goto exit_loop    

notfoundCn:

    ${StrContains} $1 "NCI" $g_strNetPos
    StrCmp $1 "" notfoundNCI
    StrCpy $g_iNetPos 4

    ; 확인용도
    ;CreateDirectory "${INSTALLPATH}\41111"
    ;MessageBox MB_ICONINFORMATION|MB_OK "#IN# 발견함!"
    Goto exit_loop


notfoundNCI:

    ${StrContains} $1 "IN" $g_strNetPos 
    StrCmp $1 "" notfoundIN
    StrCpy $g_iNetPos 1    
    
    ; 확인용도    
    ;CreateDirectory "${INSTALLPATH}\11111"
    ;MessageBox MB_ICONINFORMATION|MB_OK "#IN# 발견함!"    
    Goto exit_loop    

notfoundIN:

    StrCpy $g_iNetPos 3  ; EX(3) 로 판단
    ;CreateDirectory "${INSTALLPATH}\33333"
    ;MessageBox MB_ICONINFORMATION|MB_OK "#EX# (CN/IN 발견못함)!"

notfoundNETPOS:
    StrCpy $g_iNetPos 0
    Goto exit_loop
    
exit_loop:
    FileClose $0    
    

        
!macroend ; end the FUNC_GETNETPOS


!macro FUNC_SETCONFIG UN

	; siteConfig - 설정


	; 기존 설치 파일 덮어쓰기 방식 (0), 기존 설치 파일 uninstall 후 설치 (1)
	; StrCpy $g_InstallOp 0

	; 시작프로그램에 등록	미사용(0), 사용(1) - 작업해야함
	StrCpy $g_UseStartProgram 0

    ; 함께 배포된 edge 삭제후 Patch 할지 여부  미사용(0), 사용(1)
	; Edge가 없는 Light 배포파일인 경우엔, Edge 삭제처리 안되도록 조건 추가
	${If} ${IS_LIGHT_PATCH} != 'TRUE'		
		StrCpy $g_iPatchEdge 1
	${endif}
    
	

	; 망위치 강제 지정 - IN(1) / CN(2) / NCI(4)/ OUT(3) / NotFound(0)
	StrCpy $g_iNetPos 0
        ; 단일망에서 우클릭모듈 문구 다르게 나오길 원한다면, build해서 Library 폴더에 따로 두고 Build하는걸 2번 해줘야함

!macroend ; end the FUNC_SETCONFIG

!macro FUNC_REMOVE_ADD_FILE_RM_DLL UN
; COM Rename	

        ; 중간망인지 판단해서, AddFileRMX64.dll / AddFileRMex0X64.dll / AddFileRMex1X64.dll 중에 어느걸 보낼 건지를 확인하는 동작
        
	Delete "${INSTALLPATH}\AddFileRMX64.dll"
        Delete "${INSTALLPATH}\AddFileRMex0X64.dll"
        Delete "${INSTALLPATH}\AddFileRMex1X64.dll"
        Delete "${INSTALLPATH}\AddFileRMex2X64.dll"
        Delete "${INSTALLPATH}\AddFileRM.dll"

	${If} ${RunningX64}
	        StrCpy $g_AddFileRM 'AddFileRMX64.dll'
	        StrCpy $g_AddFileRM0 'AddFileRMex0X64.dll'
	        StrCpy $g_AddFileRM1 'AddFileRMex1X64.dll'
	        StrCpy $g_AddFileRM2 'AddFileRMex2X64.dll'
  	${Else}        
        	StrCpy $g_AddFileRM 'AddFileRM.dll'
  	${EndIf}
	

	; 단일망 - 'AddFileRMX64.dll' 사용
	StrCpy $g_bAddFileRMFind 0
	StrCpy $g_iAddFileRMCount 1
	StrCpy $g_iCount 1
	${While} $g_bAddFileRMFind < 1
		StrCpy $g_strAddFileRMCompareStr $g_AddFileRM$g_iCount        
	
		IfFileExists ${INSTALLPATH}\$g_strAddFileRMCompareStr Findg_AddFileRM NotFindg_AddFileRM
		Findg_AddFileRM:         
			IntOp $g_iCount $g_iCount + 1         
			goto ENDg_AddFileRM
		NotFindg_AddFileRM:
			Rename ${INSTALLPATH}\$g_AddFileRM ${INSTALLPATH}\$g_strAddFileRMCompareStr
			StrCpy $g_bAddFileRMFind 1         
		ENDg_AddFileRM:
	${EndWhile}

	; 다중망 - 'AddFileRMex0X64.dll' 사용	
        StrCpy $g_bAddFileRMFind 0
	StrCpy $g_iAddFileRMCount 1
	StrCpy $g_iCount 1
	${While} $g_bAddFileRMFind < 1
		StrCpy $g_strAddFileRM0CompareStr $g_AddFileRM0$g_iCount     
	
		IfFileExists ${INSTALLPATH}\$g_strAddFileRM0CompareStr Findg_AddFileRM0 NotFindg_AddFileRM0
		Findg_AddFileRM0:         
			IntOp $g_iCount $g_iCount + 1         
			goto ENDg_AddFileRM0
		NotFindg_AddFileRM0:
			Rename ${INSTALLPATH}\$g_AddFileRM0 ${INSTALLPATH}\$g_strAddFileRM0CompareStr
			StrCpy $g_bAddFileRMFind 1         
		ENDg_AddFileRM0:
	${EndWhile}

	; 다중망 - 'AddFileRMex1X64.dll' 사용	
        StrCpy $g_bAddFileRMFind 0
	StrCpy $g_iAddFileRMCount 1
	StrCpy $g_iCount 1
	${While} $g_bAddFileRMFind < 1
		StrCpy $g_strAddFileRM1CompareStr $g_AddFileRM1$g_iCount     
	
		IfFileExists ${INSTALLPATH}\$g_strAddFileRM1CompareStr Findg_AddFileRM1 NotFindg_AddFileRM1
		Findg_AddFileRM1:         
			IntOp $g_iCount $g_iCount + 1         
			goto ENDg_AddFileRM1
		NotFindg_AddFileRM1:
			Rename ${INSTALLPATH}\$g_AddFileRM1 ${INSTALLPATH}\$g_strAddFileRM1CompareStr
			StrCpy $g_bAddFileRMFind 1         
		ENDg_AddFileRM1:
	${EndWhile}	

	; 다중망 - 'AddFileRMex2X64.dll' 사용
        StrCpy $g_bAddFileRMFind 0
	StrCpy $g_iAddFileRMCount 1
	StrCpy $g_iCount 1
	${While} $g_bAddFileRMFind < 1
		StrCpy $g_strAddFileRM2CompareStr $g_AddFileRM2$g_iCount

		IfFileExists ${INSTALLPATH}\$g_strAddFileRM2CompareStr Findg_AddFileRM2 NotFindg_AddFileRM2
		Findg_AddFileRM2:
			IntOp $g_iCount $g_iCount + 1
			goto ENDg_AddFileRM2
		NotFindg_AddFileRM2:
			Rename ${INSTALLPATH}\$g_AddFileRM2 ${INSTALLPATH}\$g_strAddFileRM2CompareStr
			StrCpy $g_bAddFileRMFind 1
		ENDg_AddFileRM2:
	${EndWhile}
	
!macroend ; end the FUNC_REMOVE_ADD_FILE_RM_DLL



Function GetNetPositionByFile
	!insertmacro FUNC_GETNETPOS ""
FunctionEnd	; end the GetNetPositionByFile

Function un.GetNetPositionByFile
	!insertmacro FUNC_GETNETPOS "un."
FunctionEnd	; end the un.GetNetPositionByFile


Function SetConfig
	!insertmacro FUNC_SETCONFIG ""
FunctionEnd	; end the SetConfig

Function un.SetConfig
	!insertmacro FUNC_SETCONFIG "un."
FunctionEnd	; end the un.SetConfig

Function ReMoveAddFileRM		
 	!insertmacro FUNC_REMOVE_ADD_FILE_RM_DLL ""
FunctionEnd ; end the ReMoveAddFileRM

Function un.ReMoveAddFileRM		
 	!insertmacro FUNC_REMOVE_ADD_FILE_RM_DLL "un."
FunctionEnd ; end the un.ReMoveAddFileRM


Function .onInit
	${If} ${IS_SILENT} == 'TRUE'		
		SetSilent silent
	${endif}
	
	;OpenNetLink 강제종료
	nsExec::Exec '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'
  
	${If} ${IS_PATCH} == 'TRUE'
		${If} ${UPDATECHECK} == 'TRUE'
			;롤백 기능을 위해 폴더 통째로 이동 처리
			${If} ${FileExists} "$TEMP\OpenNetLink\*.*"
				RMDir /r "$TEMP\OpenNetLink\"
			${EndIf}				
			
			CopyFiles "C:\HANSSAK\OpenNetLink\*.*" "$TEMP\OpenNetLink"
			
			RMDir /r "C:\HANSSAK\OpenNetLink\"	;설치 전 기존 파일들 삭제		
			
			;삭제 후 남아있는 파일들은 백업
			FindFirst $2 $3 "C:\HANSSAK\OpenNetLink\*"
			loop:
				StrCmp $3 "" done			
				${If} ${FileExists} "$TEMP\OpenNetLink_Patch"		
					Delete "$TEMP\OpenNetLink_Patch\*.*"
				${Else}				
					CreateDirectory "$TEMP\OpenNetLink_Patch"
				${EndIf}							
				Rename "C:\HANSSAK\OpenNetLink\$3" "$TEMP\OpenNetLink_Patch\$3" ;다른 프로세스로 인해 삭제가 불가한 파일은 이동처리
				FindNext $2 $3
				Goto loop
			done:
			FindClose $2			
		${Else}        
			CopyFiles /SILENT /FILESONLY "C:\HANSSAK\OpenNetLink\wwwroot\conf\NetWork.json" "$TEMP" 
			CopyFiles /SILENT /FILESONLY "C:\HANSSAK\OpenNetLink\wwwroot\conf\AppEnvSetting.json" "$TEMP" 
			CopyFiles /FILESONLY "C:\HANSSAK\OpenNetLink\wwwroot\db\SGNotifyDB.db" "$TEMP" 
			CopyFiles /FILESONLY "C:\HANSSAK\OpenNetLink\wwwroot\db\SGSettingsDB.db" "$TEMP"	
			CopyFiles /FILESONLY "C:\HANSSAK\OpenNetLink\wwwroot\conf\hsck" "$TEMP"	
		${EndIf}
	
		Banner::show "Calculating important stuff..."
		Banner::getWindow
		Pop $1

		again:
		  IntOp $0 $0 + 1
		  Sleep 1
		  StrCmp $0 100 0 again

		GetDlgItem $2 $1 1030
		SendMessage $2 ${WM_SETTEXT} 0 "STR:Calculating more important stuff..."

		again2:
		  IntOp $0 $0 + 1
		  Sleep 1
		  StrCmp $0 200 0 again2
		Banner::destroy
  ${EndIf}
  
  ; OpenNetLink 강제종료
  ;ExecWait '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'
  nsExec::Exec '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'
	
FunctionEnd

Function .onInstSuccess
    ;옵션에 따라 먼저 NetLink 삭제하기
	${If} ${DELETE_NETLINK} == 'TRUE'		
		ExecWait '"$SYSDIR\taskkill.exe" /f /im SecureGate.exe'	
		
		;HideWindow
		${If} ${FileExists} "C:\HANSSAK\SecureGate\SecureGate.exe"
			;NetLink를 Silence로 삭제할 수 있는 Uninstall 적용
			Delete "C:\HANSSAK\SecureGate\uninstall.exe"			
			CopyFiles /FILESONLY "$INSTDIR\Library\NetLink.Uninstall\uninstall.exe" "C:\HANSSAK\SecureGate" 			
			ExecWait '"C:\HANSSAK\SecureGate\uninstall.exe'			
		${endif}	
	${endif}  
  
	${If} ${IS_PATCH} == 'TRUE'
		${If} ${UPDATECHECK} == 'TRUE'
			CopyFiles /SILENT /FILESONLY "$TEMP\OpenNetLink\wwwroot\conf\NetWork.json" "C:\HANSSAK\OpenNetLink\wwwroot\conf" 
			CopyFiles /SILENT /FILESONLY "$TEMP\OpenNetLink\wwwroot\conf\AppEnvSetting.json" "C:\HANSSAK\OpenNetLink\wwwroot\conf"
			CopyFiles /FILESONLY "$TEMP\OpenNetLink\wwwroot\db\SGNotifyDB.db" "C:\HANSSAK\OpenNetLink\wwwroot\db"
			CopyFiles /FILESONLY "$TEMP\OpenNetLink\wwwroot\db\SGSettingsDB.db" "C:\HANSSAK\OpenNetLink\wwwroot\db"  	
			CopyFiles /FILESONLY "$TEMP\OpenNetLink\wwwroot\conf\UpdateResult.txt" "C:\HANSSAK\OpenNetLink\wwwroot\conf\UpdateResult.txt"  	
			CopyFiles /FILESONLY "$TEMP\OpenNetLink\wwwroot\conf\hsck" "C:\HANSSAK\OpenNetLink\wwwroot\conf\hsck"  	
			Rename "$TEMP\OpenNetLink\wwwroot\Log" "C:\HANSSAK\OpenNetLink\wwwroot\Log"  ; 로그 이동
			
			${If} ${IS_LIGHT_PATCH} == 'TRUE'
				Rename "$TEMP\OpenNetLink\wwwroot\edge" "C:\HANSSAK\OpenNetLink\wwwroot\edge"  ; edge 이동
			${EndIf}
			
			${SetPatchLog} "TRUE" ""		
		${Else}
			; 하위 exist 작업을 위해, 패치본의 json/db는 삭제	                                                       		
			CopyFiles /SILENT /FILESONLY "$TEMP\NetWork.json" "C:\HANSSAK\OpenNetLink\wwwroot\conf" 
			CopyFiles /SILENT /FILESONLY "$TEMP\AppEnvSetting.json" "C:\HANSSAK\OpenNetLink\wwwroot\conf"
			CopyFiles /FILESONLY "$TEMP\SGNotifyDB.db" "C:\HANSSAK\OpenNetLink\wwwroot\db"
			CopyFiles /FILESONLY "$TEMP\SGSettingsDB.db" "C:\HANSSAK\OpenNetLink\wwwroot\db"  
			CopyFiles /FILESONLY "$TEMP\hsck" "C:\HANSSAK\OpenNetLink\wwwroot\conf" 
			
			;백업된 json/db 가 복사 완료될때까지, 대기
			StrCpy $R0 0
			loop:
			IntCmp $R0 1 endloop
				${If} ${FileExists} "C:\HANSSAK\OpenNetLink\wwwroot\conf\NetWork.json"
					${If} ${FileExists} "C:\HANSSAK\OpenNetLink\wwwroot\conf\AppEnvSetting.json"
						${If} ${FileExists} "C:\HANSSAK\OpenNetLink\wwwroot\db\SGNotifyDB.db"
							${If} ${FileExists} "C:\HANSSAK\OpenNetLink\wwwroot\db\SGSettingsDB.db"
								IntOp $R0 $R0 + 1
								sleep 2000
								Goto loop
							${endif}			
						${endif}
					${endif}
				${endif}
			sleep 1000
			Goto loop
			endloop:
		${endif}
	${endif}
  
	;(위치이동) 설치 성공시에 프로그램 정보 등록
	WriteUninstaller "$INSTDIR\uninst.exe"
	WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\ContextTransferClient.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\OpenNetLinkApp.exe"

	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
	WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"

	;;IfSilent 0 +2
	${If} ${STARTAUTO} == 'TRUE'
	Exec '"$INSTDIR\OpenNetLinkApp.exe"'
	${endif}
  
FunctionEnd

Section "MainSection" SEC01
	SetOutPath "$INSTDIR"
	SetOverwrite on  
	; OpenNetLink 강제종료
	;ExecWait '"$SYSDIR\taskkill.exe" /f /im ContextTransferClient.exe'
	;ExecWait '"$SYSDIR\taskkill.exe" /f /im PreviewUtil.exe'
	;ExecWait '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'

	; Kill 안먹힘
	;ExecShell "open" '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe' SW_HIDE

	;설치 파일 설정을 로드한다
	Call SetConfig
	Call ReMoveAddFileRM

	${If} ${IS_PATCH} == 'TRUE'
		Call GetNetPositionByFile  
		${If} $g_iPatchEdge == 1
			RMDir /r "$INSTDIR\wwwroot\edge\" 
		${EndIf}    
	${Else}
		; 재배포 Package는 설치때만
		File "Appcasts\preinstall\windows\VC_redist.x64.exe"
		File "Appcasts\preinstall\windows\VC_redist.x86.exe"

		${If} ${RunningX64}
			ExecWait '"$INSTDIR\VC_redist.x64.exe" /q /norestart'
			;ExecWait 'vcredist_x64.exe'
		${Else}
			ExecWait '"$INSTDIR\VC_redist.x86.exe" /q /norestart'
			  ;ExecWait 'vcredist_x86.exe'
		${EndIf}
	${EndIf}

	; 한꺼번에 지정하는 방식 사용
	File /r "artifacts\windows\published\"

	SetOutPath "$INSTDIR"
	File "bin_addon\SecureGateChromiumExtension_v1.1.crx"

	${If} ${IS_PATCH} == 'TRUE'
		${If} $g_iNetPos == 2 ;CN
		  ;File "artifacts\windows\published\AddFileRMex0X64.dll"
		  ;File "artifacts\windows\published\AddFileRMex1X64.dll"
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex0X64.dll"'
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex1X64.dll"'
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex2X64.dll"'
		${ElseIf}  $g_iNetPos == 4 ;NCI
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex0X64.dll"'
		${Else}

		  ${If} $g_iNetPos == 1	; IN
			ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMX64.dll"'
		  ${Else}
			ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMX64.dll"'
		  ${EndIf}		  
		${EndIf}
		
		${If} ${UPDATECHECK} == 'TRUE'
			;패치파일 내 들어있는 UpdateFilelist.txt가 있다면, 목록과 패치 파일 비교
			${If} ${FileExists} "C:\HANSSAK\OpenNetLink\wwwroot\conf\temp\UpdateFileList.txt"
				FileOpen $0 "C:\HANSSAK\OpenNetLink\wwwroot\conf\temp\UpdateFileList.txt" r
			
				LOOP_CHECK: 
				IfErrors exit_loop
				FileRead $0 $1
				
				${Trim} $g_UpdateFilePath $1
				StrCmp $g_UpdateFilePath "" exit_loop				
				IfFileExists ${INSTALLPATH}\$g_UpdateFilePath FileFind FileNotFind	;패치파일 중 존재하는지 확인
					FileFind:
						Goto LOOP_CHECK
					FileNotFind:
						FileClose $0						
						MessageBox MB_OK|MB_TOPMOST "Patch failed.$\r$\nFile['$g_UpdateFilePath'] is missing.$\r$\nRoll back the program."
						RMDir /r "C:\HANSSAK\OpenNetLink\"	;설치하던것 삭제
						
						;삭제 후 남아있는 파일들은 백업
						FindFirst $2 $3 "C:\HANSSAK\OpenNetLink\*"
						loop:
							StrCmp $3 "" done
							
							${If} ${FileExists} "$TEMP\OpenNetLink_Patch"
								Delete "$TEMP\OpenNetLink_Patch\*.*"
							${Else}				
								CreateDirectory "$TEMP\OpenNetLink_Patch" ; 확인용
							${EndIf}							
							Rename "C:\HANSSAK\OpenNetLink\$3" "$TEMP\OpenNetLink_Patch\$3" ;다른 프로세스로 인해 삭제가 불가한 파일은 이동처리
							
							FindNext $2 $3
							Goto loop
						done:
						FindClose $2				
						CopyFiles "$TEMP\OpenNetLink\*.*" "C:\HANSSAK\OpenNetLink";기존 설치본 원복
						${SetPatchLog} "FALSE" "File['$g_UpdateFilePath'] is missing."						
						Quit				
				exit_loop:
				FindClose $0								
				Delete $0
			${endif}
		${endif}
	${Else}	;${IS_PATCH} == 'FALSE'

	  ${If} ${NETWORK_FLAG} == 'CN'
		  ;CreateDirectory "${INSTALLPATH}\22222" ; 확인용
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex0X64.dll"'
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex1X64.dll"'
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex2X64.dll"'
	  ${ElseIf}  ${NETWORK_FLAG} == 'NCI'
		  ;CreateDirectory "${INSTALLPATH}\44444" ; 확인용
		  ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMex0X64.dll"'
	  ${Else}	  	
		  ${If} ${NETWORK_FLAG} == 'IN'
				;CreateDirectory "${INSTALLPATH}\11111" ; 확인용
			ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMX64.dll"'
		  ${Else}
				;CreateDirectory "${INSTALLPATH}\33333" ; 확인용
			ExecWait '"$SYSDIR\regsvr32.exe" /s "$INSTDIR\AddFileRMX64.dll"'
		  ${EndIf}
	  ${EndIf}

	${EndIf} ; ${IS_PATCH} == 'TRUE'

  
  ; 단축아이콘 생성
  CreateDirectory "$SMPROGRAMS\OpenNetLink"
  CreateShortCut "$SMPROGRAMS\OpenNetLink\OpenNetLink.lnk" "$INSTDIR\OpenNetLinkApp.exe"
  CreateShortCut "C:\Users\Public\Desktop\OpenNetLink.lnk" "$INSTDIR\OpenNetLinkApp.exe"  
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\OpenNetLink\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\OpenNetLink\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd


Section -Post
  ;설치 성공시에 프로그램 정보 등록하도록 이동
  ; WriteUninstaller "$INSTDIR\uninst.exe"
  ; WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\ContextTransferClient.exe"
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\OpenNetLinkApp.exe"
  
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  ; WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"


  ${If} ${REG_CRX} == "TRUE"
  
	  ; CRX 파일 - 강제등록
	  ${If} ${NETWORK_FLAG} == "IN"
	  
		  ; edge
		  WriteRegStr HKLM "SOFTWARE\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://edge.microsoft.com/extensionwebstorebase/v1/crx"
		  ;WriteRegStr HKLM "SOFTWARE\WOW6432Node\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://edge.microsoft.com/extensionwebstorebase/v1/crx"
		  ;WriteRegStr HKCU "Software\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://edge.microsoft.com/extensionwebstorebase/v1/crx"
		  
		  ; chrome
		  WriteRegStr HKLM "SOFTWARE\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://clients2.google.com/service/update2/crx"
		  ;WriteRegStr HKLM "SOFTWARE\WOW6432Node\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://clients2.google.com/service/update2/crx"
		  ;WriteRegStr HKCU "Software\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam" "update_url" "https://clients2.google.com/service/update2/crx"	  
		  
	  ${EndIf}  

  ${EndIf}  

SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) has been successfully deleted."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to delete $(^Name)?" IDYES +2
  Abort
FunctionEnd

Section Uninstall

  /*
  ; 실행중인 OpenNetLink 종료
  ExecWait '"$INSTDIR\${PRODUCT_KILL_FILE_NAME}"
  Sleep 1000
  ExecWait '"$INSTDIR\${PRODUCT_KILL_FILE_NAME}"
  Sleep 1000
  ;unicode 동작못함 - 수정필요
  ;KillProcDLL::KillProc "${PRODUCT_MAIN_FILE_NAME}"
  */

  ; OpenNetLink 강제종료
  ; ExecWait '"$SYSDIR\taskkill.exe" /f /im ContextTransferClient.exe'
  ;ExecWait '"$SYSDIR\taskkill.exe" /f /im PreviewUtil.exe'
  ;ExecWait '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'
  nsExec::Exec '"$SYSDIR\taskkill.exe" /f /im PreviewUtil.exe'
  nsExec::Exec '"$SYSDIR\taskkill.exe" /f /im OpenNetLinkApp.exe'
  ;설치 파일 설정을 로드한다
  Call un.SetConfig
  
  ; IN / CN / EX 알아야할때만 사용
  ;Call un.GetNetPositionByFile

  ExecWait '"$SYSDIR\regsvr32.exe" /u /s "$INSTDIR\AddFileRMX64.dll"'
  ExecWait '"$SYSDIR\regsvr32.exe" /u /s "$INSTDIR\AddFileRMex0X64.dll"'
  ExecWait '"$SYSDIR\regsvr32.exe" /u /s "$INSTDIR\AddFileRMex1X64.dll"'
  ExecWait '"$SYSDIR\regsvr32.exe" /u /s "$INSTDIR\AddFileRMex2X64.dll"'

  Delete "$SMPROGRAMS\OpenNetLink\Uninstall.lnk"
  Delete "$SMPROGRAMS\OpenNetLink\Website.lnk"
  Delete "C:\Users\Public\Desktop\OpenNetLink.lnk"
  Delete "$SMPROGRAMS\OpenNetLink\OpenNetLink.lnk"

  RMDir "$SMPROGRAMS\OpenNetLink"
  RMDir /r "$INSTDIR"

  Call un.ReMoveAddFileRM

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"

	${If} ${REG_CRX} == 'TRUE'

	  ; CRX강제등록 제거
	  DeleteRegKey HKLM "SOFTWARE\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"
	  ;DeleteRegKey HKLM "SOFTWARE\WOW6432Node\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"
	  ;DeleteRegKey HKCU "SOFTWARE\Microsoft\Edge\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"  

	  DeleteRegKey HKLM "SOFTWARE\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"
	  ;DeleteRegKey HKLM "SOFTWARE\WOW6432Node\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"
	  ;DeleteRegKey HKCU "SOFTWARE\Google\Chrome\Extensions\gbbehmiepgfmmnifjbnknjaebgmnpbam"

	${EndIf}  
  
  SetAutoClose true
  
SectionEnd