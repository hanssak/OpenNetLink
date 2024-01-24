using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace OpenNetLinkService.Site
{
    class KoReg
    {
        public static int ProcessPerform(HttpListenerRequest request)
        {
            int result = 0;
            try
            {
                if (request.HttpMethod == "GET")
                {
                    NameValueCollection parameters = request.QueryString;
                    if (parameters.HasKeys() && parameters.GetKey(0) == "UserID")
                    {
                        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                        string currentExePath = Path.Combine(currentDir, "OpenNetLinkApp.exe SERVICE");
                        string currentMessagePath = Path.Combine(currentDir, "KoReg.txt");
                        string message = $@"UserID={parameters["UserID"]}
Page={parameters["Page"]}";
                        //USERID, PAGE 파일에 쓰기 
                        using (FileStream fs = File.Create(currentMessagePath))
                        {
                            byte[] hashInfo = new UTF8Encoding(true).GetBytes(message);
                            fs.Write(hashInfo, 0, hashInfo.Length);
                        }

                        ApplicationLauncher.CreateProcessInConsoleSession(currentExePath, false);

                    }
                    else
                    {
                        result = -1;
                    }
                }
                else
                {
                    result = -1;
                }
            }
            catch(Exception ex)
            {
                result = -1;   
            }

            return result;
        }

    }
}
