using Serilog;
using AgLogManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenNetLinkApp.LoginMethod
{
    class LoginTemplate01
    {
        class RequestParameter
        {
            public string empNo { get; set; }

            public string encPwd { get; set; }

            public RequestParameter(string empNo, string encPwd)
            {
                this.empNo = empNo;
                this.encPwd = encPwd;
            }

            public string GetJsonString()
            {
                var opt = new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(this, opt);
                return Encoding.UTF8.GetString(json);
            }
        }
        class ResponseParameter
        {
            public bool result { get; set; }

            public string errMsg { get; set; }
        }

        public (bool, string) Login(string id, string pw, string url)
        {
            bool result = false;
            Log.Logger.Here().Information($"Url : {url}");
            //암호화
            string key = "SnuhGwEnDSecValK";
            string iv = "SnuhGwVecValKeyU";

            byte[] bEncPw = HsNetWorkSG.SGCrypto.AESEncrypt(pw, key, iv);
            string strEncPw = Convert.ToBase64String(bEncPw);

            string method = "GET";
            string contentType = "application/json; utf-8";
            
            //전송
            RequestParameter requestParameter = new RequestParameter(id, strEncPw);
            Log.Logger.Here().Information($"requestParameter : {requestParameter.GetJsonString()}");
            string strResult = WebRequestCommon(method, contentType, url, 10, requestParameter.GetJsonString());
            string msg = "";
            Log.Logger.Here().Information($"responseParameter : {strResult}");
            if (!String.IsNullOrEmpty(strResult))
            {
                ResponseParameter response = JsonSerializer.Deserialize<ResponseParameter>(strResult);
                result = response.result;

                switch(response.errMsg)
                {
                    case "decryptErr":
                        msg = "복호화 도중 에러가 발생했습니다.";
                        break;
                    case "NotFoundUser":
                        msg = "사용자가 존재하지 않습니다.";
                        break;
                    case "AccountLocked":
                        msg = "계정이 잠겨 있습니다.";
                        break;
                    case "RetiredUser":
                        msg = "사용자 퇴직 상태 입니다.";
                        break;
                    case "LoginFailed":
                        msg = "패스워드 불일치 입니다.";
                        break;
                    case "":
                        msg = "로그인 성공";
                        break;
                    default:
                        msg = "";
                        break;
                }
            }

            return (result, msg);

        }



        public static string WebRequestCommon(string method , string contentType, string url, int ReqTimeOut, string json)
        {

            WebRequest wreq = WebRequest.Create(url);
            wreq.Method = method;
            wreq.Timeout = ReqTimeOut;
            wreq.ContentType = contentType;

            using (var streamWriter = new StreamWriter(wreq.GetRequestStream())) //전송
            {
                streamWriter.Write(json);
            }
            var response = wreq.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string apiResult = streamReader.ReadToEnd();
                return apiResult;
            }
        }
    }
}
