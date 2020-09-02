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
					break;
				case eFileAddErr.eFADAYSIZEOVER:                                // 일일 전송사이즈 제한. 
					break;

				case eFileAddErr.eUnZipInnerZipOpenFail:                                // zip파일 내부의 zip Open 실패 
					break;

				case eFileAddErr.eUnZipInnerZipPassword:                                // zip파일에 내부의 zip 비밀번호 사용 중
					break;

				case eFileAddErr.eUnZipInnerExt:                                // zip파일에 내부의 zip 확장자 제한 파일 포함
					break;

				case eFileAddErr.eUnZipInnerExtChange:                               // zip파일에 내부의 zip 위변조 파일 포함
					break;

				case eFileAddErr.eUnZipInnerExtUnknown:                                // zip파일에 내부의 zip 알수 없는 파일형식 포함
					break;

				case eFileAddErr.eUnZipInnerFileEmpty:                                // zip파일에 내부의 zip 비어있는 파일 
					break;

				case eFileAddErr.eUnZipInnerLengthOver:                                // zip파일에 내부의 zip Length Over
					break;

				case eFileAddErr.eUnZipInnerLeftZip:                                // zip파일검사 후 남아 있는 zip포함
					break;

				case eFileAddErr.eUnZipInnerDRM:                                // zip파일에 내부의 DRM 파일
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

			char sep = (char)'\\';
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
			string strExt = Path.GetExtension(hsStream.FileName);
			if(await IsValidFileExt(hsStream.stream, strExt) != 0)
            {
				string strFileName = hsStream.FileName;
				string strRelativePath = hsStream.RelativePath;
				AddData(strFileName, eFileAddErr.eFACHG, strRelativePath);
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
			//fileinfo.Delete();
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

        private const int MaxBufferSize = 1024 * 64;
		private static int DefaultAddFirst = 0;
        private static async Task<byte[]> StreamToByteArray(Stream stInput, int nMaxSize)
        {
            if (stInput == null) return null;
            byte[] buffer = new byte[nMaxSize];
            stInput.Position = 0;
			
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                read = await stInput.ReadAsync(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, read);
                byte[] temp = ms.ToArray();

                return temp;
            }
		}
        
        /**
        * @breif 파일확장자 위변조 검사 수행 
        * @param stFile : 위변조 검사 대상 파일의 MemoryStream or FileStream 
        * @param strExt : 위변조 검사 대상 파일의 확장자 
        * @return 위변조 여부 ( true : 정상, false : 위변조 또는 확인 불가)
        */
        public async Task<int> IsValidFileExt(Stream stFile, string strExt)
        {
			DefaultAdd();
			byte[] btFileData = await StreamToByteArray(stFile, MaxBufferSize);
			string strFileMime = MimeGuesser.GuessMimeType(btFileData);
            
            if (String.Compare(strFileMime, "text/plain") == 0) return 0;
			if (String.IsNullOrEmpty(strExt) == true) {
                if (String.Compare(strFileMime, "application/x-executable") == 0) return 0;

                return -1;
            }
			else {
				strExt = strExt.Replace(".", "");
            }

            string strFileExt = MimeGuesser.GuessExtension(btFileData);
            Debug.WriteLine("FileExt [" + strFileExt + "] Ext[" + strExt + "]"); 
            if (String.Compare(strFileExt, strExt) == 0) return 0;

            string strExtMime = MimeTypesMap.GetMimeType(strExt);
            Debug.WriteLine("ExtMime [" + strFileMime + "] Ext [" + strExtMime + "]"); 
            if (String.Compare(strFileMime, strExtMime) == 0) return 0;

            string strFileMimeToExt = MimeTypesMap.GetExtension(strExtMime);
            Debug.WriteLine("ExtFileMimeToExt [" + strFileMimeToExt + "] Ext [" + strExt + "]"); 
            if (String.Compare(strFileMimeToExt, strExt) == 0) return 0;

            return -1;
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
	}
}
