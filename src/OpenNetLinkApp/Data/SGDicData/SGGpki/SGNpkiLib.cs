using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using AgLogManager;
using System.Collections.Generic;
using System.Linq;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGDicData.SGNpki
{ 
    
    public class NPKICA
    {
        Dictionary<string, string> DicNpkiCA = new Dictionary<string, string>();
        public void SetData(string tag, string value)
        {
            string strTmp = "";
            if (DicNpkiCA.TryGetValue(tag, out strTmp) == true)
            {
                DicNpkiCA.Remove(tag);
            }
            DicNpkiCA[tag] = value;
        }
        public string GetData(string tag)
        {
            string strTmp = "";
            if (DicNpkiCA.TryGetValue(tag, out strTmp) != true)
            {
                return strTmp;
            }
            return DicNpkiCA[tag];
        }
    }
    public class NPKIFileInfo
    {

        /// <summary>
        /// NPKI 인증서 파일(Der) : FullPath
        /// </summary>
        public string m_strFileName;

        /// <summary>
        /// NPKI 인증서 파일(Der)에 같이 있는 함게보내질 File들 fullPath목록
        /// </summary>
        public List<string> m_listDerFilePath = new List<string>();

        public string m_strUserID;              // NPKI 인증서 ID
        public string m_strExpiredDate;         // NPKI 인증서 만료일자
        public string m_strKeyUse;              // NPKI 인증서 사용 용도( 구분 ) 입력되는 항목
        public string m_strOrg;                 // NPKI 인증서 발급기관.
        //public GPKICA m_gpkiCA;                 // NPKI 인증서 발급기관 상세정보.
        //public string m_strOID;                   // NPKI 인증서 OID
        public int m_nRemainDay;                  // NPKI 인증서 남은 유효 기간(날짜기준, int값)
        public bool m_bCheckDisable { get; set; } = false;  // 인증서 ui에서 전송하도록 체크할 수 없음
        public bool m_bCheck { get; set; } = false;         // 인증서 선택. 체크상태
        public byte[] m_pKeyData;

        public EnumSysPos ePos = EnumSysPos.None;    // NPKI가 어디에 있는 것인지에 대한 정보



        public NPKIFileInfo()
        {
            m_strFileName = m_strUserID = m_strKeyUse = m_strOrg = "";
            m_nRemainDay = 0;
            m_bCheck = false;
            //m_strOID = "";
            //m_gpkiCA = new GPKICA();
            m_pKeyData = null;
        }

        /// <summary>
        /// userID : 사용자<br></br>
        /// expiredDate : 만기일정보<br></br>
        /// KeyUse : OID값
        /// Org : 발급자
        /// nRemainDay : 만기일 남은 날짜
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="expiredDate"></param>
        /// <param name="KeyUse"></param>
        /// <param name="Org"></param>
        /// <param name="nRemainDay"></param>
        public void SetNPKIInfo(string userID, string expiredDate, string KeyUse, string Org, int nRemainDay)
        {
            m_strUserID = userID;               // NPKI 인증서 사용자 계정.(사용자 이름)
            m_strExpiredDate = expiredDate;     // NPKI 인증서 만료일자(만료일)
            m_strKeyUse = KeyUse;               // NPKI 인증서 사용 용도.(UI상에 구분 항목)
            m_strOrg = Org;                     // NPKI 인증서 발급 기관.(발급자)
            m_nRemainDay = nRemainDay;          // NPKI 인증서 사용가능 날짜
        }

        /// <summary>
        /// 인증서형식 Der 파일인지 확인
        /// </summary>
        /// <param name="strSearchPath"></param>
        /// <returns></returns>
        static public bool isDerFile(string strSearchPath)
        {

            bool bRet = false;
            byte[] Buf = null;
            if (File.Exists(strSearchPath))
            {
                //Log.Logger.Here().Information($"isDerFile(doCheck) : {strSearchPath}");

                using (FileStream fs = File.OpenRead(strSearchPath))
                {
                    try
                    {
                        Buf = new byte[FileAddManage.MaxBufferSize];
                        if (fs.Length > 2)
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            if (fs.Read(Buf, 0, Buf.Length) > 2)
                            {
                                bRet = FileAddManage.IsDER(Buf, "");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Here().Error($"isDerFile, Exception - Message(MSG) : {ex.Message}");

                    }
                    finally
                    {
                        fs?.Dispose();
                    }
                }

            }

            //Log.Logger.Here().Information($"isDerFile(#)({bRet}) : {strSearchPath}");

            return bRet;
        }


        public bool GetDerFileList()
        {
            m_listDerFilePath.Clear();
            string strDerFilePath = "";

            int nPos = m_strFileName.LastIndexOf('\\');
            if (nPos > 0)
            {
                strDerFilePath = m_strFileName.Substring(0, nPos);
                Log.Logger.Here().Information($"Search Der Files-Path : {strDerFilePath}");
                if (Directory.Exists(strDerFilePath) == false)
                    return false;
            }

            Log.Logger.Here().Information($"Search Der Files-Path : Search Start!");

            IEnumerable<string> fList = null;
            try
            {
                fList = Directory.EnumerateFileSystemEntries(strDerFilePath)
                .Where(f => (new System.IO.FileInfo(f).Attributes & (FileAttributes.Hidden)) == 0)
                .Where(f => (new System.IO.FileInfo(f).Attributes & (FileAttributes.Directory)) == 0);

                if ( (fList?.Count() ?? 0) > 0 )
                {
                    foreach(string strFoundFilePath in fList)
                    {
                        bool bIsDer = isDerFile(strFoundFilePath);
                        Log.Logger.Here().Information($"Search pki files(#)(DER:{bIsDer}) : {strFoundFilePath}");
                        if (bIsDer)
                            m_listDerFilePath.Add(strFoundFilePath);
                    }
                }
                else
                {
                    Log.Logger.Here().Information($"Search Der Files-Path :Empty!");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Information($"GetDerFileList, Exception(MSG) : {ex.Message}");
            }

            Log.Logger.Here().Information($"Search Der Files-Path : Search End!");

            return (m_listDerFilePath.Count > 0 );
        }


    }

}
