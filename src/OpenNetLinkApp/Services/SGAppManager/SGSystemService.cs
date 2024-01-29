using HsNetWorkSG;
using OpenNetLinkApp.Models.SGSystem;
using System;
using System.IO;
using System.Text;
using AgLogManager;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGSystemService
    {
        SGSystemData SystemInfo { get { return GetSGSystemService(); } }
        public SGSystemData GetSGSystemService();
        /* To Manage Corporate Identity State */
        /// <summary>
        /// CIPath is CI Image path.
        /// </summary>
        string CIPath { get; }
        /// <summary>
        /// CI Event Delegate, Modified by User or System.
        /// </summary>
        event Action OnChangeCI;
        /// <summary>
        /// To Set CI Image File Path
        /// </summary>
        /// <param name="ciPath"> 
        /// </param>
        /// <returns>void</returns>
        void SetCIPath(string ciPath);
    }
    internal class SGSystemService : ISGSystemService
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SGSystemService>();
        public SGSystemService()
        {
        }

        public SGSystemData GetSGSystemService() => SystemInfo;

        private static SGSystemData _SystemInfo { get; set; } = null;
        public static SGSystemData SystemInfo
        {
            get
            {
                if (_SystemInfo == null) _SystemInfo = new SGSystemData();
                return _SystemInfo;
            }
        }

        public string CIPath { get; private set; } = String.Empty;
        public event Action OnChangeCI;
        private void NotifyStateChangedCI() => OnChangeCI?.Invoke();
        public void SetCIPath(string ciPath)
        {
            CIPath = ciPath;
            NotifyStateChangedCI();
        }

        public string[] GetStartArg() => SystemInfo.StartArg;

        public static bool IsStartedByNAC()
        {
            if (SystemInfo.StartArg != null && SystemInfo.StartArg.Length > 0)
                return SystemInfo.StartArg[0].ToString().ToUpper() == "NAC";
            else
                return false;
        }

        public static bool IsStartedBySERVICE()
        {
            if (SystemInfo.StartArg != null && SystemInfo.StartArg.Length > 0)
                return SystemInfo.StartArg[0].ToString().ToUpper() == "SERVICE";
            else
                return false;
        }

        public static void SetNacFileValue(string getNacFilePath = "")
        {
            string NacFilePath = getNacFilePath;
            try
            {
                //Default Path
                if (string.IsNullOrEmpty(NacFilePath.Trim()))
                    NacFilePath = Path.Combine(Environment.CurrentDirectory, "NAC");

                if (File.Exists(NacFilePath))
                {
                    string strNacFileValue = File.ReadAllText(NacFilePath);
                    SGSystemService.SystemInfo.NacFileValue = strNacFileValue;    //실행 시점에 NAC 파일 Value 저장
                    CLog.Here().Information($"NacFileValue : {SystemInfo.NacFileValue}");
                }
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"SetNacFileValue Exception - {ex.ToString()}");
            }
        }

        public static string GetGenianNACUserID(string nacEncryptKey)
        {
            try
            {
                if (IsStartedByNAC() == false)
                    return string.Empty;

                if (!string.IsNullOrEmpty(SystemInfo.NacFileValue.Trim()))
                {
                    CLog.Here().Information($"GetGenianNACUserID NAC Value: {SystemInfo.NacFileValue}");
                    //test
                    //strNacEncData = "kf+MFAB+M5Poc54osw5R6izVKLaGMqneqKMvg7N/Qla1M7DY0A3llIle1HwpV3YoTLPGxh3QHjY2mm4WoYHNxg=="; //yhkim41
                    //strNacEncData = "kf+MFAB+M5Poc54osw5R6qbFcbsKBN4KPIUR+fFZNE+rIR8KzHLKCXL9wlOPtBHM/yTDc84P53BKFNWYzXEY2Q=="; //yhkim40
                    //strNacEncData = "kf+MFAB+M5Poc54osw5R6oL7R7mw5MRfIICRC9ZJWfWH4qaYi9RHGReEiRvIMiUic65TVy4huouqqtt4jstt9Ym31lG2V5b3ZW0XTaQ0RW8="; //errId

                    string strNacDec = string.Empty;
                    if (string.IsNullOrEmpty(nacEncryptKey.Trim()))
                    {
                        //NAC 계정 암호화 사용하지 않으면 그대로 계정 사용
                        strNacDec = SystemInfo.NacFileValue;
                    }
                    else
                    {
                        //Base64 디코딩
                        byte[] arrNacEncData = Convert.FromBase64String(SystemInfo.NacFileValue);

                        //AES128로 복호화 필요함 (OP에 NAC 키 있음)
                        byte[] decKey = new byte[16];
                        byte[] arrNACEnc = Encoding.UTF8.GetBytes(nacEncryptKey);
                        Array.Copy(arrNACEnc, 0, decKey, 0, arrNACEnc.Length);

                        byte[] arrNACDec = null;
                        SGCrypto.AESDecrypt128(arrNacEncData, ref decKey, System.Security.Cryptography.PaddingMode.None, ref arrNACDec);
                        strNacDec = Encoding.UTF8.GetString(arrNACDec);

                        //format. -nac "-authid:test" -IP:192.168.1.168 -MAC:00:24:1D:88:1D:B4
                        strNacDec = strNacDec.Replace("\"", "");
                        string findKey = "-authid:";
                        foreach (string arg in strNacDec.Split(" "))
                        {
                            if (arg.Contains(findKey))
                            {
                                return arg.Replace(findKey, "");
                            }
                        }
                    }
                }
                else
                {
                    CLog.Here().Error($"GetGenianNACUserID NAC Value is Empty : {SystemInfo.NacFileValue}");
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                CLog.Here().Error($"GetGenianNACUserID Exception : " + ex.ToString());
                return string.Empty;
            }
        }
    }
}