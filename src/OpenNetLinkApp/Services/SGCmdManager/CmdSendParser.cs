using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using HsNetWorkSG;

namespace OpenNetLinkApp.Services
{
    public class CmdSendParser
    {
        public static Dictionary<int, CancellationTokenSource> m_DicCt = new Dictionary<int, CancellationTokenSource>();
        public CmdSendParser()
        {

        }
        ~CmdSendParser()
        {

        }
        /*
        public static void Noti(object obj, SGEventArgs e)
        {
            CmdRecvParser cmdRecvParser = new CmdRecvParser();  // 서비스로 등록할 것이기 때문에 추후 수정 필요
            cmdRecvParser.RecvSGCmd(e);
        }
        */
        public bool RequestSessionKey(string strCmd, string strAppID, string strRSAPublicKey)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);
            args.strCmd = strCmd;

            //UTF8Encoding utf8Enc = new UTF8Encoding();
            //byte[] tmpByte = utf8Enc.GetBytes(strAppID);
            //args.MsgRecode["APPID"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            args.MsgRecode["APPID"] = strAppID;
            args.MsgRecode["ENCOP"] = "1";
            args.MsgRecode["RSAPUBLICKEY"] = strRSAPublicKey;
            args.MsgRecode["KEYTYPE"] = "1";

            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return true;

        }

        public int RequestSessionCount(string strCmd, string strAppID, string strUserID, string strPasswd, string strOTPNo, string PasswordType,int iLoginType=0)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);

            SGCrypto crypt = new SGCrypto();
            //sgCrypt.SetSessionKey();
            string strEncID = "";
            strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
            string strEncPW = "";
            strEncPW = crypt.Aes256TagValueEncrypt(strPasswd, out strEncPW);
            string strEncOTPNo = "";
            strEncOTPNo = crypt.Aes256TagValueEncrypt(strPasswd, out strEncOTPNo);

            args.strCmd = strCmd;

            //UTF8Encoding utf8Enc = new UTF8Encoding();
            //byte[] tmpByte = utf8Enc.GetBytes(strAppID);
            //args.MsgRecode["APPID"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            args.MsgRecode["APPID"] = strAppID;
            args.MsgRecode["CLIENTID"] = strEncID;

            if (iLoginType == 0)
            {
                args.MsgRecode["PASSWORD"] = strEncPW;
            }
            else
            {
                string strEncPWType = "";
                strEncPWType = string.Format("{0}{1}", iLoginType, strEncPW);
                args.MsgRecode["PASSWORD"] = strEncPWType;
            }

            //if (optFlag == true)
            //   args.MsgRecode["OTP"] = strEncOTPNo;

            args.MsgRecode["PASSWORDTYPE"] = PasswordType;


            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return 0;


        }
        public int RequestLogin(string strCmd, string strAppID, string strUserID, string strPasswd, string strOTPNo, int iLoginType = 0, int iSSoUse=0, string strAgentHash="")
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);


            SGCrypto crypt = new SGCrypto();
            //sgCrypt.SetSessionKey();
            string strEncID = "";
             strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
            string strEncPW = "";
            strEncPW = crypt.Aes256TagValueEncrypt(strPasswd, out strEncPW);
            string strEncOTPNo = "";
            strEncOTPNo = crypt.Aes256TagValueEncrypt(strPasswd, out strEncOTPNo);
            
            args.strCmd = strCmd;

            /*
            UTF8Encoding utf8Enc = new UTF8Encoding();
            byte[] tmpByte = utf8Enc.GetBytes(strAppID);
            args.MsgRecode["APPID"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            args.MsgRecode["CLIENTID"] = strEncID;
            if(iLoginType==0)
            {
                args.MsgRecode["PASSWORD"] = strEncPW;
            }
            else
            {
                string strEncPWType = "";
                strEncPWType = string.Format("{0}{1}", iLoginType, strEncPW);
                args.MsgRecode["PASSWORD"] = strEncPWType;
            }

            tmpByte = utf8Enc.GetBytes("6");
            args.MsgRecode["LINETYPE"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            tmpByte = utf8Enc.GetBytes("Ver 2.0");
            args.MsgRecode["VERSION"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            tmpByte = utf8Enc.GetBytes("NetLInk 2.01");
            args.MsgRecode["CLIVERSION"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            tmpByte = utf8Enc.GetBytes("");
            args.MsgRecode["CLIENTIP"] = "";
            tmpByte = utf8Enc.GetBytes("");
            args.MsgRecode["CLIENTMAC"] = "";
            tmpByte = utf8Enc.GetBytes("UTF-8");
            args.MsgRecode["CHARSET"] = "";
            

            args.MsgRecode["APPID"] = strAppID;
            if (iLoginType == 0)
            {
                args.MsgRecode["PASSWORD"] = strEncPW;
            }
            else
            {
                string strEncPWType = "";
                strEncPWType = string.Format("{0}{1}", iLoginType, strEncPW);
                args.MsgRecode["PASSWORD"] = strEncPWType;
            }

            //if (optFlag == true)
            //   args.MsgRecode["OTP"] = strEncOTPNo;

 
            if (iSSoUse == 0)
            {
                tmpByte = utf8Enc.GetBytes("0");
                args.MsgRecode["PASSWORDTYPE"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length); 
            }
                
            else
            {
                tmpByte = utf8Enc.GetBytes("9");
                args.MsgRecode["PASSWORDTYPE"] = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
            }

            args.MsgRecode["AGENTHASH"] = "";
            */

            args.MsgRecode["LINETYPE"] = "6";
            args.MsgRecode["VERSION"] = "2.0";
            args.MsgRecode["CLIVERSION"] = "NetLink 2.01";
            args.MsgRecode["CLIENTIP"] = "";
            args.MsgRecode["CLIENTMAC"] = "";
            args.MsgRecode["CHARSET"] = "UTF-8";
            args.MsgRecode["AGENTHASH"] = "";


            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return 0;
        }

        public bool RequestCmd(string strCmd, Dictionary<string,string> dic)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);
            UTF8Encoding utf8Enc = new UTF8Encoding();
            SGCrypto crypt = new SGCrypto();
            for (int i=0;i<dic.Count;i++)
            {
                if(dic.Keys.ToList()[i] == "CLIENTID")
                {
                    //sgCrypt.SetSessionKey();
                    string strUserID = dic["CLIENTID"];
                    string strEncID = "";
                    strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
                    args.MsgRecode["CLIENTID"] = strEncID;
                }
                else
                {
                    string strKey = dic.Keys.ToList()[i];
                    byte[] tmpByte = utf8Enc.GetBytes(dic[strKey]);
                    string strValue = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
                    args.MsgRecode[strKey] = strValue;
                }
            }

            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return true;
        }

        public bool RequestAPTandVirusConfirm(string strCmd, Dictionary<string, string> dic)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);
            UTF8Encoding utf8Enc = new UTF8Encoding();
            SGCrypto crypt = new SGCrypto();
            for (int i = 0; i < dic.Count; i++)
            {
                if (dic.Keys.ToList()[i] == "CLIENTID")
                {
                    //sgCrypt.SetSessionKey();
                    string strUserID = dic["CLIENTID"];
                    string strEncID = "";
                    strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
                    args.MsgRecode["CLIENTID"] = strEncID;
                }
                else
                {
                    string strKey = dic.Keys.ToList()[i];
                    byte[] tmpByte = utf8Enc.GetBytes(dic[strKey]);
                    string strValue = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
                    args.MsgRecode[strKey] = strValue;
                }
            }

            return true;
        }

        public bool RequestChangePW(string strCmd, Dictionary<string, string> dic)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);

            UTF8Encoding utf8Enc = new UTF8Encoding();
            SGCrypto crypt = new SGCrypto();
            for (int i = 0; i < dic.Count; i++)
            {
                if (dic.Keys.ToList()[i] == "CLIENTID")
                {
                    //sgCrypt.SetSessionKey();
                    string strUserID = dic["CLIENTID"];
                    string strEncID = "";
                    strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
                    args.MsgRecode["CLIENTID"] = strEncID;
                }
                else if (dic.Keys.ToList()[i] == "OLDPASSWORD")
                {
                    //sgCrypt.SetSessionKey();
                    string strOldPW = dic["OLDPASSWORD"];
                    string strEncOldPW = "";
                    strEncOldPW = crypt.Aes256TagValueEncrypt(strOldPW, out strEncOldPW);
                    args.MsgRecode["OLDPASSWORD"] = strEncOldPW;
                }
                else if (dic.Keys.ToList()[i] == "NEWPASSWORD")
                {
                    //sgCrypt.SetSessionKey();
                    string strNewPW = dic["NEWPASSWORD"];
                    string strEncNewPW = "";
                    strEncNewPW = crypt.Aes256TagValueEncrypt(strNewPW, out strEncNewPW);
                    args.MsgRecode["NEWPASSWORD"] = strEncNewPW;
                }
                else
                {
                    string strKey = dic.Keys.ToList()[i];
                    byte[] tmpByte = utf8Enc.GetBytes(dic[strKey]);
                    string strValue = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
                    args.MsgRecode[strKey] = strValue;
                }
            }

            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return true;
        }

        public bool RequestClientUnlock(string strCmd,Dictionary<string, string> dic)
        {
            SGEventArgs args = new SGEventArgs();
            SGCmdDef sgCmdDef = new SGCmdDef();
            strCmd = sgCmdDef.SendCmdtoString(strCmd);
            UTF8Encoding utf8Enc = new UTF8Encoding();

            SGCrypto crypt = new SGCrypto();
            for (int i = 0; i < dic.Count; i++)
            {
                if (dic.Keys.ToList()[i] == "CLIENTID")
                {
                    //sgCrypt.SetSessionKey();
                    string strUserID = dic["CLIENTID"];
                    string strEncID = "";
                    strEncID = crypt.Aes256TagValueEncrypt(strUserID, out strEncID);
                    args.MsgRecode["CLIENTID"] = strEncID;
                }
                else if (dic.Keys.ToList()[i] == "PASSWORD")
                {
                    //sgCrypt.SetSessionKey();
                    string strPW = dic["PASSWORD"];
                    string strEncPW = "";
                    strEncPW = crypt.Aes256TagValueEncrypt(strPW, out strEncPW);
                    args.MsgRecode["PASSWORD"] = strEncPW;
                }
                else
                {
                    string strKey = dic.Keys.ToList()[i];
                    byte[] tmpByte = utf8Enc.GetBytes(dic[strKey]);
                    string strValue = Convert.ToBase64String(tmpByte, 0, tmpByte.Length);
                    args.MsgRecode[strKey] = strValue;
                }
            }

            CancellationTokenSource ct = new CancellationTokenSource();
            int CmdID = sgCmdDef.SendCmdtoID(strCmd);
            m_DicCt[CmdID] = ct;

            while (true)
            {
                Thread.Sleep(200);
                if (m_DicCt[CmdID].IsCancellationRequested)
                {
                    break;
                }
            }

            return true;
        }

    }
}
