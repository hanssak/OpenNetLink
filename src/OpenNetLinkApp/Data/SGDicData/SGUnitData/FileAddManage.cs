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
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.Archives;
using Serilog;
using Serilog.Events;
using AgLogManager;
using OpenNetLinkApp.PageEvent;
using Org.BouncyCastle.Math.EC;
using BlazorInputFile;
using System.Collections;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eFileAddErr
    {
        eFANone = 0,        // None
        eFAREG,             // 등록된 파일
        eFADRM,             // DRM
        eFADLP,             // DLP
        /// <summary>
        /// 확장자제한
        /// </summary>
        eFAEXT,             // 확장자제한
        eFAZIP,             // zip파일내 확장자
        eFACHG,             // 파일변경
        eFAVIRUS,           // 바이러스검출
        /// <summary>
        /// 1회 전송가능 파일 사이즈
        /// </summary>
        eFAFileSize,        // 파일사이즈
        /// <summary>
        /// 1회 전송가능 파일 개수
        /// </summary>
        eFAFileCount,       // 1회 전송가능 파일 개수
        eFANotFound,        // 파일 찾기 실패
        eFAHidden,          // 숨김파일
        /// <summary>
        /// zip 파일 비번 있을 때
        /// </summary>
        eFAZipPW,           // zip 파일 비번 있을 때
        eFAZipNotPW,        // zip 파일 비번 없을 때
        eFAZipError,        // zip 파일 손상 또는 zip 파일이 아닌경우
        eFAEMPTY,           // 빈파일
        /// <summary>
        /// 알수없는파일형식
        /// </summary>
        eFAUNKNOWN,         // 알수없는파일형식
        eFAEML,             // EML파일과 다른파일을 함께 등록할 경우
        eFAEMPTY_ATTACH,    // 빈파일(첨부파일)
        eFAUNKNOWN_ATTACH,  // 알수없는파일형식(첨부파일)
        eFACHG_ATTACH,      // 파일변경(첨부파일)
        eFAEXT_ATTACH,      // 확장자제한(첨부파일)
        eFAZIP_ATTACH,      // zip파일내 확장자(첨부파일)
        eFAEML_ONLYONE,     // EML 파일등록 2건이상일때
        eFAEMLTOPDF_ERROR,  // EML to PDF 변환오류

        eFAOfficeSizeError,     // Office > pdf 변환하려는 파일이 설정되크기보다 클 경우
        eFAOfficeNoinstalled,   // Office 설치않되어있음. 파일변환기능사용. Office파일 전송하려할 경우

        eFADLPERR,              // 개인정보 검출에러
        eFAUnZipOutOfSpace,     //UnZip Disk용량부족
        eFAUnZipLengthOver,     //UnZip Length Over
        eFAUnZipCheckStop,      //UnZip 체크 중단
        /// <summary>
        /// 파일읽기 권한오류 - @@@ NetLink에 없는거(1)
        /// </summary>
        eFA_FILE_READ_ERROR,    //파일읽기 권한오류 - @@@ NetLink에 없는거(1)

        /// <summary>
        /// 일일 전송 횟수 제한
        /// </summary>
        eFADAYCOUNTOVER = 51,   // 일일 전송횟수 제한.
        /// <summary>
        /// (일일)전송가능 파일 사이즈
        /// </summary>
        eFADAYSIZEOVER,         // 일일 전송사이즈 제한.

        // - @@@ NetLink에 없는거(아래쭉~)
        eUnZipInnerZipOpenFail = 60,            // zip파일 내부의 zip Open 실패
        /// <summary>
        /// zip파일에 내부의 zip 비밀번호 사용 중
        /// </summary>
        eUnZipInnerZipPassword,                 // zip파일에 내부의 zip 비밀번호 사용 중
        eUnZipInnerExt,                         // zip파일에 내부의 zip 확장자 제한 파일 포함
        eUnZipInnerExtChange,                   // zip파일에 내부의 zip 위변조 파일 포함
        eUnZipInnerExtUnknown,                  // zip파일에 내부B의 zip 알수 없는 파일형식 포함
        eUnZipInnerFileEmpty,                   // zip파일에 내부의 zip 비어있는 파일
        eUnZipInnerLengthOver,                  // zip파일에 내부의 zip Length Over
        /// <summary>
        /// zip파일검사 후 남아 있는 zip포함
        /// </summary>
        eUnZipInnerLeftZip,                     // zip파일검사 후 남아 있는 zip포함
        eUnZipInnerDRM,                         // zip파일에 내부의 DRM 파일

        eFA_LONG_PATH = 70,                     // OS에서 지원하는 최대 전송 길이를 초과(윈:250, 기타:90)
        eFA_LONG_PATH_PARENT,                   // 전체경로중 각 단계별 Folder 의 길이 초과(윈:250, 기타:90)
        eFA_LONG_PATH_FILEORPATH,                // 전송되는 파일 및 폴더의 이름 길이초과(윈:250, 기타:90)

        #region 문서 파일 검사 오류 목록
        /// <summary>
        /// OLE 추출 실패
        /// <para>101</para>
        /// </summary>
        eFADOC_OLE_COMMONE = 101,
        /// <summary>
        /// 비밀번호 설정된 문서
        /// <para>102</para>
        /// </summary>
        eFADOC_OLE_PASSWORD,
        /// <summary>
        /// OLE 검사 결과 -> 마임검사 실패
        /// <para>103</para>
        /// </summary>
        eFADOC_OLE_MIME,
        /// <summary>
        /// OLE 검사 결과 -> 확장자 제한
        /// <para>104</para>
        /// </summary>
        eFADOC_OLE_EXTENSION,
        /// <summary>
        /// OLE 검사 결과 -> 위변조 제한
        /// <para>105</para>
        /// </summary>
        eFADOC_OLE_CHANGE,
        /// <summary>
        /// 압축형식 검사 결과 -> 확장자 제한
        /// <para>106</para>
        /// </summary>
        eFADOC_COMPRESS_EXTENSION,
        /// <summary>
        /// 압축형식 검사 결과 -> 위변조 제한
        /// <para>107</para>
        /// </summary>
        eFADOC_COMPRESS_CHANGE,

        /// <summary>
        /// 검출 직후에도 남겨진 파일
        /// <para>120</para>
        /// </summary>
        eFADOC_OLE_REMAININGFILES = 120,

        #endregion
    }

    public class FileAddErr
    {
        XmlConfService xmlConf = new XmlConfService();

        public string FileName { get; set; } = "";
        public eFileAddErr eErrType = eFileAddErr.eFANone;
        public string FilePath = "";
        public string ExceptionReason = "";
        public bool bSub = false;

        /// <summary>
        /// 하위 폴더나 파일에 검사 오류 항목이 존재하는 경우 True
        /// </summary>
        public bool HasChildrenErr { get; set; }

        /// <summary>
        /// 하위 폴더
        /// </summary>
        public List<FileAddErr> ChildrenFiles { get; set; } = null;

        /// <summary>
        /// ZIP파일의 경우, 해당 파일을 압축한 ZIP 파일명
        /// </summary>
        public string ParentFileName { get; set; } = "";

        /// <summary>
        /// 전체경로길이 체크용
        /// </summary>
        public int m_nFilePathMax
        {
            get
            {
                HsNetWork hsNetwork = new HsNetWork();
                return hsNetwork.GetSendFilePathLengthMax();
            }
        }

        /// <summary>
        /// 1개의 파일(혹은 폴더)이름 길이 체크용
        /// </summary>
        public int m_nFileLengthMax
        {
            get
            {
                HsNetWork hsNetwork = new HsNetWork();
                return hsNetwork.GetSendFileNameLengthMax();
            }
        }
        public FileAddErr()
        {

        }

        //public FileAddErr(FileAddErr err)
        //{
        //    FileName = err.FileName;
        //    eErrType = err.eErrType;
        //    FilePath = err.FilePath;
        //    ExceptionReason = SetExceptionReason(err.eErrType);
        //    bSub = err.bSub;
        //}
        ~FileAddErr()
        {

        }

        public FileAddErr CreateChildren(string getFileName, string getFilePath, string getParentFileName)
        {
            if (ChildrenFiles == null)
                ChildrenFiles = new List<FileAddErr>();

            FileAddErr oneChild = new FileAddErr() { FileName = getFileName, FilePath = getFilePath, ParentFileName = getParentFileName };
            ChildrenFiles.Add(oneChild);
            return oneChild;
        }

        //public void SetFileAddErr(string strFilename, eFileAddErr err, string strFilePath, bool Sub = false, string strParentFileName = "")
        //{
        //    FileName = strFilename;
        //    eErrType = err;
        //    FilePath = strFilePath;
        //    ExceptionReason = SetExceptionReason(err);
        //    bSub = Sub;
        //    ParentFileName = strParentFileName;
        //}


        public string GetFileAddErrContent(string strFileName, eFileAddErr efa, string strFileLimitSize = "1500")
        {
            string strMsg = "";
            switch (efa)
            {
                // 1 ~ 5
                case eFileAddErr.eFAREG:                                        // 이미 등록되어 있는 파일인 경우
                    strMsg = xmlConf.GetWarnMsg("W_0002");                      // {0} 파일은 전송파일에 등록된 파일입니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFADRM:                                        // DRM 편집권한이 없는 경우
                    strMsg = xmlConf.GetWarnMsg("W_0177");                      // {0} 파일은 DRM이 걸려있어 편집 권한이 없습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFADLP:                                        // DLP 개인정보가 포함되어 있는 경우
                    strMsg = xmlConf.GetWarnMsg("W_0175");                      // {0} 파일은 개인정보가 포함되어 있어 전송할 수 없습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAEXT:                                        // 확장자 제한이 걸린 파일인 경우
                    strMsg = xmlConf.GetWarnMsg("W_0001");                      // {0} 파일은 전송이 제한된 파일입니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAZIP:
                    strMsg = xmlConf.GetWarnMsg("W_0185");                      // {0} 파일에 전송이 제한된 파일이 포함되어 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;


                // 6 ~ 10
                case eFileAddErr.eFACHG:                                        // 위변조 걸린 파일
                    strMsg = xmlConf.GetWarnMsg("W_0006");                      // {0}파일은 확장자가 변경된 파일입니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAVIRUS:
                    strMsg = xmlConf.GetWarnMsg("W_0184");                      // {0} 파일에서 바이러스가 검출되었습니다.
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

                // 11 ~ 15
                case eFileAddErr.eFAZipPW:
                    strMsg = xmlConf.GetWarnMsg("W_0097");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있어 전송이 제한된 파일입니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAZipNotPW:
                    strMsg = xmlConf.GetWarnMsg("W_0100");                      // {0} 파일은 압축파일에 비밀번호가 걸려 있지 않아 전송이 제한된 파일입니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAZipError:
                    strMsg = xmlConf.GetWarnMsg("W_0099");                      // {0} 파일은 분할압축파일 또는 zip 파일이 아니거나 손상된 파일입니다.
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

                // 16 ~ 20
                case eFileAddErr.eFAEML:
                    strMsg = xmlConf.GetWarnMsg("W_0178");                      // {0} 파일은 Eml 파일이 아닙니다. Eml 파일과 형식이 다른파일은 함께 전송 할수 없습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAEMPTY_ATTACH:
                    strMsg = xmlConf.GetWarnMsg("W_0116");                      // {0} 에 비어있는 첨부파일이 포함되어 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAUNKNOWN_ATTACH:
                    strMsg = xmlConf.GetWarnMsg("W_0117");                      // {0} 에 알수 없는 파일형식의 첨부파일이 포함되어 있습니다.
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

                // 21 ~ 25
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
                case eFileAddErr.eFAOfficeSizeError:
                    strMsg = xmlConf.GetWarnMsg("W_0261");                      // {0} Office 파일은 지정한 Size까지만 전송할 수 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFAOfficeNoinstalled:
                    strMsg = xmlConf.GetWarnMsg("W_0262");                      // [Microsoft Office]가 설치 되어 있지 않아/r/n/r/n{0} Office파일을 pdf파일로 변환 전송할 수 없습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;

                // 26 ~ 30
                case eFileAddErr.eFADLPERR:
                    strMsg = xmlConf.GetWarnMsg("W_0176");                      // {0} 파일에 대한 개인정보 검사에 실패하였습니다.
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
                    strMsg = xmlConf.GetErrMsg("E_0220");                      // {0} 파일에 읽기 권한이 없습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;

                // 51 ~ 52
                case eFileAddErr.eFADAYCOUNTOVER:
                    strMsg = xmlConf.GetWarnMsg("W_0181");                      // 일일 전송 횟수를 초과하였습니다.
                    break;
                case eFileAddErr.eFADAYSIZEOVER:
                    //strMsg = xmlConf.GetWarnMsg("W_0182");                      // 일일 전송 사이즈를 초과하였습니다.
                    strMsg = xmlConf.GetWarnMsg("W_0247");                      // {0} 파일은 일일 전송 가능 용량이 초과되어 차단되었습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;

                // 60 ~ 68
                case eFileAddErr.eUnZipInnerZipOpenFail:
                    strMsg = xmlConf.GetWarnMsg("W_0263");                      // 압축파일에 읽을수 없는 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerZipPassword:
                    strMsg = xmlConf.GetWarnMsg("W_0264");                      // 압축파일안에 비밀번호가 있는 {0}압축파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerExt:
                    strMsg = xmlConf.GetWarnMsg("W_0265");                      // 압축파일안에 확장자 제한이된 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerExtChange:
                    strMsg = xmlConf.GetWarnMsg("W_0266");                      // 압축파일안에 확장자가 변경된 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerExtUnknown:
                    strMsg = xmlConf.GetWarnMsg("W_0267");                      // 압축파일안에 알수 없는 형식의 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerFileEmpty:
                    strMsg = xmlConf.GetWarnMsg("W_0268");                      // 압축파일안에 빈 파일(0kb) {0}이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerLengthOver:
                    strMsg = xmlConf.GetWarnMsg("W_0269");                      // 압축파일안에 최대 파일명 길이를 초과하는 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerLeftZip:
                    strMsg = xmlConf.GetWarnMsg("W_0270");                      // 압축파일안에 또 압축파일 {0}이 남아있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eUnZipInnerDRM:
                    strMsg = xmlConf.GetWarnMsg("W_0271");                      // 압축파일안에 DRM이 적용된 {0}파일이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;


                // 70 ~ 72
                case eFileAddErr.eFA_LONG_PATH:
                    strMsg = xmlConf.GetWarnMsg("W_0272");                      // {0} 파일은 최대경로길이를 초과했습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFA_LONG_PATH_PARENT:
                    strMsg = xmlConf.GetWarnMsg("W_0273");                      // {0} 파일의 경로중에 최대이름길이를 초과하는 folder이름이 있습니다.
                    strMsg = String.Format(strMsg, strFileName);
                    break;
                case eFileAddErr.eFA_LONG_PATH_FILEORPATH:
                    strMsg = xmlConf.GetWarnMsg("W_0274");                      // {0} 파일은 이름이 최대길이를 초과했습니다.
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
        private List<FileAddErr> m_FileAddErrList = new List<FileAddErr>();

        public List<(string reason, string count)> m_FileAddErrReason = new List<(string reason, string count)>();
        public List<string> ListFile = null;

        XmlConfService xmlConf = new XmlConfService();

        public long m_nTansCurSize = 0;
        public long m_nCurRegisteringSize = 0;

        public int m_nTransCurCount = 0;

        /// <summary>
        /// 0KB 파일 전송 가능 유무
        /// true :전송 가능
        /// </summary>
        public bool bEmptyFIleNoCheck = false;

        /// <summary>
        /// OLE개체 및 압축형식 검사가 필요한 문서 확장자 대상 목록
        /// <para>"ODT", "DOC", "DOCM", "DOCX", "DOT", "DOTM", "DOTX", "RTF"</para>
        /// <para>"XLS", "XLSB", "XLSM", "XLSX", "XLT", "XLTM", "XLTX", "XLW"</para>
        /// <para>"POT", "PPT", "POTM", "POTX", "PPS", "PPSM", "PPSX", "PPTM", "PPTX"</para>
        /// <para>"HWP", "HWPX"</para>
        /// </summary>
        public readonly List<string> ListCheckableDocumentExtension = new List<string>() { "ODT", "DOC", "DOCM", "DOCX", "DOT", "DOTM", "DOTX", "RTF"
                                            , "XLS", "XLSB", "XLSM", "XLSX", "XLT", "XLTM", "XLTX", "XLW"
                                            , "POT", "PPT", "POTM", "POTX", "PPS", "PPSM", "PPSX", "PPTM", "PPTX"
                                            , "HWP", "HWPX"};

        /// <summary>
        /// 전체경로길이 체크용
        /// </summary>
        public int m_nFilePathMax
        {
            get
            {
                HsNetWork hsNetwork = new HsNetWork();
                return hsNetwork.GetSendFilePathLengthMax();
            }
        }

        /// <summary>
        /// 1개의 파일(혹은 폴더)이름 길이 체크용
        /// </summary>
        public int m_nFileLengthMax
        {
            get
            {
                HsNetWork hsNetwork = new HsNetWork();
                return hsNetwork.GetSendFileNameLengthMax();
            }
        }

        public FileAddManage()
        {
            ListFile = new List<string>();
        }
        public FileAddManage(int groupID)
        {
            ListFile = new List<string>();
            LoadMimeConf(groupID);
            //서버 적용 시 사용 예정
            //LoadOLEMimeConf(groupID);
        }
        ~FileAddManage()
        {

        }
        //사전에 전송하기 전에 전송량을 미리 계산하는 경우가 있어서 차단되는 경우는 다시 빼준다. 2021/04/23 YKH
        public void RestoreFileSizeLimit()
        {
            m_nTansCurSize -= m_nCurRegisteringSize;
            m_nCurRegisteringSize = 0;

            m_nTransCurCount = 0;
        }

        public void Copy(FileAddManage fileaddManage)
        {
            m_FileAddErrList = new List<FileAddErr>(fileaddManage.m_FileAddErrList);
            m_FileAddErrReason = new List<(string, string)>(fileaddManage.m_FileAddErrReason);
            ListFile = new List<string>(fileaddManage.ListFile);
        }

        public FileAddErr CreateFileAddErrInfo(string getFileName, string getFilePath, string getParentFileName)
        {
            FileAddErr createFile = new FileAddErr() { FileName = getFileName, FilePath = getFilePath, ParentFileName = getParentFileName };
            m_FileAddErrList.Add(createFile);
            return createFile;
        }
        //public void AddErrData(string strFilename, eFileAddErr err, string strFilePath, bool bSub = false, string strParentFileName = "")
        //{
        //    FileAddErr fileAddErr = new FileAddErr();
        //    fileAddErr.SetFileAddErr(strFilename, err, strFilePath, bSub, strParentFileName);
        //    m_FileAddErrList.Add(fileAddErr);

        //    Log.Information("[AddData] Cheked to Error[{Err}] File[{CurZipFile}] in {OrgZipFile}", err, strFilename, strFilePath);
        //}

        public void DataClear()
        {
            m_FileAddErrList.Clear();
            m_FileAddErrReason.Clear();
        }
        public int GetAddErrCount()
        {
            return m_FileAddErrList.Count;
        }

        /// <summary>
        /// OLE 개체 검출 함수 호출 후 반환된 Result Code의 Err 정보
        /// </summary>
        /// <param name="BaseResult"></param>
        /// <returns></returns>
        public eFileAddErr GetOLEError(int BaseResult) =>
            BaseResult switch
            {
                -1 => eFileAddErr.eFADOC_OLE_COMMONE, //공통
                -2 => eFileAddErr.eFADOC_OLE_PASSWORD,//암호화 되어 있을때
                -3 => eFileAddErr.eFADOC_OLE_COMMONE,//지원하지 않는 파일형식일때
                -4 => eFileAddErr.eFADOC_OLE_COMMONE,//outfolder 찾을 수 없을때
                -5 => eFileAddErr.eFADOC_OLE_COMMONE,//inputfile 파일을 찾을수 없을때
                -6 => eFileAddErr.eFADOC_OLE_COMMONE,//inputfile, outfolder null 일때
                -7 => eFileAddErr.eFADOC_OLE_COMMONE,//excel Workbook을 찾을 수 없을때
                -10 => eFileAddErr.eFADOC_COMPRESS_CHANGE, //압축형식의 위변조 파일 발생
                _ => eFileAddErr.eFADOC_OLE_COMMONE  //정의되지 않은 에러
            };

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
                case eFileAddErr.eFAFileSize:                           //  1회 전송가능 파일사이즈
                    str = xmlConf.GetTitle("T_eFAFileSize");                 // 1회 전송용량 초과
                    break;
                case eFileAddErr.eFAFileCount:                           //  1회 전송가능 파일 개수
                    str = xmlConf.GetTitle("T_eFAFileCount");                 // 1회 전송 파일 개수 초과
                    break;
                case eFileAddErr.eFANotFound:                                // 파일 찾기 실패
                    str = xmlConf.GetTitle("T_eFANotFound");                 // 찾을 수 없음
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
                    //str = xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH");					// 파일명 및 폴더명 길이초과(80자)
                    str = string.Format(xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH_NO_VAL"), m_nFileLengthMax); // 파일명 및 폴더명 길이초과(250자)
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
                                                                               // str = xmlConf.GetTitle("L_eFA_LONG_PATH");                 // 전송 길이초과(90자)
                    str = string.Format(xmlConf.GetTitle("L_eFA_LONG_PATH_NO_VAL"), m_nFilePathMax);
                    break;

                case eFileAddErr.eFA_LONG_PATH_PARENT:                                //상위폴더 길이초과
                                                                                      //str = xmlConf.GetTitle("L_eFA_LONG_PATH_PARENT");                 // 상위폴더명 길이초과(80자)
                    str = string.Format(xmlConf.GetTitle("L_eFA_LONG_PATH_PARENT_NO_VAL"), m_nFileLengthMax);
                    break;

                case eFileAddErr.eFA_LONG_PATH_FILEORPATH:                                //파일 및 폴더 길이초과
                                                                                          //str = xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH");                 // 파일명 및 폴더명 길이초과(80자)
                    str = string.Format(xmlConf.GetTitle("L_eFA_LONG_PATH_FILEORPATH_NO_VAL"), m_nFileLengthMax);
                    break;

                case eFileAddErr.eFA_FILE_READ_ERROR:                                // 파일 읽기 권한 오류
                    str = xmlConf.GetTitle("L_eFA_FILE_READ_ERROR");                 // 파일 읽기 권한 오류
                    break;

                #region OLD 관련 에러
                case eFileAddErr.eFADOC_OLE_COMMONE:
                    str = xmlConf.GetTitle("T_eOLE_COMMON");                                                  //OLE개체 추출 실패
                    break;
                case eFileAddErr.eFADOC_OLE_PASSWORD:
                    str = xmlConf.GetTitle("T_eOLE_PASSWORD");                                                  //문서 비밀번호 설정
                    break;
                case eFileAddErr.eFADOC_OLE_MIME:
                    str = xmlConf.GetTitle("T_eOLE_MIME");                                                      //OLE개체 마임타입
                    break;
                case eFileAddErr.eFADOC_OLE_EXTENSION:
                    str = xmlConf.GetTitle("T_eOLE_EXTENSION");                                                 //OLE개체 확장자 제한
                    break;
                case eFileAddErr.eFADOC_OLE_CHANGE:
                    str = xmlConf.GetTitle("T_eOLE_CHANGE");                                                    //OLE개체 위변조 제한
                    break;
                case eFileAddErr.eFADOC_COMPRESS_EXTENSION:
                    str = xmlConf.GetTitle("T_eCOMPRESS_EXTENSION");                                            //압축형식 확장자 제한
                    break;
                case eFileAddErr.eFADOC_COMPRESS_CHANGE:
                    str = xmlConf.GetTitle("T_eCOMPRESS_CHANGE");                                               //압축형식 위변조 제한
                    break;
                case eFileAddErr.eFADOC_OLE_REMAININGFILES:
                    str = xmlConf.GetTitle("eFADOC_OLE_REMAININGFILES");
                    break;
                //case eFileAddErr.eOLE_Exception:                                                            // 공통
                //    str = xmlConf.GetTitle("T_eOLE_Exception");
                //    break;
                //case eFileAddErr.eOLE_OEFileIsPasswordProtected:                                            // 암호화 되어있을때 
                //    str = xmlConf.GetTitle("T_eOLE_OEFileIsPasswordProtected");
                //    break;
                //case eFileAddErr.eOLE_OEFileTypeNotSupported:                                               // 지원하지 않는 파일형식일때
                //    str = xmlConf.GetTitle("T_eOLE_OEFileTypeNotSupported");
                //    break;
                //case eFileAddErr.eOLE_DirectoryNotFoundException:                                           // outfolder 찾을 수 없을때
                //    str = xmlConf.GetTitle("T_eOLE_DirectoryNotFoundException");
                //    break;
                //case eFileAddErr.eOLE_FileNotFoundException:                                                // inputfile 파일을 찾을수 없을때
                //    str = xmlConf.GetTitle("T_eOLE_FileNotFoundException");
                //    break;
                //case eFileAddErr.eOLE_ArgumentNullException:                                                // inputfile, outfolder null 일때
                //    str = xmlConf.GetTitle("T_eOLE_ArgumentNullException");
                //    break;
                //case eFileAddErr.eOLE_OEFileIsCorrupt:                                                      // Excel workbook 찾을 수 없을때, 
                //    str = xmlConf.GetTitle("T_eOLE_OEFileIsCorrupt");
                //    break;
                //case eFileAddErr.eOLE_UnDefinedError:                                                       // 정의되지 않은 에러
                //    str = xmlConf.GetTitle("T_eOLE_UnDefinedError");
                //    break;
                //case eFileAddErr.eOLE_Inner_Ext:                                                            //문서내 OLE 개체의 확장자 제한 파일 포함
                //    str = xmlConf.GetTitle("T_eOLE_Inner_Ext");
                //    break;
                #endregion

                default:
                    str = "-";
                    break;

            }
            return str;
        }

        public string GetExceptionCountString(int count)
        {
            string str = xmlConf.GetTitle("T_ETC_FAEXCEPTIONCOUNT");                // {0} 개
            str = String.Format(str, count);
            return str;
        }
        /// <summary>
        /// 확장자 제한에 걸린 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
        public int GetExtExceptionCount()
        {
            int nTotalCount = GetAddErrCount();
            if (nTotalCount <= 0)
                return nTotalCount;

            int count = 0;
            for (int i = 0; i < nTotalCount; i++)
            {
                eFileAddErr e = m_FileAddErrList[i].eErrType;
                if (e == eFileAddErr.eFAEXT)                     // 확장자 제한
                    count++;
            }
            return count;
        }

        /// <summary>
        /// zip 파일 내 확장자 제한에 걸린 파일의 개수를 반환
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 파일 위변조에 걸린 파일의 개수를 반환한다
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 파일 사이즈 초과된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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
        public int GetCountExceptionCount()
        {
            int nTotalCount = GetAddErrCount();
            if (nTotalCount <= 0)
                return nTotalCount;

            int count = 0;
            for (int i = 0; i < nTotalCount; i++)
            {
                eFileAddErr e = m_FileAddErrList[i].eErrType;
                if (e == eFileAddErr.eFAFileCount)                     // 1회 전송가능 파일 갯수
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 존재하지 않는 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 숨김 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 비번 있을 때 제외된 파일의 개수를 반환
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 비번 없을 때 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일이 손상되어 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 빈파일로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 알수 없는 파일 형식으로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 일일 전송횟수 제한으로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 일일 전송사이즈 제한으로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 전송길이 초과로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 상위폴더명 길이 초과로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// 파일명 및 폴더명 길이 초과로 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 zip 파일 Open 실패한 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 zip 파일 비번 사용 중인 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 확장자 제한 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 확장자 변경 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 알수 없는 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 빈 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 zip Length Over 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 검사 후 남아있는 zip 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// zip 파일의 내부 DRM 파일 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 읽기 권한이 없어 제외된 파일의 개수를 반환한다.
        /// </summary>
        /// <returns></returns>
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

            if (nStandardSize < nRegSize)       // 단일크기가 전송가능 Size보다 클때
                return false;

            if (nStandardSize < nAddedTotalSize)       // 추가로 누적된 크기가 전송가능 Size보다 클때
                return false;

            return true;
        }

        public bool GetCountEnable(int nStandardCount, int nRegCount)
        {
            int nAddedTotalCount = m_nTransCurCount + nRegCount;

            if (nStandardCount <= 0)        //사용가능한 수량이 없는 경우
                return false;

            if (nStandardCount < nRegCount)     //등록할 파일 수량이 가능 수량보다 큰 경우
                return false;

            if (nStandardCount < nAddedTotalCount) //누적 등록 파일 수량이 가능 수량보다 큰 경우
                return false;

            return true;
        }

        public bool GetDaySizeEnable(long FileTransMaxSize, long RemainFileTransSize, long nRegSize)
        {
            if (FileTransMaxSize <= 0)
                return false;

            if (RemainFileTransSize < nRegSize)
                return false;
            return true;
        }

        /// <summary>
        /// 일일 전송 횟수 확인
        /// </summary>
        /// <param name="DayFileTransCountStandard"></param>
        /// <param name="RegCount"></param>
        /// <returns></returns>
        public bool GetDayCountEnable(int DayFileTransCountStandard, int DayFileTransCountRemain, int RegCount = 1)
        {
            if (DayFileTransCountRemain <= 0)
                return false;

            if (DayFileTransCountStandard < RegCount)           //등록 시도 횟수가 일일 전송 횟수를 초과할 경우
                return false;

            return true;
        }

        /// <summary>
        /// nRegSize 가 0보다 큰 경우 True
        /// </summary>
        /// <param name="nRegSize"></param>
        /// <returns></returns>
        public bool GetEmptyEnable(long nRegSize)
        {
            if (nRegSize <= 0)
                return false;
            return true;
        }
        public bool GetRegExtEnable(bool bWhite, string strStandardFileExtInfo, string strExt)
        {
            if ((strStandardFileExtInfo.Equals("")) || (strStandardFileExtInfo.Equals(";")))
                return !bWhite;

            char sep = (char)';';
            string[] strExtList = strStandardFileExtInfo.Split(sep);
            int count = strExtList.Length;
            if (count <= 0)
                return !bWhite;

            bool bFind = false;
            for (int i = 0; i < count; i++)
            {
                // if (strExtList[i].Equals(strExt))
                if (String.Compare(strExtList[i], strExt, true) == 0)
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

            strFileName = SgExtFunc.hsFileRename(bMode, strFileName);

            return strFileName;
        }

        public string GetConvertTitleDesc(bool bMode, string str)
        {
            if (bMode)
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
                str = str.Replace("&lt;", "<");
                str = str.Replace("&gt;", ">");
                str = str.Replace("$ET;", "\n");
                str = str.Replace("&quot", "\"");
                str = str.Replace("&apos", "\'");
                str = str.Replace("&amp", "&");
            }
            if (!str.IsNormalized(NormalizationForm.FormC))
                str = str.Normalize(NormalizationForm.FormC);
            return str;
        }
        private bool FilePathLength(string strFileRelativePath)
        {
            string strFileReName = strFileRelativePath;
            /*string strFileReName = GetFileRename(true, strFileRelativePath);
            byte[] temp = Encoding.Default.GetBytes(strFileReName);
            strFileReName = Encoding.UTF8.GetString(temp);*/

            Log.Logger.Here().Information("FilePath Length - Check(MaxLength:{0}) : filename : {1}(length : {2})", m_nFilePathMax, strFileReName, strFileReName.Length);
            if (strFileReName.Length > m_nFilePathMax)                          // 전체 경로 길이 확인 (90 / 250자)
            {
                Log.Logger.Here().Error("FilePath Length - Check(MaxLength:{0}) : filename : {1}(length : {2})", m_nFilePathMax, strFileReName, strFileReName.Length);
                return false;
            }
            return true;
        }

        private bool FileFolderNameLength(string strFileRelativePath, out bool bSuper)
        {
            string strFileReName = strFileRelativePath;

            // 특수문자로 변환전 길이에 대해서 체크
            /*string strFileReName = GetFileRename(true, strFileRelativePath);
            byte[] temp = Encoding.Default.GetBytes(strFileReName);
            strFileReName = Encoding.UTF8.GetString(temp);*/

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
            for (index = 0; index < strUnitPath.Length; index++)
            {
                string strName = strUnitPath[index];
                Log.Logger.Here().Information("FileFolderName Length - Check(MaxLength:{0}) : filename : {1}(length : {2})", m_nFilePathMax, strName, strName.Length);
                if (strName.Length > m_nFileLengthMax)                                       // 폴더 및 파일 경로 길이 확인 (80자 / 250자)
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

        //public void SetFileReadError(HsStream hsStream)
        //{
        //    string strFileName = hsStream.FileName;
        //    string strRelativePath = hsStream.RelativePath;
        //    AddData(strFileName, eFileAddErr.eFA_FILE_READ_ERROR, strRelativePath);
        //}

        /// <summary>
        /// 확장자 위변조 확인
        /// </summary>
        /// <param name="hsStream"></param>
        /// <returns></returns>
        public async Task<eFileAddErr> GetExamFileExtChange(HsStream hsStream)
        {
            eFileAddErr enRet;
            string strExt = Path.GetExtension(hsStream.FileName);
            enRet = await IsValidFileExt(hsStream.stream, strExt);
            return enRet;
            //if (enRet != eFileAddErr.eFANone)
            //{
            //    //string strFileName = hsStream.FileName;
            //    //string strRelativePath = hsStream.RelativePath;
            //    //AddData(strFileName, enRet, strRelativePath);
            //    return -1;
            //}
            //return 0;
        }

        /// <summary>
        /// 사이즈,횟수,Black/White 리스트, 숨김, 이름길이 등 체크
        /// </summary>
        /// <param name="hsStream"></param>
        /// <param name="bWhite"></param>
        /// <param name="strFileExtInfo"></param>
        /// <param name="bHidden"></param>
        /// <param name="ConvEnableSize">1회 전송가능한 사이즈</param>
        /// <param name="RegSize">등록할 파일의 사이즈</param>
        /// <param name="FileTransMaxSize">일일 전송가능한 사이즈</param>
        /// <param name="RemainFileTransSize">일일 잔여 사이즈</param>
        /// <param name="EnableFileCount">1회 전송가능한 파일 갯수</param>
        /// <param name="DayCountStandard">일일 전송 횟수</param>
        /// <param name="DayCountRemain">일일 전송 잔여 횟수</param>
        /// <param name="RegFileCount">등록할 파일의 갯수</param>
        /// <returns></returns>
        public bool GetExamFileAddEnable(HsStream hsStream, FileAddErr currentFile, bool bWhite, string strFileExtInfo, bool bHidden, long ConvEnableSize, long RegSize, long FileTransMaxSize, long RemainFileTransSize, int EnableFileCount, int DayCountStandard, int DayCountRemain, int RegFileCount)
        {
            if (hsStream == null)
                return true;

            bool bSizeEnable = false;                       //1회 전송가능 사이즈 용량 검사 결과
            bool bCountEnable = false;                      //1회 전송가능 파일 갯수 검사 결과.

            bool bDaySizeEnable = false;                    // 일일 전송 사이즈 용량 검사 결과.
            bool bDayCountEnable = false;                   //일일 전송 횟수 검사 결과.

            bool bExtEnable = false;                        // 확장자 제한 검사 결과
            bool bHiddenEnable = false;                     // 숨김 파일인지 검사 결과
            bool bFilePathEnable = false;                   // 긴파일명 전체 경로 길이 검사
            bool bFileFolderNameEnable = false;             // 폴더 및 파일 경로 길이 확인 (80자)
            bool bEmpty = false;                            // 빈파일인지 여부 검사 


            //파일 전송 시 체크했던 제한 사항을 파일 등록 시 체크하도록 추가(1회 파일갯수,일일 전송사이즈, 일일 전송횟수) by 2022.08.19 KYH

            //1회 전송가능 용량 제한
            bSizeEnable = GetSizeEnable(ConvEnableSize, RegSize);       //1회 전송가능 용량 제한
            if (!bSizeEnable)
            {
                currentFile.eErrType = eFileAddErr.eFAFileSize;
                return false;
            }

            //1회 전송가능 파일 갯수
            bCountEnable = GetRegCountEnable(EnableFileCount, RegFileCount);    //1회 전송가능 파일 갯수
            if (!bCountEnable)
            {
                currentFile.eErrType = eFileAddErr.eFAFileCount;
                return false;
            }

            //일일 전송가능 횟수
            bDayCountEnable = GetDayCountEnable(DayCountStandard, DayCountRemain, 1);
            if (!bDayCountEnable)
            {
                currentFile.eErrType = eFileAddErr.eFADAYCOUNTOVER;
                return false;
            }

            //일일 전송가능 용량 제한
            bDaySizeEnable = GetDaySizeEnable(FileTransMaxSize, RemainFileTransSize, RegSize);
            if (!bDaySizeEnable)
            {
                currentFile.eErrType = eFileAddErr.eFADAYSIZEOVER;
                return false;
            }

            //black, white 리스트 체크
            bExtEnable = GetRegExtEnable(bWhite, strFileExtInfo, hsStream.Type);
            if (!bExtEnable)
            {
                currentFile.eErrType = eFileAddErr.eFAEXT;
                return false;
            }

            //숨김파일 체크
            bHiddenEnable = (!bHidden);
            if (!bHiddenEnable)
            {
                currentFile.eErrType = eFileAddErr.eFAHidden;
                return false;
            }

            //OS 지원 전송길이 체크
            bFilePathEnable = FilePathLength(hsStream.RelativePath);
            if (!bFilePathEnable)
            {
                currentFile.eErrType = eFileAddErr.eFA_LONG_PATH;
                return false;
            }

            //폴더명, 파일명 길이 체크
            bool bSuper = false;    // 
            bFileFolderNameEnable = FileFolderNameLength(hsStream.RelativePath, out bSuper);
            if (!bFileFolderNameEnable)
            {
                currentFile.eErrType = (bSuper) ? eFileAddErr.eFA_LONG_PATH_PARENT          //상위폴더 길이 초과
                                                : eFileAddErr.eFA_LONG_PATH_FILEORPATH;     //파일 및 폴더명 길이 초과
                return false;
            }

            //빈파일 체크 (0kb 허용)
            bEmpty = (bEmptyFIleNoCheck || GetEmptyEnable(hsStream.Size));//GetRegFileEmptyEnable(hsStream.Size);
            if (!bEmpty)
            {
                currentFile.eErrType = eFileAddErr.eFAEMPTY;
                return false;
            }

            bool bRet = (bExtEnable & bHiddenEnable & bFilePathEnable & bFileFolderNameEnable & bEmpty);
            if (bRet)
            {
                m_nCurRegisteringSize = RegSize;
                m_nTansCurSize += RegSize;
                m_nTransCurCount += RegFileCount;

                currentFile.eErrType = eFileAddErr.eFANone;
            }
            return bRet;
        }

        #region [간소화로 사용안함]
        ///// <summary>
        ///// 1회 전송가능 용량 제한
        ///// </summary>
        ///// <param name="ConvEnableSize"></param>
        ///// <param name="RegSize"></param>
        ///// <param name="strExt"></param>
        ///// <param name="strFileName"></param>
        ///// <param name="strRelativePath"></param>
        ///// <returns></returns>
        //public bool GetRegSizeEnable(FileAddErr currentFile, long ConvEnableSize, long RegSize, string strExt, string strFileName, string strRelativePath)
        //{
        //    if (GetSizeEnable(ConvEnableSize, RegSize) != true)
        //    {

        //        return false;
        //    }

        //    return true;
        //}
        ///// <summary>
        ///// 1회 전송가능 파일 갯수
        ///// </summary>
        ///// <param name="EnableCount">1회 전송가능 파일 갯수 기준</param>
        ///// <param name="RegCount">전송할 파일 갯수</param>
        ///// <param name="strFileName"></param>
        ///// <param name="strRelativePath"></param>
        ///// <returns></returns>
        //public bool GetRegCountEnable(FileAddErr currentFile, int EnableCount, int RegCount, string strFileName, string strRelativePath)
        //{
        //    if (GetCountEnable(EnableCount, RegCount) != true)
        //    {
        //        currentFile.eErrType = eFileAddErr.eFAFileCount;
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 일일 전송가능 용량 제한
        ///// </summary>
        ///// <param name="FileTransMaxSize"></param>
        ///// <param name="RemainFileTransSize"></param>
        ///// <param name="RegSize"></param>
        ///// <param name="strFileName"></param>
        ///// <param name="strRelativePath"></param>
        ///// <returns></returns>
        //public bool GetDayRegSizeEnable(long FileTransMaxSize, long RemainFileTransSize, long RegSize, string strFileName, string strRelativePath)
        //{
        //    if (GetDaySizeEnable(FileTransMaxSize, RemainFileTransSize, RegSize) != true)
        //    {
        //        AddData(strFileName, eFileAddErr.eFADAYSIZEOVER, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 일일 전송 횟수
        ///// </summary>        
        ///// <param name="DayTransCountStandard">일일 전송 횟수 기준</param>
        ///// <param name="RegCount">등록 횟수( 기본 : 1)</param>
        ///// <param name="DayTransCountRemain">일일 전송 잔여 횟수</param>
        ///// <returns></returns>
        //public bool GetDayRegCountEnable(int DayTransCountStandard, int DayTransCountRemain, int RegCount, string strFileName, string strRelativePath)
        //{
        //    if (GetDayCountEnable(DayTransCountStandard, DayTransCountRemain, RegCount) != true)
        //    {
        //        AddData(strFileName, eFileAddErr.eFADAYCOUNTOVER, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Black , White 리스트 체크
        ///// </summary>
        ///// <param name="bWhite"></param>
        ///// <param name="strFileExtInfo"></param>
        ///// <param name="strExt"></param>
        ///// <param name="strFileName"></param>
        ///// <param name="strRelativePath"></param>
        ///// <returns></returns>
        //public bool GetRegExtEnable(bool bWhite, string strFileExtInfo, string strExt, string strFileName, string strRelativePath)
        //{
        //    if (GetRegExtEnable(bWhite, strFileExtInfo, strExt) != true)
        //    {
        //        AddData(strFileName, eFileAddErr.eFAEXT, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool GetRegHiddenEnable(bool bHidden, string strFileName, string strRelativePath)
        //{
        //    if (bHidden)
        //    {
        //        AddData(strFileName, eFileAddErr.eFAHidden, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool GetRegFilePathEnable(string strFileName, string strRelativePath)
        //{
        //    if (FilePathLength(strRelativePath) != true)
        //    {
        //        AddData(strFileName, eFileAddErr.eFA_LONG_PATH, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool GetRegFileFolderNameEnable(string strFileName, string strRelativePath)
        //{
        //    bool bSuper = false;
        //    if (FileFolderNameLength(strRelativePath, out bSuper) != true)
        //    {
        //        if (bSuper)
        //            AddData(strFileName, eFileAddErr.eFA_LONG_PATH_PARENT, strRelativePath);                    // 상위폴더 길이 초과
        //        else
        //            AddData(strFileName, eFileAddErr.eFA_LONG_PATH_FILEORPATH, strRelativePath);                // 파일 및 폴더명 길이 초과
        //        return false;
        //    }
        //    return true;
        //}
        //public bool GetRegFileEmptyEnable(string strFileName, string strRelativePath, long nSize)
        //{

        //    // 0kb 파일 허용
        //    if (bEmptyFIleNoCheck == false && GetEmptyEnable(nSize) != true)
        //    {
        //        AddData(strFileName, eFileAddErr.eFAEMPTY, strRelativePath);
        //        return false;
        //    }
        //    return true;
        //} 
        #endregion

        ///// <summary>
        ///// 등록 시도 파일 중 오류 난 파일의 오류 상세 항목 표시 List
        ///// </summary>
        ///// <returns></returns>
        //public List<FileAddErr> GetDisplayError()
        //{
        //    //에러 발생한 파일들만 표시할지 결정 필요
        //    foreach (FileAddErr file in m_FileAddErrList)
        //    {

        //    }

        //    //현재는 등록된 파일 전부 표시
        //    return m_FileAddErrList;
        //}

        //private List<FileAddErr> getDisplayError()
        //{ }

        /// <summary>
        /// 파일검사하며 등록한 에러 리스트 항목에 사유 및 Tree Source 구성
        /// </summary>
        /// <param name="ListReason"></param>
        /// <param name="ListDisaplayErrSource"></param>
        /// <returns></returns>
        public bool GetReasonAndDisplayOfErrSource(out List<(string reason, string count)> ListReason, out List<FileAddErr> ListDisaplayErrSource)
        {
            m_FileAddErrReason.Clear();
            FileAddErr fileAddErr = new FileAddErr();
            Dictionary<eFileAddErr, int> fileAddErrReason = new Dictionary<eFileAddErr, int>();

            bool hasErr = getReasonAndDisplaySource(m_FileAddErrList, ref fileAddErrReason);

            string strReason, strCount = "";
            foreach (eFileAddErr err in fileAddErrReason.Keys)
            {
                strReason = SetExceptionReason(err);
                strCount = GetExceptionCountString(fileAddErrReason[err]);
                m_FileAddErrReason.Add((strReason, strCount));
            }
            ListReason = m_FileAddErrReason;

            //Parent 혹은 Children의 Err를 가진 항목만 필터링 하여 표시
            ListDisaplayErrSource = m_FileAddErrList.FindAll(file => file.eErrType != eFileAddErr.eFANone || file.HasChildrenErr);
            #region [간소화로 사용안함]
            //string strReason = "";
            //string strCount = "";

            //FileAddErr fileAddErr = new FileAddErr();

            //int nExtExceptionCount = 0;
            //nExtExceptionCount = GetExtExceptionCount();
            //if (nExtExceptionCount > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAEXT);
            //    strCount = fileAddErr.GetExceptionCountString(nExtExceptionCount);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nChangeExceptionCount = 0;
            //nChangeExceptionCount = GetChangeExceptionCount();
            //if (nChangeExceptionCount > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFACHG);
            //    strCount = fileAddErr.GetExceptionCountString(nChangeExceptionCount);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nHiddenExceptionCount = 0;
            //nHiddenExceptionCount = GetHiddenExceptionCount();
            //if (nHiddenExceptionCount > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAHidden);
            //    strCount = fileAddErr.GetExceptionCountString(nHiddenExceptionCount);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}
            //int nEmptyExceptionCount = 0;
            //nEmptyExceptionCount = GetEmptyExceptionCount();
            //if (nEmptyExceptionCount > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAEMPTY);
            //    strCount = fileAddErr.GetExceptionCountString(nEmptyExceptionCount);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nFilePathOverExcetpion = 0;
            //nFilePathOverExcetpion = GetFilePathOverExceptionCount();
            //if (nFilePathOverExcetpion > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH);
            //    strCount = fileAddErr.GetExceptionCountString(nFilePathOverExcetpion);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nSuperFolderNameOverException = 0;
            //nSuperFolderNameOverException = GetSuperFolderNameOverExceptionCount();
            //if (nSuperFolderNameOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_PARENT);
            //    strCount = fileAddErr.GetExceptionCountString(nSuperFolderNameOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nFileFolderNameOverException = 0;
            //nFileFolderNameOverException = GetFileFolderNameOverExceptionCount();
            //if (nFileFolderNameOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_LONG_PATH_FILEORPATH);
            //    strCount = fileAddErr.GetExceptionCountString(nFileFolderNameOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nFileSizeOverException = 0;
            //nFileSizeOverException = GetSizeExceptionCount();
            //if (nFileSizeOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAFileSize);
            //    strCount = fileAddErr.GetExceptionCountString(nFileSizeOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nFileCountOverException = GetCountExceptionCount();
            //if (nFileCountOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAFileCount);
            //    strCount = fileAddErr.GetExceptionCountString(nFileCountOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}



            //int nFileReadAccessException = 0;
            //nFileReadAccessException = GetReadDenyCount();
            //if (nFileReadAccessException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFA_FILE_READ_ERROR);
            //    strCount = fileAddErr.GetExceptionCountString(nFileReadAccessException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}


            //int nFileDaySizeOverException = 0;
            //nFileDaySizeOverException = GetDaySizeOverExceptionCount();
            //if (nFileDaySizeOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFADAYSIZEOVER);
            //    strCount = fileAddErr.GetExceptionCountString(nFileDaySizeOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nFileDayCountOverException = GetDayCountOverExceptionCount();
            //if (nFileDayCountOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFADAYCOUNTOVER);
            //    strCount = fileAddErr.GetExceptionCountString(nFileDayCountOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}


            //int nZIpPWException = 0;
            //nZIpPWException = GetZipPWExceptionCount();
            //if (nZIpPWException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipPW);
            //    strCount = fileAddErr.GetExceptionCountString(nZIpPWException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nZIpNotPWException = 0;
            //nZIpNotPWException = GetZipNotPWExceptionCount();
            //if (nZIpNotPWException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipNotPW);
            //    strCount = fileAddErr.GetExceptionCountString(nZIpNotPWException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nZIpErrorException = 0;
            //nZIpErrorException = GetZipErrorExceptionCount();
            //if (nZIpErrorException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZipError);
            //    strCount = fileAddErr.GetExceptionCountString(nZIpErrorException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nZipException = 0;
            //nZipException = GetZipExtExceptionCount();
            //if (nZipException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eFAZIP);
            //    strCount = fileAddErr.GetExceptionCountString(nZipException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipOpenFailException = 0;
            //nInnerZipOpenFailException = GetInnerZipOpenFailCount();
            //if (nInnerZipOpenFailException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerZipOpenFail);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipOpenFailException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipPassWordException = 0;
            //nInnerZipPassWordException = GetInnerZipPassWordCount();
            //if (nInnerZipPassWordException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerZipPassword);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipPassWordException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipExtException = 0;
            //nInnerZipExtException = GetInnerZipExtCount();
            //if (nInnerZipExtException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExt);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipExtException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipExtChangeException = 0;
            //nInnerZipExtChangeException = GetInnerZipExtChangeCount();
            //if (nInnerZipExtChangeException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExtChange);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipExtChangeException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipExtUnKnownException = 0;
            //nInnerZipExtUnKnownException = GetInnerZipUnKnownCount();
            //if (nInnerZipExtUnKnownException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerExtUnknown);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipExtUnKnownException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipFileEmptyException = 0;
            //nInnerZipFileEmptyException = GetInnerZipEmptyCount();
            //if (nInnerZipFileEmptyException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerFileEmpty);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipFileEmptyException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipLengthOverException = 0;
            //nInnerZipLengthOverException = GetInnerZipLengthOverCount();
            //if (nInnerZipLengthOverException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerLengthOver);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipLengthOverException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipLeftZipException = 0;
            //nInnerZipLeftZipException = GetInnerZipLeftZipCount();
            //if (nInnerZipLeftZipException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerLeftZip);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipLeftZipException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //int nInnerZipDRMException = 0;
            //nInnerZipDRMException = GetInnerZipDRMCount();
            //if (nInnerZipDRMException > 0)
            //{
            //    strReason = fileAddErr.SetExceptionReason(eFileAddErr.eUnZipInnerDRM);
            //    strReason = " => " + strReason;
            //    strCount = fileAddErr.GetExceptionCountString(nInnerZipDRMException);
            //    strReason = strReason + " : " + strCount;
            //    m_FileAddErrReason.Add(strReason);
            //    strReason = "";
            //}

            //return m_FileAddErrReason; 
            #endregion

            return hasErr;
        }
        /// <summary>
        /// 하위 폴더,파일들까지 Reason 세팅 및 에러 존재 여부(HasChildren 세팅
        /// </summary>
        /// <param name="getFileAddErrList"></param>
        /// <param name="getFileAddErrReason"></param>
        private bool getReasonAndDisplaySource(List<FileAddErr> getFileAddErrList, ref Dictionary<eFileAddErr, int> getFileAddErrReason)
        {
            bool includeErr = false;
            foreach (FileAddErr err in getFileAddErrList)
            {
                if (err.eErrType != eFileAddErr.eFANone)
                {
                    includeErr = true;

                    //Tree에 표시할 사유 세팅
                    err.ExceptionReason = SetExceptionReason(err.eErrType);
                    int errCnt = getFileAddErrReason.FirstOrDefault(i => i.Key == err.eErrType).Value;
                    getFileAddErrReason[err.eErrType] = ++errCnt;
                }

                //하위 파일 Err 확인
                if (err.ChildrenFiles != null && err.ChildrenFiles.Count > 0)
                {
                    bool hasChildrenErr = getReasonAndDisplaySource(err.ChildrenFiles, ref getFileAddErrReason);
                    err.HasChildrenErr = hasChildrenErr;
                    includeErr = includeErr || hasChildrenErr;
                }
            }
            return includeErr;
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
            if (!str.Equals(""))
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

        /// <summary>
        /// EGG 파일인지 검사한다. (EGG 파일)(strExt 확장자 명을 받아올 버퍼)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsEGG(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x45, 0x47, 0x47, 0x41 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// 워드 문서인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsWord(byte[] btFileData, string strExt)
        {
            Log.Debug("**** IsWord(), ");

            if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
                FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true)
            {

                if (String.Compare(strExt, "doc", true) == 0)
                {
                    if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("theme")) == true)
                        return true;
                }

                if (String.Compare(strExt, "docx", true) == 0)
                {
                    if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("docProps")) == true &&
                        FindZipContent(btFileData, Encoding.UTF8.GetBytes("word")) == true)
                        return true;
                }
            }

            if (String.Compare(strExt, "doc", true) == 0)
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


        /// <summary>
        /// 엑셀 문서인지 검사한다.(param strExt 확장자명)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsXls(byte[] btFileData, string strExt)
        {
            if (FindZipContent(btFileData, Encoding.UTF8.GetBytes("_rels")) == true &&
                FindZipContent(btFileData, Encoding.UTF8.GetBytes("[Content_Types].xml")) == true)
            {
                if (String.Compare(strExt, "xls", true) == 0 && FindZipContent(btFileData, Encoding.UTF8.GetBytes("drs")) == true) return true;
                if (String.Compare(strExt, "xlsx", true) == 0 && FindZipContent(btFileData, Encoding.UTF8.GetBytes("xl")) == true) return true;
            }

            if (String.Compare(strExt, "xls", true) == 0)
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
            if (String.Compare(strExt, "ppt", true) == 0)
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

            if (String.Compare(strExt, "pptx", true) == 0 &&
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

        /// <summary>
        /// 한글 문서인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// PDF 문서인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsPDF(byte[] btFileData, string strExt)
        {
            if (ByteArrayCompare(btFileData, Encoding.UTF8.GetBytes("%PDF-")) == true) return true;

            return false;
        }

        /// <summary>
        /// jpg 파일인지 검사한다.(strExt 확장자 명을 받아올 버퍼)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsJPG(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header;

            btHLP_Header = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4a, 0x46, 0x49, 0x46, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            btHLP_Header = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// gif 파일인지 검사한다.(strExt 확장자 명을 받아올 버퍼)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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


        /// <summary>
        /// png 파일인지 검사한다.(strExt 확장자 명을 받아올 버퍼)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsPNG(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// bmp 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsBMP(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x42, 0x4d };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// DWF 파일인지 검사한다. (CAD 관련 파일, 도면 교환 파일 ASCII 또는 이진)(true:dwg)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsDWF(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x28, 0x44, 0x57, 0x46, 0x20, 0x56 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// rar 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsRAR(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// arj 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsARJ(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x60, 0xEA };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// iso 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// jar 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsJAR(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// msg 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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


        /// <summary>
        /// msi 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsMSI(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0x00, 0x04, 0x00, 0xFE, 0xFF, 0x0C, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// com 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsCOM(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// scr 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsSCR(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// ocx 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsOCX(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4D, 0x5A };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// arc 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// lha 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsLHA(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x2D, 0x6C, 0x68 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }


        /// <summary>
        /// lzh 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsLZH(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x2D, 0x6C, 0x68 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// pak 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsPAK(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x1A, 0x0B };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }


        /// <summary>
        /// tar 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsTAR(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x42, 0x5A, 0x68 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }


        /// <summary>
        /// tbz 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsTGZ(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x1F, 0x8B, 0x08 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// zoo 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsZOO(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x5A, 0x4F, 0x4F, 0x20 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// DWG 파일인지 검사한다. (CAD 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsDWG(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x41, 0x43, 0x31, 0x30 };//
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// LNK 파일인지 검사한다. (바로가기 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsLNK(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4C, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// OBJ 파일인지 검사한다. (오브젝트)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsOBJ(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4C, 0x01 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// HLP 파일인지 검사한다. (Windows Help File)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsHLP(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header;
            btHLP_Header = new byte[] { 0x3F, 0x5F, 0x03, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            btHLP_Header = new byte[] { 0x4C, 0x4E, 0x02, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// DER 파일인지 검사한다. (공인인증서 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsDER(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x30, 0x82 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// MP3 파일인지 검사한다. (MP3 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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


        /// <summary>
        /// MGB 파일인지 검사한다. (마이다스 CAD관련 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsMGB(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x4D, 0x47, 0x45, 0x4E };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// CAD 관련 STL 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsSTL(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x53, 0x54, 0x4C, 0x42, 0x20, 0x41, 0x54, 0x46 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// HPT 파일인지 검사한다. (슬라이드쇼 관련 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsHPT(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x52, 0x6F, 0x62, 0x75, 0x73, 0x20, 0x44, 0x61, 0x20, 0x46, 0x69, 0x6C, 0x65, 0x00 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// Matroska media containter, including WebM
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsMKV(byte[] btFileData, string strExt)
        {
            // https://en.wikipedia.org/wiki/List_of_file_signatures
            byte[] btHLP_Header = new byte[] { 0x1A, 0x45, 0xDF, 0xA3 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// EPS 파일인지 검사한다. (Adobe PostScript)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// CHM 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsCHM(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x49, 0x54, 0x53, 0x46 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }


        /// <summary>
        /// MIF 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsMIF(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header;
            btHLP_Header = new byte[] { 0x3C, 0x4D, 0x61, 0x6B, 0x65, 0x72, 0x46, 0x69 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            btHLP_Header = new byte[] { 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// CVD 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsCVD(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x43, 0x6C, 0x61, 0x6D, 0x41, 0x56, 0x2D, 0x56, 0x44, 0x42 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// SAS7BDAT 파일인지 검사한다.
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsSAS7BDAT(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC2, 0xEA,
                0x81, 0x60, 0xB3, 0x14, 0x11, 0xCF, 0xBD, 0x92, 0x08, 0x00, 0x09, 0xC7, 0x31, 0x8C, 0x18, 0x1F, 0x10 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// ALZ 파일인지 검사한다. (ALZ 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        private static bool IsALZ(byte[] btFileData, string strExt)
        {
            byte[] btHLP_Header = new byte[] { 0x41, 0x4C, 0x5A, 0x01, 0x0A, 0x00, 0x00, 0x00, 0x42, 0x4C, 0x5A };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true) return true;

            return false;
        }

        /// <summary>
        /// pst 파일인지 검사한다. (PST 파일)
        /// </summary>
        /// <param name="btFileData"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
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
                if (String.Compare(strExt, "egg", true) == 0) return IsEGG(btFileData, strExt);

                if (String.Compare(strExt, "doc", true) == 0 || String.Compare(strExt, "docx", true) == 0)
                    return IsWord(btFileData, strExt);

                if (String.Compare(strExt, "xls", true) == 0 || String.Compare(strExt, "xlsx", true) == 0)
                    return IsXls(btFileData, strExt);

                if (String.Compare(strExt, "ppt", true) == 0 || String.Compare(strExt, "pptx", true) == 0)
                    return IsPPT(btFileData, strExt);

                if (String.Compare(strExt, "xps", true) == 0) return IsXPS(btFileData, strExt);

                if (String.Compare(strExt, "hwp", true) == 0) return IsHWP(btFileData, strExt);

                if (String.Compare(strExt, "txt", true) == 0 || String.Compare(strExt, "log", true) == 0 ||
                    String.Compare(strExt, "ini", true) == 0 || String.Compare(strExt, "sql", true) == 0 ||
                    String.Compare(strExt, "conf", true) == 0)
                    return IsTXT(btFileData, strExt);

                /* 이미지 파일*/
                if (String.Compare(strExt, "pdf", true) == 0) return IsPDF(btFileData, strExt);
                if (String.Compare(strExt, "jpg", true) == 0) return IsJPG(btFileData, strExt);
                if (String.Compare(strExt, "gif", true) == 0) return IsGIF(btFileData, strExt);
                if (String.Compare(strExt, "png", true) == 0) return IsPNG(btFileData, strExt);
                if (String.Compare(strExt, "bmp", true) == 0) return IsBMP(btFileData, strExt);

                /* CAD 파일 */
                if (String.Compare(strExt, "dwf", true) == 0) return IsDWF(btFileData, strExt);

                /* 압축파일 */
                if (String.Compare(strExt, "rar", true) == 0) return IsRAR(btFileData, strExt);
                if (String.Compare(strExt, "arj", true) == 0) return IsARJ(btFileData, strExt);
                if (String.Compare(strExt, "iso", true) == 0) return IsISO(btFileData, strExt);
                if (String.Compare(strExt, "jar", true) == 0) return IsJAR(btFileData, strExt);

                /* 기타파일 */
                if (String.Compare(strExt, "msg", true) == 0) return IsMSG(btFileData, strExt);

                if (String.Compare(strExt, "msi", true) == 0) return IsMSI(btFileData, strExt);

                if (String.Compare(strExt, "com", true) == 0) return IsCOM(btFileData, strExt);
                if (String.Compare(strExt, "scr", true) == 0) return IsSCR(btFileData, strExt);
                if (String.Compare(strExt, "ocx", true) == 0) return IsOCX(btFileData, strExt);

                if (String.Compare(strExt, "arc", true) == 0) return IsARC(btFileData, strExt);
                if (String.Compare(strExt, "lha", true) == 0) return IsLHA(btFileData, strExt);
                if (String.Compare(strExt, "lzh", true) == 0) return IsLZH(btFileData, strExt);
                if (String.Compare(strExt, "pak", true) == 0) return IsPAK(btFileData, strExt);
                if (String.Compare(strExt, "tar", true) == 0) return IsTAR(btFileData, strExt);
                if (String.Compare(strExt, "tgz", true) == 0) return IsTGZ(btFileData, strExt);
                if (String.Compare(strExt, "zoo", true) == 0) return IsZOO(btFileData, strExt);
                if (String.Compare(strExt, "dwg", true) == 0) return IsDWG(btFileData, strExt);  // CAD 파일
                if (String.Compare(strExt, "obj", true) == 0) return IsOBJ(btFileData, strExt);  // OBJ 파일
                if (String.Compare(strExt, "hlp", true) == 0) return IsHLP(btFileData, strExt);  // HLP 파일
                if (String.Compare(strExt, "lnk", true) == 0) return IsLNK(btFileData, strExt);  // LNK 파일
                if (String.Compare(strExt, "der", true) == 0) return IsDER(btFileData, strExt);  // DER 파일
                if (String.Compare(strExt, "mp3", true) == 0) return IsMP3(btFileData, strExt);  // MP3 파일
                if (String.Compare(strExt, "mgb", true) == 0) return IsMGB(btFileData, strExt);  // 마이다스 파일(CAD관련)
                if (String.Compare(strExt, "hpt", true) == 0) return IsHPT(btFileData, strExt);  // 슬라이드쇼 관련

                /* Matroska media containter, including WebM */
                /* mkv, mka, mks, mk3d, webm */
                if (String.Compare(strExt, "mkv", true) == 0) return IsMKV(btFileData, strExt);
                if (String.Compare(strExt, "eps", true) == 0) return IsEPS(btFileData, strExt);  // Adobe PostScript
                if (String.Compare(strExt, "stl", true) == 0) return IsSTL(btFileData, strExt);  // CAD 관련 STL 파일
                if (String.Compare(strExt, "chm", true) == 0) return IsCHM(btFileData, strExt);
                if (String.Compare(strExt, "mif", true) == 0) return IsMIF(btFileData, strExt);
                if (String.Compare(strExt, "cvd", true) == 0) return IsCVD(btFileData, strExt);
                if (String.Compare(strExt, "sas7bdat", true) == 0) return IsSAS7BDAT(btFileData, strExt);
                if (String.Compare(strExt, "alz", true) == 0) return IsALZ(btFileData, strExt);  // 알집
                if (String.Compare(strExt, "pst", true) == 0) return IsPST(btFileData, strExt);
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
            btHLP_Header = new byte[] { 0x53, 0x43, 0x44, 0x53, 0x41, 0x30, 0x30 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - softcamp !");
                return true;
            }

            /* <DOCUMENT SAFER */
            btHLP_Header = new byte[] { 0x3C, 0x44, 0x4F, 0x43, 0x55, 0x4D, 0x45, 0x4E, 0x54, 0x20, 0x53, 0x41, 0x46, 0x45, 0x52, 0x20 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - MarkAny(1) !");
                return true;
            }

            btHLP_Header = new byte[] { 0x3C, 0x44, 0x4F, 0x43, 0x55, 0x4D, 0x45, 0x4E, 0x54, 0x53, 0x41, 0x46, 0x45, 0x52, 0x5F };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - MarkAny(2) !");
                return true;
            }

            /* <!-- FasooSecureContainer */
            btHLP_Header = new byte[] { 0x3C, 0x21, 0x2D, 0x2D, 0x20, 0x46, 0x61, 0x73, 0x6F, 0x6F, 0x53, 0x65, 0x63, 0x75, 0x72, 0x65,
                0x43, 0x6F, 0x6E, 0x74, 0x61, 0x69, 0x6E, 0x65, 0x72, 0x20 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - fasoo(1) !");
                return true;
            }

            /* ?DRMONE  This Document is encrypted and protected by Fasoo DRM */
            btHLP_Header = new byte[] { 0x9B, 0x20, 0x44, 0x52, 0x4D, 0x4F, 0x4E, 0x45, 0x20, 0x20, 0x54, 0x68, 0x69, 0x73, 0x20, 0x44, 0x6F,
                0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x69, 0x73, 0x20, 0x65, 0x6E, 0x63, 0x72, 0x79, 0x70,
                0x74, 0x65, 0x64, 0x20, 0x61, 0x6E, 0x64, 0x20, 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65,
                0x64, 0x20, 0x62, 0x79, 0x20, 0x46, 0x61, 0x73, 0x6F, 0x6F, 0x20, 0x44, 0x52, 0x4D, 0x20 };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - fasoo(2) !");
                return true;
            }

            btHLP_Header = new byte[] { 0x9B, 0x20, 0x44, 0x52, 0x4D, 0x4F, 0x4E, 0x45, 0x20, 0x54, 0x68, 0x69, 0x73, 0x20, 0x44, 0x6F,
                0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x69, 0x73, 0x20, 0x65, 0x6E, 0x63, 0x72, 0x79, 0x70,
                0x74, 0x65, 0x64, 0x20, 0x61, 0x6E, 0x64, 0x20, 0x70, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x65,
                0x64, 0x20, 0x62, 0x79, 0x20, 0x46, 0x61, 0x73, 0x6F, 0x6F, 0x20, 0x44, 0x52, 0x4D };
            if (ByteArrayCompare(btFileData, btHLP_Header) == true)
            {
                Log.Information("[IsDRM] - fasoo(3) !");
                return true;
            }

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

        /// <summary>
        /// 파일확장자 위변조 검사 수행
        /// <br> </br>stFile : 위변조 검사 대상 파일의 MemoryStream or FileStream 
        /// <br> </br>strExt : 위변조 검사 대상 파일의 확장자 
        /// </summary>
        /// <param name="stFile"></param>
        /// <param name="strExt"></param>
        /// <param name="blAllowDRM"></param>
        /// <returns></returns>
        public async Task<eFileAddErr> IsValidFileExt(Stream stFile, string strExt, bool blAllowDRM = true)
        {
            byte[] btFileData = await StreamToByteArrayAsync(stFile, MaxBufferSize);

            /* Check DRM File */
            if (IsDRM(btFileData) == true)
            {
                if (blAllowDRM == true) return eFileAddErr.eFANone;
                else return eFileAddErr.eFAUNKNOWN;
            }

            string strFileMime = MimeGuesser.GuessMimeType(btFileData);
            Log.Information("[IsValidFileExt] FileMime[{0}] Ext[{1}] AllowDrmF[{2}]", strFileMime, strExt, blAllowDRM);

            // 0kb			
            if (bEmptyFIleNoCheck && String.Compare(strFileMime, "application/x-empty") == 0) return eFileAddErr.eFANone;

            if (String.Compare(strFileMime, "text/plain") == 0) return eFileAddErr.eFANone;

            if (String.IsNullOrEmpty(strExt) == true)
            {
                if (String.Compare(strFileMime, "application/x-executable") == 0) return eFileAddErr.eFANone;
                return eFileAddErr.eFAUNKNOWN;
            }

            if (IsValidMimeAndExtension(strFileMime, strExt) == true) return eFileAddErr.eFANone;

            strExt = strExt.Replace(".", "");
            btFileData = await StreamToByteArrayAsync(stFile, MaxBufferSize2);
            if (CheckExtForFileByteData(btFileData, strExt) == true)
                return eFileAddErr.eFANone;

            return eFileAddErr.eFACHG;
        }

        public eFileAddErr IsValidFileExtInnerZip(string strFile, string strExt, bool blAllowDRM)
        {
            var fsStream = new FileStream(strFile, FileMode.Open, FileAccess.Read);
            byte[] btFileData = StreamToByteArray(fsStream, MaxBufferSize);
            fsStream.Close();

            if (IsDRM(btFileData) == true)
            {
                if (blAllowDRM == true) return eFileAddErr.eFANone;
                else return eFileAddErr.eUnZipInnerDRM;
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

        /// <summary>
        /// 문서 내 OLE 개체의 파일확장자 위변조 검사 수행 
        /// <br> </br>stFile : 위변조 검사 대상 파일의 MemoryStream or FileStream 
        /// <br> </br>strExt : 위변조 검사 대상 파일의 확장자 
        /// </summary>
        /// <param name="stFile"></param>
        /// <param name="strExt"></param>
        /// <param name="blAllowDRM"></param>
        /// <returns></returns>
        public bool IsValidFileExtOfOLEObject(Stream stFile, string strExt, bool blAllowDRM = true)
        {
            byte[] btFileData = StreamToByteArray(stFile, MaxBufferSize);

            /* Check DRM File */
            if (IsDRM(btFileData) == true)
            {
                Log.Information($"[IsValidFileExtOnOLEObject] IsDRM - Ext[{strExt}] AllowDrm[{blAllowDRM}]");
                return (blAllowDRM == true);
            }

            string strFileMime = MimeGuesser.GuessMimeType(btFileData);
            Log.Information($"[IsValidFileExtOnOLEObject] FileMime[{strFileMime}] Ext[{strExt}] AllowDrm[{blAllowDRM}]");

            // 0kb			
            if (bEmptyFIleNoCheck && String.Compare(strFileMime, "application/x-empty") == 0)
                return true;

            if (String.Compare(strFileMime, "text/plain") == 0)
                return true;

            if (String.IsNullOrEmpty(strExt) == true)
            {
                Log.Information($"[IsValidFileExtOnOLEObject] Extension is Empty - FileMime[{strFileMime}] Ext[{strExt}] AllowDrm[{blAllowDRM}]");
                return (String.Compare(strFileMime, "application/x-executable") == 0);
            }

            strExt = strExt.Replace(".", "");
            btFileData = StreamToByteArray(stFile, MaxBufferSize2);
            return (CheckExtForFileByteData(btFileData, strExt) == true);
        }

        /// <summary>
        /// MimeType 및 확장자 정보 DB인 magic.mgc을 다른 파일로 갱신시 사용
        /// <br></br> stFilePath : magic.mgc 파일 경로
        /// </summary>
        /// <param name="strFilePath"></param>
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
            ["application/CDFV2"] = "db",
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
            ["application/gzip"] = "gz tgz prproj",
            ["application/haansofthwp"] = "frm hwp hwt",
            ["application/hyperstudio"] = "stk",
            ["application/inkml+xml"] = "ink inkml",
            ["application/ipfix"] = "ipfix",
            ["application/java-archive"] = "jar",
            ["application/java-serialized-object"] = "ser",
            ["application/java-vm"] = "class",
            ["application/javascript"] = "js",
            ["application/json"] = "json dat conf txt",
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
            ["application/octet-stream"] = "bin lha lzh exe class so dll img iso log dmp js ini vrv t2s ofp hdr obj t2st clr obj stl tef trc",
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
            ["application/vnd.ms-cab-compressed"] = "cab msu ahc",
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
            ["application/vnd.ms-outlook"] = "msg",
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
            ["application/x-7z-compressed"] = "7z 001",
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
            ["application/x-dosexec"] = "exe aex",
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
            ["application/x-font-ttf"] = "ttf dat",
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
            ["application/x-wine-extension-ini"] = "ini conf fb sif txt asm dof pbi iss if2 inf dat xsh url lng lst ipc ipf rul prj ecf cfg idl h in inc sql asm xcf mof log conf",
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
            ["application/x-dbt"] = "t2p",
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
            ["application/zip"] = "war zip hwpx hwpt drp zipx",
            ["application/zlib"] = "dll inf ppkg xrm-ms",
            ["audio/adpcm"] = "adp",
            ["audio/amr"] = "amr",
            ["audio/flac"] = "flac",
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
            ["audio/x-m4a"] = "m4a mp4",
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
            ["font/sfnt"] = "ttf",
            ["font/ttf"] = "ttf ttc",
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
            ["image/vnd.microsoft.icon"] = "ico ibd",
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
            ["text/html"] = "html c cpp h xml dat js java jsp txt css vb out xls doc cell r cpg prj eml sas xaml vcxproj Master ascx aspx cs hhc hhk rc",
            ["text/n3"] = "n3",
            ["text/plain"] = "conf",
            ["text/plain"] = "txt text ini pcdf csv def in list log mtl",
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
            ["text/vcard"] = "vcard vcf txt",
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
            ["text/x-Algol68"] = "rst ini",
            ["text/x-asm"] = "asm s c cpp h css js sql txt xml dat cell dxf r cpg prj sas",
            ["text/x-awk"] = "awk cell r cpg prj sas",
            ["text/x-bcpl"] = "cell r cpg prj sas",
            ["text/x-c"] = "c cc cpp cxx dic h hh html htm doc dat js java log txt cell dxf r cpg prj sas rc cs fx hhp hpj idl odl rc2 rc3 rcm vb eml",
            ["text/x-c++"] = "c cpp h html htm dat js java log txt cell dxf r cpg prj sas cs eml",
            ["text/x-csv"] = "csv",
            ["text/x-diff"] = "diff log txt cell r cpg prj sas",
            ["text/x-fortran"] = "f f77 f90 for js key txt html cell r cpg prj sas pst",
            ["text/x-gawk"] = "awk cell r cpg prj sas",
            ["text/x-info"] = "info cell r cpg prj sas",
            ["text/x-java"] = "java cell r cpg prj sas",
            ["text/x-java-source"] = "java",
            ["text/x-lisp"] = "lisp cell r cpg prj",
            ["text/x-lua"] = "lua cell r cpg prj",
            ["text/x-m4"] = "m4 cell r cpg prj",
            ["text/x-makefile"] = "makefile cell r cpg prj",
            ["text/xml"] = "xml xsl dat doc html man hwp config dwl dwl2 kml resx datasource csproj cd spdata vcxproj AddIn bdcm datasvcmap dbml diagram disco edmx feature filters layout map mfcribbon-ms myapp package settings svcinfo svcmap sync user vbproj vsixmanifest webpart wsdl xsc xsd xss xsx vcproj manifest xslt jmx rules bpr hxc hxt hxk xrm-ms man config nvi forms strings forms cfg dalp opax opal vbox vbox-prev propdesc managed_manifest uicfg con if psd whc rsh dtsx ps1xml vsixmanifest ue-theme prq mf snippet mum",
            ["text/x-msdos-batch"] = "bat cell r cpg prj",
            ["text/x-ms-regedit"] = "reg",
            ["text/x-nawk"] = "awk cell r cpg prj",
            ["text/x-nfo"] = "nfo",
            ["text/x-objective-c"] = "c h cpp",
            ["text/x-opml"] = "opml",
            ["text/x-pascal"] = "p pas txt c cpp h js java log lib cell r cpg prj rul",
            ["text/x-perl"] = "perl pl cell r cpg prj",
            ["text/x-php"] = "php cell r cpg prj sas",
            ["text/x-po"] = "po pot html htm",
            ["text/x-pod"] = "pod cell r cpg prj sas",
            ["text/x-python"] = "py html cell r cpg prj sas",
            ["text/x-ruby"] = "rudy cell r cpg prj",
            ["text/x-setext"] = "etx",
            ["text/x-sfv"] = "sfv",
            ["text/x-shell"] = "sh",
            ["text/x-shellscript"] = "sh cell r cpg prj sas",
            ["text/x-tcl"] = "tcl cell r cpg prj",
            ["text/x-tex"] = "tex ltx sty cls log txt xml texi html cell r cpg prj sas",
            ["text/x-texinfo"] = "texi cell r cpg prj sas",
            ["text/x-uuencode"] = "uu",
            ["text/x-vcalendar"] = "vcs",
            ["text/x-vcard"] = "vcf cell r cpg prj",
            ["text/x-xmcd"] = "xmcd cell r cpg prj",
            ["video/3gpp"] = "3gp m4a mp4",
            ["video/3gpp2"] = "3g2",
            ["video/h261"] = "h261",
            ["video/h263"] = "h263",
            ["video/h264"] = "h264",
            ["video/jpeg"] = "jpgv",
            ["video/jpm"] = "jpgm jpm",
            ["video/mj2"] = "mj2 mjp2",
            ["video/mp2p"] = "mp2 mpg",
            ["video/mp2t"] = "ts",
            ["video/mp4"] = "mp4 mp4v mpg4",
            ["video/mp4v-es"] = "mp4v",
            ["video/mpeg"] = "m1v m2v mpeg mpg mpe mpg",
            ["video/mpeg4-generic"] = "mpeg mpg mpe",
            ["video/mpv"] = "mpv",
            ["video/ogg"] = "ogv",
            ["video/quicktime"] = "qt mov mp4",
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
            ["video/x-ms-asf"] = "asf asx wmv wma",
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

        /// <summary>
        /// OLE개체의 마임타입 리스트
        /// </summary>
        private static Lazy<Dictionary<string, string>> gOLEMimeTypeMap = new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

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
                string[] exts = result.Split(' ');
                foreach (var ext in exts)
                {
                    if (string.Compare(fileExt, ext) == 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 파일확장자 및 MimeType 정보 등록 및 갱신
        /// <br></br>strMime: 확장자의 Mime 정보 
        /// <br></br>strExt : 확장자 
        /// </summary>
        /// <param name="strMime"></param>
        /// <param name="strExt"></param>
        public void MimeTypeMapAddOrUpdate(string strMime, string strExt)
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

        /// <summary>
        /// OLE 개체의 파일확장자 및 MimeType 정보 등록 및 갱신
        /// </summary>
        /// <param name="strMime"></param>
        /// <param name="strExt"></param>
        public void OLEMimeTypeMapAddOrUpdate(string strMime, string strExt)
        {
            if (gOLEMimeTypeMap.Value.TryGetValue(strMime, out string result))
            {
                gOLEMimeTypeMap.Value[strMime] += " " + strExt;
            }
            else
            {
                gOLEMimeTypeMap.Value[strMime] = strExt;
            }
        }

        //public void AddDataForInnerZip(int nErrCount, string strOrgZipFile, string strOrgZipFileRelativePath, string strErrFileName, eFileAddErr enErr, string strParentFileName)
        //{
        //    if (nErrCount == 1) AddData(strOrgZipFile, eFileAddErr.eFAZIP, strOrgZipFileRelativePath);
        //    AddData(strErrFileName, enErr, strOrgZipFile, true, strParentFileName);
        //}

        /// <summary>
        /// ZIP 파일내부 검사(현재 : ZIP 파일을 temp쪽에 복사해서 분석함) <br/>
        /// hsStream : zip파일FileStream <br/>
        /// bDenyPasswordZIP : zip파일에 password 있으면 차단할지 유무(true:차단) <br/>
        /// blWhite : FileFilter Type(true:White방식) <br/>
        /// strExtInfo : FileFilter 정보 <br/>
        /// SGFileExamEvent : 압축해제 및 분석 진행상황 UI쪽에 전달하는데 사용되는 함수 <br/>
        /// ExamCount : 사용자가 추가해서 내부검사해야되는 ZIP 파일 Index <br/>
        /// TotalCount : 사용자가 추가해서 내부검사해야되는 ZIP 파일개수 <br/> 
        /// nMaxDepth : CLIENT_ZIP_DEPTH의 1번값(3: ZIP 파일 내부에 ZIP이 발견되면 3depth 까지 해제함) <br/>
        /// nOption : CLIENT_ZIP_DEPTH의 2번값(0: 1번째 zip depth에 또 zip이 발견되면 차단, 1 : 허용) <br/>
        /// blAllowDRM : drm 파일 허용유무(true:허용)
        /// </summary>
        /// <param name="hsStream">zip파일FileStream</param>
        /// <param name="bDenyPasswordZIP">zip파일에 password 있으면 차단할지 유무(true:차단)</param>
        /// <param name="blWhite">FileFilter Type(true:White방식)</param>
        /// <param name="strExtInfo">FileFilter 정보</param>
        /// <param name="SGFileExamEvent">압축해제 및 분석 진행상황 UI쪽에 전달하는데 사용되는 함수</param>
        /// <param name="ExamCount">사용자가 추가해서 내부검사해야되는 ZIP 파일 Index</param>
        /// <param name="TotalCount">사용자가 추가해서 내부검사해야되는 ZIP 파일개수 </param>
        /// <param name="nMaxDepth"> CLIENT_ZIP_DEPTH의 1번값(3: ZIP 파일 내부에 ZIP이 발견되면 3depth 까지 해제함) </param>
        /// <param name="nOption">CLIENT_ZIP_DEPTH의 2번값(0: 1번째 zip depth에 또 zip이 발견되면 차단, 1 : 허용)</param>
        /// <param name="blAllowDRM"> drm 파일 허용유무(true:허용)</param>
        /// <returns></returns>
        public async Task<int> CheckZipFile(HsStream hsStream, FileAddErr currentFile, bool bDenyPasswordZIP, bool blWhite, string strExtInfo, FileExamEvent SGFileExamEvent, int ExamCount, int TotalCount, int nMaxDepth = 3, int nOption = 0, bool blAllowDRM = true)
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

            ////빈파일 체크 (0kb 허용)
            //if(hsStream.Size <= 0 && bEmptyFIleNoCheck && )

            // Setting Default Value
            stStream = hsStream.stream;

            strTempZipPath = "Temp";
            strExtractTempZipPath = Path.Combine(strTempZipPath, "ZipExtract");
            strZipFile = Path.Combine(strTempZipPath, Path.GetFileName(hsStream.FileName));
            strOrgZipFile = hsStream.FileName;
            strOrgZipFileRelativePath = hsStream.RelativePath;

            // Create Temp Directory 
            DirectoryInfo dirZipBase = new DirectoryInfo(strTempZipPath);
            if (!dirZipBase.Exists)
                dirZipBase.Create();

            Log.Information("[CheckZipFile] ZipFile[{0}] Ext[WhiteF({1})-Info({2})] ZipCheck[MaxDepth({3})-BlockOption({4})] AllowDrmF[{5}]",
                 Path.GetFileName(hsStream.FileName), blWhite, strExtInfo, nMaxDepth, nOption, blAllowDRM);

            // Zip File Temp쪽에 Copy 및 Scan 
            using (var fileStream = new FileStream(strZipFile, FileMode.Create, FileAccess.Write))
            {
                await stStream.CopyToAsync(fileStream);
                fileStream.Close();

                enRet = ScanZipFile(currentFile, strOrgZipFile, strOrgZipFileRelativePath, strZipFile, strExtractTempZipPath, nMaxDepth, nOption, 1, blWhite, strExtInfo, 0,
                    out nTotalErrCount, out strOverMaxDepthInnerZipFile, blAllowDRM, SGFileExamEvent, ExamCount, TotalCount, bDenyPasswordZIP);

                // KKW
                /*if (enRet == eFileAddErr.eFANone && nOption == 0 && nTotalErrCount == 0 && String.IsNullOrEmpty(strOverMaxDepthInnerZipFile) == false)
                {
                    enRet = eFileAddErr.eUnZipInnerLeftZip;
                    AddDataForInnerZip(nTotalErrCount, strOrgZipFile, strOrgZipFileRelativePath, strOverMaxDepthInnerZipFile, enRet);
                }*/

                //if (enRet == eFileAddErr.eFAZipPW) AddData(strOrgZipFile, enRet, strOrgZipFileRelativePath);

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

        /// <summary>
        /// ZIP 파일이 경우, 별도로 압축 해제하여 내부 확인
        /// </summary>
        /// <param name="currentFile">현재 Zip 파일</param>
        /// <param name="strOrgZipFile"></param>
        /// <param name="strOrgZipFileRelativePath"></param>
        /// <param name="strZipFile"></param>
        /// <param name="strBasePath"></param>
        /// <param name="nMaxDepth"></param>
        /// <param name="nBlockOption"></param>
        /// <param name="nCurDepth"></param>
        /// <param name="blWhite"></param>
        /// <param name="strExtInfo"></param>
        /// <param name="nErrCount"></param>
        /// <param name="nTotalErrCount"></param>
        /// <param name="strOverMaxDepthInnerZipFile"></param>
        /// <param name="blAllowDRM"></param>
        /// <param name="SGFileExamEvent"></param>
        /// <param name="ExamCount"></param>
        /// <param name="TotalCount"></param>
        /// <param name="bZipPasswdCheck"></param>
        /// <returns></returns>
        public eFileAddErr ScanZipFile(FileAddErr currentFile, string strOrgZipFile, string strOrgZipFileRelativePath, string strZipFile, string strBasePath, int nMaxDepth, int nBlockOption, int nCurDepth,
            bool blWhite, string strExtInfo, int nErrCount, out int nTotalErrCount, out string strOverMaxDepthInnerZipFile, bool blAllowDRM, FileExamEvent SGFileExamEvent, int ExamCount, int TotalCount, bool bZipPasswdCheck = true)
        {
            eFileAddErr enErr;
            string strExt;
            int nCurErrCount;
            string strOverMaxDepthZipFile = "";

            var opts = new SharpCompress.Readers.ReaderOptions();
            var encoding = Encoding.Default;

            byte[] buff = null;
            using (FileStream fsSource = new FileStream(strZipFile, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fsSource);
                long numBytes = 8;
                buff = br.ReadBytes((int)numBytes);

                //Zip File Foramt : Local File Header 구조 바이트 차트
                //Signature 4byte / Version 2Byte / Flags 2Byte / => Flags Bit가 서로 다름 Mac의 경우 8 그 이외는 0
                encoding = (buff[6] == 0x08) ? Encoding.Default : Encoding.GetEncoding(949);
            }

            opts.ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding();
            opts.ArchiveEncoding.CustomDecoder = (data, x, y) =>
            {
                return encoding.GetString(data);
            };

            //2022.10.06 BY kYH sharpPress 버전업하며, WriteToDirectory 호출 시 존재하지 않는 폴더는 오류가 발생하여 추가
            DirectoryInfo dirZipInner = new DirectoryInfo(strBasePath);
            if (dirZipInner.Exists != true) dirZipInner.Create();

            enErr = eFileAddErr.eFANone;
            nCurErrCount = nErrCount;
            try
            {
                using (var archive = ArchiveFactory.Open(strZipFile, opts))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        FileAddErr childFile = currentFile.CreateChildren(entry.Key, strOrgZipFileRelativePath, currentFile.FileName);            //zip파일의 자식 File 생성

                        Log.Information("[ScanZipFile] Check File[{0}] in {1}", entry.Key, Path.GetFileName(strZipFile));
                        int per = (ExamCount * 100) / TotalCount;
                        if (per < 20)
                            per = 20;

                        if (SGFileExamEvent != null)
                            SGFileExamEvent(per, entry.Key);

                        // Check Password	
                        if (entry.IsEncrypted == true && bZipPasswdCheck == true)
                        {
                            if (nCurDepth != 1)
                            {
                                childFile.eErrType = eFileAddErr.eUnZipInnerZipPassword;
                                enErr = eFileAddErr.eUnZipInnerZipPassword;

                                //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(strZipFile), enErr, Path.GetFileName(strZipFile));
                            }
                            else
                            {
                                currentFile.eErrType = eFileAddErr.eFAZipPW;        //하위 파일이 아닌 검사 대상 자체 ZIP에 암호화된 경우
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
                            //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, entry.Key, enErr, Path.GetFileName(strZipFile));
                            childFile.eErrType = eFileAddErr.eUnZipInnerLengthOver;
                            continue;
                        }

                        // Check Directory 
                        if (entry.IsDirectory == true) continue;

                        // Check Empty File 
                        // 0kb 파일 허용(기본)
                        if (bEmptyFIleNoCheck == false && entry.Size <= 0)
                        {
                            enErr = eFileAddErr.eUnZipInnerFileEmpty;
                            childFile.eErrType = eFileAddErr.eUnZipInnerFileEmpty;
                            //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr, Path.GetFileName(strZipFile));
                            continue;
                        }

                        // Check Block File Extension
                        strExt = Path.GetExtension(entry.Key);
                        if (GetRegExtEnable(blWhite, strExtInfo, strExt.Replace(".", "")) != true)
                        {
                            enErr = eFileAddErr.eUnZipInnerExt;
                            childFile.eErrType = eFileAddErr.eUnZipInnerExt;
                            //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr, Path.GetFileName(strZipFile));
                            continue;
                        }

                        // Extract File in Zip 
                        entry.WriteToDirectory(strBasePath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true,
                        });

                        // Check Changed File Extension 
                        enErr = IsValidFileExtInnerZip(Path.Combine(strBasePath, entry.Key), strExt.Replace(".", ""), blAllowDRM);
                        if (enErr != eFileAddErr.eFANone)
                        {
                            childFile.eErrType = enErr;
                            //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr, Path.GetFileName(strZipFile));
                            continue;
                        }

                        // Check Zip File  (압축파일 내 압축파일이 또 존재하는 경우.
                        if ((String.Compare(strExt, ".zip", true) != 0) && (String.Compare(strExt, ".7z", true) != 0)) continue;

                        if (nCurDepth >= nMaxDepth)
                        {
                            Log.Information($"[ScanZipFile] Skip to check zip file[{Path.GetFileName(strZipFile)}]. MaxDepth[{nMaxDepth}] CurDepth[{nCurDepth}] BlockOption[{nBlockOption}] Remain Zip File[{strZipFile}] in {strOrgZipFile}");
                            strOverMaxDepthZipFile = entry.Key;

                            //2022.10.07 BY KYH - CLIENT_ZIP_DEPTH 의 Block 옵션 활용
                            if (nBlockOption <= 0)
                            {
                                // kkw 추가
                                enErr = eFileAddErr.eUnZipInnerLeftZip;
                                childFile.eErrType = eFileAddErr.eUnZipInnerLeftZip;
                                //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(entry.Key), enErr, Path.GetFileName(strZipFile));
                            }
                            continue;
                        }

                        // Scan Zip File in Zip
                        int nInnerErrCount = 0;
                        string strCurZip = Path.Combine(strBasePath, entry.Key);
                        string strExtractPath = Path.Combine(strBasePath, Path.GetFileNameWithoutExtension(entry.Key));
                        eFileAddErr enRet = ScanZipFile(childFile, strOrgZipFile, strOrgZipFileRelativePath, strCurZip, strExtractPath, nMaxDepth, nBlockOption, nCurDepth + 1,
                            blWhite, strExtInfo, nCurErrCount, out nInnerErrCount, out strOverMaxDepthZipFile, blAllowDRM, SGFileExamEvent, ExamCount, TotalCount);
                        if (enRet != eFileAddErr.eFANone) enErr = enRet;
                        nCurErrCount += nInnerErrCount;
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (bZipPasswdCheck == true)
                {
                    // Check Passowrd in 7zip(7z)
                    if (nCurDepth != 1)
                    {
                        enErr = eFileAddErr.eUnZipInnerZipPassword;
                        currentFile.eErrType = eFileAddErr.eUnZipInnerZipPassword;
                        //AddDataForInnerZip(++nCurErrCount, strOrgZipFile, strOrgZipFileRelativePath, Path.GetFileName(strZipFile), enErr, Path.GetFileName(strZipFile));
                    }
                    else
                    {
                        enErr = eFileAddErr.eFAZipPW;
                        currentFile.eErrType = eFileAddErr.eFAZipPW;
                        nCurErrCount++;
                    }
                    nTotalErrCount = nCurErrCount;
                    strOverMaxDepthInnerZipFile = strOverMaxDepthZipFile;
                    return enErr;
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

        public async Task<int> CheckDocumentFile(HsStream hsStream, FileAddErr currentFile, int documentExtractType, bool isOLEMimeTypeWhite, bool isWhite, string fileFilterExtInfo, bool isDocumentWhite, string documentFileFilterExtInfo)
        {
            string strDocumentExtractRootPath = Path.Combine("Temp", "Document_Extract");

            try
            {
                Log.Information($"[CheckDocumentFile] DocumentFile[{Path.GetFileName(hsStream.FileName)}] Ext[White({isWhite})-Info({fileFilterExtInfo})] DocExt[DocumentWhite({isDocumentWhite})-DocInfo({documentFileFilterExtInfo})]");

                //bool usecheckOLE_Mime = true;       //기본 OLE 기능 - OLE 검출 함수 호출 여부
                //bool usecheckOLE_Extension = true;  //OLE 검사 옵션1 - OLE개체 마임리스트 검사 여부
                int scanDepth = 1;               //OLE개체 검사 하위 범위 (0인 상태에서도 개체 검출 시 Block)

                await scanDocumentFile(hsStream, currentFile, strDocumentExtractRootPath, isOLEMimeTypeWhite, isWhite, fileFilterExtInfo, isDocumentWhite, documentFileFilterExtInfo, scanDepth, documentExtractType);
                return (currentFile.HasChildrenErr) ? -1 : 0;
            }
            catch (Exception ex)
            {
                Log.Error($"[CheckDocumentFile] Exception = [{ex.ToString()}]");
                return -1;
            }
            finally
            {
                try
                { Directory.Delete(strDocumentExtractRootPath, true); }
                catch (System.Exception err)
                { Log.Warning("[CheckDocumentFile] Fail Directory.Delete() " + err.Message + " " + err.GetType().FullName); }
            }
        }

        /// <summary>
        /// 문서 추출 방식으로 문서 검사
        /// </summary>
        /// <param name="hsStream">검사 대상 문서</param>
        /// <param name="currentFile">검사 대상 문서의 FileAddErr 객체</param>
        /// <param name="currentRootPath">문서를 추출할 경로</param>
        /// <param name="isOLEMimeTypeWhite">문서용 마임타입 FileFilterType (OLECHECKMIMETYPE) - 2105 Command 응답건과 함께 사용 </param>
        /// <param name="isWhite">기존 FileFilterType</param>
        /// <param name="fileFilterExtInfo">기존 FileFilter 목록</param>
        /// <param name="isDocumentWhite">문서용 FileFilterType</param>
        /// <param name="documentFileFilterExtInfo">문서용 FileFilter 목록</param>
        /// <param name="scanDepth">엑셀문서에 한하여, 하위 검사 횟수 (default =1)</param>
        /// <param name="documentExtractType">서버에서 받아온 추출파일 검사 설정값 (1:OLE개체형식 검사 / 2:압축형식 검사 / 3: 모두 검사)</param>
        /// <returns></returns>
        async Task<int> scanDocumentFile(HsStream hsStream, FileAddErr currentFile, string currentRootPath, bool isOLEMimeTypeWhite, bool isWhite, string fileFilterExtInfo, bool isDocumentWhite, string documentFileFilterExtInfo, int scanDepth, int documentExtractType)
        {
            string strExtractFilePath = Path.Combine(currentRootPath, Path.GetFileNameWithoutExtension(hsStream.FileName));                              //Temp에 Copy된 문서의 OLE 개체를 보관할 폴더
            DirectoryInfo extractFileDir = new DirectoryInfo(strExtractFilePath);
            if (!extractFileDir.Exists)
                extractFileDir.Create();

            Stream fileStream = hsStream.stream;
            int extractorResult = 0;


            //Docuement로 OLE 개체 검사 모듈 실행
            using (MemoryStream fileMemoryStream = new MemoryStream())
            {
                if (hsStream.MemoryType == HsStreamType.MemoryStream)
                {
                    byte[] buf = new byte[fileStream.Length];
                    await fileStream.ReadAsync(buf, 0, (int)fileStream.Length);
                    fileMemoryStream.Write(buf);
                }
                else if (hsStream.MemoryType == HsStreamType.FileStream)
                {
                    fileStream.CopyTo(fileMemoryStream);
                }

                extractorResult = OfficeExtractor.Controller.ExcuteExtractor(fileMemoryStream, hsStream.FileName, strExtractFilePath);
                Log.Information($"[scanDocumentFile]  ExcuteExtractor DocumentFile[{Path.GetFileName(hsStream.FileName)}] extractorResult[{extractorResult}]");
            }

            if (extractorResult == 0)        //검출된 OLE 개체 없음 (정상처리)
                return 0;

            if (extractorResult == -10)          //압축형식의 위변조 파일 및 임의 파일 발견
            {
                currentFile.eErrType = eFileAddErr.eFADOC_COMPRESS_CHANGE;
                currentFile.HasChildrenErr = true;
                foreach (FileInfo oleObject in extractFileDir.GetFiles())
                {
                    FileAddErr oleFile = currentFile.CreateChildren(oleObject.Name, oleObject.FullName, currentFile.FileName);
                    oleFile.eErrType = eFileAddErr.eFADOC_COMPRESS_CHANGE;
                }
                return extractorResult;
            }

            if (extractorResult < 0)      //OLE 개체 검출 중 오류 발생
            {
                //오류 표시
                currentFile.eErrType = GetOLEError(extractorResult);
                currentFile.ChildrenFiles = null;
                return extractorResult;
            }

            //검출된 파일 대상 검사
            foreach (FileInfo extractFile in extractFileDir.GetFiles())
            {
                FileAddErr oleFile = currentFile.CreateChildren(extractFile.Name, extractFile.FullName, currentFile.FileName);
                string oleExtension = extractFile.Extension.Substring(1);
                DocumentExtractType documentExtract = (DocumentExtractType)documentExtractType;

                //하위 검사 횟수를 모두 소진하여도 개체가 검출된다면 Block
                if (scanDepth <= 0)
                {
                    oleFile.eErrType = eFileAddErr.eFADOC_OLE_REMAININGFILES;
                    currentFile.HasChildrenErr = true;
                    continue;
                }
                using (Stream oleFileStream = File.OpenRead(extractFile.FullName))
                {
                    byte[] btFileData = StreamToByteArray(oleFileStream, MaxBufferSize);
                    string oleFileMime = MimeGuesser.GuessMimeType(btFileData);
                    // Check Empty File 
                    // 0kb 파일 Block
                    if (bEmptyFIleNoCheck == false && btFileData.Length <= 0)
                    {
                        oleFile.eErrType = eFileAddErr.eFAEMPTY;
                        currentFile.HasChildrenErr = true;
                        continue;
                    }

                    //OLE개체의 마임리스트 체크 
                    isOLEMimeTypeWhite = true;   //서버와 2105 통신 추가 전까지는 White List 로 임시 설정
                    if (IsValidOLEMimeType(oleFileMime, extractFile.Name, isOLEMimeTypeWhite))      
                    {
                        oleFile.eErrType = eFileAddErr.eFANone;
                        continue;
                    }

                    #region [OLE형식 검사]
                    if (documentExtract.HasFlag(DocumentExtractType.OLEOBJECT_EXTRACT))
                    {
                        //기본 확장자 제한 black, white 리스트 체크
                        if (!GetRegExtEnable(isWhite, fileFilterExtInfo, oleExtension))
                        {
                            oleFile.eErrType = eFileAddErr.eFADOC_OLE_EXTENSION;
                            currentFile.HasChildrenErr = true;
                            continue;
                        }

                        //위변조 제한, 0KB 체크
                        if (!IsValidFileExtOfOLEObject(oleFileStream, oleExtension))
                        {
                            oleFile.eErrType = eFileAddErr.eFADOC_OLE_CHANGE;
                            currentFile.HasChildrenErr = true;
                            continue;
                        }
                    }
                    #endregion

                    #region [압축형식 검사]
                    if (documentExtract.HasFlag(DocumentExtractType.COMPRESS_EXTRACT))
                    {
                        //확장자 제한 black, white 리스트 체크
                        //기본 확장자 제한 black, white 리스트 체크
                        if (!GetRegExtEnable(isDocumentWhite, documentFileFilterExtInfo, oleExtension))
                        {
                            oleFile.eErrType = eFileAddErr.eFADOC_OLE_EXTENSION;
                            currentFile.HasChildrenErr = true;
                            continue;
                        }

                        //위변조 제한, 0KB 체크
                        if (!IsValidFileExtOfOLEObject(oleFileStream, oleExtension))
                        {
                            oleFile.eErrType = eFileAddErr.eFADOC_OLE_CHANGE;
                            currentFile.HasChildrenErr = true;
                            continue;
                        }
                    }
                    #endregion
                  
                    //추출 개체가 엑셀인 경우, 차단 전 한번 더 검사 허용
                    if (oleExtension.ToUpper() == "XLSX" || oleExtension.ToUpper() == "XLS")
                    {
                        //추출 개체가 엑셀인 경우, 한번 더 검사 허용
                        HsStream oleHsStream = new HsStream() { stream = oleFileStream, FileName = extractFile.FullName, MemoryType = HsStreamType.FileStream };
                        int extractResult = await scanDocumentFile(oleHsStream, oleFile, strExtractFilePath, isOLEMimeTypeWhite, isWhite, fileFilterExtInfo, isDocumentWhite, documentFileFilterExtInfo, (scanDepth-1), documentExtractType);
                        if (extractResult != 0)
                            currentFile.HasChildrenErr = true;
                    }
                    else
                    {
                        //엑셀 개체가 아닌 파일이 추출되면 차단.
                        oleFile.eErrType = eFileAddErr.eFADOC_OLE_REMAININGFILES;
                        currentFile.HasChildrenErr = true;
                    }
                }
            }
            return (currentFile.HasChildrenErr) ? -1 : 0;
        }

        /// <summary>
        /// OLE개체 마임타입 체크
        /// <para>gOLEMimeTypeMap과 mime 비교</para>
        /// </summary>
        /// <param name="mime">해당파일의 MIME</param>
        /// <param name="fileName">파일명</param>
        /// <param name="isOLEMimeWhite">OLE개체 검사의 MimeList의 W/B 여부(Tag:OLECHECKMIMETYPE)</param>
        /// <returns></returns>
        private bool IsValidOLEMimeType(string mime, string fileName, bool isOLEMimeWhite)
        {
            string fileExt = fileName;
            var ind = fileExt.LastIndexOf('.');
            if (ind != -1 && fileExt.Length > ind + 1)
                fileExt = fileName.Substring(ind + 1).ToLower();

            bool existsInMimeList = false;
            if (gOLEMimeTypeMap.Value.TryGetValue(mime, out string result))
            {
                Log.Debug($"IsValidOLEMimeType, Get Extensions[{result}] for Mime[{mime}] (isOLEMimeWhite[{isOLEMimeWhite}])");
                string[] exts = result.Split(' ');
                foreach (var ext in exts)
                {
                    //OLE 마임리스트에 존재 시 false 처리
                    if (string.Compare(fileExt, ext) == 0)
                    {
                        existsInMimeList = true;
                        break;
                    }
                }
            }
            return (isOLEMimeWhite) ? existsInMimeList : !existsInMimeList;
        }

        public void LoadMimeConf(int groupID)
        {
            string strFileName = String.Format("FileMime.{0}.conf", groupID.ToString());
            strFileName = Path.Combine("wwwroot/conf", strFileName);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strFileName = strFileName.Replace("/", "\\");
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
                for (int i = 1; i < strMimeList.Length; i++)
                {
                    string[] strSplit = strMimeList[i].Split(' ');
                    if (strSplit.Length < 2)
                        continue;
                    MimeTypeMapAddOrUpdate(strSplit[0], strSplit[1]);
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Log.Information("LoadMimeConf Exception Msg = [{0}]", ioEx.Message);
            }
        }

        public void LoadOLEMimeConf(int groupID)
        {
            string strFileName = String.Format("OLEFileMime.{0}.conf", groupID.ToString());
            strFileName = Path.Combine("wwwroot/conf", strFileName);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strFileName = strFileName.Replace("/", "\\");
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
                for (int i = 1; i < strMimeList.Length; i++)
                {
                    string[] strSplit = strMimeList[i].Split(' ');
                    if (strSplit.Length < 2)
                        continue;
                    OLEMimeTypeMapAddOrUpdate(strSplit[0], strSplit[1]);
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Log.Information("LoadOLEMimeConf Exception Msg = [{0}]", ioEx.Message);
            }
        }
    }
}
