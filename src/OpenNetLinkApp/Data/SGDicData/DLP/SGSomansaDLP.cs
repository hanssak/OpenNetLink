using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

namespace OpenNetLinkApp.Data.SGDicData.DLP
{
    class SGSomansaDLP
    {
        public static object COMCreateObject(string sProgID)
        {
            Type oType = Type.GetTypeFromProgID(sProgID);
            if (oType != null)
            {
                return Activator.CreateInstance(oType);
            }
            return null;
        }
        public static (int, string) DlpCheck(string filePath)
        {
            if (!Directory.Exists("TempDLP"))
            {
                Directory.CreateDirectory("TempDLP");
            }

            string tempPath = Path.Combine("TempDLP", Path.GetFileName(filePath));
            File.Copy(filePath, tempPath, true);

            (int, string) result = (0, "");
            try
            {
                Type objClassType = Type.GetTypeFromProgID("PIScanLibCom.PIScanLib.1");

                if (objClassType != null)
                {
                    object obj = Activator.CreateInstance(objClassType);

                    //정책 타입 설정
                    obj.GetType().InvokeMember("SetPolicyType", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { 1 });

                    //개인정보 검사 수행
                    object value = obj.GetType().InvokeMember("ScanPrivacy", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { filePath });

                    //개인정보 검사 성공 여부 조회
                    object errorCode = obj.GetType().InvokeMember("GetErrorCode", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);

                    switch (errorCode.ToString())
                    {
                        case "0":
                            {
                                //개인 정보 검사 결과 코드 조회
                                object resultCode = obj.GetType().InvokeMember("GetResultCode", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);

                                switch (resultCode.ToString())
                                {
                                    case "1":
                                        {
                                            //검출
                                            result.Item1 = 1;
                                            result.Item2 = GetResultParsing(value.ToString());
                                        }
                                        break;
                                    case "2":
                                        {
                                            //미검출
                                            result.Item1 = 2;
                                            result.Item2 = "";
                                        }
                                        break;
                                    case "3":
                                        {
                                            //분석 불가
                                            result.Item1 = 2;
                                            result.Item2 = "";
                                        }
                                        break;

                                }
                            }
                            break;
                        case "50010":
                        case "50015":
                            // Privacy-i StandAlone 버전 재설치 필요
                            result.Item1 = 3;
                            result.Item2 = "Privacy-i StandAlone 버전 재설치 필요";
                            break;
                        case "50020":
                            // 최신 모듈로 업데이트 필요
                            result.Item1 = 3;
                            result.Item2 = "최신 모듈로 업데이트 필요";
                            break;
                        case "50025":
                            // 라이선스 발급 필요
                            result.Item1 = 3;
                            result.Item2 = "라이선스 발급 필요";
                            break;
                        default:
                            // 올바르지 않은 파일
                            result.Item1 = 3;
                            result.Item2 = "올바르지 않은 파일";
                            break;
                    }
                    // Clean up
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                result.Item1 = 3;
                result.Item2 = ex.Message;
            }
            finally
            {
                if (Directory.Exists("TempDLP"))
                    Directory.Delete("TempDLP", true);
            }

            return result;
        }

        public static string GetResultParsing(string result)
        {
            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(new StringReader(result));

            Dictionary<string, int> dlpResult = new Dictionary<string, int>();
            XmlNodeList nodes = xdoc.SelectNodes("PatternList");
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (!dlpResult.Keys.Contains(childNode.Attributes["PatternName"].Value))
                    {
                        dlpResult.Add(childNode.Attributes["PatternName"].Value, Convert.ToInt32(childNode.Attributes["Repetition"].Value));
                    }
                    else
                    {
                        dlpResult[childNode.Attributes["PatternName"].Value] += Convert.ToInt32(childNode.Attributes["Repetition"].Value);
                    }
                }
            }

            string finalResult = "";

            if (dlpResult.Count > 0)
            {
                foreach (string key in dlpResult.Keys)
                {
                    finalResult += $"{key} : {dlpResult[key]}, ";
                }

                finalResult = finalResult.Substring(0, finalResult.Length - 2);
            }

            return finalResult;
        }
    }
}
