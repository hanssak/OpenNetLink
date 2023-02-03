using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using AgLogManager;
using System.Collections.Generic;
using System.Linq;

namespace OpenNetLinkApp.Data.SGDicData.SGGpki
{ 
    internal class HsGpkiLib
    {
    #if _WINDOWS
        //public const string strGpkiLibName = "E:\\OpenOS\\SRC\\OpenNetLink\\src\\OpenNetLinkApp\\Library\\gpkiapi64.dll";
        public const string strGpkiLibName = "gpkiapi64.dll";
    #elif _LINUX
        public const string strGpkiLibName = "libgpkiapi.so";
    #elif _MACOSX
        public const string strGpkiLibName = "libgpkiapi.so";
    #else
        public const string strGpkiLibName = "libgpkiapi.so";
    #endif
#if _WINDOWS
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GPKI_API_Init(ref IntPtr ppCleintCtx, StringBuilder workDir);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_Finish(ref IntPtr ppCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_GetErrInfo(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbErrInfo);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_GetVersion(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbVersion);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_GetInfo(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbAPIInfo);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_SetOption(IntPtr pCleintCtx, int nOption);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_SetConfFile(IntPtr pCleintCtx, StringBuilder sbConfFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_SetCaPubs(IntPtr pCleintCtx, IntPtr psbBinStrCaPubs);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_FreeCaPubs(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_Str2Time(IntPtr pCleintCtx, StringBuilder sbTime, long time);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_API_SetHashAlgo(IntPtr pCleintCtx, int nHashAlg);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_BINSTR_Create(byte[] pbinStr);  // byte[] pbinStr

        //internal static extern int GPKI_BINSTR_Create(IntPtr pbinStr);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_BINSTR_Delete(ref IntPtr pbinStr);  // byte[] pbinStr

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_BINSTR_SetData(byte[] pData, int nDataLen, byte[] pBinData);  //IntPtr pbinStr

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_Load(IntPtr pCleintCtx, IntPtr pbinStrCert);   // ref IntPtr pbinStrCert - byte[]
        //internal static extern int GPKI_CERT_Load(IntPtr pCleintCtx, IntPtr pbinStrCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_Unload(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //internal static extern int GPKI_CERT_GetUID(IntPtr pCleintCtx,int nAllocLen, StringBuilder sbUID);
        internal static extern int GPKI_CERT_GetUID(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbUID);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetVersion(IntPtr pCleintCtx, IntPtr pnVerSion);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSerialNum(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSerialNum);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSignatureAlgorithm(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSignAlg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetIssuerName(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbIssuerName);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetValidity(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbValidity);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSubjectName(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSubjectName);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetPubKeyAlg(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbAlg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetPubKeyLen(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbLen);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetPubKey(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbPubKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSignature(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSignature);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetKeyUsage(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbKeyUsage);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetBasicConstraints(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbBasicConstraints);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetCertPolicy(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbCertPolicy);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetCertPolicyID(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbCertPolicyID);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetExtKeyUsage(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbExtKeyUsage);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSubjectAltName(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSubAltName);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetAuthKeyID(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbAKI);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetSubKeyID(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbSKI);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetCRLDP(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbCRLDP);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetAIA(IntPtr pCleintCtx, int nAllocLen, StringBuilder sbAIA);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetRemainDays(IntPtr pCleintCtx, IntPtr pbinstrCert, ref int pnRemainDays);    // byte[]
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_AddCert(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrCerts);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetCertCount(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pnCount);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_GetCert(IntPtr pCleintCtx, IntPtr pbinstrCerts, int nIndex, IntPtr pbinstrCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_VerifyByIVS(IntPtr pCleintCtx, StringBuilder sbConfFilePath, IntPtr pbinstrCert, IntPtr pbinstrMyCert );

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_AddTrustedCert(IntPtr pCleintCtx, IntPtr pbinstrTrustedCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_SetVerifyEnv(IntPtr pCleintCtx, int nRange, int nCertCheck, bool bUseCache, StringBuilder sbTime, StringBuilder sbOCSPURL);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_Verify(IntPtr pCleintCtx, IntPtr pbinstrCert, int nCertType, StringBuilder sbCertPolicies, StringBuilder sbConfFilePath, bool bSign, IntPtr pbinstrMyCert, IntPtr pbinstrMyPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_CheckStatByOCSP(IntPtr pCleintCtx, IntPtr pbinstrCert, bool bSign, IntPtr pbinstrMyCert, IntPtr pbinstrMyPriKey,StringBuilder sbURL, IntPtr pbinstrOCSPSvrCert, Byte[] RevocationDate, IntPtr pnRevReason);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CERT_CheckStatByCRL(IntPtr pCleintCtx, IntPtr pbinstrCert, Byte[] RevocationDate,IntPtr pnRevReason);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_OCSP_MakeOCSPReq(IntPtr pCleintCtx, IntPtr pbinstrCerts, bool bSign, IntPtr pbinstrMyCert, IntPtr pbinstrMyPriKey, IntPtr pbinstrReqMsg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_OCSP_SendAndRecv(IntPtr pCleintCtx, StringBuilder sbURL, StringBuilder sbCert,StringBuilder sbReqMsg, StringBuilder sbResMsg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_OCSP_VerifyResMsg(IntPtr pCleintCtx, IntPtr pbinstrReqMsg, IntPtr pbinstrResMsg, IntPtr pnCertStatCnt, IntPtr pnOCSPSvrCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_OCSP_GetCertStatus(IntPtr pCleintCtx, int nIndex, IntPtr pnStatus, Byte[] RevocationDate, IntPtr pnRevReason);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PRIKEY_Encrypt(IntPtr pCleintCtx, int nSymAlg, StringBuilder sbPasswd, IntPtr pbinstrPriKey, IntPtr pbinstrEncPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PRIKEY_Decrypt(IntPtr pCleintCtx, StringBuilder sbPasswd, IntPtr pbinstrEncPriKey, IntPtr pbinstrPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PRIKEY_ChangePasswd(IntPtr pCleintCtx, StringBuilder sbOldPasswd, StringBuilder sbNewPasswd, IntPtr pbinstrEncPriKey, IntPtr pbinstrNewPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PRIKEY_CheckKeyPair(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_Load(IntPtr pCleintCtx, StringBuilder sbLibPath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_Unload(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //internal static extern int GPKI_STORAGE_ReadCert(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, int nDataType, IntPtr pbinstrCert);
        internal static extern int GPKI_STORAGE_ReadCert(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, int nDataType, ref IntPtr pbinstrCert); // byte[] pbinstrCert
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_ReadPriKey(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, StringBuilder sbPasswd, int nDataType, ref IntPtr pbinStr);   // byte[] pbinstrPriKey  BINSTR
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_WriteCert(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, int nDataType, IntPtr pbinstrCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_WritePriKey(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, StringBuilder sbPasswd, int nDataType, int nSymAlg, IntPtr pbinstrPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_DeleteCert(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, int nDataType);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_DeletePriKey(IntPtr pCleintCtx, int nMediaType, StringBuilder sbInfo, StringBuilder sbPasswd, int nDataType);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_ReadFile(IntPtr pCleintCtx, StringBuilder sbFilePath, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_STORAGE_WriteFile(IntPtr pCleintCtx, StringBuilder sbFilePath, IntPtr pbinstrData);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PRIKEY_CheckKeyPair(IntPtr pCleintCtx, byte[] pbinstrCert, byte[] pbinstrPriKey);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeSignedData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrTBSData, StringBuilder sbSignTime, ref IntPtr pbinstrSignedData); // IntPtr StringBuilder sbSignTime
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeSignedData_File(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, StringBuilder sbTBSDataFilePath, StringBuilder sbSignTime, StringBuilder sbSignedDataFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeSignedData_NoContent_File(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, StringBuilder sbTBSDataFilePath, StringBuilder sbSignTime, StringBuilder sbSignedDataFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeSignedDataWithAddSigner(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrSignedDataIn, StringBuilder sbSignTime, IntPtr pbinstrSignedDataOut);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessSignedData(IntPtr pCleintCtx, IntPtr pbinstrSignedData, IntPtr pbinstrData, IntPtr pbinstrSignerCert, Byte[] SignTime);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessSignedData2(IntPtr pCleintCtx, IntPtr pbinstrSignedData, IntPtr pbinstrData, IntPtr pnSignerCnt);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessSignedData_File(IntPtr pCleintCtx, StringBuilder sbSignedDataFilePath, StringBuilder sbDataFile, IntPtr pbinstrSignerCert, Byte[] SignTime);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessSignedData_NoContent_File(IntPtr pCleintCtx, StringBuilder sbSignedDataFilePath, StringBuilder sbDataFile, IntPtr pbinstrSignerCert, Byte[] SignTime);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_GetSigner(IntPtr pCleintCtx, int nIndex, IntPtr pbinstrCert, Byte[] SignTime);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_GetSignerCaPubs(IntPtr pCleintCtx, IntPtr pbinstrCaPubs);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_GetTBTData(IntPtr pCleintCtx, int nSignerIndex, IntPtr pbinstrTBTData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_GetTimeStampToken(IntPtr pCleintCtx, int nSignerIndex, IntPtr pbinstrTST);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_SetTimeStampToken(IntPtr pCleintCtx, IntPtr pbinstrSignedData, int nSignerIndex, IntPtr pbinstrTST, IntPtr pbinstrSignedDataWithTST);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeEnvelopedData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrTBEData, int nSymAlg, IntPtr pbinstrEnvelopedData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeEnvelopedData_File(IntPtr pCleintCtx, IntPtr pbinstrCert, StringBuilder sbTBEDataFilePath, int nSymAlg, StringBuilder sbEnvelopedDataFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeEnvelopedData_NoContent_File(IntPtr pCleintCtx, IntPtr pbinstrCert, StringBuilder sbTBEDataFilePath, int nSymAlg, StringBuilder sbEnvelopedDataFilePath, StringBuilder sbEncryptedContentFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeEnvelopedDataWithMultiRecipients(IntPtr pCleintCtx, IntPtr pbinstrCerts, IntPtr pbinstrTBEData, int nSymAlg, IntPtr pbinstrEnvelopedData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessEnvelopedData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrEnvelopedData, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessEnvelopedData_File(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, StringBuilder sbEnvelopedDataFilePath, StringBuilder sbDataFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessEnvelopedData_NoContent_File(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, StringBuilder sbEnvelopedDataFilePath, StringBuilder sbEncryptedConentFilePath, StringBuilder sbDataFilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeSignedAndEnvData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrRecCert, IntPtr pbinstrData, int nSymAlg, IntPtr pbinstrSignedAndEnvlopedData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessSignedAndEnvData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrSignedAndEnvlopedData, IntPtr pbinstrData, IntPtr pbinstrSignerCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_MakeEncryptedData(IntPtr pCleintCtx, IntPtr pbinstrTBEData, IntPtr pbinstrEncryptedData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CMS_ProcessEncryptedData(IntPtr pCleintCtx, IntPtr pbinstrKey, IntPtr pbinstrEncryptedData, IntPtr pbinstrData);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_WCMS_MakeSignedContent(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrTBSData, StringBuilder sbSignTime, IntPtr pbinstrSignedContent);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_WCMS_ProcessSignedContent(IntPtr pCleintCtx, IntPtr pbinstrSignedContent, IntPtr pbinstrData, IntPtr pbinstrSignerCert, Byte[] SignTime);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_WCMS_MakeWapEnvelopedData(IntPtr pCleintCtx, IntPtr pbinstrRecCert, IntPtr pbinstrTBEData, int nSymAlg, IntPtr pbinstrWapEnvelopedData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_WCMS_ProcessWapEnvelopedData(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrWapEnvelopedData, IntPtr pbinstrData);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_MakeReqMsg(IntPtr pCleintCtx, IntPtr pbinstrMsg, int nHashAlg, StringBuilder sbPolicy, bool bSign, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrReqMsg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_SendAndRecv(IntPtr pCleintCtx, StringBuilder sbIP, int nPort, IntPtr pbinstrReqMsg,IntPtr pbinstrResMsg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_VerifyResMsg(IntPtr pCleintCtx, IntPtr pbinstrResMsg, IntPtr pbinstrTSACert, IntPtr pbinstrToken);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_VerifyToken(IntPtr pCleintCtx, IntPtr pbinstrDoc, IntPtr pbinstrToken);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_VerifyToken2(IntPtr pCleintCtx, IntPtr pbinstrDoc, IntPtr pbinstrToken,IntPtr pbinstrTSACert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_TSP_GetTokenInfo(IntPtr pCleintCtx, IntPtr pbinstrToken, int nAllocLen, StringBuilder sbCN, StringBuilder sbDN, StringBuilder sbPolicy, StringBuilder sbHashAlg, StringBuilder sbHashValue, StringBuilder sbSerialNum, StringBuilder sbGenTime, StringBuilder sbNonce);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_VID_GetRandomFromPriKey(IntPtr pCleintCtx, IntPtr pbinstrPriKey, IntPtr pbinstrRandom);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_VID_Verify(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrRandom, StringBuilder sbIDN);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_VID_VerifyByIVS(IntPtr pCleintCtx, StringBuilder sbConfFilePath, IntPtr pbinstrCert, IntPtr pbinstrRandom, StringBuilder sbIDN, IntPtr pbinstrMyCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PFX_Export(IntPtr pCleintCtx, StringBuilder sbPasswd, IntPtr pbinstrSignCert, IntPtr pbinstrSignPriKey, IntPtr pbinstrKmCert, IntPtr pbinstrKmPriKey, IntPtr pbinstrPFX);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_PFX_Import(IntPtr pCleintCtx, StringBuilder sbPasswd, IntPtr pbinstrPFX, IntPtr pbinstrSignCert, IntPtr pbinstrSignPriKey, IntPtr pbinstrKmCert, IntPtr pbinstrKmPriKey);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_GenRandom(IntPtr pCleintCtx, int nLen, IntPtr pbinstrRandom);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_GenKeyAndIV(IntPtr pCleintCtx, int nSymAlg);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_SetKeyAndIV(IntPtr pCleintCtx, int nSymAlg, IntPtr pbinstrKey, IntPtr pbinstrIV);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_GetKeyAndIV(IntPtr pCleintCtx, IntPtr pnSymAlg, IntPtr pbinstrKey, IntPtr pbinstrIV);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_ClearKeyAndIV(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Encrypt(IntPtr pCleintCtx, IntPtr pbinstrPlainText, IntPtr pbinstrCipherText);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Encrypt_File(IntPtr pCleintCtx, StringBuilder sbPlainTextFile, StringBuilder sbCipherTextFile);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Decrypt(IntPtr pCleintCtx, IntPtr pbinstrCipherText, IntPtr pbinstrPlainText);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Decrypt_File(IntPtr pCleintCtx, StringBuilder sbCipherTextFile, StringBuilder sbPlainTextFile);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Sign(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, int nHashAlg, IntPtr pbinstrTBSData, IntPtr pbinstrSignature);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Sign_File(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey, int nHashAlg, StringBuilder sbDataFilePath, StringBuilder sbSignatuoutilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Verify(IntPtr pCleintCtx, IntPtr pbinstrCert, int nHashAlg, IntPtr pbinstrData, IntPtr pbinstrSignature);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Verify_File(IntPtr pCleintCtx, IntPtr pbinstrCert, int nHashAlg, StringBuilder sbDataFilePath, StringBuilder sbSignatuoutilePath);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_AsymEncrypt(IntPtr pCleintCtx, int nKeyType, IntPtr pbinstrKey, IntPtr pbinstrTBEData, IntPtr pbinstrEncData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_AsymDecrypt(IntPtr pCleintCtx, int nKeyType, IntPtr pbinstrKey, IntPtr pbinstrEncData, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_Hash(IntPtr pCleintCtx, int nHashAlg, IntPtr pbinstrTBHData, IntPtr pbinstrDigest);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_GenMAC(IntPtr pCleintCtx, int nMACAlg, StringBuilder sbPasswd, IntPtr pbinstrTBMData, IntPtr pbinstrMAC);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_CRYPT_VerifyMAC(IntPtr pCleintCtx, int nMACAlg, StringBuilder sbPasswd, IntPtr pbinstrData, IntPtr pbinstrMAC);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_GetSlotList(IntPtr pCleintCtx, uint[] pSlotList, IntPtr puintSlotCnt);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_GetTokenInfo(IntPtr pCleintCtx, uint ulSlotID, IntPtr p_PKCS11_TOKEN_INFO_TokenInfo);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_OpenSession(IntPtr pCleintCtx, uint ulSlotID, int nTokenName);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_Login(IntPtr pCleintCtx, StringBuilder sbPIN);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_Logout(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_CloseSession(IntPtr pCleintCtx);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_ChangePIN(IntPtr pCleintCtx, StringBuilder sbOldPIN, StringBuilder sbNewPIN);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_GetCertCnt(IntPtr pCleintCtx, IntPtr pnCnt);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_GetCertInfo(IntPtr pCleintCtx, int nIndex, IntPtr pnCertType,IntPtr pbinstrCertDN, IntPtr pbinstrKeyID);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_ReadCert(IntPtr pCleintCtx, IntPtr pbinstrKeyID, IntPtr pbinstrCert);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_ReadRandomForVID(IntPtr pCleintCtx, IntPtr pbinstrKeyID, IntPtr pbinstrRandom);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_WriteCertAndPriKey(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrPriKey);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_DeleteCertAndPriKey(IntPtr pCleintCtx, IntPtr pbinstrKeyID);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_HSM_SignData(IntPtr pCleintCtx, IntPtr pbinstrKeyID, IntPtr pbinstrData, IntPtr pbinstrSignedData);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_BASE64_Encode(IntPtr pCleintCtx, IntPtr pbinstrData, IntPtr pbinstrEncData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_BASE64_Decode(IntPtr pCleintCtx, IntPtr pbinstrEncData, IntPtr pbinstrData);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_LDAP_GetDataByURL(IntPtr pCleintCtx, int nDataType, StringBuilder sbURL, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_LDAP_GetDataByCN(IntPtr pCleintCtx, StringBuilder sbIP, int nPort, StringBuilder sbCN, int nDataType, Byte[] FullDN, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_LDAP_GetAnyDataByURL(IntPtr pCleintCtx, StringBuilder sbAttribute, StringBuilder sbURL, IntPtr pbinstrData);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_LDAP_GetCRLByCert(IntPtr pCleintCtx, IntPtr pbinstrCert, IntPtr pbinstrCRL);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_LDAP_GetCertPath(IntPtr pCleintCtx, IntPtr pbinstrCert, StringBuilder sbConfFilePath, IntPtr pbinstrPath);

        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_SIGEA_MakeChallenge(IntPtr pCleintCtx, IntPtr pbinstrChallenge);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_SIGEA_MakeResponse(IntPtr pCleintCtx, IntPtr pbinstrChallenge, IntPtr pbinstrCert, IntPtr pbinstrPriKey, IntPtr pbinstrResponse);
        [DllImport(strGpkiLibName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GPKI_SIGEA_VerifyResponse(IntPtr pCleintCtx, IntPtr pbinstrResponse, IntPtr pbinstrChallenge, IntPtr pbinstrCert);
#endif

    }

    public class GPKICA
    {
        Dictionary<string, string> DicGpkiCA = new Dictionary<string, string>();
        public void SetData(string tag, string value)
        {
            string strTmp = "";
            if (DicGpkiCA.TryGetValue(tag, out strTmp) == true)
            {
                DicGpkiCA.Remove(tag);
            }
            DicGpkiCA[tag] = value;
        }
        public string GetData(string tag)
        {
            string strTmp = "";
            if (DicGpkiCA.TryGetValue(tag, out strTmp) != true)
            {
                return strTmp;
            }
            return DicGpkiCA[tag];
        }
    }
    public class GPKIFileInfo
    {
        public string m_strFileName;            // GPKI 인증서 파일(Cer)명
        public string m_strKeyFilePath;            // GPKI 인증서 파일(Key)명
        public string m_strUserID;              // GPKI 인증서 ID
        public string m_strExpiredDate;         // GPKI 인증서 만료일자
        public string m_strKeyUse;              // GPKI 인증서 사용 용도.
        public string m_strOrg;                 // GPKI 인증서 발급기관.
        //public GPKICA m_gpkiCA;                 // GPKI 인증서 발급기관 상세정보.
        //public string m_strOID;                   // GPKI 인증서 OID
        public int m_nRemainDay;                  // GPKI 인증서 남은 유효 기간
        public string m_selected { get; set; }
        public bool m_bIsRegisteredServer;         // Server에 CN이 등록되어 있는지 유무
        public byte[] m_pKeyData;


        public GPKIFileInfo()
        {
            m_strFileName = m_strUserID = m_strKeyUse = m_strOrg = "";
            m_nRemainDay = 0;
            m_selected = "";
            //m_strOID = "";
            //m_gpkiCA = new GPKICA();
            m_pKeyData = null;
        }
        public void SetGPKIInfo(string userID, string expiredDate, string KeyUse, string Org, int nRemainDay)
        {
            m_strUserID = userID;               // GPKI 인증서 사용자 계정.
            m_strExpiredDate = expiredDate;     // GPKI 인증서 만료일자
            m_strKeyUse = KeyUse;               // GPKI 인증서 사용 용도.
            m_strOrg = Org;                     // GPKI 인증서 발급 기관.
            m_nRemainDay = nRemainDay;          // GPKI 인증서 사용가능 날짜
        }
    }
    public class SGGpkiLib
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGGpkiLib>();

        public List<GPKIFileInfo> listGpkiFile = new List<GPKIFileInfo>();
        public string m_strWorkDir = "GPKI";
        private IntPtr m_pClientCtx = IntPtr.Zero;
        private string m_strBaseGPKIPath = "GPKI/certificate/Class2";

        private Dictionary<string, string> DicGpkiIsser = new Dictionary<string, string>();
        private Dictionary<string, string> DicGpkiOid = new Dictionary<string, string>();

        public SGGpkiLib()
        {
            GPKIUsageInit();
            GPKIOidInit();
        }
        ~SGGpkiLib()
        {
            GPKI_Finish();
        }
        public void GPKIUsageInit()
        {
            DicGpkiOid["1.2.410.100001.2.2.1"] = "행정기관(개인용)";
            DicGpkiOid["1.2.410.100001.2.1.1"] = "행정기관(전자관인용)";
            DicGpkiOid["1.2.410.100001.2.1.2"] = "행정기관(서버용)";
            DicGpkiOid["1.2.410.100001.2.1.3"] = "행정기관(특수목적용)";

            DicGpkiOid["1.2.410.100001.2.2.2"] = "공공기관(개인용)";
            DicGpkiOid["1.2.410.100001.2.1.4"] = "공공기관(전자관인용)";
            DicGpkiOid["1.2.410.100001.2.1.5"] = "공공기관(서버용)";
            DicGpkiOid["1.2.410.100001.2.1.6"] = "공공기관(특수목적용)";
        }
        public string GetConvGpkiOID(string strOID)
        {
            strOID = strOID.Replace(" ", ".");
            string strTmp = "";
            if(DicGpkiOid.TryGetValue(strOID, out strTmp)!=true)
            {
                return "공무원용";
            }
            return DicGpkiOid[strOID];
        }
        public void GPKIOidInit()
        {
            DicGpkiIsser["CertRSA01"] = "한국정보보호진흥원(KISA";
            DicGpkiIsser["KISA RootCA 1"] = "한국정보보호진흥원(KISA";
            DicGpkiIsser["KISA RootCA 4"] = "한국정보보호진흥원(KISA";
            DicGpkiIsser["yessignCA"] = "금융결제원";
            DicGpkiIsser["NCASign CA"] = "한국전산원";
            DicGpkiIsser["NCASignCA"] = "한국전산원";
            DicGpkiIsser["SignKorea CA"] = "KOSCOM";
            DicGpkiIsser["signGATE CA"] = "정보인증";
            DicGpkiIsser["signGATE CA2"] = "정보인증";
            DicGpkiIsser["signGATE FTCA02"] = "정보인증";
            DicGpkiIsser["CrossCertCA"] = "전자인증";
            DicGpkiIsser["CrossCert Certificate Authority"] = "전자인증";
            DicGpkiIsser["TradeSignCA"] = "한국무역정보통신";
            DicGpkiIsser["CA131000002"] = "행정안전부";
            DicGpkiIsser["CA128000002"] = "대검찰청";
            DicGpkiIsser["CA130000002"] = "병무청";
            DicGpkiIsser["CA129000001"] = "국방부";
            DicGpkiIsser["CA131000001"] = "행정안전부";
            DicGpkiIsser["CA131000005"] = "행정안전부";
            DicGpkiIsser["CA131000009"] = "행정안전부";
            DicGpkiIsser["CA131000010"] = "행정안전부";
            DicGpkiIsser["CA134040001"] = "교육과학기술부";
            DicGpkiIsser["CA974000001"] = "대법원";
            DicGpkiIsser["CA134100031"] = "교과부";
            DicGpkiIsser["CA134040031T"] = "교과부";
            DicGpkiIsser["SignKorea CA2"] = "KOSCOM";
            DicGpkiIsser["yessignCA Class 1"] = "금융결제원";
            DicGpkiIsser["CrossCertCA2"] = "전자인증";
            DicGpkiIsser["signGATE CA4"] = "정보인증";
            DicGpkiIsser["TradeSignCA2"] = "한국무역정보통신";
            DicGpkiIsser["GPKIRootCA1 "] = "행정안전부";
            DicGpkiIsser["GPKIRootCA"] = "행정안전부";
            DicGpkiIsser["Root CA"] = "행정안전부";
            DicGpkiIsser["Class 3 CA"] = "행정안전부";
            DicGpkiIsser["CA131000001"] = "행정안전부";
            DicGpkiIsser["CA131100001"] = "행정안전부";
            DicGpkiIsser["CA131000002"] = "행정안전부";
            DicGpkiIsser["CA128000001"] = "행정안전부";
            DicGpkiIsser["CA128000002"] = "행정안전부";
            DicGpkiIsser["CA128000002"] = "행정안전부";
            DicGpkiIsser["LGCare Online CA"] = "전자인증";
            DicGpkiIsser["MND CA"] = "국방부";
            DicGpkiIsser["CA131100002"] = "행정안전부";
            DicGpkiIsser["CA974000002"] = "대법원";
            DicGpkiIsser["CA974000031"] = "대법원";
            DicGpkiIsser["CA128000031"] = "대검찰청";
            DicGpkiIsser["CA128000032"] = "대검찰청";
            DicGpkiIsser["CA130000031"] = "병무청";
        }
        public string GetConvGpkiIsser(string strIsser)
        {
            if ( (!strIsser.Contains("CN=")) && (!strIsser.Contains("cn=")) )
                return "-";

            string[] strSubIsser = null;
            strSubIsser=strIsser.Split(",");
            if ((strSubIsser == null) || (strSubIsser.Length <= 0))
                return "-";

            string strCN = strSubIsser[0].Substring(2, strSubIsser[0].Length-2);
            strCN = strCN.Trim();

            string strTmp = "";
            if (DicGpkiIsser.TryGetValue(strCN, out strTmp) != true)
            {
                return "행정안전부";
            }
            return DicGpkiIsser[strCN];
        }
        public bool GPKI_Init()
        {
        #if _WINDOWS
            int ret = -1;
            m_strWorkDir = Path.Combine(System.IO.Directory.GetCurrentDirectory(), m_strWorkDir);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                m_strWorkDir = m_strWorkDir.Replace("/", "\\");
            }
            else
            {
                m_strWorkDir = m_strWorkDir.Replace("\\", "/");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(m_strWorkDir);

            try 
            {
                ret = HsGpkiLib.GPKI_API_Init(ref m_pClientCtx, sb);
                if ((ret != 0) && (m_pClientCtx != IntPtr.Zero))
                {
                    CLog.Here().Error($"GPKI_API_Init error!! ret={ret}");
                    CLog.Here().Error(String.Format($"GPKI_API_Init error!! ret={ret}"));
                    return false;
                }
            }
            catch
            {
                CLog.Here().Error($"System.DllNotFoundException: 'Unable to load DLL 'gpkiapi64.dll'");
                return false;
            }

        #endif
            return true;
        }
        public void GPKI_Finish()
        {
        #if _WINDOWS
            if (m_pClientCtx != IntPtr.Zero)
                HsGpkiLib.GPKI_API_Finish(ref m_pClientCtx);
        #endif
        }

        /**
        *@breif BINSTR 객체 생성 여부를 확인한다.
        *@param ref BINSTR
        *@param out byte[]
        *@return true 성공
        */
        public bool GPKIBinStrCreate(ref BINSTR binstr, out byte[] byteBinStr)
        {
        #if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
            {
                byteBinStr = null;
                return false;
            }

            IntPtr bytePtr = IntPtr.Zero;
            byte[] bData = StructToBytes(binstr);
            int nRet = HsGpkiLib.GPKI_BINSTR_Create(bData);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                CLog.Here().Error($"GPKI_BINSTR_Create Error");
                byteBinStr = null;
                return false;
            }
            byteBinStr = bData;
			return true;
        #else
            byteBinStr = null;
            return false;
        #endif
        }




        /**
       *@breif BINSTR 객체 제거 동작
       *@param byte[] 타입
       *@return true 성공
       */
        public int GPKI_BINSTR_Delete(ref byte[] byteFree)
        {
        #if _WINDOWS
            int nRet = (int)eGpkiError.ERR_READY;

            try
            {
                unsafe
                {
                    fixed (byte* pData = byteFree)
                    {
                        IntPtr ptrData = (IntPtr)pData;
                        nRet = HsGpkiLib.GPKI_BINSTR_Delete(ref ptrData);
                    }

                    byteFree = null;
                }
            }
            catch (Exception e)
            {
                nRet = (int)eGpkiError.ERR_EXCEPTION;
                CLog.Here().Error($"GPKI_BINSTR_Delete Exception(source:{e.Source}) - Msg : {e.Message}");
            }
            finally
            {
            }

            return nRet;
        #else
            
            return 0;
        #endif
        }

        /**
       *@breif BINSTR 객체 해제 여부를 확인한다.
       *@param BINSTR
       *@return true 성공
       */
        public bool GPKIBinReadPriKey(GPKIFileInfo gpkiFile, string strPassWD)
        {
        #if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return false;

            StringBuilder sbKeyPath = new StringBuilder();
            StringBuilder sbPassWD = new StringBuilder();
            sbKeyPath.Append(gpkiFile.m_strKeyFilePath);
            sbPassWD.Append(strPassWD);

            IntPtr ptrPkiKey = IntPtr.Zero;

            int nRet = HsGpkiLib.GPKI_STORAGE_ReadPriKey(m_pClientCtx, (int)eGpkiMediaType.MEDIA_TYPE_FILE_PATH, 
                sbKeyPath, sbPassWD, (int)eGpkiDataType.DATA_TYPE_GPKI_SIGN, ref ptrPkiKey);

            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = "";
                if (nRet == (int)eGpkiError.ERR_WRONG_PRIKEY)
                    strErrMsg = "ERR_WRONG_PRIKEY";
                else if (nRet == (int)eGpkiError.ERR_WRONG_PASSWORD)
                    strErrMsg = "ERR_WRONG_PASSWORD";
                else if (nRet == (int)eGpkiError.ERR_ENCRYPT_PRIKEY)
                    strErrMsg = "ERR_ENCRYPT_PRIKEY";
                else if (nRet == (int)eGpkiError.ERR_NOT_MATCHED_KEY_PAIR)
                    strErrMsg = "ERR_NOT_MATCHED_KEY_PAIR";
                else if (nRet == (int)eGpkiError.ERR_WRONG_ENC_PRIKEY)
                    strErrMsg = "ERR_WRONG_ENC_PRIKEY";
                else if (nRet == (int)eGpkiError.ERR_NOT_PRIKEY)
                    strErrMsg = "ERR_NOT_PRIKEY";
                else if (nRet == (int)eGpkiError.ERR_NOT_ENC_PRIKEY)
                    strErrMsg = "ERR_NOT_ENC_PRIKEY";
                else
                    strErrMsg = GetGpkiError((eGpkiError)nRet);

                CLog.Here().Error($"GPKI_STORAGE_ReadPriKey Error - PW check - Failed (errorCode :{nRet}) : {strErrMsg}");

                return false;
            }

            if (gpkiFile.m_pKeyData != null)
            {
                // nRet = GPKI_BINSTR_Delete(ref gpkiFile.m_pKeyData);  // error
                gpkiFile.m_pKeyData = null;
            }


            bool bRet = false;

            try
            {
                unsafe
                {
                    int* pDataSize = (int*)&ptrPkiKey;
                    pDataSize += 2; // Size를 알수  있는 위치로 pointer 이동
                    Int32* pSizePos = (Int32*)pDataSize;
                    Int32 nDataSize = *pSizePos;
                    byte* ptrData = (byte*)ptrPkiKey.ToPointer();

                    gpkiFile.m_pKeyData = new byte[nDataSize];
                    Marshal.Copy(ptrPkiKey, gpkiFile.m_pKeyData, 0, nDataSize);
                    HsGpkiLib.GPKI_BINSTR_Delete(ref ptrPkiKey);
                    bRet = true;
                }

            }
            catch(Exception e)
            {
                bRet = false;
                CLog.Here().Error($"GPKI_STORAGE_ReadPriKey - Copy Memory Exception(source:{e.Source}) - Msg : {e.Message}");
            }
            finally
            {
            }
			
            // 예전 사용 code
            /*
               ref byte[] byteBinStr;
gpkiFile.m_pKeyData = new byte[bsPkiKey.nLen];
bsPkiKey.pData.CopyTo(gpkiFile.m_pKeyData, 0);
HsGpkiLib.GPKI_BINSTR_Delete(ptrPkiKey);

gpkiFile.m_pKeyData = new byte[byteBinStr.Length];
byteBinStr.CopyTo(gpkiFile.m_pKeyData, 0);
HsGpkiLib.GPKI_BINSTR_Delete(byteBinStr);*/			
			
            return bRet;

        #else

            return true;

        #endif

        }

        /**
       *@breif BINSTR 객체 해제 여부를 확인한다.
       *@param BINSTR
       *@return true 성공
       */
        public int GPKIBinStrDelete(ref BINSTR binstr)
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
            {
                return -1;
            }
            IntPtr bytePtr = IntPtr.Zero;
            byte[] bData = StructToBytes(binstr);
            return GPKI_BINSTR_Delete(ref bData);
#else
            return 0;
#endif
        }

        /**
        *@breif GPKI 동작관련 에러 메시지를 반환한다. 
        *@param eGpkiError 에러코드
        *@return 에러메시지
        */
        public string GetGpkiError(eGpkiError err)
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_API_GetErrInfo(m_pClientCtx, 128, sb);
            if (nRet == (int)eGpkiError.GPKI_OK)
            {
                CLog.Here().Error($"Gpki Error Code = {err.ToString()}");
                CLog.Here().Error($"Gpki Error Msg = {sb.ToString()}");
                //return "";
            }
            return sb.ToString();
#else
            return "";
#endif
        }
        /**
        *@breif 정보를 확인할 인증서를 로드한다. 
        *@param bsCert
        *@return true 성공
        */
        public bool GpkiLoad(IntPtr binStr)  // ref byte[] binStr , ref IntPtr ptrBinStr
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return false;

            if (!GpkiUnLoad())
            {
                //HsLog.err(String.Format("GpkiLoad Unload Error!"));
                return false;
            }
            int nRet = HsGpkiLib.GPKI_CERT_Load(m_pClientCtx, binStr);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GpkiLoad ErrMsg = {strErrMsg}");
                return false;
            }
#endif
            return true;
        }
        /**
        *@breif 로드된 인증서를 Unload 한다. 
        *@return true 성공
        */
        public bool GpkiUnLoad()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return false;

            int nRet = HsGpkiLib.GPKI_CERT_Unload(m_pClientCtx);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GpkiUnLoad ErrMsg = {strErrMsg}");
                return false;
            }
#endif
            return true;
        }
        /**
        *@breif 해당 인증서의 사용자 계정을 반환한다.
        *@return 사용자 계정
        */
        public string GetGpkiUserID()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_CERT_GetUID(m_pClientCtx, 128, sb);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGpkiUserID ErrMsg = {strErrMsg}");
                return "";
            }
            return sb.ToString();
#else
            return "";
#endif
        }
        /**
        *@breif 인증서의 유효기간을 반환한다.
        *@return 인증서 유효기간
        */
        public string GetGpkiValidate()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_CERT_GetValidity(m_pClientCtx, 128, sb);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGpkiValidate ErrMsg = {strErrMsg}");
                return "";
            }

            string strDate = sb.ToString();
            string[] strSubDate = strDate.Split("~");
            if ( (strSubDate == null) || (strSubDate.Length <= 1) )
                return "";
            strDate = strSubDate[1];
            strDate = strDate.Trim();
            DateTime time = Convert.ToDateTime(strDate);
            return time.ToString("yyyy-MM-dd");
#else
            return "";
#endif
        }
        /**
        *@breif 인증서의 용도를 반환한다.
        *@return 인증서 용도
        */
        public string GetGpkiKeyUseCase()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_CERT_GetKeyUsage(m_pClientCtx, 128, sb);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGpkiKeyUseCase ErrMsg = {strErrMsg}");
                return "";
            }
            return sb.ToString();
#else
            return "";
#endif
        }
        /**
        *@breif 해당 인증서의 남은 날짜를 반환한다.
        *@param bsCert
        *@return 해당 인증서의 남은 날짜
        */
        public int GetRemainDays(IntPtr ptrCerBinstr)  // byte[] bsCert
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return -1;

            //IntPtr ptrRemainDay = new IntPtr(0);
            int nRemainDay = 0;
            int nRet = HsGpkiLib.GPKI_CERT_GetRemainDays(m_pClientCtx, ptrCerBinstr, ref nRemainDay);
            if (nRet != (int)eGpkiError.GPKI_OK && nRet != (int)eGpkiError.ERR_EXPIRED_CERT)	// 만료된 인증서 1203 return
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetRemainDays ErrMsg = {strErrMsg}");
                return -1;
            }

            //return ptrRemainDay.ToInt32();
            return nRemainDay;
#else
            return 0;
#endif
        }
        /**
        *@breif 인증서 정책 식별자의 OID 값을 반환한다.
        *@return 인증서 정책 식별자의 OID 값
        */
        public string GetGPKIOID()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "-";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_CERT_GetCertPolicyID(m_pClientCtx, 128,sb);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGPKIOID ErrMsg = {strErrMsg}");
                return "-";
            }

            string strGPKIOID = GetConvGpkiOID(sb.ToString());
            return strGPKIOID;
#else
            return "";
#endif
        }

        /**
        *@breif 발급자 이름을 반환한다.
        *@return 발급자 이름
        */
        public string GetGPKIIssuerName()
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return "-";

            StringBuilder sb = new StringBuilder(128);
            int nRet = HsGpkiLib.GPKI_CERT_GetIssuerName(m_pClientCtx, 128, sb);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGPKIIssuerName ErrMsg = {strErrMsg}");
                return "-";
            }

            string strIsserName = GetConvGpkiIsser(sb.ToString());
            return strIsserName;
#else
            return "";
#endif
        }
        /**
        *@breif 랜덤값을 생성한다.
        *@param 생성된 랜덤값 버퍼
        *@return 랜덤값 길이
        */
        public int GetGenRandom(ref Byte[] randomKey)
        {
#if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return -1;

            IntPtr ptrRandom = IntPtr.Zero;
            int nRet = HsGpkiLib.GPKI_BINSTR_Create(randomKey);
            //int nRet = HsGpkiLib.GPKI_BINSTR_Create(out ptrRandom);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGenRandom GPKI_BINSTR_Create ErrMsg = {strErrMsg}");
                return -1;
            }

            nRet = HsGpkiLib.GPKI_CRYPT_GenRandom(m_pClientCtx, 20, ptrRandom);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GetGenRandom GPKI_CRYPT_GenRandom ErrMsg = {strErrMsg}");
                return -1;
            }

            BINSTR binStr = new BINSTR();
            Marshal.PtrToStructure(ptrRandom, binStr);

            int nRetLen = binStr.nLen;
            if (nRetLen != 20)
            {
                CLog.Here().Error($"GetGenRandom binStr.nLen Error!");
                return -1;
            }
            else
                Buffer.BlockCopy(randomKey, 0, binStr.pData, 0, nRetLen);

            Marshal.StructureToPtr(binStr, ptrRandom, true);
            if (ptrRandom == IntPtr.Zero)
            {
                CLog.Here().Error($"GetGenRandom BINSTR to IntPtr Error!");
                return -1;
            }

            return nRetLen;
#else
            return 0;
#endif
        }


        /**
        *@breif GPKI 인증 적용된  Data를 읽어온다.
        *@param bsCert
        *@param bsCert
        *@return 
        */
        public unsafe int GPKI_CMS_MakeSignedData(IntPtr ptrCert, IntPtr ptrPrikey, IntPtr ptrRandom, StringBuilder sbSignTime, ref byte[] byteSignedData)
        {
        #if _WINDOWS
            int nRet = 0;
            IntPtr ptrSignedData = IntPtr.Zero;

            nRet = HsGpkiLib.GPKI_CMS_MakeSignedData(m_pClientCtx, ptrCert, ptrPrikey, ptrRandom, sbSignTime, ref ptrSignedData);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GPKI_CMS_MakeSignedData Error(errorCode :{nRet}), Message : {strErrMsg}");
                return nRet;
            }

            try
            {
                unsafe
                {
                    int* pDataSize = (int*)&ptrSignedData;
                    pDataSize += 2; // Size를 알수  있는 위치로 pointer 이동
                    Int32* pSizePos = (Int32*)pDataSize;
                    Int32 nDataSize = *pSizePos;
                    byte* ptrData = (byte*)ptrSignedData.ToPointer();

                    byteSignedData = new byte[nDataSize];
                    Marshal.Copy(ptrSignedData, byteSignedData, 0, nDataSize);
                    HsGpkiLib.GPKI_BINSTR_Delete(ref ptrSignedData);
                }

            }
            catch (Exception e)
            {
                nRet = (int)eGpkiError.ERR_EXCEPTION;
                CLog.Here().Error($"GPKI_CMS_MakeSignedData - Copy Memory Exception(source:{e.Source}) - Msg : {e.Message}");                
            }
            finally
            {
            }

            return nRet;

        #else

            return 0;

        #endif
        }


        /**
        *@breif GPKI 인증서 정보를 저장매체에서 읽어온다.(Copy)
        *@param strGPKIFullPath GPKI 파일 Full Path
        *@param bsCert
        *@return IntPtr
        **/
        public int GPKI_STORAGE_ReadCert(string strGPKIFullPath, ref byte[] ptrCert)
        {
        #if _WINDOWS
            if (m_pClientCtx == IntPtr.Zero)
                return (int)eGpkiError.ERR_UNDEF_APP_ERROR;

            StringBuilder sbGPKIFullPath = new StringBuilder(strGPKIFullPath);

            IntPtr ptrCertData = IntPtr.Zero;

            int nRet = HsGpkiLib.GPKI_STORAGE_ReadCert(m_pClientCtx, (int)eGpkiMediaType.MEDIA_TYPE_FILE_PATH, sbGPKIFullPath, (int)eGpkiDataType.DATA_TYPE_GPKI_SIGN, ref ptrCertData);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GPKI_STORAGE_ReadCert Error(errorCode :{nRet}), Message : {strErrMsg}");
                return nRet;
            }

            try
            {
                unsafe
                {
                    int* pDataSize = (int*)&ptrCertData;
                    pDataSize += 2; // Size를 알수  있는 위치로 pointer 이동
                    Int32* pSizePos = (Int32*)pDataSize;
                    Int32 nDataSize = *pSizePos;
                    byte* ptrData = (byte*)ptrCertData.ToPointer();

                    ptrCert = new byte[nDataSize];
                    Marshal.Copy(ptrCertData, ptrCert, 0, nDataSize);
                    HsGpkiLib.GPKI_BINSTR_Delete(ref ptrCertData);
                }

            }
            catch (Exception e)
            {
                nRet = (int)eGpkiError.ERR_EXCEPTION;
                CLog.Here().Error($"GPKI_STORAGE_ReadPriKey - Copy Memory Exception(source:{e.Source}) - Msg : {e.Message}");
            }
            finally
            {
            }

            return nRet;

        #else
		        return 0;
        #endif

        }


        /**
        *@breif 특정경로의 GPKI 파일리스트를 가져온다.
        *@param GPKI 파일 경로.
        *@return GPKI 파일 리스트
        */
        public string[] FindGPKIFile(string strGPKIPath)
        {
            DirectoryInfo di = new DirectoryInfo(strGPKIPath);
            if(!di.Exists)
            {
                CLog.Here().Information($"GPKI Directory Not Found = {strGPKIPath}");
                return null;
            }
            string[] strFileList = null;
            strFileList = Directory.GetFiles(strGPKIPath);
            if ((strFileList.Length <= 0) || (strFileList == null))
                return null;

            int nFileListLength = 0;
            for (int i = 0; i < strFileList.Length; i++)
            {
                string strFileName = Path.GetFileName(strFileList[i]);
                if (strFileName.Contains("_sig"))
                    nFileListLength++;
            }

            if (nFileListLength <= 0)
                return null;

            string[] strRetFileList = null;
            strRetFileList = new string[nFileListLength];
            int idx = 0;
            for (int i = 0; i < strFileList.Length; i++)
            {
                if (idx > nFileListLength)
                    break;
                string strFileName = Path.GetFileName(strFileList[i]);
                if (strFileName.Contains("_sig"))
                    strRetFileList[idx++] = strFileList[i];
            }

            return strRetFileList;
        }
        /**
        *@breif 특정경로의 GPKI 파일리스트를 가져온다.
        *@param GPKI 파일 경로.
        *@return GPKI 파일 리스트
        */
        public int FindGPKIFileWithKey(string strGPKIPath, ref Dictionary<string, string> DicGpkiFile)
        {
            DirectoryInfo di = new DirectoryInfo(strGPKIPath);
            if (!di.Exists)
            {
                CLog.Here().Information($"GPKI Directory Not Found = {strGPKIPath}");
                return 0;
            }
            string[] strFileList = null;
            strFileList = Directory.GetFiles(strGPKIPath);
            if ((strFileList.Length <= 0) || (strFileList == null))
            {
                CLog.Here().Information($"GPKI FileList Not Found = {strGPKIPath}");
                return 0;
            }

            for (int i = 0; i < strFileList.Length; i++)
            {
                string strFileName = Path.GetFileName(strFileList[i]);
                string strFileExt = Path.GetExtension(strFileList[i]);
                strFileExt.ToLower();
                if (strFileName.Contains("_sig") && strFileExt.Equals(".cer"))
                {

                    CLog.Here().Information($"GPKI Cert File Found = {strFileList[i]}");

                    string strKeyFilePath = Path.ChangeExtension(strFileList[i], "key");

                    if (File.Exists(strKeyFilePath))
                    {
                        CLog.Here().Information($"GPKI Key File Found = {strKeyFilePath} ");
                        DicGpkiFile.Add(strFileList[i], strKeyFilePath);                        
                    }
                    else
                    {
                        CLog.Here().Information($"But Key File Not Found = {strKeyFilePath} ");
                    }
                }
            }

            return DicGpkiFile.Count;
        }

        /**
        *@breif 현재 Drive GPKI 인증서 CN의 등록상태를 확인
        *@return true 성공
        */
        public bool RequestGPKILocalCNStatus()
        {

            return true;
        }

        /**
        *@breif 기본 하드디스크의 GPKI 파일들을 로드한다..
        *@return true 성공
        */
        public bool BINSTR2inPtr(BINSTR binstr, ref IntPtr ptrBinSTRData)
        {

            // Marshal.StructureToPtr<BINSTR>(binstr, ptrBinSTRData, true);

            if (binstr.pData.Length < 1 || binstr.nLen < 1)
                return false;


            byte[] buffer = new byte[12];
            bool bRet = false;

            try
            {
                unsafe
                {
                    fixed (byte* ptrBuf = binstr.pData)
                    {
                        long ptrDataPos = (long)ptrBuf;

                        ptrBinSTRData = (IntPtr)ptrBuf;
                        // buffer = ptrBuf;

                        byte[] byteAddress = BitConverter.GetBytes(ptrDataPos);
                        byte[] byteSize = BitConverter.GetBytes(binstr.nLen);

                        byteAddress.CopyTo(buffer, 0);
                        Buffer.BlockCopy(byteSize, 0, buffer, 8, 4);

                        // do you stuff here                        
                    }

                    fixed (byte* ptrBinStr = buffer)
                    {
                        ptrBinSTRData = (IntPtr)ptrBinStr;
                        bRet = true;
                    }
                }
            }
            catch (Exception e)
            {
                bRet = false;
                CLog.Here().Error($"BINSTR2inPtr - Copy Memory Exception(source:{e.Source}) - Msg : {e.Message}");
            }
            finally
            {
            }


            return bRet;
        }

        /**
        *@breif 
        *@return true 성공
        */
        public unsafe bool LoadCertKeyFileList(string strGPKIFullPath)
        {

            CLog.Here().Information($"GPKI Path = {strGPKIFullPath}");

            Dictionary<string, string> DicGpkiFile = new Dictionary<string, string>();
            int nGpkiWithKeyFileCnt = FindGPKIFileWithKey(strGPKIFullPath, ref DicGpkiFile);
            if (nGpkiWithKeyFileCnt < 1)
            {
                CLog.Here().Information($"GPKI File(Cert & key Pair Exist) Empty!!");
                listGpkiFile.Clear();
                return false;
            }

            listGpkiFile.Clear();

            byte[] byteBinStr = null;

            for (int i = 0; i < DicGpkiFile.Count; i++)
            {

                string strFilename = Path.GetFileName(DicGpkiFile.ElementAt(i).Key);

                IntPtr ptrBinstr = IntPtr.Zero;
                byteBinStr = null;

                int nRet = GPKI_STORAGE_ReadCert(DicGpkiFile.ElementAt(i).Key, ref byteBinStr);
                if (nRet != (int)eGpkiError.GPKI_OK)
                {
                    string strErrMsg = GetGpkiError((eGpkiError)nRet);
                    CLog.Here().Error($"GPKI_STORAGE_ReadCert (read Fail) - error:{nRet} - msg : {strErrMsg}, File : {strFilename} ");
                    continue;
                }

                BINSTR binstr = new BINSTR();

                binstr.nLen = byteBinStr.Length;
                binstr.pData = byteBinStr;
                IntPtr ptrBinSTRData = IntPtr.Zero;

                if (BINSTR2inPtr(binstr, ref ptrBinSTRData) == false)
                {
                    CLog.Here().Error($"BiNSTR2inPtr error!!");
                    continue;
                }

                if (!GpkiLoad(ptrBinSTRData))  // ref ptrBinSTRData
                {
                    CLog.Here().Error($"{strFilename} is Load Fail");
                    continue;
                }

                int nRemainDay = GetRemainDays(ptrBinSTRData);

                GPKIFileInfo gpkiFileInfo = null;
                gpkiFileInfo = new GPKIFileInfo();
                gpkiFileInfo.m_strFileName = DicGpkiFile.ElementAt(i).Key;
                gpkiFileInfo.m_strKeyFilePath = DicGpkiFile.ElementAt(i).Value;

                string strUserID = GetGpkiUserID();
                string strExpiredDate = GetGpkiValidate();
                string strKeyUse = GetGPKIOID();
                string strOrg = GetGPKIIssuerName();

                gpkiFileInfo.SetGPKIInfo(strUserID, strExpiredDate, strKeyUse, strOrg, nRemainDay);
                listGpkiFile.Add(gpkiFileInfo);
            }

            return true;
        }

        /**
        *@breif 기본 하드디스크의 GPKI 파일들을 로드한다..
        *@return true 성공
        */
        public unsafe bool LoadHardDiskGPKICertWithKeyFile()
        {
            string strGPKIFullPath = m_strBaseGPKIPath;
            string strDriveName = "C:\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strGPKIFullPath = Path.Combine(strDriveName, strGPKIFullPath);
                strGPKIFullPath = strGPKIFullPath.Replace("/", "\\");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string strFullHomePath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                strGPKIFullPath = Path.Combine(strFullHomePath, strGPKIFullPath);
                strGPKIFullPath = strGPKIFullPath.Replace("\\", "/");
            }

            return LoadCertKeyFileList(strGPKIFullPath);

        }

        /**
        *@breif 이동식 디스크의 GPKI 파일들을 로드한다.
        *@param strDriveName 이동식 디스크의 드라이브명
        *@return true 성공
        */
        public bool LoadMoveDiskGPKICertWithKeyFile(string strDriveName)
        {
            string strGPKIFullPath = m_strBaseGPKIPath;
            strGPKIFullPath = Path.Combine(strDriveName, strGPKIFullPath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strGPKIFullPath = strGPKIFullPath.Replace("/", "\\");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                strGPKIFullPath = strGPKIFullPath.Replace("\\", "/");
            }

            return LoadCertKeyFileList(strGPKIFullPath);
        }

        /**
        *@breif GPKI 파일이 cer,key 쌍으로 존재하는지 여부를 확인한다.
        *@param strGPKIFileName GPKI 파일명
        *@return true 성공
        */
        public bool GetGPKIFileExam(string strGPKIFileFullName)
        {
            bool bCerFind = false;
            bool bKeyFind = false;
            string strFileName = Path.GetFileNameWithoutExtension(strGPKIFileFullName);
            string strFilePath = Path.GetDirectoryName(strGPKIFileFullName);

            string strFileCerName = strFileName + ".cer";
            strFileCerName = Path.Combine(strFilePath, strFileCerName);
            string strFileKeyName = strFileName + ".key";
            strFileKeyName = Path.Combine(strFilePath, strFileKeyName);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                strFileCerName = strFileCerName.Replace("/", "\\");
                strFileKeyName = strFileKeyName.Replace("/", "\\");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                strFileCerName = strFileCerName.Replace("\\", "/");
                strFileKeyName = strFileKeyName.Replace("\\", "/");
            }
            bCerFind = File.Exists(strFileCerName);
            bKeyFind = File.Exists(strFileKeyName);
            return (bCerFind & bKeyFind);
        }
        public List<GPKIFileInfo> GetGpkiFileList()
        {
            return listGpkiFile;
        }

        public byte[] StructToBytes(object obj)
        { 
            //구조체 사이즈 
            int iSize = Marshal.SizeOf(obj); 
            
            //사이즈 만큼 메모리 할당 받기 
            byte[] arr = new byte[iSize]; 
            IntPtr ptr = Marshal.AllocHGlobal(iSize); 

            //구조체 주소값 가져오기 
            Marshal.StructureToPtr(obj, ptr, false); 
            //메모리 복사 
            Marshal.Copy(ptr, arr, 0, iSize); 
            Marshal.FreeHGlobal(ptr); 
            return arr; 
        }


        public bool IsValiedPW(GPKIFileInfo gpkiFile, string strUserinputPW)
        {

            if (GPKIBinReadPriKey(gpkiFile, strUserinputPW) == false)            
            {
                CLog.Here().Error($"PW Identify Fail : {gpkiFile.m_strFileName}");
                return false;
            }

            return true;
        }

        public bool IsValiedGPKIFile(GPKIFileInfo gpkiFile, string strUserinputPW, ref string strReason)
        {

            // Password 길이확인
            if (strUserinputPW.Length < 1)
            {
                strReason = "Password 입력 필요함.";
                return false;
            }

            // 만료된 인증서 : 유효한 인증서 받고 나서 주석 풀어 적용
            /*if (gpkiFile.m_nRemainDay <= 0)
            {
                strReason = "만료된 인증서";
                return false;
            }*/

            // password가 틀림
            if (IsValiedPW(gpkiFile, strUserinputPW) == false)
            {
                strReason = "잘못된 password";
                return false;
            }

            // ...

            return true;
        }

        public unsafe bool GetGpkiSignedData(GPKIFileInfo gpkiFile, ref byte[] pDataRandom, ref byte[] pSignedData)
        {

            if (m_pClientCtx == IntPtr.Zero)
            {
                CLog.Here().Error($"GetGpkiSignedData - m_pClientCtx is null!");
                return false;
            }

            IntPtr ptrRandom = IntPtr.Zero;
            IntPtr ptrCert = IntPtr.Zero;
            IntPtr ptrPrikey = IntPtr.Zero;
            IntPtr ptrSignedData = IntPtr.Zero;

            BINSTR bsRandom = new BINSTR();
            BINSTR bsCert = new BINSTR();
            BINSTR bsPriKey = new BINSTR();
            // BINSTR bsSigned = new BINSTR();
            
            // byte[] byteRandom = null;
            byte[] byteCert = null;
            // byte[] bytePrikey = null;
            byte[] byteSigned = null;

            int nRet = GPKI_STORAGE_ReadCert(gpkiFile.m_strFileName, ref byteCert);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GPKI_STORAGE_ReadCert (read Fail) - error:{nRet} - msg : {strErrMsg}, File : {gpkiFile.m_strFileName} ");
                return false;
            }

            // Cert BINSTR 생성
            bsCert.nLen = byteCert.Length;
            bsCert.pData = byteCert;            

            if (BINSTR2inPtr(bsCert, ref ptrCert) == false)
            {
                CLog.Here().Error($"BiNSTR2inPtr - Cert - error!!");
                return false;
            }

            // PriKey  BINSTR 생성
            bsPriKey.nLen = gpkiFile.m_pKeyData.Length;
            bsPriKey.pData = gpkiFile.m_pKeyData;

            if (BINSTR2inPtr(bsPriKey, ref ptrPrikey) == false)
            {
                CLog.Here().Error($"BiNSTR2inPtr - PriKey - error!!");
                return false;
            }

            // Random  BINSTR 생성
            bsRandom.nLen = pDataRandom.Length;
            bsRandom.pData = pDataRandom;

            if (BINSTR2inPtr(bsRandom, ref ptrRandom) == false)
            {
                CLog.Here().Error($"BiNSTR2inPtr - Random - error!!");
                return false;
            }

            nRet = GPKI_CMS_MakeSignedData(ptrCert, ptrPrikey, ptrRandom, null, ref byteSigned);
            if (nRet != (int)eGpkiError.GPKI_OK)
            {
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GPKI_CMS_MakeSignedData - Fail(error:{nRet}) - Message :{strErrMsg}, File : {gpkiFile.m_strFileName}");
                return false;
            }

            if (byteSigned ==null || byteSigned.Length < 1)
            {
                nRet = (int)eGpkiError.ERR_UNDEF_APP_ERROR;
                string strErrMsg = GetGpkiError((eGpkiError)nRet);
                CLog.Here().Error($"GPKI_CMS_MakeSignedData - Fail(error:{nRet}) - Message :{strErrMsg}, File : {gpkiFile.m_strFileName}");
                return false;
            }

            pSignedData = new byte[byteSigned.Length];
            byteSigned.CopyTo(pSignedData, 0);

            string strSignedDataHex = Convert.ToBase64String(pSignedData);
            CLog.Here().Error($"GetGpkiSignedData - Signed Data - Base64 : {strSignedDataHex}");

            return true;
        }

    }
}
