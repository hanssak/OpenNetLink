using System;
using System.Collections.Generic;
using System.Text;
using OpenNetLinkApp.Services;

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
		eUnZipInnerDRM							// zip파일에 내부의 DRM 파일
	}

	public class FileAddErr
    {
		public string FileName = "";
		public eFileAddErr eErrType = eFileAddErr.eFANone;
		public string FilePath = "";
		public string ExceptionReason = "";
		public bool bSub = false;
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

		public string SetExceptionReason(eFileAddErr err)
		{
			string str = "";
			XmlConfService xmlConf = new XmlConfService();
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
		public FileAddManage()
        {

        }
		~FileAddManage()
        {

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

		public static bool GetEmptyEnable(long nRegSize)
        {
			if (nRegSize <= 0)
				return false;
			return true;
        }
		public static bool GetRegExtEnable(bool bWhite,string strStandardFileExtInfo, string strExt)
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
	}
}
