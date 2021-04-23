using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HeyRed.Mime;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using SharpCompress.Common;
using SharpCompress.Archives;
using Serilog;
using Serilog.Events;
using AgLogManager;
using OpenNetLinkApp.PageEvent;
using Org.BouncyCastle.Math.EC;
using BlazorInputFile;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eFileAddErr
    {
		eFANone = 0,		// None
		eFAREG,             // 등록된 파일
		eFADRM,             // DRM
		eFADLP,             // DLP
		eFAEXT,             // 확장자제한
		eFAZIP,             // zip파일내 확장자
		eFACHG,             // 파일변경
		eFAVIRUS,           // 바이러스검출
		eFAFileSize,        // 파일사이즈
		eFANotFound,        // 파일 찾기 실패
		eFAHidden,          // 숨김파일
		eFAZipPW,           // zip 파일 비번 있을 때
		eFAZipNotPW,        // zip 파일 비번 없을 때
		eFAZipError,        // zip 파일 손상 또는 zip 파일이 아닌경우
		eFAEMPTY,           // 빈파일
		eFAUNKNOWN,         // 알수없는파일형식
		eFAEML,             // EML파일과 다른파일을 함께 등록할 경우
		eFAEMPTY_ATTACH,    // 빈파일(첨부파일)
		eFAUNKNOWN_ATTACH,  // 알수없는파일형식(첨부파일)
		eFACHG_ATTACH,      // 파일변경(첨부파일)
		eFAEXT_ATTACH,      // 확장자제한(첨부파일)
		eFAZIP_ATTACH,      // zip파일내 확장자(첨부파일)
		eFAEML_ONLYONE,     // EML 파일등록 2건이상일때
		eFAEMLTOPDF_ERROR,  // EML to PDF 변환오류
		eFA_FILE_READ_ERROR,	//파일읽기 권한오류

		eFAOfficeSizeError,		// Office > pdf 변환하려는 파일이 설정되크기보다 클 경우
		eFAOfficeNoinstalled,   // Office 설치않되어있음. 파일변환기능사용. Office파일 전송하려할 경우

		eFADLPERR,				// 개인정보 검출에러
		eFAUnZipOutOfSpace,		//UnZip Disk용량부족
		eFAUnZipLengthOver,		//UnZip Length Over
		eFAUnZipCheckStop,		//UnZip 체크 중단

		eFADAYCOUNTOVER = 51,	// 일일 전송횟수 제한.
		eFADAYSIZEOVER,         // 일일 전송사이즈 제한.

		eUnZipInnerZipOpenFail = 60,			// zip파일 내부의 zip Open 실패
		eUnZipInnerZipPassword,					// zip파일에 내부의 zip 비밀번호 사용 중
		eUnZipInnerExt,							// zip파일에 내부의 zip 확장자 제한 파일 포함
		eUnZipInnerExtChange,					// zip파일에 내부의 zip 위변조 파일 포함
		eUnZipInnerExtUnknown,					// zip파일에 내부의 zip 알수 없는 파일형식 포함
		eUnZipInnerFileEmpty,					// zip파일에 내부의 zip 비어있는 파일
		eUnZipInnerLengthOver,					// zip파일에 내부의 zip Length Over
		eUnZipInnerLeftZip,						// zip파일검사 후 남아 있는 zip포함
		eUnZipInnerDRM,							// zip파일에 내부의 DRM 파일

		eFA_LONG_PATH = 70,						//전송 길이초과
		eFA_LONG_PATH_PARENT,				    //상위폴더 길이초과
		eFA_LONG_PATH_FILEORPATH				//파일 및 폴더 길이초과
	}

	public class FileAddErr
    {
		public string FileName = "";
		public eFileAddErr eErrType = eFileAddErr.eFANone;
		public string FilePath = "";
		public string ExceptionReason = "";
		public bool bSub = false;

		XmlConfService xmlConf = new XmlConfService();
		public FileAddErr()
        {

		}

		public FileAddErr(FileAddErr err)
		{
			FileName = err.FileName;
			eErrType = err.eErrType;
			FilePath = err.FilePath;
			ExceptionReason = SetExceptionReason(err.eErrType);
			bSub = err.bSub;
		}
		~FileAddErr()
        {

        }

		public void SetFileAddErr(string strFilename, eFileAddErr err, string strFilePath, bool Sub = false)
        {
			FileName = strFilename;
			eErrType = err;
			FilePath = strFilePath;
			ExceptionReason = SetExceptionReason(err);
			bSub = Sub;
		}

		public string GetExceptionCountString(int count)
        {
			string str = xmlConf.GetTitle("T_ETC_FAEXCEPTIONCOUNT");                // {0} 개
			str = String.Format(str, count);
			return str;
		}

		public string SetExceptionReason(eFileAddErr err)
		{
			string str = "";
			switch (err)
			{
				case eFileAddErr.eFAREG:                                // 등록된 파일
					str = xmlConf.GetTitle("T_eFAREG");                 // 이미 등록된 파일
					break;
				case eFileAddErr.eFADRM:                                // DRM
					str = xmlConf.GetTitle("T_eFADRM");                 // DRM 편집 권한없음
					break;
				case eFileAddErr.eFADLP:                                // DLP
					str = xmlConf.GetTitle("T_eFADLP");                 // 개인정보 포함
					break;
				case eFileAddErr.eFAEXT:                                // 확장자제한
					str = xmlConf.GetTitle("T_eFAEXT");                 // 확장자 제한
					break;
				case eFileAddErr.eFAZIP:                                // zip파일내 확장자
					str = xmlConf.GetTitle("T_eFAZIP");                 // 압축파일내 제한된 파일
					break;
				case eFileAddErr.eFACHG:                                // 파일변경
					str = xmlConf.GetTitle("T_eFACHG");                 // 확장자 변경
					break;
				case eFileAddErr.eFAVIRUS:                              // 바이러스검출
					str = xmlConf.GetTitle("T_eFAVIRUS");                 // 바이러스 포함
					break;
				case eFileAddErr.eFAFileSize:                           //  파일사이즈
					str = xmlConf.GetTitle("T_eFAFileSize");                 // 용량초과
					break;
				case eFileAddErr.eFANotFound:                                // 파일 찾기 실패
					str = xmlConf.GetTitle("T_eFAFileSize");                 // 찾을 수 없음
					break;
				case eFileAddErr.eFAHidden:                                // 숨김파일
					str = xmlConf.GetTitle("T_eFAHidden");                 // 숨김 파일
					break;
				case eFileAddErr.eFAZipPW:                                //  zip 파일 비번 있을 때
					str = xmlConf.GetTitle("T_eFAZipPW");                 // 암호설정된 압축파일
					break;
				case eFileAddErr.eFAZipNotPW:                                //  zip 파일 비번 없을 때
					str = xmlConf.GetTitle("T_eFAZipNotPW");                 // 암호 미설정된 압축파일
					break;
				case eFileAddErr.eFAZipError:                                // zip 파일 손상 또는 zip 파일이 아닌경우
					str = xmlConf.GetTitle("T_eFAZipError");                 // 손상된 파일
					break;
				case eFileAddErr.eFAEMPTY:                                // 빈파일
					str = xmlConf.GetTitle("T_eFAEMPTY");                 // 빈파일
					break;
				case eFileAddErr.eFAUNKNOWN:                                // 알수없는파일형식
					str = xmlConf.GetTitle("T_eFAUNKNOWN");                 // 알수 없는 형식
					break;
				case eFileAddErr.eFAEML:                                // EML파일과 다른파일을 함께 등록할 경우
					str = xmlConf.GetTitle("T_eFAEML");                 // EML파일이 아님
					break;
				case eFileAddErr.eFAEMPTY_ATTACH:                                // 빈파일(첨부파일)
					str = xmlConf.GetTitle("T_eFAEMPTY_ATTACH");                 // EML파일내 빈파일 포함
					break;
				case eFileAddErr.eFAUNKNOWN_ATTACH:                                // 알수없는파일형식(첨부파일)
					str = xmlConf.GetTitle("T_eFAUNKNOWN_ATTACH");                 // EML내 알수 없는 형식의 파일 포함
					break;
				case eFileAddErr.eFACHG_ATTACH:                                // 파일변경(첨부파일)
					str = xmlConf.GetTitle("T_eFACHG_ATTACH");                 // EML내 확장자 변경 파일 포함
					break;

				case eFileAddErr.eFAEXT_ATTACH:                                // 확장자제한(첨부파일)
					str = xmlConf.GetTitle("T_eFAEXT_ATTACH");                 // EML내 확장자 제한 파일 포함
					break;

				case eFileAddErr.eFAZIP_ATTACH:                                // zip파일내 확장자(첨부파일)
					str = xmlConf.GetTitle("T_eFAZIP_ATTACH");                 // EML내 압축파일의 제한된 파일 포함
					break;

				case eFileAddErr.eFAEML_ONLYONE:                                // EML 파일등록 2건이상일때
					str = xmlConf.GetTitle("T_eFAEML_ONLYONE");                 // EML은 1건만 가능
					break;

				case eFileAddErr.eFAEMLTOPDF_ERROR:                                // EML to PDF 변환오류
					str = xmlConf.GetTitle("T_eFAEMLTOPDF_ERROR");                 // EML to PDF 변환오류
					break;

				case eFileAddErr.eFAOfficeSizeError:                                // Office > pdf 변환하려는 파일이 설정되크기보다 클 경우
					str = xmlConf.GetTitle("T_eFAOfficeSizeError");                 // Office to PDF 변환크기 오류
					break;

				case eFileAddErr.eFAOfficeNoinstalled:                                // Office 설치않되어있음. 파일변환기능사용. Office파일 전송하려할 경우
					str = xmlConf.GetTitle("T_eFAOfficeNoinstalled");                 // Office  미설치, 변환오류
					break;

				case eFileAddErr.eFADLPERR:                                // 개인정보 검출에러
					str = xmlConf.GetTitle("T_eFADLPERR");                 // 개인정보 검출 오류
					break;
				case eFileAddErr.eFAUnZipOutOfSpace:                                //UnZip Disk용량부족
					str = xmlConf.GetTitle("T_eUNZIP_OUT_OF_SPACE");                 // 압축파일 검사 Disk 용량 부족
					break;
				case eFileAddErr.eFAUnZipLengthOver:                                // UnZip Length Over
					str = xmlConf.GetTitle("T_eUNZIP_OUT_OF_LENGTH");                 // 압축파일 내부의 파일 및 경로 길이 초과로 검사실패
					break;
				case eFileAddErr.eFAUnZipCheckStop:                                //UnZip 체크 중단
					str = xmlConf.GetTitle("T_eUNZIP_CHECK_STOP");                 // 압축파일 검사취소
					break;
				case eFileAddErr.eFADAYCOUNTOVER:                                // 일일 전송횟수 제한.
					/* TODO */
					 str = xmlConf.GetTitle("T_INFO_ONEDAY_TRANCE_COUNTLIMIT"); 
					//str = "일일 전송횟수 제한";
					break;
				case eFileAddErr.eFADAYSIZEOVER:                                // 일일 전송용량 제한. 
                    /* TODO */
                    str = xmlConf.GetTitle("T_INFO_ONEDAY_TRANCE_SIZELIMIT");
					//str = "일일 전송사이즈 제한";
					break;

				case eFileAddErr.eUnZipInnerZipOpenFail:                                // zip파일 내부의 zip Open 실패 
					/* TODO */
					str = xmlConf.GetTitle("T_eUNZIP_OPEN_FAIL"); 
					//str = "ZIP 열기 오류";
					break;

				case eFileAddErr.eUnZipInnerZipPassword:                                // zip파일에 내부의 zip 비밀번호 사용 중
					/* TODO */
					str = xmlConf.GetTitle("T_eUnZipInnerZipPassword"); 
					//str = xmlConf.GetTitle("T_eFAZipPW");
					break;

				case eFileAddErr.eUnZipInnerExt:                                // zip파일에 내부의 zip 확장자 제한 파일 포함
					/* TODO */
					 str = xmlConf.GetTitle("T_eUnZipInnerExt"); 
					//str = xmlConf.GetTitle("T_eFAEXT");
					break;

				case eFileAddErr.eUnZipInnerExtChange:                               // zip파일에 내부의 zip 위변조 파일 포함
					/* TODO */
					 str = xmlConf.GetTitle("T_eUnZipInnerExtChange"); 
					//str = xmlConf.GetTitle("T_eFACHG");
					break;

				case eFileAddErr.eUnZipInnerExtUnknown:                                // zip파일에 내부의 zip 알수 없는 파일형식 포함
					/* TODO */
					 str = xmlConf.GetTitle("T_eUnZipInnerExtUnknown"); 
					//str = xmlConf.GetTitle("T_eFAUNKNOWN");
					break;

				case eFileAddErr.eUnZipInnerFileEmpty:                                // zip파일에 내부의 zip 비어있는 파일  
					/* TODO */
					 str = xmlConf.GetTitle("T_eUnZipInnerFileEmpty"); 
					//str = xmlConf.GetTitle("T_eFAEMPTY");
					break;

				case eFileAddErr.eUnZipInnerLengthOver:                                // zip파일에 내부의 zip Length Over
					/* TODO */
					str = xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH");               // 파일명 및 폴더명 길이초과(80자)
					//str = "파일 및 폴더명 길이 초과";
					break;

				case eFileAddErr.eUnZipInnerLeftZip:                                // zip파일검사 후 남아 있는 zip포함
					/* TODO */
					str = xmlConf.GetTitle("T_eUNZIP_LEFT_ZIP");                    // ZIP파일 검사후 잔여 ZIP 포함
					//str = "ZIP파일 검사후 잔여 ZIP 포함";
					break;

				case eFileAddErr.eUnZipInnerDRM:                                // zip파일에 내부의 DRM 파일
					/* TODO */
					str = xmlConf.GetTitle("T_eUnZipInnerDRM");                 // T_eUNZIP_INNER_DRMFILE 
					//str = "ZIP파일 내부 DRM 파일";
					break;

				case eFileAddErr.eFA_LONG_PATH:                                //전송 길이초과
					str = xmlConf.GetTitle("L_eFA_LONG_PATH");                 // 전송 길이초과(90자)
					break;

				case eFileAddErr.eFA_LONG_PATH_PARENT:                                //상위폴더 길이초과
					str = xmlConf.GetTitle("L_eFA_LONG_PATH_PARENT");                 // 상위폴더명 길이초과(80자)
					break;

				case eFileAddErr.eFA_LONG_PATH_FILEORPATH:                                //파일 및 폴더 길이초과
					str = xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH");                 // 파일명 및 폴더명 길이초과(80자)
					break;

				case eFileAddErr.eFA_FILE_READ_ERROR:                                // 파일 읽기 권한 오류
					str = xmlConf.GetTitle("L_eFA_FILE_READ_ERROR");                 // 파일 읽기 권한 오류
					break;

				default:
					str = "-";
					break;

			}
			return str;
        }

		public string GetFileAddErrContent(string strFileName, eFileAddErr efa, string strFileLimitSize="1500")
		{
			string strMsg = "";
			switch (efa)
			{
				case eFileAddErr.eFAREG:                                        // 이미 등록되어 있는 파일인 경우
					strMsg = xmlConf.GetWarnMsg("W_0002");                      // {0} 파일은 전송파일에 등록된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEXT:                                        // 확장자 제한이 걸린 파일인 경우
					strMsg = xmlConf.GetWarnMsg("W_0001");                      // {0} 파일은 전송이 제한된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFADLP:                                        // DLP 개인정보가 포함되어 있는 경우
					strMsg = xmlConf.GetWarnMsg("W_0175");                      // {0} 파일은 개인정보가 포함되어 있어 전송할 수 없습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFADLPERR:
					strMsg = xmlConf.GetWarnMsg("W_0176");                      // {0} 파일에 대한 개인정보 검사에 실패하였습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFADRM:                                        // DRM 편집권한이 없는 경우
					strMsg = xmlConf.GetWarnMsg("W_0177");                      // {0} 파일은 DRM이 걸려있어 편집 권한이 없습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAZIP:
					strMsg = xmlConf.GetWarnMsg("W_0185");                      // {0} 파일에 전송이 제한된 파일이 포함되어 있습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFACHG:                                        // 위변조 걸린 파일
					strMsg = xmlConf.GetWarnMsg("W_0006");                      // {0} 파일은 확장자가 변경된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEMPTY:                                      // 빈파일
					strMsg = xmlConf.GetWarnMsg("W_0111");                      // {0} 파일은 비어있는 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAUNKNOWN:
					strMsg = xmlConf.GetWarnMsg("W_0113");                      // {0} 파일은 알수 없는 파일형식입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEML:
					strMsg = xmlConf.GetWarnMsg("W_0178");                      // {0} 파일은 Eml 파일이 아닙니다. Eml 파일과 형식이 다른파일은 함께 전송 할수 없습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEMPTY_ATTACH:
					strMsg = xmlConf.GetWarnMsg("W_0116");                      // {0} 에 비어있는 첨부파일이 포함되어 있습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFACHG_ATTACH:
					strMsg = xmlConf.GetWarnMsg("W_0118");                      // {0} 에 확장자가 변경된 첨부파일이 포함되어 있습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEXT_ATTACH:
					strMsg = xmlConf.GetWarnMsg("W_0119");                      // {0} 에 전송이 제한된 첨부파일이 포함되어 있습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAZIP_ATTACH:
					strMsg = xmlConf.GetWarnMsg("W_0185");                      // {0} 파일에 전송이 제한된 파일이 포함되어 있습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEML_ONLYONE:
					strMsg = xmlConf.GetWarnMsg("W_0179");                      // {0} 파일은 Eml 파일이므로 첨부가 불가합니다. Eml 파일은 1건만 전송이 가능합니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAEMLTOPDF_ERROR:
					strMsg = xmlConf.GetWarnMsg("W_0128");                      // {0} 파일을 PDF로 변환 중 오류가 발생하였습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAFileSize:
					//strMsg = xmlConf.GetWarnMsg("W_0027");                      // 파일은 {0} MB까지 전송할 수 있습니다.
					strMsg = xmlConf.GetWarnMsg("W_0246");                      // {0} 파일은 용량이 초과되어 차단되었습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFANotFound:
					strMsg = xmlConf.GetWarnMsg("W_0028");                      // {0} 파일을 찾을 수 없습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAHidden:
					strMsg = xmlConf.GetWarnMsg("W_0180");                      // {0} 파일은 숨김파일이므로 파일 첨부가 불가합니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAZipPW:
					strMsg = xmlConf.GetWarnMsg("W_0097");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있어 전송이 제한된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAZipNotPW:
					strMsg = xmlConf.GetWarnMsg("W_0100");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있어 전송이 제한된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAZipError:
					strMsg = xmlConf.GetWarnMsg("W_0099");                      // {0} 파일은 분할압축파일 또는 zip 파일이 아니거나 손상된 파일입니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFADAYCOUNTOVER:
					strMsg = xmlConf.GetWarnMsg("W_0181");                      // 일일 전송 횟수를 초과하였습니다.
					break;
				case eFileAddErr.eFADAYSIZEOVER:
					//strMsg = xmlConf.GetWarnMsg("W_0182");                      // 일일 전송 사이즈를 초과하였습니다.
					strMsg = xmlConf.GetWarnMsg("W_0247");                      // {0} 파일은 일일 전송 가능 용량이 초과되어 차단되었습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAVIRUS:
					strMsg = xmlConf.GetWarnMsg("W_0184");                      // {0} 파일에서 바이러스가 검출되었습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAUnZipOutOfSpace:
					strMsg = xmlConf.GetErrMsg("E_0191");                      // {0} 파일은/r/nDisk 용량이 부족하여 검사를 할수 없습니다./r/nC:\를 정리하여 용량을 확보하여 다시 시도 하십시오.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAUnZipLengthOver:
					strMsg = xmlConf.GetErrMsg("E_0192");                      // {0} 파일은/r/n압축파일 내부의 파일 및 경로가 길어 검사가 실패하였습니다./r/n압축파일 내부의 파일 및 경로를 변경하여 다시 시도 하십시오.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFAUnZipCheckStop:
					strMsg = xmlConf.GetErrMsg("E_0199");                      // {0} 파일의 압축파일 검사를 취소 하셨습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;
				case eFileAddErr.eFA_FILE_READ_ERROR:
					strMsg = xmlConf.GetErrMsg("E_0220");                      // {0} 파일의 압축파일 검사를 취소 하셨습니다.
					strMsg = String.Format(strMsg, strFileName);
					break;

				default:
					break;
			}
			return strMsg;
		}
		
	}
    public class FileAddManage
    {
		public List<FileAddErr> m_FileAddErrList = new List<FileAddErr>();
		public List<string> m_FileAddErrReason = new List<string>();
		public List<string> ListFile = null;

		public long m_nTansCurSize = 0;
		public long m_nCurRegisteringSize = 0;

		public FileAddManage()
        {
			ListFile = new List<string>();
		}
		public FileAddManage(int groupID)
		{
			ListFile = new List<string>();
			LoadMimeConf(groupID);
		}
		~FileAddManage()
        {

        }
		//사전에 전송하기 전에 전송량을 미리 계산하는 경우가 있어서 차단되는 경우는 다시 빼준다. 2021/04/23 YKH
		public void RestoreFileSizeLimit()
        {
			m_nTansCurSize -= m_nCurRegisteringSize;
			m_nCurRegisteringSize = 0;
		}

		public void Copy(FileAddManage fileaddManage)
		{
			m_FileAddErrList = new List<FileAddErr>(fileaddManage.m_FileAddErrList);
			m_FileAddErrReason = new List<string>(fileaddManage.m_FileAddErrReason);
			ListFile = new List<string>(fileaddManage.ListFile);
		}

		public void AddData(string strFilename, eFileAddErr err, string strFilePath, bool bSub = false)
        {
			FileAddErr fileAddErr = new FileAddErr();
			fileAddErr.SetFileAddErr(strFilename, err, strFilePath,bSub);
			m_FileAddErrList.Add(fileAddErr);
			
			Log.Information("[AddData] Cheked to Error[{Err}] File[{CurZipFile}] in {OrgZipFile}", err, strFilename, strFilePath);
		}

		public void AddData(FileAddErr fileAddErr)
        {
			FileAddErr fileTemp = new FileAddErr(fileAddErr);
			m_FileAddErrList.Add(fileAddErr);
		}

		public void DataClear()
        {
			m_FileAddErrList.Clear();
			m_FileAddErrReason.Clear();
		}
		public int GetAddErrCount()
        {
			return m_FileAddErrList.Count;

		}

		/**
		 * @breif 확장자 제한에 걸린 파일의 개수를 반환한다.
		 * @return 확장자 제한에 걸린 파일의 개수
		 */
		public int GetExtExceptionCount()
        {
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for(int i=0;i<nTotalCount;i++)
            {
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAEXT)                     // 확장자 제한
					count++;
			}
			return count;
        }

		/**
		 * @breif zip 파일 내 확장자 제한에 걸린 파일의 개수를 반환한다.
		 * @return zip 파일 내 확장자 제한에 걸린 파일의 개수
		 */
		public int GetZipExtExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAZIP)                     // zip 파일 내 확장자 제한
					count++;
			}
			return count;
		}
		/**
		 * @breif 파일 위변조에 걸린 파일의 개수를 반환한다.
		 * @return 파일 위변조에 걸린 파일의 개수
		 */
		public int GetChangeExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFACHG)                     // 파일 위변조
					count++;
			}
			return count;
		}

		/**
		 * @breif 파일 사이즈 초과된 파일의 개수를 반환한다.
		 * @return 파일 사이즈 초과된 파일의 개수
		 */
		public int GetSizeExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAFileSize)                     // 파일 사이즈 초과
					count++;
			}
			return count;
		}

		/**
		 * @breif 존재하지 않는 파일의 개수를 반환한다.
		 * @return 존재하지 않는 파일의 개수
		 */
		public int GetNotFoundExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFANotFound)                     // 존재하지 않는 파일의 개수
					count++;
			}
			return count;
		}
		/**
		 * @breif 숨김 파일의 개수를 반환한다.
		 * @return 숨김 파일의 개수
		 */
		public int GetHiddenExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAHidden)                     // 숨김 파일의 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 비번 있을 때 제외된 파일의 개수를 반환한다.
		 * @return zip 파일의 비번 있을 때 제외된 파일의 개수
		 */
		public int GetZipPWExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAZipPW)                     // zip 파일의 비번 있을 때 제외
					count++;
			}
			return count;
		}
		/**
		 * @breif zip 파일의 비번 없을 때 제외된 파일의 개수를 반환한다.
		 * @return zip 파일의 비번 없을 때 제외된 파일의 개수
		 */
		public int GetZipNotPWExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAZipNotPW)                     // zip 파일의 비번 없을 때 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일이 손상되어 제외된 파일의 개수를 반환한다.
		 * @return zip 파일이 손상되어 제외된 파일의 개수
		 */
		public int GetZipErrorExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAZipError)                     // zip 파일이 손상되어 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif 빈파일로 제외된 파일의 개수를 반환한다.
		 * @return 빈파일로 제외된 파일의 개수
		 */
		public int GetEmptyExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAEMPTY)                     // 빈파일로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif 알수 없는 파일 형식으로 제외된 파일의 개수를 반환한다.
		 * @return 알수 없는 파일 형식으로 제외된 파일의 개수
		 */
		public int GetUnKnownExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFAUNKNOWN)                     // 알수 없는 파일 형식으로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif 일일 전송횟수 제한으로 제외된 파일의 개수를 반환한다.
		 * @return 일일 전송횟수 제한으로 제외된 파일의 개수
		 */
		public int GetDayCountOverExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFADAYCOUNTOVER)                     // 일일 전송횟수 제한으로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif 일일 전송사이즈 제한으로 제외된 파일의 개수를 반환한다.
		 * @return 일일 전송사이즈 제한으로 제외된 파일의 개수
		 */
		public int GetDaySizeOverExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFADAYSIZEOVER)                     // 일일 전송사이즈 제한으로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif 전송길이 초과로 제외된 파일의 개수를 반환한다.
		 * @return 전송길이 초과로 제외된 파일의 개수
		 */
		public int GetFilePathOverExceptionCount()
        {
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFA_LONG_PATH)                     // 전송길이 초과 제외
					count++;
			}
			return count;
		}
		/**
		 * @breif 상위폴더명 길이 초과로 제외된 파일의 개수를 반환한다.
		 * @return 상위폴더명 길이 초과로 제외된 파일의 개수
		 */
		public int GetSuperFolderNameOverExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFA_LONG_PATH_PARENT)                     // 상위폴더명 길이 초과로 제외
					count++;
			}
			return count;
		}
		/**
		 * @breif 파일명 및 폴더명 길이 초과로 제외된 파일의 개수를 반환한다.
		 * @return 파일명 및 폴더명 길이 초과로 제외된 파일의 개수
		 */
		public int GetFileFolderNameOverExceptionCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFA_LONG_PATH_FILEORPATH)                     // 파일명 및 폴더명 길이 초과로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 zip 파일 Open 실패한 개수를 반환한다.
		 * @return zip 파일의 내부 zip 파일 Open 실패한 개수
		 */
		public int GetInnerZipOpenFailCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerZipOpenFail)                     // zip 파일의 내부 zip 파일 Open 실패로 제외
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 zip 파일 비번 사용 중인 개수를 반환한다.
		 * @return zip 파일의 내부 zip 파일 비번 사용 중인 개수
		 */
		public int GetInnerZipPassWordCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerZipPassword)                     // zip 파일의 내부 zip 파일 비번 사용 중인 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 확장자 제한 파일 개수를 반환한다.
		 * @return zip 파일의 내부 확장자 제한 파일 개수
		 */
		public int GetInnerZipExtCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerExt)                     // zip 파일의 내부 확장자 제한 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 확장자 변경 파일 개수를 반환한다.
		 * @return zip 파일의 내부 확장자 변경 파일 개수
		 */
		public int GetInnerZipExtChangeCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerExtChange)                     // zip 파일의 내부 확장자 변경 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 알수 없는 파일 개수를 반환한다.
		 * @return zip 파일의 내부 알수 없는 파일 개수
		 */
		public int GetInnerZipUnKnownCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerExtUnknown)                     // zip 파일의 내부 알수 없는 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 빈 파일 개수를 반환한다.
		 * @return zip 파일의 내부 빈 파일 개수
		 */
		public int GetInnerZipEmptyCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerFileEmpty)                     // zip 파일의 내부 빈 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 zip Length Over 개수를 반환한다.
		 * @return  zip 파일의 내부 zip Length Over 개수
		 */
		public int GetInnerZipLengthOverCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerLengthOver)                     // zip 파일의 내부 zip Length Over 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 검사 후 남아있는 zip 파일 개수를 반환한다.
		 * @return  zip 파일의 내부 검사 후 남아있는 zip 파일 개수
		 */
		public int GetInnerZipLeftZipCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerLeftZip)                     // zip 파일의 내부 검사 후 남아있는 zip 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif zip 파일의 내부 DRM 파일 개수를 반환한다.
		 * @return  zip 파일의 내부 DRM 파일 개수
		 */
		public int GetInnerZipDRMCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eUnZipInnerDRM)                     // zip 파일의 내부 DRM 파일 개수
					count++;
			}
			return count;
		}

		/**
		 * @breif 읽기 권한이 없어 제외된 파일의 개수를 반환한다.
		 * @return  읽기 권한이 없어 제외된 파일의 개수
		 */
		public int GetReadDenyCount()
		{
			int nTotalCount = GetAddErrCount();
			if (nTotalCount <= 0)
				return nTotalCount;

			int count = 0;
			for (int i = 0; i < nTotalCount; i++)
			{
				eFileAddErr e = m_FileAddErrList[i].eErrType;
				if (e == eFileAddErr.eFA_FILE_READ_ERROR)                     // 읽기 권한이 없어 제외된 파일의 개수
					count++;
			}
			return count;
		}

		public static bool GetRegCountEnable(int nStandardCount, int nRegCount)
        {
			if (nStandardCount < nRegCount)
				return false;
			return true;
        }

		public bool GetSizeEnable(long nStandardSize, long nRegSize)
		{

			long nAddedTotalSize = m_nTansCurSize + nRegSize;

			if (nStandardSize < nRegSize)		// 단일크기가 전송가능 Size보다 클때
				return false;

			if (nStandardSize < nAddedTotalSize)       // 추가로 누적된 크기가 전송가능 Size보다 클때
				return false;

			return true;
		}

		public bool GetDaySizeEnable(long FileTransMaxSize,long RemainFileTransSize, long nRegSize)
		{
			if (FileTransMaxSize <= 0)
				return true;

			if (RemainFileTransSize < nRegSize)
				return false;
			return true;
		}

		public bool GetEmptyEnable(long nRegSize)
        {
			if (nRegSize <= 0)
				return false;
			return true;
        }
		public bool GetRegExtEnable(bool bWhite,string strStandardFileExtInfo, string strExt)
		{
			if ((strStandardFileExtInfo.Equals("")) || (strStandardFileExtInfo.Equals(";")))
				return !bWhite;

			char sep = (char)';';
			string[] strExtList = strStandardFileExtInfo.Split(sep);
			int count = strExtList.Length;
			if (count <= 0)
				return !bWhite;

			bool bFind = false;
			for(int i=0;i<count;i++)
            {
				if(strExtList[i].Equals(strExt))
                {
					bFind = true;
					break;
                }
            }

			if (bWhite && bFind)
				return true;
			else if ((!bWhite) && bFind)
				return false;
			else if (bWhite && (!bFind))
				return false;
			else if ((!bWhite) && (!bFind))
				return true;
			else
				return false;

		}
		public string GetFileRename(bool bMode, string strFileName)
		{
			if (bMode == true)
			{
				strFileName = strFileName.Replace("`", "^TD^");
				strFileName = strFileName.Replace("&", "^AP^");
				strFileName = strFileName.Replace("%", "^PC^");
				strFileName = strFileName.Replace("!", "^EM^");
				strFileName = strFileName.Replace("@", "^AT^");

				strFileName = strFileName.Replace("#", "^SH^");
				strFileName = strFileName.Replace("$", "^DL^");
				strFileName = strFileName.Replace("*", "^AS^");
				strFileName = strFileName.Replace("(", "^LR^");
				strFileName = strFileName.Replace(")", "^RR^");

				strFileName = strFileName.Replace("-", "^DS^");
				strFileName = strFileName.Replace("+", "^PL^");
				strFileName = strFileName.Replace("=", "^EQ^");
				strFileName = strFileName.Replace(";", "^SC^");
				strFileName = strFileName.Replace("'", "^SQ^");
			}
			else
			{
				strFileName = strFileName.Replace("^TD^", "`");
				strFileName = strFileName.Replace("^AP^", "&");
				strFileName = strFileName.Replace("^PC^", "%");
				strFileName = strFileName.Replace("^EM^", "!");
				strFileName = strFileName.Replace("^AT^", "@");

				strFileName = strFileName.Replace("^SH^", "#");
				strFileName = strFileName.Replace("^DL^", "$");
				strFileName = strFileName.Replace("^AS^", "*");
				strFileName = strFileName.Replace("^LR^", "(");
				strFileName = strFileName.Replace("^RR^", ")");

				strFileName = strFileName.Replace("^DS^", "-");
				strFileName = strFileName.Replace("^PL^", "+");
				strFileName = strFileName.Replace("^EQ^", "=");
				strFileName = strFileName.Replace("^SC^", ";");
				strFileName = strFileName.Replace("^SQ^", "'");
			}

			return strFileName;
		}

		public string GetConvertTitleDesc(bool bMode, string str)
        {
			if(bMode)
            {
				str = str.Replace("&", "&amp");
				str = str.Replace("<", "&lt;");
				str = str.Replace(">", "&gt;");
				str = str.Replace("\n", "$ET;");
				str = str.Replace("\"", "&quot");
				str = str.Replace("\'", "&apos");
			}
			else
            {
				str = str.Replace("&lt;","<");
				str = str.Replace("&gt;",">");
				str = str.Replace("$ET;","\n");
				str = str.Replace("&quot","\"");
				str = str.Replace("&apos","\'");
				str = str.Replace("&amp", "&");
			}
			return str;
        }
		private bool FilePathLength(string strFileRelativePath)
		{
			string strFileReName = strFileRelativePath;
			//string strFileReName = GetFileRename(true, strFileRelativePath);
			//byte[] temp = Encoding.Default.GetBytes(strFileReName);
			//strFileReName = Encoding.UTF8.GetString(temp);
			if (strFileReName.Length >= 90 )							// 전체 경로 길이 확인 (90자)
			{
				return false;
			}
			return true;
		}

		private bool FileFolderNameLength(string strFileRelativePath, out bool bSuper)
		{
			string strFileReName = strFileRelativePath;
			//string strFileReName = GetFileRename(true, strFileRelativePath);
			//byte[] temp = Encoding.Default.GetBytes(strFileReName);
			//strFileReName = Encoding.UTF8.GetString(temp);

			char sep;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				sep = (char)'\\';
			}
			else
            {
				sep = (char)'/';
			}
			string[] strUnitPath = strFileReName.Split(sep);

			bool bRet = true;
			int index = 0;
			for(index = 0; index < strUnitPath.Length; index++)
            {
				string strName = strUnitPath[index];
				if (strName.Length >= 80)                                       // 폴더 및 파일 경로 길이 확인 (80자)
                {
					bRet = false;
					break;
				}
			}
			if (index == strUnitPath.Length - 1)
				bSuper = false;
			else
				bSuper = true;

			return bRet;
		}

		public void SetFileReadError(HsStream hsStream)
        {
			string strFileName = hsStream.FileName;
			string strRelativePath = hsStream.RelativePath;
			AddData(strFileName, eFileAddErr.eFA_FILE_READ_ERROR, strRelativePath);
		}

		public async Task<int> GetExamFileExtChange(HsStream hsStream)
		{
			eFileAddErr enRet;
			string strExt = Path.GetExtension(hsStream.FileName);
			enRet = await IsValidFileExt(hsStream.stream, strExt);
			if(enRet != eFileAddErr.eFANone)
            {
				string strFileName = hsStream.FileName;
				string strRelativePath = hsStream.RelativePath;
				AddData(strFileName, enRet, strRelativePath);
				return -1;
            }
			return 0;
		}

		public bool GetExamFileAddEnable(HsStream hsStream, bool bWhite, string strFileExtInfo, bool bHidden, long ConvEnableSize, long RegSize, long FileTransMaxSize, long RemainFileTransSize)
        {
			if (hsStream == null)
				return true;

			bool bSizeEnable = false;                       // 사이즈 용량 검사 결과
			bool bDaySizeEnable = false;				    // 일일 전송 사이즈 용량 검사 결과.
			bool bExtEnable = false;                        // 확장자 제한 검사 결과
			bool bHiddenEnable = false;                     // 숨김 파일인지 검사 결과
			bool bFilePathEnable = false;                   // 긴파일명 전체 경로 길이 검사
			bool bFileFolderNameEnable = false;             // 폴더 및 파일 경로 길이 확인 (80자)
			bool bEmpty = false;                            // 빈파일인지 여부 검사 

			bSizeEnable = GetRegSizeEnable(ConvEnableSize, RegSize, hsStream.Type, hsStream.FileName, hsStream.RelativePath);
			if (!bSizeEnable)
				return false;

			bDaySizeEnable = GetDayRegSizeEnable(FileTransMaxSize, RemainFileTransSize, RegSize, hsStream.Type, hsStream.FileName, hsStream.RelativePath);
			if (!bSizeEnable)
				return false;

			bExtEnable = GetRegExtEnable(bWhite, strFileExtInfo, hsStream.Type, hsStream.FileName, hsStream.RelativePath);
			if (!bExtEnable)
				return false;

			bHiddenEnable = GetRegHiddenEnable(bHidden, hsStream.FileName, hsStream.RelativePath);
			if (!bHiddenEnable)
				return false;

			bFilePathEnable = GetRegFilePathEnable(hsStream.FileName, hsStream.RelativePath);
			if (!bFilePathEnable)
				return false;

			bFileFolderNameEnable = GetRegFileFolderNameEnable(hsStream.FileName, hsStream.RelativePath);
			if (!bFileFolderNameEnable)
				return false;

			bEmpty = GetRegFileEmptyEnable(hsStream.FileName, hsStream.RelativePath,hsStream.Size);
			if (!bEmpty)
				return false;

			bool bRet = (bExtEnable & bHiddenEnable & bFilePathEnable & bFileFolderNameEnable & bEmpty);
			if(bRet)
			{
				m_nCurRegisteringSize = RegSize;
				m_nTansCurSize += RegSize;
			}
			return bRet;

		}
		public bool GetRegSizeEnable(long ConvEnableSize, long RegSize, string strExt, string strFileName, string strRelativePath)
		{
			if (GetSizeEnable(ConvEnableSize, RegSize) != true)
			{
				AddData(strFileName, eFileAddErr.eFAFileSize, strRelativePath);
				return false;
			}

			return true;
		}

		public bool GetDayRegSizeEnable(long FileTransMaxSize, long RemainFileTransSize,long RegSize, string strExt, string strFileName, string strRelativePath)
		{
			if (GetDaySizeEnable(FileTransMaxSize, RemainFileTransSize,RegSize) != true)
			{
				AddData(strFileName, eFileAddErr.eFADAYSIZEOVER, strRelativePath);
				return false;
			}
			return true;
		}
		public bool GetRegExtEnable(bool bWhite, string strFileExtInfo, string strExt, string strFileName, string strRelativePath)
		{
			if (GetRegExtEnable(bWhite, strFileExtInfo, strExt) != true)
			{
				AddData(strFileName, eFileAddErr.eFAEXT, strRelativePath);
				return false;
			}
			return true;
		}

		public bool GetRegHiddenEnable(bool bHidden, string strFileName, string strRelativePath)
		{
			if (bHidden)
			{
				AddData(strFileName, eFileAddErr.eFAHidden, strRelativePath);
				return false;
			}
			return true;
		}

		public bool GetRegFilePathEnable(string strFileName, string strRelativePath)
		{
			if(FilePathLength(strRelativePath)!=true)
            {
				AddData(strFileName, eFileAddErr.eFA_LONG_PATH, strRelativePath);
				return false;
			}
			return true;
		}

		public bool GetRegFileFolderNameEnable(string strFileName, string strRelativePath)
		{
			bool bSuper = false;
			if (FileFolderNameLength(strRelativePath,out bSuper) != true)
			{
				if(bSuper)
					AddData(strFileName, eFileAddErr.eFA_LONG_PATH_PARENT, strRelativePath);					// 상위폴더 길이 초과
				else
					AddData(strFileName, eFileAddErr.eFA_LONG_PATH_FILEORPATH, strRelativePath);                // 파일 및 폴더명 길이 초과
				return false;
			}
			return true;
		}
		public bool GetRegFileEmptyEnable(string strFileName, string strRelativePath,long nSize)
		{
			if (GetEmptyEnable(nSize) != true)
			{
				AddData(strFileName, eFileAddErr.eFAEMPTY, strRelativePath);                    // 상위폴더 길이 초과
				return false;
			}
			return true;
		}
		public List<string> GetMakeReason()
        {
			string strReason = "";
			string strCount = "";
			m_FileAddErrReason.Clear();
			FileAddErr fileAddErr = new FileAddErr();

			int nExtExceptionCount = 0;
			nExtExceptionCount = GetExtExceptionCount();
			if (nExtExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAEXT);
				strCount = fileAddErr.GetExceptionCountString(nExtExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nChangeExceptionCount = 0;
			nChangeExceptionCount = GetChangeExceptionCount();
			if (nChangeExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFACHG);
				strCount = fileAddErr.GetExceptionCountString(nChangeExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nHiddenExceptionCount = 0;
			nHiddenExceptionCount = GetHiddenExceptionCount();
			if (nHiddenExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAHidden);
				strCount = fileAddErr.GetExceptionCountString(nHiddenExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}
			int nEmptyExceptionCount = 0;
			nEmptyExceptionCount = GetEmptyExceptionCount();
			if (nEmptyExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAEMPTY);
				strCount = fileAddErr.GetExceptionCountString(nEmptyExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFilePathOverExcetpion = 0;
			nFilePathOverExcetpion = GetFilePathOverExceptionCount();
			if (nFilePathOverExcetpion > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH);
				strCount = fileAddErr.GetExceptionCountString(nFilePathOverExcetpion);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nSuperFolderNameOverException = 0;
			nSuperFolderNameOverException = GetSuperFolderNameOverExceptionCount();
			if (nSuperFolderNameOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_PARENT);
				strCount = fileAddErr.GetExceptionCountString(nSuperFolderNameOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFileFolderNameOverException = 0;
			nFileFolderNameOverException = GetFileFolderNameOverExceptionCount();
			if (nFileFolderNameOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_FILEORPATH);
				strCount = fileAddErr.GetExceptionCountString(nFileFolderNameOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFileSizeOverException = 0;
			nFileSizeOverException = GetSizeExceptionCount();
			if (nFileSizeOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAFileSize);
				strCount = fileAddErr.GetExceptionCountString(nFileSizeOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFileReadAccessException = 0;
			nFileReadAccessException = GetReadDenyCount();
			if(nFileReadAccessException>0)
            {
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_FILE_READ_ERROR);
				strCount = fileAddErr.GetExceptionCountString(nFileReadAccessException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}


			int nFileDaySizeOverException = 0;
			nFileDaySizeOverException = GetDaySizeOverExceptionCount();
			if (nFileDaySizeOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFADAYSIZEOVER);
				strCount = fileAddErr.GetExceptionCountString(nFileDaySizeOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nZIpPWException = 0;
			nZIpPWException = GetZipPWExceptionCount();
			if (nZIpPWException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipPW);
				strCount = fileAddErr.GetExceptionCountString(nZIpPWException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nZIpNotPWException = 0;
			nZIpNotPWException = GetZipNotPWExceptionCount();
			if (nZIpNotPWException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipNotPW);
				strCount = fileAddErr.GetExceptionCountString(nZIpNotPWException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nZIpErrorException = 0;
			nZIpErrorException = GetZipErrorExceptionCount();
			if (nZIpErrorException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipError);
				strCount = fileAddErr.GetExceptionCountString(nZIpErrorException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nZipException = 0;
			nZipException = GetZipExtExceptionCount();
			if (nZipException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZIP);
				strCount = fileAddErr.GetExceptionCountString(nZipException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipOpenFailException = 0;
			nInnerZipOpenFailException = GetInnerZipOpenFailCount();
			if (nInnerZipOpenFailException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerZipOpenFail);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipOpenFailException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipPassWordException = 0;
			nInnerZipPassWordException = GetInnerZipPassWordCount();
			if (nInnerZipPassWordException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerZipPassword);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipPassWordException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipExtException = 0;
			nInnerZipExtException = GetInnerZipExtCount();
			if (nInnerZipExtException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExt);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipExtException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipExtChangeException = 0;
			nInnerZipExtChangeException = GetInnerZipExtChangeCount();
			if (nInnerZipExtChangeException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExtChange);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipExtChangeException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipExtUnKnownException = 0;
			nInnerZipExtUnKnownException = GetInnerZipUnKnownCount();
			if (nInnerZipExtUnKnownException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExtUnknown);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipExtUnKnownException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipFileEmptyException = 0;
			nInnerZipFileEmptyException = GetInnerZipEmptyCount();
			if (nInnerZipFileEmptyException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerFileEmpty);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipFileEmptyException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipLengthOverException = 0;
			nInnerZipLengthOverException = GetInnerZipLengthOverCount();
			if (nInnerZipLengthOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerLengthOver);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipLengthOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipLeftZipException = 0;
			nInnerZipLeftZipException = GetInnerZipLeftZipCount();
			if (nInnerZipLeftZipException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerLeftZip);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipLeftZipException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nInnerZipDRMException = 0;
			nInnerZipDRMException = GetInnerZipDRMCount();
			if (nInnerZipDRMException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerDRM);
				strReason = " => " + strReason;
				strCount = fileAddErr.GetExceptionCountString(nInnerZipDRMException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			return m_FileAddErrReason;
		}
		public List<string> LoadRMFileAdd(string strFilePath)
		{
			ListFile.Clear();
			string strFilePathList = System.IO.File.ReadAllText(strFilePath);
			/*
			char sep = (char)'\n';
			string[] strArray = strFilePathList.Split(sep);
			*/
			string[] strArray = strFilePathList.Split('\n').ToArray();
			int count = 0;
			foreach (var item in strArray)
			{
				if (count == 0)
				{	
					count++;
					continue;
				}

				string str = item;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					str = str.Replace("/", "\\");
				}
				else
					str = str.Replace("\\", "/");
				str = str.Replace("\r", "");
				str = str.Replace("\"", "");
				ListFile.Add(str);
				count++;
			}

			FileInfo fileinfo = new FileInfo(strFilePath);
			fileinfo.Delete();
			ListFile = ListFile.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
			return ListFile;
		}

		public int LoadRMFileGroupID(string strFilePath)
		{
			ListFile.Clear();
			string strFilePathList = System.IO.File.ReadAllText(strFilePath);
			char sep = (char)'\n';
			string[] strArray = strFilePathList.Split(sep);
			string str = "";
			foreach (var item in strArray)
			{
				str = item;
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					str = str.Replace("/", "\\");
				}
				else
                {
					str = str.Replace("\\", "/");
				}
				str = str.Replace("\r", "");
				break;
			}

			int nRet = 0;
			if(!str.Equals(""))
            {
				nRet = Convert.ToInt32(str);
			}
			return nRet;
		}

		public bool RMFileExist(string strFilePath)
        {
			FileInfo fileInfo = new FileInfo(strFilePath);
			return fileInfo.Exists;
        }


		static bool ByteArrayCompare(byte[] a1, byte[] a2, int nA1Pos = 0)
		{
			if (a1.Length - nA1Pos <= a2.Length) return false;

			for (int i = 0; i < (a1.Length - nA1Pos) && i < a2.Length; i++)
				if (a1[i + nA1Pos] != a2[i])
					return false;

			return true;
		}

		private static bool FindZipContent(byte[] btFileData, byte[] btSig, int nFileAddPos = 0)
		{
			bool blRet;

			if ((btFileData.Length - nFileAddPos) <= btSig.Length) return false;

			for (int nFilePos = nFileAddPos; nFilePos < (btFileData.Length - btSig.Length); nFilePos++)
			{
				blRet = true;
				for (int nSigPos = 0; nSigPos < btSig.Length; nSigPos++)
				{
					if (btFileData[nFilePos + nSigPos] != btSig[nSigPos])
					{
						blRet = false;
						break;
					}
				}
				if (blRet == true) return true;
			}

			return false;
		}
		/**
        *@biref EGG 파일인지 검사한다. (EGG 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:EGG
        */
		private static bool IsEGG(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x45, 0x47, 0x47, 0x41 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref 워드 문서인지 검사한다.
        *@return true:워드문서
        */
		private static bool IsWord(byte[] btFileData, string strExt)
		{
			Log.Debug("**** IsWord(), ");

			if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true)
			{

				if (String.Compare(strExt, "doc") == 0)
				{
					if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("theme")) == true)
						return true;
				}

				if (String.Compare(strExt, "docx") == 0)
				{
					if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("docProps")) == true &&
						FindZipContent(btFileData, Encoding.UTF8.GetBytes("word")) == true)
						return true;
				}
			}
			
			if (String.Compare(strExt, "doc") == 0)
			{
				byte[] btHLP_Header = new byte[] {
					0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x03, 0x00, 0xFE, 0xFF, 0x09, 0x00,
					0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
				};
				if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;
			}

			return false;
		}

		/**
        *@biref 엑셀 문서인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:워드문서
        */
		private static bool IsXls(byte[] btFileData, string strExt)
		{
			if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true)
			{
				if (String.Compare(strExt, "xls") == 0 && FindZipContent(btFileData, Encoding.UTF8.GetBytes("drs")) == true) return true;
				if (String.Compare(strExt, "xlsx") == 0 && FindZipContent(btFileData, Encoding.UTF8.GetBytes("xl")) == true) return true;
			}

			if (String.Compare(strExt, "xls") == 0)
			{
				byte[] btHLP_Header = new byte[] {
					0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
				byte[] btHLP_Header2 = new byte[] { 
					0x00, 0x03, 0x00, 0xFE, 0xFF, 0x09, 0x00 };
				if (ByteArrayCompare(btFileData, btHLP_Header) == true && ByteArrayCompare(btFileData, btHLP_Header2, 0x19) == true) return true;
			}

			return false;
		}

		private static bool IsPPT(byte[] btFileData, string strExt)
		{
			if (String.Compare(strExt, "ppt") == 0)
			{
				byte[] btHLP_Header = new byte[] {
					0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
					0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x03, 0x00, 0xFE, 0xFF, 0x09, 0x00,
					0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
				};
				if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

				if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("drs")) == true
				&& FindZipContent(btFileData, Encoding.UTF8.GetBytes("downrev.xml")) == true)
					return true;
			}

			if (String.Compare(strExt, "pptx") == 0 &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("docProps")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("ppt")) == true)
				return true;

			return false;
		}
		private static bool IsXPS(byte[] btFileData, string strExt)
		{
			if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("Documents")) == true)
				return true;

			return false;
		}

		/**
        *@biref 한글 문서인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:한글
        */
		private static bool IsHWP(byte[] btFileData, string strExt)
		{
			if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("HWP Document File")) == true) return true;

			byte[] btHLP_Header = new byte[] {0x05, 0x00, 0x48, 0x00, 0x77, 0x00, 0x70, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6D, 0x00,
				0x6D, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00, 0x49, 0x00, 0x6E, 0x00, 0x66, 0x00, 0x6F, 0x00, 0x72, 0x00, 0x6D,
				0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00 };

			if (ByteArrayCompare(btFileData, btHLP_Header, 0xa00) == true) return true;

			return false;
		}

		private static bool IsTXT(byte[] btFileData, string strExt)
		{
			int nCheckLen = 1024 * 5;
			if (nCheckLen > btFileData.Length) nCheckLen = btFileData.Length;

			for (int i = 0; i < nCheckLen; i++)
			{
				if (btFileData[i] <= 0x00 || btFileData[i] > 0x7F)
				{
					if (btFileData[i] > 0x7F)
					{   // 한글
						if ((i + 1) < nCheckLen && btFileData[i + 1] > 0x7F) i += 1;
					}
					else return false;
				}
			}

			return true;
		}

		/**
        *@biref PDF 문서인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:PDF
        */
		private static bool IsPDF(byte[] btFileData, string strExt)
		{
			if (ByteArrayCompare(btFileData, Encoding.UTF8.GetBytes("%PDF-")) == true) return true;

			return false;
		}

		/**
        *@biref jpg 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:jpgtch(
        */
		private static bool IsJPG(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header;

			btHLP_Header = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref gif 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:gif
        */
		private static bool IsGIF(byte[] btFileData, string strExt)
		{
			/** 
            const char GIF_Header[] = "GIF89a";
            short* width = (short*)(m_pByte + 6);
            short* height = (short*)(m_pByte + 8);
            BYTE gct = (BYTE)(m_pByte + 0x0a)[0];
            if (memcmp(m_pByte, GIF_Header, 6) == 0 && *width > 0 && *height > 0 && gct == 0xf7)
            {
                strExt = _T("gif");
                return true;
            }
            */

			if (ByteArrayCompare(btFileData, Encoding.UTF8.GetBytes("GIF89a")) == true && btFileData[0x0a] == 0xf7) return true;

			return false;
		}

		/**
        *@biref png 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:png
        */
		private static bool IsPNG(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}
		
		/**
        *@biref bmp 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:bmp
        */
		private static bool IsBMP(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x42, 0x4d };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref DWF 파일인지 검사한다. (CAD 관련 파일, 도면 교환 파일 ASCII 또는 이진)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:dwg
        */
		private static bool IsDWF(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x28, 0x44, 0x57, 0x46, 0x20, 0x56 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref rar 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:rar
        */
		private static bool IsRAR(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref arj 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:arj
        */
		private static bool IsARJ(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x60, 0xEA };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref iso 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:iso
        */
		private static bool IsISO(byte[] btFileData, string strExt)
		{
			/*	ISO-9660 CD Disc Image
                This signature usually occurs at byte offset 32769 (0x8001),
                34817 (0x8801), or 36865 (0x9001). 
            */
			if (btFileData.Length <= 36865 + 5) return false;

			byte[] btHLP_Header = new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 };
			if (ByteArrayCompare(btFileData, btHLP_Header, 32769) == true) return true;
			if (ByteArrayCompare(btFileData, btHLP_Header, 34817) == true) return true;
			if (ByteArrayCompare(btFileData, btHLP_Header, 36865) == true) return true;

			return false;
		}

		/**
        *@biref jar 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:jar
        */
		private static bool IsJAR(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}
		
		/**
        *@biref msg 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:msg
        */
		private static bool IsMSG(byte[] btFileData, string strExt)
		{
            byte[] btHLP_Header = new byte[] {
                0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x03, 0x00, 0xFE, 0xFF, 0x09, 0x00,
                0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		
		/**
        *@biref msi 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:msi
        */
		private static bool IsMSI(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x04, 0x00, 0xFE, 0xFF, 0x0C, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref com 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:com
        */
		private static bool IsCOM(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref scr 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:scr
        */
		private static bool IsSCR(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref ocx 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:ocx
        */
		private static bool IsOCX(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref arc 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:arc
        */
		private static bool IsARC(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header;
			btHLP_Header = new byte[] { 0x1A, 0x02 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x1A, 0x03 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x1A, 0x04 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x1A, 0x08 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x1A, 0x09 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref lha 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:lha
        */
		private static bool IsLHA(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x2D, 0x6C, 0x68 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref lzh 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:lzh
        */
		private static bool IsLZH(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x2D, 0x6C, 0x68 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref pak 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:pak
        */
		private static bool IsPAK(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x1A, 0x0B };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref tar 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:tar
        */
		private static bool IsTAR(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x42, 0x5A, 0x68 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref tbz 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:tbz
        */
		private static bool IsTGZ(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x1F, 0x8B, 0x08 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref zoo 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:zoo
        */
		private static bool IsZOO(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x5A, 0x4F, 0x4F, 0x20 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref DWG 파일인지 검사한다. (CAD 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:dwg
        */
		private static bool IsDWG(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x41, 0x43, 0x31, 0x30 };//
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref LNK 파일인지 검사한다. (바로가기 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:lnk
        */
		private static bool IsLNK(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4C, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref OBJ 파일인지 검사한다. (오브젝트)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:lnk
        */
		private static bool IsOBJ(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4C, 0x01 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref HLP 파일인지 검사한다. (Windows Help File)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:hlp
        */
		private static bool IsHLP(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header;
			btHLP_Header = new byte[] { 0x3F, 0x5F, 0x03, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x4C, 0x4E, 0x02, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref DER 파일인지 검사한다. (공인인증서 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:der
        */
		private static bool IsDER(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x30, 0x82 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref MP3 파일인지 검사한다. (MP3 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:MP3
        */
		private static bool IsMP3(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x49, 0x44, 0x33 };
			int nCheckLen = 48;
			if (nCheckLen > btFileData.Length) nCheckLen = btFileData.Length;

			for (int i = 0; i < nCheckLen; i++)
			{
				if (btFileData[i] == 0x49 && ByteArrayCompare(btFileData, btHLP_Header, i) == true)
					return true;
			}

			return false;
		}


		/**
        *@biref MGB 파일인지 검사한다. (마이다스 CAD관련 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:MGB
        */
		private static bool IsMGB(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x4D, 0x47, 0x45, 0x4E };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref CAD 관련 STL 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:STL
        */
		private static bool IsSTL(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x53, 0x54, 0x4C, 0x42, 0x20, 0x41, 0x54, 0x46 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref HPT 파일인지 검사한다. (슬라이드쇼 관련 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:HPT
        */
		private static bool IsHPT(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x52, 0x6F, 0x62, 0x75, 0x73, 0x20, 0x44, 0x61, 0x20, 0x46, 0x69, 0x6C, 0x65, 0x00 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref Matroska media containter, including WebM
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:mkv, mka, mks, mk3d, webm
        */
		private static bool IsMKV(byte[] btFileData, string strExt)
		{
			// https://en.wikipedia.org/wiki/List_of_file_signatures
			byte[] btHLP_Header = new byte[] { 0x1A, 0x45, 0xDF, 0xA3 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref EPS 파일인지 검사한다. (Adobe PostScript)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:EPS
        */
		private static bool IsEPS(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0xC5, 0xD0, 0xD3, 0xC6 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			int nCheckLen = 48;
			if (nCheckLen > btFileData.Length) nCheckLen = btFileData.Length;
			btHLP_Header = new byte[] { 0xC5, 0xD0, 0xD3, 0xC6 };

			for (int i = 0; i < nCheckLen; i++)
			{
				if (btFileData[i] == 0x25 && ByteArrayCompare(btFileData, btHLP_Header, i) == true) return true;
			}

			return false;
		}

		/**
        *@biref CHM 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:CHM
        */
		private static bool IsCHM(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x49, 0x54, 0x53, 0x46 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}


		/**
        *@biref MIF 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:MIF
        */
		private static bool IsMIF(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header;
			btHLP_Header = new byte[] { 0x3C, 0x4D, 0x61, 0x6B, 0x65, 0x72, 0x46, 0x69 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			btHLP_Header = new byte[] { 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref CVD 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:CVD
        */
		private static bool IsCVD(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x43, 0x6C, 0x61, 0x6D, 0x41, 0x56, 0x2D, 0x56, 0x44, 0x42 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref SAS7BDAT 파일인지 검사한다.
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:SAS7BDAT
        */
		private static bool IsSAS7BDAT(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC2, 0xEA,
				0x81, 0x60, 0xB3, 0x14, 0x11, 0xCF, 0xBD, 0x92, 0x08, 0x00, 0x09, 0xC7, 0x31, 0x8C, 0x18, 0x1F, 0x10 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref ALZ 파일인지 검사한다. (ALZ 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:ALZ
        */
		private static bool IsALZ(byte[] btFileData, string strExt)
		{
			byte[] btHLP_Header = new byte[] { 0x41, 0x4C, 0x5A, 0x01, 0x0A, 0x00, 0x00, 0x00, 0x42, 0x4C, 0x5A };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}

		/**
        *@biref pst 파일인지 검사한다. (PST 파일)
        *@param strExt 확장자 명을 받아올 버퍼
        *@return true:PST
        */
		private static bool IsPST(byte[] btFileData, string strExt)
		{
			if (ByteArrayCompare(btFileData, Encoding.UTF8.GetBytes("!BD")) == true) return true;

			return false;
		}

		public static bool CheckExtForFileByteData(byte[] btFileData, string strExt)
		{
			Log.Debug("[CheckExtForFileByteData] Check hex data for File");
			try
			{
				if (String.Compare(strExt, "egg") == 0) return IsEGG(btFileData, strExt);
				if (String.Compare(strExt, "doc") == 0 || String.Compare(strExt, "docx") == 0)
					return IsWord(btFileData, strExt);
				if (String.Compare(strExt, "xls") == 0 || String.Compare(strExt, "xlsx") == 0)
					return IsXls(btFileData, strExt);
				if (String.Compare(strExt, "ppt") == 0 || String.Compare(strExt, "pptx") == 0)
					return IsPPT(btFileData, strExt);
				if (String.Compare(strExt, "xps") == 0) return IsXPS(btFileData, strExt);
				if (String.Compare(strExt, "hwp") == 0) return IsHWP(btFileData, strExt);
				if (String.Compare(strExt, "txt") == 0 || String.Compare(strExt, "log") == 0 ||
					String.Compare(strExt, "ini") == 0 || String.Compare(strExt, "sql") == 0 ||
					String.Compare(strExt, "conf") == 0)
					return IsTXT(btFileData, strExt);

				/* 이미지 파일*/
				if (String.Compare(strExt, "pdf") == 0) return IsPDF(btFileData, strExt);
				if (String.Compare(strExt, "jpg") == 0) return IsJPG(btFileData, strExt);
				if (String.Compare(strExt, "gif") == 0) return IsGIF(btFileData, strExt);
				if (String.Compare(strExt, "png") == 0) return IsPNG(btFileData, strExt);
				if (String.Compare(strExt, "bmp") == 0) return IsBMP(btFileData, strExt);

				/* CAD 파일 */
				if (String.Compare(strExt, "dwf") == 0) return IsDWF(btFileData, strExt);

				/* 압축파일 */
				if (String.Compare(strExt, "rar") == 0) return IsRAR(btFileData, strExt);
				if (String.Compare(strExt, "arj") == 0) return IsARJ(btFileData, strExt);
				if (String.Compare(strExt, "iso") == 0) return IsISO(btFileData, strExt);
				if (String.Compare(strExt, "jar") == 0) return IsJAR(btFileData, strExt);

				/* 기타파일 */
				if (String.Compare(strExt, "msg") == 0) return IsMSG(btFileData, strExt);
				
				if (String.Compare(strExt, "msi") == 0) return IsMSI(btFileData, strExt);

				if (String.Compare(strExt, "com") == 0) return IsCOM(btFileData, strExt);
				if (String.Compare(strExt, "scr") == 0) return IsSCR(btFileData, strExt);
				if (String.Compare(strExt, "ocx") == 0) return IsOCX(btFileData, strExt);

				if (String.Compare(strExt, "arc") == 0) return IsARC(btFileData, strExt);
				if (String.Compare(strExt, "lha") == 0) return IsLHA(btFileData, strExt);
				if (String.Compare(strExt, "lzh") == 0) return IsLZH(btFileData, strExt);
				if (String.Compare(strExt, "pak") == 0) return IsPAK(btFileData, strExt);
				if (String.Compare(strExt, "tar") == 0) return IsTAR(btFileData, strExt);
				if (String.Compare(strExt, "tgz") == 0) return IsTGZ(btFileData, strExt);
				if (String.Compare(strExt, "zoo") == 0) return IsZOO(btFileData, strExt);
				if (String.Compare(strExt, "dwg") == 0) return IsDWG(btFileData, strExt);  // CAD 파일
				if (String.Compare(strExt, "obj") == 0) return IsOBJ(btFileData, strExt);  // OBJ 파일
				if (String.Compare(strExt, "hlp") == 0) return IsHLP(btFileData, strExt);  // HLP 파일
				if (String.Compare(strExt, "lnk") == 0) return IsLNK(btFileData, strExt);  // LNK 파일
				if (String.Compare(strExt, "der") == 0) return IsDER(btFileData, strExt);  // DER 파일
				if (String.Compare(strExt, "mp3") == 0) return IsMP3(btFileData, strExt);  // MP3 파일
				if (String.Compare(strExt, "mgb") == 0) return IsMGB(btFileData, strExt);  // 마이다스 파일(CAD관련)
				if (String.Compare(strExt, "hpt") == 0) return IsHPT(btFileData, strExt);  // 슬라이드쇼 관련

				/* Matroska media containter, including WebM */
				/* mkv, mka, mks, mk3d, webm */
				if (String.Compare(strExt, "mkv") == 0) return IsMKV(btFileData, strExt);
				if (String.Compare(strExt, "eps") == 0) return IsEPS(btFileData, strExt);  // Adobe PostScript
				if (String.Compare(strExt, "stl") == 0) return IsSTL(btFileData, strExt);  // CAD 관련 STL 파일
				if (String.Compare(strExt, "chm") == 0) return IsCHM(btFileData, strExt);
				if (String.Compare(strExt, "mif") == 0) return IsMIF(btFileData, strExt);
				if (String.Compare(strExt, "cvd") == 0) return IsCVD(btFileData, strExt);
				if (String.Compare(strExt, "sas7bdat") == 0) return IsSAS7BDAT(btFileData, strExt);
				if (String.Compare(strExt, "alz") == 0) return IsALZ(btFileData, strExt);  // 알집
				if (String.Compare(strExt, "pst") == 0) return IsPST(btFileData, strExt);
			}
			catch (System.Exception err)
			{
				Log.Warning("[CheckExtForFileByteData] Err[{0}-{1}]", err.Message, err.GetType().FullName);
			}
			return false;
		}

		public static bool IsDRM(byte[] btFileData)
		{
			byte[] btHLP_Header;
			/* SCDSA00 */
			btHLP_Header  = new byte[] { 0x53, 0x43, 0x44, 0x53, 0x41, 0x30, 0x30 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			/* <DOCUMENT SAFER */
			btHLP_Header  = new byte[] { 0x3C, 0x44, 0x4F, 0x43, 0x55, 0x4D, 0x45, 0x4E, 0x54, 0x20, 0x53, 0x41, 0x46, 0x45, 0x52, 0x20 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;
			
			
			/* <!-- FasooSecureContainer */
			btHLP_Header  = new byte[] { 0x3C, 0x21, 0x2D, 0x2D, 0x20, 0x46, 0x61, 0x73, 0x6F, 0x6F, 0x53, 0x65, 0x63, 0x75, 0x72, 0x65,
				0x43, 0x6F, 0x6E, 0x74, 0x61, 0x69, 0x6E, 0x65, 0x72, 0x20 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;
			
			/* ?DRMONE  This Document is encrypted and protected by Fasoo DRM */ 
			btHLP_Header  = new byte[] { 0x9B, 0x20, 0x44, 0x52, 0x4D, 0x4F, 0x4E, 0x45, 0x20, 0x20, 0x54, 0x68, 0x69, 0x73, 0x20, 0x44,
				0x6F, 0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x69, 0x73, 0x20, 0x65, 0x6E, 0x63, 0x72, 0x79, 0x70, 0x74, 0x65, 0x64,
				0x20, 0x61, 0x6E, 0x64, 0x20, 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65, 0x64, 0x20, 0x62, 0x79, 0x20, 0x46, 0x61,
				0x73, 0x6F, 0x6F, 0x20, 0x44, 0x52, 0x4D, 0x20 };
			if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

			return false;
		}
		
		private const int MaxBufferSize = 1024 * 64;
		private const int MaxBufferSize2 = 1024 * 1024 * 6;
        private static async Task<byte[]> StreamToByteArrayAsync(Stream stInput, int nMaxSize)
        {
            if (stInput == null) return null;
            byte[] buffer = new byte[nMaxSize];
            stInput.Seek(0, SeekOrigin.Begin);
			
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                read = await stInput.ReadAsync(buffer, 0, buffer.Length);
                stInput.Seek(0, SeekOrigin.Begin);
                ms.Write(buffer, 0, read);
                byte[] temp = ms.ToArray();

                return temp;
            }
		}

		private static byte[] StreamToByteArray(Stream stInput, int nMaxSize)
		{
			if (stInput == null) return null;
			byte[] buffer = new byte[nMaxSize];
			stInput.Seek(0, SeekOrigin.Begin);

			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				read = stInput.Read(buffer, 0, buffer.Length);
				stInput.Seek(0, SeekOrigin.Begin);
				ms.Write(buffer, 0, read);
				byte[] temp = ms.ToArray();

				return temp;
			}
		}

		/**
        * @breif 파일확장자 위변조 검사 수행 
        * @param stFile : 위변조 검사 대상 파일의 MemoryStream or FileStream 
        * @param strExt : 위변조 검사 대상 파일의 확장자 
        */
		public async Task<eFileAddErr> IsValidFileExt(Stream stFile, string strExt, bool blAllowDRM = true)
        {
			byte[] btFileData = await StreamToByteArrayAsync(stFile, MaxBufferSize);

			/* Check DRM File */
			if (IsDRM(btFileData) == true)
            {
				if (blAllowDRM == true) return eFileAddErr.eFANone;
				else					return eFileAddErr.eFAUNKNOWN;
			}

			string strFileMime = MimeGuesser.GuessMimeType(btFileData);
			Log.Information("[IsValidFileExt] FileMime[{0}] Ext[{1}] AllowDrmF[{2}]", strFileMime, strExt, blAllowDRM); 
            
			if (String.Compare(strFileMime, "text/plain") == 0) return eFileAddErr.eFANone;
			
			if (String.IsNullOrEmpty(strExt) == true) {
                if (String.Compare(strFileMime, "application/x-executable") == 0) return eFileAddErr.eFANone;
                return eFileAddErr.eFAUNKNOWN;
            }
			
			if (IsValidMimeAndExtension(strFileMime, strExt) == true) return eFileAddErr.eFANone;
            
			strExt = strExt.Replace(".", "");
			btFileData = await StreamToByteArrayAsync(stFile, MaxBufferSize2);
            if (CheckExtForFileByteData(btFileData, strExt) == true) return eFileAddErr.eFANone;
            
			return eFileAddErr.eFACHG;
        }

		public eFileAddErr IsValidFileExtInnerZip(string strFile, string strExt, bool blAllowDRM)
		{
			var fsStream = new FileStream(strFile, FileMode.Open, FileAccess.Read);
			byte[] btFileData = StreamToByteArray(fsStream, MaxBufferSize);
			fsStream.Close();
			
			if (IsDRM(btFileData) == true)
            {
				if (blAllowDRM == true)		return eFileAddErr.eFANone;
				else						return eFileAddErr.eUnZipInnerDRM;
			}
			
			string strFileMime = MimeGuesser.GuessMimeType(btFileData);
			Log.Information("[IsValidFileExtInnerZip] FileMime[{0}] Ext[{1}] AllowDrmF[{2}]", strFileMime, strExt, blAllowDRM); 
			if (String.Compare(strFileMime, "text/plain") == 0) return eFileAddErr.eFANone;
			
			if (String.IsNullOrEmpty(strExt) == true)
			{
				if (String.Compare(strFileMime, "application/x-executable") == 0) return eFileAddErr.eFANone;
                return eFileAddErr.eUnZipInnerExtUnknown;
			}
			
			if (IsValidMimeAndExtension(strFileMime, strExt) == true) return eFileAddErr.eFANone;
			
			strExt = strExt.Replace(".", "");
			fsStream = new FileStream(strFile, FileMode.Open, FileAccess.Read);
			btFileData = StreamToByteArray(fsStream, MaxBufferSize2);
			fsStream.Close();
            
            Log.Debug("[IsValidFileExtInnerZip] Unknown file signature"); 
            if (CheckExtForFileByteData(btFileData, strExt) == true) return eFileAddErr.eFANone;
			
			return eFileAddErr.eUnZipInnerExtChange;
		}

		/**
        * @breif MimeType 및 확장자 정보 DB인 magic.mgc을 다른 파일로 갱신시 사용 
        * @param stFilePath : magic.mgc 파일 경로 
        */
		public void UpdateMagicDB(string strFilePath)
        {
            MimeGuesser.MagicFilePath = strFilePath;
        }
       
        private static Lazy<Dictionary<string, string>> gMimeTypeMap = new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
			["application/andrew-inset"] = "ez",
			["application/applixware"] = "aw",
			["application/atom+xml"] = "atom",
			["application/atomcat+xml"] = "atomcat",
			["application/atomsvc+xml"] = "atomsvc",
			["application/ccxml+xml"] = "ccxml",
			["application/cdmi-capability"] = "cdmia",
			["application/cdmi-container"] = "cdmic",
			["application/cdmi-domain"] = "cdmid",
			["application/cdmi-object"] = "cdmio",
			["application/cdmi-queue"] = "cdmiq",
			["application/csv"] = "csv",
			["application/cu-seeme"] = "cu",
			["application/davmount+xml"] = "davmount",
			["application/dicom"] = "dcm",
			["application/docbook+xml"] = "dbk",
			["application/dssc+der"] = "dssc",
			["application/dssc+xml"] = "xdssc",
			["application/ecmascript"] = "ecma",
			["application/emma+xml"] = "emma",
			["application/epub"] = "zip xml",
			["application/epub+zip"] = "epub xml",
			["application/exi"] = "exi",
			["application/font-tdpfr"] = "pfr",
			["application/gml+xml"] = "gml",
			["application/gpx+xml"] = "gpx",
			["application/gxf"] = "gxf",
			["application/gzip"] = "gz tgz",
			["application/haansofthwp"] = "frm hwp hwt",
			["application/hyperstudio"] = "stk",
			["application/inkml+xml"] = "ink inkml",
			["application/ipfix"] = "ipfix",
			["application/java-archive"] = "jar",
			["application/java-serialized-object"] = "ser",
			["application/java-vm"] = "class",
			["application/javascript"] = "js",
			["application/json"] = "json",
			["application/jsonml+json"] = "jsonml",
			["application/lost+xml"] = "lostxml",
			["application/mac-binhex40"] = "hqx",
			["application/mac-compactpro"] = "cpt",
			["application/mads+xml"] = "mads",
			["application/marc"] = "mrc marc",
			["application/marcxml+xml"] = "mrcx",
			["application/mathematica"] = "ma mb nb",
			["application/mathml+xml"] = "mathml",
			["application/mbox"] = "mbox",
			["application/mediaservercontrol+xml"] = "mscml",
			["application/metalink+xml"] = "metalink",
			["application/metalink4+xml"] = "meta4",
			["application/mets+xml"] = "mets",
			["application/mods+xml"] = "mods",
			["application/mp21"] = "m21 mp21",
			["application/mp4"] = "mp4s",
			["application/msword"] = "doc dot docm dotm docx dotx",
			["application/mxf"] = "mxf",
			["application/octet-stream"] = "bin lha lzh exe class so dll img iso log",
			["application/oda"] = "oda",
			["application/oebps-package+xml"] = "opf",
			["application/ogg"] = "ogg ogx",
			["application/omdoc+xml"] = "omdoc",
			["application/onenote"] = "onepkg onetmp onetoc onetoc2",
			["application/oxps"] = "oxps",
			["application/patch-ops-error+xml"] = "xer",
			["application/pdf"] = "pdf",
			["application/pgp"] = "pgp",
			["application/pgp-encrypted"] = "pgp",
			["application/pgp-keys"] = "pgp",
			["application/pgp-signature"] = "asc sig",
			["application/pics-rules"] = "prf",
			["application/pkcs10"] = "p10",
			["application/pkcs7-mime"] = "p7c p7m",
			["application/pkcs7-signature"] = "p7s",
			["application/pkcs8"] = "p8",
			["application/pkix-attr-cert"] = "ac",
			["application/pkix-cert"] = "cer",
			["application/pkix-crl"] = "crl",
			["application/pkix-pkipath"] = "pkipath",
			["application/pkixcmp"] = "pki",
			["application/pls+xml"] = "pls",
			["application/post"] = "ai eps ps",
			["application/postscript"] = "ai eps ps",
			["application/prs.cww"] = "cww",
			["application/pskc+xml"] = "pskcxml",
			["application/rdf+xml"] = "rdf",
			["application/reginfo+xml"] = "rif",
			["application/relax-ng-compact-syntax"] = "rnc",
			["application/resource-lists+xml"] = "rl",
			["application/resource-lists-diff+xml"] = "rld",
			["application/rls-services+xml"] = "rs",
			["application/rpki-ghostbusters"] = "gbr",
			["application/rpki-manifest"] = "mft",
			["application/rpki-roa"] = "roa",
			["application/rsd+xml"] = "rsd",
			["application/rss+xml"] = "rss",
			["application/rtf"] = "rtf",
			["application/sbml+xml"] = "sbml",
			["application/scvp-cv-request"] = "scq",
			["application/scvp-cv-response"] = "scs",
			["application/scvp-vp-request"] = "spq",
			["application/scvp-vp-response"] = "spp",
			["application/sdp"] = "sdp",
			["application/set-payment-initiation"] = "setpay",
			["application/set-registration-initiation"] = "setreg",
			["application/shf+xml"] = "shf",
			["application/smil+xml"] = "smi smil",
			["application/sparql-query"] = "rq",
			["application/sparql-results+xml"] = "srx",
			["application/srgs"] = "gram",
			["application/srgs+xml"] = "grxml",
			["application/sru+xml"] = "sru",
			["application/ssdl+xml"] = "ssdl",
			["application/ssml+xml"] = "ssml",
			["application/tei+xml"] = "tei teicorpus",
			["application/thraud+xml"] = "tfi",
			["application/timestamped-data"] = "tsd",
			["application/vnd.3gpp.pic-bw-large"] = "plb",
			["application/vnd.3gpp.pic-bw-small"] = "psb",
			["application/vnd.3gpp.pic-bw-var"] = "pvb",
			["application/vnd.3gpp2.tcap"] = "tcap",
			["application/vnd.3m.post-it-notes"] = "pwn",
			["application/vnd.accpac.simply.aso"] = "aso",
			["application/vnd.accpac.simply.imp"] = "imp",
			["application/vnd.acucobol"] = "acu",
			["application/vnd.acucorp"] = "acutc atc",
			["application/vnd.adobe.air-application-installer-package+zip"] = "air",
			["application/vnd.adobe.formscentral.fcdt"] = "fcdt",
			["application/vnd.adobe.fxp"] = "fxp fxpl",
			["application/vnd.adobe.xdp+xml"] = "xdp",
			["application/vnd.adobe.xfdf"] = "xfdf",
			["application/vnd.ahead.space"] = "ahead",
			["application/vnd.airzip.filesecure.azf"] = "azf",
			["application/vnd.airzip.filesecure.azs"] = "azs",
			["application/vnd.amazon.ebook"] = "azw",
			["application/vnd.americandynamics.acc"] = "acc",
			["application/vnd.amiga.ami"] = "ami",
			["application/vnd.android.package-archive"] = "apk",
			["application/vnd.anser-web-certificate-issue-initiation"] = "cii",
			["application/vnd.anser-web-funds-transfer-initiation"] = "fti",
			["application/vnd.antix.game-component"] = "atx",
			["application/vnd.apple.installer+xml"] = "mpkg",
			["application/vnd.apple.mpegurl"] = "m3u8",
			["application/vnd.aristanetworks.swi"] = "swi",
			["application/vnd.astraea-software.iota"] = "iota",
			["application/vnd.audiograph"] = "aep",
			["application/vnd.blueice.multipass"] = "mpm",
			["application/vnd.bmi"] = "bmi",
			["application/vnd.businessobjects"] = "rep",
			["application/vnd.chemdraw+xml"] = "cdxml",
			["application/vnd.chipnuts.karaoke-mmd"] = "mmd",
			["application/vnd.cinderella"] = "cdy",
			["application/vnd.claymore"] = "cla",
			["application/vnd.cloanto.rp9"] = "rp9",
			["application/vnd.clonk.c4group"] = "c4d c4f c4g c4p c4u",
			["application/vnd.cluetrust.cartomobile-config"] = "c11amc",
			["application/vnd.cluetrust.cartomobile-config-pkg"] = "c11amz",
			["application/vnd.commonspace"] = "csp",
			["application/vnd.contact.cmsg"] = "cdbcmsg",
			["application/vnd.cosmocaller"] = "cmc",
			["application/vnd.crick.clicker"] = "clkx",
			["application/vnd.crick.clicker.keyboard"] = "clkk",
			["application/vnd.crick.clicker.palette"] = "clkp",
			["application/vnd.crick.clicker.template"] = "clkt",
			["application/vnd.crick.clicker.wordbank"] = "clkw",
			["application/vnd.criticaltools.wbs+xml"] = "wbs",
			["application/vnd.ctc-posml"] = "pml",
			["application/vnd.cups-ppd"] = "ppd",
			["application/vnd.cups-raster"] = "ppd",
			["application/vnd.curl.car"] = "car",
			["application/vnd.curl.pcurl"] = "pcurl",
			["application/vnd.dart"] = "dart",
			["application/vnd.data-vision.rdz"] = "rdz",
			["application/vnd.dece.data"] = "uvd uvf uvvd uvvf",
			["application/vnd.dece.ttml+xml"] = "uvt uvvt",
			["application/vnd.dece.unspecified"] = "uvvx uvx",
			["application/vnd.dece.zip"] = "uvvz uvz",
			["application/vnd.denovo.fcselayout-link"] = "fe_launch",
			["application/vnd.dna"] = "dna",
			["application/vnd.dolby.mlp"] = "mlp",
			["application/vnd.dpgraph"] = "dpg",
			["application/vnd.dreamfactory"] = "dfac",
			["application/vnd.ds-keypoint"] = "kpxx",
			["application/vnd.dvb.ait"] = "ait",
			["application/vnd.dvb.service"] = "svc",
			["application/vnd.dynageo"] = "geo",
			["application/vnd.ecowin.chart"] = "mag",
			["application/vnd.enliven"] = "nml",
			["application/vnd.epson.esf"] = "esf",
			["application/vnd.epson.msf"] = "msf",
			["application/vnd.epson.quickanime"] = "qam",
			["application/vnd.epson.salt"] = "slt",
			["application/vnd.epson.ssf"] = "ssf",
			["application/vnd.eszigno3+xml"] = "es3 et3",
			["application/vnd.ezpix-album"] = "ez2",
			["application/vnd.ezpix-package"] = "ez3",
			["application/vnd.fdf"] = "fdf",
			["application/vnd.fdsn.mseed"] = "mseed",
			["application/vnd.fdsn.seed"] = "dataless seed",
			["application/vnd.flographit"] = "gph",
			["application/vnd.fluxtime.clip"] = "ftc",
			["application/vnd.font-fontforge-sfd"] = "sfd",
			["application/vnd.framemaker"] = "book fm frame maker",
			["application/vnd.frogans.fnc"] = "fnc",
			["application/vnd.frogans.ltf"] = "ltf",
			["application/vnd.fsc.weblaunch"] = "fsc",
			["application/vnd.fujitsu.oasys"] = "oas",
			["application/vnd.fujitsu.oasys2"] = "oa2",
			["application/vnd.fujitsu.oasys3"] = "oa3",
			["application/vnd.fujitsu.oasysgp"] = "fg5",
			["application/vnd.fujitsu.oasysprs"] = "bh2",
			["application/vnd.fujixerox.ddd"] = "ddd",
			["application/vnd.fujixerox.docuworks"] = "xdw",
			["application/vnd.fujixerox.docuworks.binder"] = "xbd",
			["application/vnd.fuzzysheet"] = "fzs",
			["application/vnd.genomatix.tuxedo"] = "txd",
			["application/vnd.geogebra.file"] = "ggb",
			["application/vnd.geogebra.tool"] = "ggt",
			["application/vnd.geometry-explorer"] = "gex gre",
			["application/vnd.geonext"] = "gxt",
			["application/vnd.geoplan"] = "g2w",
			["application/vnd.geospace"] = "g3w",
			["application/vnd.gmx"] = "gmx",
			["application/vnd.google-earth.kml"] = "xml kml",
			["application/vnd.google-earth.kml+xml"] = "kml",
			["application/vnd.google-earth.kmz"] = "kmz",
			["application/vnd.grafeq"] = "gqf gqs",
			["application/vnd.groove-account"] = "gac",
			["application/vnd.groove-help"] = "ghf",
			["application/vnd.groove-identity-message"] = "gim",
			["application/vnd.groove-injector"] = "grv",
			["application/vnd.groove-tool-message"] = "gtm",
			["application/vnd.groove-tool-template"] = "tpl",
			["application/vnd.groove-vcard"] = "vcg",
			["application/vnd.hal+xml"] = "hal",
			["application/vnd.hancom.hwp"] = "hwp",
			["application/vnd.handheld-entertainment+xml"] = "zmm",
			["application/vnd.hbci"] = "hbci",
			["application/vnd.hhe.lesson-player"] = "les",
			["application/vnd.hp-hpgl"] = "hpgl",
			["application/vnd.hp-hpid"] = "hpid",
			["application/vnd.hp-hps"] = "hps",
			["application/vnd.hp-jlyt"] = "jlt",
			["application/vnd.hp-pcl"] = "pcl",
			["application/vnd.hp-pclxl"] = "pclxl",
			["application/vnd.hydrostatix.sof-data"] = "sfd-hdstx",
			["application/vnd.ibm.minipay"] = "mpy",
			["application/vnd.ibm.modcap"] = "afp",
			["application/vnd.ibm.modcap"] = "list3820",
			["application/vnd.ibm.modcap"] = "listafp",
			["application/vnd.ibm.rights-management"] = "irm",
			["application/vnd.ibm.secure-container"] = "sc",
			["application/vnd.iccprofile"] = "icc",
			["application/vnd.iccprofile"] = "icc icm",
			["application/vnd.iccprofile"] = "icm",
			["application/vnd.igloader"] = "igl",
			["application/vnd.immervision-ivp"] = "ivp",
			["application/vnd.immervision-ivu"] = "ivu",
			["application/vnd.insors.igm"] = "igm",
			["application/vnd.intercon.formnet"] = "xpw xpx",
			["application/vnd.intergeo"] = "i2g",
			["application/vnd.intu.qbo"] = "qbo",
			["application/vnd.intu.qfx"] = "qfx",
			["application/vnd.ipunplugged.rcprofile"] = "rcprofile",
			["application/vnd.irepository.package+xml"] = "irp",
			["application/vnd.is-xpr"] = "xpr",
			["application/vnd.isac.fcs"] = "fcs",
			["application/vnd.jam"] = "jam",
			["application/vnd.jcp.javame.midlet-rms"] = "rms",
			["application/vnd.jisp"] = "jisp",
			["application/vnd.joost.joda-archive"] = "joda",
			["application/vnd.kahootz"] = "ktr ktz",
			["application/vnd.kde.karbon"] = "karbon",
			["application/vnd.kde.kchart"] = "chrt",
			["application/vnd.kde.kformula"] = "kfo",
			["application/vnd.kde.kivio"] = "flw",
			["application/vnd.kde.kontour"] = "kon",
			["application/vnd.kde.kpresenter"] = "kpr kpt",
			["application/vnd.kde.kspread"] = "ksp",
			["application/vnd.kde.kword"] = "kwd kwt",
			["application/vnd.kenameaapp"] = "htke",
			["application/vnd.kidspiration"] = "kia",
			["application/vnd.kinar"] = "kne knp",
			["application/vnd.koan"] = "skd skm skp skt",
			["application/vnd.kodak-descriptor"] = "sse",
			["application/vnd.las.las+xml"] = "lasxml",
			["application/vnd.llamagraphics.life-balance.desktop"] = "lbd",
			["application/vnd.llamagraphics.life-balance.exchange+xml"] = "lbe",
			["application/vnd.lotus-1-2-3"] = "123",
			["application/vnd.lotus-approach"] = "apr",
			["application/vnd.lotus-freelance"] = "pre",
			["application/vnd.lotus-notes"] = "nsf",
			["application/vnd.lotus-organizer"] = "org",
			["application/vnd.lotus-screencam"] = "scm",
			["application/vnd.lotus-wordpro"] = "lwp sam",
			["application/vnd.macports.portpkg"] = "portpkg",
			["application/vnd.mcd"] = "mcd",
			["application/vnd.medcalcdata"] = "mc1",
			["application/vnd.mediastation.cdkey"] = "cdkey",
			["application/vnd.mfer"] = "mwf",
			["application/vnd.mfmp"] = "mfm",
			["application/vnd.micrografx.flo"] = "flo",
			["application/vnd.micrografx.igx"] = "igx",
			["application/vnd.mif"] = "mif",
			["application/vnd.mobius.daf"] = "daf",
			["application/vnd.mobius.dis"] = "dis",
			["application/vnd.mobius.mbk"] = "mbk",
			["application/vnd.mobius.mqy"] = "mqy",
			["application/vnd.mobius.msl"] = "msl",
			["application/vnd.mobius.plc"] = "plc",
			["application/vnd.mobius.txf"] = "txf",
			["application/vnd.mophun.application"] = "mpn",
			["application/vnd.mophun.certificate"] = "mpc",
			["application/vnd.mozilla.xul+xml"] = "xul",
			["application/vnd.ms-artgalry"] = "cil",
			["application/vnd.ms-cab-compressed"] = "cab msu",
			["application/vnd.ms-excel"] = "xla xlc xlm xlw xls xlb xlt xlam xlsb",
			["application/vnd.ms-excel.addin.macroenabled.12"] = "xlam",
			["application/vnd.ms-excel.sheet.binary.macroenabled.12"] = "xlsb",
			["application/vnd.ms-excel.sheet.macroenabled.12"] = "xlsm",
			["application/vnd.ms-excel.template.macroenabled.12"] = "xltm",
			["application/vnd.ms-fontobject"] = "eot",
			["application/vnd.ms-htmlhelp"] = "chm",
			["application/vnd.ms-ims"] = "ims",
			["application/vnd.ms-lrm"] = "lrm",
			["application/vnd.ms-officetheme"] = "thmx",
			["application/vnd.ms-opentype"] = "otf",
			["application/vnd.ms-pki.seccat"] = "cat",
			["application/vnd.ms-pki.stl"] = "stl",
			["application/vnd.ms-powerpoint"] = "pot ppt pps ppam pptm sldm ppsm potm pptx",
			["application/vnd.ms-powerpoint.addin.macroenabled.12"] = "ppam",
			["application/vnd.ms-powerpoint.presentation.macroenabled.12"] = "pptm",
			["application/vnd.ms-powerpoint.slide.macroenabled.12"] = "sldm",
			["application/vnd.ms-powerpoint.slideshow.macroenabled.12"] = "ppsm",
			["application/vnd.ms-powerpoint.template.macroenabled.12"] = "potm",
			["application/vnd.ms-project"] = "mpp mpt",
			["application/vnd.ms-tnef"] = "tnef tnf",
			["application/vnd.ms-word.document.macroenabled.12"] = "docm",
			["application/vnd.ms-word.template.macroenabled.12"] = "dotm",
			["application/vnd.ms-works"] = "wcm wdb wks wps",
			["application/vnd.ms-wpl"] = "wpl",
			["application/vnd.ms-xpsdocument"] = "xps",
			["application/vnd.mseq"] = "mseq",
			["application/vnd.musician"] = "mus",
			["application/vnd.muvee.style"] = "msty",
			["application/vnd.mynfc"] = "taglet",
			["application/vnd.neurolanguage.nlu"] = "nlu",
			["application/vnd.nitf"] = "nitf ntf",
			["application/vnd.noblenet-directory"] = "nnd",
			["application/vnd.noblenet-sealer"] = "nns",
			["application/vnd.noblenet-web"] = "nnw",
			["application/vnd.nokia.n-gage.data"] = "ngdat",
			["application/vnd.nokia.n-gage.symbian.install"] = "n-gage",
			["application/vnd.nokia.radio-preset"] = "rpst",
			["application/vnd.nokia.radio-presets"] = "rpss",
			["application/vnd.novadigm.edm"] = "edm",
			["application/vnd.novadigm.edx"] = "edx",
			["application/vnd.novadigm.ext"] = "ext",
			["application/vnd.oasis.opendocument.chart"] = "odc",
			["application/vnd.oasis.opendocument.chart-template"] = "otc",
			["application/vnd.oasis.opendocument.database"] = "odb",
			["application/vnd.oasis.opendocument.formula"] = "odf",
			["application/vnd.oasis.opendocument.formula-template"] = "odf odft",
			["application/vnd.oasis.opendocument.graphics"] = "odg",
			["application/vnd.oasis.opendocument.graphics-template"] = "otg",
			["application/vnd.oasis.opendocument.image"] = "odi",
			["application/vnd.oasis.opendocument.image-template"] = "oti",
			["application/vnd.oasis.opendocument.presentation"] = "odp",
			["application/vnd.oasis.opendocument.presentation-template"] = "otp",
			["application/vnd.oasis.opendocument.spreadsheet"] = "ods",
			["application/vnd.oasis.opendocument.spreadsheet-template"] = "ots",
			["application/vnd.oasis.opendocument.text"] = "odt",
			["application/vnd.oasis.opendocument.text-master"] = "odm",
			["application/vnd.oasis.opendocument.text-template"] = "ott",
			["application/vnd.oasis.opendocument.text-web"] = "oth",
			["application/vnd.olpc-sugar"] = "xo",
			["application/vnd.oma.dd2+xml"] = "dd2",
			["application/vnd.openofficeorg.extension"] = "oxt",
			["application/vnd.openxmlformats-officedocument.presentationml.presentation"] = "ppt pptm pptx",
			["application/vnd.openxmlformats-officedocument.presentationml.slide"] = "sldx",
			["application/vnd.openxmlformats-officedocument.presentationml.slideshow"] = "ppsx",
			["application/vnd.openxmlformats-officedocument.presentationml.template"] = "potx",
			["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = "xlsx",
			["application/vnd.openxmlformats-officedocument.spreadsheetml.template"] = "xltx",
			["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = "docx",
			["application/vnd.openxmlformats-officedocument.wordprocessingml.template"] = "dotx",
			["application/vnd.osgeo.mapguide.package"] = "mgp",
			["application/vnd.osgi.dp"] = "dp",
			["application/vnd.osgi.subsystem"] = "esa",
			["application/vnd.palm"] = "oprc pdb pqa",
			["application/vnd.pawaafile"] = "paw",
			["application/vnd.pg.format"] = "str",
			["application/vnd.pg.osasli"] = "ei6",
			["application/vnd.picsel"] = "efif",
			["application/vnd.pmi.widget"] = "wg",
			["application/vnd.pocketlearn"] = "plf",
			["application/vnd.powerbuilder6"] = "pbd",
			["application/vnd.previewsystems.box"] = "box",
			["application/vnd.proteus.magazine"] = "mgz",
			["application/vnd.publishare-delta-tree"] = "qps",
			["application/vnd.pvi.ptid1"] = "ptid",
			["application/vnd.quark.quarkxpress"] = "qwd qwt qxb qxd qxl qxt",
			["application/vnd.realvnc.bed"] = "bed",
			["application/vnd.recordare.musicxml"] = "mxl",
			["application/vnd.recordare.musicxml+xml"] = "musicxml",
			["application/vnd.rig.cryptonote"] = "cryptonote",
			["application/vnd.rim.cod"] = "cod",
			["application/vnd.rn-realmedia"] = "rm",
			["application/vnd.rn-realmedia-vbr"] = "rmvb",
			["application/vnd.route66.link66+xml"] = "link66",
			["application/vnd.sailingtracker.track"] = "st",
			["application/vnd.seemail"] = "see",
			["application/vnd.sema"] = "sema",
			["application/vnd.semd"] = "semd",
			["application/vnd.semf"] = "semf",
			["application/vnd.shana.informed.formdata"] = "ifm",
			["application/vnd.shana.informed.formtemplate"] = "itp",
			["application/vnd.shana.informed.interchange"] = "iif",
			["application/vnd.shana.informed.package"] = "ipk",
			["application/vnd.simtech-mindmapper"] = "twd twds",
			["application/vnd.smaf"] = "mmf",
			["application/vnd.smart.teacher"] = "teacher",
			["application/vnd.solent.sdkm+xml"] = "sdkd sdkm",
			["application/vnd.spotfire.dxp"] = "dxp",
			["application/vnd.spotfire.sfs"] = "sfs",
			["application/vnd.stardivision.calc"] = "sdc",
			["application/vnd.stardivision.draw"] = "sda",
			["application/vnd.stardivision.impress"] = "sdd",
			["application/vnd.stardivision.math"] = "smf",
			["application/vnd.stardivision.writer"] = "sdw vor",
			["application/vnd.stardivision.writer-global"] = "sgl",
			["application/vnd.stepmania.package"] = "smzip",
			["application/vnd.stepmania.stepchart"] = "sm",
			["application/vnd.sun.xml.calc"] = "sxc",
			["application/vnd.sun.xml.calc.template"] = "stc",
			["application/vnd.sun.xml.draw"] = "sxd",
			["application/vnd.sun.xml.draw.template"] = "std",
			["application/vnd.sun.xml.impress"] = "sxi",
			["application/vnd.sun.xml.impress.template"] = "sti",
			["application/vnd.sun.xml.math"] = "sxm",
			["application/vnd.sun.xml.writer"] = "sxw",
			["application/vnd.sun.xml.writer.global"] = "sxg",
			["application/vnd.sun.xml.writer.template"] = "stw",
			["application/vnd.sus-calendar"] = "sus susp",
			["application/vnd.svd"] = "svd",
			["application/vnd.symbian.install"] = "sis sisx",
			["application/vnd.syncml+xml"] = "xsm",
			["application/vnd.syncml.dm+wbxml"] = "bdm",
			["application/vnd.syncml.dm+xml"] = "xdm",
			["application/vnd.tao.intent-module-archive"] = "tao",
			["application/vnd.tcpdump.pcap"] = "cap dmp pcap",
			["application/vnd.tmobile-livetv"] = "tmo",
			["application/vnd.trid.tpt"] = "tpt",
			["application/vnd.triscape.mxs"] = "mxs",
			["application/vnd.trueapp"] = "tra",
			["application/vnd.ufdl"] = "ufd ufdl",
			["application/vnd.uiq.theme"] = "utz",
			["application/vnd.umajin"] = "umj",
			["application/vnd.unity"] = "unityweb",
			["application/vnd.uoml+xml"] = "uoml",
			["application/vnd.vcx"] = "vcx",
			["application/vnd.visio"] = "vsd vss vst vsw",
			["application/vnd.visionary"] = "vis",
			["application/vnd.vsf"] = "vsf",
			["application/vnd.wap.wbxml"] = "wbxml",
			["application/vnd.wap.wmlc"] = "wmlc",
			["application/vnd.wap.wmlscriptc"] = "wmlsc",
			["application/vnd.webturbo"] = "wtb",
			["application/vnd.wolfram.player"] = "nbp",
			["application/vnd.wordperfect"] = "wpd",
			["application/vnd.wqd"] = "wqd",
			["application/vnd.wt.stf"] = "stf",
			["application/vnd.xara"] = "xar",
			["application/vnd.xfdl"] = "xfdl",
			["application/vnd.yamaha.hv-dic"] = "hvd",
			["application/vnd.yamaha.hv-script"] = "hvs",
			["application/vnd.yamaha.hv-voice"] = "hvp",
			["application/vnd.yamaha.openscoreformat"] = "osf",
			["application/vnd.yamaha.openscoreformat.osfpvg+xml"] = "osfpvg",
			["application/vnd.yamaha.smaf-audio"] = "saf",
			["application/vnd.yamaha.smaf-phrase"] = "spf",
			["application/vnd.yellowriver-custom-menu"] = "cmp",
			["application/vnd.zul"] = "zir zirz",
			["application/vnd.zzazz.deck+xml"] = "zaz",
			["application/voicexml+xml"] = "vxml",
			["application/warc"] = "warc",
			["application/widget"] = "wgt",
			["application/winhlp"] = "hlp",
			["application/wsdl+xml"] = "wsdl",
			["application/wspolicy+xml"] = "wspolicy",
			["application/x-123"] = "123 wk",
			["application/x-7z-compressed"] = "7z",
			["application/x-abiword"] = "abw",
			["application/x-ace-compressed"] = "ace",
			["application/x-apple-diskimage"] = "dmg",
			["application/x-arc"] = "arc",
			["application/x-archive"] = "ar",
			["application/x-arj"] = "arj",
			["application/x-authorware-bin"] = "aab u32 vox x32",
			["application/x-authorware-map"] = "aam",
			["application/x-authorware-seg"] = "aas",
			["application/x-bcpio"] = "bcpio",
			["application/x-bittorrent"] = "torrent",
			["application/x-blorb"] = "blb blorb",
			["application/x-bzip"] = "bz",
			["application/x-bzip2"] = "boz bz2",
			["application/x-cbr"] = "cb7 cba cbr cbt cbz",
			["application/x-cdlink"] = "vcd",
			["application/x-cfs-compressed"] = "cfs",
			["application/x-chat"] = "chat",
			["application/x-chess-pgn"] = "pgn",
			["application/x-compress"] = "z",
			["application/x-conference"] = "nsc",
			["application/x-coredump"] = "core",
			["application/x-cpio"] = "cpio",
			["application/x-csh"] = "csh",
			["application/x-dbm"] = "dbm",
			["application/x-debian-package"] = "deb udeb",
			["application/x-dgc-compressed"] = "dgc",
			["application/x-director"] = "cct cst cxt dcr dir dxr fgd swa w3d",
			["application/x-doom"] = "wad",
			["application/x-dosexec"] = "exe",
			["application/x-dtbncx+xml"] = "ncx",
			["application/x-dtbook+xml"] = "dtb",
			["application/x-dtbresource+xml"] = "res",
			["application/x-dvi"] = "dvi",
			["application/x-eet"] = "eet",
			["application/x-elc"] = "elc",
			["application/x-envoy"] = "evy",
			["application/x-epoc-app"] = "app",
			["application/x-epoc-opl"] = "opl",
			["application/x-epoc-opo"] = "opo",
			["application/x-epoc-sheet"] = "xls",
			["application/x-epoc-word"] = "doc",
			["application/x-eva"] = "eva",
			["application/x-executable"] = "exe",
			["application/x-font-bdf"] = "bdf",
			["application/x-font-ghostscript"] = "gsf",
			["application/x-font-linux-psf"] = "psf",
			["application/x-font-pcf"] = "pcf",
			["application/x-font-snf"] = "snf",
			["application/x-font-ttf"] = "ttf",
			["application/x-font-type1"] = "afm pfa pfb pfm",
			["application/x-freearc"] = "arc",
			["application/x-freemind"] = "mm",
			["application/x-futuresplash"] = "spl",
			["application/x-gca-compressed"] = "gca",
			["application/x-gdbm"] = "dbm",
			["application/x-glulx"] = "ulx",
			["application/x-gnucash"] = "gnucash",
			["application/x-gnumeric"] = "gnm gnumeric",
			["application/x-gnupg-keyring"] = "gpg",
			["application/x-gramps-xml"] = "gramps",
			["application/x-gtar"] = "gtar",
			["application/x-gzip"] = "gz tgz",
			["application/x-hdf"] = "hdf",
			["application/x-hwp"] = "hwp",
			["application/x-ia-arc"] = "arc",
			["application/x-ichitaro4"] = "jtd",
			["application/x-ichitaro5"] = "jtd",
			["application/x-ichitaro6"] = "jtd",
			["application/x-install-instructions"] = "install",
			["application/x-iso9660-image"] = "iso",
			["application/x-java-applet"] = "class",
			["application/x-java-jce-keystore"] = "jks",
			["application/x-java-jnlp-file"] = "jnlp",
			["application/x-java-keystore"] = "jks",
			["application/x-java-pack200"] = "pack",
			["application/x-kdelnk"] = "kdelnk",
			["application/x-latex"] = "latex",
			["application/x-lha"] = "lha",
			["application/x-lharc"] = "sfx",
			["application/x-lrzip"] = "lrz",
			["application/x-lzh-compressed"] = "lha lzh",
			["application/x-lzip"] = "lz",
			["application/x-lzma"] = "lzma",
			["application/x-mie"] = "mie",
			["application/x-mif"] = "mif",
			["application/x-mobipocket-ebook"] = "mobi",
			["application/x-mobipocket-ebook"] = "prc",
			["application/x-ms-application"] = "application",
			["application/x-ms-reader"] = "",
			["application/x-ms-shortcut"] = "lnk",
			["application/x-ms-wmd"] = "wmd",
			["application/x-ms-wmz"] = "wmz",
			["application/x-ms-xbap"] = "xbap",
			["application/x-msaccess"] = "mdb",
			["application/x-msbinder"] = "obd",
			["application/x-mscardfile"] = "crd",
			["application/x-msclip"] = "clp",
			["application/x-msdownload"] = "bat com dll exe msi",
			["application/x-msmediaview"] = "m13 m14 mvb",
			["application/x-msmetafile"] = "emf emz wmf",
			["application/x-msmoney"] = "mny",
			["application/x-mspublisher"] = "pub",
			["application/x-msschedule"] = "scd",
			["application/x-msterminal"] = "trm",
			["application/x-mswrite"] = "wri",
			["application/x-netcdf"] = "cdf nc",
			["application/x-nzb"] = "nzb",
			["application/x-object"] = "obj",
			["application/x-pgp-keyring"] = "pgp",
			["application/x-pkcs12"] = "p12 pfx",
			["application/x-pkcs7-certificates"] = "p7b spc",
			["application/x-pkcs7-certreqresp"] = "p7r",
			["application/x-quark-xpress-3"] = "qxp",
			["application/x-quicktime-player"] = "qtl",
			["application/x-rar"] = "rar",
			["application/x-rar-compressed"] = "rar",
			["application/x-research-info-systems"] = "ris",
			["application/x-rpm"] = "rpm",
			["application/x-sc"] = "sc",
			["application/x-scribus"] = "scd",
			["application/x-setup"] = "xnf",
			["application/x-setupscript"] = "xnf",
			["application/x-sh"] = "sh",
			["application/x-shar"] = "shar",
			["application/x-sharedlib"] = "so",
			["application/x-shockwave-flash"] = "swf swf swfl",
			["application/x-silverlight-app"] = "xap",
			["application/x-sql"] = "sql",
			["application/x-stuffit"] = "sit",
			["application/x-stuffitx"] = "sitx",
			["application/x-subrip"] = "srt",
			["application/x-sv4cpio"] = "sv4cpio",
			["application/x-sv4crc"] = "sv4crc",
			["application/x-svr4-package"] = "pkg",
			["application/x-t3vm-image"] = "t3",
			["application/x-tads"] = "gam",
			["application/x-tar"] = "tar",
			["application/x-tcl"] = "tcl",
			["application/x-tex"] = "tex",
			["application/x-tex-tfm"] = "tfm",
			["application/x-texinfo"] = "texi texinfo",
			["application/x-tgif"] = "obj",
			["application/x-tokyocabinet-btree"] = "kch",
			["application/x-tokyocabinet-fixed"] = "kch",
			["application/x-tokyocabinet-hash"] = "kch",
			["application/x-tokyocabinet-table"] = "kch",
			["application/x-ustar"] = "ustar",
			["application/x-wais-source"] = "src",
			["application/x-x509-ca-cert"] = "crt der",
			["application/x-xfig"] = "fig",
			["application/x-xliff+xml"] = "xlf",
			["application/x-xpinstall"] = "xpi",
			["application/x-xz"] = "xz",
			["application/x-zip-compressed"] = "zip",
			["application/x-zmachine"] = "z1 z2 z3 z4 z5 z6 z7 z8",
			["application/x-zoo"] = "zoo",
			["application/xaml+xml"] = "xaml",
			["application/xcap-diff+xml"] = "xdf",
			["application/xenc+xml"] = "xenc",
			["application/xhtml+xml"] = "xht xhtml",
			["application/xml"] = "xml xsl",
			["application/xml-dtd"] = "dtd",
			["application/xml-sitemap"] = "xml",
			["application/xop+xml"] = "xop",
			["application/xproc+xml"] = "xpl",
			["application/xslt+xml"] = "xslt",
			["application/xspf+xml"] = "xspf",
			["application/xv+xml"] = "mxml xhvml xvm xvml",
			["application/yang"] = "yang",
			["application/yin+xml"] = "yin",
			["application/zip"] = "war zip",
			["audio/adpcm"] = "adp",
			["audio/basic"] = "au snd",
			["audio/midi"] = "kar mid midi rmi",
			["audio/mp4"] = "m4a mp4 mp4a",
			["audio/mpeg"] = "m2a m3a mp2 mp2a mp3 mpeg mpga",
			["audio/ogg"] = "oga ogg spx",
			["audio/s3m"] = "s3m",
			["audio/silk"] = "sil",
			["audio/vnd.dece.audio"] = "uva uvva",
			["audio/vnd.digital-winds"] = "eol",
			["audio/vnd.dra"] = "dra",
			["audio/vnd.dts"] = "dts",
			["audio/vnd.dts.hd"] = "dtshd",
			["audio/vnd.lucent.voice"] = "lvp",
			["audio/vnd.ms-playready.media.pya"] = "pya",
			["audio/vnd.nuera.ecelp4800"] = "ecelp4800",
			["audio/vnd.nuera.ecelp7470"] = "ecelp7470",
			["audio/vnd.nuera.ecelp9600"] = "ecelp9600",
			["audio/vnd.rip"] = "rip",
			["audio/webm"] = "weba",
			["audio/x-aac"] = "aac",
			["audio/x-adpcm"] = "pcm",
			["audio/x-aiff"] = "aif aifc aiff",
			["audio/x-ape"] = "ape",
			["audio/x-caf"] = "caf",
			["audio/x-dec-basic"] = "au",
			["audio/x-flac"] = "fla flac",
			["audio/x-hx-aac-adif"] = "aac",
			["audio/x-hx-aac-adts"] = "aac",
			["audio/x-matroska"] = "mka",
			["audio/x-mod"] = "mod",
			["audio/x-mp4a-latm"] = "mp4a",
			["audio/x-mpegurl"] = "m3u",
			["audio/x-ms-wax"] = "wax",
			["audio/x-ms-wma"] = "wma",
			["audio/x-pn-realaudio"] = "ra ram",
			["audio/x-pn-realaudio-plugin"] = "rmp",
			["audio/x-w64"] = "w64",
			["audio/x-wav"] = "wav",
			["audio/xm"] = "xm",
			["chemical/x-cdx"] = "cdx",
			["chemical/x-cif"] = "cif",
			["chemical/x-cmdf"] = "cmdf",
			["chemical/x-cml"] = "cml",
			["chemical/x-csml"] = "csml",
			["chemical/x-pdb"] = "pdb ent",
			["chemical/x-xyz"] = "xyz",
			["font/collection"] = "ttc",
			["font/otf"] = "otf",
			["font/ttf"] = "ttf",
			["font/woff"] = "woff",
			["font/woff2"] = "woff2",
			["image/bmp"] = "bmp",
			["image/cgm"] = "cgm",
			["image/g3fax"] = "g3",
			["image/gif"] = "gif",
			["image/ief"] = "ief",
			["image/jp2"] = "jp2 jk2",
			["image/jpeg"] = "jpe jpeg jpg",
			["image/ktx"] = "ktx",
			["image/png"] = "png",
			["image/ppp"] = "aaz kkk ppp",
			["image/prs.btif"] = "btif",
			["image/sgi"] = "sgi",
			["image/svg"] = "xml svg svgz",
			["image/svg+xml"] = "svg svgz",
			["image/tiff"] = "tif tiff",
			["image/vnd.adobe.photoshop"] = "psd",
			["image/vnd.dece.graphic"] = "uvg uvi uvvg uvvi",
			["image/vnd.djvu"] = "djvu djv",
			["image/vnd.dvb.subtitle"] = "sub",
			["image/vnd.dwg"] = "dwg",
			["image/vnd.dxf"] = "dxf",
			["image/vnd.fastbidsheet"] = "fbs",
			["image/vnd.fpx"] = "fpx",
			["image/vnd.fst"] = "fst",
			["image/vnd.fujixerox.edmics-mmr"] = "mmr",
			["image/vnd.fujixerox.edmics-rlc"] = "rlc",
			["image/vnd.ms-modi"] = "mdi",
			["image/vnd.ms-photo"] = "wdp",
			["image/vnd.net-fpx"] = "npx",
			["image/vnd.wap.wbmp"] = "wbmp",
			["image/vnd.xiff"] = "xif",
			["image/webp"] = "webp",
			["image/x-3ds"] = "3ds",
			["image/x-canon-cr2"] = "cr2",
			["image/x-canon-crw"] = "crw",
			["image/x-cmu-raster"] = "ras",
			["image/x-cmx"] = "cmx",
			["image/x-coreldraw"] = "cdr",
			["image/x-cpi"] = "cpi",
			["image/x-epoc-mbm"] = "mbm",
			["image/x-epoc-sketch"] = "Sketch sketch",
			["image/x-freehand"] = "fh fh4 fh5 fh7 fhc",
			["image/x-icon"] = "ico",
			["image/x-mrsid-image"] = "sid",
			["image/x-ms-bmp"] = "bmp",
			["image/x-niff"] = "niff nif",
			["image/x-olympus-orf"] = "orf",
			["image/x-paintnet"] = "pdn tga",
			["image/x-pcx"] = "pcx",
			["image/x-pict"] = "pct pic",
			["image/x-portable-anymap"] = "pnm",
			["image/x-portable-bitmap"] = "pbm",
			["image/x-portable-graymap"] = "pgm",
			["image/x-portable-greymap"] = "pgm",
			["image/x-portable-pixmap"] = "ppm",
			["image/x-quicktime"] = "qt mov",
			["image/x-rgb"] = "rgb",
			["image/x-tga"] = "tga",
			["image/x-unknown"] = "",
			["image/x-x3f"] = "x3f",
			["image/x-xbitmap"] = "xbm",
			["image/x-xcf"] = "xcf",
			["image/x-xcursor"] = "",
			["image/x-xpixmap"] = "xpm",
			["image/x-xpmi"] = "xpm",
			["image/x-xwindowdump"] = "xwd",
			["message/news"] = "news",
			["message/rfc822"] = "eml mail art hdf mime",
			["model/iges"] = "iges igs",
			["model/mesh"] = "mesh msh silo",
			["model/vnd.collada+xml"] = "dae",
			["model/vnd.dwf"] = "dwf",
			["model/vnd.gdl"] = "gdl",
			["model/vnd.gtw"] = "gtw",
			["model/vnd.mts"] = "mts",
			["model/vnd.vtu"] = "vtu",
			["model/vrml"] = "wrl vrml",
			["model/x3d"] = "x3d x3db x3dv",
			["model/x3d+binary"] = "x3db x3dbz",
			["model/x3d+vrml"] = "x3dv x3dvz",
			["model/x3d+xml"] = "x3d x3dz",
			["text/PGP"] = "pgp",
			["text/cache-manifest"] = "appcache",
			["text/calendar"] = "ics icalendar ifb",
			["text/css"] = "css",
			["text/csv"] = "csv",
			["text/html"] = "htm html hcdf eml txt",
			["text/n3"] = "n3",
			["text/plain"] = "conf",
			["text/plain"] = "txt text ini pcdf csv def in list log",
			["text/prs.lines.tag"] = "dsc",
			["text/richtext"] = "rtx",
			["text/rtf"] = "rtf",
			["text/sgml"] = "sgm sgml",
			["text/tab-separated-values"] = "tsv",
			["text/texmacs"] = "fsf",
			["text/troff"] = "man me ms roff",
			["text/troff"] = "t tr troff",
			["text/turtle"] = "ttl",
			["text/uri-list"] = "uri uris urls",
			["text/vcard"] = "vcard",
			["text/vnd.curl"] = "curl",
			["text/vnd.curl.dcurl"] = "dcurl",
			["text/vnd.curl.mcurl"] = "mcurl",
			["text/vnd.curl.scurl"] = "scurl",
			["text/vnd.fly"] = "fly",
			["text/vnd.fmi.flexstor"] = "flx",
			["text/vnd.graphviz"] = "gv",
			["text/vnd.in3d.3dml"] = "3dml",
			["text/vnd.in3d.spot"] = "spot",
			["text/vnd.sun.j2me.app-descriptor"] = "jad",
			["text/vnd.wap.wml"] = "wml",
			["text/vnd.wap.wmlscript"] = "wmls",
			["text/x-asm"] = "asm s",
			["text/x-awk"] = "awk",
			["text/x-bcpl"] = "",
			["text/x-c"] = "c cc cpp cxx dic h hh",
			["text/x-c++"] = "cpp",
			["text/x-csv"] = "csv",
			["text/x-diff"] = "diff",
			["text/x-fortran"] = "f f77 f90 for",
			["text/x-gawk"] = "awk",
			["text/x-info"] = "info",
			["text/x-java"] = "java",
			["text/x-java-source"] = "java",
			["text/x-lisp"] = "lisp",
			["text/x-lua"] = "lua",
			["text/x-m4"] = "m4",
			["text/x-makefile"] = "makefile",
			["text/x-msdos-batch"] = "bat",
			["text/x-nawk"] = "awk",
			["text/x-nfo"] = "nfo",
			["text/x-opml"] = "opml",
			["text/x-pascal"] = "p pas",
			["text/x-perl"] = "perl",
			["text/x-php"] = "php",
			["text/x-pod"] = "pod",
			["text/x-python"] = "py",
			["text/x-ruby"] = "rudy",
			["text/x-setext"] = "etx",
			["text/x-sfv"] = "sfv",
			["text/x-shell"] = "sh",
			["text/x-shellscript"] = "sh",
			["text/x-tcl"] = "tcl",
			["text/x-tex"] = "tex ltx sty cls",
			["text/x-texinfo"] = "texi",
			["text/x-uuencode"] = "uu",
			["text/x-vcalendar"] = "vcs",
			["text/x-vcard"] = "vcf",
			["text/x-xmcd"] = "xmcd",
			["video/3gpp"] = "3gp",
			["video/3gpp2"] = "3g2",
			["video/h261"] = "h261",
			["video/h263"] = "h263",
			["video/h264"] = "h264",
			["video/jpeg"] = "jpgv",
			["video/jpm"] = "jpgm jpm",
			["video/mj2"] = "mj2 mjp2",
			["video/mp2p"] = "mp2",
			["video/mp2t"] = "ts",
			["video/mp4"] = "mp4 mp4v mpg4",
			["video/mp4v-es"] = "mp4v",
			["video/mpeg"] = "m1v m2v mpeg mpg mpe mpg",
			["video/mpeg4-generic"] = "mpeg mpg mpe",
			["video/mpv"] = "mpv",
			["video/ogg"] = "ogv",
			["video/quicktime"] = "qt mov",
			["video/vnd.dece.hd"] = "uvh uvvh",
			["video/vnd.dece.mobile"] = "uvm uvvm",
			["video/vnd.dece.pd"] = "uvp uvvp",
			["video/vnd.dece.sd"] = "uvs uvvs",
			["video/vnd.dece.video"] = "uvv uvvv",
			["video/vnd.dvb.file"] = "dvb",
			["video/vnd.fvt"] = "fvt",
			["video/vnd.mpegurl"] = "m4u mxu",
			["video/vnd.ms-playready.media.pyv"] = "pyv",
			["video/vnd.uvvu.mp4"] = "uvu uvvu",
			["video/vnd.vivo"] = "viv",
			["video/webm"] = "webm",
			["video/x-f4v"] = "f4v",
			["video/x-flc"] = "flc",
			["video/x-fli"] = "fli",
			["video/x-flv"] = "flv",
			["video/x-jng"] = "jng",
			["video/x-m4v"] = "m4v",
			["video/x-matroska"] = "mk3d mks mkv mpv",
			["video/x-mng"] = "mng",
			["video/x-ms-asf"] = "asf asx",
			["video/x-ms-vob"] = "vob",
			["video/x-ms-wm"] = "wm",
			["video/x-ms-wmv"] = "wmv",
			["video/x-ms-wmx"] = "wmx",
			["video/x-ms-wvx"] = "wvx",
			["video/x-msvideo"] = "avi",
			["video/x-sgi-movie"] = "movie",
			["video/x-smv"] = "smv",
			["x-conference/x-cooltalk"] = "ice",
			["x-epoc/x-sisx-app"] = "sisx",
		});

        public static bool IsValidMimeAndExtension(string mime, string fileName)
        {
            string fileExt = fileName;
            var ind = fileExt.LastIndexOf('.');
            if (ind != -1 && fileExt.Length > ind + 1)
            {
                fileExt = fileName.Substring(ind + 1).ToLower();
            }

            if (gMimeTypeMap.Value.TryGetValue(mime, out string result))
            {
				Log.Debug("IsValidMimeAndExtension, Get Extensions[{0}] for Mime[{1}]", result, mime);
				string [] exts = result.Split(' ');
				foreach( var ext in exts ) {
					if ( string.Compare(fileExt, ext) == 0) return true;	
				}
			}
            return false;
        }

        /**
        * @breif 파일확장자 및 MimeType 정보 등록 및 갱신
        * @param strMime: 확장자의 Mime 정보 
        * @param strExt : 확장자 
        */
        public void AddOrUpdate(string strMime, string strExt)
		{
            if (gMimeTypeMap.Value.TryGetValue(strMime, out string result))
            {
				gMimeTypeMap.Value[strMime] = gMimeTypeMap.Value[strMime] + " " + strExt;		
			}
			else
			{
				gMimeTypeMap.Value[strMime] = strExt;		
			}
		}

        public void AddDataForInnerZip(int nErrCount, string strOrgZipFile, string strOrgZipFileRelativePath, string strErrFileName, eFileAddErr enErr)
        {
			if (nErrCount == 1) AddData(strOrgZipFile, eFileAddErr.eFAZIP, strOrgZipFileRelativePath);
            AddData(strErrFileName, enErr, strOrgZipFile, true);
        }	

		public async Task<int> CheckZipFile(HsStream hsStream, bool blWhite, string strExtInfo,FileExamEvent SGFileExamEvent, int ExamCount, int TotalCount,int nMaxDepth = 3, int nOption = 0, bool blAllowDRM = true)
		{
			int nTotalErrCount = 0;
			eFileAddErr enRet;
			
			Stream stStream;
			FileInfo fiZipFile;

			string strOrgZipFile;
			string strTempZipPath;
			string strExtractTempZipPath;
			string strOrgZipFileRelativePath;
			string strOverMaxDepthInnerZipFile;
			string strZipFile;
		
			// Setting Default Value
			stStream = hsStream.stream;

			strTempZipPath = "Temp";
			strExtractTempZipPath = Path.Combine(strTempZipPath, "ZipExtract");
			strZipFile = Path.Combine(strTempZipPath, Path.GetFileName(hsStream.FileName));
			strOrgZipFile = hsStream.FileName;
			strOrgZipFileRelativePath  = hsStream.RelativePath;

			// Create Temp Directory 
			DirectoryInfo dirZipBase = new DirectoryInfo(strTempZipPath);
			if (dirZipBase.Exists != true)
			{
				dirZipBase.Create();
			}
			
			Log.Information("[CheckZipFile] ZipFile[{0}] Ext[WhiteF({1})-Info({2})] ZipCheck[MaxDepth({3})-BlockOption({4})] AllowDrmF[{5}]",
				 Path.GetFileName(hsStream.FileName), blWhite, strExtInfo, nMaxDepth, nOption, blAllowDRM);

			// Zip File Create and Scan 
			using (var fileStream = new FileStream(strZipFile, FileMode.Create, FileAccess.Write))
			{
				await stStream.CopyToAsync(fileStream);
				fileStream.Close();

				enRet = ScanZipFile(strOrgZipFile, strOrgZipFileRelativePath, strZipFile, strExtractTempZipPath, 3, 1, blWhite, strExtInfo, 0, 
					out nTotalErrCount, out strOverMaxDepthInnerZipFile, blAllowDRM, SGFileExamEvent, ExamCount,TotalCount);
				if (enRet == eFileAddErr.eFANone && nOption == 0 && nTotalErrCount == 0 && String.IsNullOrEmpty(strOverMaxDepthInnerZipFile) == false)
				{
					enRet = eFileAddErr.eUnZipInnerLeftZip;
					AddDataForInnerZip(nTotalErrCount, strOrgZipFile, strOrgZipFileRelativePath, strOverMaxDepthInnerZipFile, enRet);
				}
				if (enRet == eFileAddErr.eFAZipPW) AddData(strOrgZipFile, enRet, strOrgZipFileRelativePath);
				
				try
				{
					Directory.Delete(strExtractTempZipPath, true);
				}
				catch (System.Exception err)
				{
					Log.Warning("[CheckZipFile] Directory.Delete() " + err.Message + " " + err.GetType().FullName);
				}

				fiZipFile = new FileInfo(strZipFile);
				fiZipFile.Delete();
			}

			stStream.Position = 0;
			if (enRet == eFileAddErr.eFANone && nTotalErrCount == 0) return 0;
			
			return -1;
		}

		public eFileAddErr ScanZipFile(string strOrgZipFile, string strOrgZipFileRelativePath, string strZipFile, string strBasePath, int nMaxDepth, int nCurDepth, 
			bool blWhite, string strExtInfo, int nErrCount, out int nTotalErrCount, out string strOverMaxDepthInnerZipFile, bool blAllowDRM, FileExamEvent SGFileExamEvent, int ExamCount, int TotalCount)
		{
			eFileAddErr enErr;
			string strExt;
			int nCurErrCount;
			string strOverMaxDepthZipFile = "";

			enErr = eFileAddErr.eFANone;
			nCurErrCount = nErrCount;
			try
			{
				using (var archive = ArchiveFactory.Open(strZipFile))
				{
					foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
					{
						Log.Debug("[ScanZipFile] Check File[{0}] in {1}", entry.Key, Path.GetFileName(strZipFile));
						int per = (ExamCount * 100) / TotalCount;
						if (per < 20)
							per = 20;
						if(SGFileExamEvent!=null)
							SGFileExamEvent(per, entry.Key);
						// Check Password	
						if (entry.IsEncrypted == true)
						{
							if (nCurDepth != 1)
							{
								enErr = eFileAddErr.eUnZipInnerZipPassword;
								AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(strZipFile), enErr);
							}
							else
							{
								enErr = eFileAddErr.eFAZipPW;
								nCurErrCount++;
							}
							nTotalErrCount = nCurErrCount;
							strOverMaxDepthInnerZipFile = strOverMaxDepthZipFile;
							return enErr;
						}
						
						// Check FileName Length 
						bool bSuper = false;
						if (nCurDepth == 1 && (FileFolderNameLength(entry.Key, out bSuper) != true || FilePathLength(entry.Key) != true))
						{
							enErr = eFileAddErr.eUnZipInnerLengthOver;
							AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, entry.Key, enErr);
							continue;
						}	
                        
						// Check Directory 
						if (entry.IsDirectory == true) continue;

						// Check Empty File 
						if (entry.Size <= 0)
						{
							enErr = eFileAddErr.eUnZipInnerFileEmpty;
							AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr);
							continue;
						}

						// Check Block File Extension
						strExt = Path.GetExtension(entry.Key);
                        if (GetRegExtEnable(blWhite, strExtInfo, strExt.Replace(".", "")) != true)
                        {
                            enErr = eFileAddErr.eUnZipInnerExt;
							AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr);
                            continue;
                        }

						// Extract File in Zip 
                        entry.WriteToDirectory(strBasePath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });

						// Check Changed File Extension 
                        enErr = IsValidFileExtInnerZip(Path.Combine(strBasePath, entry.Key), strExt.Replace(".", ""), blAllowDRM);
                        if (enErr != eFileAddErr.eFANone)    
                        {
							AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr);
                            continue;
                        }
                        
						// Check Zip File 
                        if ((String.Compare(strExt, ".zip") != 0) && (String.Compare(strExt, ".7z") != 0)) continue;

                        if (nCurDepth >= nMaxDepth)
                        {
							Log.Information("[ScanZipFile] Skip to check zip file[{0}]. MaxDepth[{1}] CurDepth[{2}] Password Zip File[{CurZipFile}] in {OrgZipFile}", nMaxDepth, nCurDepth, Path.GetFileName(strZipFile), strOrgZipFile);
							strOverMaxDepthZipFile = entry.Key;
                            continue;
                        }

						// Scan Zip File in Zip
						int nInnerErrCount=0;
                        string strCurZip = Path.Combine(strBasePath, entry.Key);
                        string strExtractPath = Path.Combine(strBasePath, Path.GetFileNameWithoutExtension(entry.Key));
                        eFileAddErr enRet = ScanZipFile(strOrgZipFile, strOrgZipFileRelativePath, strCurZip, strExtractPath, nMaxDepth, nCurDepth + 1, 
							blWhite, strExtInfo, nCurErrCount, out nInnerErrCount, out strOverMaxDepthZipFile, blAllowDRM, SGFileExamEvent, ExamCount, TotalCount);
						if (enRet != eFileAddErr.eFANone) enErr = enRet;
						nCurErrCount += nInnerErrCount;
					}
				}
			}
			catch (System.Exception )
			{
				// Check Passowrd in 7zip(7z)
				if (nCurDepth != 1)
				{
					enErr = eFileAddErr.eUnZipInnerZipPassword;
					AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(strZipFile), enErr);
				}
				else
				{
					enErr = eFileAddErr.eFAZipPW;
					nCurErrCount++;
				}
			}
			
			nTotalErrCount = nCurErrCount;
			strOverMaxDepthInnerZipFile = strOverMaxDepthZipFile;

			// 가장 마지막에 난 error 값 넣음
			if (nTotalErrCount > 0)
			{
				FileAddErr faerr = m_FileAddErrList.ElementAt<FileAddErr>(m_FileAddErrList.Count - 1);
				enErr = faerr.eErrType;
			}

			return enErr;
		}

		public void LoadMimeConf(int groupID)
        {
			string strFileName = String.Format("FileMime.{0}.conf",groupID.ToString());
			strFileName = Path.Combine("wwwroot/conf", strFileName);
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				strFileName = strFileName.Replace("/","\\");
			}
			else
			{
				strFileName = strFileName.Replace("\\", "/");
			}
			try
            {
				string strEncMimeInfo = System.IO.File.ReadAllText(strFileName);
				SGRSACrypto sgRSACrypto = new SGRSACrypto();
				string strMimeInfo = sgRSACrypto.MimeConfDecrypt(strEncMimeInfo);

				if (strMimeInfo.Equals(""))
					return;

				if (strMimeInfo[strMimeInfo.Length - 1] == '\n')
					strMimeInfo = strMimeInfo.Substring(0, strMimeInfo.Length - 1);
				string[] strMimeList = strMimeInfo.Split('\n');
				if (strMimeList.Length <= 1)
					return;
				for(int i=1;i<strMimeList.Length;i++)
                {
					string[] strSplit = strMimeList[i].Split(' ');
					if (strSplit.Length < 2)
						continue;
					AddOrUpdate(strSplit[0], strSplit[1]);
                }
            }
			catch(FileNotFoundException ioEx)
            {
				Log.Information("LoadMimeConf Exception Msg = [{0}]", ioEx.Message);
			}
        }
	}
}
