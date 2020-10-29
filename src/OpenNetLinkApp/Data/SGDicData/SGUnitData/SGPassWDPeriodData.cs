using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Pages.Transfer;
using System.Globalization;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGPassWDPeriodData : SGData
    {
        public SGPassWDPeriodData()
        {
        }
        ~SGPassWDPeriodData()
        {

        }
        public string LoaclPassWDDelaySaveString(string strDBPassWDStr,string strUserSeq, string strDateTime)
        {
            string strRet = "";
            if( (strDBPassWDStr==null) || (strDBPassWDStr.Equals("")) || (!strDBPassWDStr.Contains("\u0001")) )
            {
                strRet = strUserSeq + "\u0001" + strDateTime;
                return strRet;
            }

            if(strDBPassWDStr.Contains(strUserSeq))
            {
                string[] strPassWDArr = strDBPassWDStr.Split("\u0003");
                if(strPassWDArr.Length<=0)
                {
                    strRet = strUserSeq + "\u0001" + strDateTime;
                    return strRet;
                }

                bool bFind = false;
                int nFindIdx = 0;
                for(int i=0;i<strPassWDArr.Length;i++)
                {
                    if (strPassWDArr[i].Contains(strUserSeq))
                    {
                        bFind = true;
                        nFindIdx = i;
                        break;
                    }
                }

                if(bFind)
                {
                    strPassWDArr[nFindIdx] = strUserSeq + "\u0001" + strDateTime;
                    for(int j=0;j<strPassWDArr.Length;j++)
                    {
                        strRet = strRet + strPassWDArr[j] + "\u0003";
                    }
                    strRet = strRet.Substring(0, strRet.Length - 1);
                }
                else
                {
                    for (int j = 0; j < strPassWDArr.Length; j++)
                    {
                        strRet = strPassWDArr[j] + "\u0003";
                    }
                    strRet = strRet + strUserSeq + "\u0001" + strDateTime;
                }
                return strRet;
            }
            else
            {
                string[] strPassWDArr = strDBPassWDStr.Split("\u0003");
                if (strPassWDArr.Length <= 0)
                {
                    strRet = strUserSeq + "\u0001" + strDateTime;
                    return strRet;
                }
                for (int j = 0; j < strPassWDArr.Length; j++)
                {
                    strRet = strRet + strPassWDArr[j] + "\u0003";
                }
                strRet = strRet + strUserSeq + "\u0001" + strDateTime;
                return strRet;
            }
        }

        public string LoaclPassWDDelayLoadString(string strDBPassWDStr, string strUserSeq)
        {
            string strDateTime = "";
            if ((strDBPassWDStr == null) || (strDBPassWDStr.Equals("")) || (!strDBPassWDStr.Contains("\u0001")))
                return strDateTime;

            if (!strDBPassWDStr.Contains(strUserSeq))
                return strDateTime;

            string[] strPassWDArr = strDBPassWDStr.Split("\u0003");
            if (strPassWDArr.Length <= 0)
                return strDateTime;

            for(int i=0;i<strPassWDArr.Length;i++)
            {
                string[] strData = strPassWDArr[i].Split("\u0001");
                if (strData.Length <= 0)
                    continue;

                if (strData[0].Equals(strUserSeq))
                    strDateTime = strData[1];
            }
            return strDateTime;
        }
        public DateTime GetLocalDBPassWDDelayTime(string strDBPassWDStr,string strUserSeq)
        {
            string strRetTime = "";
            DateTime retTime = DateTime.Now;
            strRetTime = LoaclPassWDDelayLoadString(strDBPassWDStr, strUserSeq);
            if ((strRetTime == null) || (strRetTime.Equals("")))
                return retTime;

            retTime = Convert.ToDateTime(strRetTime);
            return retTime;
        }
    }
}
