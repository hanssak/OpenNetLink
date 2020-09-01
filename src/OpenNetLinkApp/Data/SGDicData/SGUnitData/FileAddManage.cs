using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using HeyRed.Mime;
using HsNetWorkSG;
using OpenNetLinkApp.Services;
using System.IO;

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
		public bool GetExamFileExtChange(HsStream hsStream)
		{
			string strExt = Path.GetExtension(hsStream.FileName);
			if(!IsValidFileExt(hsStream.stream, strExt))
            {
				string strFileName = hsStream.FileName;
				string strRelativePath = hsStream.RelativePath;
				AddData(strFileName, eFileAddErr.eFACHG, strRelativePath);
				return false;
            }
			return true;
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
			char sep = (char)'\n';
			string[] strArray = strFilePathList.Split(sep);
			int count = 0;
			foreach (var item in strArray)
			{
				if (count == 0)
				{	
					count++;
					continue;
				}
				string str = item;
				str = str.Replace("/", "\\");
				str = str.Replace("\r", "");
				ListFile.Add(str);
				count++;
			}

			FileInfo fileinfo = new FileInfo(strFilePath);
			fileinfo.Delete();
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
        private static byte[] StreamToByteArray(Stream stInput, int nMaxSize)
        {
            if (stInput == null) return null;
            byte[] buffer = new byte[nMaxSize];
            stInput.Position = 0;
			
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                read = stInput.Read(buffer, 0, buffer.Length);
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
        public bool IsValidFileExt(Stream stFile, string strExt)
        {
            byte[] btFileData = StreamToByteArray(stFile, MaxBufferSize);
            string strFileMime = MimeGuesser.GuessMimeType(btFileData);
            if (String.IsNullOrEmpty(strExt) == true) {
                if (String.Compare(strFileMime, "text/plain") == 0) return true;
                if (String.Compare(strFileMime, "application/x-executable") == 0) return true;

                return false;
            }

            string strFileExt = MimeGuesser.GuessExtension(btFileData);
            Console.WriteLine("FileExt [" + strFileExt + "] Ext[" + strExt + "]"); 
            if (String.Compare(strFileExt, strExt) == 0) return true;

            string strExtMime = MimeTypesMap.GetMimeType(strExt);
            Console.WriteLine("ExtMime [" + strFileMime + "] Ext [" + strExtMime + "]"); 
            if (String.Compare(strFileMime, strExtMime) == 0) return true;

            string strFileMimeToExt = MimeTypesMap.GetExtension(strExtMime);
            Console.WriteLine("ExtFileMimeToExt [" + strFileMimeToExt + "] Ext [" + strExt + "]"); 
            if (String.Compare(strFileMimeToExt, strExt) == 0) return true;

            return false;
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
	}
}
