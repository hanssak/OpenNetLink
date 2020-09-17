using System.Linq;
using System;
using System.Collections.Generic;
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
					/* str = xmlConf.GetTitle("T_eFADAYCOUNTOVER"); */
					str = "일일 전송횟수 제한";
					break;
				case eFileAddErr.eFADAYSIZEOVER:                                // 일일 전송사이즈 제한. 
                    /* TODO */
                    /* str = xmlConf.GetTitle("T_eFADAYSIZEOVER"); */
					str = "일일 전송사이즈 제한";
					break;

				case eFileAddErr.eUnZipInnerZipOpenFail:                                // zip파일 내부의 zip Open 실패 
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerZipOpenFail"); */
					str = "ZIP 열기 오류";
					break;

				case eFileAddErr.eUnZipInnerZipPassword:                                // zip파일에 내부의 zip 비밀번호 사용 중
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerZipPassword"); */
					str = xmlConf.GetTitle("T_eFAZipPW");
					break;

				case eFileAddErr.eUnZipInnerExt:                                // zip파일에 내부의 zip 확장자 제한 파일 포함
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerExt"); */
					str = xmlConf.GetTitle("T_eFAEXT");
					break;

				case eFileAddErr.eUnZipInnerExtChange:                               // zip파일에 내부의 zip 위변조 파일 포함
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerExtChange"); */
					str = xmlConf.GetTitle("T_eFACHG");
					break;

				case eFileAddErr.eUnZipInnerExtUnknown:                                // zip파일에 내부의 zip 알수 없는 파일형식 포함
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerExtUnknown"); */
					str = xmlConf.GetTitle("T_eFAUNKNOWN");
					break;

				case eFileAddErr.eUnZipInnerFileEmpty:                                // zip파일에 내부의 zip 비어있는 파일 
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerFileEmpty"); */
					str = xmlConf.GetTitle("T_eFAEMPTY");
					break;

				case eFileAddErr.eUnZipInnerLengthOver:                                // zip파일에 내부의 zip Length Over
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerLengthOver"); */
					str = "파일 및 폴더명 길이 초과";
					break;

				case eFileAddErr.eUnZipInnerLeftZip:                                // zip파일검사 후 남아 있는 zip포함
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerLeftZip"); */
					str = "ZIP파일 검사후 잔여 ZIP 포함";
					break;

				case eFileAddErr.eUnZipInnerDRM:                                // zip파일에 내부의 DRM 파일
					/* TODO */
					/* str = xmlConf.GetTitle("T_eUnZipInnerDRM"); */
					str = "ZIP파일 내부 DRM 파일";
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
					strMsg = xmlConf.GetWarnMsg("W_0027");                      // 파일은 {0} MB까지 전송할 수 있습니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFANotFound:
					strMsg = xmlConf.GetWarnMsg("W_0028");                      // {0} 파일을 찾을 수 없습니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAHidden:
					strMsg = xmlConf.GetWarnMsg("W_0180");                      // {0} 파일은 숨김파일이므로 파일 첨부가 불가합니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAZipPW:
					strMsg = xmlConf.GetWarnMsg("W_0097");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있어 전송이 제한된 파일입니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAZipNotPW:
					strMsg = xmlConf.GetWarnMsg("W_0100");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있어 전송이 제한된 파일입니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAZipError:
					strMsg = xmlConf.GetWarnMsg("W_0099");                      // {0} 파일은 분할압축파일 또는 zip 파일이 아니거나 손상된 파일입니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFADAYCOUNTOVER:
					strMsg = xmlConf.GetWarnMsg("W_0181");                      // 일일 전송 횟수를 초과하였습니다.
					break;
				case eFileAddErr.eFADAYSIZEOVER:
					strMsg = xmlConf.GetWarnMsg("W_0182");                      // 일일 전송 사이즈를 초과하였습니다.
					break;
				case eFileAddErr.eFAVIRUS:
					strMsg = xmlConf.GetWarnMsg("W_0184");                      // {0} 파일에서 바이러스가 검출되었습니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAUnZipOutOfSpace:
					strMsg = xmlConf.GetErrMsg("E_0191");                      // {0} 파일은/r/nDisk 용량이 부족하여 검사를 할수 없습니다./r/nC:\를 정리하여 용량을 확보하여 다시 시도 하십시오.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAUnZipLengthOver:
					strMsg = xmlConf.GetErrMsg("E_0192");                      // {0} 파일은/r/n압축파일 내부의 파일 및 경로가 길어 검사가 실패하였습니다./r/n압축파일 내부의 파일 및 경로를 변경하여 다시 시도 하십시오.
					strMsg = String.Format(strMsg, strFileLimitSize);
					break;
				case eFileAddErr.eFAUnZipCheckStop:
					strMsg = xmlConf.GetErrMsg("E_0199");                      // {0} 파일의 압축파일 검사를 취소 하셨습니다.
					strMsg = String.Format(strMsg, strFileLimitSize);
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
		public FileAddManage()
        {
			ListFile = new List<string>();
		}
		~FileAddManage()
        {

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

		public static bool GetRegCountEnable(int nStandardCount, int nRegCount)
        {
			if (nStandardCount < nRegCount)
				return false;
			return true;
        }

		public static bool GetRegSizeEnable(long nStandardSize, long nRegSize)
		{
			if (nStandardSize < nRegSize)
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
		private bool FilePathLength(string strFileRelativePath)
		{
			string strFileReName = GetFileRename(true, strFileRelativePath);
			byte[] temp = Encoding.Default.GetBytes(strFileReName);
			strFileReName = Encoding.UTF8.GetString(temp);
			if (strFileReName.Length >= 90 )							// 전체 경로 길이 확인 (90자)
			{
				return false;
			}
			return true;
		}

		private bool FileFolderNameLength(string strFileRelativePath, out bool bSuper)
		{
			string strFileReName = GetFileRename(true, strFileRelativePath);
			byte[] temp = Encoding.Default.GetBytes(strFileReName);
			strFileReName = Encoding.UTF8.GetString(temp);

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

		public bool GetExamFileAddEnable(HsStream hsStream, bool bWhite, string strFileExtInfo, bool bHidden)
        {
			if (hsStream == null)
				return true;

			bool bExtEnable = false;                        // 확장자 제한 검사 결과
			bool bHiddenEnable = false;                     // 숨김 파일인지 검사 결과
			bool bFilePathEnable = false;                   // 긴파일명 전체 경로 길이 검사
			bool bFileFolderNameEnable = false;             // 폴더 및 파일 경로 길이 확인 (80자)
			bool bEmpty = false;							// 빈파일인지 여부 검사 

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
			return bRet;

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

			int nChangeExceptionCount = GetChangeExceptionCount();
			nChangeExceptionCount = GetChangeExceptionCount();
			if (nChangeExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFACHG);
				strCount = fileAddErr.GetExceptionCountString(nChangeExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nHiddenExceptionCount = GetHiddenExceptionCount();
			nHiddenExceptionCount = GetHiddenExceptionCount();
			if (nHiddenExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAHidden);
				strCount = fileAddErr.GetExceptionCountString(nHiddenExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}
			int nEmptyExceptionCount = GetEmptyExceptionCount();
			nEmptyExceptionCount = GetEmptyExceptionCount();
			if (nEmptyExceptionCount > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAEMPTY);
				strCount = fileAddErr.GetExceptionCountString(nEmptyExceptionCount);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFilePathOverExcetpion = GetFilePathOverExceptionCount();
			nFilePathOverExcetpion = GetFilePathOverExceptionCount();
			if (nFilePathOverExcetpion > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH);
				strCount = fileAddErr.GetExceptionCountString(nFilePathOverExcetpion);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nSuperFolderNameOverException = GetSuperFolderNameOverExceptionCount();
			nSuperFolderNameOverException = GetSuperFolderNameOverExceptionCount();
			if (nSuperFolderNameOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_PARENT);
				strCount = fileAddErr.GetExceptionCountString(nSuperFolderNameOverException);
				strReason = strReason + " : " + strCount;
				m_FileAddErrReason.Add(strReason);
				strReason = "";
			}

			int nFileFolderNameOverException = GetFileFolderNameOverExceptionCount();
			nFileFolderNameOverException = GetFileFolderNameOverExceptionCount();
			if (nFileFolderNameOverException > 0)
			{
				strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_FILEORPATH);
				strCount = fileAddErr.GetExceptionCountString(nFileFolderNameOverException);
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
				str = str.Replace("/", "\\");
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
			Console.WriteLine("**** IsWord(), ");

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
				byte[] btHLP_Header = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x03, 0x00, 0xFE, 0xFF, 0x09, 0x00 };
				byte[] btHLP_Header2 = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
				if (ByteArrayCompare(btFileData, btHLP_Header) == true && ByteArrayCompare(btFileData, btHLP_Header2, 0x3f0) == true) return true;
			}

			return false;
		}

		private static bool IsPPT(byte[] btFileData, string strExt)
		{
			if (String.Compare(strExt, "ppt") == 0 &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("drs")) == true &&
				FindZipContent(btFileData, Encoding.UTF8.GetBytes("downrev.xml")) == true)
				return true;

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
			if (ByteArrayCompare(btFileData, Encoding.UTF8.GetBytes("HWP Document File")) == true) return true;

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

				/* CAD 파일 */
				if (String.Compare(strExt, "dwf") == 0) return IsDWF(btFileData, strExt);

				/* 압축파일 */
				if (String.Compare(strExt, "rar") == 0) return IsRAR(btFileData, strExt);
				if (String.Compare(strExt, "arj") == 0) return IsARJ(btFileData, strExt);
				if (String.Compare(strExt, "iso") == 0) return IsISO(btFileData, strExt);
				if (String.Compare(strExt, "jar") == 0) return IsJAR(btFileData, strExt);

				/* 기타파일 */
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
		private static int DefaultAddFirst = 0;
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
			DefaultAdd();
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

			if (String.Compare(strFileMime, "application/octet-stream") == 0)
			{
				btFileData = await StreamToByteArrayAsync(stFile, MaxBufferSize2);
				
				Log.Debug("[IsValidFileExt] Unknown file signature"); 
                if (CheckExtForFileByteData(btFileData, strExt) == true) return eFileAddErr.eFANone;
		
				return eFileAddErr.eFAUNKNOWN;
			}	
            
			string strFileExt = MimeGuesser.GuessExtension(btFileData);
			strExt = strExt.Replace(".", "");
            Log.Debug("[IsValidFileExt] FileDataExt [" + strFileExt+ "] Ext [" + strExt + "]"); 
            if (String.Compare(strFileExt, strExt) == 0) return eFileAddErr.eFANone;

            string strExtMime = MimeTypesMap.GetMimeType(strExt);
            Log.Debug("[IsValidFileExt] FileDataMime [" + strFileMime + "] ExtMime [" + strExtMime + "]"); 
            if (String.Compare(strFileMime, strExtMime) == 0) return eFileAddErr.eFANone;

            string strFileMimeToExt = MimeTypesMap.GetExtension(strFileMime);
            Log.Debug("[IsValidFileExt] FileDataMimeExt [" + strFileMimeToExt + "] Ext [" + strExt + "]"); 
            if (String.Compare(strFileMimeToExt, strExt) == 0) return eFileAddErr.eFANone;

            return eFileAddErr.eFACHG;
        }

		public eFileAddErr IsValidFileExtInnerZip(string strFile, string strExt, bool blAllowDRM)
		{
			DefaultAdd();
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

			if (String.Compare(strFileMime, "application/octet-stream") == 0)
			{
                
				var fsStreamMore = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                btFileData = StreamToByteArray(fsStreamMore, MaxBufferSize2);
                fsStreamMore.Close();
				
				Log.Debug("[IsValidFileExtInnerZip] Unknown file signature"); 
                if (CheckExtForFileByteData(btFileData, strExt) == true) return eFileAddErr.eFANone;
				
				return eFileAddErr.eUnZipInnerExtUnknown;
			}
		
			/* Check Ext of file data and Ext of file name */
			string strFileExt = MimeGuesser.GuessExtension(btFileData);
			strExt = strExt.Replace(".", "");
            Log.Debug("[IsValidFileExtInnerZip] FileDataExt [" + strFileExt+ "] Ext [" + strExt + "]"); 
			if (String.Compare(strFileExt, strExt) == 0) return eFileAddErr.eFANone;

			/* Check Mime of file data and Mime of file name extension */
			string strExtMime = MimeTypesMap.GetMimeType(strExt);
            Log.Debug("[IsValidFileExtInnerZip] FileDataMime [" + strFileMime + "] ExtMime [" + strExtMime + "]"); 
			if (String.Compare(strFileMime, strExtMime) == 0) return eFileAddErr.eFANone;

			/* Check Ext of file data mime and Ext of file name */
			string strFileMimeToExt = MimeTypesMap.GetExtension(strFileMime);
            Log.Debug("[IsValidFileExtInnerZip] FileDataMimeExt [" + strFileMimeToExt + "] Ext [" + strExt + "]"); 
			if (String.Compare(strFileMimeToExt, strExt) == 0) return eFileAddErr.eFANone;
			
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
        
        /**
        * @breif 파일확장자 및 MimeType 정보 등록 및 갱신
        * @param strMime: 확장자의 Mime 정보 
        * @param strExt : 확장자 
        */
        public void AddOrUpdate(string strMime, string strExt) => MimeTypesMap.AddOrUpdate(strMime, strExt);
		public void DefaultAdd()
		{
			if (DefaultAddFirst == 0)
			{
				AddOrUpdate("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
				AddOrUpdate("application/x-hwp", "hwp");
				AddOrUpdate("application/x-sharedlib", "so");
				AddOrUpdate("application/zip", "zip");
				AddOrUpdate("application/gzip", "tgz");
				AddOrUpdate("image/png", "png");
				AddOrUpdate("video/mp4", "mp4");

				AddOrUpdate("application/csv", "csv");
				AddOrUpdate("application/dicom", "dcm");
//				AddOrUpdate("application/epub", "zip");
				AddOrUpdate("application/epub+zip", "xml");
				AddOrUpdate("application/haansofthwp", "frm");
//				AddOrUpdate("application/haansofthwp", "hwp");
				AddOrUpdate("application/haansofthwp", "hwt");
				AddOrUpdate("application/mac-binhex40", "hqx");
				AddOrUpdate("application/marc", "marc");
				AddOrUpdate("application/marc", "mrc");
				AddOrUpdate("application/msword", "doc");
				AddOrUpdate("application/msword", "docm");
//				AddOrUpdate("application/msword", "docx");
				AddOrUpdate("application/msword", "docxxxx");
				AddOrUpdate("application/msword", "dot");
				AddOrUpdate("application/msword", "dotm");
				AddOrUpdate("application/msword", "dotx");
//				AddOrUpdate("application/octet-stream", "bin");
//				AddOrUpdate("application/octet-stream", "class");
//				AddOrUpdate("application/octet-stream", "dll");
//				AddOrUpdate("application/octet-stream", "ebs");
//				AddOrUpdate("application/octet-stream", "exe");
//				AddOrUpdate("application/octet-stream", "img");
//				AddOrUpdate("application/octet-stream", "iso");
//				AddOrUpdate("application/octet-stream", "lha");
//				AddOrUpdate("application/octet-stream", "lzh");
//				AddOrUpdate("application/octet-stream", "pptx");
//				AddOrUpdate("application/octet-stream", "pptx_dec");
//				AddOrUpdate("application/octet-stream", "so");
//				AddOrUpdate("application/octet-stream", "xlsx");
				AddOrUpdate("application/ogg", "ogg");
				AddOrUpdate("application/pdf", "pdf");
				AddOrUpdate("application/pgp", "pgp");
				AddOrUpdate("application/pgp-signature", "sig");
				AddOrUpdate("application/post", "ai");
				AddOrUpdate("application/post", "eps");
				AddOrUpdate("application/post", "ps");
				AddOrUpdate("application/vnd.cups-raster", "ppd");
				AddOrUpdate("application/vnd.font-fontforge-sfd", "sfd");
				AddOrUpdate("application/vnd.google-earth.kml+xml", "kml");
				AddOrUpdate("application/vnd.google-earth.kmz", "kmz");
				AddOrUpdate("application/vnd.iccprofile", "icc");
				AddOrUpdate("application/vnd.iccprofile", "icm");
				AddOrUpdate("application/vnd.lotus-wordpro", "lwp");
				AddOrUpdate("application/vnd.lotus-wordpro", "sam");
				AddOrUpdate("application/vnd.ms-cab-compressed", "cab");
				AddOrUpdate("application/vnd.ms-cab-compressed", "msu");
//				AddOrUpdate("application/vnd.ms-excel", "png");
				AddOrUpdate("application/vnd.ms-excel", "xlam");
				AddOrUpdate("application/vnd.ms-excel", "xlb");
				AddOrUpdate("application/vnd.ms-excel", "xls");
				AddOrUpdate("application/vnd.ms-excel", "xlsm");
				AddOrUpdate("application/vnd.ms-excel", "xlt");
				AddOrUpdate("application/vnd.ms-excel", "xltm");
				AddOrUpdate("application/vnd.ms-excel", "xltx");
				AddOrUpdate("application/vnd.ms-fontobject", "eot");
				AddOrUpdate("application/vnd.ms-opentype", "otf");
				AddOrUpdate("application/vnd.ms-powerpoint", "potm");
				AddOrUpdate("application/vnd.ms-powerpoint", "potx");
				AddOrUpdate("application/vnd.ms-powerpoint", "ppam");
				AddOrUpdate("application/vnd.ms-powerpoint", "pps");
				AddOrUpdate("application/vnd.ms-powerpoint", "ppsm");
				AddOrUpdate("application/vnd.ms-powerpoint", "ppsx");
				AddOrUpdate("application/vnd.ms-powerpoint", "ppt");
				AddOrUpdate("application/vnd.ms-powerpoint", "pptm");
				AddOrUpdate("application/vnd.ms-powerpoint", "sldm");
				AddOrUpdate("application/vnd.ms-powerpoint", "sldx");
				AddOrUpdate("application/vnd.ms-tnef", "tnef");
				AddOrUpdate("application/vnd.ms-tnef", "tnf");
				AddOrUpdate("application/vnd.oasis.opendocument.chart", "odc");
				AddOrUpdate("application/vnd.oasis.opendocument.chart-template", "otc");
				AddOrUpdate("application/vnd.oasis.opendocument.database", "odb");
				AddOrUpdate("application/vnd.oasis.opendocument.formula", "odf");
				AddOrUpdate("application/vnd.oasis.opendocument.graphics", "odg");
				AddOrUpdate("application/vnd.oasis.opendocument.graphics-template", "otg");
				AddOrUpdate("application/vnd.oasis.opendocument.image", "odi");
				AddOrUpdate("application/vnd.oasis.opendocument.image-template", "oti");
				AddOrUpdate("application/vnd.oasis.opendocument.presentation", "odp");
				AddOrUpdate("application/vnd.oasis.opendocument.presentation-template", "otp");
				AddOrUpdate("application/vnd.oasis.opendocument.spreadsheet", "ods");
				AddOrUpdate("application/vnd.oasis.opendocument.spreadsheet-template", "ots");
				AddOrUpdate("application/vnd.oasis.opendocument.text", "odt");
				AddOrUpdate("application/vnd.oasis.opendocument.text-master", "odm");
				AddOrUpdate("application/vnd.oasis.opendocument.text-template", "ott");
				AddOrUpdate("application/vnd.oasis.opendocument.text-web", "oth");
				AddOrUpdate("application/vnd.rn-realmedia", "rm");
				AddOrUpdate("application/vnd.symbian.install", "sis");
				AddOrUpdate("application/vnd.tcpdump.pcap", "pcap");
				AddOrUpdate("application/warc", "warc");
				AddOrUpdate("application/x-123", "123");
				AddOrUpdate("application/x-123", "wk");
				AddOrUpdate("application/x-7z-compressed", "7z");
				AddOrUpdate("application/x-arc", "arc");
				AddOrUpdate("application/x-archive", "ar");
				AddOrUpdate("application/x-arj", "arj");
				AddOrUpdate("application/x-bittorrent", "torrent");
				AddOrUpdate("application/x-bzip2", "bz2");
				AddOrUpdate("application/x-compress", "z");
				AddOrUpdate("application/x-coredump", "core");
				AddOrUpdate("application/x-cpio", "cpio");
				AddOrUpdate("application/x-dbm", "dbm");
				AddOrUpdate("application/x-debian-package", "deb");
				AddOrUpdate("application/x-dvi", "dvi");
				AddOrUpdate("application/x-eet", "eet");
				AddOrUpdate("application/x-elc", "elc");
				AddOrUpdate("application/x-empty", "webm");
				AddOrUpdate("application/x-epoc-app", "app");
				AddOrUpdate("application/x-epoc-opl", "opl");
				AddOrUpdate("application/x-epoc-opo", "opo");
				AddOrUpdate("application/x-font-ttf", "ttf");
				AddOrUpdate("application/x-freemind", "mm");
				AddOrUpdate("application/x-gnucash", "gnucash");
				AddOrUpdate("application/x-gnumeric", "gnm");
				AddOrUpdate("application/x-gnumeric", "gnumeric");
				AddOrUpdate("application/x-gnupg-keyring", "gpg");
				AddOrUpdate("application/x-gzip", "gz");
//				AddOrUpdate("application/x-gzip", "tgz");
				AddOrUpdate("application/x-hdf", "hdf");
				AddOrUpdate("application/x-ichitaro4", "jtd");
				AddOrUpdate("application/x-java-jce-keystore", "jks");
				AddOrUpdate("application/x-java-pack200", "pack");
				AddOrUpdate("application/x-kdelnk", "kdelnk");
				AddOrUpdate("application/x-lharc", "sfx");
				AddOrUpdate("application/x-lrzip", "lrz");
				AddOrUpdate("application/x-lzip", "lz");
				AddOrUpdate("application/x-lzma", "lzma");
				AddOrUpdate("application/x-mif", "mif");
				AddOrUpdate("application/x-msaccess", "mdb");
				AddOrUpdate("application/x-object", "obj");
				AddOrUpdate("application/x-quark-xpress-3", "qxp");
				AddOrUpdate("application/x-quicktime-player", "qtl");
				AddOrUpdate("application/x-rar", "rar");
				AddOrUpdate("application/x-rpm", "rpm");
				AddOrUpdate("application/x-sc", "sc");
				AddOrUpdate("application/x-scribus", "scd");
				AddOrUpdate("application/x-setup", "xnf");
				AddOrUpdate("application/x-shockwave-flash", "swf");
				AddOrUpdate("application/x-shockwave-flash", "swfl");
				AddOrUpdate("application/x-stuffit", "sit");
				AddOrUpdate("application/x-stuffit", "sitx");
				AddOrUpdate("application/x-svr4-package", "pkg");
				AddOrUpdate("application/x-tar", "tar");
				AddOrUpdate("application/x-tex-tfm", "tfm");
				AddOrUpdate("application/x-tokyocabinet-btree", "kch");
				AddOrUpdate("application/x-xz", "xz");
				AddOrUpdate("application/x-zoo", "zoo");
				AddOrUpdate("audio/basic", "au");
				AddOrUpdate("audio/basic", "snd");
				AddOrUpdate("audio/midi", "midi");
//				AddOrUpdate("audio/mp4", "mp4");
				AddOrUpdate("audio/mpeg", "mpeg");
				AddOrUpdate("audio/x-adpcm", "pcm");
				AddOrUpdate("audio/x-aiff", "aiff");
				AddOrUpdate("audio/x-ape", "ape");
				AddOrUpdate("audio/x-flac", "fla");
				AddOrUpdate("audio/x-flac", "flac");
				AddOrUpdate("audio/x-hx-aac-adif", "aac");
				AddOrUpdate("audio/x-mod", "mod");
				AddOrUpdate("audio/x-mp4a-latm", "mp4a");
				AddOrUpdate("audio/x-pn-realaudio", "ra");
				AddOrUpdate("audio/x-w64", "w64");
				AddOrUpdate("audio/x-wav", "wav");
				AddOrUpdate("chemical/x-pdb", "ent");
				AddOrUpdate("chemical/x-pdb", "pdb");
				AddOrUpdate("image/gif", "gif");
				AddOrUpdate("image/jp2", "jk2");
				AddOrUpdate("image/jp2", "jp2");
				AddOrUpdate("image/jpeg", "jpeg");
				AddOrUpdate("image/jpeg", "jpg");
				AddOrUpdate("image/svg+xml", "svg");
				AddOrUpdate("image/tiff", "tif");
				AddOrUpdate("image/tiff", "tiff");
				AddOrUpdate("image/vnd.adobe.photoshop", "psd");
				AddOrUpdate("image/vnd.djvu", "djv");
				AddOrUpdate("image/vnd.djvu", "djvu");
				AddOrUpdate("image/x-canon-cr2", "cr2");
				AddOrUpdate("image/x-canon-crw", "crw");
				AddOrUpdate("image/x-coreldraw", "cdr");
				AddOrUpdate("image/x-cpi", "cpi");
				AddOrUpdate("image/x-epoc-mbm", "mbm");
				AddOrUpdate("image/x-epoc-sketch", "Sketch");
				AddOrUpdate("image/x-epoc-sketch", "sketch");
				AddOrUpdate("image/x-icon", "ico");
				AddOrUpdate("image/x-ms-bmp", "bmp");
				AddOrUpdate("image/x-niff", "niff");
				AddOrUpdate("image/x-olympus-orf", "orf");
				AddOrUpdate("image/x-paintnet", "pdn");
				AddOrUpdate("image/x-paintnet", "tga");
				AddOrUpdate("image/x-portable-bitmap", "pbm");
				AddOrUpdate("image/x-portable-greymap", "pgm");
				AddOrUpdate("image/x-portable-pixmap", "ppm");
				AddOrUpdate("image/x-quicktime", "mov");
				AddOrUpdate("image/x-quicktime", "qt");
				AddOrUpdate("image/x-x3f", "x3f");
				AddOrUpdate("image/x-xcf", "xcf");
				AddOrUpdate("image/x-xpmi", "xpm");
				AddOrUpdate("image/x-xwindowdump", "xwd");
				AddOrUpdate("message/news", "news");
				AddOrUpdate("message/rfc822", "art");
				AddOrUpdate("message/rfc822", "eml");
				AddOrUpdate("message/rfc822", "mail");
				AddOrUpdate("model/vrml", "vrml");
				AddOrUpdate("model/vrml", "wrl");
				AddOrUpdate("model/x3d", "x3d");
				AddOrUpdate("model/x3d", "x3db");
				AddOrUpdate("model/x3d", "x3dv");
				AddOrUpdate("msword", ".doc");
				AddOrUpdate("text/calendar", "icalendar");
				AddOrUpdate("text/calendar", "ics");
				AddOrUpdate("text/html", "htm");
				AddOrUpdate("text/html", "html");
				AddOrUpdate("text/html", "txt");
				AddOrUpdate("text/plain", "log");
				AddOrUpdate("text/plain", "text");
				AddOrUpdate("text/rtf", "rtf");
				AddOrUpdate("text/texmacs", "fsf");
				AddOrUpdate("text/troff", "troff");
				AddOrUpdate("text/x-asm", "asm");
				AddOrUpdate("text/x-awk", "awk");
				AddOrUpdate("text/x-c", "c");
				AddOrUpdate("text/x-c", "cpp");
				AddOrUpdate("text/x-diff", "diff");
				AddOrUpdate("text/x-info", "info");
				AddOrUpdate("text/x-java", "java");
				AddOrUpdate("text/x-lisp", "lisp");
				AddOrUpdate("text/x-lua", "lua");
				AddOrUpdate("text/x-m4", "m4");
				AddOrUpdate("text/x-makefile", "makefile");
				AddOrUpdate("text/x-msdos-batch", "bat");
				AddOrUpdate("text/x-pascal", "pas");
				AddOrUpdate("text/x-perl", "perl");
				AddOrUpdate("text/x-php", "php");
				AddOrUpdate("text/x-pod", "pod");
				AddOrUpdate("text/x-python", "py");
				AddOrUpdate("text/x-ruby", "rudy");
				AddOrUpdate("text/x-shell", "sh");
				AddOrUpdate("text/x-tcl", "tcl");
				AddOrUpdate("text/x-tex", "cls");
				AddOrUpdate("text/x-tex", "ltx");
				AddOrUpdate("text/x-tex", "sty");
				AddOrUpdate("text/x-tex", "tex");
				AddOrUpdate("text/x-texinfo", "texi");
				AddOrUpdate("text/x-vcard", "vcf");
				AddOrUpdate("text/x-xmcd", "xmcd");
				AddOrUpdate("video/3gpp", "3gp");
				AddOrUpdate("video/3gpp2", "3g2");
				AddOrUpdate("video/h264", "h264");
				AddOrUpdate("video/mp2p", "mp2");
				AddOrUpdate("video/mp2t", "ts");
				AddOrUpdate("video/mp4v-es", "mp4v");
				AddOrUpdate("video/mpeg", "mpe");
				AddOrUpdate("video/mpeg", "mpg");
				AddOrUpdate("video/mpv", "mpv");
				AddOrUpdate("video/x-flc", "flc");
				AddOrUpdate("video/x-fli", "fli");
				AddOrUpdate("video/x-flv", "flv");
				AddOrUpdate("video/x-jng", "jng");
				AddOrUpdate("video/x-mng", "mng");
				AddOrUpdate("video/x-ms-asf", "asf");
				AddOrUpdate("video/x-ms-asf", "asx");
				AddOrUpdate("video/x-msvideo", "avi");
				AddOrUpdate("video/x-sgi-movie", "movie");
				AddOrUpdate("x-epoc/x-sisx-app", "sisx");

				DefaultAddFirst = 1;
			}
		}

        public void AddDataForInnerZip(int nErrCount, string strOrgZipFile, string strOrgZipFileRelativePath, string strErrFileName, eFileAddErr enErr)
        {
			if (nErrCount == 1) AddData(strOrgZipFile, eFileAddErr.eFAZIP, strOrgZipFileRelativePath);
            AddData(strErrFileName, enErr, strOrgZipFile, true);
        }	

		public async Task<int> CheckZipFile(HsStream hsStream, bool blWhite, string strExtInfo, int nMaxDepth = 3, int nOption = 0, bool blAllowDRM = true)
		{
			int nTotalErrCount;
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
					out nTotalErrCount, out strOverMaxDepthInnerZipFile, blAllowDRM);
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
			if (enRet == eFileAddErr.eFANone) return 0;
			
			return -1;
		}

		public eFileAddErr ScanZipFile(string strOrgZipFile, string strOrgZipFileRelativePath, string strZipFile, string strBasePath, int nMaxDepth, int nCurDepth, 
			bool blWhite, string strExtInfo, int nErrCount, out int nTotalErrCount, out string strOverMaxDepthInnerZipFile, bool blAllowDRM)
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
						int nInnerErrCount;
                        string strCurZip = Path.Combine(strBasePath, entry.Key);
                        string strExtractPath = Path.Combine(strBasePath, Path.GetFileNameWithoutExtension(entry.Key));
                        eFileAddErr enRet = ScanZipFile(strOrgZipFile, strOrgZipFileRelativePath, strCurZip, strExtractPath, nMaxDepth, nCurDepth + 1, 
							blWhite, strExtInfo, nCurErrCount, out nInnerErrCount, out strOverMaxDepthZipFile, blAllowDRM);
                        if (enRet != eFileAddErr.eFANone) enErr = enRet;
						nCurErrCount = nInnerErrCount;
					}
				}
			}
			catch (System.Exception err)
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
			return enErr;
		}
	}
}
